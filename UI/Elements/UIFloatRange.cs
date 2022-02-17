using Microsoft.Xna.Framework;
using System;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
    internal class UIFloatRange : UIRange
    {
        public float CurrentValue
        {
            get => slider.SliderPosition;
            set
            {
                value = MathHelper.Clamp(value, 0f, 1f);
                slider.SliderPosition = value;
            }
        }
        public string CurrentString => MathF.Round(min + ((max - min) * CurrentValue), 2).ToString();

        float min;
        float max;

        public UIFloatRange(float minLabel = 0f, float maxLabel = 1f, float sliderPosition = 0) : base(MathF.Round(minLabel, 2).ToString(), MathF.Round(maxLabel, 2).ToString(), sliderPosition)
        {
            CurrentValue = sliderPosition;
            min = minLabel;
            max = maxLabel;

            input.SetText(CurrentString);

            OnInputFocusedChanged += delegate (UIElement el)
            {
                if (float.TryParse(input.CurrentString, out float parsed))
                {
                    CurrentValue = parsed;
                }
            };

            slider.OnSliderInput += delegate (UIElement el)
            {
                input.SetText(CurrentString);
            };
        }
    }
}
