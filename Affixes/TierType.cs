using PathOfModifiers.UI;
using PathOfModifiers.UI.Elements;
using System;
using Terraria;
using Terraria.UI;
using Terraria.Utilities;

namespace PathOfModifiers.Affixes
{
    public class TierType
    {
        /// <summary>
        /// Whether the positive effect scales outwards from the middle or from the 0th tier
        /// </summary>
        public virtual bool TwoWay { get; set; }
        /// <summary>
        /// Whether the value can be anything inbetween the 2 tiers or clamped to one of them
        /// </summary>
        public virtual bool IsRange { get; set; }
        public virtual int MaxTiers { get; }
        public virtual int TierText { get; }
        public virtual float TierMultiplier { get; protected set; }

        public int Tier { get; private set; }

        public void SetTier(int tier)
        {
            Tier = tier;
            UpdateValue();
        }

        /// <summary>
        /// Returns a random tier based on assigned weights
        /// </summary>
        /// <returns></returns>
        public virtual int RollTier() { return 0; }
        public virtual void SetTierMultiplier(float multiplier)
        {
            TierMultiplier = multiplier;
            UpdateValue();
        }
        public virtual void UpdateValue() { }
    }
    public class TierType<T> : TierType
    {
        public class WeightedTier
        {
            public T min;
            public T max;
            public double weight;

            public WeightedTier(T min, T max, double weight)
            {
                this.min = min;
                this.max = max;
                this.weight = weight;
            }
        }

        public WeightedTier[] Tiers { get; set; }

        public override int MaxTiers => Tiers.Length;
        public override int TierText => MaxTiers - Tier;

        public override int RollTier()
        {
            Tuple<int, double>[] tierWeights = new Tuple<int, double>[MaxTiers];
            for (int i = 0; i < MaxTiers; i++)
            {
                tierWeights[i] = new Tuple<int, double>(i, Tiers[i].weight);
            }

            int tier = new WeightedRandom<int>(Main.rand, tierWeights);
            return tier;
        }

        public virtual T GetValue(int? tier = null)
        {
            return Tiers[tier ?? Tier].min;
        }
    }
    public class TTFloat : TierType<float>, IUIDrawable
    {
        float Value { get; set; }

        public override float GetValue(int? tier = null)
        {
            if (IsRange)
            {
                return Value;
            }
            else
            {
                return base.GetValue(tier);
            }
        }
        public float FormatValue(float valueToFormat, float multiplyBy = 100, bool round = true, float decimalsIfSmallerThan = 10f, int decimalPlaces = 2)
        {
            float value = Math.Abs(valueToFormat * multiplyBy);

            if (round)
            {
                int decimals = 0;
                if (value < decimalsIfSmallerThan)
                {
                    decimals = decimalPlaces;
                }

                value = (float)Math.Round(value, decimals);
            }

            return value;
        }
        public float GetCurrentValueFormat(float multiplyBy = 100, bool round = true, float decimalsIfSmallerThan = 10f, int decimalPlaces = 2) => FormatValue(GetValue(), multiplyBy, round, decimalsIfSmallerThan, decimalPlaces);
        public float GetMinValueFormat(float multiplyBy = 100, bool round = true, float decimalsIfSmallerThan = 10f, int decimalPlaces = 2) => FormatValue(Tiers[Tier].min, multiplyBy, round, decimalsIfSmallerThan, decimalPlaces);
        public float GetMaxValueFormat(float multiplyBy = 100, bool round = true, float decimalsIfSmallerThan = 10f, int decimalPlaces = 2) => FormatValue(Tiers[Tier].max, multiplyBy, round, decimalsIfSmallerThan, decimalPlaces);

        public override void UpdateValue()
        {
            var wt = Tiers[Tier];
            Value = wt.min + ((wt.max - wt.min) * TierMultiplier);
        }

