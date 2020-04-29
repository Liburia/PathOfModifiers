using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Dusts;
using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace PathOfModifiers.Projectiles
{
    public class ChainLightning : ModProjectile, INonTriggerringProjectile
    {
        static readonly Vector3 emittedLight = new Vector3(1, 0.952f, 0.552f);
        static readonly Vector2 minVariance = new Vector2(16f, 8f);
        const int jumpTimeLimit = 30 * 2;    //* 2 because extra updates
        const int maxJumps = 10;
        const int maxNodes = 10;
        const float varianceLength = 128;
        const float varianceXOffset = -8f;
        const float snapRadiusSqr = 64f * 64f;
        const float dustInterval = 5f;
        const float boundHeight = 32f;
        const float maxJumpLengthSqr = 1024f * 1024f;

        LinkedList<Vector2> nodes = new LinkedList<Vector2>();
        HashSet<Entity> hitEntities = new HashSet<Entity>();
        bool removingNodes;
        bool isNPC;
        Entity target;
        float inboundY;
        int jumpsLeft;
        bool justWentBack;
        bool init;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ChainLightning");
        }

        public override void SetDefaults()
        {
            projectile.penetrate = 1;
            projectile.width = 32;
            projectile.height = 6;
            projectile.timeLeft = jumpTimeLimit;
            projectile.hostile = false;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            jumpsLeft = maxJumps;
            inboundY = minVariance.Y;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreAI()
        {
            if (!init)
            {
                nodes.AddLast(projectile.Center);
                isNPC = projectile.velocity.X == 1;
                target = isNPC ? (Entity)Main.npc[(int)projectile.velocity.Y] : (Entity)Main.player[(int)projectile.velocity.Y];
                init = true;
            }

            Vector2 targetPosition = target.Center;

            if (projectile.position != targetPosition)
            {
                float nextInboundY = Main.rand.NextFloat(0, boundHeight * 2);
                float diff = nextInboundY - inboundY;
                if (Math.Abs(diff) < minVariance.X)
                {
                    nextInboundY = inboundY + (Math.Sign(diff) * minVariance.X);
                }

                float velocityX = Main.rand.NextFloat(0, varianceLength) + varianceXOffset;
                if (velocityX < 0)
                {
                    if (justWentBack)
                    {
                        velocityX = minVariance.Y;
                        justWentBack = false;
                    }
                    else if (velocityX > -minVariance.Y)
                    {
                        velocityX = -minVariance.Y;
                    }
                }
                else
                {
                    justWentBack = false;
                }
                if (velocityX < 0)
                {
                    justWentBack = true;
                }

                Vector2 position = new Vector2(projectile.position.X + velocityX, projectile.position.Y - inboundY + nextInboundY);
                position = position.RotatedBy((target.position - nodes.Last.Value).ToRotation(), nodes.Last.Value);
                inboundY = nextInboundY;

                if ((targetPosition - position).LengthSquared() < snapRadiusSqr)
                {
                    position = targetPosition;
                    projectile.timeLeft = PathOfModifiers.ailmentDuration;
                }

                projectile.position = position;

                nodes.AddLast(projectile.position);

                if (!removingNodes && nodes.Count > maxNodes)
                {
                    removingNodes = true;
                }

                foreach (var node in nodes)
                {
                    Lighting.AddLight(node, emittedLight);
                }
            }
            else
            {
                Player owner = Main.player[projectile.owner];
                if (isNPC)
                {
                    NPC npc = (NPC)target;
                    if (PoMHelper.CanHitNPC(npc))
                    {
                        if (Main.myPlayer == projectile.owner)
                        {
                            owner.ApplyDamageToNPC(npc, projectile.damage, 1, npc.direction, false);
                            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
                            pomNPC.AddShockedBuff(npc, projectile.ai[0], PathOfModifiers.ailmentDuration, true);
                        }
                        SpawnDebris(targetPosition);
                        hitEntities.Add(npc);
                    }
                }
                else
                {
                    Player player = (Player)target;
                    if (PoMHelper.CanHitPvp(owner, player))
                    {
                        if (player.whoAmI == Main.myPlayer)
                        {
                            player.Hurt(PlayerDeathReason.ByPlayer(projectile.owner), projectile.damage, player.direction, true);
                            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
                            pomPlayer.AddShockedBuff(player, projectile.ai[0], PathOfModifiers.ailmentDuration, true);
                        }
                        SpawnDebris(targetPosition);
                        hitEntities.Add(player);
                    }
                }

                if (jumpsLeft > 0)
                {
                    if (FindNewTarget())
                    {
                        jumpsLeft--;
                        projectile.timeLeft = jumpTimeLimit;
                    }
                    else
                    {
                        projectile.Kill();
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }

            if (removingNodes && nodes.Count > 1)
            {
                SpawnVapour(nodes.First.Value, nodes.First.Next.Value);
                nodes.RemoveFirst();
            }

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (init)
            {
                var position1 = nodes.First;
                var position2 = position1.Next;
                for (int i = 1; i < nodes.Count; i++)
                {
                    Vector2 velocity = position2.Value - position1.Value;
                    Vector2 direction = velocity.SafeNormalize(Vector2.Zero);
                    Vector2 drawPosition = (position1.Value - (direction * 5));
                    Vector2 drawPositionScreen = drawPosition - Main.screenPosition;
                    float drawLength = velocity.Length() + 5;
                    float rotation = velocity.ToRotation();

                    Texture2D texture = Main.projectileTexture[projectile.type];
                    Vector2 textureHalf = new Vector2(texture.Width / 2, texture.Height / 2);
                    Rectangle destination = new Rectangle(
                        (int)(drawPositionScreen.X),
                        (int)(drawPositionScreen.Y),
                        (int)drawLength,
                        texture.Height);
                    spriteBatch.Draw(texture, destination, null, new Color(1f, 1f, 1f, 0.9f), rotation, new Vector2(0, textureHalf.Y), SpriteEffects.None, 0);

                    position1 = position2;
                    position2 = position1.Next;
                }
            }

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
            if (init)
            {
                var position1 = nodes.First;
                var position2 = position1.Next;
                for (int i = 1; i < nodes.Count; i++)
                {
                    SpawnVapour(position1.Value, position2.Value);

                    position1 = position2;
                    position2 = position1.Next;
                }
            }
        }

        bool FindNewTarget()
        {
            Entity closestEntity = null;
            float minLengthSqr = float.MaxValue;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (PoMHelper.CanHitNPC(npc))
                {
                    float lengthSqr = (npc.Center - projectile.position).LengthSquared();
                    if (lengthSqr < maxJumpLengthSqr && lengthSqr < minLengthSqr && !hitEntities.Contains(npc))
                    {
                        closestEntity = npc;
                        minLengthSqr = lengthSqr;
                    }
                }
            }
            Player owner = Main.player[projectile.owner];
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (PoMHelper.CanHitPvp(owner, player))
                {
                    float lengthSqr = (player.Center - projectile.position).LengthSquared();
                    if (lengthSqr < maxJumpLengthSqr && lengthSqr < minLengthSqr && !hitEntities.Contains(player))
                    {
                        closestEntity = player;
                        minLengthSqr = lengthSqr;
                    }
                }
            }

            isNPC = closestEntity is NPC;
            target = closestEntity;

            return closestEntity != null;
        }

        void SpawnVapour(Vector2 position1, Vector2 position2)
        {
            Vector2 velocity = position2 - position1;
            Vector2 direction = velocity.SafeNormalize(Vector2.Zero);
            int howMany = (int)(velocity.Length() / dustInterval);

            for (int j = 0; j < howMany; j++)
            {
                Dust.NewDustPerfect(position1 + (direction * dustInterval * j), ModContent.DustType<LightningVapour>(), Velocity: new Vector2(0, Main.rand.NextFloat(-0.3f, -1f)), Scale: Main.rand.NextFloat(0.3f, 0.5f));
            }
        }
        void SpawnDebris(Vector2 position)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(3f, 6f);
                Dust.NewDustPerfect(position, ModContent.DustType<LightningDebris>(), velocity, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }
    }
}