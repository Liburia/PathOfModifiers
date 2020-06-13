using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class FrostPulse : ModProjectile, INonTriggerringProjectile
    {
        const int baseTimeLeft = 600;
        const int timeLeftAfterTileCollide = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FrostPulse");
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.alpha = 100;
            projectile.timeLeft = baseTimeLeft;
            projectile.penetrate = 20;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;
            projectile.rotation = projectile.velocity.ToRotation();

            Vector2 position = projectile.position + new Vector2(
                Main.rand.NextFloat(projectile.width),
                Main.rand.NextFloat(projectile.height));
            Dust.NewDustPerfect(
                position,
                ModContent.DustType<Dusts.FrostDebris>(),
                Velocity: projectile.velocity * 0.2f,
                Alpha: 100,
                Scale: Main.rand.NextFloat(2.2f, 3.3f));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle sourceRectangle = texture.Bounds;
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, Color.White, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Hit(target);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Hit(target);
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitPlayer(target, damage, crit);
        }

        void Hit(NPC target)
        {
            target.GetGlobalNPC<BuffNPC>().AddChilledBuff(target, projectile.ai[0], PathOfModifiers.ailmentDuration);
        }
        void Hit(Player target)
        {
            target.GetModPlayer<BuffPlayer>().AddChilledBuff(target, projectile.ai[0], PathOfModifiers.ailmentDuration);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.timeLeft > timeLeftAfterTileCollide)
            {
                projectile.timeLeft = timeLeftAfterTileCollide;
            }

            projectile.velocity = oldVelocity;
            projectile.tileCollide = false;

            return false;
        }

        public override void Kill(int timeLeft)
        {
            PlayKillSound();
        }

        void PlayKillSound()
        {
            Main.PlaySound(SoundID.Item21.WithVolume(1f).WithPitchVariance(0.3f), projectile.Center);
        }
    }
}