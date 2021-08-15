using Microsoft.Xna.Framework;
using System.Globalization;
using Terraria;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI.Chat
{
	public enum KeywordType
    {
		Poison = 0,
		Bleed,
		Shock,
		Ignite,
		Chill,
		Dodge,
		Adrenaline,
		MoltenShell,
	}
	public class Keyword
    {
		public static KeywordDefinition[] keywords = new KeywordDefinition[]
		{
			new Poison(),
			new Bleed(),
			new Shock(),
			new Ignite(),
			new Chill(),
			new Dodge(),
			new Adrenaline(),
			new MoltenShell(),
		};

		public abstract class KeywordDefinition
		{
			public abstract Color Color { get; }
			public abstract string Name { get; }
			public abstract string Description { get; }
		}

		public class Poison : KeywordDefinition
		{
			public override Color Color => new(56, 118, 29);
			public override string Name => "Poison";
			public override string Description => "Deals % of damage dealt per second over the duration, stacks.";
		}
		public class Bleed : KeywordDefinition
		{
			public override Color Color => new(153, 0, 0);
			public override string Name => "Bleed";
			public override string Description => "Deals % of damage dealt per second over the duration";
		}
		public class Shock : KeywordDefinition
		{
			public override Color Color => new(241, 194, 50);
			public override string Name => "Shock";
			public override string Description => "Increases damage taken by % over ailment duration(5s)";
		}
		public class Ignite : KeywordDefinition
		{
			public override Color Color => new(180, 95, 6);
			public override string Name => "Ignite";
			public override string Description => "Deals % of damage dealt per second over ailment duration(5s), players take up to 1% of max life per second";
		}
		public class Chill : KeywordDefinition
		{
			public override Color Color => new(61, 133, 198);
			public override string Name => "Chill";
			public override string Description => "Reduces damage dealt over ailment duration(5s)";
		}
		public class Dodge : KeywordDefinition
		{
			public override Color Color => Color.LightGray;
			public override string Name => "Dodge";
			public override string Description => "Chance to avoid damage";
		}
		public class Adrenaline : KeywordDefinition
		{
			public override Color Color => Color.LightGray;
			public override string Name => "Adrenaline";
			public override string Description => "+50% damage, +30% attack speed, +30% move speed, -10% damage taken";
		}
		public class MoltenShell : KeywordDefinition
		{
			public override Color Color => Color.LightGray;
			public override string Name => "Molten Shell";
			public override string Description => "-10% damage taken, explodes at the end of the duration for\ntotal damage taken + (total damage taken / player max HP * 0.15)% of enemy HP";
		}
	}

	public class KeywordTagHandler : ITagHandler
	{
		private class KeywordSnippet : TextSnippet
		{
			private Keyword.KeywordDefinition _keyword;

			public KeywordSnippet(Keyword.KeywordDefinition keyword)
			{
				_keyword = keyword;
				Text = _keyword.Name;
			}

            public override Color GetVisibleColor()
            {
				return _keyword.Color;
            }

            public override void OnHover()
			{
				Main.instance.MouseText(_keyword.Description);
			}
		}

		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options) {
			if (!int.TryParse(options, out int keywordTypeInt)
				|| keywordTypeInt >= Keyword.keywords.Length)
				return new TextSnippet(text);

			var keyword = Keyword.keywords[keywordTypeInt];

			return new KeywordSnippet(keyword);
		}

		public static string GenerateTag(KeywordType keywordType)
		{
			var keyword = Keyword.keywords[(int)keywordType];
			return $"[pomkw/{(int)keywordType}:{keyword.Name}]";
		}
	}
}
