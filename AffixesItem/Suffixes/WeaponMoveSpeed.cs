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

namespace PathOfModifiers.AffixesItem.Suffixes
{
    public class WeaponMoveSpeed : Suffix, ITieredStatFloat3Affix
    {
        public override float weight => 0.5f;

        string addedTextTiered = string.Empty;
        float addedTextWeightTiered = 1;
        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float[] tiers1 = new float[] { 1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f };
        static Tuple<int, double>[] tierWeights1 = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 3),
            new Tuple<int, double>(1, 2.5),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 1.5),
            new Tuple<int, double>(4, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static int maxTier1 => tiers1.Length - 2;
        int tierText1 => MaxTier1 - Tier1 + 1;
        static float[] tiers2 = new float[] { 0f, 0.5f, 1f, 1.5f, 2f, 2.5f, 3f };
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
        static float[] tiers3 = new float[] { 6f, 5f, 4f, 3f, 2f, 1f, 0f };
        static Tuple<int, double>[] tierWeights3 = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 3),
            new Tuple<int, double>(1, 2.5),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 1.5),
            new Tuple<int, double>(4, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static int maxTier3 => tiers3.Length - 2;
        int tierText3 => MaxTier3 - Tier3 + 1;

        static string[] tierNames = new string[] {
            "of Spurring",
            "of Hurrying",
            "of Quickening",
            "of Hastening",
            "of Stimulation",
            "of Rushing",
        };

        int compoundTier => (Tier1 + Tier2 + Tier3) / 3;
        int maxCompoundTier => (MaxTier1 + MaxTier2 + MaxTier3) / 3;
        int compoundTierText => MaxCompoundTier - CompoundTier + 1;

        int tier1 = 0;
        float tierMultiplier1 = 0;
        float multiplier1 = 1;
        int tier2 = 0;
        float tierMultiplier2 = 0;
        float multiplier2 = 1;
        int tier3 = 0;
        float tierMultiplier3 = 0;
        float multiplier3 = 1;

        double lastProc = 0;

        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            float percent1 = Math.Abs((Multiplier1 - 1) * 100);
            float percent2 = (float)Math.Round(Multiplier2, 1);
            float percent3 = (float)Math.Round(Multiplier3, 1);

            int decimals1 = 0;
            if (percent1 < 10)
            {
                decimals1 = 2;
            }
            percent1 = (float)Math.Round(percent1, decimals1);

            string plusMinus = multiplier3 >= 1 ? "+" : "-";

            return $"Gain {plusMinus}{percent1}% move speed on hit for {percent2}s ({percent3}s CD)";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            OnHit(item, player);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            OnHit(item, player);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(item, player);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            OnHit(item, player);
        }

        void OnHit(Item item, Player player)
        {
            if (item == player.HeldItem && (PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds - lastProc) / 1000.0 >= multiplier3)
                GainMoveSpeed(player);
        }

        void GainMoveSpeed(Player player)
        {
            int duration = (int)MathHelper.Clamp(multiplier2 * 60, 1, 9999999);
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.AddMoveSpeedBuff(player, multiplier1, duration);
            lastProc = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds;
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

        public float[] Tiers3 => tiers3;
        public Tuple<int, double>[] TierWeights3 => tierWeights3;
        public int MaxTier3 => maxTier3;
        public int TierText3 => tierText3;

        public int Tier1 { get { return tier1; } set { tier1 = value; } }
        public float TierMultiplier1 { get { return tierMultiplier1; } set { tierMultiplier1 = value; } }
        public float Multiplier1 { get { return multiplier1; } set { multiplier1 = value; } }

        public int Tier2 { get { return tier2; } set { tier2 = value; } }
        public float TierMultiplier2 { get { return tierMultiplier2; } set { tierMultiplier2 = value; } }
        public float Multiplier2 { get { return multiplier2; } set { multiplier2 = value; } }

        public int Tier3 { get { return tier3; } set { tier3 = value; } }
        public float TierMultiplier3 { get { return tierMultiplier3; } set { tierMultiplier3 = value; } }
        public float Multiplier3 { get { return multiplier3; } set { multiplier3 = value; } }
        #endregion
        #region Helped Methods
        void SetTier(int t1, int t2, int t3, bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            TieredAffixHelper.SetTier(this, t1, t2, t3, ignore1, ignore2, ignore3);
        }
        void SetTierMultiplier(float m1, float m2, float m3, bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            TieredAffixHelper.SetTierMultiplier(this, m1, m2, m3, ignore1, ignore2, ignore3);
        }
        public override Affix Clone()
        {
            return TieredAffixHelper.Clone(this, (ITieredStatFloat3Affix)base.Clone());
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