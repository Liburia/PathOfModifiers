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
using System.Collections.Generic;
using System.Linq;
using PathOfModifiers.Utilities.Extensions;
using Microsoft.Xna.Framework.Graphics;

namespace PathOfModifiers.Tiles
{
    public class MapDevice : ModTile
    {
        public static MapDeviceTE activeMD;

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
            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<MapDeviceTE>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.Origin = new Point16(1, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            //TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            //TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Map Device");
			AddMapEntry(new Color(107, 66, 130), name);
			disableSmartCursor = false;
			drop = 0;
            animationFrameHeight = 72;

            activeAnimationFirstFrame = 1;
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
                var mapDevice = (MapDeviceTE)te;

                if (!mapDevice.mapItem.IsAir || mapDevice.timeLeft > 0)
                {
                    int frame = Main.tileFrame[type];
                    if (frame >= activeAnimationFrameCount)
                    {
                        frame = activeAnimationFrameCount - (frame - activeAnimationFrameCount) - 2;
                    }

                    //Map present or portal is open
                    frameYOffset = animationFrameHeight * (activeAnimationFirstFrame + frame);
                }
                else
                {
                    //Map absent
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
                Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType("MapDevice"));

            MapDeviceTE tileEntity = (MapDeviceTE)TileEntity.ByID[mod.GetTileEntity<MapDeviceTE>().Find(i, j)];

            if (Main.netMode != 2 && activeMD == tileEntity)
                MapDeviceUI.HideUI();

            tileEntity.Kill(i, j);
        }

