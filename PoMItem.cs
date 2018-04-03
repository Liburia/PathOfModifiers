using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using PathOfModifiers.Affixes;
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
    public class PoMItem : GlobalItem
    {
        public override bool CloneNewInstances => false;

        public override bool InstancePerEntity => true;

        public Rarity rarity;

        public List<Affix> affixes;
        public List<Prefix> prefixes;
        public List<Suffix> suffixes;

        public int FreeAffixes => rarity.maxAffixes - affixes.Count;
        public int FreePrefixes => Math.Min(FreeAffixes, rarity.maxPrefixes - prefixes.Count);
        public int FreeSuffixes => Math.Min(FreeAffixes, rarity.maxSuffixes - suffixes.Count);

        public PoMItem()
        {
            rarity = ((PoMAffixController.rarities?.Length ?? 0) == 0) ? new None() : PoMAffixController.rarities[PoMAffixController.rarityMap[typeof(None)]];
            affixes = new List<Affix>();
            prefixes = new List<Prefix>();
            suffixes = new List<Suffix>();
        }

        #region Item conditions
        public static bool IsRollable(Item item)
        {
            return IsWeapon(item) || IsAccessory(item) || IsAnyArmor(item);
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
                rarity = PoMAffixController.rarities[PoMAffixController.rarityMap[typeof(None)]];
                return false;
            }
        }
        public void RollItem(Item item)
        {
            ClearAffixes(item);
            rarity = PoMAffixController.RollRarity(item);
            Affix newAffix;
            int freeAffixes = FreeAffixes;
            for (int i = 0; i < freeAffixes; i++)
            {
                if (i != 0 && Main.rand.NextFloat(0, 1) > rarity.chanceToRollAffix)
                    break;

                newAffix = PoMAffixController.RollNewAffix(this, item);
                if (newAffix == null)
                    break;

                AddAffix(newAffix, item);
            }
            UpdateName(item);
        }
        public void UpdateName(Item item)
        {
            if (rarity == null || rarity.GetType() == typeof(None))
                item.ClearNameOverride();
            else
            {
                string addedPrefix = string.Empty;
                float addedPrefixWeight = 0f;
                foreach (Prefix prefix in prefixes)
                {
                    if (prefix.addedTextWeight > addedPrefixWeight && prefix.addedText != string.Empty)
                        addedPrefix = prefix.addedText;
                }
                string addedSuffix = string.Empty;
                float addedSuffixWeight = 0f;
                foreach (Suffix suffix in suffixes)
                {
                    if (suffix.addedTextWeight > addedSuffixWeight && suffix.addedText != string.Empty)
                        addedSuffix = suffix.addedText;
                }
                item.SetNameOverride($"{rarity.name} {addedPrefix}{(addedPrefix != string.Empty ? " " : string.Empty)}{GetBaseName(item)}{(addedSuffix != string.Empty ? " " : string.Empty)}{addedSuffix} [{FreeAffixes}] [{FreePrefixes}] [{FreeSuffixes}]");
                item.rare = rarity.vanillaRarity;
            }
        }

        public void AddAffix(Affix affix, Item item, bool clone = false)
        {
            affix.AddAffix(item, clone);

            affixes.Add(affix);

            Prefix prefix = affix as Prefix;
            if (prefix != null)
            {
                prefixes.Add(prefix);
                return;
            }

            Suffix suffix = affix as Suffix;
            if (suffix != null)
                suffixes.Add(suffix);
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

        #region Affix Hooks
        public override bool ConsumeAmmo(Item item, Player player)
        {
            foreach (Prefix prefix in prefixes)
            {
                if (!prefix.ConsumeAmmo(item, player))
                    return false;
            }
            foreach (Suffix suffix in suffixes)
            {
                if (!suffix.ConsumeAmmo(item, player))
                    return false;
            }
            return true;
        }
        public override void GetWeaponCrit(Item item, Player player, ref int crit)
        {
            float multiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.GetWeaponDamage(item, player, ref multiplier);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.GetWeaponDamage(item, player, ref multiplier);
            }
            crit = (int)Math.Round(crit * multiplier);
        }
        public override void GetWeaponDamage(Item item, Player player, ref int damage)
        {
            float multiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.GetWeaponDamage(item, player, ref multiplier);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.GetWeaponDamage(item, player, ref multiplier);
            }
            damage = (int)Math.Round(damage * multiplier);
        }
        public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
        {
            float multiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.GetWeaponKnockback(item, player, ref multiplier);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.GetWeaponKnockback(item, player, ref multiplier);
            }
            knockback = (float)Math.Round(knockback * multiplier);
        }
        public override float UseTimeMultiplier(Item item, Player player)
        {
            float multiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.UseTimeMultiplier(item, player, ref multiplier);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.UseTimeMultiplier(item, player, ref multiplier);
            }
            return multiplier;
        }
        public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //TODO: affix projectile count/speed/damage?/knockback?
            return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            //TODO: affix wings
        }
        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            //TODO: affix wings
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            //TODO: affix accessories
        }
        public override void UpdateEquip(Item item, Player player)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.UpdateEquip(item, player);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.UpdateEquip(item, player);
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.UpdateInventory(item, player);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.UpdateInventory(item, player);
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.HoldItem(item, player);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.HoldItem(item, player);
            }
        }
        public override bool UseItem(Item item, Player player)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.UseItem(item, player);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.UseItem(item, player);
            }
            return false;
        }
        public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.ModifyHitNPC(item, player, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.ModifyHitNPC(item, player, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            knockBack *= knockbackMultiplier;
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref int damage, ref bool crit)
        {
            float damageMultiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.ModifyHitPvp(item, player, target, ref damageMultiplier, ref crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.ModifyHitPvp(item, player, target, ref damageMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.OnHitNPC(item, player, target, damage, knockBack, crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.OnHitNPC(item, player, target, damage, knockBack, crit);
            }
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.OnHitPvp(item, player, target, damage, crit);
            }
            foreach (Suffix suffix in suffixes)
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
            foreach (Prefix prefix in prefixes)
            {
                prefix.ProjModifyHitNPC(item, player, projectile, target, ref damageMultiplier, ref knockbackMultiplier, ref crit, ref hitDirection);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.ProjModifyHitNPC(item, player, projectile, target, ref damageMultiplier, ref knockbackMultiplier, ref crit, ref hitDirection);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            knockback *= knockbackMultiplier;
        }
        public void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref int damage, ref bool crit)
        {
            float damageMultiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.ProjModifyHitPvp(item, player, projectile, target, ref damageMultiplier, ref crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.ProjModifyHitPvp(item, player, projectile, target, ref damageMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.ProjOnHitNPC(item, player, projectile, target, damage, knockback, crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.ProjOnHitNPC(item, player, projectile, target, damage, knockback, crit);
            }
        }
        public void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.ProjOnHitPvp(item, player, projectile, target, damage, crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.ProjOnHitPvp(item, player, projectile, target, damage, crit);
            }
        }
        #endregion
        #region Player Hooks
        public bool PlayerConsumeAmmo(Player player, Item item, Item ammo)
        {
            foreach (Prefix prefix in prefixes)
            {
                if (!prefix.PlayerConsumeAmmo(player, item, ammo))
                    return false;
            }
            foreach (Suffix suffix in suffixes)
            {
                if (!suffix.PlayerConsumeAmmo(player, item, ammo))
                    return false;
            }
            return true;
        }
        public bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            float damageMultiplier = 1f;
            bool hurt = true;
            foreach (Prefix prefix in prefixes)
            {
                if (!prefix.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            foreach (Suffix suffix in suffixes)
            {
                if (!suffix.PreHurt(item, player, pvp, quiet, ref damageMultiplier, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource))
                    hurt = false;
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            return hurt;
        }
        public void NaturalLifeRegen(Item item, Player player, ref float regen)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.NaturalLifeRegen(item, player, ref regen);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.NaturalLifeRegen(item, player, ref regen);
            }
        }
        public void ModifyHitByNPC(Item item, Player player, NPC npc, ref int damage, ref bool crit)
        {
            float damageMultiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.ModifyHitByNPC(item, player, npc, ref damage, ref crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.ModifyHitByNPC(item, player, npc, ref damage, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.OnHitByNPC(item, player, npc, damage, crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.OnHitByNPC(item, player, npc, damage, crit);
            }
        }
        public void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            float damageMultiplier = 1f;
            float knockbackMultiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.PlayerModifyHitNPC(affixItem, player, item, target, ref damageMultiplier, ref knockbackMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
            knockback *= knockbackMultiplier;
        }
        public void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref int damage, ref bool crit)
        {
            float damageMultiplier = 1f;
            foreach (Prefix prefix in prefixes)
            {
                prefix.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.PlayerModifyHitPvp(affixItem, player, item, target, ref damageMultiplier, ref crit);
            }
            damage = (int)Math.Round(damage * damageMultiplier);
        }
        public void PlayerOnHitNPC(Item affixItem, Player player, Item item, NPC target, int damage, float knockback, bool crit)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.PlayerOnHitNPC(affixItem, player, item, target, damage, knockback, crit);
            }
        }
        public void PlayerOnHitPvp(Item affixItem, Player player, Item item, Player target, int damage, bool crit)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.PlayerOnHitPvp(affixItem, player, item, target, damage, crit);
            }
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
                ErrorLogger.Log(e.ToString());
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
                ErrorLogger.Log(e.ToString());
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.ModifyTooltips(PathOfModifiers.Instance, item, tooltips);
            }
            foreach (Suffix suffix in suffixes)
            {
                suffix.ModifyTooltips(PathOfModifiers.Instance, item, tooltips);
            }
            if (rarity.GetType() != typeof(None))
            {
                foreach(TooltipLine line in tooltips)
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
            //TODO: Or just use PostUpdate?
        }
        public override void PostUpdate(Item item)
        {
            if (rarity == null || rarity.GetType() == typeof(None))
            {
                try
                {
                    TryRollItem(item);
                }
                catch (Exception e)
                {
                    ErrorLogger.Log(e.ToString());
                }
            }
            //TODO: Add light/dust?
        }
        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Main.npcShop == 0 && (rarity == null || rarity.GetType() == typeof(None)))
            {
                try
                {
                    TryRollItem(item);
                }
                catch (Exception e)
                {
                    ErrorLogger.Log(e.ToString());
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
                PathOfModifiers.Log($"PathOfModifiers: Mod not found {rarityModName}");
                return;
            }
            string rarityFullName = tag.GetString("rarityFullName");
            Type type = mod.Code.GetType(rarityFullName);
            if (type == null)
            {
                PathOfModifiers.Log($"PathOfModifiers: Rarity not found {rarityFullName}");
                return;
            }
            rarity = PoMAffixController.rarities[PoMAffixController.rarityMap[type]];
            int affixCount = tag.GetAsInt("affixCount");
            TagCompound affixTag;
            Affix affix;
            for (int i = 0; i < affixCount; i++)
            {
                affixTag = tag.GetCompound(i.ToString());
                mod = ModLoader.GetMod(affixTag.GetString("affixMod"));
                if (mod == null)
                {
                    PathOfModifiers.Log("PathOfModifiers: Mod not found");
                    continue;
                }
                type = mod.Code.GetType(affixTag.GetString("affixFullName"));
                if (type == null)
                {
                    PathOfModifiers.Log("PathOfModifiers: Affix not found");
                    continue;
                }
                affix = PoMAffixController.affixes[PoMAffixController.affixMap[type]].Clone();
                affix.Load(affixTag, item);
                AddAffix(affix, item);
            }
            UpdateName(item);
        }

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            PoMItem newItem = (PoMItem)NewInstance(itemClone);

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
                writer.Write(PoMAffixController.rarityMap[rarity.GetType()]);

                writer.Write((byte)affixes.Count);
                Affix affix;
                for(int i = 0; i < affixes.Count; i++)
                {
                    affix = affixes[i];
                    writer.Write(PoMAffixController.affixMap[affix.GetType()]);
                    affix.NetSend(item, writer);
                }
            }
            catch(Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            try
            {
                rarity = PoMAffixController.rarities[reader.ReadInt32()];

                int affixCount = reader.ReadByte();
                Affix affix;
                for (int i = 0; i < affixCount; i++)
                {
                    affix = PoMAffixController.affixes[reader.ReadInt32()].Clone();
                    affix.NetReceive(item, reader);
                    AddAffix(affix, item);
                }
                UpdateName(item);
            }
            catch (Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
    }
}
