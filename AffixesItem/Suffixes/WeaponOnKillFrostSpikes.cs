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
    public class WeaponOnKillFrostSpikes : Suffix, ITieredStatFloatAffix
    {
        public override float weight => 0.5f;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static int spikeCount = 8;

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
            "of Chill",
            "of Frost",
            "of Ice",
            "of Winter",
            "of Glacier",
            "of Arctic",
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
            return $"[T{tierText}] {(int)Math.Round(multiplier * 100)}% chance to release frost spikes on kill";
        }

        public override void PlayerOnKillNPC(Item item, Player player, NPC target)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 0) < multiplier)
                SpawnFrostSpikes(player, target);
        }
        public override void PlayerOnKillPvp(Item item, Player player, Player target)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 0) < multiplier)
                SpawnFrostSpikes(player, target);
        }

        void SpawnFrostSpikes(Player player, Entity target)
        {
            float angle = Main.rand.NextFloatDirection();
            float angleIncrement = (float)Math.PI * 2 / spikeCount;
            int targetLife = 0;
            NPC targetNPC = target as NPC;
            if (targetNPC != null)
            {
                targetLife = (int)Math.Round(Math.Sqrt(targetNPC.lifeMax));
            }
            else
            {
                Player targetPlayer = target as Player;
                if (targetPlayer != null)
                {
                    targetLife = (int)Math.Round(Math.Sqrt(targetPlayer.statLifeMax2));
                }
            }
            int damage = (int)Math.Round(Math.Sqrt(targetLife));
            int knockback = (int)Math.Round(Math.Sqrt(damage));

            for (int i = 0; i < spikeCount; i++)
            {
                Vector2 velocity = angle.ToRotationVector2() * 8;
                int projectileID = Projectile.NewProjectile(target.Center, velocity, ModContent.ProjectileType<FrostSpike>(), damage, knockback, player.whoAmI);
                angle += angleIncrement;
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