using PathOfModifiers.UI;
using PathOfModifiers.UI.Elements;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace PathOfModifiers.Affixes.Items
{
    [DisableAffix]
    public abstract class AffixTiered : Affix
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

        public virtual int CompoundTier => 0;
        public virtual int MaxCompoundTier => 0;

        public abstract void SetCompoundTier(int tier);
        public abstract void ImproveCompoundTier(int by = 1);
        public abstract void RollValue(bool rollTier = true);
        public abstract void ImproveValue();
    }

    [DisableAffix]
    public class AffixTiered<T> : AffixTiered, IUIDrawable
        where T : TierType, IUIDrawable
    {
        public virtual T Type1 { get; }

        public override int CompoundTier
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
        public override int MaxCompoundTier => Type1.MaxTiers;
        public int CompoundTierText => MaxCompoundTier - CompoundTier;

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
        public override void SetCompoundTier(int tier)
        {
            SetTier(0);
            ImproveCompoundTier(tier);
        }
        public override void ImproveCompoundTier(int by = 1)
        {
            int canImproveBy1 = Type1.MaxTiers - Type1.Tier - 1;
            Type1.SetTier(Type1.Tier + Math.Min(by, canImproveBy1));
        }
        public override void RollValue(bool rollTier)
        {
            if (rollTier)
                RollTier();
            RollTierMultiplier();
        }
        public override void ImproveValue()
        {
            float range1 = 1f - Type1.TierMultiplier;
            float added1 = Main.rand.NextFloat(0f, range1);
            Type1.SetTierMultiplier(Type1.TierMultiplier + added1);
        }
        public override void ReforgePrice(Item item, ref int price)
        {
            price += (int)Math.Round(item.value * 0.2f * CompoundTier / 4 / Weight);
        }
        public override void Save(TagCompound tag, Item item)
        {
            base.Save(tag, item);
            tag.Set("tier1", Type1.Tier);
            tag.Set("tierMultiplier1", Type1.TierMultiplier);
        }
        public override void Load(TagCompound tag, Item item)
        {
            base.Load(tag, item);
            SetTier(tag.GetInt("tier1"));
            SetTierMultiplier(tag.Get<float>("tierMultiplier1"));
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            base.NetSend(item, writer);
            writer.Write(Type1.Tier);
            writer.Write(Type1.TierMultiplier);
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            base.NetReceive(item, reader);
            int t1 = reader.ReadInt32();
            float tm1 = reader.ReadSingle();
            SetTier(t1);
            SetTierMultiplier(tm1);
        }

        public override string GetForgeText()
        {
            return $"      { UI.Chat.TierTagHandler.GenerateTag(Type1.TierText, Type1.MaxTiers) } { GetAffixText(true) }";
        }

        UIElement IUIDrawable.CreateUI(UIElement parent, Action onChangeCallback)
        {
            UIElement affixUI = new();
            affixUI.Width.Set(0f, 1f);
            parent.Append(affixUI);

            var affixText = new UIText(GetAffixText(), UICommon.textMedium);
            affixText.TextColor = Color;
            affixUI.Append(affixText);

            Action onChange = delegate ()
            {
                affixText.SetText(GetAffixText());
                onChangeCallback?.Invoke();
            };

            var text = new UIText("Type 1", UICommon.textMedium);
            text.Top.Pixels = affixText.Top.Pixels + affixText.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(text);

            var ui1 = Type1.CreateUI(affixUI, onChange);
            ui1.Top.Pixels = text.Top.Pixels + text.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(ui1);

            affixUI.Height.Set(ui1.Top.Pixels + ui1.GetDimensions().Height, 0f);
            affixUI.Recalculate();

            return affixUI;
        }
    }
    [DisableAffix]
    public class AffixTiered<T, U> : AffixTiered, IUIDrawable
        where T : TierType, IUIDrawable
        where U : TierType, IUIDrawable
    {
        public virtual T Type1 { get; }
        public virtual U Type2 { get; }

        public override int CompoundTier
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
        public override int MaxCompoundTier => (Type1.MaxTiers + Type2.MaxTiers) / 2;
        public int CompoundTierText => MaxCompoundTier - CompoundTier;

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
        public override void SetCompoundTier(int tier)
        {
            SetTier(0, 0);
            ImproveCompoundTier(tier);
        }
        public override void ImproveCompoundTier(int by = 1)
        {
            int canImproveBy1 = Type1.MaxTiers - Type1.Tier - 1;
            Type1.SetTier(Type1.Tier + Math.Min(by, canImproveBy1));

            int canImproveBy2 = Type2.MaxTiers - Type2.Tier - 1;
            Type2.SetTier(Type2.Tier + Math.Min(by, canImproveBy2));
        }
        public override void RollValue(bool rollTier)
        {
            if (rollTier)
                RollTier();
            RollTierMultiplier();
        }
        public override void ImproveValue()
        {
            float range1 = 1f - Type1.TierMultiplier;
            float added1 = Main.rand.NextFloat(0f, range1);
            Type1.SetTierMultiplier(Type1.TierMultiplier + added1);

            float range2 = 1f - Type2.TierMultiplier;
            float added2 = Main.rand.NextFloat(0f, range2);
            Type2.SetTierMultiplier(Type2.TierMultiplier + added2);
        }
        public override void ReforgePrice(Item item, ref int price)
        {
            price += (int)Math.Round(item.value * 0.2f * CompoundTier / 4 / Weight);
        }
        public override void Save(TagCompound tag, Item item)
        {
            base.Save(tag, item);
            tag.Set("tier1", Type1.Tier);
            tag.Set("tierMultiplier1", Type1.TierMultiplier);
            tag.Set("tier2", Type2.Tier);
            tag.Set("tierMultiplier2", Type2.TierMultiplier);
        }
        public override void Load(TagCompound tag, Item item)
        {
            base.Load(tag, item);
            SetTier(tag.GetInt("tier1"), tag.GetInt("tier2"));
            SetTierMultiplier(tag.Get<float>("tierMultiplier1"), tag.Get<float>("tierMultiplier2"));
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            base.NetSend(item, writer);
            writer.Write(Type1.Tier);
            writer.Write(Type1.TierMultiplier);
            writer.Write(Type2.Tier);
            writer.Write(Type2.TierMultiplier);
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            base.NetReceive(item, reader);
            int t1 = reader.ReadInt32();
            float tm1 = reader.ReadSingle();
            int t2 = reader.ReadInt32();
            float tm2 = reader.ReadSingle();
            SetTier(t1, t2);
            SetTierMultiplier(tm1, tm2);
        }

        public override string GetForgeText()
        {
            return $"   { UI.Chat.TierTagHandler.GenerateTag(Type1.TierText, Type1.MaxTiers) }{ UI.Chat.TierTagHandler.GenerateTag(Type2.TierText, Type2.MaxTiers) } { GetAffixText(true) }";
        }

        UIElement IUIDrawable.CreateUI(UIElement parent, Action onChangeCallback)
        {
            UIElement affixUI = new();
            affixUI.Width.Set(0f, 1f);
            parent.Append(affixUI);

            var affixText = new UIText(GetAffixText(), UICommon.textMedium);
            affixText.TextColor = Color;
            affixUI.Append(affixText);

            Action onChange = delegate ()
            {
                affixText.SetText(GetAffixText());
                onChangeCallback?.Invoke();
            };

            var text = new UIText("Type 1", UICommon.textMedium);
            text.Top.Pixels = affixText.Top.Pixels + affixText.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(text);

            var ui1 = Type1.CreateUI(affixUI, onChange);
            ui1.Top.Pixels = text.Top.Pixels + text.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(ui1);

            var ui2 = Type2.CreateUI(affixUI, onChange);
            ui2.Top.Pixels = ui1.Top.Pixels + ui1.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(ui2);

            affixUI.Height.Set(ui2.Top.Pixels + ui2.GetDimensions().Height, 0f);
            affixUI.Recalculate();

            return affixUI;
        }
    }
    [DisableAffix]
    public class AffixTiered<T, U, O> : AffixTiered, IUIDrawable
        where T : TierType, IUIDrawable
        where U : TierType, IUIDrawable
        where O : TierType, IUIDrawable
    {
        public virtual T Type1 { get; }
        public virtual U Type2 { get; }
        public virtual O Type3 { get; }

        public override int CompoundTier
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
        public override int MaxCompoundTier => (Type1.MaxTiers + Type2.MaxTiers + Type3.MaxTiers) / 3;
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
        public override void SetCompoundTier(int tier)
        {
            SetTier(0, 0, 0);
            ImproveCompoundTier(tier);
        }
        public override void ImproveCompoundTier(int by = 1)
        {
            int canImproveBy1 = Type1.MaxTiers - Type1.Tier - 1;
            Type1.SetTier(Type1.Tier + Math.Min(by, canImproveBy1));

            int canImproveBy2 = Type2.MaxTiers - Type2.Tier - 1;
            Type2.SetTier(Type2.Tier + Math.Min(by, canImproveBy2));

            int canImproveBy3 = Type3.MaxTiers - Type3.Tier - 1;
            Type3.SetTier(Type3.Tier + Math.Min(by, canImproveBy3));
        }
        public override void RollValue(bool rollTier)
        {
            if (rollTier)
                RollTier();
            RollTierMultiplier();
        }
        public override void ImproveValue()
        {
            float range1 = 1f - Type1.TierMultiplier;
            float added1 = Main.rand.NextFloat(0f, range1);
            Type1.SetTierMultiplier(Type1.TierMultiplier + added1);

            float range2 = 1f - Type2.TierMultiplier;
            float added2 = Main.rand.NextFloat(0f, range2);
            Type2.SetTierMultiplier(Type2.TierMultiplier + added2);

            float range3 = 1f - Type3.TierMultiplier;
            float added3 = Main.rand.NextFloat(0f, range3);
            Type3.SetTierMultiplier(Type3.TierMultiplier + added3);
        }
        public override void ReforgePrice(Item item, ref int price)
        {
            price += (int)Math.Round(item.value * 0.2f * CompoundTier / 4 / Weight);
        }
        public override void Save(TagCompound tag, Item item)
        {
            base.Save(tag, item);
            tag.Set("tier1", Type1.Tier);
            tag.Set("tierMultiplier1", Type1.TierMultiplier);
            tag.Set("tier2", Type2.Tier);
            tag.Set("tierMultiplier2", Type2.TierMultiplier);
            tag.Set("tier3", Type3.Tier);
            tag.Set("tierMultiplier3", Type3.TierMultiplier);
        }
        public override void Load(TagCompound tag, Item item)
        {
            base.Load(tag, item);
            SetTier(tag.GetInt("tier1"), tag.GetInt("tier2"), tag.GetInt("tier3"));
            SetTierMultiplier(tag.Get<float>("tierMultiplier1"), tag.Get<float>("tierMultiplier2"), tag.Get<float>("tierMultiplier3"));
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            base.NetSend(item, writer);
            writer.Write(Type1.Tier);
            writer.Write(Type1.TierMultiplier);
            writer.Write(Type2.Tier);
            writer.Write(Type2.TierMultiplier);
            writer.Write(Type3.Tier);
            writer.Write(Type3.TierMultiplier);
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            base.NetReceive(item, reader);
            int t1 = reader.ReadInt32();
            float tm1 = reader.ReadSingle();
            int t2 = reader.ReadInt32();
            float tm2 = reader.ReadSingle();
            int t3 = reader.ReadInt32();
            float tm3 = reader.ReadSingle();
            SetTier(t1, t2, t3);
            SetTierMultiplier(tm1, tm2, tm3);
        }

        public override string GetForgeText()
        {
            return $"{ UI.Chat.TierTagHandler.GenerateTag(Type1.TierText, Type1.MaxTiers) }{ UI.Chat.TierTagHandler.GenerateTag(Type2.TierText, Type2.MaxTiers) }{ UI.Chat.TierTagHandler.GenerateTag(Type3.TierText, Type3.MaxTiers) } { GetAffixText(true) }";
        }

        UIElement IUIDrawable.CreateUI(UIElement parent, Action onChangeCallback)
        {
            UIElement affixUI = new();
            affixUI.Width.Set(0f, 1f);
            parent.Append(affixUI);

            var affixText = new UIText(GetAffixText(), UICommon.textMedium);
            affixText.TextColor = Color;
            affixUI.Append(affixText);

            Action onChange = delegate ()
            {
                affixText.SetText(GetAffixText());
                onChangeCallback?.Invoke();
            };

            var text = new UIText("Type 1", UICommon.textMedium);
            text.Top.Pixels = affixText.Top.Pixels + affixText.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(text);

            var ui1 = Type1.CreateUI(affixUI, onChange);
            ui1.Top.Pixels = text.Top.Pixels + text.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(ui1);

            var text2 = new UIText("Type 2", UICommon.textMedium);
            text2.Top.Pixels = ui1.Top.Pixels + ui1.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(text2);

            var ui2 = Type2.CreateUI(affixUI, onChange);
            ui2.Top.Set(text2.Top.Pixels + text2.GetDimensions().Height + UICommon.spacing, 0f);
            affixUI.Append(ui2);

            var text3 = new UIText("Type 3", UICommon.textMedium);
            text3.Top.Pixels = ui2.Top.Pixels + ui2.GetDimensions().Height + UICommon.spacing;
            affixUI.Append(text3);

            var ui3 = Type3.CreateUI(affixUI, onChange);
            ui3.Top.Set(text3.Top.Pixels + text3.GetDimensions().Height + UICommon.spacing, 0f);
            affixUI.Append(ui3);

            affixUI.Height.Set(ui3.Top.Pixels + ui3.GetDimensions().Height, 0f);

            return affixUI;
        }
    }
}
