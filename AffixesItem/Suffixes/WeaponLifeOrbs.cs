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
    public class WeaponLifeOrbs : Suffix, ITieredStatFloat2IntValueAffix
    {
        public override float weight => 0.5f;

        string addedTextTiered = string.Empty;
        float addedTextWeightTiered = 1;
        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float[] tiers1 = new float[] { 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f };
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
        static float[] tiers2 = new float[] { 0f, 0.0016f, 0.0033f, 0.005f, 0.0066f, 0.0083f, 0.001f };
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
        static int[] tiers3 = new int[] { 1, 2, 3, 4, 5, 6 };
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
            "of Blood",
            "of Bloodthirst",
            "of Crimson",
            "of Scarlet",
            "of Sanguine",
            "of Vampirism",
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

        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            float percent1 = multiplier1 * 100;
            float percent2 = multiplier2 * 100;

            int decimals1 = 0;
            int decimals2 = 0;

            if (percent1 < 10)
            {
                decimals1 = 2;
            }
            if (percent2 < 10)
            {
                decimals2 = 2;
            }

            percent1 = (float)Math.Round(percent1, decimals1);
            percent2 = (float)Math.Round(percent2, decimals2);

            return $"{percent1}% chance to release {Tiers3[Tier3]} life orbs on hit that heal {percent2}% of damage dealt";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < multiplier1)
                SpawnLifeOrbs(player, target, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < multiplier1)
                SpawnLifeOrbs(player, target, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < multiplier1)
                SpawnLifeOrbs(player, target, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < multiplier1)
                SpawnLifeOrbs(player, target, damage);
        }

        void SpawnLifeOrbs(Player player, Entity target, int damage)
        {
            int projectileNumber = Tiers3[Tier3];
            for (int i = 0; i < projectileNumber; i++)
            {
                Vector2 direction = Main.rand.NextVector2Unit();
                Vector2 velocity = direction * Main.rand.NextFloat(5, 10);
                Vector2 projTarget = player.Center + Main.rand.NextVector2Circular(player.width * 1.5f, player.height * 1.5f);
                int heal = (int)MathHelper.Clamp(damage * multiplier2, 1, 999999);
                Projectile.NewProjectile(target.Center, velocity, ModContent.ProjectileType<LifeOrb>(), heal, 0, player.whoAmI, projTarget.X, projTarget.Y);
            }
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

        public int[] Tiers3 => tiers3;
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
        #endregion
        #region Helped Methods
        void SetTier(int t1, int t2, int t3, bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            TieredAffixHelper.SetTier(this, t1, t2, t3, ignore1, ignore2, ignore3);
        }
        void SetTierMultiplier(float m1, float m2, bool ignore1 = false, bool ignore2 = false)
        {
            TieredAffixHelper.SetTierMultiplier(this, m1, m2, ignore1, ignore2);
        }
        public override Affix Clone()
        {
            return TieredAffixHelper.Clone(this, (ITieredStatFloat2IntValueAffix)base.Clone());
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