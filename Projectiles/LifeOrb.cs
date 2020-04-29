using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Dusts;
using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class LifeOrb : ModProjectile, INonTriggerringProjectile
    {
        enum DebrisType
        {
            Expired,
            Tile,
            Entity,
        }

        readonly Vector3 emittedLight = new Vector3(0.3f, 0, 0);
        const int baseTime = 600;
        const float friction = 0.95f;
        const float maxAcceleration = 2f;
        const int timeToMaxAcceleration = 120;
        const float targetSizeSqr = 24 * 24;

        float Magnitude => MathHelper.Clamp((float)Math.Sqrt(Math.Sqrt(projectile.damage)), 1, 999);

        bool isHomingOnTarget = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LifeOrb");
        }

        public override void SetDefaults()
        {
            projectile.penetrate = 1;
            projectile.width = 6;
            projectile.height = 6;
            projectile.timeLeft = baseTime;
            projectile.hostile = false;
            projectile.friendly = false;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.owner == Main.myPlayer)
            {
                Rectangle bounds = projectile.getRect();
                //TODO: Don't need to loop, just check the main player.
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && (i == projectile.owner || (player.team != 0 && player.team == Main.player[projectile.owner].team)) && bounds.Intersects(player.getRect()))
                    {
                        player.statLife += projectile.damage;
                        player.HealEffect(projectile.damage);
                        SpawnDebris(DebrisType.Entity);
                        projectile.Kill();

                        return;
                    }
                }
            }

            float mag = Magnitude;

            Dust.NewDustPerfect(projectile.Center, ModContent.DustType<LifeOrbTrail>(), Scale: mag);
            Lighting.AddLight(projectile.position, emittedLight * mag);

            projectile.rotation += 0.1f;

            Vector2 accelerationDirection;
            if (isHomingOnTarget)
            {
                Vector2 targetPosition = new Vector2(projectile.ai[0], projectile.ai[1]);
                Vector2 toTarget = targetPosition - projectile.Center;

                if (toTarget.LengthSquared() <= targetSizeSqr)
                {
                    isHomingOnTarget = false;
                }

                projectile.velocity *= friction;
                accelerationDirection = (targetPosition - projectile.Center).SafeNormalize(Vector2.Zero);
            }
            else
            {
                accelerationDirection = projectile.velocity.SafeNormalize(Vector2.Zero);
            }
            float currentAcceleration = maxAcceleration * ((baseTime - projectile.timeLeft) / (float)timeToMaxAcceleration);
            projectile.velocity += accelerationDirection * currentAcceleration;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
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
            if (timeLeft == 0)
            {
                SpawnDebris(DebrisType.Expired);
            }
        }

        void SpawnDebris(DebrisType debrisType)
        {
            if (debrisType == DebrisType.Expired)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(1, 1) * 2;
                    float scale = 1.2f * Magnitude;
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<LifeOrbDebris>(), velocity.X, velocity.Y, Scale: scale);
                }
            }
            else if (debrisType == DebrisType.Tile)
            {
                for (int i = 0; i < 10; i++)
                {
                    float minAngle = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 0.4f;
                    float maxAngle = minAngle + 0.8f;
                    Vector2 velocity = Main.rand.NextVector2Unit(minAngle, maxAngle) * 1.2f;
                    float scale = 1.2f * Magnitude;
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<LifeOrbDebris>(), velocity.X, velocity.Y, Scale: scale);
                }
            }
            else if (debrisType == DebrisType.Entity)
            {
                for (int i = 0; i < 5; i++)
                {
                    float minAngle = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) - 0.2f;
                    float maxAngle = minAngle + 0.4f;
                    Vector2 velocity = Main.rand.NextVector2Unit(minAngle, maxAngle) * 0.7f;
                    float scale = 0.7f * Magnitude;
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<LifeOrbDebris>(), velocity.X, velocity.Y, Scale: scale);
                }
            }
        }
    }
}