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

namespace PathOfModifiers.AffixesItem.Suffixes
{
    public class WeaponMultipleProjectiles : Suffix, ITieredStatIntValueAffix
    {
        public override float weight => 0;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static int[] tiers = new int[] { 2, 3, 4, 5, 6 };
        static float damageMultiplierPerProjectile = 0.6f;
        static Tuple<int, double>[] tierWeights = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 3),
            new Tuple<int, double>(1, 2.5),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 1.5),
            new Tuple<int, double>(4, 1),
        };
        static string[] tierNames = new string[] {
            "of Viper",
            "of Noxiousness",
            "of Venom",
            "of Lethality",
            "of Virulence",
        };
        static int maxTier => tiers.Length - 1;

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
            return $"[T{tierText}] {(int)Math.Round(multiplier * 100)}% chance to TEST";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.HeldItem == item)
            {
                if (Main.rand.NextFloat(0, 1) < multiplier)
                {
                    target.AddBuff(70, Main.rand.Next(90, 330), false);
                }
            }
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            if (player.HeldItem == item)
            {
                if (Main.rand.NextFloat(0, 1) < multiplier)
                {
                    target.AddBuff(70, Main.rand.Next(90, 330), false);
                }
            }
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (player.HeldItem == item)
            {
                if (Main.rand.NextFloat(0, 1) < multiplier)
                {
                    target.AddBuff(70, Main.rand.Next(90, 330), false);
                }
            }
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            if (player.HeldItem == item)
            {
                if (Main.rand.NextFloat(0, 1) < multiplier)
                {
                    target.AddBuff(70, Main.rand.Next(90, 330), false);
                }
            }
        }

        public override bool PlayerShoot(Item affixItem, Player player, Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num219 = Projectile.NewProjectile(position.X, position.Y, -speedX, -speedY, type, (int)Math.Round(damage * Math.Pow(damageMultiplierPerProjectile, Tiers[Tier] - 1)), knockBack, player.whoAmI, 0f, 0f);
            if (item.type == 726)
            {
                Main.projectile[num219].magic = true;
            }
            if (item.type == 724 || item.type == 676)
            {
                Main.projectile[num219].melee = true;
            }
            if (type == 80)
            {
                Main.projectile[num219].ai[0] = (float)Player.tileTargetX;
                Main.projectile[num219].ai[1] = (float)Player.tileTargetY;
            }
            if (type == 442)
            {
                Main.projectile[num219].ai[0] = (float)Player.tileTargetX;
                Main.projectile[num219].ai[1] = (float)Player.tileTargetY;
            }
            if ((player.thrownCost50 || player.thrownCost33) && item.thrown)
            {
                Main.projectile[num219].noDropItem = true;
            }

            return true;
        }

        #region Interface Properties
        public float Weight => weight;
        public int[] Tiers => tiers;
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
        public override Affix Clone()
        {
            return TieredAffixHelper.Clone(this, (ITieredStatIntValueAffix)base.Clone());
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
        #endregion
    }
}
