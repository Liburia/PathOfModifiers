using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class FrostPulse : ModProjectile, INonTriggerringProjectile
    {
        const int baseTimeLeft = 600;
        const int timeLeftAfterTileCollide = 5;

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.alpha = 100;
            Projectile.timeLeft = baseTimeLeft;
            Projectile.penetrate = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 position = Projectile.position + new Vector2(
                Main.rand.NextFloat(Projectile.width),
                Main.rand.NextFloat(Projectile.height));
            Dust.NewDustPerfect(
                position,
                ModContent.DustType<Dusts.FrostDebris>(),
                Velocity: Projectile.velocity * 0.2f,
                Alpha: 100,
                Scale: Main.rand.NextFloat(2.2f, 3.3f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle sourceRectangle = texture.Bounds;
            Vector2 origin = sourceRectangle.Size() / 2f;

            var drawData = new DrawData(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(drawData);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Hit(target);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Hit(target);
        }

        void Hit(NPC target)
        {
            target.GetGlobalNPC<BuffNPC>().AddChilledBuff(target, Projectile.ai[0], PoMGlobals.ailmentDuration);
        }
        void Hit(Player target)
        {
            target.GetModPlayer<BuffPlayer>().AddChilledBuff(target, Projectile.ai[0], PoMGlobals.ailmentDuration);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > timeLeftAfterTileCollide)
            {
                Projectile.timeLeft = timeLeftAfterTileCollide;
            }

            Projectile.velocity = oldVelocity;
            Projectile.tileCollide = false;

            return false;
        }

        public override void Kill(int timeLeft)
        {
            PlayKillSound();
        }

        void PlayKillSound()
        {
            SoundEngine.PlaySound(SoundID.Item21.WithVolumeScale(1f).WithPitchOffset(0.3f), Projectile.Center);
        }
    }
}