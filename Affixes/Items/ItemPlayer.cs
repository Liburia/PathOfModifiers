using log4net.Util;
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

namespace PathOfModifiers.Affixes.Items
{
    public class ItemPlayer : ModPlayer
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
        public float damageTaken;

        public float useSpeed;
        public float meleeSpeed;
        public float pickSpeed;

        public float moveSpeed;

        public float potionDelayTime;
        public float restorationDelayTime;

        public float dodgeChance;

        public float reflectMeleeDamage;

        public GoldDropChanceColletion goldDropChances;
        #endregion

        public Player lastAttackingPlayer;

        public override void Initialize()
        {
            lastAttackingPlayer = null;
            goldDropChances = new GoldDropChanceColletion();
        }

        public override void PlayerConnect(Player player)
        {
            ModPacketHandler.CPlayerConnected();
            //var mapBorder = ModContent.GetInstance<MapBorder>();
            //MapBorder.ClearActiveBounds();
        }
        public override void PlayerDisconnect(Player player)
        {
            //var mapBorder = ModContent.GetInstance<MapBorder>();
            //MapBorder.ClearActiveBounds();
        }

        public override bool ConsumeAmmo(Item weapon, Item ammo)
        {
            bool consume = true;
            Item item;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    consume = consume && modItem.PlayerConsumeAmmo(Player, item, ammo);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    consume = consume && modItem.PlayerConsumeAmmo(Player, item, ammo);
                }
            }
            return consume;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (pvp)
            {
                if (damageSource.SourcePlayerIndex >= 0)
                {
                    lastAttackingPlayer = Main.player[damageSource.SourcePlayerIndex];
                    ModifyHitByPvp(lastAttackingPlayer, ref damage, ref crit);
                }
            }

            Item item;
            float damageMultiplier = damageTaken;
            bool hurt = true;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    if (!modItem.PreHurt(item, Player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                        hurt = false;
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    if (!modItem.PreHurt(item, Player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                        hurt = false;
                }
            }
            damage = (int)Math.Round(damage * damageMultiplier);

            if (hurt)
            {
                hurt = Main.rand.NextFloat(1) >= dodgeChance;
                PoMUtil.MakeImmune(Player, (int)PoMUtil.PlayerImmuneTime.Parry);
            }

            return hurt;
        }
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (pvp)
            {
                OnHitByPvp(lastAttackingPlayer, (int)damage, crit);
            }

            Item item;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PostHurt(item, Player, pvp, quiet, damage, hitDirection, crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PostHurt(item, Player, pvp, quiet, damage, hitDirection, crit);
                }
            }
        }
        public override void NaturalLifeRegen(ref float regen)
        {
            Item item;
            float regenMultiplier = 1;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.NaturalLifeRegen(item, Player, ref regenMultiplier);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.NaturalLifeRegen(item, Player, ref regenMultiplier);
                }
            }
            regen = (regen * regenMultiplier) + (Player.lifeRegen * regenMultiplier) - Player.lifeRegen;
        }

        public override float UseSpeedMultiplier(Item item)
        {
            return useSpeed;
        }
        public override void ModifyWeaponCrit(Item heldItem, ref int crit)
        {
            Item item;
            float multiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyWeaponCrit(item, heldItem, Player, ref multiplier);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyWeaponCrit(item, heldItem, Player, ref multiplier);
                }
            }
            crit = (int)Math.Round(crit * multiplier);
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            Item item;
            float damageMultiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByNPC(item, Player, npc, ref damageMultiplier, ref crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByNPC(item, Player, npc, ref damageMultiplier, ref crit);
                }
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            Item item;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByNPC(item, Player, npc, damage, crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByNPC(item, Player, npc, damage, crit);
                }
            }

            if (reflectMeleeDamage > 0)
            {
                int reflectDamage = (int)Math.Round(damage * reflectMeleeDamage);
                float reflectKnockback = 5;
                int reflectDirection = npc.Center.X > Player.Center.X ? 1 : -1;
                Player.ApplyDamageToNPC(npc, reflectDamage, reflectKnockback, reflectDirection, crit);
            }
        }
        public void ModifyHitByPvp(Player attacker, ref int damage, ref bool crit)
        {
            Item item;
            float damageMultiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByPvp(item, Player, attacker, ref damageMultiplier, ref crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByPvp(item, Player, attacker, ref damageMultiplier, ref crit);
                }
            }
            damage = (int)Math.Round(damage * damageMultiplier);

        }
        public void OnHitByPvp(Player attacker, int damage, bool crit)
        {
            Item item;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByPvp(item, Player, attacker, damage, crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByPvp(item, Player, attacker, damage, crit);
                }
            }


            if (reflectMeleeDamage > 0)
            {
                int reflectDamage = (int)Math.Round(damage * reflectMeleeDamage);
                int reflectDirection = attacker.Center.X > Player.Center.X ? 1 : -1;
                attacker.Hurt(PlayerDeathReason.ByPlayer(attacker.whoAmI), reflectDamage, reflectDirection, true, false, crit);
                attacker.immune = false;
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            Item item;
            float damageMultiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByProjectile(item, Player, proj, ref damageMultiplier, ref crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByProjectile(item, Player, proj, ref damageMultiplier, ref crit);
                }
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            Item item;
            
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByProjectile(item, Player, proj, damage, crit);
                }

            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByProjectile(item, Player, proj, damage, crit);
                }

            }
        }

        public void OnKillNPC(NPC target)
        {
            Item affixItem;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnKillNPC(affixItem, Player, target);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnKillNPC(affixItem, Player, target);
                }
            }
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            Item affixItem;
            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyHitNPC(affixItem, Player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyHitNPC(affixItem, Player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
                }
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            knockback *= knockbackMultiplier;
        }
        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            Item affixItem;
            float damageMultiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyHitPvp(affixItem, Player, item, target, ref damageMultiplier, ref crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyHitPvp(affixItem, Player, item, target, ref damageMultiplier, ref crit);
                }
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!(proj.ModProjectile is Projectiles.INonTriggerringProjectile))
            {
                Item item;
                float damageMultiplier = 1f;
                float knockBackMultiplier = 1f;
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    item = Player.inventory[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjModifyHitNPC(item, Player, proj, target, ref damageMultiplier, ref knockBackMultiplier, ref crit, ref hitDirection);
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    item = Player.armor[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjModifyHitNPC(item, Player, proj, target, ref damageMultiplier, ref knockBackMultiplier, ref crit, ref hitDirection);
                    }
                }
                damage = (int)Math.Round(damage * damageMultiplier);
                knockback = (int)Math.Round(knockback * knockBackMultiplier);
            }
        }
        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            if (!(proj.ModProjectile is Projectiles.INonTriggerringProjectile))
            {
                Item item;
                float damageMultiplier = 1f;
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    item = Player.inventory[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjModifyHitPvp(item, Player, proj, target, ref damageMultiplier, ref crit);
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    item = Player.armor[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjModifyHitPvp(item, Player, proj, target, ref damageMultiplier, ref crit);
                    }
                }
                damage = (int)Math.Round(damage * damageMultiplier);
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            Item affixItem;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnHitNPC(affixItem, Player, item, target, damage, knockback, crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnHitNPC(affixItem, Player, item, target, damage, knockback, crit);
                }
            }
        }
        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            Item affixItem;
            
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnHitPvp(affixItem, Player, item, target, damage, crit);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnHitPvp(affixItem, Player, item, target, damage, crit);
                }
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (!(proj.ModProjectile is Projectiles.INonTriggerringProjectile))
            {
                Item item;
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    item = Player.inventory[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjOnHitNPC(item, Player, proj, target, damage, knockback, crit);
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    item = Player.armor[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjOnHitNPC(item, Player, proj, target, damage, knockback, crit);
                    }
                }
            }
        }
        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if (!(proj.ModProjectile is Projectiles.INonTriggerringProjectile))
            {

                Item item;
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    item = Player.inventory[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjOnHitPvp(item, Player, proj, target, damage, crit);
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    item = Player.armor[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjOnHitPvp(item, Player, proj, target, damage, crit);
                    }
                }
            }
        }
        public override bool Shoot(Item item, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item affixItem;
            bool shoot = true;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    if (!modItem.PlayerShoot(affixItem, Player, item, source, position, velocity, type, damage, knockback))
                        shoot = false;
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.type == 0 || affixItem.stack == 0)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    if (!modItem.PlayerShoot(affixItem, Player, item, source, position, velocity, type, damage, knockback))
                        shoot = false;
                }
            }
            return shoot;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
        }

        public override void ResetEffects()
        {
            damageTaken = 1;

            useSpeed = 1;
            meleeSpeed = 1;
            pickSpeed = 1;

            moveSpeed = 1;

            potionDelayTime = 1;
            restorationDelayTime = 1;

            dodgeChance = 0;

            reflectMeleeDamage = 0;

            goldDropChances.ResetEffects();
            if (Main.time == 0)
            {
                goldDropChances.ClearDisabled();
            }
        }
        public override void PostUpdateEquips()
        {
            Player.meleeSpeed *= meleeSpeed;
            Player.pickSpeed *= pickSpeed;

            Player.potionDelayTime = (int)Math.Round(Player.potionDelayTime * potionDelayTime);
            Player.restorationDelayTime = (int)Math.Round(Player.restorationDelayTime * restorationDelayTime);
        }
        public override void PostUpdateRunSpeeds()
        {
            Player.runAcceleration *= moveSpeed;
            Player.maxRunSpeed *= moveSpeed;
            Player.accRunSpeed *= moveSpeed;
        }
        public override void UpdateBadLifeRegen()
        {
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
        }
    }
}