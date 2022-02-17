using System;
using Terraria.UI;

namespace PathOfModifiers.Affixes
{
    public interface IPrefix { }
    public interface ISuffix { }

    public interface IUIDrawable
    {
        public UIElement CreateUI(UIElement parent, Action onChangeCallback = null);
    }
}
