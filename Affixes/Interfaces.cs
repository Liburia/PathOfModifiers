using PathOfModifiers.UI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
