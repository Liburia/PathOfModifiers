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

        DoTInstanceCollection dotInstanceCollection = new DoTInstanceCollection();

        float moveSpeedBuffMultiplier = 1;
        public bool moveSpeedBuff = false;

        public void AddDoTBuff(Player player, DamageOverTime buff, int dps, int durationTicks, bool syncMP = true)
        {
            Type dotBuffType = buff.GetType();
            double durationMs = (durationTicks / 60f) * 1000;
            dotInstanceCollection.AddInstance(dotBuffType, dps, durationMs);

            if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
            {
                BuffPacketHandler.CSendAddDoTBuffPlayer(player.whoAmI, buff.Type, dps, durationTicks);
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
            dotInstanceCollection = new DoTInstanceCollection();
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

            dotInstanceCollection.ResetEffects();

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
            foreach (var kv in dotInstanceCollection.dotInstances)
            {
                Type type = kv.Key;
                int dps = kv.Value.dps;
                if (dps > 0)
                {
                    int debuffDamage = (int)Math.Round(dps * DamageOverTime.damageMultiplierHalfSecond);
                    player.lifeRegenTime = 0;
                    if (player.lifeRegen > 0)
                    {
                        player.lifeRegen = 0;
                    }
                    player.lifeRegen -= debuffDamage;

                    //TODO: this only works with buffs from this mod; could use BuffLoader.buffs
                    player.AddBuff(mod.BuffType(type.Name), 2, true);
                }
            }
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (removeDebuffs)
            {
                dotInstanceCollection.Clear();
            }
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound
            {
                { "dotInstanceCollection", dotInstanceCollection },
                { "moveSpeedBuffMultiplier", moveSpeedBuffMultiplier }
            };

            return tag;
        }
        public override void Load(TagCompound tag)
        {
            dotInstanceCollection = tag.Get<DoTInstanceCollection>("dotInstanceCollection");
            moveSpeedBuffMultiplier = tag.GetFloat("moveSpeedBuffMultiplier");
        }
    }
}