using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;

namespace PathOfModifiers.Buffs
{
    public class Bleed : DamageOverTime
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Terraria/Buff_30";
            return true;
        }

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Bleeding");
            Description.SetDefault("Taking damage over time");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.dotBuffActive = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.dotBuffActive = true;
        }
    }
}
