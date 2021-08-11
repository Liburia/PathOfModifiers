
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
	public class UIToggleImage : UIElement
	{
		private Asset<Texture2D> _onTexture;
		private Asset<Texture2D> _offTexture;
		private bool _isOn;

		public bool IsOn => _isOn;

		public UIToggleImage(Asset<Texture2D> onTexture, Asset<Texture2D> offTexture)
		{
			_onTexture = onTexture;
			_offTexture = offTexture;
			Width.Set(onTexture.Width(), 0f);
			Height.Set(onTexture.Height(), 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dims = GetDimensions();
			var texture = IsOn ? _onTexture.Value : _offTexture.Value;
			var color = IsMouseHovering ? Color.White : Color.Silver;
			spriteBatch.Draw(texture, dims.Position(), color);
		}

		public override void Click(UIMouseEvent evt)
		{
			Toggle();
			base.Click(evt);
		}

		public void SetState(bool value)
		{
			_isOn = value;
		}

		public void Toggle()
		{
			_isOn = !_isOn;
		}
	}
}
