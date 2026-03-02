using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.UI.ItemSlot;

namespace PathOfModifiers.UI
{
    public class UIPlayer : ModPlayer
    {
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            if (States.ModifierForge.IsOpen)
            {
                if (
                    context
                    is Context.InventoryItem
                    or Context.InventoryCoin
                    or Context.InventoryAmmo
                    or Context.HotbarItem
                    )
                {
                    if (Systems.UI.ModifierForgeState.fragmentSlot.TryInsertItem(inventory[slot], true, out var oldItem) || Systems.UI.ModifierForgeState.itemSlot.TryInsertItem(inventory[slot], true, out oldItem))
                    {
                        inventory[slot] = oldItem;

                        return true;
                    }
                }
            }

            return base.ShiftClickSlot(inventory, context, slot);
        }
    }
}