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
        static readonly Vector3 emittedLight = new Vector3(1, 0.952f, 0.552f);
        public static float size = 300;
        public static float hitRadiusSqr = (size * 0.5f) * (size * 0.5f);
        public static Point frameSize = new Point(32, 32);
        public static Point frameNumber = new Point(4, 4);
        public static int frameTime = 3;

        HashSet<Entity> hitEntities = new HashSet<Entity>();
        bool init;
        int[] frameVariants = new int[4];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("StaticStrike");
        }

        public override void SetDefaults()
        {
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 40;
            projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;

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
                    Dust.NewDustPerfect(projectile.position, ModContent.DustType<Dusts.LightningDebris>(), dustVelocity, 0, Color.White, Scale: Main.rand.NextFloat(0.8f, 1.6f));
                }
                init = true;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter >= frameTime)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= frameNumber.Y)
                {
                    projectile.Kill();
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Player owner = Main.player[projectile.owner];

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !player.dead && player != owner && (player.team != owner.team || player.team == 0) && player.hostile && owner.hostile)
                    {
                        if (!hitEntities.Contains(player) && (player.Center - projectile.position).LengthSquared() < hitRadiusSqr)
                        {
                            player.Hurt(PlayerDeathReason.ByPlayer(projectile.owner), projectile.damage, player.direction, true);
                            hitEntities.Add(player);
                        }
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.townNPC)
                        {
                            if (!hitEntities.Contains(npc) && (npc.Center - projectile.position).LengthSquared() < hitRadiusSqr)
                            {
                                owner.ApplyDamageToNPC(npc, projectile.damage, 1, npc.direction, false);
                                hitEntities.Add(npc);
                            }
                        }
                    }
                }
            }

            Lighting.AddLight(projectile.position, emittedLight);

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            float halfSize = size / 2;
            for (int i = 0; i < 4; i++)
            {
                Rectangle sourceRectangle = new Rectangle(
                    frameSize.X * frameVariants[i],
                    frameSize.Y * projectile.frame,
                    frameSize.X,
                    frameSize.Y);
                Rectangle destination = new Rectangle(
                    (int)(projectile.position.X - Main.screenPosition.X),
                    (int)(projectile.position.Y - Main.screenPosition.Y),
                    (int)halfSize,
                    (int)halfSize);

                float rotation = MathHelper.PiOver2 * i;

                spriteBatch.Draw(
                    texture,
                    destination,
                    sourceRectangle,
                    Color.White,
                    rotation + projectile.rotation,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0f);
            }

            return false;
        }
    }
}