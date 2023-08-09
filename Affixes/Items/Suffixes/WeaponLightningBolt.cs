using Microsoft.Xna.Framework;
using PathOfModifiers.Projectiles;
using PathOfModifiers.UI.Chat;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponLightningBolt : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
                new TTFloat.WeightedTier(0.2f, 0.5f, 3),
                new TTFloat.WeightedTier(0.5f, 0.8f, 2.5),
                new TTFloat.WeightedTier(0.8f, 1.1f, 2),
                new TTFloat.WeightedTier(1.1f, 1.4f, 1.5),
                new TTFloat.WeightedTier(1.4f, 1.7f, 1),
                new TTFloat.WeightedTier(1.7f, 2.0f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = true,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.03f, 0.05f, 3),
                new TTFloat.WeightedTier(0.05f, 0.07f, 2.5),
                new TTFloat.WeightedTier(0.07f, 0.09f, 2),
                new TTFloat.WeightedTier(0.09f, 0.11f, 1.5),
                new TTFloat.WeightedTier(0.11f, 0.13f, 1),
                new TTFloat.WeightedTier(0.13f, 0.15f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Grounding", 0.5),
            new WeightedTierName("of Discharge", 1),
            new WeightedTierName("of Dissonance", 1.5),
            new WeightedTierName("of Disruption", 2),
            new WeightedTierName("of Instability", 2.5),
            new WeightedTierName("of Volatility", 3),
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
            string plusMinus = Type3.GetValue() >= 0 ? "+" : "-";
            var shockedAir = Keyword.GetTextOrTag(KeywordType.ShockedAir, useChatTags);
            return $"{ valueRange1 }% chance for lightning to strike for { valueRange2 }% damage and leave { shockedAir }({ plusMinus }{ valueRange3 }%)";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Hit(item, player, target, damageDone);
        }
        public override void OnHitPvp(Item item, Player player, Player target, Player.HurtInfo hurtInfo)
        {
            Hit(item, player, target, hurtInfo.Damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Hit(item, player, target, damageDone);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            Hit(item, player, target, damageDone);
        }

        void Hit(Item item, Player player, NPC target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnLightningBolt(player, target, hitDamage);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnLightningBolt(player, target, hitDamage);
            }
        }

        void SpawnLightningBolt(Player player, Entity target, int hitDamage)
        {
            float height = 512;
            Vector2 position = target.Center + Main.rand.NextVector2Circular(target.width, target.height) - new Vector2(0, height);

            PlaySound(position);

            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);
            Projectile.NewProjectile(
                player.GetSource_FromThis(),
                position, Vector2.Zero, ModContent.ProjectileType<LightningBolt>(), damage, 0, player.whoAmI, Type3.GetValue(), height);
        }

        void PlaySound(Vector2 position)
        {
            SoundEngine.PlaySound(SoundID.Item94.WithVolumeScale(1f).WithPitchOffset(0.3f), position);
        }
    }
}