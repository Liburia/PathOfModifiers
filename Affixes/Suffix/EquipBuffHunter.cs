using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Suffixes
{
    public class EquipBuffHunter : Suffix
    {
        public override float weight => 0.5f;

        public override string addedText => "of the Hunter";
        public override float addedTextWeight => 0.5f;
        
        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAnyArmor(item) ||
                PoMItem.IsAccessory(item);
        }

        public override void ModifyTooltips(Mod mod, Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "EquipBuffHunter", "Player can see enemies");
            line.overrideColor = color;
            tooltips.Add(line);
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.AddBuff(BuffID.Hunter, 2);
        }

        public override void ReforgePrice(Item item, ref int price)
        {
            price = (int)Math.Round(price * 0.4f * weight);
        }
    }
}
