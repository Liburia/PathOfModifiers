using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes;
using PathOfModifiers.Buffs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;

namespace PathOfModifiers
{
    public class PoMPlayer : ModPlayer
    {
        #region Stats
        public float meleeCrit;
        public float magicCrit;
        public float rangedCrit;
        public float thrownCrit;

        public float meleeDamage;
        public float magicDamage;
        public float rangedDamage;
        public float throwingDamage;
        public float minionDamage;

        public float meleeSpeed;
        public float pickSpeed;

        public float moveSpeed;

        public float potionDelayTime;
        public float restorationDelayTime;
        #endregion

        /// <summary>
        /// Stores the damage of the hit that procced the debuff.
        /// </summary>
        Dictionary<Type, int> damageDotDebuffDamages = new Dictionary<Type, int>();

        public bool dddDamageDotDebuff = false;

        public void AddDamageDoTBuff(Player player, DamageDoTDebuff buff, int damage, int time, bool syncMP = true, int ignoreClient = -1)
        {
            int dddDamage = 0;
            Type buffType = buff.GetType();
            if (damageDotDebuffDamages.TryGetValue(buffType, out dddDamage))
            {
                if (damage > dddDamage)
                    damageDotDebuffDamages[buffType] = damage;
            }
            else
            {
                damageDotDebuffDamages.Add(buffType, damage);
            }
            player.AddBuff(buff.Type, time, true);

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)MsgType.AddDamageDoTDebuffPlayer);
                packet.Write(player.whoAmI);
                packet.Write(buff.Type);
                packet.Write(damage);
                packet.Write(time);
                packet.Send();
            }
        }

        public override void Initialize()
        {
            damageDotDebuffDamages = new Dictionary<Type, int>();
        }

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

        public override bool ConsumeAmmo(Item weapon, Item ammo)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                if (!pomItem.PlayerConsumeAmmo(player, item, ammo))
                    return false;
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                if (!pomItem.PlayerConsumeAmmo(player, item, ammo))
                    return false;
            }
            return true;
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            Item item;
            PoMItem pomItem;
            float damageMultiplier = 1f;
            bool hurt = true;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                if (!pomItem.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                if (!pomItem.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            return hurt;
        }
        public override void NaturalLifeRegen(ref float regen)
        {
            Item item;
            PoMItem pomItem;
            float regenMultiplier = 1;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.NaturalLifeRegen(item, player, ref regenMultiplier);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.NaturalLifeRegen(item, player, ref regenMultiplier);
            }
            regen = (regen * regenMultiplier) + (player.lifeRegen * regenMultiplier) - player.lifeRegen;
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ModifyHitByNPC(item, player, npc, ref damage, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ModifyHitByNPC(item, player, npc, ref damage, ref crit);
            }
        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.OnHitByNPC(item, player, npc, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.OnHitByNPC(item, player, npc, damage, crit);
            }
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            Item affixItem;
            PoMItem pomItem;
            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>(mod);
                pomItem.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>(mod);
                pomItem.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            knockback *= knockbackMultiplier;
        }
        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            Item affixItem;
            PoMItem pomItem;
            float damageMultiplier = 1f;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>(mod);
                pomItem.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>(mod);
                pomItem.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjModifyHitNPC(item, player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjModifyHitNPC(item, player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
            }
        }
        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjModifyHitPvp(item, player, proj, target, ref damage, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjModifyHitPvp(item, player, proj, target, ref damage, ref crit);
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            Item affixItem;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>(mod);
                pomItem.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>(mod);
                pomItem.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
        }
        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            Item affixItem;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>(mod);
                pomItem.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>(mod);
                pomItem.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjOnHitNPC(item, player, proj, target, damage, knockback, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjOnHitNPC(item, player, proj, target, damage, knockback, crit);
            }
        }
        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjOnHitPvp(item, player, proj, target, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>(mod);
                pomItem.ProjOnHitPvp(item, player, proj, target, damage, crit);
            }
        }

        public override void ResetEffects()
        {
            meleeCrit = 1;
            magicCrit = 1;
            rangedCrit = 1;
            thrownCrit = 1;

            meleeDamage = 1;
            magicDamage = 1;
            rangedDamage = 1;
            throwingDamage = 1;
            minionDamage = 1;

            meleeSpeed = 1;
            pickSpeed = 1;

            moveSpeed = 1;

            potionDelayTime = 1;
            restorationDelayTime = 1;


            dddDamageDotDebuff = false;
        }
        public override void PostUpdateEquips()
        {
            player.meleeCrit = (int)Math.Round(player.meleeCrit * meleeCrit);
            player.magicCrit = (int)Math.Round(player.magicCrit * magicCrit);
            player.rangedCrit = (int)Math.Round(player.rangedCrit * rangedCrit);
            player.thrownCrit = (int)Math.Round(player.thrownCrit * thrownCrit);

            player.meleeDamage *= meleeDamage;
            player.magicDamage *= magicDamage;
            player.rangedDamage *= rangedDamage;
            player.thrownDamage *= throwingDamage;
            player.minionDamage = minionDamage;
            
            player.meleeSpeed *= meleeSpeed;
            player.pickSpeed *= pickSpeed;

            player.moveSpeed *= moveSpeed;

            player.potionDelayTime = (int)Math.Round(player.potionDelayTime * potionDelayTime);
            player.restorationDelayTime = (int)Math.Round(player.restorationDelayTime * restorationDelayTime);
        }
        public override void UpdateBadLifeRegen()
        {
            int debuffDamage;
            if (dddDamageDotDebuff)
            {
                debuffDamage = (int)Math.Round(damageDotDebuffDamages[typeof(DamageDoTDebuff)] * DamageDoTDebuff.damageMultiplierHalfSecond);
                player.lifeRegen -= debuffDamage;
            }
        }
    }
}