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

namespace PathOfModifiers.AffixesItem.Suffixes
{
    public class WeaponCrit : Suffix, ITieredStatFloat2Affix
    {
        public override float weight => 0.5f;

        string addedTextTiered = string.Empty;
        float addedTextWeightTiered = 1;
        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float[] tiers1 = new float[] { 0.14f, 0.2f, 0.33f, 1f, 3f, 5f, 7f };
        static Tuple<int, double>[] tierWeights1 = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 0.5),
            new Tuple<int, double>(1, 1.2),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 2),
            new Tuple<int, double>(4, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static int maxTier1 => tiers1.Length - 2;
        int tierText1 => MaxTier1 - Tier1 + 1;
        static float[] tiers2 = new float[] { 5f, 4.5f, 4f, 3.5f, 3f, 2.5f, 2f };
        static Tuple<int, double>[] tierWeights2 = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 3),
            new Tuple<int, double>(1, 2.5),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 1.5),
            new Tuple<int, double>(4, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static int maxTier2 => tiers2.Length - 2;
        int tierText2 => MaxTier2 - Tier2 + 1;

        static string[] tierNames = new string[] {
            "of Debilitation",
            "of Fatigue",
            "of Stumbling",
            "of Anticipation",
            "of Preparation",
            "of Planning",
        };

        int compoundTier => (Tier1 + Tier2) / 3;
        int maxCompoundTier => (MaxTier1 + MaxTier2) / 3;
        int compoundTierText => MaxCompoundTier - CompoundTier + 1;

        int tier1 = 0;
        float tierMultiplier1 = 0;
        float multiplier1 = 1;
        int tier2 = 0;
        float tierMultiplier2 = 0;
        float multiplier2 = 1;

        double lastProcTime = 0;

        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            float percent1 = Math.Abs((multiplier1 - 1) * 100);
            int decimals1 = 0;
            if (percent1 < 10)
            {
                decimals1 = 2;
            }

            percent1 = (float)Math.Round(percent1, decimals1);
            float cooldown = (float)Math.Round(multiplier2, 1);
            string plusMinus = multiplier1 >= 1 ? "+" : "-";

            return $"Deal {plusMinus}{percent1}% damage ({cooldown}s CD)";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            Hit(item, player, target, ref damageMultiplier);
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit)
        {
            Hit(item, player, target, ref damageMultiplier);
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            Hit(item, player, target, ref damageMultiplier);
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            Hit(item, player, target, ref damageMultiplier);
        }

        void Hit(Item item, Player player, NPC target, ref float damageMultiplier)
        {
            if (item == player.HeldItem && (PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds - lastProcTime) >= multiplier2 * 1000)
            {
                Crit(target, ref damageMultiplier);
            }
        }
        void Hit(Item item, Player player, Player target, ref float damageMultiplier)
        {
            if (item == player.HeldItem && (PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds - lastProcTime) >= multiplier2 * 1000)
            {
                Crit(target, ref damageMultiplier);
            }
        }

        void Crit(NPC target, ref float damageMultiplier)
        {
            damageMultiplier += multiplier1 - 1;
            PoMEffectHelper.Crit(target.position, target.width, target.height, 50);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncCrit(target, 50);
            }
            lastProcTime = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds;
        }
        void Crit(Player target, ref float damageMultiplier)
        {
            damageMultiplier += multiplier1 - 1;
            PoMEffectHelper.Crit(target.position, target.width, target.height, 50);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncCrit(target, 50);
            }
            lastProcTime = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds;
        }

        #region Interface Properties
        public float Weight => weight;

        public string[] TierNames => tierNames;
        public int CompoundTierText => compoundTierText;
        public string AddedTextTiered { get { return AddedTextTiered; } set { addedTextTiered = value; } }
        public float AddedTextWeightTiered { get { return addedTextWeightTiered; } set { addedTextWeightTiered = value; } }
        public int CompoundTier => compoundTier;
        public int MaxCompoundTier => maxCompoundTier;

        public float[] Tiers1 => tiers1;
        public Tuple<int, double>[] TierWeights1 => tierWeights1;
        public int MaxTier1 => maxTier1;
        public int TierText1 => tierText1;

        public float[] Tiers2 => tiers2;
        public Tuple<int, double>[] TierWeights2 => tierWeights2;
        public int MaxTier2 => maxTier2;
        public int TierText2 => tierText2;

        public int Tier1 { get { return tier1; } set { tier1 = value; } }
        public float TierMultiplier1 { get { return tierMultiplier1; } set { tierMultiplier1 = value; } }
        public float Multiplier1 { get { return multiplier1; } set { multiplier1 = value; } }

        public int Tier2 { get { return tier2; } set { tier2 = value; } }
        public float TierMultiplier2 { get { return tierMultiplier2; } set { tierMultiplier2 = value; } }
        public float Multiplier2 { get { return multiplier2; } set { multiplier2 = value; } }
        #endregion
        #region Helped Methods
        void SetTier(int t1, int t2, int t3, bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            TieredAffixHelper.SetTier(this, t1, t2, ignore1, ignore2);
        }
        void SetTierMultiplier(float m1, float m2, bool ignore1 = false, bool ignore2 = false)
        {
            TieredAffixHelper.SetTierMultiplier(this, m1, m2, ignore1, ignore2);
        }
        public override Affix Clone()
        {
            return TieredAffixHelper.Clone(this, (ITieredStatFloat2Affix)base.Clone());
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