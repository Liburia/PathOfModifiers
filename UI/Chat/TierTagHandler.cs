using Microsoft.Xna.Framework;
using System.Globalization;
using Terraria;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI.Chat
{
	public class TierTagHandler : ITagHandler
	{
		public class TierSnippet : TextSnippet
		{
			public TierSnippet(string tierText, Color color) : base($"[{ tierText }]", color) { }

            public override void OnHover()
			{
				Main.instance.MouseText("Affix tiers");
			}
        }

		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options) {
			return new TierSnippet(text, baseColor);
		}

		public static string GenerateTag(string tierText)
		{
			return $"[pomtier:{tierText}]";
		}
	}
}
