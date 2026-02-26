using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI.States.ModifierForgeElements
{
    public class AffixText : UIElement
    {
        private DynamicSpriteFont _font;
        private object _text = "";
        private float _textScale = 1f;
        private Color _baseColor = Color.White;
        private Color _currentColor = Color.White;
        public bool DynamicallyScaleDownToWidth;
        private TextSnippet[] _parsedText = [];

        private Optional<Color> _flashing_color = new();
        /// <summary>
        /// Rate of time change per second
        /// </summary>
        private float _flashing_speed = 2f;
        /// <summary>
        /// [0,1] oscillates, 0 being _color, 1 being flashing color
        /// </summary>
        private float _flashing_time = 0f;
        private float _flashing_direction = 1f;

        public string Text => _text.ToString();

        public Color TextColor
        {
            get
            {
                return _baseColor;
            }
            set
            {
                _baseColor = value;
            }
        }

        public AffixText()
        {
            _font = FontAssets.MouseText.Value;
        }

        public void SetText(string text)
        {
            _text = text;
            Recalculate();
        }
        public void SetText(LocalizedText text)
        {
            _text = text;
            Recalculate();
        }
        public void SetText(string text, float textScale)
        {
            _text = text;
            _textScale = textScale;
            Recalculate();
        }
        public void SetText(LocalizedText text, float textScale)
        {
            _text = text;
            _textScale = textScale;
            Recalculate();
        }
        public void StartFlashing(Color color, float speed)
        {
            _flashing_color = new(color);
            _flashing_speed = speed;
            _flashing_time = 0f;
        }
        public void StartFlashing()
        {
            Color newColor = new(0.1f, 0.1f, 0.1f, 1.0f);
            StartFlashing(newColor, _flashing_speed);
        }
        public void StopFlashing()
        {
            _flashing_color = new();
            _currentColor = _baseColor;
        }

        public override void Recalculate()
        {
            InternalSetText(_text, _textScale);
            base.Recalculate();
        }

        private void InternalSetText(object text, float textScale)
        {
            _text = text;
            _textScale = textScale;
            _parsedText = ChatManager.ParseMessage(Text, _currentColor).ToArray();

            var textSize = UICommon.Text.Measure(_parsedText, _font, _textScale, MaxWidth.GetValue(Parent.GetInnerDimensions().Width));

            MinWidth.Set(textSize.X + PaddingLeft + PaddingBottom, 0f);
            MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_flashing_color.HasValue)
            {
                float change = _flashing_speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _flashing_time += change * _flashing_direction;
                if (_flashing_time >= 1f || _flashing_time <= 0f)
                {
                    _flashing_direction = -_flashing_direction;
                    _flashing_time = MathHelper.Clamp(_flashing_time, 0f, 1f);
                }
                _currentColor = Color.Lerp(_baseColor, _flashing_color.Value, _flashing_time / 2.5f);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            Vector2 pos = innerDimensions.Position();

            //var size = UICommon.Text.Measure(_parsedText, _font, _textScale, innerDimensions.Width);
            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), Color.Red);

            int? hoveredSnippet = UICommon.Text.Draw(spriteBatch, _parsedText, _font, pos, _textScale, innerDimensions.Width);
            if (hoveredSnippet.HasValue)
            {
                _parsedText[hoveredSnippet.Value].OnHover();
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    _parsedText[hoveredSnippet.Value].OnClick();
            }
        }
    }
}
