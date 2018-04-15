using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class Fireball : ModProjectile
    {
        static int timeLeft = 600;
        static int ignoreTime = 10;

        public Entity ignoreTarget;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
        }

        public override void SetDefaults()
        {
            projectile.damage = 10;
            projectile.penetrate = 1;
            projectile.width = 16;
            projectile.height = 16;
            projectile.timeLeft = timeLeft;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.rotation = projectile.velocity.ToRotation() + 0.8f;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 textureHalf = new Vector2(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], new Vector2(projectile.position.X - Main.screenPosition.X + textureHalf.X, projectile.position.Y - Main.screenPosition.Y + textureHalf.Y), null, Color.White, projectile.rotation, textureHalf, 1, SpriteEffects.None, 1);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            NPC ignoreNPC = ignoreTarget as NPC;
            if (ignoreNPC != target || timeLeft - projectile.timeLeft > ignoreTime)
                return true;
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            Player ignorePlayer = ignoreTarget as Player;
            if (ignorePlayer != target || timeLeft - projectile.timeLeft > ignoreTime)
                return true;
            return false;
        }
        public override bool CanHitPvp(Player target)
        {
            return CanHitPlayer(target);
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

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
        }
    }
}