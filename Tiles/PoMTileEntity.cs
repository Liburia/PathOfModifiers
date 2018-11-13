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
    public class PoMTileEntity : ModTileEntity
    {
        public override bool ValidTile(int i, int j) { return false; }

        public void Sync(int id, int ignoreClient = -1)
        {
            if (Main.netMode == 1)
            {
                PoMNetMessage.SyncTileEntity(ID, ByID.ContainsKey(ID), this);
            }
            else if (Main.netMode == 2)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, ignoreClient, null, id, Position.X, Position.Y);
            }
        }
    }
}