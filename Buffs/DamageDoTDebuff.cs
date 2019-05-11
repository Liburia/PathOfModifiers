using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;

namespace PathOfModifiers.Buffs
{
	public class DamageDoTDebuff : ModBuff
	{
        //TODO: Not referenced anywhere, remove?
        public static float damageMultiplier => 0.5f;
        public static float damageMultiplierHalfSecond => 2.0f;

        //TODO: Actually load it
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Terraria/Buff_20";
            return true;
        }

        public override void SetDefaults()
		{
			DisplayName.SetDefault("DamageDoTDebuff");
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
            pomNPC.dddDamageDotDebuff = true;
        }
        public override void Update(Player player, ref int buffIndex)
		{
			PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.dddDamageDotDebuff = true;
        }
	}
}
