using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;
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
            Terraria.GameContent.TextureAssets.Projectile[Projectile.type] = Terraria.GameContent.TextureAssets.MagicPixel;
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = baseTimeLeft;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.direction = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.width += sizeAdd;
            Projectile.height += sizeAdd;

            if (Projectile.timeLeft == spawnAirAtTime && Projectile.ai[1] != 0)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChilledAir>(), 0, 0, Projectile.owner, 200f, Projectile.ai[0]);
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 position = Projectile.position + new Vector2(
                    Main.rand.NextFloat(Projectile.width),
                    Main.rand.NextFloat(Projectile.height));
                Dust.NewDustPerfect(
                    position,
                    ModContent.DustType<Dusts.FrostDebris>(),
                    Velocity: Projectile.velocity * 0.03f,
                    Alpha: 100,
                    Scale: Main.rand.NextFloat(2.2f, 3.3f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}