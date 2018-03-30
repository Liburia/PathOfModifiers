using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class ArmorUncommon : Rarity
    {
        public override float weight => 1f;
        public override byte maxAffixes => 1;
        public override byte maxPrefixes => 1;
        public override byte maxSuffixes => 1;
        public override float chanceToRollAffix => 0.4f;
        public override Color color => new Color(0.208f, 0.859f, 0.255f, 1f);
        public override int vanillaRarity => 2;
        public override string name => "Uncommon";

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollArmor(item);
        }
    }
}
