using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class FrostSpike : ModProjectile, INonTriggerringProjectile
    {
        static int timeLeft = 600;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Spike");
            Main.projFrames[projectile.type] = 5;
        }

        public override string Texture => "Terraria/Projectile_337";

        public override void SetDefaults()
        {
            projectile.damage = 10;
            projectile.penetrate = 1;
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
                if (projectile.frame >= 3)
                {
                    projectile.frame = 0;
                }
            }

            Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Frozen);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.rotation = projectile.velocity.ToRotation() + 1.57f;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 90, false);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 90, false);
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitPlayer(target, damage, crit);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Frozen);
        }
    }
}