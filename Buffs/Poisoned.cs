using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace PathOfModifiers.Buffs
{
    public class Poisoned : StackingDamageOverTime
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            CreateDust(npc.position, npc.width, npc.height);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            CreateDust(player.position, player.width, player.height);
        }

        void CreateDust(Vector2 position, int width, int height)
        {
            if (Main.rand.Next(10) == 0)
            {
                int dustType = 46; //poison
                int dust = Dust.NewDust(position, width, height, dustType, Alpha: 150, Scale: 0.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].fadeIn = 1.9f;
            }
        }
    }
}
