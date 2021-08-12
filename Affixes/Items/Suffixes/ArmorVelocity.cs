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
    public class ArmorVelocity : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-11f, -8f, 0.5),
                new TTFloat.WeightedTier(-8f, -5f, 1),
                new TTFloat.WeightedTier(-5f, -2f, 2),
                new TTFloat.WeightedTier(2f, 5f, 2),
                new TTFloat.WeightedTier(5f, 8f, 1),
                new TTFloat.WeightedTier(8f, 11f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Repulsion", 3),
            new WeightedTierName("of Repel", 2),
            new WeightedTierName("of Fending", 0.5),
            new WeightedTierName("of Pulling", 0.5),
            new WeightedTierName("of Attraction", 2),
            new WeightedTierName("of Magnetism", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText()
        {
            string towardsAway = Type1.GetValue() >= 0 ? "towards" : "away from";
            return $"Gain { Type1.GetValueFormat(1) } velocity { towardsAway } target when hit";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            GainVelocity(item, player, npc.Center);
        }
        public override void OnHitByPvp(Item item, Player player, Player attacker, int damage, bool crit)
        {
            GainVelocity(item, player, attacker.Center);
        }
        public override void OnHitByProjectile(Item item, Player player, Projectile projectile, int damage, bool crit)
        {
            GainVelocity(item, player, projectile.Center);
        }

        void GainVelocity(Item item, Player player, Vector2 position)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                Vector2 addVelocity = (position - player.Center).SafeNormalize(Vector2.Zero) * Type1.GetValue();
                player.velocity += addVelocity;
            }
        }
    }
}