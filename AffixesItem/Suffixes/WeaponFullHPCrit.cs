using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using PathOfModifiers.ModNet.PacketHandlers;
using Terraria.ID;

namespace PathOfModifiers.AffixesItem.Suffixes
{
    public class WeaponFullHPCrit : Suffix, ITieredStatFloatAffix
    {
        public override float weight => 0.5f;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float[] tiers = new float[] { 0f, 0.016f, 0.033f, 0.05f, 0.066f, 0.084f, 0.1f };
        static Tuple<int, double>[] tierWeights = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 3),
            new Tuple<int, double>(1, 2.5),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 1.5),
            new Tuple<int, double>(4, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static string[] tierNames = new string[] {
            "of Decimation",
            "of Butchery",
            "of Slaying",
            "of Assassination",
            "of Eradication",
            "of Annihilation",
        };
        static int maxTier => tiers.Length - 2;

        int tierText => maxTier - tier + 1;

        int tier = 0;
        string addedTextTiered = string.Empty;
        float addedTextWeightTiered = 1;

        float tierMultiplier = 0;
        float multiplier = 1;

        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            float percent1 = Multiplier * 100;

            int decimals1 = 0;
            if (percent1 < 1)
            {
                decimals1 = 2;
            }
            percent1 = (float)Math.Round(percent1, decimals1);

            return $"Deal {percent1}% of enemy HP with the first attack";
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
                DoDamage(player, target);
            }
        }
        void Hit(Item item, Player player, Player target)
        {
            if (item == player.HeldItem && target.statLife >= target.statLifeMax2)
            {
                DoDamage(player, target);
            }
        }

        void DoDamage(Player player, NPC target)
        {
            int critDamage = (int)Math.Round(target.lifeMax * multiplier);
            int direction = (target.Center.X - player.Center.X) > 0 ? 1 : -1;
            player.ApplyDamageToNPC(target, critDamage, 0, direction, false);
            PoMEffectHelper.Crit(target.position, target.width, target.height, 100);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncCrit(target, 100);
            }
        }
        void DoDamage(Player player, Player target)
        {
            int critDamage = (int)Math.Round(target.statLifeMax2 * multiplier);
            int direction = (target.Center.X - player.Center.X) > 0 ? 1 : -1;
            target.Hurt(Terraria.DataStructures.PlayerDeathReason.ByPlayer(player.whoAmI), critDamage, direction, true, false, false);
            PoMEffectHelper.Crit(target.position, target.width, target.height, 100);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncCrit(target, 100);
            }
        }

        #region Interface Properties
        public float Weight => weight;
        public float[] Tiers => tiers;
        public Tuple<int, double>[] TierWeights => tierWeights;
        public string[] TierNames => tierNames;
        public int MaxTier => maxTier;
        public int TierText => tierText;
        public int Tier { get { return tier; } set { tier = value; } }
        public string AddedTextTiered { get { return AddedTextTiered; } set { addedTextTiered = value; } }
        public float AddedTextWeightTiered { get { return addedTextWeightTiered; } set { addedTextWeightTiered = value; } }
        public float TierMultiplier { get { return tierMultiplier; } set { tierMultiplier = value; } }
        public float Multiplier { get { return multiplier; } set { multiplier = value; } }
        #endregion
        #region Helped Methods
        void SetTier(int tier)
        {
            TieredAffixHelper.SetTier(this, tier);
        }
        void SetTierMultiplier(float tierMultiplier)
        {
            TieredAffixHelper.SetTierMultiplier(this, tierMultiplier);
        }
        public override Affix Clone()
        {
            return TieredAffixHelper.Clone(this, (ITieredStatFloatAffix)base.Clone());
        }
        public override void RollValue(bool rollTier = true)
        {
            TieredAffixHelper.RollValue(this, rollTier);
        }
        public override void ReforgePrice(Item item, ref int price)
        {
            TieredAffixHelper.ReforgePrice(this, item, ref price);
        }
        public override void Save(TagCompound tag, Item item)
        {
            TieredAffixHelper.Save(this, tag, item);
        }
        public override void Load(TagCompound tag, Item item)
        {
            TieredAffixHelper.Load(this, tag, item);
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            TieredAffixHelper.NetSend(this, item, writer);
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            TieredAffixHelper.NetReceive(this, item, reader);
        }
        public override string GetForgeText(Item item)
        {
            return TieredAffixHelper.GetForgeText(this, item);
        }
        #endregion
    }
}