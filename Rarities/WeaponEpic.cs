﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public class WeaponEpic : RarityItem
    {
        public WeaponEpic(Mod mod) : base(mod) { }

        public override double Weight { get; } = 0.2;
        public override byte minAffixes => 3;
        public override byte maxAffixes => 6;
        public override byte maxPrefixes => 4;
        public override byte maxSuffixes => 2;
        public override float chanceToRollAffix => 0.7f;
        public override Color color => new Color(0.741f, 0f, 0.702f, 1f);
        public override int vanillaRarity => 11;
        public override string name => "Epic";
        public override int forgeCost => 4;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollWeapon(item);
        }
    }
}
