using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class GreavesNoManaCost : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.5f, 2.5),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.5f, 1.5),
                new TTFloat.WeightedTier(2f, 1),
                new TTFloat.WeightedTier(2.5f, 0.5),
                new TTFloat.WeightedTier(3f, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(10f, 3),
                new TTFloat.WeightedTier(9f, 2.5),
                new TTFloat.WeightedTier(8f, 2),
                new TTFloat.WeightedTier(7f, 1.5),
                new TTFloat.WeightedTier(6f, 1),
                new TTFloat.WeightedTier(5f, 0.5),
                new TTFloat.WeightedTier(4f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Obscurity", 0.5),
            new WeightedTierName("of Mystery", 1),
            new WeightedTierName("of Mystic", 1.5),
            new WeightedTierName("of Cabal", 2),
            new WeightedTierName("of Occult", 2.5),
            new WeightedTierName("of Arcana", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsLegArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"No mana cost for { Type1.GetValueFormat(1) }s when hit ({ Type2.GetValueFormat(1) }s CD)";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (AffixItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type2.GetValue() * 60))
            {
                player.GetModPlayer<BuffPlayer>().AddNoManaCostBuff(player, (int)Math.Round(Type1.GetValue() * 60));

                lastProcTime = Main.GameUpdateCount;
            }
        }

        public override Affix Clone()
        {
            var affix = (GreavesNoManaCost)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}