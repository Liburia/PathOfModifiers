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
	internal class UIIntRange : UIRange
	{
        int _currentValue;
		public int CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = Math.Clamp(value, 0, steps);
                slider.SliderPosition = _currentValue * interval;
            }
        }
        public string CurrentString => CurrentValue.ToString();

        int steps;
        float interval;

		public UIIntRange(int steps, int startingValue) : base("0", steps.ToString())
        {
            this.steps = steps;
            interval = 1f / steps;
            CurrentValue = startingValue;

            input.SetText(CurrentString);

            OnInputFocusedChanged += delegate (UIElement el)
            {
                if (int.TryParse(input.CurrentString, out int parsed))
                {
                    CurrentValue = parsed;
                }
            };

            slider.OnSliderInput += delegate (UIElement el)
            {

                CurrentValue = (int)MathF.Round(slider.SliderPosition / interval);
                input.SetText(CurrentString);
            };
        }
    }
}
