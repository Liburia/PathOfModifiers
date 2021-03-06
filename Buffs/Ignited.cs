using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace PathOfModifiers.Buffs
{
    public class Ignited : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Losing or restoring life over time");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = true;
        }

        void CreateDust(Vector2 position, int width, int height)
        {
            if (Main.rand.NextBool(2))
            {
                Vector2 dustPosition = position + new Vector2(Main.rand.NextFloat(width), Main.rand.NextFloat(height));
                Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.FireDebris>(), new Vector2(0, Main.rand.NextFloat(-3f, -0.5f)), Alpha: 100, Scale: Main.rand.NextFloat(1f, 2f));
            }
        }
    }
}
