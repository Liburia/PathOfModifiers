using Microsoft.Xna.Framework;
using PathOfModifiers.AffixesItem;
using PathOfModifiers.Buffs;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;

namespace PathOfModifiers
{
    public class PoMPlayer : ModPlayer
    {
        #region Stats
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

        public Entity lastDamageDealer;

        /// <summary>
        /// Stores the damage of the hit that procced the debuff.
        /// </summary>
        Dictionary<Type, int> dotBuffInstances = new Dictionary<Type, int>();
        public bool dotBuffActive = false;

        float moveSpeedBuffMultiplier = 1;
        public bool moveSpeedBuff = false;

        public void AddDoTBuff(Player player, DamageOverTime buff, int damage, int time, bool syncMP = true)
        {
            Type buffType = buff.GetType();
            if (dotBuffInstances.TryGetValue(buffType, out int dddDamage))
            {
                if (damage > dddDamage || !player.HasBuff(buff.Type))
                    dotBuffInstances[buffType] = damage;
            }
            else
            {
                dotBuffInstances.Add(buffType, damage);
            }
            player.AddBuff(buff.Type, time, true);

            if (Main.netMode != NetmodeID.MultiplayerClient && syncMP)
            {
                BuffPacketHandler.CSendAddDamageDoTDebuffPlayer(player.whoAmI, buff.Type, damage, time);
            }
        }
        public void AddMoveSpeedBuff(Player player, float speedMultiplier, int time, bool syncMP = true, int ignoreClient = -1)
        {
            moveSpeedBuffMultiplier = speedMultiplier;
            player.AddBuff(ModContent.BuffType<MoveSpeed>(), time, true);

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                BuffPacketHandler.CSendAddMoveSpeedBuffPlayer(player.whoAmI, speedMultiplier, time);
            }
        }

        public override void Initialize()
        {
            lastDamageDealer = null;
            dotBuffInstances = new Dictionary<Type, int>();
        }

        public override void OnEnterWorld(Player player)
        {
            if (Main.netMode == 1)
            {
                ModPacketHandler.CPlayerConnected();
            }
        }
        public override void PlayerConnect(Player player)
        {
            //PathOfModifiers.Instance.Logger.Debug($"PlayerConnect: {Main.netMode}");
            if (Main.LocalPlayer == player)
            {
                var mapBorder = ModContent.GetInstance<MapBorder>();
                MapBorder.ClearActiveBounds();
            }
        }
        public override void PlayerDisconnect(Player player)
        {
            //PathOfModifiers.Instance.Logger.Debug($"PlayerDisconnect: {Main.netMode}");

            if (Main.LocalPlayer == player)
            {
                var mapBorder = ModContent.GetInstance<MapBorder>();
                MapBorder.ClearActiveBounds();
            }
        }

        public override bool ConsumeAmmo(Item weapon, Item ammo)
        {
            Item item;
            PoMItem pomItem;
            //TODO: Does this mean the affix will apply when in the inventory unless it has logic itself? Check and fix.
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                if (!pomItem.PlayerConsumeAmmo(player, item, ammo))
                    return false;
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
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

                pomItem = item.GetGlobalItem<PoMItem>();
                if (!pomItem.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
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

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.NaturalLifeRegen(item, player, ref regenMultiplier);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.NaturalLifeRegen(item, player, ref regenMultiplier);
            }
            regen = (regen * regenMultiplier) + (player.lifeRegen * regenMultiplier) - player.lifeRegen;
        }

        public override void GetWeaponCrit(Item heldItem, ref int crit)
        {
            Item item;
            PoMItem pomItem;
            float multiplier = 1f;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.PlayerGetWeaponCrit(item, heldItem, player, ref multiplier);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.PlayerGetWeaponCrit(item, heldItem, player, ref multiplier);
            }
            crit = (int)Math.Round(crit * multiplier);
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

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.ModifyHitByNPC(item, player, npc, ref damage, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
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

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.OnHitByNPC(item, player, npc, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.OnHitByNPC(item, player, npc, damage, crit);
            }
        }

