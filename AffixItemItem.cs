using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using PathOfModifiers.Affixes.Items;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Rarities;
using Terraria.DataStructures;
using PathOfModifiers.Items;

namespace PathOfModifiers
{
    //TODO: When item is dropped roll only on server? How it work?
    public class AffixItemItem : GlobalItem
    {
        public override bool CloneNewInstances => false;

        public override bool InstancePerEntity => true;

        public RarityItem rarity;

        public List<Affix> affixes;
        public List<Affix> prefixes;
        public List<Affix> suffixes;

        public int FreeAffixes => rarity.maxAffixes - affixes.Count;
        public int FreePrefixes => Math.Min(FreeAffixes, rarity.maxPrefixes - prefixes.Count);
        public int FreeSuffixes => Math.Min(FreeAffixes, rarity.maxSuffixes - suffixes.Count);

        public AffixItemItem()
        {
            rarity = ((PoMDataLoader.raritiesItem?.Length ?? 0) == 0) ? new ItemNone() : PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(ItemNone)]];
            affixes = new List<Affix>();
            prefixes = new List<Affix>();
            suffixes = new List<Affix>();
        }

        #region Item conditions
        public static bool IsRollable(Item item)
        {
            return IsWeapon(item) || IsAccessory(item) || IsAnyArmor(item) || IsMap(item);
        }

        public static bool IsWeapon(Item item)
        {
            return item.damage > 0 && item.maxStack == 1 && item.useStyle != 2 && item.useStyle != 4;
        }
        public static bool CanCrit(Item item)
        {
            return item.crit > 0;
        }
        public static bool CanKnockback(Item item)
        {
            return item.knockBack > 0;
        }
        public static bool CanCostMana(Item item)
        {
            return item.mana > 0;
        }
        public static bool CanConsumeAmmo(Item item)
        {
            return item.useAmmo != AmmoID.None;
        }
        public static bool CanMelee(Item item)
        {
            return !item.noMelee;
        }
        public static bool IsSwinging(Item item)
        {
            return item.useStyle == 1;
        }
        public static bool IsStabbing(Item item)
        {
            return item.useStyle == 3;
        }

        public static bool IsPickaxe(Item item)
        {
            return item.pick > 0;
        }
        public static bool IsAxe(Item item)
        {
            return item.axe > 0;
        }
        public static bool IsHammer(Item item)
        {
            return item.hammer > 0;
        }
        public static bool IsSpear(Item item)
        {
            return item.melee && item.noMelee && item.shoot > -1 && item.useStyle == 5 && !item.channel;
        }
        public static bool IsFlailOrYoyo(Item item)
        {
            return item.melee && item.noMelee && item.shoot > -1 && item.useStyle == 5 && item.channel;
        }
        public static bool IsUsingAmmo(Item item)
        {
            return item.useAmmo != AmmoID.None;
        }

        public static bool IsMelee(Item item)
        {
            return item.melee;
        }
        public static bool IsRanged(Item item)
        {
            return item.ranged;
        }
        public static bool IsMagic(Item item)
        {
            return item.magic;
        }
        public static bool IsThrowing(Item item)
        {
            return item.thrown;
        }
        public static bool IsSummon(Item item)
        {
            return item.summon;
        }

        public static bool IsAccessory(Item item)
        {
            return item.accessory;
        }
        public static bool IsHeadArmor(Item item)
        {
            return item.headSlot > -1;
        }
        public static bool IsBodyArmor(Item item)
        {
            return item.bodySlot > -1;
        }
        public static bool IsLegArmor(Item item)
        {
            return item.legSlot > -1;
        }
        public static bool IsAnyArmor(Item item)
        {
            return IsHeadArmor(item) || IsBodyArmor(item) || IsLegArmor(item);
        }

        public static bool IsPotion(Item item)
        {
            return item.potion;
        }

        public static bool IsMap(Item item)
        {
            return item.modItem is Map;
        }


        public static bool IsEquipped(Item item, Player player)
        {
            return IsArmorEquipped(item, player) || IsAccessoryEquipped(item, player);
        }
        public static bool IsArmorEquipped(Item item, Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (player.armor[i] == item)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsAccessoryEquipped(Item item, Player player)
        {
            for (int i = 3; i < 8 + player.extraAccessorySlots; i++)
            {
                if (player.armor[i] == item)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        public string GetBaseName(Item item) => Lang.GetItemNameValue(item.type);

        public void UpdateName(Item item)
        {
            if (rarity == null || rarity.GetType() == typeof(ItemNone))
                item.ClearNameOverride();
            else
            {
                string addedPrefix = string.Empty;
                double addedPrefixWeight = 0f;
                foreach (var prefix in prefixes)
                {
                    if (prefix.AddedTextWeight > addedPrefixWeight && prefix.AddedText != string.Empty)
                    {
                        addedPrefix = prefix.AddedText;
                        addedPrefixWeight = prefix.AddedTextWeight;
                    }
                }
                string addedSuffix = string.Empty;
                float addedSuffixWeight = 0f;
                foreach (var suffix in suffixes)
                {
                    if (suffix.AddedTextWeight > addedSuffixWeight && suffix.AddedText != string.Empty)
                    {
                        addedSuffix = suffix.AddedText;
                        addedPrefixWeight = suffix.AddedTextWeight;
                    }
                }
                item.SetNameOverride($"{rarity.name} {addedPrefix}{(addedPrefix != string.Empty ? " " : string.Empty)}{GetBaseName(item)}{(addedSuffix != string.Empty ? " " : string.Empty)}{addedSuffix}");
                item.rare = rarity.vanillaRarity;
            }
        }
        /// <summary>
        /// Roll item if it's rollable.
        /// </summary>
        public bool TryRollItem(Item item)
        {
            if (IsRollable(item))
            {
                RollItem(item);
                return true;
            }
            else
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(ItemNone)]];
                return false;
            }
        }

        /// <summary>
        /// Completely rerolls rarity and affixes.
        /// </summary>
        /// <param name="item"></param>
        public void RollItem(Item item)
        {
            ClearAffixes(item);
            rarity = PoMAffixController.RollRarity(item);
            RollAffixes(item);
            UpdateName(item);
        }
        /// <summary>
        /// Completely rerolls affixes.
        /// </summary>
        /// <param name="item"></param>
        public void RerollAffixes(Item item)
        {
            ClearAffixes(item);
            RollAffixes(item);
            UpdateName(item);
        }
        /// <summary>
        /// Validly adds affixes to the item.
        /// </summary>
        /// <param name="item"></param>
        public void RollAffixes(Item item)
        {
            Affix newAffix;
            int freeAffixes = FreeAffixes;
            for (int i = 0; i < freeAffixes; i++)
            {
                if (i >= rarity.minAffixes && Main.rand.NextFloat(0, 1) > rarity.chanceToRollAffix)
                    break;

                newAffix = PoMAffixController.RollNewAffix(this, item);
                if (newAffix == null)
                    break;

                AddAffix(newAffix, item);
            }
        }
        public bool RaiseRarity(Item item)
        {
            Type rarityType = rarity.GetType();
            bool raised = false;
            if (rarityType == typeof(WeaponCommon))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(WeaponUncommon)]];
                raised = true;
            }
            else if (rarityType == typeof(WeaponUncommon))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(WeaponRare)]];
                raised = true;
            }
            else if (rarityType == typeof(WeaponRare))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(WeaponEpic)]];
                raised = true;
            }
            else if (rarityType == typeof(WeaponEpic))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(WeaponLegendary)]];
                raised = true;
            }
            else if (rarityType == typeof(ArmorCommon))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(ArmorUncommon)]];
                raised = true;
            }
            else if (rarityType == typeof(ArmorUncommon))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(ArmorRare)]];
                raised = true;
            }
            else if (rarityType == typeof(ArmorRare))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(ArmorEpic)]];
                raised = true;
            }
            else if (rarityType == typeof(ArmorEpic))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(ArmorLegendary)]];
                raised = true;
            }
            else if (rarityType == typeof(AccessoryCommon))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(AccessoryUncommon)]];
                raised = true;
            }
            else if (rarityType == typeof(AccessoryUncommon))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(AccessoryRare)]];
                raised = true;
            }
            else if (rarityType == typeof(AccessoryRare))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(AccessoryEpic)]];
                raised = true;
            }
            else if (rarityType == typeof(AccessoryEpic))
            {
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(AccessoryLegendary)]];
                raised = true;
            }
            if (raised)
                UpdateName(item);
            return raised;
        }
        /// <summary>
        /// Validly adds an affix to the item.
        /// </summary>
        /// <param name="item"></param>
        public bool AddRandomAffix(Item item)
        {
            Affix newAffix = PoMAffixController.RollNewAffix(this, item);
            if (newAffix == null)
                return false;

            AddAffix(newAffix, item);

            UpdateName(item);
            return true;
        }
        /// <summary>
        /// Validly adds a prefix to the item.
        /// </summary>
        /// <param name="item"></param>
        public bool AddRandomPrefix(Item item)
        {
            Affix newPrefix = PoMAffixController.RollNewPrefix(this, item);
            if (newPrefix == null)
                return false;

            AddAffix(newPrefix, item);

            UpdateName(item);
            return true;
        }
        /// <summary>
        /// Validly adds a suffix to the item.
        /// </summary>
        /// <param name="item"></param>
        public bool AddRandomSuffix(Item item)
        {
            Affix newSuffix = PoMAffixController.RollNewSuffix(this, item);
            if (newSuffix == null)
                return false;

            AddAffix(newSuffix, item);

            UpdateName(item);
            return true;
        }
        public void RemoveAll(Item item)
        {
            ClearAffixes(item);

            Type rarityType = rarity.GetType();
            if (rarityType == typeof(WeaponUncommon) || rarityType == typeof(WeaponRare) || rarityType == typeof(WeaponEpic) || rarityType == typeof(WeaponLegendary))
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(WeaponCommon)]];
            else if (rarityType == typeof(ArmorUncommon) || rarityType == typeof(ArmorRare) || rarityType == typeof(ArmorEpic) || rarityType == typeof(ArmorLegendary))
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(ArmorCommon)]];
            else if (rarityType == typeof(AccessoryUncommon) || rarityType == typeof(AccessoryRare) || rarityType == typeof(AccessoryEpic) || rarityType == typeof(AccessoryLegendary))
                rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[typeof(AccessoryCommon)]];

            UpdateName(item);
        }
        public void RemovePrefixes(Item item)
        {
            ClearPrefixes(item);
            UpdateName(item);
        }
        public void RemoveSuffixes(Item item)
        {
            ClearSuffixes(item);
            UpdateName(item);
        }
        public void RollAffixTierMultipliers(Item item)
        {
            foreach (Affix affix in affixes)
            {
                affix.RollValue(false);
            }
            UpdateName(item);
        }
        public void RollPrefixTierMultipliers(Item item)
        {
            foreach (Affix prefix in prefixes)
            {
                prefix.RollValue(false);
            }
            UpdateName(item);
        }
        public void RollSuffixTierMultipliers(Item item)
        {
            foreach (Affix suffix in suffixes)
            {
                suffix.RollValue(false);
            }
            UpdateName(item);
        }

        public void AddAffix(Affix affix, Item item, bool clone = false)
        {
            affix.AddAffix(item, clone);

            affixes.Add(affix);

            if (affix.IsPrefix)
            {
                prefixes.Add(affix);
                return;
            }

            if (affix.IsSuffix)
            {
                suffixes.Add(affix);
                return;
            }
        }
        public void RemoveAffix(Affix affix, Item item)
        {
            affix.RemoveAffix(item);
            affixes.Remove(affix);
            if (affix.IsPrefix)
                prefixes.Remove(affix);
            else
            {
                if (affix.IsSuffix)
                    suffixes.Remove(affix);
            }
        }
        public void ClearAffixes(Item item)
        {
            foreach (Affix affix in affixes)
            {
                affix.RemoveAffix(item);
            }
            affixes.Clear();
            prefixes.Clear();
            suffixes.Clear();
        }
        public void ClearPrefixes(Item item)
        {
            foreach (var prefix in prefixes)
            {
                prefix.RemoveAffix(item);
                affixes.Remove(prefix);
            }
            prefixes.Clear();
        }
        public void ClearSuffixes(Item item)
        {
            foreach (var suffix in suffixes)
            {
                suffix.RemoveAffix(item);
                affixes.Remove(suffix);
            }
            suffixes.Clear();
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            return (PathOfModifiers.disableVanillaModifiersWeapons && IsWeapon(item)) || (PathOfModifiers.disableVanillaModifiersAccessories && IsAccessory(item)) ? mod.PrefixType("") : -1;
        }
        public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
        {
            foreach (Affix affix in affixes)
            {
                affix.ReforgePrice(item, ref reforgePrice);
            }
            return true;
        }

        #region Item Hooks
        public override bool ConsumeAmmo(Item item, Player player)
        {
            float chanceToNotConsume = 0;
            bool consume = true;
            foreach (var prefix in prefixes)
            {
                if (!prefix.ConsumeAmmo(item, player, ref chanceToNotConsume))
                    consume = false;
            }
            foreach (var suffix in suffixes)
            {
                if (!suffix.ConsumeAmmo(item, player, ref chanceToNotConsume))
                    consume = false;
            }
            consume = consume && Main.rand.NextFloat(1) > chanceToNotConsume;
            return consume;
        }
        public override void GetWeaponCrit(Item item, Player player, ref int crit)
        {
            float multiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.GetWeaponCrit(item, player, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.GetWeaponCrit(item, player, ref multiplier);
            }
            crit = (int)Math.Round(crit * multiplier);
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyWeaponDamage(item, player, ref add, ref mult, ref flat);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyWeaponDamage(item, player, ref add, ref mult, ref flat);
            }
        }
        public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
        {
            float multiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.GetWeaponKnockback(item, player, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.GetWeaponKnockback(item, player, ref multiplier);
            }
            knockback = (float)Math.Round(knockback * multiplier);
        }
        public override float UseTimeMultiplier(Item item, Player player)
        {
            float multiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.UseTimeMultiplier(item, player, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.UseTimeMultiplier(item, player, ref multiplier);
            }
            return multiplier;
        }
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyManaCost(item, player, ref reduce, ref mult);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyManaCost(item, player, ref reduce, ref mult);
            }
        }
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool shoot = true;
            foreach (var prefix in prefixes)
            {
                if (!prefix.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack))
                    shoot = false;
            }
            foreach (var suffix in suffixes)
            {
                if (!suffix.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack))
                    shoot = false;
            }
            return shoot;
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            //UpdateEquip covers accessories
        }
        public override void UpdateEquip(Item item, Player player)
        {
            AffixItemPlayer pomPlayer = player.GetModPlayer<AffixItemPlayer>();
            foreach (var prefix in prefixes)
            {
                prefix.UpdateEquip(item, pomPlayer);
            }
            foreach (var suffix in suffixes)
            {
                suffix.UpdateEquip(item, pomPlayer);
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            AffixItemPlayer pomPlayer = player.GetModPlayer<AffixItemPlayer>();
            foreach (var prefix in prefixes)
            {
                prefix.UpdateInventory(item, pomPlayer);
            }
            foreach (var suffix in suffixes)
            {
                suffix.UpdateInventory(item, pomPlayer);
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            foreach (var prefix in prefixes)
            {
                prefix.HoldItem(item, player);
            }
            foreach (var suffix in suffixes)
            {
                suffix.HoldItem(item, player);
            }
        }
        public override bool UseItem(Item item, Player player)
        {
            foreach (var prefix in prefixes)
            {
                prefix.UseItem(item, player);
            }
            foreach (var suffix in suffixes)
            {
                suffix.UseItem(item, player);
            }
            return false;
        }
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitNPC(item, player, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitNPC(item, player, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            knockBack *= knockbackMultiplier;
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref int damage, ref bool crit)
        {
            float damageMultiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitPvp(item, player, target, ref damageMultiplier, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitPvp(item, player, target, ref damageMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitNPC(item, player, target, damage, knockBack, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitNPC(item, player, target, damage, knockBack, crit);
            }
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitPvp(item, player, target, damage, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitPvp(item, player, target, damage, crit);
            }
        }
        #endregion
        #region Projectile hooks
        public void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.ProjModifyHitNPC(item, player, projectile, target, ref damageMultiplier, ref knockbackMultiplier, ref crit, ref hitDirection);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ProjModifyHitNPC(item, player, projectile, target, ref damageMultiplier, ref knockbackMultiplier, ref crit, ref hitDirection);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            knockback *= knockbackMultiplier;
        }
        public void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            float damageMultiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.ProjModifyHitPvp(item, player, projectile, target, ref damageMultiplier, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ProjModifyHitPvp(item, player, projectile, target, ref damageMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ProjOnHitNPC(item, player, projectile, target, damage, knockback, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ProjOnHitNPC(item, player, projectile, target, damage, knockback, crit);
            }
        }
        public void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ProjOnHitPvp(item, player, projectile, target, damage, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ProjOnHitPvp(item, player, projectile, target, damage, crit);
            }
        }
        #endregion
        // Player hooks trigger on the whole inventory and equipped items;
        #region Player Hooks
        public bool PlayerConsumeAmmo(Player player, Item item, Item ammo, ref float chanceToNotConsume)
        {
            bool consume = true;
            foreach (var prefix in prefixes)
            {
                if (!prefix.PlayerConsumeAmmo(player, item, ammo, ref chanceToNotConsume))
                    consume = false;
            }
            foreach (var suffix in suffixes)
            {
                if (!suffix.PlayerConsumeAmmo(player, item, ammo, ref chanceToNotConsume))
                    consume = false;
            }
            return consume;
        }
        public bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool hurt = true;
            foreach (var prefix in prefixes)
            {
                if (!prefix.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            foreach (var suffix in suffixes)
            {
                if (!suffix.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            return hurt;
        }
        public void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PostHurt(item, player, pvp, quiet, damage, hitDirection, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PostHurt(item, player, pvp, quiet, damage, hitDirection, crit);
            }
        }
        public void NaturalLifeRegen(Item item, Player player, ref float regenMultiplier)
        {
            foreach (var prefix in prefixes)
            {
                prefix.NaturalLifeRegen(item, player, ref regenMultiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.NaturalLifeRegen(item, player, ref regenMultiplier);
            }
        }
        public void PlayerGetWeaponCrit(Item item, Item heldItem, Player player, ref float multiplier)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerGetWeaponCrit(item, heldItem, player, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerGetWeaponCrit(item, heldItem, player, ref multiplier);
            }
        }
        public void ModifyHitByNPC(Item item, Player player, NPC npc, ref float damageMultiplier, ref bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitByNPC(item, player, npc, ref damageMultiplier, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitByNPC(item, player, npc, ref damageMultiplier, ref crit);
            }
        }
        public void ModifyHitByPvp(Item item, Player player, Player attacker, ref float damageMultiplier, ref bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitByPvp(item, player, attacker, ref damageMultiplier, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitByPvp(item, player, attacker, ref damageMultiplier, ref crit);
            }
        }
        public void ModifyHitByProjectile(Item item, Player player, Projectile projectile, ref float damageMultiplier, ref bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitByProjectile(item, player, projectile, ref damageMultiplier, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitByProjectile(item, player, projectile, ref damageMultiplier, ref crit);
            }
        }
        public void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitByNPC(item, player, npc, damage, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitByNPC(item, player, npc, damage, crit);
            }
        }
        public void OnHitByPvp(Item item, Player player, Player attacker, int damage, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitByPvp(item, player, attacker, damage, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitByPvp(item, player, attacker, damage, crit);
            }
        }
        public void OnHitByProjectile(Item item, Player player, Projectile projectile, int damage, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitByProjectile(item, player, projectile, damage, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitByProjectile(item, player, projectile, damage, crit);
            }
        }
        public void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
        }
        public void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
        }
        public void PlayerOnHitNPC(Item affixItem, Player player, Item item, NPC target, int damage, float knockback, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
        }
        public void PlayerOnHitPvp(Item affixItem, Player player, Item item, Player target, int damage, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
        }

        public void PlayerOnKillNPC(Item item, Player player, NPC target)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerOnKillNPC(item, player, target);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerOnKillNPC(item, player, target);
            }
        }
        public void PlayerOnKillPvp(Item item, Player player, Player target)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerOnKillPvp(item, player, target);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerOnKillPvp(item, player, target);
            }
        }

        public bool PlayerShoot(Item affixItem, Player player, Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool shoot = true;
            foreach (var prefix in prefixes)
            {
                if (!prefix.PlayerShoot(affixItem, player, item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack))
                    shoot = false;
            }
            foreach (var suffix in suffixes)
            {
                if (!suffix.PlayerShoot(affixItem, player, item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack))
                    shoot = false;
            }
            return shoot;
        }
        #endregion

        public override void PostReforge(Item item)
        {
            try
            {
                TryRollItem(item);
            }
            catch (Exception e)
            {
                mod.Logger.Error(e.ToString());
            }
        }
        public override void OnCraft(Item item, Recipe recipe)
        {
            try
            {
                TryRollItem(item);
            }
            catch (Exception e)
            {
                mod.Logger.Error(e.ToString());
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyTooltips(PathOfModifiers.Instance, item, tooltips);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyTooltips(PathOfModifiers.Instance, item, tooltips);
            }
            if (rarity.GetType() != typeof(ItemNone))
            {
                foreach (TooltipLine line in tooltips)
                {
                    if (line.mod == "Terraria" && line.Name == "ItemName")
                    {
                        line.overrideColor = rarity.color;
                        break;
                    }
                }
            }
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //TOOD: Draw rarity BG?
            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            //TOOD: Draw rarity BG?
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
        }
        public override void PostUpdate(Item item)
        {
            if (rarity == null || rarity.GetType() == typeof(ItemNone))
            {
                try
                {
                    TryRollItem(item);
                }
                catch (Exception e)
                {
                    mod.Logger.Error(e.ToString());
                }
            }
            //TODO: Add light/dust?
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.npcShop == 0 && (rarity == null || rarity.GetType() == typeof(ItemNone)))
            {
                try
                {
                    TryRollItem(item);
                }
                catch (Exception e)
                {
                    mod.Logger.Error(e.ToString());
                }
            }
        }
        public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
        {
            //TODO: add light/dust?
        }

        public override bool NeedsSaving(Item item)
        {
            return rarity != null;
        }
        public override TagCompound Save(Item item)
        {
            TagCompound tag = new TagCompound();
            tag.Set("rarityMod", rarity.mod.Name);
            tag.Set("rarityFullName", rarity.GetType().FullName);
            tag.Set("affixCount", affixes.Count);
            TagCompound affixTag;
            Affix affix;
            for (int i = 0; i < affixes.Count; i++)
            {
                affixTag = new TagCompound();
                affix = affixes[i];
                affixTag.Set("affixMod", affix.mod.Name);
                affixTag.Set("affixFullName", affix.GetType().FullName);
                affix.Save(affixTag, item);
                tag.Set(i.ToString(), affixTag);
            }
            return tag;
        }
        public override void Load(Item item, TagCompound tag)
        {
            string rarityModName = tag.GetString("rarityMod");
            Mod mod = ModLoader.GetMod(rarityModName);
            if (mod == null)
            {
                PathOfModifiers.Instance.Logger.Warn($"Mod '{rarityModName}' not found");
                return;
            }
            string rarityFullName = tag.GetString("rarityFullName");
            Type type = mod.Code.GetType(rarityFullName);
            if (type == null)
            {
                PathOfModifiers.Instance.Logger.Warn($"Rarity '{type.FullName}' doesn't exist");
                return;
            }
            if (type.IsDefined(typeof(DisableAffix), false))
            {
                PathOfModifiers.Instance.Logger.Warn($"Rarity '{type.FullName}' is disabled");
                return;
            }
            rarity = PoMDataLoader.raritiesItem[PoMDataLoader.rarityItemMap[type]];
            int affixCount = tag.GetAsInt("affixCount");
            TagCompound affixTag;
            Affix affix;
            for (int i = 0; i < affixCount; i++)
            {
                affixTag = tag.GetCompound(i.ToString());
                string affixModName = affixTag.GetString("affixMod");
                mod = ModLoader.GetMod(affixModName);
                if (mod == null)
                {
                    PathOfModifiers.Instance.Logger.Warn($"Mod '{affixModName}' not found");
                    continue;
                }
                string affixFullName = affixTag.GetString("affixFullName");
                type = mod.Code.GetType(affixFullName);
                if (type == null)
                {
                    PathOfModifiers.Instance.Logger.Warn($"Affix '{affixFullName}' doesn't exist");
                    continue;
                }
                if (type.IsDefined(typeof(DisableAffix), false))
                {
                    PathOfModifiers.Instance.Logger.Warn($"Affix '{affixFullName}' is disabled");
                    continue;
                }
                affix = PoMDataLoader.affixesItem[PoMDataLoader.affixItemMap[type]].Clone();
                affix.Load(affixTag, item);
                AddAffix(affix, item);
            }
            UpdateName(item);
        }

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            AffixItemItem newItem = (AffixItemItem)NewInstance(itemClone);

            newItem.rarity = rarity;

            Affix affixClone;
            foreach (Affix affix in affixes)
            {
                affixClone = affix.Clone();
                newItem.AddAffix(affixClone, item, true);
            }
            newItem.UpdateName(itemClone);

            return newItem;
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            try
            {
                writer.Write(PoMDataLoader.rarityItemMap[rarity.GetType()]);

                writer.Write((byte)affixes.Count);
                Affix affix;
                for (int i = 0; i < affixes.Count; i++)
                {
                    affix = affixes[i];
                    writer.Write(PoMDataLoader.affixItemMap[affix.GetType()]);
                    affix.NetSend(item, writer);
                }
            }
            catch (Exception e)
            {
                mod.Logger.Error(e.ToString());
            }
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            try
            {
                rarity = PoMDataLoader.raritiesItem[reader.ReadInt32()];

                int affixCount = reader.ReadByte();
                Affix affix;
                for (int i = 0; i < affixCount; i++)
                {
                    affix = PoMDataLoader.affixesItem[reader.ReadInt32()].Clone();
                    affix.NetReceive(item, reader);
                    AddAffix(affix, item);
                }
                UpdateName(item);
            }
            catch (Exception e)
            {
                mod.Logger.Error(e.ToString());
            }
        }
    }
}
