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
    public class HelmetReflectNova : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.005f, 2.5),
                new TTFloat.WeightedTier(0.01f, 2),
                new TTFloat.WeightedTier(0.015f, 1.5),
                new TTFloat.WeightedTier(0.02f, 1),
                new TTFloat.WeightedTier(0.025f, 0.5),
                new TTFloat.WeightedTier(0.03f, 0),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(6f, 3),
                new TTFloat.WeightedTier(5f, 2.5),
                new TTFloat.WeightedTier(4f, 2),
                new TTFloat.WeightedTier(3f, 1.5),
                new TTFloat.WeightedTier(2f, 1),
                new TTFloat.WeightedTier(1f, 0.5),
                new TTFloat.WeightedTier(0f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Bristles", 0.5),
            new WeightedTierName("of Barbs", 1),
            new WeightedTierName("or Thorns", 1.5),
            new WeightedTierName("of Brambles", 2),
            new WeightedTierName("of Spikes", 2.5),
            new WeightedTierName("of Reflect", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsHeadArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"Nova for { Type1.GetValueFormat() }% of damage taken + { Type2.GetValueFormat() }% of target's HP as damage when hit ({ Type3.GetValueFormat(1) }s CD)";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            SpawnNova(item, player, damage);
        }
        public override void OnHitByProjectile(Item item, Player player, Projectile projectile, int damage, bool crit)
        {
            SpawnNova(item, player, damage);
        }
        public override void OnHitByPvp(Item item, Player player, Player attacker, int damage, bool crit)
        {
            SpawnNova(item, player, damage);
        }

        void SpawnNova(Item item, Player player, int damageTaken)
        {
            if (AffixItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type3.GetValue() * 60))
            {
                PlaySound(player);

                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<ReflectNova>(), (int)Math.Round(damageTaken * Type1.GetValue()), 0, player.whoAmI, Type2.GetValue());

                lastProcTime = Main.GameUpdateCount;
            }
        }

        void PlaySound(Player player)
        {
            Main.PlaySound(SoundID.Item74.WithVolume(0.5f).WithPitchVariance(0.3f), player.Center);
        }

        public override Affix Clone()
        {
            var affix = (HelmetReflectNova)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}