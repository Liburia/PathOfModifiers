using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ArmorDamageTakenAndDealt : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.2f, -0.14f, 0.5),
                new TTFloat.WeightedTier(-0.14f, -0.07f, 1),
                new TTFloat.WeightedTier(-0.07f, 0, 2),
                new TTFloat.WeightedTier(0, 0.07f, 2),
                new TTFloat.WeightedTier(0.07f, 0.14f, 1),
                new TTFloat.WeightedTier(0.14f, 0.2f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Discordant", 3),
            new WeightedTierName("Tense", 1.5),
            new WeightedTierName("Balanced", 0.5),
            new WeightedTierName("Tranquil", 0.5),
            new WeightedTierName("Harmonic", 1.5),
            new WeightedTierName("Concordant", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText()
        {
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"Take and deal { plusMinus }{ Type1.GetValueFormat() }% damage";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue();
            }
            return true;
        }

        public override void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackmultiplier, ref bool crit)
        {
            if (ItemItem.IsArmorEquipped(affixItem, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (ItemItem.IsArmorEquipped(affixItem, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
    }
}
