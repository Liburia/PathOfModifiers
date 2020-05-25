using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class AccessoryRare : RarityItem
    {
        public override double Weight { get; } = 0.5;
        public override byte minAffixes => 1;
        public override byte maxAffixes => 1;
        public override byte maxPrefixes => 1;
        public override byte maxSuffixes => 1;
        public override float chanceToRollAffix => 0.3f;
        public override Color color => new Color(0.149f, 0.388f, 0.827f, 1f);
        public override int vanillaRarity => 1;
        public override string name => "Rare";
        public override int forgeCost => 3;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollAccessory(item);
        }
    }
}
