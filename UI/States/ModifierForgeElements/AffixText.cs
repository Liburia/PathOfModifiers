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
        private Color _color = Color.White;
        public bool DynamicallyScaleDownToWidth;
        private TextSnippet[] _parsedText = new TextSnippet[0];

        public string Text => _text.ToString();

        public Color TextColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
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

        public override void Recalculate()
        {
            InternalSetText(_text, _textScale);
            base.Recalculate();
        }

        private void InternalSetText(object text, float textScale)
        {
            _text = text;
            _textScale = textScale;
            _parsedText = ChatManager.ParseMessage(Text, TextColor).ToArray();

            var textSize = UICommon.Text.Measure(_parsedText, _font, _textScale, MaxWidth.GetValue(Parent.GetInnerDimensions().Width));

            MinWidth.Set(textSize.X + PaddingLeft + PaddingBottom, 0f);
            MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
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
