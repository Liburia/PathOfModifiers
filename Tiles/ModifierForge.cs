using System;
using System.IO;
using Microsoft.Xna.Framework;
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
        public static TEModifierForge activeForge;

        public override void SetDefaults()
        {
            Main.tileSpelunker[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileValue[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEModifierForge>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			//TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			//TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Modifier Forge");
			AddMapEntry(new Color(200, 200, 200), name);
			disableSmartCursor = false;
			drop = 0;
		}

		public override bool HasSmartInteract()
		{
			return true;
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode != 1)
                Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType("ModifierForge"));

            TEModifierForge tileEntity = (TEModifierForge)TileEntity.ByID[mod.GetTileEntity<TEModifierForge>().Find(i, j)];

            if (Main.netMode != 2 && activeForge == tileEntity)
                HideUI();

            tileEntity.Kill(i, j);
        }

        public override void RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
            }
            TEModifierForge clickedForge = (TEModifierForge)TileEntity.ByPosition[new Point16(left, top)];
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

        public static void ShowUI(TEModifierForge forge)
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
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
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
	}

    public class TEModifierForge : ModTileEntity
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
                Sync(ID);
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
                Sync(ID);
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
                    Sync(ID);
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
                    Sync(ID);
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
                    Sync(ID);
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
                    Sync(ID);
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
                Sync(ID);
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
                Sync(ID);
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
                Sync(ID);
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
                Sync(ID);
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
                Sync(ID);
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
                Sync(ID);
            }
        }

        public void Sync(int id, int ignoreClient = -1)
        {
            if (Main.netMode == 1)
            {
                PoMNetMessage.SyncTEModifierForge(ID, ByID.ContainsKey(ID), this);
            }
            else if (Main.netMode == 2)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, ignoreClient, null, id, Position.X, Position.Y);
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
            return tile.active() && tile.type == mod.TileType<ModifierForge>() && tile.frameX == 0 && tile.frameY == 0;
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