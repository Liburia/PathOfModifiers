using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class FrostDebris : ModDust
    {
        static readonly Vector3 emittedLight = new Vector3(0.094f, 0.749f, 0.933f);
        const float scaleMultiplier = 0.95f;
        const float removeAtScale = 0.5f;
        const int frameHeight = 10;


        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, frameHeight * Main.rand.Next(3), 10, 10);
        }

        public override bool Update(Dust dust)
        {
            if (dust.scale <= removeAtScale)
            {
                dust.active = false;
                return false;
            }

            dust.scale *= scaleMultiplier;
            dust.position += dust.velocity;

            Lighting.AddLight(dust.position, emittedLight);

            return false;
        }
    }
}