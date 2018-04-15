using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    internal class UIPanelButton : UIElement
    {
        static int CORNER_SIZE = 12;
        static int BAR_SIZE = 4;

        public Func<bool> interactiveCondition = delegate () { return true; };

        public bool isToggle = false;
        public bool toggleState = false;

        public Color borderColor = Color.Black;
        public Color borderColorHover = Color.Gold;
        public Color backgroundColor = new Color(63, 82, 151) * 0.7f;
        public Color backgroundColorHover = new Color(63, 82, 151) * 0.7f;

        Texture2D borderTexture;
        Texture2D backgroundTexture;

        public UIPanelButton(Texture2D borderTexture = null, Texture2D backgroundTexture = null)
        {
            if (borderTexture == null)
            {
                borderTexture = TextureManager.Load("Images/UI/PanelBorder");
            }
            if (backgroundTexture == null)
            {
                backgroundTexture = TextureManager.Load("Images/UI/PanelBackground");
            }
            this.borderTexture = borderTexture;
            this.backgroundTexture = backgroundTexture;

            SetPadding(CORNER_SIZE);
        }

        void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            CalculatedStyle dimensions = base.GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Point point2 = new Point(point.X + (int)dimensions.Width - CORNER_SIZE, point.Y + (int)dimensions.Height - CORNER_SIZE);
            int width = point2.X - point.X - CORNER_SIZE;
            int height = point2.Y - point.Y - CORNER_SIZE;
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, CORNER_SIZE, CORNER_SIZE), new Rectangle?(new Rectangle(0, 0, CORNER_SIZE, CORNER_SIZE)), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, CORNER_SIZE, CORNER_SIZE), new Rectangle?(new Rectangle(CORNER_SIZE + BAR_SIZE, 0, CORNER_SIZE, CORNER_SIZE)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, CORNER_SIZE, CORNER_SIZE), new Rectangle?(new Rectangle(0, CORNER_SIZE + BAR_SIZE, CORNER_SIZE, CORNER_SIZE)), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, CORNER_SIZE, CORNER_SIZE), new Rectangle?(new Rectangle(CORNER_SIZE + BAR_SIZE, CORNER_SIZE + BAR_SIZE, CORNER_SIZE, CORNER_SIZE)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + CORNER_SIZE, point.Y, width, CORNER_SIZE), new Rectangle?(new Rectangle(CORNER_SIZE, 0, BAR_SIZE, CORNER_SIZE)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + CORNER_SIZE, point2.Y, width, CORNER_SIZE), new Rectangle?(new Rectangle(CORNER_SIZE, CORNER_SIZE + BAR_SIZE, BAR_SIZE, CORNER_SIZE)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + CORNER_SIZE, CORNER_SIZE, height), new Rectangle?(new Rectangle(0, CORNER_SIZE, CORNER_SIZE, BAR_SIZE)), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + CORNER_SIZE, CORNER_SIZE, height), new Rectangle?(new Rectangle(CORNER_SIZE + BAR_SIZE, CORNER_SIZE, CORNER_SIZE, BAR_SIZE)), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + CORNER_SIZE, point.Y + CORNER_SIZE, width, height), new Rectangle?(new Rectangle(CORNER_SIZE, CORNER_SIZE, BAR_SIZE, BAR_SIZE)), color);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            bool isActive = IsMouseHovering || (isToggle && toggleState);
            DrawPanel(spriteBatch, backgroundTexture, isActive ? backgroundColorHover : backgroundColor);
            DrawPanel(spriteBatch, borderTexture, isActive ? borderColorHover : borderColor);
        }

        public override void Click(UIMouseEvent evt)
    {
            if (interactiveCondition())
            {
                base.Click(evt);
                toggleState = !toggleState;
            }
        }
    }
}