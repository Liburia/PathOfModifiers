using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Items;
using PathOfModifiers.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace PathOfModifiers.Tiles
{
    public class ModifierForge : ModTile
    {
        public static ModifierForgeTE activeForge;

        static int activeAnimationFirstFrame;
        static int activeAnimationFrameCount;
        static int activeAnimationFullFrameCount;

        public override void SetDefaults()
        {
            Main.tileSpelunker[Type] = false;
            //Main.tileShine2[Type] = true;
            //Main.tileShine[Type] = 1200;
            Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileValue[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<ModifierForgeTE>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.DrawYOffset = 2;
            //TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            //TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Modifier Forge");
			AddMapEntry(new Color(192, 120, 0), name);
			disableSmartCursor = false;
			drop = 0;
            animationFrameHeight = 38;
            
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
            if (PoMHelper.TryGetTileEntity(i, j, 18, 18, out TileEntity te))
            {
                var forge = (ModifierForgeTE)te;

                if (forge.modifierItem.modItem is ModifierFragment)
                {
                    if (forge.modifiedItem.IsAir)
                    {
                        //Filled but not active
                        frameYOffset = animationFrameHeight;
                    }
                    else
                    {
                        int frame = Main.tileFrame[type];
                        if (frame >= activeAnimationFrameCount)
                        {
                            frame = activeAnimationFrameCount - (frame - activeAnimationFrameCount) - 2;
                        }

                        //Filled and active
                        frameYOffset = animationFrameHeight * (activeAnimationFirstFrame + frame);
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
                Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType("ModifierForge"));

            ModifierForgeTE tileEntity = (ModifierForgeTE)TileEntity.ByID[mod.GetTileEntity<ModifierForgeTE>().Find(i, j)];

            if (Main.netMode != 2 && activeForge == tileEntity)
                HideUI();

            tileEntity.Kill(i, j);
        }

        public override void RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Main.mouseRightRelease = false;
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
            }

            //Hardcoded frame coordinate values because using TileObjectData is cancer.
            PoMHelper.TryGetTileEntity(i, j, 18, 18, out TileEntity te);
            ModifierForgeTE clickedForge = (ModifierForgeTE)te;
            if (ModifierForgeUI.Instance.Visible && activeForge == clickedForge)
            {
                HideUI();
            }
            else
            {
                ShowUI(clickedForge);
            }
            return;
		}

        public static void ShowUI(ModifierForgeTE forge)
        {
            if (activeForge != null)
                Main.PlaySound(SoundID.MenuTick);
            else
                Main.PlaySound(SoundID.MenuOpen);
            activeForge = forge;
            Main.playerInventory = true;
            ModifierForgeUI.Instance.Visible = true;
        }
        public static void HideUI()
        {
            if (activeForge != null)
                Main.PlaySound(SoundID.MenuClose);
            activeForge = null;
            ModifierForgeUI.Instance.Visible = false;
        }

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.showItemIconText = "Modifier Forge";
			if (player.showItemIconText == "Modifier Forge")
			{
				player.showItemIcon2 = mod.ItemType("ModifierForge");
				player.showItemIconText = "";
			}
			player.noThrow = 2;
			player.showItemIcon = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
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
                    var screenDrawOffset = PoMHelper.DrawToScreenOffset();

                    var itemTexture = Main.itemTexture[forge.modifiedItem.type];
                    var itemWidth = 24;
                    var itemHeight = (int)(itemWidth * (itemTexture.Height / (float)itemTexture.Width));
                    var itemDrawOffset = new Point16((48 - itemWidth) / 2, 5 - itemHeight);

                    var destRect = new Rectangle(
                        forgePos.X * 16 - (int)Main.screenPosition.X + (int)screenDrawOffset.X + itemDrawOffset.X,
                        forgePos.Y * 16 - (int)Main.screenPosition.Y + (int)screenDrawOffset.Y + itemDrawOffset.Y,
                        itemWidth,
                        itemHeight);
                    spriteBatch.Draw(itemTexture, destRect, Color.White);
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
                if (!forge.modifiedItem.IsAir && forge.modifierItem.modItem is ModifierFragment)
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
        
        int[] forgeActionCostMultipliers = { 1, 5, 10, 15, 15, 5, 15, 15, 5, 15, 15 };

        /// <summary>
        /// Sets <see cref="cost"/>.
        /// </summary>
        /// <param name="action"></param>
        public int CalculateCost(ForgeAction action)
        {
            if (modifiedItem == null || modifierItem == null || modifiedItem.IsAir || modifierItem.IsAir)
                return 0;
            PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
            return pomItem.rarity.forgeCost * forgeActionCostMultipliers[(int)action];
        }

        public bool CanForge(int cost)
        {
            return !modifiedItem.IsAir && !modifierItem.IsAir && modifierItem.stack >= cost && ItemLoader.PreReforge(modifiedItem);
        }
        
        /// <summary>
        /// Vanilla reforge
        /// </summary>
        public void Reforge()
        {
            int cost = CalculateCost(ForgeAction.Reforge);
            if (CanForge(cost))
            {
                bool favorited = modifiedItem.favorited;
                int stack = modifiedItem.stack;

                Item reforgedItem = new Item();
                reforgedItem.netDefaults(modifiedItem.netID);
                reforgedItem = reforgedItem.CloneWithModdedDataFrom(modifiedItem);
                reforgedItem.Prefix(-2);

                modifiedItem = reforgedItem;
                modifiedItem.position.X = Main.player[Main.myPlayer].position.X + (float)(Main.player[Main.myPlayer].width / 2) - (float)(modifiedItem.width / 2);
                modifiedItem.position.Y = Main.player[Main.myPlayer].position.Y + (float)(Main.player[Main.myPlayer].height / 2) - (float)(modifiedItem.height / 2);
                modifiedItem.favorited = favorited;
                modifiedItem.stack = stack;

                modifierItem.stack -= cost;

                ItemLoader.PostReforge(modifiedItem);
                ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                Sync();
            }
        }
        public void RerollAffixes()
        {
            int cost = CalculateCost(ForgeAction.Reforge);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                pomItem.RerollAffixes(modifiedItem);
                modifierItem.stack -= cost;

                ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                Sync();
            }
        }
        public void Rarify()
        {
            int cost = CalculateCost(ForgeAction.Rarify);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                if (pomItem.RaiseRarity(modifiedItem))
                {
                    modifierItem.stack -= cost;

                    ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                    Main.PlaySound(SoundID.Item37, -1, -1);
                    ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                    Sync();
                }
            }
        }
        public void AddAffix()
        {
            int cost = CalculateCost(ForgeAction.AddAffix);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                if (pomItem.AddRandomAffix(modifiedItem))
                {
                    modifierItem.stack -= cost;

                    ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                    Main.PlaySound(SoundID.Item37, -1, -1);
                    ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                    Sync();
                }
            }
        }
        public void AddPrefix()
        {
            int cost = CalculateCost(ForgeAction.AddPrefix);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                if (pomItem.AddRandomPrefix(modifiedItem))
                {
                    modifierItem.stack -= cost;

                    ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                    Main.PlaySound(SoundID.Item37, -1, -1);
                    ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                    Sync();
                }
            }
        }
        public void AddSuffix()
        {
            int cost = CalculateCost(ForgeAction.AddSuffix);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                if (pomItem.AddRandomSuffix(modifiedItem))
                {
                    modifierItem.stack -= cost;

                    ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                    Main.PlaySound(SoundID.Item37, -1, -1);
                    ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                    Sync();
                }
            }
        }
        public void RemoveAll()
        {
            int cost = CalculateCost(ForgeAction.RemoveAll);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                pomItem.RemoveAll(modifiedItem);
                modifierItem.stack -= cost;

                ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                Sync();
            }
        }
        public void RemovePrefixes()
        {
            int cost = CalculateCost(ForgeAction.RemovePrefixes);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                pomItem.RemovePrefixes(modifiedItem);
                modifierItem.stack -= cost;

                ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                Sync();
            }
        }
        public void RemoveSuffixes()
        {
            int cost = CalculateCost(ForgeAction.RemoveSuffixes);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                pomItem.RemoveSuffixes(modifiedItem);
                modifierItem.stack -= cost;

                ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                Sync();
            }
        }
        public void RollAffixes()
        {
            int cost = CalculateCost(ForgeAction.RollAffixes);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                pomItem.RollAffixTierMultipliers(modifiedItem);
                modifierItem.stack -= cost;

                ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                Sync();
            }
        }
        public void RollPrefixes()
        {
            int cost = CalculateCost(ForgeAction.RollPrefixes);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                pomItem.RollPrefixTierMultipliers(modifiedItem);
                modifierItem.stack -= cost;

                ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                Sync();
            }
        }
        public void RollSuffixes()
        {
            int cost = CalculateCost(ForgeAction.RollSuffixes);
            if (CanForge(cost))
            {
                PoMItem pomItem = modifiedItem.GetGlobalItem<PoMItem>();
                pomItem.RollSuffixTierMultipliers(modifiedItem);
                modifierItem.stack -= cost;

                ItemText.NewText(modifiedItem, modifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                Sync();
            }
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            //PathOfModifiers.Log($"NetSend{Main.netMode}");
            ItemIO.Send(modifiedItem, writer, true);
            ItemIO.Send(modifierItem, writer, true);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            //PathOfModifiers.Log($"NetReceive{Main.netMode}");
            modifiedItem = ItemIO.Receive(reader, true);
            modifierItem = ItemIO.Receive(reader, true);
            if (Main.netMode != 2)
            {
                if (ModifierForge.activeForge != null && ModifierForge.activeForge.Position == Position)
                    ModifierForge.ShowUI(this);
                else if (ModifierForge.activeForge == this)
                    ModifierForgeUI.Instance.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
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
            return tile.active() && tile.type == mod.TileType<ModifierForge>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            //Main.NewText("i " + i + " j " + j + " t " + type + " s " + style + " d " + direction);
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(87, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override void OnKill()
        {
            if (Main.netMode != 1)
            {
                if (!modifiedItem.IsAir)
                {
                    PoMHelper.DropItem(new Vector2(Position.X * 16, Position.Y * 16), modifiedItem, 2);
                }
                if (!modifierItem.IsAir)
                {
                    PoMHelper.DropItem(new Vector2(Position.X * 16, Position.Y * 16), modifierItem, 2);
                }
            }
        }
    }
}