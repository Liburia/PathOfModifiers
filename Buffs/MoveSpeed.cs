using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;

namespace PathOfModifiers.Buffs
{
    public class MoveSpeed : ModBuff
    {
        //TODO: Actually load it
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Terraria/Buff_15";
            return true;
        }

        public override void SetDefaults()
        {
            DisplayName.SetDefault("DamageDoTDebuff");
            Description.SetDefault("Taking damage over time");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.moveSpeedBuff = true;

            for (int i = 0; i < 1; i++)
            {
                Vector2 dustPosition = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(0, player.height));
                Vector2 dustVelocity = player.velocity * 0.3f;
                if (dustVelocity == Vector2.Zero)
                {
                    dustVelocity = new Vector2(1, 0) * -player.direction;
                }
                float dustScale = Main.rand.NextFloat(0.5f, 2f);
                Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.SpeedEffect>(), dustVelocity, Scale: dustScale);
            }
        }
    }
}
