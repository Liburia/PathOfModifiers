using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class MoltenShell : ModDust
    {
        const float scaleMultiplier = 0.95f;
        const float removeAtScale = 0.1f;
        const int alphaAdd = 15;
        const int spawnFireInterval = 2;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Value.Bounds;
            dust.alpha = 0;
            dust.customData = 0;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale *= scaleMultiplier;
            dust.alpha += alphaAdd;

            if (dust.scale <= removeAtScale)
            {
                dust.active = false;
            }

            return false;
        }
    }
}