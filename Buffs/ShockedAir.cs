using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Buffs
{
    public class ShockedAir : ModBuff
    {
        public override void SetStaticDefaults()
        {
            //TODO: DisplayName.SetDefault(GetType().Name);
            // Description.SetDefault("Damage taken is modified");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = false;
        }
    }
}
