using Microsoft.Xna.Framework;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class Rare : Rarity
    {
        public override float weight => 0.5f;
        public override byte maxAffixes => 4;
        public override byte maxPrefixes => 3;
        public override byte maxSuffixes => 2;
        public override Color color => new Color(0.141f, 0.160f, 0.761f, 1f);
        public override int vanillaRarity => 1;
        public override string name => "Rare";
        public override float chanceToRollAffix => 0.7f;
    }
}
