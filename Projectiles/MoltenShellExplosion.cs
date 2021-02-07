using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class MoltenShellExplosion : ModProjectile, INonTriggerringProjectile
    {
        bool[] hitNPCs = new bool[Main.maxNPCs];
        bool hitPlayer;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Shell Explosion");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.scale = 5f;
            projectile.Size = new Vector2(98, 98) * 0.2f * projectile.scale;
            projectile.timeLeft = 600;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            drawHeldProjInFrontOfHeldItemAndArms = true;
        }

        public override void AI()
        {
            Main.player[projectile.owner].heldProj = projectile.whoAmI;

            projectile.frameCounter++;
            if (projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > Main.projFrames[projectile.type])
                {
                    projectile.Kill();
                }
            }

            for (int i = 0; i < 5; i++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<FireDebris>());


            if (Main.netMode != NetmodeID.Server)
            {
                Player owner = Main.player[projectile.owner];

                Player player = Main.LocalPlayer;
                if (!hitPlayer && PoMUtil.CanHitPvp(owner, player))
                {
                    Rectangle localRect = player.getRect();
                    if (localRect.Intersects(projectile.Hitbox))
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(projectile.owner), projectile.damage + (int)Math.Round(player.statLife * projectile.ai[0]), player.direction, true);
                        hitPlayer = true;
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        NPC realNPC = npc.realLife >= 0 ? Main.npc[npc.realLife] : npc;
                        if (!hitNPCs[realNPC.whoAmI] && PoMUtil.CanHitNPC(npc))
                        {
                            Rectangle npcRect = npc.getRect();
                            if (npcRect.Intersects(projectile.Hitbox))
                            {
                                owner.ApplyDamageToNPC(npc, projectile.damage + (int)Math.Round(realNPC.lifeMax * projectile.ai[0] * 0.10f), 1, npc.direction, false);
                                hitNPCs[realNPC.whoAmI] = true;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = projectile.GetAlpha(lightColor);
            Main.spriteBatch.Draw(texture,
                projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
                sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);

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
    }
}