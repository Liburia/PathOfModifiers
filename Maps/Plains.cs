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

        public override int baseNNPCs => 20;

        public override Pack[] packs => new Pack[]
        {
            new Pack(new Tuple<int, int>[]{
                new Tuple<int, int>(NPCID.Zombie, 3),
                new Tuple<int, int>(NPCID.FemaleZombie, 2),
                new Tuple<int, int>(NPCID.SmallZombie, 1),
            }),
            new Pack(new Tuple<int, int>[]{
                new Tuple<int, int>(NPCID.PurpleSlime, 3),
                new Tuple<int, int>(NPCID.RedSlime, 2),
                new Tuple<int, int>(NPCID.YellowSlime, 1),
            }),
            new Pack(new Tuple<int, int>[]{
                new Tuple<int, int>(NPCID.Skeleton, 3),
            }, 0.1f),
        };
        public override Pack[] bossPacks => new Pack[]
        {
            new Pack(new Tuple<int, int>[]{
                new Tuple<int, int>(NPCID.ArmedZombie, 2),
            }),
        };

        public override void Generate(Rectangle dimensions)
        {
            LayeredSurfaceCaves gen = (LayeredSurfaceCaves)generator;
            gen.SetupSineWaves(0.5f, 10, 25f);
            gen.SetupNoise(10, 3, 0.35f);
            gen.SetupBezier(1, 4, 1, 10, 4);
            gen.SetupTiles(true, true, true);
            base.Generate(dimensions);
        }
    }
}