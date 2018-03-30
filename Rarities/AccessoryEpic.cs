using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class AccessoryEpic : Rarity
    {
        public override float weight => 0.2f;
        public override byte maxAffixes => 2;
        public override byte maxPrefixes => 2;
        public override byte maxSuffixes => 1;
        public override float chanceToRollAffix => 0.4f;
        public override Color color => new Color(0.741f, 0f, 0.702f, 1f);
        public override int vanillaRarity => 11;
        public override string name => "Epic";

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollAccessory(item);
        }
    }
}
