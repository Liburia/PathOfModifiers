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
        const float dustScarcity = 10000f;


        Rectangle airRect;
        bool init;

        public override void AutoStaticDefaults()
        {
            Terraria.GameContent.TextureAssets.Projectile[Projectile.type] = Terraria.GameContent.TextureAssets.MagicPixel;
            Main.projFrames[Projectile.type] = 1;
            if (DisplayName.IsDefault())
                DisplayName.SetDefault(Regex.Replace(Name, "([A-Z])", " $1").Trim());
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("BurningAir");
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = PoMGlobals.ailmentDuration;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreAI()
        {
            if (!init)
            {
                float airRadius = Projectile.ai[0];
                float airDiameter = airRadius * 2;
                airRect = new Rectangle(
                    (int)(Projectile.position.X - airRadius),
                    (int)(Projectile.position.Y - airRadius),
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
                        player.GetModPlayer<BuffPlayer>().AddBurningAirBuff(player, Projectile.damage);
                    }
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && (!npc.friendly || npc.townNPC) && !npc.dontTakeDamage)
                {
                    Rectangle npcRect = npc.getRect();
                    if (npcRect.Intersects(airRect))
                    {
                        BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
                        pomNPC.AddBurningAirBuff(npc, Projectile.damage);
                    }
                }
            }

            Lighting.AddLight(Projectile.Center, emittedLight);

            float dustsF = (airRect.Width * airRect.Height) / dustScarcity;
            int dusts = (int)Math.Ceiling(dustsF);
            if (Main.rand.NextFloat(1f) <= dustsF)
            {
                for (int i = 0; i < dusts; i++)
                {
                    Dust.NewDustPerfect(new Vector2(Main.rand.NextFloat(airRect.Left, airRect.Right), Main.rand.NextFloat(airRect.Bottom, airRect.Top)), ModContent.DustType<FireDebris>(), Alpha: 130, Velocity: new Vector2(Main.rand.NextFloat(-0.7f, 0.7f), Main.rand.NextFloat(-0.8f, -0.2f)), Scale: Main.rand.NextFloat(1f, 4f));
                }
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
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