        public void OnKillNPC(NPC target)
        {
            Item affixItem;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerOnKillNPC(affixItem, player, target);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerOnKillNPC(affixItem, player, target);
            }
        }
        public void OnKillPvp(Player target)
        {
            Item affixItem;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerOnKillPvp(affixItem, player, target);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerOnKillPvp(affixItem, player, target);
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

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
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

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
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

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.ProjModifyHitNPC(item, player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
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

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.ProjModifyHitPvp(item, player, proj, target, ref damage, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.ProjModifyHitPvp(item, player, proj, target, ref damage, ref crit);
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<PoMNPC>().lastDamageDealer = player;

            Item affixItem;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
        }
        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            target.GetModPlayer<PoMPlayer>().lastDamageDealer = player;

            Item affixItem;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                pomItem.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<PoMNPC>().lastDamageDealer = player;

            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.ProjOnHitNPC(item, player, proj, target, damage, knockback, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.ProjOnHitNPC(item, player, proj, target, damage, knockback, crit);
            }
        }
        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            target.GetModPlayer<PoMPlayer>().lastDamageDealer = player;

            Item item;
            PoMItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.ProjOnHitPvp(item, player, proj, target, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<PoMItem>();
                pomItem.ProjOnHitPvp(item, player, proj, target, damage, crit);
            }
        }
        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Item affixItem;
            PoMItem pomItem;
            bool shoot = true;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                if (!pomItem.PlayerShoot(affixItem, player, item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack))
                    shoot = false;
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<PoMItem>();
                if (!pomItem.PlayerShoot(affixItem, player, item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack))
                    shoot = false;
            }
            return shoot;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            Player lastDamageDealerPlayer = lastDamageDealer as Player;
            if (lastDamageDealerPlayer != null)
            {
                lastDamageDealerPlayer.GetModPlayer<PoMPlayer>().OnKillPvp(player);
            }
        }

        public override void ResetEffects()
        {
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


            dotBuffActive = false;
            moveSpeedBuff = false;
        }
        public override void PostUpdateEquips()
        {
            if (moveSpeedBuff)
            {
                moveSpeed += moveSpeedBuffMultiplier - 1;
            }

            player.meleeDamage *= meleeDamage;
            player.magicDamage *= magicDamage;
            player.rangedDamage *= rangedDamage;
            player.thrownDamage *= throwingDamage;
            player.minionDamage *= minionDamage;

            player.meleeSpeed *= meleeSpeed;
            player.pickSpeed *= pickSpeed;

            player.moveSpeed *= moveSpeed;
            player.maxRunSpeed *= moveSpeed;

            player.potionDelayTime = (int)Math.Round(player.potionDelayTime * potionDelayTime);
            player.restorationDelayTime = (int)Math.Round(player.restorationDelayTime * restorationDelayTime);
        }
        public override void UpdateBadLifeRegen()
        {
            int debuffDamage;
            if (dotBuffActive)
            {
                foreach (var dotInstance in dotBuffInstances.Values)
                {
                    debuffDamage = (int)Math.Round(dotInstance * DamageOverTime.damageMultiplierHalfSecond);
                    player.lifeRegen = 0;
                    player.lifeRegenTime = 0;
                    player.lifeRegen -= debuffDamage;
                }
            }
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            var dotTypeNames = new List<string>(dotBuffInstances.Count);
            foreach (var dotType in dotBuffInstances.Keys)
            {
                dotTypeNames.Add(dotType.AssemblyQualifiedName);
            }
            tag.Add("dotTypeNames", dotTypeNames);
            tag.Add("dotValues", dotBuffInstances.Values.ToList());
            tag.Add("moveSpeedBuffMultiplier", moveSpeedBuffMultiplier);

            return tag;
        }
        public override void Load(TagCompound tag)
        {
            //Load the DoT damages dictionary discarding non-existant buff types and buffs that don't inherit from DamageOverTime
            var dotTypeNames = tag.Get<List<string>>("dotTypeNames");
            var dotTypes = new List<Type>(dotTypeNames.Count);
            var dotValues = tag.Get<List<int>>("dotValues");
            var dvRemoveIndicies = new Stack<int>();
            for (int i = 0; i < dotTypeNames.Count; i++)
            {
                var dotTypeName = dotTypeNames[i];
                Type dotType = Type.GetType(dotTypeName, false);
                if (dotType == null || !dotType.IsSubclassOf(typeof(DamageOverTime)))
                {
                    dvRemoveIndicies.Push(i);
                    mod.Logger.Warn("Buff \"{dotTypeName}\" not found.");
                }
                else
                {
                    dotTypes.Add(dotType);
                }
            }
            while (dvRemoveIndicies.Count > 0)
            {
                dotValues.RemoveAt(dvRemoveIndicies.Pop());
            }

            dotBuffInstances = dotTypes.Zip(dotValues, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);

            moveSpeedBuffMultiplier = tag.GetFloat("moveSpeedBuffMultiplier");
        }
    }
}