using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;

namespace PathOfModifiers.Buffs
{
    public class Shocked : StackingDamageOverTime
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Terraria/Buff_20";   //poison
            return true;
        }

        public override void SetDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Damage taken is modified");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.isShocked = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.isShocked = true;
        }
    }
}
