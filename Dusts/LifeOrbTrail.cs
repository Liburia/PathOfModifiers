using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class LifeOrbTrail : ModDust
    {
        const float scaleMultiplier = 0.9f;
        const float removeAtScale = 0.2f;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 6, 6);
            dust.alpha = 40;
        }

        public override bool Update(Dust dust)
        {
            if (dust.scale <= removeAtScale)
            {
                dust.active = false;
            }
            dust.scale *= scaleMultiplier;

            return false;
        }
    }
}