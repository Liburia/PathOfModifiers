using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace PathOfModifiers.Affixes.NPCs
{
    [DisableAffix]
    public class Affix : IUIDrawable
    {
        public Mod mod;
        public virtual double Weight => 0;
        public virtual Color Color => new Color(0.99f, 0.99f, 0.99f, 1);

        public virtual string AddedText => string.Empty;
        public virtual double AddedTextWeight => 1;

        public virtual void InitializeNPC(NPCNPC pomNPC, NPC npc)
        {
            RollValue();

            SetStaticDefaults(pomNPC, npc);
        }

        public virtual Affix Clone()
        {
            Affix newAffix = (Affix)Activator.CreateInstance(GetType());
            newAffix.mod = mod;
            return newAffix;
        }

        public virtual bool CanRoll(NPCNPC pomNPC, NPC npc) { return false; }
        public virtual bool AffixSpaceAvailable(NPCNPC npc) { return npc.FreeAffixes > 0; }

        public virtual void RollValue(bool rollTier = true) { }

        #region NPC Hooks
        public virtual void SetStaticDefaults(NPCNPC pomNPC, NPC npc) { }
        public virtual bool? CanBeHitByItem(NPC npc, Player player, Item item) { return null; }
        public virtual bool? CanBeHitByProjectile(NPC npc, Projectile projectile) { return null; }
        public virtual void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit) { }
        public virtual void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) { }
        public virtual void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit) { }
        public virtual void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit) { }
        public virtual void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) { }
        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) { }
        public virtual void OnHitPlayer(NPC npc, Player target, int damage, bool crit) { }
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


        public virtual void Save(TagCompound tag)
        {
        }
        public virtual void Load(TagCompound tag)
        {
        }

        public virtual void NetSend(BinaryWriter writer)
        {
        }
        public virtual void NetReceive(BinaryReader reader)
        {
        }

        public virtual void AddAffix(NPC npc, bool clone) { }
        public virtual void RemoveAffix(NPC npc) { }

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
