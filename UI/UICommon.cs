using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
	static class UICommon
	{
		public const float spacing = 5f;

		public readonly static Color backgroundColor = new Color(63, 82, 151) * 0.7f;
		public readonly static Color hoverBackgroundColor = new Color(63, 82, 151) * 0.9f;
		public readonly static Color activeBackgroundColor = Color.Yellow * 0.7f;
	}
}
