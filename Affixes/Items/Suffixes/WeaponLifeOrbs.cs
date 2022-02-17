using Microsoft.Xna.Framework;
using PathOfModifiers.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponLifeOrbs : AffixTiered<TTFloat, TTInt, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.10f, 0.18f, 3),
                new TTFloat.WeightedTier(0.18f, 0.27f, 2.5),
                new TTFloat.WeightedTier(0.27f, 0.35f, 2),
                new TTFloat.WeightedTier(0.35f, 0.44f, 1.5),
                new TTFloat.WeightedTier(0.44f, 0.52f, 1),
                new TTFloat.WeightedTier(0.52f, 0.60f, 0.5),
            },
        };
        public override TTInt Type2 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(1, 2, 3),
                new TTInt.WeightedTier(2, 3, 2.5),
                new TTInt.WeightedTier(3, 4, 2),
                new TTInt.WeightedTier(4, 5, 1.5),
                new TTInt.WeightedTier(5, 6, 1),
                new TTInt.WeightedTier(6, 7, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.0010f, 0.0025f, 3),
                new TTFloat.WeightedTier(0.0025f, 0.0040f, 2.5),
                new TTFloat.WeightedTier(0.0040f, 0.0055f, 2),
                new TTFloat.WeightedTier(0.0055f, 0.0070f, 1.5),
                new TTFloat.WeightedTier(0.0070f, 0.0085f, 1),
                new TTFloat.WeightedTier(0.0085f, 0.0100f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Blood", 0.5),
            new WeightedTierName("of Bloodthirst", 1),
            new WeightedTierName("of Crimson", 1.5),
            new WeightedTierName("of Scarlet", 2),
            new WeightedTierName("of Sanguine", 2.5),
            new WeightedTierName("of Vampirism", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            var valueRange3 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(), Type3.GetMinValueFormat(), Type3.GetMaxValueFormat(), useChatTags);
            return $"{ valueRange1 }% chance to release { valueRange2 } life orbs on hit that heal { valueRange3 }% of damage dealt";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnLifeOrbs(player, target, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnLifeOrbs(player, target, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnLifeOrbs(player, target, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnLifeOrbs(player, target, damage);
        }

        void SpawnLifeOrbs(Player player, Entity target, int damage)
        {
            int projectileNumber = Type2.GetValue();
            for (int i = 0; i < projectileNumber; i++)
            {
                Vector2 direction = Main.rand.NextVector2Unit();
                Vector2 velocity = direction * Main.rand.NextFloat(5, 10);
                Vector2 projTarget = player.Center + Main.rand.NextVector2Circular(player.width * 1.5f, player.height * 1.5f);
                int heal = (int)MathHelper.Clamp(damage * Type3.GetValue(), 1, 999999);
                Projectile.NewProjectile(
                new PoMGlobals.ProjectileSource.PlayerSource(player),
                target.Center, velocity, ModContent.ProjectileType<LifeOrb>(), heal, 0, player.whoAmI, projTarget.X, projTarget.Y);
            }
        }
    }
}