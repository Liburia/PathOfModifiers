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

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryCritDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.150f, -0.115f, 0.5),
                new TTFloat.WeightedTier(-0.115f, -0.085f, 1),
                new TTFloat.WeightedTier(-0.085f, -0.050f, 2),
                new TTFloat.WeightedTier(0.050f, 0.085f, 2),
                new TTFloat.WeightedTier(0.085f, 0.115f, 1),
                new TTFloat.WeightedTier(0.115f, 0.150f, 0.5),
            },
        };

        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Gentle", 3),
            new WeightedTierName("Calm", 2),
            new WeightedTierName("Tame", 0.5),
            new WeightedTierName("Fierce", 0.5),
            new WeightedTierName("Ferocious", 2),
            new WeightedTierName("Savage", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% crit damage";
        }

        public override void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            if (crit && ItemItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue();
        }
        public override void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (crit && ItemItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue();
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            if (crit && ItemItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue();
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (crit && ItemItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue();
        }
    }
}
