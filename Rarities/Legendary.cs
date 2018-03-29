using Microsoft.Xna.Framework;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class Legendary : Rarity
    {
        public override float weight => 0.1f;
        public override byte maxAffixes => 7;
        public override byte maxPrefixes => 5;
        public override byte maxSuffixes => 4;
        public override Color color => new Color(1f, 0.274f, 0f, 1f);
        public override int vanillaRarity => -11;
        public override string name => "Legendary";
        public override float chanceToRollAffix => 0.9f;
    }
}
