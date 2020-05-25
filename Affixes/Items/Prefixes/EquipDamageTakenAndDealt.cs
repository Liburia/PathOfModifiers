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
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class EquipDamageTakenAndDealt : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
    {
                new TTFloat.WeightedTier(0.9f, 0.5),
                new TTFloat.WeightedTier(0.93f, 1.2),
                new TTFloat.WeightedTier(0.97f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.03f, 1),
                new TTFloat.WeightedTier(1.07f, 0.5),
                new TTFloat.WeightedTier(1.1f, 0),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Harmony", 3),
            new WeightedTierName("Parity", 1.5),
            new WeightedTierName("Balance", 0.5),
            new WeightedTierName("Tranquility", 0.5),
            new WeightedTierName("Concord", 1.5),
            new WeightedTierName("Tension", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAnyArmor(item) ||
                PoMItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            string moreLess = Type1.GetValue() > 1 ? "increased" : "reduced";
            return $"Take and deal {Type1.GetValueFormat() - 100}% {moreLess} damage";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (PoMItem.IsEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue() - 1;
            }
            return true;
        }

        public override void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackmultiplier, ref bool crit)
        {
            if (PoMItem.IsEquipped(affixItem, player))
            {
                damageMultiplier += Type1.GetValue() - 1;
            }
        }
        public override void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (PoMItem.IsEquipped(affixItem, player))
            {
                damageMultiplier += Type1.GetValue() - 1;
            }
        }
    }
}
