using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class IceBurst : ModProjectile, INonTriggerringProjectile
    {
        const int sizeAdd = 2;
        const int baseTimeLeft = 30;
        const int spawnAirAtTime = baseTimeLeft / 2;

        public override void AutoStaticDefaults()
        {
            Main.projectileTexture[projectile.type] = Main.magicPixel;
            Main.projFrames[projectile.type] = 1;
            if (DisplayName.IsDefault())
                DisplayName.SetDefault(Regex.Replace(Name, "([A-Z])", " $1").Trim());
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FrostPulse");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.timeLeft = baseTimeLeft;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.idStaticNPCHitCooldown = 5;
            projectile.usesIDStaticNPCImmunity = true;
        }

        public override void AI()
        {
            projectile.direction = projectile.velocity.X > 0f ? 1 : -1;
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.width += sizeAdd;
            projectile.height += sizeAdd;

            if (projectile.timeLeft == spawnAirAtTime && projectile.ai[1] != 0)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChilledAir>(), 0, 0, projectile.owner, 200f, projectile.ai[0]);
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 position = projectile.position + new Vector2(
                    Main.rand.NextFloat(projectile.width),
                    Main.rand.NextFloat(projectile.height));
                Dust.NewDustPerfect(
                    position,
                    ModContent.DustType<Dusts.FrostDebris>(),
                    Velocity: projectile.velocity * 0.03f,
                    Alpha: 100,
                    Scale: Main.rand.NextFloat(2.2f, 3.3f));
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }
    }
}