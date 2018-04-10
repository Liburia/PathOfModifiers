using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI
{
    internal class UITextButton : UIPanelButton
    {
        public string text;
        Color textColor;
        public float textScale;
        public DynamicSpriteFont textFont;
        public Vector2 textSize;
        
        public UITextButton(string text, Color textColor, float textScale = 1, DynamicSpriteFont textFont = null, Texture2D borderTexture = null, Texture2D backgroundTexture = null)
            :base(borderTexture, backgroundTexture)
        {
            this.text = text;
            this.textColor = textColor;
            this.textScale = textScale;
            if (textFont == null)
                textFont = Main.fontMouseText;
            this.textFont = textFont;
            textSize = new Vector2(textFont.MeasureString(text).X, 16f) * textScale;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle innerDimensions = base.GetInnerDimensions();
            Vector2 pos = innerDimensions.Position();
            pos.X += (innerDimensions.Width - textSize.X) * 0.5f;
            pos.Y += (innerDimensions.Height - textSize.Y) * 0.5f;
            Utils.DrawBorderString(spriteBatch, text, pos, textColor, textScale, 0f, 0f, -1);
        }
    }
}