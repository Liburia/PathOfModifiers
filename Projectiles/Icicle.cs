using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class Icicle : ModProjectile, INonTriggerringProjectile
    {
        static int timeLeft = 300;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.damage = 10;
            projectile.width = 13;
            projectile.height = 13;
            projectile.timeLeft = timeLeft;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= 5)
                {
                    projectile.frame = 0;
                }
            }

            projectile.rotation = projectile.velocity.ToRotation();

            Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.FrostDebris>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = projectile.GetAlpha(lightColor);
            Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            PlayKillSound();
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.FrostDebris>());
            }
        }

        void PlayKillSound()
        {
            Main.PlaySound(SoundID.Item27.WithVolume(1f).WithPitchVariance(0.3f), projectile.Center);
        }
    }
}