using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using Terraria.ID;
using PathOfModifiers.ModNet.PacketHandlers;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class AccessoryShockChance : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.016f, 2.5),
                new TTFloat.WeightedTier(0.033f, 2),
                new TTFloat.WeightedTier(0.05f, 1.5),
                new TTFloat.WeightedTier(0.066f, 1),
                new TTFloat.WeightedTier(0.084f, 0.5),
                new TTFloat.WeightedTier(0.1f, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.016f, 2.5),
                new TTFloat.WeightedTier(0.033f, 2),
                new TTFloat.WeightedTier(0.05f, 1.5),
                new TTFloat.WeightedTier(0.066f, 1),
                new TTFloat.WeightedTier(0.084f, 0.5),
                new TTFloat.WeightedTier(0.1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Static", 0.5),
            new WeightedTierName("of Sparking", 1),
            new WeightedTierName("of Discharge", 1.5),
            new WeightedTierName("of Lightning", 2),
            new WeightedTierName("of Electrification", 2.5),
            new WeightedTierName("of Galvanism", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            char plusMinus = Type2.GetValue() < 0 ? '-' : '+';
            return $"{ Type1.GetValueFormat() }% chance to Shock({ plusMinus }{ Type2.GetValueFormat() }%)";
        }

        public override void PlayerOnHitNPC(Item affixItem, Player player, Item item, NPC target, int damage, float knockback, bool crit)
        {
            Shock(item, player, target);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Shock(item, player, target);
        }
        public override void PlayerOnHitPvp(Item affixItem, Player player, Item item, Player target, int damage, bool crit)
        {
            Shock(item, player, target);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            Shock(item, player, target);
        }

        void Shock(Item item, Player player, NPC target)
        {
            if (ItemItem.IsAccessoryEquipped(item, player) && Main.rand.NextFloat(1f) < Type1.GetValue())
            {
                target.GetGlobalNPC<BuffNPC>().AddShockedBuff(target, Type2.GetValue(), PoMGlobals.ailmentDuration);
            }
        }
        void Shock(Item item, Player player, Player target)
        {
            if (ItemItem.IsAccessoryEquipped(item, player) && Main.rand.NextFloat(1f) < Type1.GetValue())
            {
                target.GetModPlayer<BuffPlayer>().AddShockedBuff(target, Type2.GetValue(), PoMGlobals.ailmentDuration);
            }
        }
    }
}