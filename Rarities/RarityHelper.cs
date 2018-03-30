using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public static class RarityHelper
    {
        public static bool CanRollWeapon(Item item)
        {
            return PoMItem.IsWeapon(item);
        }
        public static bool CanRollArmor(Item item)
        {
            return PoMItem.IsAnyArmor(item);
        }
        public static bool CanRollAccessory(Item item)
        {
            return PoMItem.IsAccessory(item);
        }
    }
}
