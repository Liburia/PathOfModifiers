using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class FireNova : ModProjectile, INonTriggerringProjectile
    {
        public static float startScale = 0.2f;
        public static float maxScale = 1;
        public static float scaleIncrease = 0.1f;
        public static Vector2 size = new Vector2(400, 400);
        public static float collisionScale = 0.7f;
        public static int timeLeft = 15;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire Nova");
            Main.projFrames[projectile.type] = 3;
        }

        public override string Texture => "Terraria/FlameRing";

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
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
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


            projectile.frameCounter++;
            if (projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= 3)
                {
                    projectile.frame = 0;
                }
            }
            
            for (int i = 0; i < 5; i++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
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

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = target.Center.X < projectile.Center.X ? -1 : 1;
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            //hitDirection = target.Center.X < projectile.Center.X ? -1 : 1;
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            ModifyHitPlayer(target, ref damage, ref crit);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 90, false);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 90, false);
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitPlayer(target, damage, crit);
        }
    }
}