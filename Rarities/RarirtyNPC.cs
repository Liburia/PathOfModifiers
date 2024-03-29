﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public abstract class RarityNPC
    {
        public Mod mod;

        public virtual double Weight => 0;
        public virtual byte minAffixes => 0;
        public virtual byte maxAffixes => 0;
        public virtual byte maxPrefixes => 0;
        public virtual byte maxSuffixes => 0;
        public virtual Color color => new Color(1, 1, 1, 1);
        public virtual int vanillaRarity => 0;
        public virtual string name => string.Empty;
        public virtual float chanceToRollAffix => 0;


        public virtual bool CanBeRolled(NPC npc)
        {
            return false;
        }
    }
}
