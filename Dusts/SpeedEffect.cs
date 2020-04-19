using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class SpeedEffect : ModDust
    {
        const float scaleMultiplier = 0.9f;
        const int alphaAdd = 10;
        const float removeAtAlpha = 230;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 18, 3);
            dust.alpha = 40;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            if (dust.alpha >= removeAtAlpha)
            {
                dust.active = false;
            }
            dust.alpha += alphaAdd;
            dust.rotation = dust.velocity.ToRotation();

            return false;
        }
    }
}