using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class LifeOrbDebris : ModDust
    {
        Vector3 emittedColor = new Vector3(0.2f, 0, 0);

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.alpha = 40;
        }

        public override bool MidUpdate(Dust dust)
        {
            Lighting.AddLight(dust.position, emittedColor * dust.scale);
            return true;
        }
    }
}