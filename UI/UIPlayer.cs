using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace PathOfModifiers.UI
{
    public class UIPlayer : ModPlayer
    {
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            if (States.ModifierForge.IsOpen)
            {
                if (Systems.UI.ModifierForgeState.fragmentSlot.TryInsertItem(inventory[slot], true, out var oldItem) || Systems.UI.ModifierForgeState.itemSlot.TryInsertItem(inventory[slot], true, out oldItem))
                {
                    inventory[slot] = oldItem;
                }

                return true;
            }

            return false;
        }
    }
}