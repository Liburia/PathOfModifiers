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
    public class Test : Generator
    {
        public override void Generate(Rectangle dimensions)
        {
            int tileTypeToPlace = 0;
            bool removeTile = false;
            Point tilePos;
            for (int x = 0; x < dimensions.Width; x++)
            {
                for (int y = 0; y < dimensions.Height; y++)
                {
                    removeTile = false;
                    tilePos = new Point(dimensions.X + x, dimensions.Y + y);
                    if (y < dimensions.Height / 2)
                        removeTile = true;
                    else
                        tileTypeToPlace = TileID.Iron;

                    if (removeTile)
                        TileRemove(tilePos);
                    else
                        TilePlace(tilePos, tileTypeToPlace);
                }
            }
        }
    }
}