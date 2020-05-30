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
            return AffixItemItem.IsWeapon(item);
        }
        public static bool CanRollArmor(Item item)
        {
            return AffixItemItem.IsAnyArmor(item);
        }
        public static bool CanRollAccessory(Item item)
        {
            return AffixItemItem.IsAccessory(item);
        }
        public static bool CanRollMap(Item item)
        {
            return AffixItemItem.IsMap(item);
        }
        public static bool CanRollNPC(NPC npc)
        {
            return true;    //TODO: ALL THIS SHIT LIKE BILLIONS OF THIS
        }
    }
}
