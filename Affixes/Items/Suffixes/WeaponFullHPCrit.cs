using PathOfModifiers.ModNet.PacketHandlers;
using System;
using Terraria;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponFullHPCrit : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.010f, 0.025f, 3),
                new TTFloat.WeightedTier(0.025f, 0.040f, 2.5),
                new TTFloat.WeightedTier(0.040f, 0.055f, 2),
                new TTFloat.WeightedTier(0.055f, 0.070f, 1.5),
                new TTFloat.WeightedTier(0.070f, 0.085f, 1),
                new TTFloat.WeightedTier(0.085f, 0.100f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Decimation", 0.5),
            new WeightedTierName("of Butchery", 1),
            new WeightedTierName("of Slaying", 1.5),
            new WeightedTierName("of Assassination", 2),
            new WeightedTierName("of Eradication", 2.5),
            new WeightedTierName("of Annihilation", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return $"Deal { valueRange1 }% of enemy HP with the first attack";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            Hit(item, player, target);
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit)
        {
            Hit(item, player, target);
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            Hit(item, player, target);
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            Hit(item, player, target);
        }

        void Hit(Item item, Player player, NPC target)
        {
            NPC realTarget = target.realLife >= 0 ? Main.npc[target.realLife] : target;
            if (item == player.HeldItem && realTarget.life >= realTarget.lifeMax)
            {
                int critDamage = (int)Math.Round(realTarget.lifeMax * Type1.GetValue());
                int direction = (target.Center.X - player.Center.X) > 0 ? 1 : -1;
                player.ApplyDamageToNPC(target, critDamage, 0, direction, false);
                PoMEffectHelper.Crit(target.position, target.width, target.height, 100);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    EffectPacketHandler.CSyncCrit(target, 100);
                }
            }
        }
        void Hit(Item item, Player player, Player target)
        {
            if (item == player.HeldItem && target.statLife >= target.statLifeMax2)
            {
                int critDamage = (int)Math.Round(target.statLifeMax2 * Type1.GetValue());
                int direction = (target.Center.X - player.Center.X) > 0 ? 1 : -1;
                target.Hurt(Terraria.DataStructures.PlayerDeathReason.ByPlayer(player.whoAmI), critDamage, direction, true, false, false);
                target.immune = false;
                PoMEffectHelper.Crit(target.position, target.width, target.height, 100);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    EffectPacketHandler.CSyncCrit(target, 100);
                }
            }
        }
    }
}