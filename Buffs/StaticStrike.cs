using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;

namespace PathOfModifiers.Buffs
{
    public class StaticStrike : ModBuff
    {
        //TODO: Actually load it
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Terraria/Buff_15";
            return true;
        }

        public override void SetDefaults()
        {
            DisplayName.SetDefault("StaticStrike");
            Description.SetDefault("Hitting things around you periodically");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<PoMPlayer>().staticStrikeBuff = true;
        }
    }
}
