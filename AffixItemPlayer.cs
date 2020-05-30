using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
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
    public class AffixItemPlayer : ModPlayer
    {
        public class GoldDropChanceColletion
        {
            public class GoldDropChance
            {
                public bool enabled;
                public float chance;
                public int amount;
            }

            public Dictionary<Affix, GoldDropChance> dict = new Dictionary<Affix, GoldDropChance>();

            public void AddOrUpdate(Affix affix, float chance, int amount)
            {
                if (dict.TryGetValue(affix, out GoldDropChance gdc))
                {
                    gdc.enabled = true;
                    gdc.chance = chance;
                    gdc.amount = amount;
                }
                else
                {
                    dict.Add(affix, new GoldDropChance()
                    {
                        enabled = true,
                        chance = chance,
                        amount = amount,
                    });
                }
            }
            public void Clear()
            {
                dict.Clear();
            }
            public void ClearDisabled()
            {
                dict = dict
                 .Where(kv => kv.Value.enabled)
                 .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            public void ResetEffects()
            {
                foreach (var pair in dict)
                {
                    pair.Value.enabled = false;
                }
            }
            public int Roll()
            {
                int totalDrop = 0;
                foreach (var kv in dict)
                {
                    if (kv.Value.enabled && Main.rand.NextFloat(1f) < kv.Value.chance)
                    {
                        totalDrop += kv.Value.amount;
                    }
                }
                return totalDrop;
            }
        }

        #region Stats
        public float meleeSpeed;
        public float pickSpeed;

        public float moveSpeed;

        public float potionDelayTime;
        public float restorationDelayTime;

        public float blockChance;

        public float reflectMeleeDamage;

        public GoldDropChanceColletion goldDropChances;
        #endregion

        public Player lastDamageDealer;

        public override void Initialize()
        {
            lastDamageDealer = null;
            goldDropChances = new GoldDropChanceColletion();
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
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                if (!pomItem.PlayerConsumeAmmo(player, item, ammo))
                    return false;
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                if (!pomItem.PlayerConsumeAmmo(player, item, ammo))
                    return false;
            }
            return true;
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            Item item;
            AffixItemItem pomItem;
            float damageMultiplier = 1f;
            bool hurt = true;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                if (!pomItem.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                if (!pomItem.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            damage = (int)Math.Round(damage * damageMultiplier);

            if (hurt)
            {
                hurt = Main.rand.NextFloat(1) >= blockChance;
                PoMUtil.MakeImmune(player, (int)PoMUtil.PlayerImmuneTime.Parry);
            }

            return hurt;
        }
        public override void NaturalLifeRegen(ref float regen)
        {
            Item item;
            AffixItemItem pomItem;
            float regenMultiplier = 1;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.NaturalLifeRegen(item, player, ref regenMultiplier);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.NaturalLifeRegen(item, player, ref regenMultiplier);
            }
            regen = (regen * regenMultiplier) + (player.lifeRegen * regenMultiplier) - player.lifeRegen;
        }

        public override void GetWeaponCrit(Item heldItem, ref int crit)
        {
            Item item;
            AffixItemItem pomItem;
            float multiplier = 1f;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerGetWeaponCrit(item, heldItem, player, ref multiplier);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerGetWeaponCrit(item, heldItem, player, ref multiplier);
            }
            crit = (int)Math.Round(crit * multiplier);
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();

            Item item;
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.ModifyHitByNPC(item, player, npc, ref damage, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.ModifyHitByNPC(item, player, npc, ref damage, ref crit);
            }

        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            Item item;
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.OnHitByNPC(item, player, npc, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.OnHitByNPC(item, player, npc, damage, crit);
            }

            if (reflectMeleeDamage > 0)
            {
                int reflectDamage = (int)Math.Round(damage * reflectMeleeDamage);
                float reflectKnockback = 5;
                int reflectDirection = npc.Center.X > player.Center.X ? 1 : -1;
                npc.StrikeNPC(reflectDamage, reflectKnockback, reflectDirection, crit);
            }
        }
        public void ModifyHitByPvp(Player attacker, ref int damage, ref bool crit)
        {
            Item item;
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.ModifyHitByPvp(item, player, attacker, ref damage, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.ModifyHitByPvp(item, player, attacker, ref damage, ref crit);
            }

        }
        public void OnHitByPvp(Player attacker, int damage, bool crit)
        {
            Item item;
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                item = player.inventory[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.OnHitByPvp(item, player, attacker, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                item = player.armor[i];
                if (item.type == 0 || item.stack == 0)
                    continue;

                pomItem = item.GetGlobalItem<AffixItemItem>();
                pomItem.OnHitByPvp(item, player, attacker, damage, crit);
            }


            if (reflectMeleeDamage > 0)
            {
                int reflectDamage = (int)Math.Round(damage * reflectMeleeDamage);
                int reflectDirection = attacker.Center.X > player.Center.X ? 1 : -1;
                attacker.Hurt(PlayerDeathReason.ByPlayer(attacker.whoAmI), reflectDamage, reflectDirection, true, false, crit);
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
        }
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            base.OnHitByProjectile(proj, damage, crit);
        }

        public void OnKillNPC(NPC target)
        {
            Item affixItem;
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerOnKillNPC(affixItem, player, target);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerOnKillNPC(affixItem, player, target);
            }
        }
        public void OnKillPvp(Player target)
        {
            Item affixItem;
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerOnKillPvp(affixItem, player, target);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerOnKillPvp(affixItem, player, target);
            }
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            target.GetGlobalNPC<AffixItemNPC>().LastDamageDealer = player;

            Item affixItem;
            AffixItemItem pomItem;
            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            knockback *= knockbackMultiplier;
        }
        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            target.GetModPlayer<AffixItemPlayer>().lastDamageDealer = player;

            Item affixItem;
            AffixItemItem pomItem;
            float damageMultiplier = 1f;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);

            target.GetModPlayer<AffixItemPlayer>().ModifyHitByPvp(player, ref damage, ref crit);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            target.GetGlobalNPC<AffixItemNPC>().LastDamageDealer = player;

            if (!(proj.modProjectile is Projectiles.INonTriggerringProjectile))
            {
                Item item;
                AffixItemItem pomItem;
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    item = player.inventory[i];
                    if (item.type == 0 || item.stack == 0)
                        continue;

                    pomItem = item.GetGlobalItem<AffixItemItem>();
                    pomItem.ProjModifyHitNPC(item, player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
                }
                for (int i = 0; i < player.armor.Length; i++)
                {
                    item = player.armor[i];
                    if (item.type == 0 || item.stack == 0)
                        continue;

                    pomItem = item.GetGlobalItem<AffixItemItem>();
                    pomItem.ProjModifyHitNPC(item, player, proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
                }
            }
        }
        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            target.GetModPlayer<AffixItemPlayer>().lastDamageDealer = player;

            if (!(proj.modProjectile is Projectiles.INonTriggerringProjectile))
            {
                Item item;
                AffixItemItem pomItem;
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    item = player.inventory[i];
                    if (item.type == 0 || item.stack == 0)
                        continue;

                    pomItem = item.GetGlobalItem<AffixItemItem>();
                    pomItem.ProjModifyHitPvp(item, player, proj, target, ref damage, ref crit);
                }
                for (int i = 0; i < player.armor.Length; i++)
                {
                    item = player.armor[i];
                    if (item.type == 0 || item.stack == 0)
                        continue;

                    pomItem = item.GetGlobalItem<AffixItemItem>();
                    pomItem.ProjModifyHitPvp(item, player, proj, target, ref damage, ref crit);
                }
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            Item affixItem;
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
        }
        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            Item affixItem;
            AffixItemItem pomItem;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                pomItem.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }

            target.GetModPlayer<AffixItemPlayer>().OnHitByPvp(player, damage, crit);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (!(proj.modProjectile is Projectiles.INonTriggerringProjectile))
            {
                Item item;
                AffixItemItem pomItem;
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    item = player.inventory[i];
                    if (item.type == 0 || item.stack == 0)
                        continue;

                    pomItem = item.GetGlobalItem<AffixItemItem>();
                    pomItem.ProjOnHitNPC(item, player, proj, target, damage, knockback, crit);
                }
                for (int i = 0; i < player.armor.Length; i++)
                {
                    item = player.armor[i];
                    if (item.type == 0 || item.stack == 0)
                        continue;

                    pomItem = item.GetGlobalItem<AffixItemItem>();
                    pomItem.ProjOnHitNPC(item, player, proj, target, damage, knockback, crit);
                }
            }
        }
        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if (!(proj.modProjectile is Projectiles.INonTriggerringProjectile))
            {

                Item item;
                AffixItemItem pomItem;
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    item = player.inventory[i];
                    if (item.type == 0 || item.stack == 0)
                        continue;

                    pomItem = item.GetGlobalItem<AffixItemItem>();
                    pomItem.ProjOnHitPvp(item, player, proj, target, damage, crit);
                }
                for (int i = 0; i < player.armor.Length; i++)
                {
                    item = player.armor[i];
                    if (item.type == 0 || item.stack == 0)
                        continue;

                    pomItem = item.GetGlobalItem<AffixItemItem>();
                    pomItem.ProjOnHitPvp(item, player, proj, target, damage, crit);
                }
            }
        }
        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Item affixItem;
            AffixItemItem pomItem;
            bool shoot = true;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                affixItem = player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
                if (!pomItem.PlayerShoot(affixItem, player, item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack))
                    shoot = false;
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                affixItem = player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                pomItem = affixItem.GetGlobalItem<AffixItemItem>();
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
                lastDamageDealerPlayer.GetModPlayer<AffixItemPlayer>().OnKillPvp(player);
            }
        }

        public override void ResetEffects()
        {
            meleeSpeed = 1;
            pickSpeed = 1;

            moveSpeed = 1;

            potionDelayTime = 1;
            restorationDelayTime = 1;

            Main.NewText(blockChance);
            blockChance = 0;

            reflectMeleeDamage = 0;

            goldDropChances.ResetEffects();
            if (Main.time == 0)
            {
                goldDropChances.ClearDisabled();
            }
        }
        public override void PostUpdateEquips()
        {
            player.meleeSpeed *= meleeSpeed;
            player.pickSpeed *= pickSpeed;

            player.potionDelayTime = (int)Math.Round(player.potionDelayTime * potionDelayTime);
            player.restorationDelayTime = (int)Math.Round(player.restorationDelayTime * restorationDelayTime);
        }
        public override void PostUpdateRunSpeeds()
        {
            player.runAcceleration *= moveSpeed;
            player.maxRunSpeed *= moveSpeed;
            player.accRunSpeed *= moveSpeed;
        }
        public override void UpdateBadLifeRegen()
        {
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
        }

        //public override TagCompound Save()
        //{
        //    TagCompound tag = new TagCompound
        //    {
        //        { "dotInstanceCollection", dotInstanceCollection },
        //        { "moveSpeedBuffMultiplier", moveSpeedBuffMultiplier }
        //    };

        //    return tag;
        //}
        //public override void Load(TagCompound tag)
        //{
        //    dotInstanceCollection = tag.Get<DoTInstanceCollection>("dotInstanceCollection");
        //    moveSpeedBuffMultiplier = tag.GetFloat("moveSpeedBuffMultiplier");
        //}
    }
}