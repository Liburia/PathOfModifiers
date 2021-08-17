using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.UI.Chat;
using ReLogic.Graphics;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

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

        public static class Panel
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

		public static class Text
		{
			public static Vector2 Measure(TextSnippet[] snippets, DynamicSpriteFont font, float scale, float maxWidth)
			{
				float scaledMaxWidth = maxWidth / scale;

				Vector2 textSize = new Vector2(0f, font.LineSpacing);

				float currentLineWidth = 0f;
				foreach (var snippet in snippets)
				{
					string[] lines = Regex.Split(snippet.Text, "(\n)");
					foreach (var line in lines.Select((text, index) => (text, index)))
					{
						if (line.index != 0)
						{
							textSize.X = MathF.Max(textSize.X, currentLineWidth);
							textSize.Y += font.LineSpacing;
							currentLineWidth = 0f;
						}

						string[] words = snippet is KeywordTagHandler.KeywordSnippet
							? new[] { line.text }
							: Regex.Split(line.text, "( )");
						for (int i = 0; i < words.Length; i++)
						{
							var word = words[i];
							var wordSize = font.MeasureString(word);
							float newLineWidth = currentLineWidth + wordSize.X;

							if (newLineWidth <= scaledMaxWidth || currentLineWidth == 0f)
							{
								currentLineWidth = newLineWidth;
							}
							else
							{
								textSize.X = MathF.Max(textSize.X, currentLineWidth);
								textSize.Y += font.LineSpacing;
								currentLineWidth = 0f;

								i--;
							}
						}
					}
				}

				textSize.X = MathF.Max(textSize.X, currentLineWidth);

				return textSize * scale;
			}

			public static int? Draw(SpriteBatch sb, TextSnippet[] snippets, DynamicSpriteFont font, Vector2 position, float scale, float maxWidth)
			{
				int? hoveredSnippet = null;
				var mousePos = Main.MouseScreen;

				float lineSpacing = font.LineSpacing * scale;

				Vector2 drawOffset = Vector2.Zero;

				foreach (var s in snippets.Select((snippet, index) => (snippet, index)))
				{
					string[] lines = Regex.Split(s.snippet.Text, "(\n)");
					foreach (var line in lines.Select((text, index) => (text, index)))
					{
						if (line.index != 0)
						{
							drawOffset.X = 0f;
							drawOffset.Y += lineSpacing;
						}

						string[] words = s.snippet is KeywordTagHandler.KeywordSnippet
							? new[] { line.text }
							: Regex.Split(line.text, "( )");
						for (int i = 0; i < words.Length; i++)
						{
							var word = words[i];
							var wordSize = font.MeasureString(word) * scale;
							float newLineWidth = drawOffset.X + wordSize.X;

							if (newLineWidth <= maxWidth || drawOffset.X == 0f)
							{
								var drawPos = position + drawOffset;
								sb.DrawString(font, word, drawPos, s.snippet.GetVisibleColor(), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
								if (mousePos.Between(drawPos, drawPos + wordSize))
									hoveredSnippet = s.index;

								drawOffset.X = newLineWidth;
							}
							else
							{
								drawOffset.X = 0f;
								drawOffset.Y += lineSpacing;

								i--;
							}
						}
					}
				}

				return hoveredSnippet;
			}
		}
	}
}
