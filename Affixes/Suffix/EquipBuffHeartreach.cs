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
    public class EquipBuffHeartreach : Suffix
    {
        public override float weight => 0.5f;

        public override string addedText => "of Heartreach";
        public override float addedTextWeight => 0.5f;
        
        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAnyArmor(item) ||
                PoMItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            return "Player magnetizes hearts";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.AddBuff(BuffID.Heartreach, 2);
        }

        public override void ReforgePrice(Item item, ref int price)
        {
            price = (int)Math.Round(price * 0.4f * weight);
        }
    }
}
