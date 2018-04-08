using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;

namespace PathOfModifiers.Rarities
{
    public class AccessoryCommon : Rarity
    {
        public override float weight => 0.5f;
        public override byte minAffixes => 0;
        public override byte maxAffixes => 0;
        public override byte maxPrefixes => 0;
        public override byte maxSuffixes => 0;
        public override float chanceToRollAffix => 0.1f;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override string name => "Common";

        public override bool CanBeRolled(Item item)
        {
            return RarityHelper.CanRollAccessory(item);
        }
    }
}
