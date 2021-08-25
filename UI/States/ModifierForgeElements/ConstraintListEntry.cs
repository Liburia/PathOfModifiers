using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.UI.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace PathOfModifiers.UI.States.ModifierForgeElements
{
    class ConstraintListEntry<T> : UIToggle
		where T : SelectableConstraint
    {
		UIText _text;
		public T constraint;

		private bool _isEnabled = true;
		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				_isEnabled = value;
			}
		}

		public ConstraintListEntry(string text, T constraint) : base()
		{
			MinWidth.Set(0f, 1f);
			MinHeight.Set(0f, 0.333f);

			SetPadding(0f);

			_text = new(text);
			_text.HAlign = 0.5f;
			_text.VAlign = 0.5f;
			Append(_text);

			this.constraint = constraint;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (IsMouseHovering)
			{
				Main.instance.MouseText(constraint.Description);
			}
		}

		public override void Click(UIMouseEvent evt)
		{
			if (IsEnabled)
			{
				base.Click(evt);
			}
		}

		protected override void DrawPanel(SpriteBatch spriteBatch)
		{
			var dims = GetDimensions();
			int x = (int)dims.X;
			int y = (int)dims.Y;
			int width = (int)dims.Width;
			int height = (int)dims.Height;

			var texture = Terraria.GameContent.TextureAssets.MagicPixel;
			Rectangle source = new(0, 0, 1, 1);

			//BG
			spriteBatch.Draw(texture.Value, new Rectangle(x + 1, y + 1, width - 2, height - 2), source, BackgroundColor);

			//horiz border
			spriteBatch.Draw(texture.Value, new Rectangle(x, y, width, 1), source, BorderColor);
			spriteBatch.Draw(texture.Value, new Rectangle(x, y + height - 1, width, 1), source, BorderColor);
			//vert border
			spriteBatch.Draw(texture.Value, new Rectangle(x, y, 2, height), source, BorderColor);
			spriteBatch.Draw(texture.Value, new Rectangle(x + width - 2, y, 2, height), source, BorderColor);

			if (!IsEnabled)
			{
				//Disabled overlay
				spriteBatch.Draw(texture.Value, new Rectangle(x + 1, y + 1, width - 2, height - 2), source, UICommon.disabledOverlayColor);
			}
		}
	}
}
