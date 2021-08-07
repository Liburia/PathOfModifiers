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
	internal class UIRange : UIElement
	{
        const float inputWidth = 30f;

        protected string min;
        protected string max;

        public UISlider slider;
        public UIFocusInputTextField input;

        public event ElementEvent OnInputFocusedChanged;

        public UIRange(string min, string max, float sliderPosition = 0)
        {
			this.min = min;
			this.max = max;

            slider = new(sliderPosition);
            Append(slider);

            Height = slider.Height;

            input = new("");
            input.Top.Set(0, 0.4f);
            input.Width.Set(inputWidth, 0f);
            input.OnTextChange += delegate (object sender, EventArgs e)
            {
                if (input.Focused)
                {
                    OnInputFocusedChanged?.Invoke(this);
                }
            };
            Append(input);
        }

        public override void Recalculate()
        {
            base.Recalculate();

            var dims = GetInnerDimensions();
            slider.Width.Set(dims.Width - (inputWidth + UICommon.spacing), 0f);
            input.Left.Set(slider.Width.Pixels + UICommon.spacing, 0f);

            slider.Recalculate();
            input.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            DrawText(sb);
        }

        void DrawText(SpriteBatch sb)
        {
            var dims = GetDimensions();
            var font = FontAssets.MouseText.Value;
            var scale = new Vector2(0.7f, 0.7f);

            var minPosition = new Vector2(slider.lineLeft, dims.Center().Y + (UISlider.lineHeight / 2) + 7);
            ChatManager.DrawColorCodedString(sb, font, min, minPosition, Color.White, 0f, Vector2.Zero, scale);

            var maxPosition = new Vector2(slider.lineLeft + slider.lineWidth, minPosition.Y);
            maxPosition.X -= font.MeasureString(max).X * scale.X;
            ChatManager.DrawColorCodedString(sb, font, max, maxPosition, Color.White, 0f, Vector2.Zero, scale);
        }
    }
}
