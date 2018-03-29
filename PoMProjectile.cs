using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using PathOfModifiers.Affixes;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Rarities;

namespace PathOfModifiers
{
    public class PoMProjectile : GlobalProjectile
    {
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            //TODO: bounce/split/ohhit
            return base.OnTileCollide(projectile, oldVelocity);
        }
        public override void Kill(Projectile projectile, int timeLeft)
        {
            //TODO: bounce/split/ohhit
        }
        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox)
        {
            //TODO: maybe size edit or just use .scale
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.owner < 255)
            {
                Main.player[projectile.owner].GetModPlayer<PoMPlayer>(mod).ProjModifyHitNPC(projectile, target, ref damage, ref knockback, ref crit, ref hitDirection);
            }
        }
        public override void ModifyHitPvp(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            if (projectile.owner < 255)
            {
                Main.player[projectile.owner].GetModPlayer<PoMPlayer>(mod).ProjModifyHitPvp(projectile, target, ref damage, ref crit);
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner < 255)
            {
                Main.player[projectile.owner].GetModPlayer<PoMPlayer>(mod).ProjOnHitNPC(projectile, target, damage, knockback, crit);
            }
        }
        public override void OnHitPvp(Projectile projectile, Player target, int damage, bool crit)
        {
            if (projectile.owner < 255)
            {
                Main.player[projectile.owner].GetModPlayer<PoMPlayer>(mod).ProjOnHitPvp(projectile, target, damage, crit);
            }
        }
    }
}
