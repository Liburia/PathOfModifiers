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
using Terraria.ModLoader.IO;

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

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(projectile.scale);
            binaryWriter.Write(projectile.WhipSettings.RangeMultiplier);
            binaryWriter.Write(projectile.WhipSettings.Segments);
        }
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            projectile.scale = binaryReader.ReadSingle();
            projectile.WhipSettings.RangeMultiplier = binaryReader.ReadSingle();
            projectile.WhipSettings.Segments = binaryReader.ReadInt32();
        }
    }
}
