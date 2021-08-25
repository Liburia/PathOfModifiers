using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Items;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using PathOfModifiers.Affixes.Items;

namespace PathOfModifiers.Tiles
{
    public class ModifierForge : ModTile
    {
        public static ModifierForgeTE activeForge;

        static int activeAnimationFirstFrame;
        static int activeAnimationFrameCount;
        static int activeAnimationFullFrameCount;

        public override void SetStaticDefaults()
        {
            Main.tileSpelunker[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<ModifierForgeTE>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Modifier Forge");
            AddMapEntry(new Color(192, 120, 0), name);
            AnimationFrameHeight = 38;

            activeAnimationFirstFrame = 2;
            activeAnimationFrameCount = 4;
            activeAnimationFullFrameCount = activeAnimationFrameCount * 2 - 2;

        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 40)
            {
                frameCounter = 0;
                frame++;
                if (frame >= activeAnimationFullFrameCount)
                {
                    frame = 0;
                }
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            //Hardcoded frame coordinate values because using TileObjectData is cancer.
            if (PoMUtil.TryGetTileEntity(i, j, 18, 18, out TileEntity te))
            {
                var forge = (ModifierForgeTE)te;

                if (forge.modifierItem.ModItem is ModifierFragment)
                {
                    if (forge.modifiedItem.IsAir)
                    {
                        //Filled but not active
                        frameYOffset = AnimationFrameHeight;
                    }
                    else
                    {
                        int frame = Main.tileFrame[type];
                        if (frame >= activeAnimationFrameCount)
                        {
                            frame = activeAnimationFrameCount - (frame - activeAnimationFrameCount) - 2;
                        }

                        //Filled and active
                        frameYOffset = AnimationFrameHeight * (activeAnimationFirstFrame + frame);
                    }
                }
                else
                {
                    //Not filled
                    frameYOffset = 0;
                }
            }
        }

        public override bool HasSmartInteract()
        {
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode != 1)
                Item.NewItem(new Vector2(i * 16, j * 16), ModContent.ItemType<Items.ModifierForge>());

            ModContent.GetInstance<ModifierForgeTE>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Main.mouseRightRelease = false;
            if (player.sign >= 0)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }

