using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
	internal class UIItemSlot : UIElement
	{
        protected Asset<Texture2D> _texture;

        public Item Item { get; private set; }

        public UIItemSlot()
        {
            _texture = Terraria.GameContent.TextureAssets.InventoryBack9;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var dims = GetDimensions();
            Rectangle dest = dims.ToRectangle();

            spriteBatch.Draw(_texture.Value, dest, Color.White);
        }
    }
}