        public override void RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
            }
            //Hardcoded frame coordinate values because using TileObjectData is cancer.
            PoMHelper.TryGetTileEntity(i, j, 18, 18, out TileEntity te);
            MapDeviceTE clickedMD = (MapDeviceTE)te;
            if (MapDeviceUI.Instance.IsVisible && activeMD == clickedMD)
            {
                MapDeviceUI. HideUI();
            }
            else
            {
                MapDeviceUI.ShowUI(clickedMD);
            }
            return;

        }

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
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

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //Hardcoded dimension values because using TileObjectData is cancer.
            var mapDevicePos = new Point16(i - 2, j - 1);
            if (TileEntity.ByPosition.TryGetValue(mapDevicePos, out TileEntity te))
            {
                var mapDevice = (MapDeviceTE)te;

                if (!mapDevice.mapItem.IsAir)
                {
                    var screenDrawOffset = PoMHelper.DrawToScreenOffset();

                    var itemTexture = Main.itemTexture[mapDevice.mapItem.type];
                    var itemSize = 24;
                    var itemScale = itemSize / (float)itemTexture.Width;
                    var itemDrawOffset = new Point16(20, 16);

                    var itemPosition = new Vector2(
                        mapDevicePos.X * 16 - Main.screenPosition.X + screenDrawOffset.X + itemDrawOffset.X,
                        mapDevicePos.Y * 16 - Main.screenPosition.Y + screenDrawOffset.Y + itemDrawOffset.Y);
                    spriteBatch.Draw(itemTexture, itemPosition, null, Color.White, 0, Vector2.Zero, itemScale, SpriteEffects.None, 0);
                    ((Items.Map)mapDevice.mapItem.modItem).map.DrawIcon(spriteBatch, itemPosition, itemTexture.Size(), Vector2.Zero, itemScale);
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            bool emitLight = false;
            //Hardcoded dimension values because using TileObjectData is cancer.
            var mapDevicePos = new Point16(i - 1, j - 1);
            if (TileEntity.ByPosition.TryGetValue(mapDevicePos, out TileEntity te))
            {
                var mapDevice = (MapDeviceTE)te;
                if (!mapDevice.mapItem.IsAir)
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

    public class MapDeviceTE : PoMTileEntity
    {
        public enum MapAction
        {
            Open = 0,
            Close = 1,
        }

        public Item mapItem = new Item();

        public int timeLeft = 0;

        public Rectangle? bounds = null;

        bool DetectBounds()
        {
            List<Rectangle> boundss = new List<Rectangle>();
            List<Tuple<Point, bool, bool>> adjacentTiles = new List<Tuple<Point, bool, bool>>();
            Point size = new Point(4, 4);
            int length = size.X * 2 + size.Y * 2 + 4;
            int x = 0;
            int y = 0;
            bool lastTileOfType = false;
            for (int i = 0; i < length; i++)
            {
                bool scanHoriz = true;
                bool scanVert = true;
                if (i == 0) { }
                else if (i > 0 && i < size.X + 2)
                {
                    x++;
                    scanHoriz = !lastTileOfType;
                }
                else if (i < size.X + 2 + size.Y + 1)
                {
                    y++;
                    scanVert = !lastTileOfType;
                }
                else if (i < size.X + 2 + size.Y + 1 + size.X + 1)
                {
                    x--;
                    scanHoriz = !lastTileOfType;
                }
                else
                {
                    y--;
                    scanVert = !lastTileOfType;
                }

                Point tilePos = new Point(Position.X - 1 + x, Position.Y - 1 + y);
                Tile tile = Main.tile[tilePos.X, tilePos.Y];
                int? tileType = PoMHelper.GetTileType(tile);
                if (tileType.HasValue && tileType == mod.TileType<MapBorder>())
                {
                    adjacentTiles.Add(new Tuple<Point, bool, bool>(tilePos, scanHoriz, scanVert));
                    lastTileOfType = true;
                }
                else
                {
                    lastTileOfType = false;
                }
            }

            foreach (var tilePos in adjacentTiles)
            {
                PoMHelper.FindAdjacentBounds(tilePos.Item1, boundss, tilePos.Item2, tilePos.Item3);
            }

            Rectangle tileBounds = new Rectangle(Position.X, Position.Y, size.X - 1, size.Y - 1);
            Rectangle tileBoundsInflated = tileBounds;
            tileBoundsInflated.Inflate(2, 2);
            for (int i = boundss.Count - 1; i >= 0; i--)
            {
                Rectangle bound = boundss[i];
                bool intersecnt = tileBoundsInflated.X < bound.X + bound.Width && bound.X < tileBoundsInflated.X + tileBoundsInflated.Width && tileBoundsInflated.Y < bound.Y + bound.Height && bound.Y < tileBoundsInflated.Y + tileBoundsInflated.Height;
                if (!bound.Intersects(tileBoundsInflated) || bound.Contains(tileBounds))
                    boundss.RemoveAt(i);
            }

            if (boundss.Count > 0)
            {
                bounds = boundss.Aggregate((b1, b2) => b1.Width * b1.Height > b2.Width * b2.Height ? b1 : b2);

                return true;
            }
            else
            {
                bounds = null;
                return false;
            }
        }

        bool CanOpen()
        {
            return !mapItem.IsAir && mapItem.modItem is Items.Map && timeLeft == 0 && DetectBounds();
        }
        bool CanClose()
        {
            return timeLeft > 0;
        }
        
        //Should never run on a client.
        public void OpenMap()
        {
            //PathOfModifiers.Instance.Logger.Info($"OpenMap: {Main.netMode}");
            if (!CanOpen())
                return;

            //TODO: Set timeLeft somewhere else(map settings/config)
            timeLeft = 10 * 60 * 60;

            Items.Map mapModItem = ((Items.Map)mapItem.modItem);
            Rectangle dimensions = new Rectangle(bounds.Value.X + 1, bounds.Value.Y + 1, bounds.Value.Width - 2, bounds.Value.Height - 2);

            var map = mapModItem.map;
            map.Open(dimensions);

            var mapBorder = mod.GetTile<MapBorder>();
            mapBorder.AddActiveBounds(bounds.Value);

            Sync();
        }
        //Should never run on a client.
        public void CloseMap()
        {
            //PathOfModifiers.Instance.Logger.Info($"CloseMap: {Main.netMode}");
            if (!CanClose())
                return;

            timeLeft = 0;
            
            Items.Map mapModItem = ((Items.Map)mapItem.modItem);
            Rectangle dimensions = new Rectangle(bounds.Value.X + 1, bounds.Value.Y + 1, bounds.Value.Width - 2, bounds.Value.Height - 2);

            var map = mapModItem.map;
            map.Close();

            var mapBorder = mod.GetTile<MapBorder>();
            mapBorder.RemoveActiveBounds(bounds.Value);

            Sync();
        }

        //Never runs on a client
        public override void Update()
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                if (timeLeft == 0)
                {
                    CloseMap();
                }
                else if (timeLeft % 60 == 0)
                {
                    if (Main.netMode == NetmodeID.Server)
                    {
                        Sync();
                    }
                    else if (MapDevice.activeMD == this && MapDeviceUI.Instance.IsVisible)
                    {
                        MapDeviceUI.Instance.UpdateText();
                    }
                }
            }
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            //PathOfModifiers.Instance.Logger.Info($"NetSend: {Main.netMode}");
            writer.Write(ID);
            writer.Write(timeLeft);
            bool isBoundsNull = !bounds.HasValue;
            writer.Write(isBoundsNull);
            if(!isBoundsNull)
                writer.Write((Rectangle)bounds);

            ItemIO.Send(mapItem, writer, true);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            //PathOfModifiers.Instance.Logger.Info($"NetReceive: {Main.netMode}");
            var oldID = reader.ReadInt32();

            timeLeft = reader.ReadInt32();
            
            var mapBorder = mod.GetTile<MapBorder>();

            bool isBoundsNull = reader.ReadBoolean();
            bounds = isBoundsNull ? null : (Rectangle?)reader.ReadRectangle();

            mapItem = ItemIO.Receive(reader, true);
            
            MapDeviceTE oldMD;
            int oldTimeLeft = 0;
            Rectangle? oldBounds = null;
            if (ByID.TryGetValue(oldID, out TileEntity oldTE))
            {
                oldMD = oldTE as MapDeviceTE;
                oldTimeLeft = oldMD.timeLeft;
                oldBounds = oldMD.bounds;
            }

            if (oldTimeLeft == 0 && timeLeft != 0)
            {
                mapBorder.AddActiveBounds(bounds.Value);
            }
            else if (oldTimeLeft != 0 && timeLeft == 0)
            {
                mapBorder.RemoveActiveBounds(oldBounds.Value);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //Cloned TE is different or something.
                if (MapDevice.activeMD != null && MapDevice.activeMD.Position == Position)
                    MapDeviceUI.ShowUI(this);
                else if (MapDevice.activeMD == this)
                {
                    MapDeviceUI.Instance.SetItemSlot(mapItem.Clone());
                    MapDeviceUI.Instance.UpdateText();
                }
            }
        }

        public override TagCompound Save()
        {
            //PathOfModifiers.Log($"Save{Main.netMode}");
            TagCompound tag = new TagCompound();
            tag.Set("timeLeft", timeLeft);
            bool isBoundsNull = bounds == null;
            tag.Set("isBoundsNull", isBoundsNull);
            if (!isBoundsNull)
                tag.Set("bounds", bounds);

            tag.Set("map", ItemIO.Save(mapItem));

            return tag;
        }
        public override void Load(TagCompound tag)
        {
            //PathOfModifiers.Log($"Load{Main.netMode}");

            var mapBorder = mod.GetTile<MapBorder>();

            timeLeft = tag.GetInt("timeLeft");
            bool isBoundsNull = tag.GetBool("isBoundsNull");
            bounds = isBoundsNull ? null : (Rectangle?)tag.Get<Rectangle>("bounds");
            if (timeLeft > 0 && bounds.HasValue)
                mapBorder.AddActiveBounds(bounds.Value);

            mapItem = ItemIO.Load(tag.GetCompound("map"));

            if (Main.netMode != 2)
                MapDeviceUI.Instance.UpdateText();
        }

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == mod.TileType<MapDevice>();
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
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (timeLeft > 0)
                {
                    CloseMap();
                }
                else if (!mapItem.IsAir)
                {
                    PoMHelper.DropItem(new Vector2(Position.X * 16, Position.Y * 16), mapItem, 2);
                }
            }
        }
    }
}