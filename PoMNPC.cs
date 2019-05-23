using Microsoft.Xna.Framework;
using PathOfModifiers.AffixesNPC;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;
using PathOfModifiers.Buffs;
using Terraria.ID;
using PathOfModifiers.Rarities;
using System.Text;
using System.IO;

namespace PathOfModifiers
{
    public class PoMNPC : GlobalNPC
    {
        /// <summary>
        /// Set to true to skip rolling rarity and affixes for the next NPC that is spawned(and has its SetDefaults called)
        /// </summary>
        public static bool dontRollNextNPC = false;

        public override bool InstancePerEntity => true;

        public Entity lastDamageDealer;

        /// <summary>
        /// Stores the damage of the hit that procced the debuff.
        /// </summary>
        Dictionary<Type, int> damageDotDebuffDamages = new Dictionary<Type, int>();

        public bool dddDamageDotDebuff = false;

        public void AddDamageDoTBuff(NPC npc, DamageDoTDebuff buff, int damage, int time, bool syncMP = true, int ignoreClient = -1)
        {
            int dddDamage = 0;
            Type buffType = buff.GetType();
            if (damageDotDebuffDamages.TryGetValue(buffType, out dddDamage))
            {
                if (damage > dddDamage)
                    damageDotDebuffDamages[buffType] = damage;
            }
            else
            {
                damageDotDebuffDamages.Add(buffType, damage);
            }
            npc.AddBuff(buff.Type, time, true);

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                PoMNetMessage.AddDamageDoTDebuffNPC(npc.whoAmI, buff.Type, damage, time);
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<PoMNPC>().lastDamageDealer = npc;
        }

        public override void ResetEffects(NPC npc)
        {
            dddDamageDotDebuff = false;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            int debuffDamage;
            if (dddDamageDotDebuff)
            {
                debuffDamage = (int)Math.Round(damageDotDebuffDamages[typeof(DamageDoTDebuff)] * DamageDoTDebuff.damageMultiplierHalfSecond);
                npc.lifeRegen -= debuffDamage;
            }
        }

        public override void NPCLoot(NPC npc)
        {
            Player lastDamageDealerPlayer = lastDamageDealer as Player;
            if (lastDamageDealerPlayer != null)
            {
                lastDamageDealerPlayer.GetModPlayer<PoMPlayer>().OnKillNPC(npc);
            }

            NPC lastDamageDealerNPC = lastDamageDealer as NPC;

            if (npc.lifeMax > 5 && npc.value > 0f && !npc.SpawnedFromStatue)
            {
                if (npc.boss || Main.rand.NextFloat(0, 1) < 0.15f)
                {
                    int stack = Main.rand.Next(1, 5) * (npc.boss ? 10 : 1);
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ModifierFragment"), stack);
                }
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Wizard)
            {
                shop.item[nextSlot].SetDefaults(mod.ItemType<Items.ModifierFragment>());
                nextSlot++;
            }
        }



        public RarityNPC rarity;

        public List<Affix> affixes;
        public List<Prefix> prefixes;
        public List<Suffix> suffixes;

        public int FreeAffixes => rarity.maxAffixes - affixes.Count;
        public int FreePrefixes => Math.Min(FreeAffixes, rarity.maxPrefixes - prefixes.Count);
        public int FreeSuffixes => Math.Min(FreeAffixes, rarity.maxSuffixes - suffixes.Count);

        public PoMNPC()
        {
            rarity = ((PoMDataLoader.raritiesNPC?.Length ?? 0) == 0) ? new NPCNone() : PoMDataLoader.raritiesNPC[PoMDataLoader.rarityNPCMap[typeof(NPCNone)]];
            affixes = new List<Affix>();
            prefixes = new List<Prefix>();
            suffixes = new List<Suffix>();
        }

        public string GetBaseName(NPC npc) => Lang.GetNPCNameValue(npc.type);

        public void UpdateName(NPC npc)
        {
            if (rarity != null && rarity.GetType() != typeof(NPCNone))
            {
                StringBuilder sb = new StringBuilder();
                foreach (Prefix prefix in prefixes)
                {
                    sb.AppendFormat("{0} ", prefix.addedText);
                }
                sb.AppendFormat("{0} ", GetBaseName(npc));
                foreach (Suffix suffix in suffixes)
                {
                    sb.AppendFormat("{0} ", suffix.addedText);
                }
                sb.Remove(sb.Length - 1, 1);
                npc.GivenName = sb.ToString();
                npc.rarity = rarity.vanillaRarity;
            }
        }

