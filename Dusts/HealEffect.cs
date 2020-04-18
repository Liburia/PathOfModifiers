using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class HealEffect : ModDust
    {
        readonly Vector3 emittedLight = new Vector3(0.4f, 0, 0);
        const float scaleMultiplier = 0.9f;
        const float removeAtScale = 0.2f;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 10, 10);
            dust.alpha = 100;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;

            if (dust.scale <= removeAtScale)
            {
                dust.active = false;
            }
            dust.scale *= scaleMultiplier;

            Lighting.AddLight(dust.position, emittedLight);

            return false;
        }
    }
}