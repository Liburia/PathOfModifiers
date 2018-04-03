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
			Main.tileContainer[Type] = true;
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
			dustType = mod.DustType("Sparkle");
			disableSmartCursor = true;
			adjTiles = new int[] { TileID.Containers };
			drop = mod.ItemType("ModifierForge");
		}

		public override bool HasSmartInteract()
		{
			return true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

        public override void PlaceInWorld(int i, int j, Item item)
        {
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, drop);
            TEModifierForge tileEntity = mod.GetTileEntity<TEModifierForge>();
            if (!tileEntity.ModifiedItem.IsAir)
                Item.NewItem(i * 16, j * 16, 32, 32, tileEntity.ModifiedItem.type);
            if (!tileEntity.ModifierItem.IsAir)
                Item.NewItem(i * 16, j * 16, 32, 32, tileEntity.ModifierItem.type);
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
                ShowUI(clickedForge, left, top);
            }
            return;
			if (Main.netMode == 1)
			{
				if (left == player.chestX && top == player.chestY && player.chest >= 0)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					Main.PlaySound(SoundID.MenuClose);
				}
				else
				{
					NetMessage.SendData(31, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
			}
			else
			{
				int chest = Chest.FindChest(left, top);
				if (chest >= 0)
				{
					Main.stackSplit = 600;
					if (chest == player.chest)
					{
						player.chest = -1;
						Main.PlaySound(SoundID.MenuClose);
					}
					else
					{
						player.chest = chest;
						Main.playerInventory = true;
						Main.recBigList = false;
						player.chestX = left;
						player.chestY = top;
						Main.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
					}
					Recipe.FindRecipes();
				}
			}
		}

        public static void ShowUI(TEModifierForge forge, int tileX, int tileY)
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
                Main.PlaySound(SoundID.MenuOpen);
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
        Item modifiedItem = new Item();
        Item modifierItem = new Item();
        public Item ModifiedItem
        {
            get { return modifiedItem; }
            set
            {
                modifiedItem = value;
                Sync(ID);
            }
        }
        public Item ModifierItem
        {
            get { return modifierItem; }
            set
            {
                modifierItem = value;
                Sync(ID);
            }
        }

        public void Reforge()
        {
            Main.player[Main.myPlayer].mouseInterface = true;
            if (ModifierItem.stack >= 5 && ItemLoader.PreReforge(ModifiedItem))
            {
                ModifierItem.stack -= 5;

                bool favorited = ModifiedItem.favorited;
                int stack = ModifiedItem.stack;

                Item reforgedItem = new Item();
                reforgedItem.netDefaults(ModifiedItem.netID);
                reforgedItem = reforgedItem.CloneWithModdedDataFrom(ModifiedItem);
                reforgedItem.Prefix(-2);
                modifiedItem = reforgedItem.Clone();
                modifiedItem.position.X = Main.player[Main.myPlayer].position.X + (float)(Main.player[Main.myPlayer].width / 2) - (float)(modifiedItem.width / 2);
                modifiedItem.position.Y = Main.player[Main.myPlayer].position.Y + (float)(Main.player[Main.myPlayer].height / 2) - (float)(modifiedItem.height / 2);
                modifiedItem.favorited = favorited;
                modifiedItem.stack = stack;
                ItemLoader.PostReforge(modifiedItem);
                ItemText.NewText(ModifiedItem, ModifiedItem.stack, true, false);
                Main.PlaySound(SoundID.Item37, -1, -1);
                if (Main.netMode != 2)
                    ModifierForgeUI.Instance.SetItemSlots(ModifiedItem.Clone(), ModifierItem.Clone());
                Sync(ID);
            }
        }

        public void Sync(int id, int ignoreClient = -1)
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)MsgType.SyncTEModifierForge);
                packet.Write(ID);
                Write(packet, this, true);
                packet.Send();
            }
            else if (Main.netMode == 2)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, ignoreClient, null, id, Position.X, Position.Y);
            }
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            PathOfModifiers.Log($"NetReceive{Main.netMode}");
            modifiedItem = ItemIO.Receive(reader, true);
            modifierItem = ItemIO.Receive(reader, true);
            if (Main.netMode != 2)
                ModifierForgeUI.Instance.SetItemSlots(ModifiedItem.Clone(), ModifierItem.Clone());
        }
        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            PathOfModifiers.Log($"NetSend{Main.netMode}");
            ItemIO.Send(ModifiedItem, writer, true);
            ItemIO.Send(ModifierItem, writer, true);
        }

        public override TagCompound Save()
        {
            PathOfModifiers.Log($"Save{Main.netMode}");
            TagCompound tag = new TagCompound();
            tag.Set("modifiedItem", ItemIO.Save(ModifiedItem));
            tag.Set("modifierItem", ItemIO.Save(ModifierItem));
            return tag;
        }
        public override void Load(TagCompound tag)
        {
            PathOfModifiers.Log($"Load{Main.netMode}");
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
    }
}