using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class Meteor : ModProjectile, INonTriggerringProjectile
    {
        static Asset<Texture2D> flameTexture;

        static readonly Vector2 explosionHalfSize = new Vector2(100f, 100f);
        const float tileCollideRadiusSqr = 100f * 100f;
        const int baseTimeLeft = 600;

        Vector2 targetPosition;
        bool init;

        public override void SetStaticDefaults()
        {
            //TODO: DisplayName.SetDefault("Meteor");
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            flameTexture = ModContent.Request<Texture2D>("PathOfModifiers/Projectiles/MeteorFlame");
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.timeLeft = baseTimeLeft;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
        }

        public override bool PreAI()
        {
            if (!init)
            {
                targetPosition = Projectile.velocity;
                Projectile.velocity = (targetPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * 8f;
                Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
                init = true;
            }

            if (++Projectile.frameCounter >= 7)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }

            if (!Projectile.tileCollide && (targetPosition - Projectile.Center).LengthSquared() < tileCollideRadiusSqr)
            {
                Projectile.tileCollide = true;
            }

            Vector2 position = Projectile.position + new Vector2(
                Main.rand.NextFloat(Projectile.width),
                Main.rand.NextFloat(Projectile.height));
            Dust.NewDustPerfect(
                position,
                ModContent.DustType<Dusts.FireDebris>(),
                Velocity: Projectile.velocity * 0.1f,
                Alpha: 100,
                Scale: Main.rand.NextFloat(2f, 3f));

            if (Main.netMode != NetmodeID.Server)
            {
                Player owner = Main.player[Projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMUtil.CanHitPvp(owner, player))
                {
                    if (Projectile.getRect().Intersects(player.getRect()))
                    {
                        Projectile.Kill();
                    }
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Rectangle rect = Projectile.getRect();
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMUtil.CanHitNPC(npc))
                        {
                            if (rect.Intersects(npc.getRect()))
                            {
                                Projectile.Kill();
                                break;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            var flame = flameTexture.Value;
            int frameHeight = flame.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle flameSourceRectangle = new Rectangle(0, startY, flame.Width, frameHeight);
            Vector2 flameOrigin = flameSourceRectangle.Size() / 2f;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Color.White;

            var drawData = new DrawData(flame,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                flameSourceRectangle, drawColor, Projectile.rotation, flameOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(drawData);

            drawData = new DrawData(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(drawData);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Explode();
        }

        void Explode()
        {
            PlayExplodeSound();
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 3.5f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FireDebris>(), velocity.X, velocity.Y, Alpha: 100, Scale: Main.rand.NextFloat(3.5f, 7f));
                velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 2.5f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1.5f, 3.1f));
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 projectileCenter = Projectile.Center;
                Rectangle hitRect = new Rectangle(
                    (int)(projectileCenter.X - explosionHalfSize.X),
                    (int)(projectileCenter.Y - explosionHalfSize.Y),
                    (int)explosionHalfSize.X * 2,
                    (int)explosionHalfSize.Y * 2);
                Player owner = Main.player[Projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMUtil.CanHitPvp(owner, player))
                {
                    if (player.getRect().Intersects(hitRect))
                    {
                        player.Hurt(PlayerDeathReason.ByProjectile(Projectile.owner, Projectile.whoAmI), Projectile.damage, player.direction, true);
                        player.GetModPlayer<BuffPlayer>().AddIgnitedBuff(player, (int)Projectile.ai[0], PoMGlobals.ailmentDuration);
                    }
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMUtil.CanHitNPC(npc))
                        {
                            Rectangle npcRect = npc.getRect();
                            if (npcRect.Intersects(hitRect))
                            {
                                owner.ApplyDamageToNPC(npc, Projectile.damage, 1, npc.direction, false);
                                npc.GetGlobalNPC<BuffNPC>().AddIgnitedBuff(npc, (int)Projectile.ai[0], PoMGlobals.ailmentDuration);
                            }
                        }
                    }

                    Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    projectileCenter, Vector2.Zero, ModContent.ProjectileType<BurningAir>(), (int)Projectile.ai[0], 0, Projectile.owner, 100f);
                }
            }
        }

        void PlayExplodeSound()
        {
            SoundEngine.PlaySound(SoundID.Item74.WithVolumeScale(1f).WithPitchOffset(0.3f), Projectile.Center);
        }
    }
}