using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class WeaponUncommon : RarityItem
    {
        public WeaponUncommon(Mod mod) : base(mod) { }

        public override double Weight { get; } = 1;
        public override byte minAffixes => 1;
        public override byte maxAffixes => 2;
        public override byte maxPrefixes => 2;
        public override byte maxSuffixes => 1;
        public override float chanceToRollAffix => 0.5f;
        public override Color color => new Color(0.208f, 0.859f, 0.255f, 1f);
        public override int vanillaRarity => 2;
        public override string name => "Uncommon";
        public override int forgeCost => 2;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollWeapon(item);
        }
    }
}
