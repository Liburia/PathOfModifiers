using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class Fireball : ModProjectile, INonTriggerringProjectile
    {
        const int baseTimeLeft = 600;
        const float explosionRadiusSqr = 256f * 256f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.timeLeft = baseTimeLeft;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.scale = 2f;
        }

        public override void AI()
        {
            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 3)
                {
                    projectile.frame = 0;
                }
            }

            projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation += MathHelper.Pi;
            }

            Vector2 position = projectile.position + new Vector2(
                Main.rand.NextFloat(projectile.width),
                Main.rand.NextFloat(projectile.height));
            Dust.NewDustPerfect(
                position,
                ModContent.DustType<Dusts.FireDebris>(),
                Velocity: projectile.velocity * 0.4f,
                Scale: Main.rand.NextFloat(2f, 3f)); ;

            if (Main.netMode != NetmodeID.Server)
            {
                Player owner = Main.player[projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMHelper.CanHitPvp(owner, player))
                {
                    if (projectile.getRect().Intersects(player.getRect()))
                    {
                        projectile.Kill();
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMHelper.CanHitNPC(npc))
                        {
                            if (projectile.getRect().Intersects(npc.getRect()))
                            {
                                projectile.Kill();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Color.White;
            Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

            return false;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override bool CanHitPvp(Player target)
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Player owner = Main.player[projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMHelper.CanHitPvp(owner, player))
                {
                    if ((player.Center - projectile.position).LengthSquared() < explosionRadiusSqr)
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(projectile.owner), projectile.damage, player.direction, true);
                        player.GetModPlayer<PoMPlayer>().AddIgnitedBuff(player, (int)projectile.ai[0], PathOfModifiers.ailmentDuration);
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMHelper.CanHitNPC(npc))
                        {
                            if ((npc.Center - projectile.position).LengthSquared() < explosionRadiusSqr)
                            {
                                owner.ApplyDamageToNPC(npc, projectile.damage, 1, npc.direction, false);
                                npc.GetGlobalNPC<PoMNPC>().AddIgnitedBuff(npc, (int)projectile.ai[0], PathOfModifiers.ailmentDuration);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.6f, 2f);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.FireDebris>(), velocity.X, velocity.Y, Scale: Main.rand.NextFloat(2f, 4f));
                velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.6f, 1.5f);
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1f, 2f));
            }
        }
    }
}