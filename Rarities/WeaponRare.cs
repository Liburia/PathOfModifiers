using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class WeaponRare : RarityItem
    {
        public override double Weight => 0.5;
        public override byte minAffixes => 2;
        public override byte maxAffixes => 4;
        public override byte maxPrefixes => 3;
        public override byte maxSuffixes => 1;
        public override float chanceToRollAffix => 0.6f;
        public override Color color => new Color(0.149f, 0.388f, 0.827f, 1f);
        public override int vanillaRarity => 1;
        public override string name => "Rare";
        public override int forgeCost => 3;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollWeapon(item);
        }
    }
}