        /// <summary>
        /// Completely rerolls rarity and affixes.
        /// </summary>
        public void RollNPC(NPC npc)
        {
            ClearAffixes(npc);
            rarity = PoMAffixController.RollRarity(npc);
            RollAffixes(npc);
            UpdateName(npc);
        }
        public void RollAffixes(NPC npc)
        {
            Affix newAffix;
            int freeAffixes = FreeAffixes;
            for (int i = 0; i < freeAffixes; i++)
            {
                if (i >= rarity.minAffixes && Main.rand.NextFloat(0, 1) > rarity.chanceToRollAffix)
                    break;

                newAffix = PoMAffixController.RollNewAffix(this, npc);
                if (newAffix == null)
                    break;

                AddAffix(newAffix, npc);
            }
        }

        public void AddAffix(Affix affix, NPC npc, bool clone = false)
        {
            affix.AddAffix(npc, clone);

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
        public void RemoveAffix(Affix affix, NPC npc)
        {
            affix.RemoveAffix(npc);
            affixes.Remove(affix);
            Prefix prefix = affix as Prefix;
            if (prefix != null)
                prefixes.Remove(prefix);
            else
            {
                Suffix suffix = affix as Suffix;
                if (suffix != null)
                    suffixes.Remove(suffix);
            }
        }
        public void ClearAffixes(NPC npc)
        {
            foreach (Affix affix in affixes)
            {
                affix.RemoveAffix(npc);
            }
            affixes.Clear();
            prefixes.Clear();
            suffixes.Clear();
        }
        public void ClearPrefixes(NPC npc)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.RemoveAffix(npc);
                affixes.Remove(prefix);
            }
            prefixes.Clear();
        }
        public void ClearSuffixes(NPC npc)
        {
            foreach (Suffix suffix in suffixes)
            {
                suffix.RemoveAffix(npc);
                affixes.Remove(suffix);
            }
            suffixes.Clear();
        }


        public override void SetDefaults(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (dontRollNextNPC)
                {
                    dontRollNextNPC = false;
                }
                else
                {
                    RollNPC(npc);
                    InitializeNPC(npc);
                    UpdateName(npc);

                    if (Main.netMode == NetmodeID.Server)
                        PoMNetMessage.cNPCSyncAffixes(npc, this);
                }
            }
        }
        public void InitializeNPC(NPC npc)
        {
            foreach (var prefix in prefixes)
            {
                prefix.InitializeNPC(this, npc);
            }
            foreach (var suffix in suffixes)
            {
                suffix.InitializeNPC(this, npc);
            }
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return base.CanBeHitByItem(npc, player, item);
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return base.CanBeHitByProjectile(npc, projectile);
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);
            }
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
            }
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitPlayer(npc, target, ref damage, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitPlayer(npc, target, ref damage, ref crit);
            }
        }
        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.ModifyHitNPC(npc, target, ref damage, ref knockback, ref crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.ModifyHitNPC(npc, target, ref damage, ref knockback, ref crit);
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitByItem(npc, player, item, damage, knockback, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitByItem(npc, player, item, damage, knockback, crit);
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            foreach (var prefix in prefixes)
            {
                prefix.OnHitByProjectile(npc, projectile, damage, knockback, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitByProjectile(npc, projectile, damage, knockback, crit);
            }
        }
        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            foreach(var prefix in prefixes)
            {
                prefix.OnHitPlayer(npc, target, damage, crit);
            }
            foreach (var suffix in suffixes)
            {
                suffix.OnHitPlayer(npc, target, damage, crit);
            }
        }

        public void NetSend(BinaryWriter writer)
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
                    affix.NetSend(writer);
                }
            }
            catch (Exception e)
            {
                mod.Logger.Error(e.ToString());
            }
        }
        public void NetReceive(BinaryReader reader, NPC npc)
        {
            try
            {
                rarity = PoMDataLoader.raritiesNPC[reader.ReadInt32()];

                int affixCount = reader.ReadByte();
                Affix affix;
                for (int i = 0; i < affixCount; i++)
                {
                    affix = PoMDataLoader.affixesNPC[reader.ReadInt32()].Clone();
                    affix.NetReceive(reader);
                    AddAffix(affix, npc);
                }
                UpdateName(npc);
            }
            catch (Exception e)
            {
                mod.Logger.Error(e.ToString());
            }
        }
    }
}