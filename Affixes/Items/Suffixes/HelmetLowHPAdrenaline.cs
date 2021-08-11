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
using Terraria.DataStructures;
using PathOfModifiers.Buffs;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class HelmetLowHPAdrenaline : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(2f, 1.5),
                new TTFloat.WeightedTier(4f, 0.5),
                new TTFloat.WeightedTier(6f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Excitement", 0.5),
            new WeightedTierName("of Motivation", 2),
            new WeightedTierName("of Adrenaline", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsHeadArmor(item);
        }

        public override string GetTolltipText()
        {
            return $"Gain Adrenaline for { Type1.GetValueFormat(1) }s when hit to low HP";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (ItemItem.IsArmorEquipped(item, player) && PoMUtil.IsLowHP(player))
            {
                player.AddBuff(ModContent.BuffType<Adrenaline>(), (int)Math.Round(Type1.GetValue() * 60));
            }
        }
    }
}