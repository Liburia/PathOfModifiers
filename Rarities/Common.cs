using Microsoft.Xna.Framework;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class Common : Rarity
    {
        public override float weight => 0.5f;
        public override byte maxAffixes => 1;
        public override byte maxPrefixes => 1;
        public override byte maxSuffixes => 0;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override string name => "Common";
        public override float chanceToRollAffix => 0.5f;
    }
}
