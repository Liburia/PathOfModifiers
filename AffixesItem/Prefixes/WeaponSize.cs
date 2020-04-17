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

namespace PathOfModifiers.AffixesItem.Prefixes
{
    public class WeaponSize : Prefix, ITieredStatFloatAffix
    {
        public override float weight => 0.5f;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float[] tiers = new float[] { 0.7f, 0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f };
        static Tuple<int, double>[] tierWeights = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 0.5),
            new Tuple<int, double>(1, 1.2),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 2),
            new Tuple<int, double>(4, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static string[] tierNames = new string[] {
            "Tiny",
            "Small",
            "Modest",
            "Bulky",
            "Large",
            "Massive",
        };
        static int maxTier => tiers.Length - 2;

        int tierText => maxTier - tier + 1;

        int tier = 0;
        string addedTextTiered = string.Empty;
        float addedTextWeightTiered = 1;

        float tierMultiplier = 0;
        float multiplier = 1;
        float baseScale = -1;


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item) &&
                PoMItem.CanMelee(item) &&
                (PoMItem.IsSwinging(item) || PoMItem.IsStabbing(item));
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(multiplier < 1 ? '-' : '+')}{(int)Math.Round(Math.Abs((multiplier - 1) * 100))}% size";
        }

        public override void UseItem(Item item, Player player)
        {
            item.scale = baseScale * multiplier;
        }

        /// <summary>
        /// Called when this affix is added to an item
        /// </summary>
        public override void AddAffix(Item item, bool clone)
        {
            if (!clone)
                baseScale = item.scale;
        }
        /// <summary>
        /// Called when this affix is removed from an item
        /// </summary>
        public override void RemoveAffix(Item item)
        {
            item.scale = baseScale;
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
        public override void RollValue(bool rollTier = true)
        {
            TieredAffixHelper.RollValue(this, rollTier);
        }
        public override void ReforgePrice(Item item, ref int price)
        {
            TieredAffixHelper.ReforgePrice(this, item, ref price);
        }
        public override Affix Clone()
        {
            WeaponSize newAffix = (WeaponSize)TieredAffixHelper.Clone(this, (ITieredStatFloatAffix)base.Clone());
            newAffix.baseScale = baseScale;
            return newAffix;
        }
        public override void Save(TagCompound tag, Item item)
        {
            TieredAffixHelper.Save(this, tag, item);
            tag.Set("baseScale", baseScale);
        }
        public override void Load(TagCompound tag, Item item)
        {
            TieredAffixHelper.Load(this, tag, item);
            baseScale = tag.GetFloat("baseScale");
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            TieredAffixHelper.NetSend(this, item, writer);
            writer.Write(baseScale);
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            TieredAffixHelper.NetReceive(this, item, reader);
            baseScale = reader.ReadSingle();
        }
        public override string GetForgeText(Item item)
        {
            return TieredAffixHelper.GetForgeText(this, item);
        }
        #endregion
    }
}
