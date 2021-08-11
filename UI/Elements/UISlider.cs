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

namespace PathOfModifiers.UI.Elements
{
	public class UISlider : UIElement
	{
        public const float lineOffset = 5;
        public const float lineHeight = 2;

        /// <summary>
        /// 0-1 range
        /// </summary>
        public virtual float SliderPosition
        {
            get => _sliderPosition;
            set => _sliderPosition = value;
        }
        protected float _sliderPosition;

        protected float SliderX => lineLeft + (lineWidth * SliderPosition);

        protected bool isDragging;

        public float lineLeft;
        public float lineWidth;

        public event ElementEvent OnSliderInput;

        public UISlider(float sliderPosition = 0f)
        {
            SliderPosition = sliderPosition;
            MinHeight.Set(16f, 0);
        }

        public override void Recalculate()
        {
            base.Recalculate();
            var dims = GetDimensions();
            lineLeft = dims.X + lineOffset;
            lineWidth = dims.Width - (lineOffset * 2);
        }

        protected override void DrawSelf(SpriteBatch sb)
        {
            base.DrawSelf(sb);
            DrawLine(sb);
            DrawSlider(sb);
        }

        void DrawLine(SpriteBatch sb)
        {
            var dims = GetDimensions();
            var source = new Rectangle(0, 0, 1, 1);
            var dest = new Rectangle((int)lineLeft, (int)(dims.Center().Y - (lineHeight / 2)), (int)lineWidth, (int)lineHeight);
            var origin = new Vector2(0f, 0f);
            sb.Draw(TextureAssets.ColorBlip.Value, dest, source, Color.LightGray, 0f, origin, SpriteEffects.None, 0f);
        }
        void DrawSlider(SpriteBatch sb)
        {
            var dimensions = GetDimensions();
            var source = new Rectangle(0, 0, 1, 1);
            var position = new Vector2(SliderX, dimensions.Center().Y);
            var origin = new Vector2(0.5f, 0.5f);
            var scale = new Vector2(10, 10);
            sb.Draw(TextureAssets.ColorBlip.Value, position, source, Color.IndianRed, MathHelper.PiOver4, origin, scale, SpriteEffects.None, 0f);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            isDragging = true;
        }
        public override void MouseUp(UIMouseEvent evt)
        {
            isDragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isDragging)
            {
                var localMouseX = Main.mouseX - lineLeft;
                var newSliderPosition = MathHelper.Clamp(localMouseX / lineWidth, 0f, 1f);
                if (SliderPosition != newSliderPosition)
                {
                    SliderPosition = newSliderPosition;
                    OnSliderInput?.Invoke(this);
                }
            }
        }
    }
}
