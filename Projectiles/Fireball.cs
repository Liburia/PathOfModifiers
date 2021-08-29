using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class Fireball : ModProjectile, INonTriggerringProjectile
    {
        static readonly Vector2 explosionHalfSize = new Vector2(60f, 60f);
        const int baseTimeLeft = 600;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
        }
        public override void SetDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = baseTimeLeft;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 2f;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            Vector2 position = Projectile.position + new Vector2(
                Main.rand.NextFloat(Projectile.width),
                Main.rand.NextFloat(Projectile.height));
            Dust.NewDustPerfect(
                position,
                ModContent.DustType<Dusts.FireDebris>(),
                Velocity: Projectile.velocity * 0.4f,
                Alpha: 100,
                Scale: Main.rand.NextFloat(2f, 3f)); ;

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
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Color.White;
            var drawData = new DrawData(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(drawData);

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
            PlaySound();
            if (Main.netMode != NetmodeID.Server)
            {
                Rectangle explosionBounds = new Rectangle(
                    (int)(Projectile.Center.X - explosionHalfSize.X),
                    (int)(Projectile.Center.Y - explosionHalfSize.Y),
                    (int)explosionHalfSize.X * 2,
                    (int)explosionHalfSize.Y * 2);
                Player owner = Main.player[Projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMUtil.CanHitPvp(owner, player))
                {
                    if (player.getRect().Intersects(explosionBounds))
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(Projectile.owner), Projectile.damage, player.direction, true);
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
                            Rectangle npcBounds = npc.getRect();
                            if (npcBounds.Intersects(explosionBounds))
                            {
                                owner.ApplyDamageToNPC(npc, Projectile.damage, 1, npc.direction, false);
                                npc.GetGlobalNPC<BuffNPC>().AddIgnitedBuff(npc, (int)Projectile.ai[0], PoMGlobals.ailmentDuration);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.6f, 2f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FireDebris>(), velocity.X, velocity.Y, Alpha: 100, Scale: Main.rand.NextFloat(2f, 4f));
                velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.6f, 1.5f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1f, 2f));
            }
        }

        void PlaySound()
        {
            SoundEngine.PlaySound(SoundID.Item14.WithVolume(1f).WithPitchVariance(0.3f), Projectile.Center);
        }
    }
}