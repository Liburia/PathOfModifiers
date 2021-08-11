using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using PathOfModifiers.UI;
using Terraria.ModLoader.IO;
using Terraria.ID;
using System.IO;
using System.Collections.Generic;
using Terraria.Utilities;
using PathOfModifiers.UI.Elements;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;

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
        public virtual int MaxTier { get; }
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
            public T value;
            public double weight;

            public WeightedTier(T tier, double weight)
            {
                this.value = tier;
                this.weight = weight;
            }
        }

        public override int MaxTier => Tiers.Length - 1;
        public override int TierText => MaxTier - Tier + 1;

        public WeightedTier[] Tiers { get; set; }

        public override int RollTier()
        {
            Tuple<int, double>[] tierWeights = new Tuple<int, double>[MaxTier];
            for (int i = 0; i < MaxTier; i++)
            {
                tierWeights[i] = new Tuple<int, double>(i, Tiers[i].weight);
            }

            int tier = new WeightedRandom<int>(Main.rand, tierWeights);
            return tier;
        }

        public virtual T GetValue(int? tier = null)
        {
            return Tiers[tier ?? Tier].value;
        }
    }
    public class TTFloat : TierType<float>, IUIDrawable
    {
        public override float TierMultiplier { get; protected set; }
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
        public float GetValueFormat(float multiplyBy = 100, bool round = true, float decimalsIfSmallerThan = 10f, int decimalPlaces = 2)
        {
            float value = Math.Abs(GetValue() * multiplyBy);

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

        public override void UpdateValue()
        {
            var value = base.GetValue();
            Value = value + ((base.GetValue(Tier + 1) - value) * TierMultiplier);
        }

        UIElement IUIDrawable.CreateUI(UIElement parent, Action onChangeCallback)
        {
            UIElement el = new();
            el.Width.Set(0f, 1f);
            parent.Append(el);

            var tierRange = new UIIntRange(MaxTier - 1, Tier);
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
        /// <summary>
        /// Whether 0 is skipped for IsRange
        /// </summary>
        public virtual bool CanBeZero { get; set; }

        public override float TierMultiplier { get; protected set; }
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
        public int GetValueFormat()
        {
            int value = Math.Abs(GetValue());

            return value;
        }

        public override void UpdateValue()
        {
            int currentValue = base.GetValue();
            int nextValue = base.GetValue(Tier + 1);
            float value = nextValue - currentValue;

            bool floor = currentValue < nextValue;
            bool zeroInrange = false;
            if (!CanBeZero)
            {
                if (floor)
                {
                    if (currentValue < 0 && nextValue > 0)
                    {
                        zeroInrange = true;
                        value -= 1;
                    }
                }
                else
                {
                    if (currentValue > 0 && nextValue < 0)
                    {
                        zeroInrange = true;
                        value += 1;
                    }
                }
            }

            value = currentValue + (value * TierMultiplier);

            if (floor)
            {
                if (zeroInrange && !CanBeZero && value >= 0)
                {
                    value += 1;
                }
                Value = (int)Math.Floor(value);
            }
            else
            {
                if (zeroInrange && !CanBeZero && value <= 0)
                {
                    value -= 1;
                }
                Value = (int)Math.Ceiling(value);
            }
        }

        UIElement IUIDrawable.CreateUI(UIElement parent, Action onChangeCallback)
        {
            UIElement el = new();
            el.Width.Set(0f, 1f);
            parent.Append(el);

            var tierRange = new UIIntRange(MaxTier - 1, Tier);
            tierRange.Width.Set(0f, 1f);

            UIElement.ElementEvent setTier = delegate (UIElement el)
            {
                SetTier(tierRange.CurrentValue);
                onChangeCallback?.Invoke();
            };
            tierRange.slider.OnSliderInput += setTier;
            tierRange.OnInputFocusedChanged += setTier;

            var valueRange = new UIIntRange(Tiers[Tier + 1].value - Tiers[Tier].value, Value - Tiers[Tier].value);
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
