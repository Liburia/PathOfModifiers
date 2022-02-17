using Microsoft.Xna.Framework;
using PathOfModifiers.Dusts;
using PathOfModifiers.ModNet.PacketHandlers;
using System;
using Terraria;
using Terraria.Enums;
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

        float Magnitude => MathHelper.Clamp((float)Math.Sqrt(Math.Sqrt(Projectile.damage)), 1, 999);

        bool isHomingOnTarget = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LifeOrb");
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.timeLeft = baseTime;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Rectangle bounds = Projectile.getRect();

            if (Main.netMode != NetmodeID.Server)
            {
                Player player = Main.LocalPlayer;
                if (!player.dead && (player.whoAmI == Projectile.owner || (player.team != (int)Team.None && player.team == Main.player[Projectile.owner].team)) && bounds.Intersects(player.getRect()))
                {
                    player.statLife += Projectile.damage;
                    player.HealEffect(Projectile.damage);
                    SpawnDebris(DebrisType.Entity);
                    if (Main.netMode == NetmodeID.SinglePlayer || Projectile.owner == Main.myPlayer)
                    {
                        Projectile.Kill();
                    }
                    else
                    {
                        ProjectilePacketHandler.CSendKill(Projectile);
                    }

                    return;
                }
            }


            float mag = Magnitude;

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LifeOrbTrail>(), Scale: mag);
            Lighting.AddLight(Projectile.position, emittedLight * mag);

            Projectile.rotation += 0.1f;

            Vector2 accelerationDirection;
            if (isHomingOnTarget)
            {
                Vector2 targetPosition = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                Vector2 toTarget = targetPosition - Projectile.Center;

                if (toTarget.LengthSquared() <= targetSizeSqr)
                {
                    isHomingOnTarget = false;
                }

                Projectile.velocity *= friction;
                accelerationDirection = (targetPosition - Projectile.Center).SafeNormalize(Vector2.Zero);
            }
            else
            {
                accelerationDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            }
            float currentAcceleration = maxAcceleration * ((baseTime - Projectile.timeLeft) / (float)timeToMaxAcceleration);
            Projectile.velocity += accelerationDirection * currentAcceleration;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnDebris(DebrisType.Tile);

            return true;
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
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LifeOrbDebris>(), velocity.X, velocity.Y, Scale: scale);
                }
            }
            else if (debrisType == DebrisType.Tile)
            {
                for (int i = 0; i < 10; i++)
                {
                    float minAngle = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - 0.4f;
                    float maxAngle = minAngle + 0.8f;
                    Vector2 velocity = Main.rand.NextVector2Unit(minAngle, maxAngle) * 1.2f;
                    float scale = 1.2f * Magnitude;
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LifeOrbDebris>(), velocity.X, velocity.Y, Scale: scale);
                }
            }
            else if (debrisType == DebrisType.Entity)
            {
                for (int i = 0; i < 5; i++)
                {
                    float minAngle = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - 0.2f;
                    float maxAngle = minAngle + 0.4f;
                    Vector2 velocity = Main.rand.NextVector2Unit(minAngle, maxAngle) * 0.7f;
                    float scale = 0.7f * Magnitude;
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<LifeOrbDebris>(), velocity.X, velocity.Y, Scale: scale);
                }
            }
        }
    }
}