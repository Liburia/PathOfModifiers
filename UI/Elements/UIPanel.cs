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
    public class UIPanel : UIElement
	{
		private Asset<Texture2D> _borderTexture;
		private Asset<Texture2D> _backgroundTexture;
		public Color BorderColor = Color.Black;
		public Color BackgroundColor = new Color(63, 82, 151) * 0.7f;
		private bool _needsTextureLoading = true;

		private void LoadTextures()
		{
			// These used to be moved to OnActivate in order to avoid texture loading on JIT thread.
			// Doing so caused issues with missing backgrounds and borders because Activate wasn't always being called.
			if (_borderTexture == null)
				_borderTexture = Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");

			if (_backgroundTexture == null)
				_backgroundTexture = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");
		}

		public UIPanel()
		{
			SetPadding(UICommon.Panel.cornerSize);
		}

		public UIPanel(Asset<Texture2D> customBackground, Asset<Texture2D> customborder)
		{
			if (_borderTexture == null)
				_borderTexture = customborder;

			if (_backgroundTexture == null)
				_backgroundTexture = customBackground;

			SetPadding(UICommon.Panel.cornerSize);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (_needsTextureLoading)
			{
				_needsTextureLoading = false;
				LoadTextures();
			}

			var dims = GetDimensions();

			if (_backgroundTexture != null)
				UICommon.Panel.Draw(spriteBatch, _backgroundTexture.Value, BackgroundColor, dims);

			if (_borderTexture != null)
				UICommon.Panel.Draw(spriteBatch, _borderTexture.Value, BorderColor, dims);
		}
	}
}
