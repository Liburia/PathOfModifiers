using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;

namespace PathOfModifiers.Buffs
{
    public class DamageOverTime : ModBuff
    {
        /// <summary>
        /// Multiply DPS by this to get damage per 0.5s
        /// </summary>
        public static float damageMultiplierHalfSecond => 2.0f;

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Terraria/Buff_20";
            return true;
        }

        public override void SetDefaults()
        {
            //TODO: Need this to equal classname because I'm lazy, look at other todos for detail
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Taking damage over time");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = true;
        }
    }
    public class StackingDamageOverTime : DamageOverTime
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Terraria/Buff_20";
            return true;
        }

        public override void SetDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Taking stacking damage over time");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = true;
        }
    }
}
