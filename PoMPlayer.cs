using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Net;

namespace PathOfModifiers
{
    public class PoMPlayer : ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            if (Main.netMode == 1)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)MsgType.PlayerConnected);
                packet.Write((byte)player.whoAmI);
                packet.Send();
            }
        }

        public void ProjModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjModifyHitNPC(item, player, projectile, target, ref damage, ref knockback, ref crit, ref hitDirection);
            }
        }
        public void ProjModifyHitPvp(Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjModifyHitPvp(item, player, projectile, target, ref damage, ref crit);
            }
        }
        public void ProjOnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjOnHitNPC(item, player, projectile, target, damage, knockback, crit);
            }
        }
        public void ProjOnHitPvp(Projectile projectile, Player target, int damage, bool crit)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjOnHitPvp(item, player, projectile, target, damage, crit);
            }
        }
    }
}