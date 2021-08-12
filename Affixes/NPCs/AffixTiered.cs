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
using System.Security.Policy;
using Terraria.Utilities;

namespace PathOfModifiers.Affixes.NPCs
{
    [DisableAffix]
    public class AffixTiered : Affix
    {
        public class WeightedTierName
        {
            public string name;
            public double weight;

            public WeightedTierName(string name, double weight)
            {
                this.name = name;
                this.weight = weight;
            }
        }
        public virtual WeightedTierName[] TierNames { get; }

        public string AddedTextTiered { get; set; }
        public double AddedTextWeightTiered { get; set; }

        public override string AddedText => AddedTextTiered;
        public override double AddedTextWeight => AddedTextWeightTiered;
    }

    [DisableAffix]
    public class AffixTiered<T> : AffixTiered
        where T : TierType
    {
        public virtual T Type1 { get; }

        public int CompoundTier
        {
            get
            {
                float t1 = Type1.Tier;

                if (Type1.TwoWay)
                {
                    t1 = Math.Abs(t1 - (Type1.MaxTiers / 2f)) * 2;
                }

                return (int)t1;
            }
        }
        public int MaxCompoundTier => Type1.MaxTiers;
        public int CompoundTierText => MaxCompoundTier - CompoundTier + 1;

        public void SetTier(int t1, bool ignore1 = false)
        {
            if (!ignore1)
            {
                Type1.SetTier(t1);
            }

            var tierName = TierNames[CompoundTier];
            AddedTextTiered = tierName.name;
            AddedTextWeightTiered = tierName.weight;
        }
        public void SetTierMultiplier(float m1, bool ignore1 = false)
        {
            if (!ignore1)
            {
                Type1.SetTierMultiplier(m1);
            }
        }
        public void RollTier(bool ignore1 = false)
        {
            SetTier(Type1.RollTier(), ignore1);
        }
        public void RollTierMultiplier(bool ignore1 = false)
        {
            SetTierMultiplier(Main.rand.NextFloat(0, 1), ignore1);
        }
        public override Affix Clone()
        {
            AffixTiered<T> newAffix = (AffixTiered<T>)base.Clone();
            newAffix.SetTier(Type1.Tier);
            newAffix.SetTierMultiplier(Type1.TierMultiplier);
            return newAffix;
        }
        public override void RollValue(bool rollTier)
        {
            if (rollTier)
                RollTier();
            RollTierMultiplier();
        }
        public override void Save(TagCompound tag)
        {
            base.Save(tag);
            tag.Set("tier1", Type1.Tier);
            tag.Set("tierMultiplier1", Type1.TierMultiplier);
        }
        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            SetTier(tag.GetInt("tier1"));
            SetTierMultiplier(tag.Get<float>("tierMultiplier1"));
        }
        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(Type1.Tier);
            writer.Write(Type1.TierMultiplier);
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int t1 = reader.ReadInt32();
            float tm1 = reader.ReadSingle();
            SetTier(t1);
            SetTierMultiplier(tm1);
        }
    }
    [DisableAffix]
    public class AffixTiered<T, U> : AffixTiered
        where T : TierType
        where U : TierType
    {
        public virtual T Type1 { get; }
        public virtual U Type2 { get; }

        public int CompoundTier
        {
            get
            {
                float t1 = Type1.Tier;
                float t2 = Type2.Tier;

                if (Type1.TwoWay)
                {
                    t1 = Math.Abs(t1 - (Type1.MaxTiers / 2f)) * 2;
                }
                if (Type2.TwoWay)
                {
                    t2 = Math.Abs(t2 - (Type2.MaxTiers / 2f)) * 2;
                }

                return (int)(t1 + t2) / 2;
            }
        }
        public int MaxCompoundTier => (Type1.MaxTiers + Type2.MaxTiers) / 2;
        public int CompoundTierText => MaxCompoundTier - CompoundTier + 1;

        public void SetTier(int t1, int t2, bool ignore1 = false, bool ignore2 = false)
        {
            if (!ignore1)
            {
                Type1.SetTier(t1);
            }
            if (!ignore2)
            {
                Type2.SetTier(t2);
            }

            var tierName = TierNames[CompoundTier];
            AddedTextTiered = tierName.name;
            AddedTextWeightTiered = tierName.weight;
        }
        public void SetTierMultiplier(float m1, float m2, bool ignore1 = false, bool ignore2 = false)
        {
            if (!ignore1)
            {
                Type1.SetTierMultiplier(m1);
            }
            if (!ignore2)
            {
                Type2.SetTierMultiplier(m2);
            }
        }
        public void RollTier(bool ignore1 = false, bool ignore2 = false)
        {
            SetTier(Type1.RollTier(), Type2.RollTier(), ignore1, ignore2);
        }
        public void RollTierMultiplier(bool ignore1 = false, bool ignore2 = false)
        {
            SetTierMultiplier(Main.rand.NextFloat(0, 1), Main.rand.NextFloat(0, 1), ignore1, ignore2);
        }
        public override Affix Clone()
        {
            AffixTiered<T, U> newAffix = (AffixTiered<T, U>)base.Clone();
            newAffix.SetTier(Type1.Tier, Type2.Tier);
            newAffix.SetTierMultiplier(Type1.TierMultiplier, Type2.TierMultiplier);
            return newAffix;
        }
        public override void RollValue(bool rollTier)
        {
            if (rollTier)
                RollTier();
            RollTierMultiplier();
        }
        public override void Save(TagCompound tag)
        {
            base.Save(tag);
            tag.Set("tier1", Type1.Tier);
            tag.Set("tierMultiplier1", Type1.TierMultiplier);
            tag.Set("tier2", Type2.Tier);
            tag.Set("tierMultiplier2", Type2.TierMultiplier);
        }
        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            SetTier(tag.GetInt("tier1"), tag.GetInt("tier2"));
            SetTierMultiplier(tag.Get<float>("tierMultiplier1"), tag.Get<float>("tierMultiplier2"));
        }
        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(Type1.Tier);
            writer.Write(Type1.TierMultiplier);
            writer.Write(Type2.Tier);
            writer.Write(Type2.TierMultiplier);
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int t1 = reader.ReadInt32();
            float tm1 = reader.ReadSingle();
            int t2 = reader.ReadInt32();
            float tm2 = reader.ReadSingle();
            SetTier(t1, t2);
            SetTierMultiplier(tm1, tm2);
        }
    }
    [DisableAffix]
    public class AffixTiered<T, U, O> : AffixTiered
        where T : TierType
        where U : TierType
        where O : TierType
    {
        public virtual T Type1 { get; }
        public virtual U Type2 { get; }
        public virtual O Type3 { get; }

        public int CompoundTier
        {
            get
            {
                float t1 = Type1.Tier;
                float t2 = Type2.Tier;
                float t3 = Type3.Tier;

                if (Type1.TwoWay)
                {
                    t1 = Math.Abs(t1 - (Type1.MaxTiers / 2f)) * 2;
                }
                if (Type2.TwoWay)
                {
                    t2 = Math.Abs(t2 - (Type2.MaxTiers / 2f)) * 2;
                }
                if (Type3.TwoWay)
                {
                    t3 = Math.Abs(t3 - (Type3.MaxTiers / 2f)) * 2;
                }

                return (int)(t1 + t2 + t3) / 3;
            }
        }
        public int MaxCompoundTier => (Type1.MaxTiers + Type2.MaxTiers + Type3.MaxTiers) / 3;
        public int CompoundTierText => MaxCompoundTier - CompoundTier + 1;

        public void SetTier(int t1, int t2, int t3, bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            if (!ignore1)
            {
                Type1.SetTier(t1);
            }
            if (!ignore2)
            {
                Type2.SetTier(t2);
            }
            if (!ignore3)
            {
                Type3.SetTier(t3);
            }

            var tierName = TierNames[CompoundTier];
            AddedTextTiered = tierName.name;
            AddedTextWeightTiered = tierName.weight;
        }
        public void SetTierMultiplier(float m1, float m2, float m3, bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            if (!ignore1)
            {
                Type1.SetTierMultiplier(m1);
            }
            if (!ignore2)
            {
                Type2.SetTierMultiplier(m2);
            }
            if (!ignore3)
            {
                Type3.SetTierMultiplier(m3);
            }
        }
        public void RollTier(bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            SetTier(Type1.RollTier(), Type2.RollTier(), Type3.RollTier(), ignore1, ignore2, ignore3);
        }
        public void RollTierMultiplier(bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            SetTierMultiplier(Main.rand.NextFloat(0, 1), Main.rand.NextFloat(0, 1), Main.rand.NextFloat(0, 1), ignore1, ignore2, ignore3);
        }
        public override Affix Clone()
        {
            AffixTiered<T, U, O> newAffix = (AffixTiered<T, U, O>)base.Clone();
            newAffix.SetTier(Type1.Tier, Type2.Tier, Type3.Tier);
            newAffix.SetTierMultiplier(Type1.TierMultiplier, Type2.TierMultiplier, Type3.TierMultiplier);
            return newAffix;
        }
        public override void RollValue(bool rollTier)
        {
            if (rollTier)
                RollTier();
            RollTierMultiplier();
        }
        public override void Save(TagCompound tag)
        {
            base.Save(tag);
            tag.Set("tier1", Type1.Tier);
            tag.Set("tierMultiplier1", Type1.TierMultiplier);
            tag.Set("tier2", Type2.Tier);
            tag.Set("tierMultiplier2", Type2.TierMultiplier);
            tag.Set("tier3", Type3.Tier);
            tag.Set("tierMultiplier3", Type3.TierMultiplier);
        }
        public override void Load(TagCompound tag)
        {
            base.Load(tag);
            SetTier(tag.GetInt("tier1"), tag.GetInt("tier2"), tag.GetInt("tier3"));
            SetTierMultiplier(tag.Get<float>("tierMultiplier1"), tag.Get<float>("tierMultiplier2"), tag.Get<float>("tierMultiplier3"));
        }
        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(Type1.Tier);
            writer.Write(Type1.TierMultiplier);
            writer.Write(Type2.Tier);
            writer.Write(Type2.TierMultiplier);
            writer.Write(Type3.Tier);
            writer.Write(Type3.TierMultiplier);
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int t1 = reader.ReadInt32();
            float tm1 = reader.ReadSingle();
            int t2 = reader.ReadInt32();
            float tm2 = reader.ReadSingle();
            int t3 = reader.ReadInt32();
            float tm3 = reader.ReadSingle();
            SetTier(t1, t2, t3);
            SetTierMultiplier(tm1, tm2, tm3);
        }
    }
}
