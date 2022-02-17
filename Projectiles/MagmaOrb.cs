using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
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
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.timeLeft = baseTimeLeft;
            Projectile.penetrate = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            addRotation = Main.rand.NextFloat(-0.2f, 0.2f);
            bouncesLeft = bounces;
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            Projectile.rotation += addRotation;

            Projectile.velocity.Y += 0.3f;
            if (Projectile.velocity.Y > 10)
            {
                Projectile.velocity.Y = 10;
            }

            Vector2 position = Projectile.position + new Vector2(
                Main.rand.NextFloat(Projectile.width),
                Main.rand.NextFloat(Projectile.height));
            Dust.NewDustPerfect(
                position,
                ModContent.DustType<Dusts.FireDebris>(),
                Velocity: Projectile.velocity * 0.2f,
                Alpha: 100,
                Scale: Main.rand.NextFloat(2.2f, 3.3f)); ;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle sourceRectangle = texture.Bounds;
            Vector2 origin = sourceRectangle.Size() / 2f;

            var drawData = new DrawData(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(drawData);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (bouncesLeft <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X * bounceFriction;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y * bounceFriction;
                }

                Explode();

                bouncesLeft--;
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            PlayKillSound();
            Explode();
        }

        void Explode()
        {
            PlayExplodeSound();
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.8f, 2.5f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FireDebris>(), velocity.X, velocity.Y, Alpha: 100, Scale: Main.rand.NextFloat(3f, 5f));
                velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.8f, 2f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f));
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
                        player.Hurt(PlayerDeathReason.ByPlayer(Projectile.owner), Projectile.damage, player.direction, true);
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
                            }
                        }
                    }

                    Projectile.NewProjectile(
                    new ProjectileSource_ProjectileParent(Projectile),
                    projectileCenter, Vector2.Zero, ModContent.ProjectileType<BurningAir>(), (int)Projectile.ai[0], 0, Projectile.owner, 48f);
                }
            }
        }

        void PlayExplodeSound()
        {
            SoundEngine.PlaySound(SoundID.Item73.WithVolume(1f).WithPitchVariance(0.3f), Projectile.Center);
        }

        void PlayKillSound()
        {
            SoundEngine.PlaySound(SoundID.Item89.WithVolume(1f).WithPitchVariance(0.3f), Projectile.Center);
        }
    }
}