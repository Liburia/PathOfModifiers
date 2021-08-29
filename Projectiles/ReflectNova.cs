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
    public class ReflectNova : ModProjectile, INonTriggerringProjectile
    {
        public static float startScale = 0.2f;
        public static float maxScale = 3;
        public static float scaleIncrease = 0.3f;
        public static Vector2 size = new Vector2(400, 400);
        public static float collisionScale = 0.7f;

        bool[] hitNPCs = new bool[Main.maxNPCs];
        bool hitPlayer;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Nova");
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.scale = startScale;
            Projectile.Size = size * collisionScale * startScale;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            if (Projectile.scale < maxScale)
            {
                Projectile.scale += scaleIncrease;
                if (Projectile.scale >= maxScale)
                    Projectile.scale = maxScale;
                Projectile.Size = size * collisionScale * Projectile.scale;
                Projectile.position -= new Vector2(texture.Width, frameHeight) * collisionScale * scaleIncrease / 2;
            }
            else
            {
                Projectile.Kill();
            }


            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            for (int i = 0; i < 5; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ReflectNovaDebris>());


            if (Main.netMode != NetmodeID.Server)
            {
                Player owner = Main.player[Projectile.owner];

                Player player = Main.LocalPlayer;
                if (!hitPlayer && PoMUtil.CanHitPvp(owner, player))
                {
                    Rectangle localRect = player.getRect();
                    if (localRect.Intersects(Projectile.Hitbox))
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(Projectile.owner), Projectile.damage + (int)Math.Round(player.statLife * Projectile.ai[0]), player.direction, true);
                        hitPlayer = true;
                    }
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        NPC realNPC = npc.realLife >= 0 ? Main.npc[npc.realLife] : npc;
                        if (!hitNPCs[realNPC.whoAmI] && PoMUtil.CanHitNPC(npc))
                        {
                            Rectangle npcRect = npc.getRect();
                            if (npcRect.Intersects(Projectile.Hitbox))
                            {
                                owner.ApplyDamageToNPC(npc, Projectile.damage + (int)Math.Round(realNPC.lifeMax * Projectile.ai[0]), 1, npc.direction, false);
                                hitNPCs[realNPC.whoAmI] = true;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Projectile.GetAlpha(lightColor);
            var drawData = new DrawData(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
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
    }
}