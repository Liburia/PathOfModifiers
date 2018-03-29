using Microsoft.Xna.Framework;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class Epic : Rarity
    {
        public override float weight => 0.2f;
        public override byte maxAffixes => 6;
        public override byte maxPrefixes => 4;
        public override byte maxSuffixes => 3;
        public override Color color => new Color(0.741f, 0f, 0.702f, 1f);
        public override int vanillaRarity => 11;
        public override string name => "Epic";
        public override float chanceToRollAffix => 0.8f;
    }
}
