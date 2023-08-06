using Microsoft.Xna.Framework;
using PathOfModifiers.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponIceBurst : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
                new TTFloat.WeightedTier(-0.03f, -0.05f, 3),
                new TTFloat.WeightedTier(-0.05f, -0.07f, 2.5),
                new TTFloat.WeightedTier(-0.07f, -0.09f, 2),
                new TTFloat.WeightedTier(-0.09f, -0.11f, 1.5),
                new TTFloat.WeightedTier(-0.11f, -0.13f, 1),
                new TTFloat.WeightedTier(-0.13f, -0.15f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Slush", 0.5),
            new WeightedTierName("of Hail", 1),
            new WeightedTierName("of Snow", 1.5),
            new WeightedTierName("of Ice", 2),
            new WeightedTierName("of Glacier", 2.5),
            new WeightedTierName("of Permafrost", 3),
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
            return $"{ valueRange1 }% chance to Ice Burst for { valueRange2 }% damage that leaves Chilled Air({ plusMinus }{ valueRange3 }%)";
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
                SpawnBurst(player, target, hitDamage);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnBurst(player, target, hitDamage);
            }
        }

        void SpawnBurst(Player player, Entity target, int hitDamage)
        {
            PlaySound(player);

            int spikeCount = 5;
            float speed = 20;
            float halfArc = 0.7f;
            float arc = halfArc * 2;
            float angleIncrement = arc / (spikeCount - 1);
            float angle = (target.Center - player.Center).ToRotation() - halfArc;
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);

            int spawnerBurst = spikeCount / 2;
            for (int i = 0; i < spikeCount; i++)
            {
                Vector2 velocity = angle.ToRotationVector2() * speed;
                Projectile.NewProjectile(
                player.GetSource_FromThis(),
                    position: player.Center,
                    velocity: velocity,
                    Type: ModContent.ProjectileType<IceBurst>(),
                    Damage: damage,
                    KnockBack: 0,
                    Owner: player.whoAmI,
                    ai0: Type3.GetValue(),
                    ai1: i == spawnerBurst ? 1f : 0f);
                angle += angleIncrement;
            }
        }

        void PlaySound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item66.WithVolumeScale(0.5f).WithPitchOffset(0.3f), player.Center);
        }
    }
}