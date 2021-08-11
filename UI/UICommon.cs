using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace PathOfModifiers.UI
{
	static class UICommon
	{
		public const float spacing = 5f;

		public const float textBig = 1f;
		public const float textMedium = 0.7f;

		public readonly static Color backgroundColor = new Color(63, 82, 151) * 0.7f;
		public readonly static Color hoverBackgroundColor = new Color(63, 82, 151) * 0.9f;
		public readonly static Color activeBackgroundColor = Color.Yellow * 0.7f;

        public class Panel
		{
			public const int cornerSize = 12;
			public const int barSize = 4;

			public static void Draw(SpriteBatch spriteBatch, Texture2D texture, Color color, CalculatedStyle dimensions, int cornerSize = cornerSize, int barSize = barSize)
			{
				Point point = new((int)dimensions.X, (int)dimensions.Y);
				Point point2 = new(point.X + (int)dimensions.Width - cornerSize, point.Y + (int)dimensions.Height - cornerSize);
				int width = point2.X - point.X - cornerSize;
				int height = point2.Y - point.Y - cornerSize;
				spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, cornerSize, cornerSize), new Rectangle(0, 0, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, 0, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, cornerSize, cornerSize), new Rectangle(0, cornerSize + barSize, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, cornerSize, cornerSize), new Rectangle(cornerSize + barSize, cornerSize + barSize, cornerSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point.Y, width, cornerSize), new Rectangle(cornerSize, 0, barSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point2.Y, width, cornerSize), new Rectangle(cornerSize, cornerSize + barSize, barSize, cornerSize), color);
				spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + cornerSize, cornerSize, height), new Rectangle(0, cornerSize, cornerSize, barSize), color);
				spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + cornerSize, cornerSize, height), new Rectangle(cornerSize + barSize, cornerSize, cornerSize, barSize), color);
				spriteBatch.Draw(texture, new Rectangle(point.X + cornerSize, point.Y + cornerSize, width, height), new Rectangle(cornerSize, cornerSize, barSize, barSize), color);
			}
		}
	}
}
