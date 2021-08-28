using Microsoft.Xna.Framework;
using System.Globalization;
using Terraria;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI.Chat
{
	public class ValueRangeTagHandler : ITagHandler
	{
		public class ValueRangeSnippet<T> : TextSnippet
		{
			public T _current;
			public T _min;
			public T _max;
			public ValueRangeSnippet(T current, T min, T max, Color color)
				: base(current.ToString(), color)
			{
				_current = current;
				_min = min;
				_max = max;
			}

            public override void OnHover()
			{
				Main.instance.MouseText($"({_min}-{_max})");
			}
		}

		TextSnippet ITagHandler.Parse(string text, Color baseColor, string optionstring) {
			var options = optionstring.Split('/');
			var numberTypeString = options[0];
			var minString = options[1];
			var maxString = options[2];

			if (numberTypeString == "f")
			{
				if (!float.TryParse(text, out var currentF)
					|| !float.TryParse(minString, out var minF)
					|| !float.TryParse(maxString, out var maxF))
				{
					return new TextSnippet(text);
				}

				return new ValueRangeSnippet<float>(currentF, minF, maxF, baseColor);
			}
			else
				{
				if (!int.TryParse(text, out var currentI)
					|| !int.TryParse(minString, out var minI)
					|| !int.TryParse(maxString, out var maxI))
				{
					return new TextSnippet(text);
				}

				return new ValueRangeSnippet<int>(currentI, minI, maxI, baseColor);
			}
		}

		/// <summary>
		/// Get either the value or the correspondig tag for the range
		/// </summary>
		public static string GetTextOrTag(int current, int min, int max, bool isTag = true)
		{
			if (isTag)
			{
				return GenerateTag(current, min, max);
			}
			else
			{
				return current.ToString();
			}
		}
		/// <summary>
		/// Get either the value or the correspondig tag for the range
		/// </summary>
		public static string GetTextOrTag(float current, float min, float max, bool isTag = true)
		{
			if (isTag)
			{
				return GenerateTag(current, min, max);
			}
			else
			{
				return current.ToString();
			}
		}
		public static string GenerateTag(int current, int min, int max)
		{
			return $"[pomvr/i/{min}/{max}:{current}]";
		}
		public static string GenerateTag(float current, float min, float max)
		{
			return $"[pomvr/f/{min}/{max}:{current}]";
		}
	}
}
