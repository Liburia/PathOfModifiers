using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using PathOfModifiers.AffixesItem;
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
            if (projectile.owner < 255)
            {
            }
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
    }
}
