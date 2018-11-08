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

namespace PathOfModifiers.Maps
{
    public class Pack
    {
        public Tuple<int, int>[] npcCounts;
        public float weight;
        public float radius;
        public bool clearSpace;

        public Pack(Tuple<int, int>[] npcCounts, float weight = 1, float radius = 100, bool clearSpace = true)
        {
            this.npcCounts = npcCounts;
            this.weight = weight;
            this.radius = radius;
            this.clearSpace = clearSpace;
        }
    }

    public class Map
    {
        public Mod mod;

        public virtual Type generatorType => typeof(Generator);

        public virtual int baseNNPCs => 0;
        public virtual Pack[] packs => new Pack[0];
        public virtual Pack[] bossPacks => new Pack[0];

        Generator _generator;
        public virtual Generator generator
        {
            get
            {
                if (_generator == null)
                    _generator = PoMDataLoader.generators[PoMDataLoader.generatorMap[generatorType]];
                return _generator;
            }
        }

        public Point size;

        public virtual Map Clone()
        {
            Map newMap = (Map)Activator.CreateInstance(GetType());
            newMap.mod = mod;
            newMap._generator = generator;
            return newMap;
        }

        public virtual void Generate(Point pos)
        {
            Rectangle dimensions = new Rectangle(pos.X, pos.Y, size.X, size.Y);
            generator.GenerateTerrain(dimensions);
            generator.SpawnPacks(dimensions, baseNNPCs, MakePackArray());
        }

        public virtual Pack[] MakePackArray()
        {
            List<Pack> wPacks = new List<Pack>();

            Tuple<Pack, double>[] packsWeights = packs
                .Where(p => p.weight > 0)
                .Select(p => new Tuple<Pack, double>(p, p.weight))
                .ToArray();
            WeightedRandom<Pack> wRandom = new WeightedRandom<Pack>(Main.rand, packsWeights);

            int npcsRemaining = baseNNPCs;
            for (int i = 0; i < bossPacks.Length; i++)
            {
                Pack p = bossPacks[i];
                for (int j = 0; j < p.npcCounts.Length; j++)
                {
                    npcsRemaining -= p.npcCounts[i].Item2;
                }
                if (npcsRemaining >= 0)
                    wPacks.Add(p);
            }
            while (npcsRemaining > 0)
            {
                Pack p = wRandom;
                for(int i = 0; i < p.npcCounts.Length; i++)
                {
                    npcsRemaining -= p.npcCounts[i].Item2;
                }
                if (npcsRemaining >= 0)
                    wPacks.Add(p);
            }

            return wPacks.ToArray();
        }

        public virtual Item MakeItem()
        {
            return new Item();
        }

        public virtual void Save(TagCompound tag)
        {
        }
        public virtual void Load(TagCompound tag)
        {
        }

        public virtual void NetSend(BinaryWriter writer)
        {
        }
        public virtual void NetReceive(BinaryReader reader)
        {
        }
    }
}