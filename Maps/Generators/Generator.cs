using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using PathOfModifiers.Affixes;
using PathOfModifiers.Rarities;
using Terraria.ID;

namespace PathOfModifiers.Maps.Generators
{
    public class Generator
    {
        public Mod mod;

        public virtual void Generate(Rectangle dimensions)
        {
        }

        protected void TilePlace(Point pos, int type, bool mute = true, bool force = true, int player = -1, int style = 0)
        {
            if (type == TileID.Grass)
                WorldGen.PlaceTile(pos.X, pos.Y, TileID.Dirt, mute, force, player, style);
            WorldGen.PlaceTile(pos.X, pos.Y, type, mute, force, player, style);
        }
        protected void TileRemove(Point pos, bool isMined = false, bool noItem = true, bool effectOnly = false)
        {
            WorldGen.KillTile(pos.X, pos.Y, isMined, effectOnly, noItem);
        }
    }
}