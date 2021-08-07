using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI.Elements
{
	internal class UIFloatRange : UIRange
	{
		public float current;
        public string CurrentString => MathF.Round(current, 2).ToString();
        protected new float min;
        protected new float max;

		public UIFloatRange(float min, float max, float sliderPosition = 0) : base(min.ToString(), max.ToString(), sliderPosition)
        {
			this.min = min;
			this.max = max;
            current = min;

            OnInputFocusedChanged += delegate (UIElement el)
            {
                if (float.TryParse(input.CurrentString, out float parsed))
                {
                    current = parsed;
                }
            };

            slider.OnSliderChanged += delegate (UIElement el)
            {
                current = min + ((max - min) * slider.SliderPosition);
                input.SetText(CurrentString);
            };
        }
    }
}
