﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public abstract class RarityItem
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
        public virtual float chanceToRollPrefixInsteadOfSuffix => 0.7f;
        public virtual int forgeCost => 0;

        public RarityItem(Mod mod)
        {
            this.mod = mod;
        }

        public virtual bool CanBeRolled(Item item)
        {
            return false;
        }
    }
}
