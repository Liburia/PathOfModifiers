using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Items;
using PathOfModifiers.Rarities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static Terraria.ModLoader.ModContent;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace PathOfModifiers.Affixes.Items
{
    //TODO: When item is dropped roll only on server? How it work?
    public class ItemItem : GlobalItem
    {
        public static string GetBaseName(Item item) => Lang.GetItemNameValue(item.type);

        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return IsRollable(entity);
        }

        public RarityItem rarity;

        public List<Affix> affixes = new();
        public List<Affix> prefixes = new();
        public List<Affix> suffixes = new();

        public int FreeAffixes => rarity.maxAffixes - affixes.Count;
        public int FreePrefixes => Math.Min(FreeAffixes, rarity.maxPrefixes - prefixes.Count);
        public int FreeSuffixes => Math.Min(FreeAffixes, rarity.maxSuffixes - suffixes.Count);


        public override void SetDefaults(Item item)
        {
            item.StatsModifiedBy.Add(Mod);
        }

        public void PostLoad()
        {
            rarity = DataManager.Item.GetRarityRef(typeof(ItemNone));
        }

        /// <summary>
        /// Used to deep clone the affix
        /// </summary>
        void InitializeReferenceData()
        {
            affixes = new();
            prefixes = new();
            suffixes = new();
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
            //Why the fuck does 0 crit equal to 4% actual crit
            //return item.crit > 0;
            return item.DamageType.Type != DamageClass.Summon.Type;
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
            return item.DamageType.CountsAsClass(DamageClass.Melee) && item.noMelee && item.shoot > -1 && item.useStyle == 5 && !item.channel;
        }
        public static bool IsFlailOrYoyo(Item item)
        {
            return item.DamageType.CountsAsClass(DamageClass.Melee) && item.noMelee && item.shoot > -1 && item.useStyle == 5 && item.channel;
        }

        public static bool IsMelee(Item item)
        {
            return item.DamageType.CountsAsClass(DamageClass.Melee);
        }
        public static bool IsRanged(Item item)
        {
            return item.DamageType.CountsAsClass(DamageClass.Ranged);
        }
        public static bool IsMagic(Item item)
        {
            return item.DamageType.CountsAsClass(DamageClass.Magic);
        }
        public static bool IsThrowing(Item item)
        {
            return item.DamageType.CountsAsClass(DamageClass.Throwing);
        }
        public static bool IsSummon(Item item)
        {
            return item.DamageType.CountsAsClass(DamageClass.Summon);
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
            return item.ModItem is Map;
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
            for (int i = 3; i < 8 + player.GetAmountOfExtraAccessorySlotsToShow(); i++)
            {
                if (player.armor[i] == item)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        public void UpdateName(Item item)
        {
            if (rarity == null || rarity.GetType() == typeof(NotRollableItem))
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
                ClearAffixes(item);
                RerollRarity(item);
                RollForRandomAffixes(item, new Constraints.None());
                return true;
            }
            else
            {
                rarity = DataManager.Item.GetRarityRef(typeof(NotRollableItem));
                return false;
            }
        }
        public void RerollRarity(Item item, bool removeExtraAffixes = true)
        {
            rarity = DataManager.Item.RollRarity(item);

            if (removeExtraAffixes)
            {
                if (affixes.Count > rarity.maxAffixes)
                {
                    for (int i = FreeAffixes; i < 0; i++)
                    {
                        RemoveAffix(affixes.Last(), item);
                    }
                }
                if (prefixes.Count > rarity.maxPrefixes)
                {
                    for (int i = FreePrefixes; i < 0; i++)
                    {
                        RemoveAffix(prefixes.Last(), item);
                    }
                }
                if (suffixes.Count > rarity.maxSuffixes)
                {
                    for (int i = FreeSuffixes; i < 0; i++)
                    {
                        RemoveAffix(suffixes.Last(), item);
                    }
                }
            }
            UpdateName(item);
        }
        /// <summary>
        /// Validly adds an affixes to the item based on rarity
        /// </summary>
        /// <param name="item"></param>
        public void RollForRandomAffixes(Item item, Constraints.Constraint constraint)
        {
            int freeAffixes = FreeAffixes;
            for (int i = 0; i < freeAffixes; i++)
            {
                if (i >= rarity.minAffixes && Main.rand.NextFloat(0, 1) > rarity.chanceToRollAffix)
                    break;

                if (FreePrefixes <= 0)
                    constraint = constraint.Then(new Constraints.Suffixes());
                if (FreeSuffixes <= 0)
                    constraint = constraint.Then(new Constraints.Prefixes());

                if (!TryAddRandomAffix(item, constraint))
                    break;
            }
            UpdateName(item);
        }

        public bool CanRaiseRarity(Item item)
        {
            Type rarityType = rarity.GetType();
            return rarityType != typeof(AccessoryLegendary) && rarityType != typeof(ArmorLegendary) && rarityType != typeof(WeaponLegendary);
        }
        public bool TryRaiseRarity(Item item)
        {
            Type rarityType = rarity.GetType();
            bool raised = false;
            if (rarityType == typeof(WeaponCommon))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(WeaponUncommon));
                raised = true;
            }
            else if (rarityType == typeof(WeaponUncommon))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(WeaponRare));
                raised = true;
            }
            else if (rarityType == typeof(WeaponRare))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(WeaponEpic));
                raised = true;
            }
            else if (rarityType == typeof(WeaponEpic))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(WeaponLegendary));
                raised = true;
            }
            else if (rarityType == typeof(ArmorCommon))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(ArmorUncommon));
                raised = true;
            }
            else if (rarityType == typeof(ArmorUncommon))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(ArmorRare));
                raised = true;
            }
            else if (rarityType == typeof(ArmorRare))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(ArmorEpic));
                raised = true;
            }
            else if (rarityType == typeof(ArmorEpic))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(ArmorLegendary));
                raised = true;
            }
            else if (rarityType == typeof(AccessoryCommon))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(AccessoryUncommon));
                raised = true;
            }
            else if (rarityType == typeof(AccessoryUncommon))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(AccessoryRare));
                raised = true;
            }
            else if (rarityType == typeof(AccessoryRare))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(AccessoryEpic));
                raised = true;
            }
            else if (rarityType == typeof(AccessoryEpic))
            {
                rarity = DataManager.Item.GetRarityRef(typeof(AccessoryLegendary));
                raised = true;
            }
            if (raised)
                UpdateName(item);
            return raised;
        }
        public void RerollAffixes(Item item, Constraints.Constraint removeConstraint, Constraints.Constraint rollConstraint)
        {
            RemoveAllAffixes(item, removeConstraint);
            RollForRandomAffixes(item, rollConstraint);
        }
        /// <summary>
        /// Validly adds an affix to the item.
        /// </summary>
        /// <param name="item"></param>
        public bool TryAddRandomAffix(Item item, Constraints.Constraint constraint)
        {
            if (DataManager.Item.TryRollNewAffix(this, item, constraint, out var newAffix)
                && TryAddAffix(newAffix, item))
            {
                UpdateName(item);
                return true;
            }
            return false;
        }
        public bool TryRemoveRandomAffix(Item item, Constraints.Constraint constraint)
        {
            var constrainedAffixes = constraint.Process(affixes).ToArray();
            if (constrainedAffixes.Length > 0)
            {
                var affixToRemove = Main.rand.Next(constrainedAffixes);
                bool wasRemoved = RemoveAffix(affixToRemove, item);
                if (wasRemoved)
                    UpdateName(item);
                return wasRemoved;
            }
            else
            {
                return false;
            }
        }
        public void RemoveAllAffixes(Item item, Constraints.Constraint constraint)
        {
            var constrainedAffixes = constraint.Process(affixes).ToArray();
            foreach (var affixToRemove in constrainedAffixes)
            {
                RemoveAffix(affixToRemove, item);
            }
            UpdateName(item);
        }
        public void RollAffixTierMultipliers(Item item, Constraints.Constraint constraint)
        {
            var constrainedAffixes = constraint.Process(affixes).ToArray();
            foreach (var affix in constrainedAffixes)
            {
                if (affix is AffixTiered affixTiered)
                    affixTiered.RollValue(false);
            }
            UpdateName(item);
        }
        public void ImproveRandomAffixTierMultiplier(Item item, Constraints.Constraint constraint)
        {
            var constrainedAffixes = constraint.Process(affixes).ToArray();
            var affixToImprove = constrainedAffixes.Length > 0 ? Main.rand.Next(constrainedAffixes) : null;
            (affixToImprove as AffixTiered)?.ImproveValue();
            UpdateName(item);
        }
        public void ImproveRandomAffixTier(Item item, Constraints.Constraint constraint)
        {
            var constrainedAffixes = constraint.Process(affixes).ToArray();
            var affixToImprove = constrainedAffixes.Length > 0 ? Main.rand.Next(constrainedAffixes) : null;
            (affixToImprove as AffixTiered)?.ImproveCompoundTier();
            UpdateName(item);
        }
        public void ExchangeRandomAffix(Item item, Constraints.Constraint constraint)
        {
            var constrainedAffixes = constraint.Process(affixes).ToArray();
            var affixToExchange = constrainedAffixes.Length > 0 ? Main.rand.Next(constrainedAffixes) : null;
            if (affixToExchange is AffixTiered affixTiered)
            {
                RemoveAffix(affixToExchange, item);

                if (DataManager.Item.TryRollNewAffix(this, item, out var newAffix))
                {
                    if (newAffix is AffixTiered newAffixTiered)
                    {
                        float tierRatio = (float)affixTiered.CompoundTier / affixTiered.MaxCompoundTier;
                        int newTier = (int)MathF.Round(newAffixTiered.MaxCompoundTier * tierRatio);
                        newAffixTiered.SetCompoundTier(newTier);
                    }

                    TryAddAffix(newAffix, item);
                }
                else
                {
                    TryAddAffix(affixToExchange, item);
                }
                UpdateName(item);
            }
        }


        public bool TryAddAffix(Affix affix, Item item, bool isItemCloned = false)
        {
            if (FreeAffixes <= 0)
                return false;

            if (affix.IsPrefix)
            {
                if (FreePrefixes <= 0)
                    return false;

                prefixes.Add(affix);
            }

            if (affix.IsSuffix)
            {
                if (FreeSuffixes <= 0)
                    return false;

                suffixes.Add(affix);
            }

            affix.AddAffix(item, isItemCloned);

            affixes.Add(affix);

            return true;
        }
        public bool RemoveAffix(Affix affix, Item item)
        {
            var wasRemoved = false;
            affix.RemoveAffix(item);
            wasRemoved = affixes.Remove(affix);
            if (affix.IsPrefix)
                prefixes.Remove(affix);
            else
            {
                if (affix.IsSuffix)
                    suffixes.Remove(affix);
            }
            return wasRemoved;
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

        public override bool AllowPrefix(Item item, int pre)
        {
            return !((GetInstance<PoMConfigServer>().DisableVanillaPrefixesWeapons && IsWeapon(item))
                || (GetInstance<PoMConfigServer>().DisableVanillaPrefixesAccessories && IsAccessory(item)));
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            return (GetInstance<PoMConfigServer>().DisableVanillaPrefixesWeapons && IsWeapon(item))
                || (GetInstance<PoMConfigServer>().DisableVanillaPrefixesAccessories && IsAccessory(item))
                ? PrefixType<PoMPrefix>() : -1;
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
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            bool consume = true;
            foreach (var prefix in prefixes)
            {
                consume = consume && prefix.CanConsumeAmmo(weapon, ammo, player);
            }
            foreach (var suffix in suffixes)
            {
                consume = consume && suffix.CanConsumeAmmo(weapon, ammo, player);
            }
            return consume;
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            float multiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.ModifyWeaponCrit(item, player, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyWeaponCrit(item, player, ref multiplier);
            }
            crit *= multiplier;
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyWeaponDamage(item, player, ref damage);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyWeaponDamage(item, player, ref damage);
            }
        }
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            float multiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.ModifyWeaponKnockback(item, player, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyWeaponKnockback(item, player, ref multiplier);
            }
            knockback *= multiplier;
        }
        public override float UseSpeedMultiplier(Item item, Player player)
        {
            float multiplier = 1f;
            foreach (var prefix in prefixes)
            {
                prefix.UseSpeedMultiplier(item, player, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.UseSpeedMultiplier(item, player, ref multiplier);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool shoot = true;
            foreach (var prefix in prefixes)
            {
                if (!prefix.Shoot(item, player, source, position, velocity, type, damage, knockback))
                    shoot = false;
            }
            foreach (var suffix in suffixes)
            {
                if (!suffix.Shoot(item, player, source, position, velocity, type, damage, knockback))
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
            ItemPlayer pomPlayer = player.GetModPlayer<ItemPlayer>();
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
            if (rarity == null || rarity.GetType() == typeof(ItemNone))
            {
                try
                {
                    TryRollItem(item);
                }
                catch (Exception e)
                {
                    Mod.Logger.Error(e.ToString());
                }
            }

            ItemPlayer pomPlayer = player.GetModPlayer<ItemPlayer>();
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
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyItemScale(item, player, ref scale);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyItemScale(item, player, ref scale);
            }
        }
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            var damage = modifiers.FinalDamage;
            var knockback = modifiers.Knockback;

            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;

            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitNPC(item, player, target, ref damageMultiplier, ref knockbackMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitNPC(item, player, target, ref damageMultiplier, ref knockbackMultiplier, ref modifiers);
            }

            damage *= damageMultiplier;
            knockback *= knockbackMultiplier;

            modifiers.FinalDamage = damage;
            modifiers.Knockback = knockback;
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            var damage = modifiers.FinalDamage;

            float damageMultiplier = 1f;

            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitPvp(item, player, target, ref damageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitPvp(item, player, target, ref damageMultiplier, ref modifiers);
            }

            damage *= damageMultiplier;

            modifiers.FinalDamage = damage;
        }
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitNPC(item, player, target, hit, damageDone);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitNPC(item, player, target, hit, damageDone);
            }
        }
        public override void OnHitPvp(Item item, Player player, Player target, Player.HurtInfo hurtInfo)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitPvp(item, player, target, hurtInfo);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitPvp(item, player, target, hurtInfo);
            }
        }
        #endregion
        #region Projectile hooks
        public void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ProjModifyHitNPC(item, player, projectile, target, ref damageMultiplier, ref knockbackMultiplier, ref critDamageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ProjModifyHitNPC(item, player, projectile, target, ref damageMultiplier, ref knockbackMultiplier, ref critDamageMultiplier, ref modifiers);
            }
        }
        public void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ProjModifyHitPvp(item, player, projectile, target, ref damageMultiplier, ref critDamageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ProjModifyHitPvp(item, player, projectile, target, ref damageMultiplier, ref critDamageMultiplier, ref modifiers);
            }
        }
        public void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ProjOnHitNPC(item, player, projectile, target, hit, damageDone);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ProjOnHitNPC(item, player, projectile, target, hit, damageDone);
            }
        }
        public void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ProjOnHitPvp(item, player, projectile, target, modifiers, damageDone);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ProjOnHitPvp(item, player, projectile, target, modifiers, damageDone);
            }   
        }
        #endregion
        // Player hooks trigger on the whole inventory and equipped items;
        #region Player Hooks
        public void PlayerGetHealLife(Item item, Item healItem, ref float multiplier)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerGetHealLife(item, healItem, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerGetHealLife(item, healItem, ref multiplier);
            }
        }
        public void PlayerGetHealMana(Item item, Item healItem, ref float multiplier)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerGetHealMana(item, healItem, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerGetHealMana(item, healItem, ref multiplier);
            }
        }
        public void PlayerModifyLuck(Item item, ref float luck)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerModifyLuck(item, ref luck);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerModifyLuck(item, ref luck);
            }
        }
        public void PlayerModifyMaxStats(Item item, ref StatModifier health, ref StatModifier mana)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerModifyMaxStats(item, ref health, ref mana);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerModifyMaxStats(item, ref health, ref mana);
            }
        }
        public void PlayerModifyCaughtFish(Item item, Item fish, ref float multiplier)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerModifyCaughtFish(item, fish, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerModifyCaughtFish(item, fish, ref multiplier);
            }
        }
        public bool? PlayerCanConsumeBait(Item item, Item bait)
        {
            bool? consume = null;
            foreach (var prefix in prefixes)
            {
                consume = prefix.PlayerCanConsumeBait(bait) ?? consume;
            }
            foreach (var suffix in suffixes)
            {
                consume = suffix.PlayerCanConsumeBait(bait) ?? consume;
            }
            return consume;
        }
        public bool PlayerCanConsumeAmmo(Player player, Item item, Item ammo)
        {
            bool consume = true;
            foreach (var prefix in prefixes)
            {
                consume = consume && prefix.PlayerConsumeAmmo(player, item, ammo);
            }
            foreach (var suffix in suffixes)
            {
                consume = consume && suffix.PlayerConsumeAmmo(player, item, ammo);
            }
            return consume;
        }
        public bool FreeDodge(Item item, Player player, ref Player.HurtInfo info)
        {
            bool dodge = false;
            foreach (var prefix in prefixes)
            {
                dodge |= prefix.FreeDodge(item, player, ref info);
            }
            foreach (var suffix in suffixes)
            {
                dodge |= suffix.FreeDodge(item, player, ref info);
            }
            return dodge;
        }
        public void PreHurt(Item item, Player player, ref float damageMultiplier, ref Player.HurtModifiers modifiers)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PreHurt(item, player, ref damageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PreHurt(item, player, ref damageMultiplier, ref modifiers);
            }
        }
        public void PostHurt(Item item, Player player, Player.HurtInfo info)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PostHurt(item, player, info);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PostHurt(item, player, info);
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
        public void PlayerModifyWeaponCrit(Item item, Item heldItem, Player player, ref float multiplier)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerModifyWeaponCrit(item, heldItem, player, ref multiplier);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerModifyWeaponCrit(item, heldItem, player, ref multiplier);
            }
        }
        public void ModifyHitByNPC(Item item, Player player, NPC npc, ref float damageMultiplier, ref Player.HurtModifiers modifiers)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitByNPC(item, player, npc, ref damageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitByNPC(item, player, npc, ref damageMultiplier, ref modifiers);
            }
        }
        public void ModifyHitByPvp(Item item, Player player, Player attacker, ref float damageMultiplier, ref Player.HurtModifiers modifiers)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitByPvp(item, player, attacker, ref damageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitByPvp(item, player, attacker, ref damageMultiplier, ref modifiers);
            }
        }
        public void ModifyHitByProjectile(Item item, Player player, Projectile projectile, ref float damageMultiplier, ref Player.HurtModifiers modifiers)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitByProjectile(item, player, projectile, ref damageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitByProjectile(item, player, projectile, ref damageMultiplier, ref modifiers);
            }
        }
        public void OnHitByNPC(Item item, Player player, NPC npc, Player.HurtInfo hurtInfo)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitByNPC(item, player, npc, hurtInfo);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitByNPC(item, player, npc, hurtInfo);
            }
        }
        public void OnHitByPvp(Item item, Player player, Player attacker, Player.HurtInfo info)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitByPvp(item, player, attacker, info);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitByPvp(item, player, attacker, info);
            }
        }
        public void OnHitByProjectile(Item item, Player player, Projectile projectile, Player.HurtInfo hurtInfo)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitByProjectile(item, player, projectile, hurtInfo);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitByProjectile(item, player, projectile, hurtInfo);
            }
        }
        public void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref critDamageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref critDamageMultiplier, ref modifiers);
            }
        }
        public void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref critDamageMultiplier, ref modifiers);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref critDamageMultiplier, ref modifiers);
            }
        }
        public void PlayerOnHitNPC(Item affixItem, Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerOnHitNPC(affixItem, player, item, target, hit, damageDone);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerOnHitNPC(affixItem, player, item, target, hit, damageDone);
            }
        }
        public void PlayerOnHitPvp(Item affixItem, Player player, Item item, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            foreach (var prefix in prefixes)
            {
                prefix.PlayerOnHitPvp(affixItem, player, item, target, modifiers, damageDone);
            }
            foreach (var suffix in suffixes)
            {
                suffix.PlayerOnHitPvp(affixItem, player, item, target, modifiers, damageDone);
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

        public bool PlayerShoot(Item affixItem, Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool shoot = true;
            foreach (var prefix in prefixes)
            {
                if (!prefix.PlayerShoot(affixItem, player, item, source, position, velocity, type, damage, knockback))
                    shoot = false;
            }
            foreach (var suffix in suffixes)
            {
                if (!suffix.PlayerShoot(affixItem, player, item, source, position, velocity, type, damage, knockback))
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
                Mod.Logger.Error(e.ToString());
            }
        }
        //On craft
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            try
            {
                TryRollItem(item);
            }
            catch (Exception e)
            {
                Mod.Logger.Error(e.ToString());
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (rarity == null)
            {
                //Item is in the crafting window
                return;
            }

            foreach (var prefix in prefixes)
            {
                prefix.ModifyTooltips(PathOfModifiers.Instance, item, tooltips);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyTooltips(PathOfModifiers.Instance, item, tooltips);
            }
            if (rarity.GetType() != typeof(ItemNone) && rarity.GetType() != typeof(NotRollableItem))
            {
                foreach (TooltipLine line in tooltips)
                {
                    if (line.Mod == "Terraria" && line.Name == "ItemName")
                    {
                        line.OverrideColor = rarity.color;
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
                    Mod.Logger.Error(e.ToString());
                }
            }
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
        }
        public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
        {
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            if (rarity == null)
                return;

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
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            string rarityModName = tag.GetString("rarityMod");
            ModLoader.TryGetMod(rarityModName, out Mod mod);
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
            rarity = DataManager.Item.GetRarityRef(type);
            int affixCount = tag.GetAsInt("affixCount");
            TagCompound affixTag;
            Affix affix;
            for (int i = 0; i < affixCount; i++)
            {
                affixTag = tag.GetCompound(i.ToString());
                string affixModName = affixTag.GetString("affixMod");
                ModLoader.TryGetMod(affixModName, out mod);
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
                affix = DataManager.Item.GetNewAffix(type);
                affix.Load(affixTag, item);
                TryAddAffix(affix, item);
            }
            UpdateName(item);
        }

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            ItemItem newItem = (ItemItem)base.Clone(item, itemClone);
            newItem.InitializeReferenceData();

            Affix affixClone;
            foreach (Affix affix in affixes)
            {
                affixClone = affix.Clone();
                newItem.TryAddAffix(affixClone, item, true);
            }
            newItem.UpdateName(itemClone);

            return newItem;
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            try
            {
                writer.Write(DataManager.Item.GetRarityIndex(rarity.GetType()));

                writer.Write((byte)affixes.Count);
                Affix affix;
                for (int i = 0; i < affixes.Count; i++)
                {
                    affix = affixes[i];
                    writer.Write(DataManager.Item.GetAffixIndex(affix.GetType()));
                    affix.NetSend(item, writer);
                }
            }
            catch (Exception e)
            {
                Mod.Logger.Error(e.ToString());
            }
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            try
            {
                rarity = DataManager.Item.GetRarityRef(reader.ReadInt32());

                int affixCount = reader.ReadByte();
                Affix affix;
                for (int i = 0; i < affixCount; i++)
                {
                    affix = DataManager.Item.GetNewAffix(reader.ReadInt32());
                    affix.NetReceive(item, reader);
                    TryAddAffix(affix, item);
                }
                UpdateName(item);
            }
            catch (Exception e)
            {
                Mod.Logger.Error(e.ToString());
            }
        }
    }
}
