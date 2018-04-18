using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;
using Terraria.ID;
using PathOfModifiers.Buffs;
using PathOfModifiers.AffixesNPC;

namespace PathOfModifiers
{
    public class PoMNPC : GlobalNPC
    {
        public const float chanceToRollAffix = 0.5f;

        public override bool CloneNewInstances => false;
        public override bool InstancePerEntity => true;

        public List<Affix> affixes;
        public List<Prefix> prefixes;
        public List<Suffix> suffixes;

        public Entity lastDamageDealer;

        public bool hasBeenRolled = false;

        /// <summary>
        /// Stores the damage of the hit that procced the debuff.
        /// </summary>
        Dictionary<Type, int> damageDotDebuffDamages = new Dictionary<Type, int>();

        public bool dddDamageDotDebuff = false;

        public PoMNPC()
        {
            affixes = new List<Affix>();
            prefixes = new List<Prefix>();
            suffixes = new List<Suffix>();
        }

        public bool IsRollable(NPC npc)
        {
            return true;
        }

        public void UpdateName(NPC npc)
        {
            if (affixes.Count == 0)
                npc.GivenName = string.Empty;
            else
            {
                string addedPrefix = string.Empty;
                float addedPrefixWeight = 0f;
                foreach (Prefix prefix in prefixes)
                {
                    if (prefix.addedTextWeight > addedPrefixWeight && prefix.addedText != string.Empty)
                    {
                        addedPrefix = prefix.addedText;
                        addedPrefixWeight = prefix.addedTextWeight;
                    }
                }
                string addedSuffix = string.Empty;
                float addedSuffixWeight = 0f;
                foreach (Suffix suffix in suffixes)
                {
                    if (suffix.addedTextWeight > addedSuffixWeight && suffix.addedText != string.Empty)
                    {
                        addedSuffix = suffix.addedText;
                        addedPrefixWeight = suffix.addedTextWeight;
                    }
                }
                npc.GivenName = $"{addedPrefix}{(addedPrefix != string.Empty ? " " : string.Empty)}{npc.TypeName}{(addedSuffix != string.Empty ? " " : string.Empty)}{addedSuffix}";
                npc.rarity = affixes.Count;
            }
        }
        /// <summary>
        /// Roll NPC if it's rollable.
        /// </summary>
        public bool TryRollNPC(NPC npc)
        {
            hasBeenRolled = true;
            if (IsRollable(npc))
            {
                RollNPC(npc);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Completely rerolls affixes.
        /// </summary>
        /// <param name="npc"></param>
        public void RollNPC(NPC npc)
        {
            ClearAffixes(npc);
            RollAffixes(npc);
            UpdateName(npc);
        }
        /// <summary>
        /// Validly adds affixes to the npc.
        /// </summary>
        /// <param name="npc"></param>
        public void RollAffixes(NPC npc)
        {
            Affix newAffix;
            bool addNextAffix = Main.rand.NextFloat(0, 1) < chanceToRollAffix;
            while (addNextAffix)
            {
                newAffix = PoMAffixController.RollNewAffix(this, npc);
                if (newAffix == null)
                    break;

                AddAffix(newAffix, npc);

                addNextAffix = Main.rand.NextFloat(0, 1) < chanceToRollAffix;
            }
        }
        /// <summary>
        /// Validly adds an affix to the NPC.
        /// </summary>
        /// <param name="npc"></param>
        public bool AddRandomAffix(NPC npc)
        {
            Affix newAffix = PoMAffixController.RollNewAffix(this, npc);
            if (newAffix == null)
                return false;

            AddAffix(newAffix, npc);

            UpdateName(npc);
            return true;
        }
        /// <summary>
        /// Validly adds a prefix to the NPC.
        /// </summary>
        /// <param name="npc"></param>
        public bool AddRandomPrefix(NPC npc)
        {
            Prefix newPrefix = PoMAffixController.RollNewPrefix(this, npc);
            if (newPrefix == null)
                return false;

            AddAffix(newPrefix, npc);

            UpdateName(npc);
            return true;
        }
        /// <summary>
        /// Validly adds a suffix to the NPC.
        /// </summary>
        /// <param name="npc"></param>
        public bool AddRandomSuffix(NPC npc)
        {
            Affix newSuffix = PoMAffixController.RollNewSuffix(this, npc);
            if (newSuffix == null)
                return false;

            AddAffix(newSuffix, npc);

            UpdateName(npc);
            return true;
        }
        public void RemoveAll(NPC npc)
        {
            ClearAffixes(npc);

            UpdateName(npc);
        }
        public void RemovePrefixes(NPC npc)
        {
            ClearPrefixes(npc);
            UpdateName(npc);
        }
        public void RemoveSuffixes(NPC npc)
        {
            ClearSuffixes(npc);
            UpdateName(npc);
        }
        public void RollAffixTierMultipliers(NPC npc)
        {
            foreach (Affix affix in affixes)
            {
                affix.RollValue(false);
            }
            UpdateName(npc);
        }
        public void RollPrefixTierMultipliers(NPC npc)
        {
            foreach (Prefix prefix in prefixes)
            {
                prefix.RollValue(false);
            }
            UpdateName(npc);
        }
        public void RollSuffixTierMultipliers(NPC npc)
        {
            foreach (Suffix suffix in suffixes)
            {
                suffix.RollValue(false);
            }
            UpdateName(npc);
        }

        #region Affix actions
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
        #endregion

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
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)MsgType.AddDamageDoTDebuffNPC);
                packet.Write(npc.whoAmI);
                packet.Write(buff.Type);
                packet.Write(damage);
                packet.Write(time);
                packet.Send();
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (!hasBeenRolled)
                TryRollNPC(npc);

            return true;
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
    }
}