using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
    internal class UIButton : UIPanel
    {
        bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;

                BackgroundColor = value ? UICommon.backgroundColor : UICommon.disabledOverlayColor;
            }
        }

        public UIButton()
        {
            BackgroundColor = UICommon.backgroundColor;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            if (IsEnabled)
            {
                base.MouseOver(evt);
                BackgroundColor = UICommon.hoverBackgroundColor;
            }
        }
        public override void MouseOut(UIMouseEvent evt)
        {
            if (IsEnabled)
            {
                base.MouseOut(evt);
                BackgroundColor = UICommon.backgroundColor;
            }
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            if (IsEnabled)
            {
                base.LeftClick(evt);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
    }
}
