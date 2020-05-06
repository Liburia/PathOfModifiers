using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class FrostCloud : ModDust
    {
        static readonly Vector2 scaleVariance = new Vector2(0.98f, 1.02f);
        const int alphaAdd = 5;
        const int removeAtAlpha = 240;
        const int frameWidth = 28;
        const int frameHeight = 28;


        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, frameHeight * Main.rand.Next(4), frameWidth, frameHeight);
        }

        public override bool Update(Dust dust)
        {
            if (dust.alpha >= removeAtAlpha)
            {
                dust.active = false;
                return false;
            }

            dust.alpha += alphaAdd;
            dust.scale *= Main.rand.NextFloat(scaleVariance.X, scaleVariance.Y);
            dust.position += dust.velocity;

            return false;
        }
    }
}