            //TODO: (Or not to do) Hardcoded frame coordinate values because using TileObjectData is cancer.
            if (PoMUtil.TryGetTileEntity(i, j, 18, 18, out TileEntity te))
            {
                ModifierForgeTE clickedForge = (ModifierForgeTE)te;
                if (UI.States.ModifierForge.IsOpen && activeForge == clickedForge)
                {
                    UI.States.ModifierForge.Close();
                }
                else
                {
                    UI.States.ModifierForge.Open(clickedForge);
                }

                return true;
            }

            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconText = "Modifier Forge";
            if (player.cursorItemIconText == "Modifier Forge")
            {
                player.cursorItemIconID = ModContent.ItemType<Items.ModifierForge>();
                player.cursorItemIconText = "";
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //Hardcoded dimension values because using TileObjectData is cancer.
            var forgePos = new Point16(i - 2, j - 1);
            if (TileEntity.ByPosition.TryGetValue(forgePos, out TileEntity te))
            {
                var forge = (ModifierForgeTE)te;

                if (!forge.modifiedItem.IsAir)
                {
                    var screenDrawOffset = PoMUtil.DrawToScreenOffset();

                    var itemTexture = Terraria.GameContent.TextureAssets.Item[forge.modifiedItem.type].Value;
                    var itemWidth = 24;
                    var itemHeight = (int)(itemWidth * (itemTexture.Height / (float)itemTexture.Width));
                    var itemScale = itemWidth / (float)itemTexture.Width;
                    var itemDrawOffset = new Point16((48 - itemWidth) / 2, 5 - itemHeight);

                    var itemPosition = new Vector2(
                        forgePos.X * 16 - Main.screenPosition.X + screenDrawOffset.X + itemDrawOffset.X,
                        forgePos.Y * 16 - Main.screenPosition.Y + screenDrawOffset.Y + itemDrawOffset.Y);
                    spriteBatch.Draw(itemTexture, itemPosition, null, Color.White, 0, Vector2.Zero, itemScale, SpriteEffects.None, 0);
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool emitLight = false;
            //Hardcoded dimension values because using TileObjectData is cancer.
            var forgePos = new Point16(i - 1, j - 1);
            if (TileEntity.ByPosition.TryGetValue(forgePos, out TileEntity te))
            {
                var forge = (ModifierForgeTE)te;
                if (!forge.modifiedItem.IsAir && forge.modifierItem.ModItem is ModifierFragment)
                {
                    emitLight = true;
                }
            }

            if (emitLight)
            {
                r = 0.506f;
                g = 0f;
                b = 0.78f;
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0;
            }
        }
    }

    public class ModifierForgeTE : PoMTileEntity
    {
        public enum ForgeAction
        {
            Reforge = 0,
            Rarify = 1,
            AddAffix = 2,
            AddPrefix = 3,
            AddSuffix = 4,
            RemoveAll = 5,
            RemovePrefixes = 6,
            RemoveSuffixes = 7,
            RollAffixes = 8,
            RollPrefixes = 9,
            RollSuffixes = 10,
        }

        public Item modifiedItem = new Item();
        public Item modifierItem = new Item();

        int[] forgeActionCostMultipliers = { 1, 50, 10, 15, 15, 5, 15, 15, 5, 15, 15 };

        /// <summary>
        /// Sets <see cref="cost"/>.
        /// </summary>
        /// <param name="action"></param>
        public int CalculateCost(ForgeAction action)
        {
            if (modifiedItem == null || modifierItem == null || modifiedItem.IsAir || modifierItem.IsAir)
                return 0;
            ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
            return pomItem.rarity.forgeCost * forgeActionCostMultipliers[(int)action];
        }

        public bool CanForge(int cost)
        {
            return !modifiedItem.IsAir && !modifierItem.IsAir && modifierItem.stack >= cost && ItemLoader.PreReforge(modifiedItem);
        }


        /// <summary>
        /// Vanilla reforge
        /// </summary>
        //public void Reforge()
        //{
        //    int cost = CalculateCost(ForgeAction.Reforge);
        //    if (CanForge(cost))
        //    {
        //        bool favorited = modifiedItem.favorited;
        //        int stack = modifiedItem.stack;

        //        Item reforgedItem = new Item();
        //        reforgedItem.netDefaults(modifiedItem.netID);
        //        reforgedItem = reforgedItem.CloneWithModdedDataFrom(modifiedItem);
        //        reforgedItem.Prefix(-2);

        //        modifiedItem = reforgedItem;
        //        modifiedItem.position.X = Main.player[Main.myPlayer].position.X + (float)(Main.player[Main.myPlayer].width / 2) - (float)(modifiedItem.width / 2);
        //        modifiedItem.position.Y = Main.player[Main.myPlayer].position.Y + (float)(Main.player[Main.myPlayer].height / 2) - (float)(modifiedItem.height / 2);
        //        modifiedItem.favorited = favorited;
        //        modifiedItem.stack = stack;

        //        modifierItem.stack -= cost;

        //        ItemLoader.PostReforge(modifiedItem);
        //        PostForge(modifiedItem, modifierItem);
        //    }
        //}
        //public void RerollAffixes()
        //{
        //    int cost = CalculateCost(ForgeAction.Reforge);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        pomItem.RerollAffixes(modifiedItem);
        //        modifierItem.stack -= cost;

        //        PostForge(modifiedItem, modifierItem);
        //    }
        //}
        //public void Rarify()
        //{
        //    int cost = CalculateCost(ForgeAction.Rarify);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        if (pomItem.RaiseRarity(modifiedItem))
        //        {
        //            modifierItem.stack -= cost;

        //            PostForge(modifiedItem, modifierItem);
        //        }
        //    }
        //}
        //public void AddAffix()
        //{
        //    int cost = CalculateCost(ForgeAction.AddAffix);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        if (pomItem.AddRandomAffix(modifiedItem))
        //        {
        //            modifierItem.stack -= cost;

        //            PostForge(modifiedItem, modifierItem);
        //        }
        //    }
        //}
        //public void AddPrefix()
        //{
        //    int cost = CalculateCost(ForgeAction.AddPrefix);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        if (pomItem.AddRandomPrefix(modifiedItem))
        //        {
        //            modifierItem.stack -= cost;

        //            PostForge(modifiedItem, modifierItem);
        //        }
        //    }
        //}
        //public void AddSuffix()
        //{
        //    int cost = CalculateCost(ForgeAction.AddSuffix);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        if (pomItem.AddRandomSuffix(modifiedItem))
        //        {
        //            modifierItem.stack -= cost;

        //            PostForge(modifiedItem, modifierItem);
        //        }
        //    }
        //}
        //public void RemoveAll()
        //{
        //    int cost = CalculateCost(ForgeAction.RemoveAll);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        pomItem.RemoveAll(modifiedItem);
        //        modifierItem.stack -= cost;

        //        PostForge(modifiedItem, modifierItem);
        //    }
        //}
        //public void RemovePrefixes()
        //{
        //    int cost = CalculateCost(ForgeAction.RemovePrefixes);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        pomItem.RemovePrefixes(modifiedItem);
        //        modifierItem.stack -= cost;

        //        PostForge(modifiedItem, modifierItem);
        //    }
        //}
        //public void RemoveSuffixes()
        //{
        //    int cost = CalculateCost(ForgeAction.RemoveSuffixes);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        pomItem.RemoveSuffixes(modifiedItem);
        //        modifierItem.stack -= cost;

        //        PostForge(modifiedItem, modifierItem);
        //    }
        //}
        //public void RollAffixes()
        //{
        //    int cost = CalculateCost(ForgeAction.RollAffixes);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        pomItem.RollAffixTierMultipliers(modifiedItem);
        //        modifierItem.stack -= cost;

        //        PostForge(modifiedItem, modifierItem);
        //    }
        //}
        //public void RollPrefixes()
        //{
        //    int cost = CalculateCost(ForgeAction.RollPrefixes);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        pomItem.RollPrefixTierMultipliers(modifiedItem);
        //        modifierItem.stack -= cost;

        //        PostForge(modifiedItem, modifierItem);
        //    }
        //}
        //public void RollSuffixes()
        //{
        //    int cost = CalculateCost(ForgeAction.RollSuffixes);
        //    if (CanForge(cost))
        //    {
        //        ItemItem pomItem = modifiedItem.GetGlobalItem<ItemItem>();
        //        pomItem.RollSuffixTierMultipliers(modifiedItem);
        //        modifierItem.stack -= cost;

        //        PostForge(modifiedItem, modifierItem);
        //    }
        //}

        public void SendModifiedItemToServer()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                ItemPacketHandler.CModifierForgeModifiedItemChanged(ID, modifiedItem);
        }
        public void SendModifierItemToServer()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                ItemPacketHandler.CModifierForgeModifierItemChanged(ID, modifierItem);
        }

