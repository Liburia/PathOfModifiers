using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class WeaponLegendary : RarityItem
    {
        public override double Weight => 0.1;
        public override byte minAffixes => 4;
        public override byte maxAffixes => 7;
        public override byte maxPrefixes => 5;
        public override byte maxSuffixes => 3;
        public override float chanceToRollAffix => 0.8f;
        public override Color color => new Color(0.957f, 0.443f, 0f, 1f);
        public override int vanillaRarity => -11;
        public override string name => "Legendary";
        public override int forgeCost => 5;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollWeapon(item);
        }
    }
}
