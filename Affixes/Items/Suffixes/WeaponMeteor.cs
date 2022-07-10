using Microsoft.Xna.Framework;
using PathOfModifiers.Projectiles;
using PathOfModifiers.UI.Chat;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponMeteor : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.06f, 0.10f, 3),
                new TTFloat.WeightedTier(0.10f, 0.14f, 2.5),
                new TTFloat.WeightedTier(0.14f, 0.18f, 2),
                new TTFloat.WeightedTier(0.18f, 0.22f, 1.5),
                new TTFloat.WeightedTier(0.22f, 0.26f, 1),
                new TTFloat.WeightedTier(0.26f, 0.30f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1.0f, 1.5f, 3),
                new TTFloat.WeightedTier(1.5f, 2.0f, 2.5),
                new TTFloat.WeightedTier(2.0f, 2.5f, 2),
                new TTFloat.WeightedTier(2.5f, 3.0f, 1.5),
                new TTFloat.WeightedTier(3.0f, 3.5f, 1),
                new TTFloat.WeightedTier(3.5f, 4.0f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = true,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.6f, 1.0f, 3),
                new TTFloat.WeightedTier(1.0f, 1.4f, 2.5),
                new TTFloat.WeightedTier(1.4f, 1.8f, 2),
                new TTFloat.WeightedTier(1.8f, 2.2f, 1.5),
                new TTFloat.WeightedTier(2.2f, 2.6f, 1),
                new TTFloat.WeightedTier(2.6f, 3f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Flare", 0.5),
            new WeightedTierName("of Scorch", 1),
            new WeightedTierName("of Blaze", 1.5),
            new WeightedTierName("of Salvo", 2),
            new WeightedTierName("of Bombardment", 2.5),
            new WeightedTierName("of Phoenix", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            var valueRange3 = ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(), Type3.GetMinValueFormat(), Type3.GetMaxValueFormat(), useChatTags);
            string plusMinus = Type3.GetValue() >= 0 ? "+" : "-";
            return $"{ valueRange1 }% chance to Meteor for { valueRange2 }% damage that { Keyword.GetTextOrTag(KeywordType.Ignite, useChatTags) }s({ plusMinus }{ valueRange3 }%) and leaves Burning Air({ plusMinus }{ valueRange3 }%)";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Hit(item, player, target, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            Hit(item, player, target, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Hit(item, player, target, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            Hit(item, player, target, damage);
        }

        void Hit(Item item, Player player, NPC target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnMeteor(player, target, hitDamage);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnMeteor(player, target, hitDamage);
            }
        }

        void SpawnMeteor(Player player, Entity target, int hitDamage)
        {
            float height = 768f;
            Vector2 spawnOffset = new Vector2(0, -height).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
            Vector2 targetPosition = target.Center + Main.rand.NextVector2Circular(target.width, target.height);
            Vector2 spawnPosition = targetPosition + spawnOffset;

            PlaySound(spawnPosition);

            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);
            Projectile.NewProjectile(
                player.GetSource_FromThis(),
                targetPosition + spawnOffset,
                targetPosition,
                ModContent.ProjectileType<Meteor>(),
                damage,
                0,
                player.whoAmI,
                ai0: hitDamage * Type3.GetValue());
        }

        void PlaySound(Vector2 position)
        {
            SoundEngine.PlaySound(SoundID.Item45.WithVolumeScale(1f).WithPitchOffset(0.3f), position);
        }
    }
}