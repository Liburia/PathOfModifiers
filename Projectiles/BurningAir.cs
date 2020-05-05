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
    public class BurningAir : ModProjectile, INonTriggerringProjectile
    {
        static readonly Vector3 emittedLight = new Vector3(1f, 0.611f, 0f);
        const float airRadius = 48f;
        const float airDiameter = airRadius * 2;


        Rectangle airRect;
        bool init;

        public override bool Autoload(ref string name)
        {
            name = GetType().Name;
            return true;
        }

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
                        PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
                        pomPlayer.AddBurningAirBuff(player, projectile.damage);
                    }
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active)
                {
                    Rectangle npcRect = npc.getRect();
                    if (npcRect.Intersects(airRect))
                    {
                        PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
                        pomNPC.AddBurningAirBuff(npc, projectile.damage);
                    }
                }
            }

            Lighting.AddLight(projectile.Center, emittedLight);

            for (int i = 0; i < 1; i++)
            {
                Dust.NewDust(new Vector2(airRect.X, airRect.Y), airRect.Width, airRect.Height, ModContent.DustType<FireDebris>(), Alpha: 100, SpeedY: -1f, Scale: Main.rand.NextFloat(1f, 4f));
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