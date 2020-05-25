using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria;

namespace PathOfModifiers.Rarities
{
    public class MapCommon : RarityItem
    {
        public override double Weight { get; } = 0.5;
        public override byte minAffixes => 1;
        public override byte maxAffixes => 3;
        public override byte maxPrefixes => 2;
        public override byte maxSuffixes => 1;
        public override float chanceToRollAffix => 0.4f;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override string name => "Common";
        public override int forgeCost => 1;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollMap(item);
        }
    }
}
