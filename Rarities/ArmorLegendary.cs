﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public class ArmorLegendary : RarityItem
    {
        public ArmorLegendary(Mod mod) : base(mod) { }

        public override double Weight { get; } = 0.1;
        public override byte minAffixes => 2;
        public override byte maxAffixes => 3;
        public override byte maxPrefixes => 2;
        public override byte maxSuffixes => 2;
        public override float chanceToRollAffix => 0.7f;
        public override Color color => new Color(0.957f, 0.443f, 0f, 1f);
        public override int vanillaRarity => -11;
        public override string name => "Legendary";
        public override int forgeCost => 5;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollArmor(item);
        }
    }
}
