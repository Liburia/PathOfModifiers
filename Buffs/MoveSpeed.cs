using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;

namespace PathOfModifiers.Buffs
{
    public class MoveSpeed : ModBuff
    {
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

        public override void Update(Player player, ref int buffIndex)
        {
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.moveSpeedBuff = true;
        }
    }
}
