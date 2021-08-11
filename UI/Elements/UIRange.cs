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
	public class UIRange : UIElement
	{
        const float inputWidth = 40f;

        protected string leftLabel;
        protected string rightLabel;

        public UISlider slider;
        public UIFocusInputTextField input;

        public event ElementEvent OnInputFocusedChanged;

        public UIRange(string leftLabel, string rightLabel, float sliderPosition = 0)
        {
			this.leftLabel = leftLabel;
			this.rightLabel = rightLabel;

            slider = new(sliderPosition);
            Append(slider);

            input = new("");
            input.Top.Set(0f, 0f);
            input.MinWidth.Set(inputWidth, 0f);
            input.MinHeight.Set(20f, 0f);
            input.OnTextChange += delegate (object sender, EventArgs e)
            {
                if (input.Focused)
                {
                    OnInputFocusedChanged?.Invoke(this);
                }
            };
            Append(input);

            MinHeight.Set(slider.GetDimensions().Height + 10f, 0f);
            Recalculate();
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

            var minPosition = new Vector2(slider.lineLeft, dims.Y + slider.GetDimensions().Height - 4f);
            ChatManager.DrawColorCodedString(sb, font, leftLabel, minPosition, Color.White, 0f, Vector2.Zero, scale);

            var maxPosition = new Vector2(slider.lineLeft + slider.lineWidth, minPosition.Y);
            maxPosition.X -= font.MeasureString(rightLabel).X * scale.X;
            ChatManager.DrawColorCodedString(sb, font, rightLabel, maxPosition, Color.White, 0f, Vector2.Zero, scale);
        }
    }
}
