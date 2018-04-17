using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class ForceWave : ModProjectile, INonTriggerringProjectile
    {
        public static float speedMult = 0.95f;
        public static float startScale = 0.3f;
        public static float maxScale = 3;
        public static float scaleIncrease = 0.15f;
        public static Vector2 size = new Vector2(50, 50);
        public static float collisionScale = 0.8f;
        public static int timeLeft = 15;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force Wave");
        }

        public override string Texture => "Terraria/Projectile_348";

        public override void SetDefaults()
        {
            projectile.damage = 10;
            projectile.scale = startScale;
            projectile.Size = size * collisionScale * startScale;
            projectile.timeLeft = timeLeft;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            projectile.velocity *= speedMult;
            projectile.rotation = projectile.velocity.ToRotation() + 1.57f;
            projectile.alpha += (timeLeft - projectile.timeLeft) * 3;

            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            if (projectile.scale < maxScale)
            {
                projectile.scale += scaleIncrease;
                if (projectile.scale >= maxScale)
                    projectile.scale = maxScale;
                projectile.Size = size * collisionScale * projectile.scale;
                projectile.position -= new Vector2(texture.Width, frameHeight) * collisionScale * scaleIncrease / 2;
            }
            
            for (int i = 0; i < 5; i++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = projectile.GetAlpha(lightColor);
            Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}