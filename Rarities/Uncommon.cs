using Microsoft.Xna.Framework;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class Uncommon : Rarity
    {
        public override float weight => 1f;
        public override byte maxAffixes => 2;
        public override byte maxPrefixes => 2;
        public override byte maxSuffixes => 1;
        public override Color color => new Color(0.208f, 0.859f, 0.255f, 1f);
        public override int vanillaRarity => 2;
        public override string name => "Uncommon";
        public override float chanceToRollAffix => 0.6f;
    }
}
