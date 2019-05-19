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

        /// <summary>
        /// Should only be called on the server
        /// </summary>
        /// <param name="ignoreClient"></param>
        public void SendToClients(int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                throw new Exception("PoMTileEntity.Sync should never be called from the client");
            }
            else if (Main.netMode == 2)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, ignoreClient, null, ID, Position.X, Position.Y);
            }
        }
    }
}