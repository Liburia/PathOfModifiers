using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public class AccessoryCommon : RarityItem
    {
        public AccessoryCommon(Mod mod) : base(mod) { }

        public override double Weight { get; } = 0.5;
        public override byte minAffixes => 0;
        public override byte maxAffixes => 0;
        public override byte maxPrefixes => 0;
        public override byte maxSuffixes => 0;
        public override float chanceToRollAffix => 0.1f;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override string name => "Common";
        public override int forgeCost => 1;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollAccessory(item);
        }
    }
}
