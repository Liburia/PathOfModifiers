using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace PathOfModifiers.Affixes.Prefixes
{
    public class WeaponAttackSpeedPerLife : Prefix, ITieredStatAffix
    {
        public override float weight => 0.5f;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float[] tiers = new float[] { 0.99f, 0.993f, 0.997f, 1f, 1.003f, 1.007f, 1.01f };
        static Tuple<int, double>[] tierWeights = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 0.5),
            new Tuple<int, double>(1, 1),
            new Tuple<int, double>(2, 2),
            new Tuple<int, double>(3, 2),
            new Tuple<int, double>(4, 1),
            new Tuple<int, double>(5, 0.5),
        };
        static string[] tierNames = new string[] {
            "Renowned",
            "Notable",
            "Inadequate",
            "Amateur",
            "Proficient",
            "Skilled",
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
                !PoMItem.IsSpear(item) &&
                !PoMItem.IsFlailOrYoyo(item);
        }

        public override void UseTimeMultiplier(Item item, Player player, ref float multiplier)
        {
            multiplier += (this.multiplier - 1) * ((((float)player.statLife / player.statLifeMax2) * 100) - 50);
        }

        public override void ModifyTooltips(Mod mod, Item item, List<TooltipLine> tooltips)
        {
            float percent = (int)Math.Round(Math.Abs((multiplier - 1) * 5000));
            TooltipLine line = new TooltipLine(mod, "WeaponAttackSpeedPerLife", $"[T{tierText}] Up to {(multiplier < 1 ? '-' : '+')}{percent}% attack speed above 50% life and up to {(multiplier < 1 ? '+' : '-')}{percent}% below");
            line.overrideColor = color;
            tooltips.Add(line);
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
            return TieredAffixHelper.Clone(this, (ITieredStatAffix)base.Clone());
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
