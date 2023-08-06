using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Items;
using PathOfModifiers.ModNet.PacketHandlers;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace PathOfModifiers.Tiles
{
    public class ModifierForge : ModTile
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static ModifierForgeTE activeForge;
#pragma warning restore CA2211 // Non-constant fields should not be visible

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
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Modifier Forge");
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

                if (forge.ModifierItem.ModItem is ModifierFragment)
                {
                    if (forge.ModifiedItem.IsAir)
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

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode != 1)
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), ModContent.ItemType<Items.ModifierForge>());

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

                if (!forge.ModifiedItem.IsAir)
                {
                    var screenDrawOffset = PoMUtil.DrawToScreenOffset();

                    var itemTexture = PoMUtil.GetItemTexture(forge.ModifiedItem.type).Value;
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
                if (!forge.ModifiedItem.IsAir && forge.ModifierItem.ModItem is ModifierFragment)
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

    public class ModifierForgeTE : ModTileEntity
    {
        Item modifiedItem = new();
        Item modifierItem = new();

        public Item ModifiedItem => modifiedItem;
        public Item ModifierItem => modifierItem;

        public void SetItem(Item item, bool updateUI = true)
        {
            item ??= new Item();
            modifiedItem = item;
            if (updateUI && Main.netMode != NetmodeID.Server && ModifierForge.activeForge == this)
            {
                UI.States.ModifierForge.UpdateItemsFromForge();
            }
        }
        public void SetFragment(Item item, bool updateUI = true)
        {
            item ??= new Item();
            modifierItem = item;
            if (updateUI && Main.netMode != NetmodeID.Server && ModifierForge.activeForge == this)
            {
                UI.States.ModifierForge.UpdateItemsFromForge();
            }
        }

        public void SendItemToServer()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                ItemPacketHandler.CModifierForgeItemChanged(ID, ModifiedItem);
        }
        public void SendFragmentToServer()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                ItemPacketHandler.CModifierForgeFragmentChanged(ID, ModifierItem);
        }

        public void SendItemToClients(int ignoreClient)
        {
            if (Main.netMode == NetmodeID.Server)
                ItemPacketHandler.SModifierForgeItemChanged(ID, ModifiedItem, ignoreClient);
        }
        public void SendFragmentToClients(int ignoreClient)
        {
            if (Main.netMode == NetmodeID.Server)
                ItemPacketHandler.SModifierForgeFragmentChanged(ID, ModifierItem, ignoreClient);
        }

        public override void NetSend(BinaryWriter writer)
        {
            //PathOfModifiers.Log($"NetSend{Main.netMode}");
            ItemIO.Send(ModifiedItem, writer, true);
            ItemIO.Send(ModifierItem, writer, true);
        }
        public override void NetReceive(BinaryReader reader)
        {
            //PathOfModifiers.Log($"NetReceive{Main.netMode}");
            SetItem(ItemIO.Receive(reader, true), true);
            SetFragment(ItemIO.Receive(reader, true), true);
        }

        public override void SaveData(TagCompound tag)
        {
            //PathOfModifiers.Log($"Save{Main.netMode}");
            tag.Set("modifiedItem", ItemIO.Save(ModifiedItem));
            tag.Set("modifierItem", ItemIO.Save(ModifierItem));
        }
        public override void LoadData(TagCompound tag)
        {
            //PathOfModifiers.Log($"Load{Main.netMode}");
            SetItem(ItemIO.Load(tag.GetCompound("modifiedItem")), true);
            SetFragment(ItemIO.Load(tag.GetCompound("modifierItem")), true);
        }

        public override bool IsTileValidForEntity(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.HasTile && tile.TileType == ModContent.TileType<ModifierForge>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i + 1, j, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
                return -1;
            }
            else
            {
                return Place(i, j);
            }
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!ModifiedItem.IsAir)
                {
                    PoMUtil.DropItem(null, new Vector2(Position.X * 16, Position.Y * 16), ModifiedItem, 2);
                }
                if (!ModifierItem.IsAir)
                {
                    PoMUtil.DropItem(null, new Vector2(Position.X * 16, Position.Y * 16), ModifierItem, 2);
                }
            }

            if (Main.netMode != NetmodeID.Server && ModifierForge.activeForge == this)
                UI.States.ModifierForge.Close();
        }
    }
}