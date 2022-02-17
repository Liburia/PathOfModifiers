using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace PathOfModifiers.Buffs
{
    public class Bleeding : DamageOverTime
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Taking damage over time");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            CreateDust(npc.Center);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            CreateDust(player.Center);
        }

        void CreateDust(Vector2 position)
        {
            int dustType = 5; //blood
            Dust.NewDustPerfect(position, dustType);
        }
    }
}
