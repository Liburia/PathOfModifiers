using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Dusts
{
    public class ShockedAir : ModDust
    {
        class CustomData
        {
            public int time;
            public int currentFrame;
            public int loopCoolingdown;
            public int loopCooldown;
        }

        const int minAlpha = 40;
        const int addAlpha = 160;
        const int frameTime = 1;
        const int frameWidth = 32;
        const int frameHeight = 32;
        const int totalFrames = 6;
        const int loopCooldown = 20;

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "PathOfModifiers/Dusts/Shock";

            return base.Autoload(ref name, ref texture);
        }

        public override void OnSpawn(Dust dust)
        {
            int frameX = Main.rand.NextBool() ? frameWidth : 0;
            dust.frame = new Rectangle(frameX, 0, frameWidth, frameHeight);
            dust.alpha = 0;
            dust.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            //time, currentFrame
            dust.customData = new CustomData()
            {
                time = 0,
                currentFrame = Main.rand.Next(totalFrames),
                loopCoolingdown = 0,
                loopCooldown = (int)(loopCooldown * Main.rand.NextFloat(0.5f, 2f)),
            };
        }

        public override bool Update(Dust dust)
        {
            var customData = (CustomData)dust.customData;

            if (customData.time >= PathOfModifiers.ailmentDuration)
            {
                dust.active = false;
            }

            customData.time++;

            if (customData.loopCoolingdown > 0)
            {
                customData.loopCoolingdown++;
                if (customData.loopCoolingdown >= customData.loopCooldown)
                {
                    customData.loopCoolingdown = 0;
                    customData.loopCooldown = (int)(loopCooldown * Main.rand.NextFloat(0.5f, 2f));
                }
            }
            else
            {
                dust.alpha = (int)(minAlpha + (addAlpha * (customData.time / (float)PathOfModifiers.ailmentDuration)));

                if (customData.time % frameTime == 0)
                {
                    customData.currentFrame++;
                    if (customData.currentFrame >= totalFrames)
                    {
                        dust.alpha = 255;
                        customData.currentFrame = 0;
                        customData.loopCoolingdown = 1;
                    }

                    dust.frame.Y = customData.currentFrame * frameHeight;
                }
            }

            return false;
        }
    }
}