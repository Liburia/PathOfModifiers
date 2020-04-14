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
    //TODO: Remove, this can't roll and is replaces with WeaponOnHitFireball
    public class WeaponFireballRelease : Suffix, ITieredStatFloatAffix
    {
        public override float weight => 0f;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float damageMultiplier = 0.2f;

        static float[] tiers = new float[] { 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f };
        static Tuple<int, double>[] tierWeights = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 3),
            new Tuple<int, double>(1, 2.5),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 1.5),
            new Tuple<int, double>(4, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static string[] tierNames = new string[] {
            "of Fire",
            "of Flame",
            "of Blaze",
            "of Conflagration",
            "of Searing",
            "of Scorching",
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
                PoMItem.IsWeapon(item) &&
                PoMItem.CanMelee(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"[T{tierText}] {(int)Math.Round(multiplier * 100)}% chance to fire fireballs on hit";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < multiplier)
                SpawnFireballs(player, target, target.Center, target.Center - player.Center, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < multiplier)
                SpawnFireballs(player, target, target.Center, target.Center - player.Center, damage);
        }

        void SpawnFireballs(Player player, Entity target, Vector2 position, Vector2 direction, int damage)
        {
            float angleMin = direction.ToRotation() - 0.4f;
            float angleMax = angleMin + 0.8f;

            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = Main.rand.NextFloat(angleMin, angleMax).ToRotationVector2() * 8;
                int bladeID = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<Fireball>(), (int)Math.Round(damage * damageMultiplier), 0, player.whoAmI);
                Fireball fireball = (Fireball)Main.projectile[bladeID].modProjectile;
                fireball.ignoreTarget = target;
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
        #endregion
    }
}