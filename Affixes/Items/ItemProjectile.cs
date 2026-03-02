using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Items;
using PathOfModifiers.Rarities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items
{
    public class ItemProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is IEntitySource_WithStatsFromItem fromItem)
            {
                Item item = fromItem.Item;
                Player player = fromItem.Player;
                if (item.TryGetGlobalItem<ItemItem>(out var pomItem))
                {
                    pomItem.ProjOnSpawn(item, player, projectile, source);
                }
            }
        }
    }
}
