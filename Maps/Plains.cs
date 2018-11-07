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
using PathOfModifiers.Maps.Generators;
using Terraria.ModLoader.IO;
using Terraria.ID;

namespace PathOfModifiers.Maps
{
    public class Plains : Map
    {
        public override Type generatorType => typeof(Generators.LayeredSurfaceCaves);

        public override void Generate(Point pos)
        {
            size = new Point(50, 50);
            LayeredSurfaceCaves gen = (LayeredSurfaceCaves)generator;
            gen.SetupSineWaves(0.5f, 10, 25f);
            gen.SetupNoise(10, 3, 0.35f);
            gen.SetupBezier(1, 4, 1, 10, 4);
            gen.SetupTiles();
            base.Generate(pos);
        }
    }
}