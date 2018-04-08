using Microsoft.Xna.Framework;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class None : Rarity
    {
        public override float weight => 0f;
        public override byte minAffixes => 0;
        public override byte maxAffixes => 0;
        public override byte maxPrefixes => 0;
        public override byte maxSuffixes => 0;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override float chanceToRollAffix => 0;
    }
}
