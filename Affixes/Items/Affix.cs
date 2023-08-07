using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.WorldBuilding;

namespace PathOfModifiers.Affixes.Items
{
    [DisableAffix]
    public class Affix : IUIDrawable
    {
        public static readonly Color prefixColor = new Color(0.25f, 1, 0.25f, 1);
        public static readonly Color suffixColor = new Color(0.25f, 1, 1, 1);

        public bool IsPrefix => this is IPrefix;
        public bool IsSuffix => this is ISuffix;

        public Mod mod;

        Color? _color;
        public Color Color
        {
            get
            {
                if (!_color.HasValue)
                {
                    _color = this is IPrefix ? prefixColor : suffixColor;
                }

                return _color.Value;
            }
        }

        public bool AffixSpaceAvailable(ItemItem item) { return this is IPrefix ? item.FreePrefixes > 0 : item.FreeSuffixes > 0; }

        public virtual double Weight => 0;

        public virtual string AddedText => string.Empty;
        public virtual double AddedTextWeight => 1;

        public Affix()
        {

        }

        public virtual Affix Clone()
        {
            Affix newAffix = (Affix)Activator.CreateInstance(GetType());
            newAffix.mod = mod;
            return newAffix;
        }

        public virtual bool CanRoll(ItemItem pomItem, Item item) { return false; }
        #region Item Hooks
        public virtual void ModifyWeaponCrit(Item item, Player player, ref float multiplier) { }
        public virtual void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) { }
        public virtual void ModifyWeaponKnockback(Item item, Player player, ref float multiplier) { }
        public virtual void UseSpeedMultiplier(Item item, Player player, ref float multiplier) { }
        public virtual void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult) { }
        public virtual void UpdateInventory(Item item, ItemPlayer player) { }
        public virtual void UpdateEquip(Item item, ItemPlayer player) { }
        public virtual bool CanConsumeAmmo(Item item, Item ammo, Player player) { return true; }
        public virtual void HoldItem(Item item, Player player) { }
        public virtual void ModifyItemScale(Item item, Player player, ref float scale) { }
        public virtual void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref NPC.HitModifiers modifiers) { }
        public virtual void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref Player.HurtModifiers modifiers) { }
        public virtual void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnHitPvp(Item item, Player player, Player target, Player.HurtInfo hurtInfo) { }
        public virtual bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) { return true; }
        #endregion
        #region Projectile Hooks
        public virtual void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers) { }
        public virtual void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers) { }
        public virtual void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, Player.HurtModifiers modifiers, int damageDone) { }
        #endregion
        #region Player Hooks
        public virtual void PlayerGetHealLife(Item item, Item healItem, ref float healMultiplier) { }
        public virtual void PlayerGetHealMana(Item item, Item healItem, ref float healMultiplier) { }
        public virtual void PlayerModifyLuck(Item item, ref float luck) { }
        public virtual void PlayerModifyMaxStats(Item item, ref StatModifier health, ref StatModifier mana) { health = StatModifier.Default; mana = StatModifier.Default; }
        public virtual void PlayerModifyCaughtFish(Item item, Item fish, ref float multiplier) { }
        public virtual bool? PlayerCanConsumeBait(Item bait) { return null; }
        public virtual bool PlayerConsumeAmmo(Player player, Item item, Item ammo) { return true; }
        public virtual bool FreeDodge(Item item, Player player, ref Player.HurtInfo info) { return false; }
        public virtual void PreHurt(Item item, Player player, ref float damageMultiplier, ref Player.HurtModifiers modifiers) { }
        public virtual void PostHurt(Item item, Player player, Player.HurtInfo info) { }
        public virtual void NaturalLifeRegen(Item item, Player player, ref float regen) { }
        public virtual void PlayerModifyWeaponCrit(Item item, Item heldItem, Player player, ref float multiplier) { }
        public virtual void ModifyHitByNPC(Item item, Player player, NPC npc, ref float damageMultiplier, ref Player.HurtModifiers modifiers) { }
        public virtual void ModifyHitByPvp(Item item, Player player, Player attacker, ref float damageMultiplier, ref Player.HurtModifiers modifiers) { }
        public virtual void ModifyHitByProjectile(Item item, Player player, Projectile projectile, ref float damageMultiplier, ref Player.HurtModifiers modifiers) { }
        public virtual void OnHitByNPC(Item item, Player player, NPC npc, Player.HurtInfo hurtInfo) { }
        public virtual void OnHitByPvp(Item item, Player player, Player attacker, Player.HurtInfo info) { }
        public virtual void OnHitByProjectile(Item item, Player player, Projectile projectile, Player.HurtInfo hurtInfo) { }
        public virtual bool PlayerShoot(Item affixItem, Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) { return true; }

        public virtual void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers) { }
        public virtual void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers) { }
        public virtual void PlayerOnHitNPC(Item affixItem, Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void PlayerOnHitPvp(Item affixItem, Player player, Item item, Player target, Player.HurtModifiers modifiers, int damageDone) { }

        public virtual void PlayerOnKillNPC(Item item, Player player, NPC target) { }
        #endregion
        public virtual void ModifyTooltips(Mod mod, Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, GetType().Name, GetAffixText())
            {
                OverrideColor = Color
            };
            tooltips.Add(line);
        }
        public virtual string GetAffixText(bool useChatTags = false) { return string.Empty; }

        public virtual string GetForgeText() { return GetAffixText(); }

        public virtual void Save(TagCompound tag, Item item)
        {
        }
        public virtual void Load(TagCompound tag, Item item)
        {
        }

        public virtual void NetSend(Item item, BinaryWriter writer)
        {
        }
        public virtual void NetReceive(Item item, BinaryReader reader)
        {
        }

        public virtual void ReforgePrice(Item item, ref int price) { }

        public virtual void AddAffix(Item item, bool clone) { }
        public virtual void RemoveAffix(Item item) { }

        UIElement IUIDrawable.CreateUI(UIElement parent, Action onChangeCallback)
        {
            Terraria.GameContent.UI.Elements.UIText text = new("Affix has no UI", 1);
            text.IgnoresMouseInteraction = true;
            text.Top.Set(0, 0);
            text.Height.Set(20, 0);
            parent.Append(text);

            return text;
        }
    }
}
