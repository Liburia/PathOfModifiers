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

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ArmorSafety : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(30f, 1.5),
                new TTFloat.WeightedTier(60f, 0.5),
                new TTFloat.WeightedTier(90f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Protection", 0.5),
            new WeightedTierName("of Safeguarding", 2),
            new WeightedTierName("of Bulwark", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText()
        {
            return $"Gain safety buffs when hit for {Type1.GetValueFormat(1)}s";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            GainBuffs(item, player);
        }
        public override void OnHitByPvp(Item item, Player player, Player attacker, int damage, bool crit)
        {
            GainBuffs(item, player);
        }
        public override void OnHitByProjectile(Item item, Player player, Projectile projectile, int damage, bool crit)
        {
            GainBuffs(item, player);
        }

        void GainBuffs(Item item, Player player)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                int durationTicks = (int)Math.Round(Type1.GetValue() * 60);
                player.AddBuff(BuffID.Dangersense, durationTicks);
                player.AddBuff(BuffID.Featherfall, durationTicks);
                player.AddBuff(BuffID.Gravitation, durationTicks);
                player.AddBuff(BuffID.Hunter, durationTicks);
                player.AddBuff(BuffID.ObsidianSkin, durationTicks);
            }
        }
    }
}