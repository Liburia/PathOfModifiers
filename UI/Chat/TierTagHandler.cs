using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI.Chat
{
    public class TierTagHandler : ITagHandler
    {
        public class TierSnippet : TextSnippet
        {
            int _tierText;
            int _maxTierText;

            public TierSnippet(int tierText, int maxTierText, Color color) : base($"[{ tierText }]", color)
            {
                _tierText = tierText;
                _maxTierText = maxTierText;
            }

            public override void OnHover()
            {
                Main.instance.MouseText($"Tier [{ _tierText }/{ _maxTierText }]");
            }
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            if (!int.TryParse(text, out int tierText) || !int.TryParse(options, out int maxTierText))
                return new TextSnippet(text);

            return new TierSnippet(tierText, maxTierText, baseColor);
        }

        public static string GenerateTag(int tierText, int maxTierText)
        {
            return $"[pomtier/{maxTierText}:{tierText}]";
        }
    }
}
