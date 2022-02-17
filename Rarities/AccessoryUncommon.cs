using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public class AccessoryUncommon : RarityItem
    {
        public AccessoryUncommon(Mod mod) : base(mod) { }

        public override double Weight { get; } = 1;
        public override byte minAffixes => 1;
        public override byte maxAffixes => 1;
        public override byte maxPrefixes => 1;
        public override byte maxSuffixes => 0;
        public override float chanceToRollAffix => 0.2f;
        public override Color color => new Color(0.208f, 0.859f, 0.255f, 1f);
        public override int vanillaRarity => 2;
        public override string name => "Uncommon";
        public override int forgeCost => 2;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollAccessory(item);
        }
    }
}
