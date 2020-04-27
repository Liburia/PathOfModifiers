using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class Shock : ModDust
    {
        class CustomData
        {
            public int time;
            public int currentFrame;
        }

        const int frameTime = 1;
        const int frameWidth = 32;
        const int frameHeight = 32;
        const int totalFrames = 6;

        public override void OnSpawn(Dust dust)
        {
            int frameX = Main.rand.NextBool() ? frameWidth : 0;
            dust.frame = new Rectangle(frameX, 0, frameWidth, frameHeight);
            dust.alpha = 60;
            dust.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            //time, currentFrame
            dust.customData = new CustomData()
            {
                time = 0,
                currentFrame = Main.rand.Next(totalFrames),
            };
        }

        public override bool Update(Dust dust)
        {
            var customData = (CustomData)dust.customData;

            customData.time++;

            if (customData.time % frameTime == 0)
            {
                customData.currentFrame++;
                if (customData.currentFrame >= totalFrames)
                {
                    dust.active = false;
                }

                dust.frame.Y = customData.currentFrame * frameHeight;
            }

            return false;
        }
    }
}