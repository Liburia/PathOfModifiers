using Microsoft.Xna.Framework;
using PathOfModifiers.Projectiles;
using PathOfModifiers.UI.Chat;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponChainLightning : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
            TwoWay = false,
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
            new WeightedTierName("of Static", 0.5),
            new WeightedTierName("of Inertia", 1),
            new WeightedTierName("of Sparking", 1.5),
            new WeightedTierName("of Lightning", 2),
            new WeightedTierName("of Electrification", 2.5),
            new WeightedTierName("of Galvanism", 3),
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
            return $"{ valueRange1 }% chance to chain lightning for { valueRange2 }% damage that { Keyword.GetTextOrTag(KeywordType.Shock, useChatTags) }s({ plusMinus }{ valueRange3 }%)";
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
                SpawnChainLightning(player, target, hitDamage, true);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnChainLightning(player, target, hitDamage, false);
            }
        }

        void SpawnChainLightning(Player player, Entity target, int hitDamage, bool isNPC)
        {
            PlaySound(player);
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);
            Projectile.NewProjectile(
                player.GetSource_FromThis(),
                position: player.Center,
                velocity: new Vector2(isNPC ? 1 : 0, target.whoAmI),
                Type: ModContent.ProjectileType<ChainLightning>(),
                Damage: damage,
                KnockBack: 0,
                Owner: player.whoAmI,
                ai0: Type3.GetValue());
        }

        void PlaySound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item72.WithVolumeScale(0.3f).WithPitchOffset(0.3f), player.Center);
        }
    }
}