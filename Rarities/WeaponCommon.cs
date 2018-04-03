﻿using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria;

namespace PathOfModifiers.Rarities
{
    public class WeaponCommon : Rarity
    {
        public override float weight => 0.5f;
        public override byte maxAffixes => 1;
        public override byte maxPrefixes => 1;
        public override byte maxSuffixes => 0;
        public override float chanceToRollAffix => 0.4f;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override string name => "Common";

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollWeapon(item);
        }
    }
}