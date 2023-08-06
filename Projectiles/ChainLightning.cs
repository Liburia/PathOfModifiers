using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Dusts;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

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
            //TODO: DisplayName.SetDefault("ChainLightning");
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.width = 32;
            Projectile.height = 6;
            Projectile.timeLeft = jumpTimeLimit;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
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
                nodes.AddLast(Projectile.Center);
                isNPC = Projectile.velocity.X == 1;
                target = isNPC ? (Entity)Main.npc[(int)Projectile.velocity.Y] : (Entity)Main.player[(int)Projectile.velocity.Y];
                init = true;
            }

            Vector2 targetPosition = target.Center;

            if (Projectile.position != targetPosition)
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

                Vector2 position = new Vector2(Projectile.position.X + velocityX, Projectile.position.Y - inboundY + nextInboundY);
                position = position.RotatedBy((target.position - nodes.Last.Value).ToRotation(), nodes.Last.Value);
                inboundY = nextInboundY;

                if ((targetPosition - position).LengthSquared() < snapRadiusSqr)
                {
                    position = targetPosition;
                    Projectile.timeLeft = PoMGlobals.ailmentDuration;
                }

                Projectile.position = position;

                nodes.AddLast(Projectile.position);

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
                Player owner = Main.player[Projectile.owner];
                if (isNPC)
                {
                    NPC npc = (NPC)target;
                    if (PoMUtil.CanHitNPC(npc))
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            PlaySound();
                            owner.ApplyDamageToNPC(npc, Projectile.damage, 1, npc.direction, false);
                            BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
                            pomNPC.AddShockedBuff(npc, Projectile.ai[0], PoMGlobals.ailmentDuration, true);
                        }
                        SpawnDebris(targetPosition);
                        hitEntities.Add(npc);
                    }
                }
                else
                {
                    Player player = (Player)target;
                    if (PoMUtil.CanHitPvp(owner, player))
                    {
                        if (player.whoAmI == Main.myPlayer)
                        {
                            PlaySound();
                            player.Hurt(PlayerDeathReason.ByProjectile(Projectile.owner, Projectile.whoAmI), Projectile.damage, player.direction, true);
                            player.GetModPlayer<BuffPlayer>().AddShockedBuff(player, Projectile.ai[0], PoMGlobals.ailmentDuration, true);
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
                        Projectile.timeLeft = jumpTimeLimit;
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }

            if (removingNodes && nodes.Count > 1)
            {
                SpawnVapour(nodes.First.Value, nodes.First.Next.Value);
                nodes.RemoveFirst();
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
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

                    Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                    Vector2 textureHalf = new Vector2(texture.Width / 2, texture.Height / 2);
                    Rectangle destination = new Rectangle(
                        (int)(drawPositionScreen.X),
                        (int)(drawPositionScreen.Y),
                        (int)drawLength,
                        texture.Height);
                    var drawData = new DrawData(texture, destination, null, new Color(1f, 1f, 1f, 0.9f), rotation, new Vector2(0, textureHalf.Y), SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(drawData);

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
                if (PoMUtil.CanHitNPC(npc))
                {
                    float lengthSqr = (npc.Center - Projectile.position).LengthSquared();
                    if (lengthSqr < maxJumpLengthSqr && lengthSqr < minLengthSqr && !hitEntities.Contains(npc))
                    {
                        closestEntity = npc;
                        minLengthSqr = lengthSqr;
                    }
                }
            }
            Player owner = Main.player[Projectile.owner];
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (PoMUtil.CanHitPvp(owner, player))
                {
                    float lengthSqr = (player.Center - Projectile.position).LengthSquared();
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

        void PlaySound()
        {
            SoundEngine.PlaySound(SoundID.Item54.WithVolumeScale(1f).WithPitchOffset(0.3f), Projectile.position);
        }
    }
}