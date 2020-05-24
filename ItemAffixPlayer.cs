using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.Buffs;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;

namespace PathOfModifiers
{
    public class ItemAffixPlayer : ModPlayer
    {
        public class GoldDropChanceColletion
        {
            public class GoldDropChance
            {
                public bool enabled;
                public float chance;
                public int amount;
            }

            public Dictionary<Affix, GoldDropChance> dict = new Dictionary<Affix, GoldDropChance>();

            public void AddOrUpdate(Affix affix, float chance, int amount)
            {
                if (dict.TryGetValue(affix, out GoldDropChance gdc))
                {
                    gdc.enabled = true;
                    gdc.chance = chance;
                    gdc.amount = amount;
                }
                else
                {
                    dict.Add(affix, new GoldDropChance()
                    {
                        enabled = true,
                        chance = chance,
                        amount = amount,
                    });
                }
            }
            public void Clear()
            {
                dict.Clear();
            }
            public void ClearDisabled()
            {
                dict = dict
                 .Where(kv => kv.Value.enabled)
                 .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            public void ResetEffects()
            {
                foreach (var pair in dict)
                {
                    pair.Value.enabled = false;
                }
            }
            public int Roll()
            {
                int totalDrop = 0;
                foreach (var kv in dict)
                {
                    if (kv.Value.enabled && Main.rand.NextFloat(1f) < kv.Value.chance)
                    {
                        totalDrop += kv.Value.amount;
                    }
                }
                return totalDrop;
            }
        }

        public GoldDropChanceColletion goldDropChances;

        public override void Initialize()
        {
            goldDropChances = new GoldDropChanceColletion();
        }

        public override void ResetEffects()
        {
            goldDropChances.ResetEffects();
            if (Main.time == 0)
            {
                goldDropChances.ClearDisabled();
            }
        }
    }
}