        //TODO: Test that these work after changing to min/max weighted tiers
        UIElement IUIDrawable.CreateUI(UIElement parent, Action onChangeCallback)
        {
            UIElement el = new();
            el.Width.Set(0f, 1f);
            parent.Append(el);

            var tierRange = new UIIntRange(MaxTiers - 1, Tier);
            tierRange.Width.Set(0f, 1f);
            el.Append(tierRange);

            UIElement.ElementEvent setTier = delegate (UIElement el)
            {
                SetTier(tierRange.CurrentValue);
                onChangeCallback?.Invoke();
            };
            tierRange.slider.OnSliderInput += setTier;
            tierRange.OnInputFocusedChanged += setTier;

            var valueRange = new UIFloatRange(0f, 1f, TierMultiplier);
            valueRange.Top.Set(tierRange.Top.Pixels + tierRange.GetDimensions().Height + UICommon.spacing, 0f);
            valueRange.Width.Set(0f, 1f);
            el.Append(valueRange);

            UIElement.ElementEvent setTierMultiplier = delegate (UIElement el)
            {
                SetTierMultiplier(valueRange.CurrentValue);
                onChangeCallback?.Invoke();
            };
            valueRange.slider.OnSliderInput += setTierMultiplier;
            valueRange.OnInputFocusedChanged += setTierMultiplier;

            el.MinHeight.Set(valueRange.Top.Pixels + valueRange.GetDimensions().Height, 0f);
            el.Recalculate();

            return el;
        }
    }
    /// <summary>
    /// When IsRange, the value is (inclusive, exclusive)
    /// </summary>
    public class TTInt : TierType<int>, IUIDrawable
    {
        int Value { get; set; }

        public override int GetValue(int? tier = null)
        {
            if (IsRange)
            {
                return Value;
            }
            else
            {
                return base.GetValue(tier);
            }
        }
        public int FormatValue(int valueToFormat)
        {
            return Math.Abs(valueToFormat);
        }
        public int GetCurrentValueFormat() => FormatValue(GetValue());
        public int GetMinValueFormat() => FormatValue(Tiers[Tier].min);
        public int GetMaxValueFormat() => FormatValue(Tiers[Tier].max);

        public override void UpdateValue()
        {
            var wt = Tiers[Tier];

            var value = wt.min + ((wt.max - wt.min) * TierMultiplier);

            if (wt.min < wt.max)
            {
                Value = (int)Math.Floor(value);
            }
            else
            {
                Value = (int)Math.Ceiling(value);
            }
        }

        UIElement IUIDrawable.CreateUI(UIElement parent, Action onChangeCallback)
        {
            UIElement el = new();
            el.Width.Set(0f, 1f);
            parent.Append(el);

            var tierRange = new UIIntRange(MaxTiers - 1, Tier);
            tierRange.Width.Set(0f, 1f);
            el.Append(tierRange);

            UIElement.ElementEvent setTier = delegate (UIElement el)
            {
                SetTier(tierRange.CurrentValue);
                onChangeCallback?.Invoke();
            };
            tierRange.slider.OnSliderInput += setTier;
            tierRange.OnInputFocusedChanged += setTier;

            var wt = Tiers[Tier];
            var valueRange = new UIFloatRange(0f, 1f, TierMultiplier);
            valueRange.Top.Set(tierRange.Top.Pixels + tierRange.GetDimensions().Height + UICommon.spacing, 0f);
            valueRange.Width.Set(0f, 1f);
            el.Append(valueRange);

            UIElement.ElementEvent setTierMultiplier = delegate (UIElement el)
            {
                SetTierMultiplier(valueRange.CurrentValue);
                onChangeCallback?.Invoke();
            };
            valueRange.slider.OnSliderInput += setTierMultiplier;
            valueRange.OnInputFocusedChanged += setTierMultiplier;


            el.MinHeight.Set(valueRange.Top.Pixels + valueRange.GetDimensions().Height, 0f);
            el.Recalculate();

            return el;
        }
    }
}
