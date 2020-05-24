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

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    //Doesn't work with pvp, no hook.
    public class AccessoryReflect : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.1f, 0.5),
                new TTFloat.WeightedTier(0.2f, 3),
                new TTFloat.WeightedTier(0.3f, 3),
                new TTFloat.WeightedTier(0.4f, 0.5),
                new TTFloat.WeightedTier(0.5f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Prickly", 4),
            new WeightedTierName("Barbed", 1.5),
            new WeightedTierName("Spiky", 1.5),
            new WeightedTierName("Spinous", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsBodyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% melee damage reflected";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            if (PoMItem.IsAccessoryEquipped(item, player))
            {
                int reflectDamage = (int)Math.Round(damage * Type1.GetValue());
                float reflectKnockback = 0;
                int reflectDirection = 1;
                npc.StrikeNPC(reflectDamage, reflectKnockback, reflectDirection, crit);
            }
        }
    }
}
