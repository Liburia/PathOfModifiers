using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public class ArmorEpic : RarityItem
    {
        public ArmorEpic(Mod mod) : base(mod) { }

        public override double Weight { get; } = 0.2;
        public override byte minAffixes => 1;
        public override byte maxAffixes => 2;
        public override byte maxPrefixes => 2;
        public override byte maxSuffixes => 1;
        public override float chanceToRollAffix => 0.6f;
        public override Color color => new Color(0.741f, 0f, 0.702f, 1f);
        public override int vanillaRarity => 11;
        public override string name => "Epic";
        public override int forgeCost => 4;

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollArmor(item);
        }
    }
}