        public override void NetSend(BinaryWriter writer)
        {
            //PathOfModifiers.Log($"NetSend{Main.netMode}");
            ItemIO.Send(modifiedItem, writer, true);
            ItemIO.Send(modifierItem, writer, true);
        }
        public override void NetReceive(BinaryReader reader)
        {
            //PathOfModifiers.Log($"NetReceive{Main.netMode}");
            modifiedItem = ItemIO.Receive(reader, true);
            modifierItem = ItemIO.Receive(reader, true);
            if (Main.netMode != 2)
            {
                if (ModifierForge.activeForge?.Position == Position)
                {
                    UI.States.ModifierForge.Open(this);
                }
            }
        }

        public override TagCompound Save()
        {
            //PathOfModifiers.Log($"Save{Main.netMode}");
            TagCompound tag = new TagCompound();
            tag.Set("modifiedItem", ItemIO.Save(modifiedItem));
            tag.Set("modifierItem", ItemIO.Save(modifierItem));
            return tag;
        }
        public override void Load(TagCompound tag)
        {
            //PathOfModifiers.Log($"Load{Main.netMode}");
            modifiedItem = ItemIO.Load(tag.GetCompound("modifiedItem"));
            modifierItem = ItemIO.Load(tag.GetCompound("modifierItem"));
        }

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.IsActive && tile.type == ModContent.TileType<ModifierForge>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            //Main.NewText("i " + i + " j " + j + " t " + type + " s " + style + " d " + direction);
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i + 1, j, 3);
                NetMessage.SendData(87, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!modifiedItem.IsAir)
                {
                    PoMUtil.DropItem(new Vector2(Position.X * 16, Position.Y * 16), modifiedItem, 2);
                }
                if (!modifierItem.IsAir)
                {
                    PoMUtil.DropItem(new Vector2(Position.X * 16, Position.Y * 16), modifierItem, 2);
                }
            }

            if (Main.netMode != NetmodeID.Server && ModifierForge.activeForge == this)
                UI.States.ModifierForge.Close();
        }
    }
}