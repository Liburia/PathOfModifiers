using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace PathOfModifiers.UI.States.DebugElements
{
    class AffixListEntry : UIElement
    {
        public Affix affix;
        public UIToggle selectToggle;
        public UIImageButton removeButton;

        public AffixListEntry(Affix affix)
        {
            this.affix = affix;

            MinWidth.Set(0f, 1f);
            MinHeight.Set(30f, 0f);

            removeButton = new(ModContent.Request<Texture2D>(PoMGlobals.Path.Image.UI.CloseButton, ReLogic.Content.AssetRequestMode.ImmediateLoad));
            removeButton.Top.Set(-(removeButton.Height.Pixels / 2f), 0.5f);
            removeButton.Left.Set(-(removeButton.Width.Pixels + 5f), 1f);
            Append(removeButton);

            selectToggle = new();
            selectToggle.MinWidth.Set(-removeButton.GetDimensions().Width, 1);
            selectToggle.MinHeight.Set(MinHeight.Pixels, 0);
            selectToggle.SetPadding(0f);
            Append(selectToggle);

            UIText text = new(affix.GetType().Name, UICommon.textMedium);
            text.IgnoresMouseInteraction = true;
            text.HAlign = 0.5f;
            text.VAlign = 0.5f;
            Append(text);
        }

        public void HideRemoveButton()
        {
            selectToggle.Width.Set(0f, 1f);

            removeButton.Remove();
            Recalculate();
        }
        public void ShowRemoveButton()
        {
            selectToggle.Width.Set(-removeButton.GetDimensions().Width, 1f);

            Append(removeButton);
        }
    }
}
