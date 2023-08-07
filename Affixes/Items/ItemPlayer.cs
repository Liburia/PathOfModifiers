using Microsoft.Xna.Framework;
using PathOfModifiers.ModNet.PacketHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

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

        public override void PlayerConnect()
        {
            ModPacketHandler.CPlayerConnected();
            //var mapBorder = ModContent.GetInstance<MapBorder>();
            //MapBorder.ClearActiveBounds();
        }
        public override void PlayerDisconnect()
        {
            //var mapBorder = ModContent.GetInstance<MapBorder>();
            //MapBorder.ClearActiveBounds();
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
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
                    consume = consume && modItem.PlayerCanConsumeAmmo(Player, item, ammo);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    consume = consume && modItem.PlayerCanConsumeAmmo(Player, item, ammo);
                }
            }
            return consume;
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            Item item;
            bool dodge = false;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    dodge |= modItem.FreeDodge(item, Player, ref info);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    dodge |= modItem.FreeDodge(item, Player, ref info);
                }
            }

            return dodge;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (modifiers.PvP)
            {
                if (modifiers.DamageSource.SourcePlayerIndex >= 0)
                {
                    lastAttackingPlayer = Main.player[modifiers.DamageSource.SourcePlayerIndex];
                    var lastAttackingItem = lastAttackingPlayer.HeldItem;
                    lastAttackingPlayer.GetModPlayer<ItemPlayer>().ModifyHitPvp(lastAttackingItem, Player, ref modifiers);
                    ModifyHitByPvp(lastAttackingPlayer, ref modifiers);
                }
            }

            Item item;
            float damageMultiplier = damageTaken;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PreHurt(item, Player, ref damageMultiplier, ref modifiers);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PreHurt(item, Player, ref damageMultiplier, ref modifiers);
                }
            }
            modifiers.FinalDamage *= damageMultiplier;

        }
        public override void PostHurt(Player.HurtInfo info)
        {
            if (info.PvP)
            {
                OnHitByPvp(lastAttackingPlayer, info);
            }

            Item item;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PostHurt(item, Player, info);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PostHurt(item, Player, info);
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
        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            Item affixItem;
            var new_health = StatModifier.Default;
            var new_mana = StatModifier.Default;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyMaxStats(affixItem, ref new_health, ref new_mana);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyMaxStats(affixItem, ref new_health, ref new_mana);
                }
            }
            health = new_health;
            mana = new_mana;
        }
        public override void ModifyLuck(ref float luck)
        {
            Item affixItem;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyLuck(affixItem, ref luck);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyLuck(affixItem, ref luck);
                }
            }
        }
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            Item affixItem;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerGetHealLife(affixItem, item, ref healValue);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerGetHealLife(affixItem, item, ref healValue);
                }
            }
        }
        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            Item affixItem;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerGetHealMana(affixItem, item, ref healValue);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerGetHealMana(affixItem, item, ref healValue);
                }
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            return useSpeed;
        }
        public override void ModifyWeaponCrit(Item heldItem, ref float crit)
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
            crit *= multiplier;
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
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
                    modItem.ModifyHitByNPC(item, Player, npc, ref damageMultiplier, ref modifiers);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByNPC(item, Player, npc, ref damageMultiplier, ref modifiers);
                }
            }
            modifiers.FinalDamage *= damageMultiplier;
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            Item item;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByNPC(item, Player, npc, hurtInfo);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByNPC(item, Player, npc, hurtInfo);
                }
            }

            if (reflectMeleeDamage > 0)
            {
                int reflectDamage = (int)Math.Round(hurtInfo.Damage * reflectMeleeDamage);
                float reflectKnockback = 5;
                int reflectDirection = npc.Center.X > Player.Center.X ? 1 : -1;
                Player.ApplyDamageToNPC(npc, reflectDamage, reflectKnockback, reflectDirection);
            }
        }
        public void ModifyHitByPvp(Player attacker, ref Player.HurtModifiers modifiers)
        {
            Item item;
            var damage = modifiers.FinalDamage;
            float damageMultiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByPvp(item, Player, attacker, ref damageMultiplier, ref modifiers);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByPvp(item, Player, attacker, ref damageMultiplier, ref modifiers);
                }
            }
            damage *= damageMultiplier;
            modifiers.FinalDamage = damage;
        }
        public void OnHitByPvp(Player attacker, Player.HurtInfo info)
        {
            Item item;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByPvp(item, Player, attacker, info);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByPvp(item, Player, attacker, info);
                }
            }


            if (reflectMeleeDamage > 0)
            {
                int reflectDamage = (int)Math.Round(info.Damage * reflectMeleeDamage);
                int reflectDirection = attacker.Center.X > Player.Center.X ? 1 : -1;
                attacker.Hurt(PlayerDeathReason.ByCustomReason("died to reflect"), reflectDamage, reflectDirection, true, false);
                attacker.immune = false;
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
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
                    modItem.ModifyHitByProjectile(item, Player, proj, ref damageMultiplier, ref modifiers);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.ModifyHitByProjectile(item, Player, proj, ref damageMultiplier, ref modifiers);
                }
            }
            modifiers.FinalDamage *= damageMultiplier;
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            Item item;

            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByProjectile(item, Player, proj, hurtInfo);
                }

            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.OnHitByProjectile(item, Player, proj, hurtInfo);
                }

            }
        }

        public override void ModifyCaughtFish(Item fish)
        {
            Item affixItem;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyCaughtFish(affixItem, fish);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyCaughtFish(affixItem, fish);
                }
            }
        }
        public override bool? CanConsumeBait(Item bait)
        {
            bool? consume = null;
            Item item;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                item = Player.inventory[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerCanConsumeBait(item, bait);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                item = Player.armor[i];
                if (item.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerCanConsumeBait(item, bait);
                }
            }

            return consume;
        }

        public void OnKillNPC(NPC target)
        {
            Item affixItem;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnKillNPC(affixItem, Player, target);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnKillNPC(affixItem, Player, target);
                }
            }
        }
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            Item affixItem;
            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;
            float critDamageMultiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyHitNPC(affixItem, Player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref critDamageMultiplier, ref modifiers);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyHitNPC(affixItem, Player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref critDamageMultiplier, ref modifiers);
                }
            }
            modifiers.FinalDamage *= damageMultiplier;
            modifiers.Knockback *= knockbackMultiplier;
            modifiers.CritDamage *= critDamageMultiplier;
        }
        public void ModifyHitPvp(Item item, Player target, ref Player.HurtModifiers modifiers)
        {
            Item affixItem;
            float damageMultiplier = 1f;
            float critDamageMultiplier = 1f;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyHitPvp(affixItem, Player, item, target, ref damageMultiplier, ref critDamageMultiplier, ref modifiers);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerModifyHitPvp(affixItem, Player, item, target, ref damageMultiplier, ref critDamageMultiplier, ref modifiers);
                }
            }
            modifiers.FinalDamage *= damageMultiplier * critDamageMultiplier;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!(proj.ModProjectile is Projectiles.INonTriggerringProjectile))
            {
                Item item;
                float damageMultiplier = 1f;
                float knockBackMultiplier = 1f;
                float critDamageMultiplier = 1f;
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    item = Player.inventory[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjModifyHitNPC(item, Player, proj, target, ref damageMultiplier, ref knockBackMultiplier, ref critDamageMultiplier, ref modifiers);
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    item = Player.armor[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjModifyHitNPC(item, Player, proj, target, ref damageMultiplier, ref knockBackMultiplier, ref critDamageMultiplier, ref modifiers);
                    }
                }
                modifiers.FinalDamage *= damageMultiplier;
                modifiers.Knockback *= knockBackMultiplier;
                modifiers.CritDamage *= critDamageMultiplier;
            }
        }
        public void ModifyHitPvpWithProj(Projectile proj, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!(proj.ModProjectile is Projectiles.INonTriggerringProjectile))
            {
                Item item;
                float damageMultiplier = 1f;
                float critDamageMultiplier = 1f;
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    item = Player.inventory[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjModifyHitPvp(item, Player, proj, target, ref damageMultiplier, ref critDamageMultiplier, ref modifiers);
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    item = Player.armor[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjModifyHitPvp(item, Player, proj, target, ref damageMultiplier, ref critDamageMultiplier, ref modifiers);
                    }
                }
                modifiers.FinalDamage *= damageMultiplier * critDamageMultiplier;
            }
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Item affixItem;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnHitNPC(affixItem, Player, item, target, hit, damageDone);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (affixItem.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnHitNPC(affixItem, Player, item, target, hit, damageDone);
                }
            }
        }
        public void OnHitPvp(Item item, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            Item affixItem;

            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnHitPvp(affixItem, Player, item, target, modifiers, damageDone);
                }
            }
            for (int i = 0; i < Player.armor.Length; i++)
            {
                affixItem = Player.armor[i];
                if (affixItem.IsAir)
                    continue;

                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    modItem.PlayerOnHitPvp(affixItem, Player, item, target, modifiers, damageDone);
                }
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
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
                        modItem.ProjOnHitNPC(item, Player, proj, target, hit, damageDone);
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    item = Player.armor[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjOnHitNPC(item, Player, proj, target, hit, damageDone);
                    }
                }
            }
        }
        public void OnHitPvpWithProj(Projectile proj, Player target, Player.HurtModifiers modifiers, int damageDone)
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
                        modItem.ProjOnHitPvp(item, Player, proj, target, modifiers, damageDone);
                    }
                }
                for (int i = 0; i < Player.armor.Length; i++)
                {
                    item = Player.armor[i];
                    if (item.IsAir)
                        continue;

                    if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        modItem.ProjOnHitPvp(item, Player, proj, target, modifiers, damageDone);
                    }
                }
            }
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item affixItem;
            bool shoot = true;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                affixItem = Player.inventory[i];
                if (affixItem.IsAir)
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
                if (affixItem.IsAir)
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
            Player.GetAttackSpeed(DamageClass.Melee) *= meleeSpeed;
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