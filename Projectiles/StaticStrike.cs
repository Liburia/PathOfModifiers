using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class StaticStrike : ModProjectile, INonTriggerringProjectile
    {
        static readonly Vector2 halfSize = new Vector2(150f, 150f);
        static readonly Vector3 emittedLight = new Vector3(1, 0.952f, 0.552f);
        static readonly Point frameSize = new Point(32, 32);
        static readonly Point frameNumber = new Point(4, 4);
        const int frameTime = 3;
        const float collisionScale = 0.85f;

        HashSet<Entity> hitEntities = new HashSet<Entity>();
        bool init;
        int[] frameVariants = new int[4];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("StaticStrike");
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 40;
            Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            for (int i = 0; i < 4; i++)
            {
                frameVariants[i] = Main.rand.Next(frameNumber.X);
            }
        }

        public override bool PreAI()
        {
            if (!init)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(5f, 10f);
                    Dust.NewDustPerfect(Projectile.position, ModContent.DustType<Dusts.LightningDebris>(), dustVelocity, 0, Color.White, Scale: Main.rand.NextFloat(0.8f, 1.6f));
                }
                init = true;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameTime)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= frameNumber.Y)
                {
                    Projectile.Kill();
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Rectangle hitRect = new Rectangle(
                    (int)(Projectile.position.X - (halfSize.X * collisionScale)),
                    (int)(Projectile.position.Y - (halfSize.Y * collisionScale)),
                    (int)(halfSize.X * 2 * collisionScale),
                    (int)(halfSize.Y * 2 * collisionScale));
                Player owner = Main.player[Projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMUtil.CanHitPvp(owner, player) && !hitEntities.Contains(player))
                {
                    if (player.getRect().Intersects(hitRect))
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(Projectile.owner), Projectile.damage, player.direction, true);
                        hitEntities.Add(player);
                    }
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMUtil.CanHitNPC(npc) && !hitEntities.Contains(npc))
                        {
                            Rectangle npcRect = npc.getRect();
                            if (npcRect.Intersects(hitRect))
                            {
                                owner.ApplyDamageToNPC(npc, Projectile.damage, 1, npc.direction, false);
                                hitEntities.Add(npc);
                            }
                        }
                    }
                }
            }

            Lighting.AddLight(Projectile.position, emittedLight);

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            for (int i = 0; i < 4; i++)
            {
                Rectangle sourceRectangle = new Rectangle(
                    frameSize.X * frameVariants[i],
                    frameSize.Y * Projectile.frame,
                    frameSize.X,
                    frameSize.Y);
                Rectangle destination = new Rectangle(
                    (int)(Projectile.position.X - Main.screenPosition.X),
                    (int)(Projectile.position.Y - Main.screenPosition.Y),
                    (int)halfSize.X,
                    (int)halfSize.Y);

                float rotation = MathHelper.PiOver2 * i;

                var drawData = new DrawData(
                    texture,
                    destination,
                    sourceRectangle,
                    Color.White,
                    rotation + Projectile.rotation,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0);
                Main.EntitySpriteDraw(drawData);
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
    }
}