using Microsoft.Xna.Framework;
using Terraria;

namespace PathOfModifiers.Rarities
{
    public class NPCNone : RarityNPC
    {
        public override double Weight { get; } = 0;
        public override byte minAffixes => 0;
        public override byte maxAffixes => 0;
        public override byte maxPrefixes => 0;
        public override byte maxSuffixes => 0;
        public override float chanceToRollAffix => 0;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override string name => "";

        public override bool CanBeRolled(NPC npc)
        {
            return false;
        }
    }
}
