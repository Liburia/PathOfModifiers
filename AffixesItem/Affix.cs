using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ID;
using System.IO;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace PathOfModifiers.AffixesItem
{
    public class Affix
    {
        public Mod mod;

        public virtual float weight => 0;
        public virtual Color color => new Color(0.99f, 0.99f, 0.99f, 1);

        public virtual string addedText => string.Empty;
        public virtual float addedTextWeight => 1;

        public virtual void InitializeItem(PoMItem item)
        {
            RollValue();
        }

        public virtual Affix Clone()
        {
            Affix newAffix = (Affix)Activator.CreateInstance(GetType());
            newAffix.mod = mod;
            return newAffix;
        }

        public virtual bool CanBeRolled(PoMItem pomItem, Item item) { return false; }
        public virtual bool AffixSpaceAvailable(PoMItem item) { return item.FreeAffixes > 0; }

        public virtual void RollValue(bool rollTier = true) { }
        #region Item Hooks
        public virtual void GetWeaponCrit(Item item, Player player, ref float multiplier) { }
        public virtual void ModifyWeaponDamage(Item item, Player player, ref float multiplier, ref float flat) { }
        public virtual void GetWeaponKnockback(Item item, Player player, ref float multiplier) { }
        public virtual void UseTimeMultiplier(Item item, Player player, ref float multiplier) { }
        public virtual void UpdateInventory(Item item, PoMPlayer player) { }
        public virtual void UpdateEquip(Item item, PoMPlayer player) { }
        public virtual bool ConsumeAmmo(Item item, Player player) { return true; }
        public virtual void HoldItem(Item item, Player player) { }
        public virtual void UseItem(Item item, Player player) { }
        public virtual void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit) { }
        public virtual void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit) { }
        public virtual void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit) { }
        public virtual void OnHitPvp(Item item, Player player, Player target, int damage, bool crit) { }
        public virtual bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) { return true; }
        #endregion
        #region Projectile Hooks
        public virtual void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection) { }
        public virtual void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit) { }
        public virtual void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit) { }
        public virtual void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit) { }
        #endregion
        #region Player Hooks
        public virtual bool PlayerConsumeAmmo(Player player, Item item, Item ammo) { return true; }
        public virtual bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) { return true; }
        public virtual void NaturalLifeRegen(Item item, Player player, ref float regen) { }
        public virtual void ModifyHitByNPC(Item item, Player player, NPC npc, ref int damage, ref bool crit) { }
        public virtual void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit) { }
        public virtual bool PlayerShoot(Item affixItem, Player player, Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) { return true; }

        public virtual void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackmultiplier, ref bool crit) { }
        public virtual void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref bool crit) { }
        public virtual void PlayerOnHitNPC(Item affixItem, Player player, Item item, NPC target, int damage, float knockback, bool crit) { }
        public virtual void PlayerOnHitPvp(Item affixItem, Player player, Item item, Player target, int damage, bool crit) { }

        public virtual void PlayerOnKillNPC(Item item, Player player, NPC target) { }
        public virtual void PlayerOnKillPvp(Item item, Player player, Player target) { }
        #endregion
        public virtual void ModifyTooltips(Mod mod, Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, GetType().Name, GetTolltipText(item));
            line.overrideColor = color;
            tooltips.Add(line);
        }
        public virtual string GetTolltipText(Item item) { return string.Empty; }

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
    }

    public abstract class Prefix : Affix
    {
        public override Color color => new Color(0.25f, 1, 0.25f, 1);

        public override bool AffixSpaceAvailable(PoMItem item) { return item.FreePrefixes > 0; }
    }

    public abstract class Suffix : Affix
    {
        public override Color color => new Color(0.25f, 1, 1, 1);

        public override bool AffixSpaceAvailable(PoMItem item) { return item.FreeSuffixes > 0; }
    }
}
