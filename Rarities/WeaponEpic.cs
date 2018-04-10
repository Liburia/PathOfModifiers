using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class WeaponEpic : Rarity
    {
        public override float weight => 0.2f;
        public override byte minAffixes => 3;
        public override byte maxAffixes => 6;
        public override byte maxPrefixes => 4;
        public override byte maxSuffixes => 3;
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
