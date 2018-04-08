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

namespace PathOfModifiers.Affixes.Prefixes
{
    public class EquipDefense : Prefix, ITieredStatIntValueAffix
    {
        public override float weight => 0.5f;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static int[] tiers = new int[] { -3, -2, -1, 1, 2, 3, 4 };
        static Tuple<int, double>[] tierWeights = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 0.5),
            new Tuple<int, double>(1, 1.2),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 2),
            new Tuple<int, double>(4, 1.5),
            new Tuple<int, double>(5, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static string[] tierNames = new string[] {
            "Soft",
            "Loose",
            "Weakened",
            "Studded",
            "Layered",
            "Reinforced",
            "Girded",
        };
        static int maxTier => tiers.Length - 2;

        int tierText => maxTier - tier + 1;

        int tier = 0;
        string addedTextTiered = string.Empty;
        float addedTextWeightTiered = 1;


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAnyArmor(item) ||
                PoMItem.IsAccessory(item);
        }

        public override void ModifyTooltips(Mod mod, Item item, List<TooltipLine> tooltips)
        {
            int value = tiers[tier];
            TooltipLine line = new TooltipLine(mod, "EquipDefense", $"[T{tierText}] {(value < 0 ? '-' : '+')}{Math.Abs(value)} defense");
            line.overrideColor = color;
            tooltips.Add(line);
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.statDefense += tiers[tier];
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
        public override void RollValue()
        {
            TieredAffixHelper.RollValue(this);
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
