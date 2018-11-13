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
using PathOfModifiers.Maps;

namespace PathOfModifiers.Tiles
{
    public class MapDevice : ModTile
    {
        public static TEMapDevice activeMD;

        public override void SetDefaults()
        {
            Main.tileSpelunker[Type] = false;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileValue[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEMapDevice>().Hook_AfterPlacement, -1, 0, true);
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
			name.SetDefault("Map Device");
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
                Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType("MapDevice"));

            TEMapDevice tileEntity = (TEMapDevice)TileEntity.ByID[mod.GetTileEntity<TEMapDevice>().Find(i, j)];

            if (Main.netMode != 2 && activeMD == tileEntity)
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
            TEMapDevice clickedMD = (TEMapDevice)TileEntity.ByPosition[new Point16(left, top)];
            if (MapDeviceUI.Instance.Visible && activeMD == clickedMD)
            {
                HideUI();
            }
            else
            {
                ShowUI(clickedMD);
            }
            return;
		}

        public static void ShowUI(TEMapDevice md)
        {
            if (activeMD != null)
                Main.PlaySound(SoundID.MenuTick);
            else
                Main.PlaySound(SoundID.MenuOpen);
            activeMD = md;
            Main.playerInventory = true;
            MapDeviceUI.Instance.Visible = true;
        }
        public static void HideUI()
        {
            if (activeMD != null)
                Main.PlaySound(SoundID.MenuClose);
            activeMD = null;
            MapDeviceUI.Instance.Visible = false;
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
			player.showItemIconText = "Map Device";
			if (player.showItemIconText == "Map Device")
			{
				player.showItemIcon2 = mod.ItemType("MapDevice");
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

    public class TEMapDevice : PoMTileEntity
    {
        public enum MapAction
        {
            Open = 0,
            Close = 1,
        }

        public Item map = new Item();

        public int timeLeft = 0;

        public bool CanBegin()
        {
            return !map.IsAir && map.modItem is Items.Map;
        }
        public bool CanEnd()
        {
            return !map.IsAir && map.modItem is Items.Map;
        }

        public void BeginMap()
        {
            if (!CanBegin())
                return;

            timeLeft = 10 * 60 * 60;

            Sync(ID, Main.myPlayer);
        }
        public void EndMap()
        {
            if (!CanEnd())
                return;

            timeLeft = 0;

            Sync(ID, Main.myPlayer);
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            //PathOfModifiers.Log($"NetSend{Main.netMode}");
            writer.Write(timeLeft);
            ItemIO.Send(map, writer, true);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            //PathOfModifiers.Log($"NetReceive{Main.netMode}");
            timeLeft = reader.ReadInt32();
            map = ItemIO.Receive(reader, true);
            if (Main.netMode != 2)
            {
                if (MapDevice.activeMD != null && MapDevice.activeMD.Position == Position)
                    MapDevice.ShowUI(this);
                else if (MapDevice.activeMD == this)
                    MapDeviceUI.Instance.SetItemSlot(map.Clone());
                MapDeviceUI.Instance.UpdateText();
            }
        }

        public override TagCompound Save()
        {
            //PathOfModifiers.Log($"Save{Main.netMode}");
            TagCompound tag = new TagCompound();
            tag.Set("timeLeft", timeLeft);
            tag.Set("map", ItemIO.Save(map));
            return tag;
        }
        public override void Load(TagCompound tag)
        {
            //PathOfModifiers.Log($"Load{Main.netMode}");
            timeLeft = tag.GetInt("timeLeft");
            map = ItemIO.Load(tag.GetCompound("map"));
            if (Main.netMode != 2)
                MapDeviceUI.Instance.UpdateText();
        }

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == mod.TileType<MapDevice>() && tile.frameX == 0 && tile.frameY == 0;
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
                if (!map.IsAir)
                {
                    PoMHelper.DropItem(new Vector2(Position.X * 16, Position.Y * 16), map, 2);
                }
            }
        }
    }
}