using Microsoft.Xna.Framework;
using PathOfModifiers.Maps.Generators;
using System;
using Terraria;
using Terraria.ID;

namespace PathOfModifiers.Maps
{
    public class Plains : Map
    {
        public override Type generatorType => typeof(Generators.LayeredSurfaceCaves);

        public override float baseNPCFrequency => 20;

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

        public override bool Open(Rectangle dimensions)
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
                new OreSetting(6, new Generator.PatchSettings(TileID.Copper, Main.rand.Next(1, 4), 4, 10, 4, 8, Main.rand.NextFloat(0.3f, 0.9f), -20, new int[]{ TileID.Dirt, TileID.Grass, TileID.Stone })),
                new OreSetting(6, new Generator.PatchSettings(TileID.Tin, Main.rand.Next(1, 4), 4, 10, 4, 8, Main.rand.NextFloat(0.3f, 0.9f), -20, new int[]{ TileID.Dirt, TileID.Grass, TileID.Stone })),
            };
            gen.SetupOres(ores);

            return base.Open(dimensions);
        }
    }
}