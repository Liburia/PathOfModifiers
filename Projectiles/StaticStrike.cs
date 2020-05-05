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
                Rectangle hitRect = new Rectangle(
                    (int)(projectile.position.X - halfSize.X),
                    (int)(projectile.position.Y - halfSize.Y),
                    (int)halfSize.X,
                    (int)halfSize.Y);
                Player owner = Main.player[projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMHelper.CanHitPvp(owner, player))
                {
                    if (player.getRect().Intersects(hitRect))
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(projectile.owner), projectile.damage, player.direction, true);
                        hitEntities.Add(player);
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMHelper.CanHitNPC(npc))
                        {
                            Rectangle npcRect = npc.getRect();
                            if (npcRect.Intersects(hitRect))
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
                    (int)halfSize.X,
                    (int)halfSize.Y);

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