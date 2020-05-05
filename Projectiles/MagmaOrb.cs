using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class MagmaOrb : ModProjectile, INonTriggerringProjectile
    {
        static readonly Vector2 explosionHalfSize = new Vector2(120f, 120f);
        const int baseTimeLeft = 600;
        const int bounces = 5;
        const float bounceFriction = 0.8f;

        float addRotation;
        int bouncesLeft;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("MagmaOrb");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.timeLeft = baseTimeLeft;
            projectile.penetrate = 20;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            addRotation = Main.rand.NextFloat(-0.2f, 0.2f);
            bouncesLeft = bounces;
        }

        public override void AI()
        {
            projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;

            projectile.rotation += addRotation;

            projectile.velocity.Y += 0.3f;
            if (projectile.velocity.Y > 10)
            {
                projectile.velocity.Y = 10;
            }

            Vector2 position = projectile.position + new Vector2(
                Main.rand.NextFloat(projectile.width),
                Main.rand.NextFloat(projectile.height));
            Dust.NewDustPerfect(
                position,
                ModContent.DustType<Dusts.FireDebris>(),
                Velocity: projectile.velocity * 0.2f,
                Alpha: 100,
                Scale: Main.rand.NextFloat(2.2f, 3.3f)); ;
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

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (bouncesLeft <= 0)
            {
                projectile.Kill();
            }
            else
            {
                Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Item10, projectile.position);

                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X * bounceFriction;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y * bounceFriction;
                }

                Explode();

                bouncesLeft--;
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Explode();
        }

        void Explode()
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.8f, 2.5f);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.FireDebris>(), velocity.X, velocity.Y, Alpha: 100, Scale: Main.rand.NextFloat(3f, 5f));
                velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.8f, 2f);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f));
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 projectileCenter = projectile.Center;
                Rectangle hitRect = new Rectangle(
                    (int)(projectileCenter.X - explosionHalfSize.X),
                    (int)(projectileCenter.Y - explosionHalfSize.Y),
                    (int)explosionHalfSize.X * 2,
                    (int)explosionHalfSize.Y * 2);
                Player owner = Main.player[projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMHelper.CanHitPvp(owner, player))
                {
                    if (player.getRect().Intersects(hitRect))
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(projectile.owner), projectile.damage, player.direction, true);
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMHelper.CanHitNPC(npc))
                        {
                            Rectangle npcRect = npc.getRect();
                            if (npcRect.Intersects(hitRect))
                            {
                                owner.ApplyDamageToNPC(npc, projectile.damage, 1, npc.direction, false);
                            }
                        }
                    }

                    Projectile.NewProjectile(projectileCenter, Vector2.Zero, ModContent.ProjectileType<BurningAir>(), (int)projectile.ai[0], 0, projectile.owner);
                }
            }
        }
    }
}