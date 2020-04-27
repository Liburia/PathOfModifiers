using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class LightningBoltVapour : ModDust
    {
        class CustomData
        {
            public int time;
            public int currentFrame;
        }

        const float scaleMultiplier = 0.95f;
        const int alphaAdd = 5;
        const int removeAtFrame = 6;
        const int frameTime = 13;
        const int frameHeight = 16;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, 0, 16, frameHeight);
            dust.alpha = 0;
            //time, currentFrame
            dust.customData = new CustomData();
        }

        public override bool Update(Dust dust)
        {
            var customData = (CustomData)dust.customData;
            if (customData.currentFrame >= removeAtFrame)
            {
                dust.active = false;
            }

            dust.position += dust.velocity;
            dust.scale *= scaleMultiplier;
            dust.alpha += alphaAdd;

            customData.time++;

            if (customData.time % frameTime == 0)
            {
                dust.frame.Y += frameHeight;
                customData.currentFrame++;
            }

            return false;
        }
    }
}