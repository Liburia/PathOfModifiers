using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace PathOfModifiers.Buffs
{
    public class StaticStrike : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_15";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("StaticStrike");
            Description.SetDefault("Hitting things around you periodically");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //player.GetModPlayer<BuffPlayer>().staticStrikeBuff = true;
        }
    }
}
