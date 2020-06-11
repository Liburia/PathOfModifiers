using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Dusts;
using Terraria;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;
using System.Text.RegularExpressions;

namespace PathOfModifiers.Projectiles
{
    public class ChilledAir : ModProjectile, INonTriggerringProjectile
    {
        static readonly Vector3 emittedLight = new Vector3(0.094f, 0.749f, 0.933f);
        const float dustScarcity = 1000000;

        Rectangle airRect;
        bool init;

        public override void AutoStaticDefaults()
        {
            Main.projectileTexture[projectile.type] = Main.magicPixel;
            Main.projFrames[projectile.type] = 1;
            if (DisplayName.IsDefault())
                DisplayName.SetDefault(Regex.Replace(Name, "([A-Z])", " $1").Trim());
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("BurningAir");
        }

        public override void SetDefaults()
        {
            projectile.timeLeft = PathOfModifiers.ailmentDuration;
            projectile.hostile = false;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreAI()
        {
            if (!init)
            {
                float airRadius = projectile.ai[0];
                float airDiameter = airRadius * 2;
                airRect = new Rectangle(
                    (int)(projectile.position.X - airRadius),
                    (int)(projectile.position.Y - airRadius),
                    (int)(airDiameter),
                    (int)(airDiameter));
                init = true;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    Rectangle playerRect = player.getRect();
                    if (playerRect.Intersects(airRect))
                    {
                        player.GetModPlayer<BuffPlayer>().AddChilledAirBuff(player, projectile.ai[1]);
                    }
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.dontTakeDamage)
                {
                    Rectangle npcRect = npc.getRect();
                    if (npcRect.Intersects(airRect))
                    {
                        BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
                        pomNPC.AddChilledAirBuff(npc, projectile.ai[1]);
                    }
                }
            }

            Lighting.AddLight(projectile.Center, emittedLight);

            float dustsF = (airRect.Width * airRect.Height) / dustScarcity;
            int dusts = (int)Math.Ceiling(dustsF);
            if (Main.rand.NextFloat(1f) <= dustsF)
            {
                for (int i = 0; i < dusts; i++)
                {
                    Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(airRect.Left, airRect.Right), Main.rand.NextFloat(airRect.Bottom, airRect.Top)), ModContent.DustType<FrostCloud>(), Alpha: 100, Scale: Main.rand.NextFloat(1f, 7f));
                }
            }

            return false;
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
    }
}