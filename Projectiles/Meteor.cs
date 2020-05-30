using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class Meteor : ModProjectile, INonTriggerringProjectile
    {
        static Texture2D flameTexture;

        static readonly Vector2 explosionHalfSize = new Vector2(100f, 100f);
        const float tileCollideRadiusSqr = 100f * 100f;
        const int baseTimeLeft = 600;

        Vector2 targetPosition;
        bool init;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            Main.projFrames[projectile.type] = 2;
            flameTexture = ModContent.GetTexture("PathOfModifiers/Projectiles/MeteorFlame");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 24;
            projectile.timeLeft = baseTimeLeft;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1f;
        }

        public override bool PreAI()
        {
            if (!init)
            {
                targetPosition = projectile.velocity;
                projectile.velocity = (targetPosition - projectile.Center).SafeNormalize(Vector2.Zero) * 5f;
                projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;
                projectile.rotation = projectile.velocity.ToRotation();
                if (projectile.spriteDirection == -1)
                {
                    projectile.rotation += MathHelper.Pi;
                }
                init = true;
            }

            if (++projectile.frameCounter >= 7)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 2)
                {
                    projectile.frame = 0;
                }
            }

            if (!projectile.tileCollide && (targetPosition - projectile.Center).LengthSquared() < tileCollideRadiusSqr)
            {
                projectile.tileCollide = true;
            }

            Vector2 position = projectile.position + new Vector2(
                Main.rand.NextFloat(projectile.width),
                Main.rand.NextFloat(projectile.height));
            Dust.NewDustPerfect(
                position,
                ModContent.DustType<Dusts.FireDebris>(),
                Velocity: projectile.velocity * 0.1f,
                Alpha: 100,
                Scale: Main.rand.NextFloat(2f, 3f));

            if (Main.netMode != NetmodeID.Server)
            {
                Player owner = Main.player[projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMUtil.CanHitPvp(owner, player))
                {
                    if (projectile.getRect().Intersects(player.getRect()))
                    {
                        projectile.Kill();
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle rect = projectile.getRect();
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMUtil.CanHitNPC(npc))
                        {
                            if (rect.Intersects(npc.getRect()))
                            {
                                projectile.Kill();
                                break;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            int frameHeight = flameTexture.Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;
            Rectangle flameSourceRectangle = new Rectangle(0, startY, flameTexture.Width, frameHeight);
            Vector2 flameOrigin = flameSourceRectangle.Size() / 2f;

            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Color.White;

            Main.spriteBatch.Draw(flameTexture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                flameSourceRectangle, drawColor, projectile.rotation, flameOrigin, projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

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
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 3.5f);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.FireDebris>(), velocity.X, velocity.Y, Alpha: 100, Scale: Main.rand.NextFloat(3.5f, 7f));
                velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 2.5f);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1.5f, 3.1f));
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
                if (PoMUtil.CanHitPvp(owner, player))
                {
                    if (player.getRect().Intersects(hitRect))
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(projectile.owner), projectile.damage, player.direction, true);
                        player.GetModPlayer<BuffPlayer>().AddIgnitedBuff(player, (int)projectile.ai[0], PathOfModifiers.ailmentDuration);
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMUtil.CanHitNPC(npc))
                        {
                            Rectangle npcRect = npc.getRect();
                            if (npcRect.Intersects(hitRect))
                            {
                                owner.ApplyDamageToNPC(npc, projectile.damage, 1, npc.direction, false);
                                npc.GetGlobalNPC<PoMNPC>().AddIgnitedBuff(npc, (int)projectile.ai[0], PathOfModifiers.ailmentDuration);
                            }
                        }
                    }

                    Projectile.NewProjectile(projectileCenter, Vector2.Zero, ModContent.ProjectileType<BurningAir>(), (int)projectile.ai[0], 0, projectile.owner, 100f);
                }
            }
        }
    }
}