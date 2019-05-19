using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public class NPCCommon : RarityNPC
    {
        public override float weight => 0.5f;
        public override byte minAffixes => 0;
        public override byte maxAffixes => 1;
        public override byte maxPrefixes => 1;
        public override byte maxSuffixes => 0;
        public override float chanceToRollAffix => 0.4f;
        public override Color color => new Color(1f, 1f, 1f, 1f);
        public override int vanillaRarity => 0;
        public override string name => "";

        public override bool CanBeRolled(NPC npc)
        {
            return RarityHelper.CanRollNPC(npc);
        }
    }
}
