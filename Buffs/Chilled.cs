using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Buffs
{
    public class Chilled : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(50))
            {
                Vector2 position = npc.position + new Vector2(Main.rand.NextFloat(0, npc.width), Main.rand.NextFloat(0, npc.height)) + new Vector2(-14, -14);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.FrostCloud>(), Vector2.Zero, 50, Color.White, Main.rand.NextFloat(0.8f, 1.6f));
            }
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (Main.rand.NextBool(50))
            {
                Vector2 position = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(0, player.height)) + new Vector2(-14, -14);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.FrostCloud>(), Vector2.Zero, 50, Color.White, Main.rand.NextFloat(0.8f, 1.6f));
            }
        }
    }
}
