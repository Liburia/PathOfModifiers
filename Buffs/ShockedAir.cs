using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;

namespace PathOfModifiers.Buffs
{
    public class ShockedAir : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Damage taken is modified");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.isOnShockedAir = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().isOnShockedAir = true;
        }
    }
}
