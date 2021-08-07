using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public class ItemNone : RarityItem
    {
        public ItemNone(Mod mod) : base(mod) { }

        public override double Weight { get; } = 0;
        public override byte minAffixes => 0;
        public override byte maxAffixes => 0;
        public override byte maxPrefixes => 0;
        public override byte maxSuffixes => 0;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override float chanceToRollAffix => 0;
    }
}
