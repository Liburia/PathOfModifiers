using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ID;
using System.IO;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace PathOfModifiers.AffixesNPC
{
    public class Affix
    {
        public Mod mod;

        public virtual float weight => 1;
        public virtual Color color => new Color(1, 1, 1, 1);

        public virtual string addedText => string.Empty;
        public virtual float addedTextWeight => 1;

        public virtual void InitializeNPC(PoMNPC npc)
        {
            RollValue();
        }

        public virtual Affix Clone()
        {
            Affix newAffix = (Affix)Activator.CreateInstance(GetType());
            newAffix.mod = mod;
            return newAffix;
        }

        public virtual bool CanBeRolled(PoMNPC pomNPC, NPC npc) { return true; }

        public virtual void RollValue(bool rollTier = true) { }
        #region NPC Hooks
        public virtual void test(NPC npc, Player player, ref float multiplier) { }
        #endregion
        #region Projectile Hooks
        #endregion
        #region Player Hooks
        #endregion

        public virtual string GetDescriptionText(NPC npc) { return string.Empty; }

        public virtual void Save(TagCompound tag, NPC npc)
        {
        }
        public virtual void Load(TagCompound tag, NPC npc)
        {
        }

        public virtual void NetSend(NPC npc, BinaryWriter writer)
        {
        }
        public virtual void NetReceive(NPC npc, BinaryReader reader)
        {
        }

        public virtual void AddAffix(NPC npc, bool clone) { }
        public virtual void RemoveAffix(NPC npc) { }
    }

    public class Prefix : Affix
    {
        public override Color color => new Color(0.25f, 1, 0.25f, 1);
    }

    public class Suffix : Affix
    {
        public override Color color => new Color(0.25f, 1, 1, 1);
    }
}
