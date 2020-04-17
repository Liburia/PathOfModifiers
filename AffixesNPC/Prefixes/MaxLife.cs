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

namespace PathOfModifiers.AffixesNPC.Prefixes
{
    public class MaxLife : Prefix, ITieredStatFloatAffix
    {
        public override float weight => 0.5f;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float[] tiers = new float[] { 0.5f, 0.75f, 1f, 1.25f, 1.5f };
        static Tuple<int, double>[] tierWeights = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 0.5),
            new Tuple<int, double>(1, 1),
            new Tuple<int, double>(2, 3),
            new Tuple<int, double>(3, 1),
        };
        static string[] tierNames = new string[] {
            "Fragile",
            "Pliant",
            "Healthy",
            "Vigorous",
        };
        static int maxTier => tiers.Length - 2;

        int tierText => maxTier - tier + 1;

        int tier = 0;
        string addedTextTiered = string.Empty;
        float addedTextWeightTiered = 1;

        float tierMultiplier = 0;
        float multiplier = 0;


        public string GetTolltipText(Item item)
        {
            string value = Math.Round((Multiplier * 100)).ToString();
            string increasedReduced = multiplier >= 1 ? "increased" : "reduced";
            return $"{value}% {increasedReduced} life";
        }


        public override bool CanBeRolled(PoMNPC pomNPC, NPC npc)
        {
            return true;
        }

        public override void SetDefaults(PoMNPC pomNPC, NPC npc)
        {
            npc.lifeMax = (int)Math.Round(npc.lifeMax * multiplier);
            npc.life = (int)Math.Round(npc.life * multiplier);
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
        public float Multiplier { get { return multiplier; } set { this.multiplier = value; } }
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
        public override void Save(TagCompound tag)
        {
            TieredAffixHelper.Save(this, tag);
        }
        public override void Load(TagCompound tag)
        {
            TieredAffixHelper.Load(this, tag);
        }
        public override void NetSend(BinaryWriter writer)
        {
            TieredAffixHelper.NetSend(this, writer);
        }
        public override void NetReceive(BinaryReader reader)
        {
            TieredAffixHelper.NetReceive(this, reader);
        }
        #endregion
    }
}
