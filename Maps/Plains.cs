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
            gen.SetupCaves(10, 3, 0.35f, 1, 4, 1, 10, 4);
            LayerSetings[] tileLayers = new LayerSetings[]
            {
                new LayerSetings(TileID.Grass, 0),
                new LayerSetings(TileID.Dirt, 1, true),
                new LayerSetings(TileID.Stone, 11, false),
            };
            LayerSetings[] wallLayers = new LayerSetings[]
            {
                new LayerSetings(WallID.Dirt, 1),
                new LayerSetings(WallID.Stone, 11),
            };
            gen.SetupTiles(tileLayers, wallLayers, true, true, true, true);

            OreSetting[] ores = new OreSetting[]
            {
                new OreSetting(6, new Generator.PatchSettings(TileID.Copper, Main.rand.Next(1, 4), 4, 10, 4, 8, Main.rand.NextFloat(0.3f, 0.9f), -20)),
                new OreSetting(6, new Generator.PatchSettings(TileID.Tin, Main.rand.Next(1, 4), 4, 10, 4, 8, Main.rand.NextFloat(0.3f, 0.9f), -20)),
            };
            gen.SetupOres(ores);
            base.Generate(dimensions);
        }
    }
}