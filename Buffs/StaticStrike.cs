using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;

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
            CanBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //player.GetModPlayer<BuffPlayer>().staticStrikeBuff = true;
        }
    }
}
