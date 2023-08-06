using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
    internal class UIToggle : UIPanel
    {
        public bool IsOn
        {
            get => _isOn;
            private set
            {
                _isOn = value;
                if (_isOn)
                {
                    BackgroundColor = activeBackgroundColor;
                }
                else
                {
                    BackgroundColor = defaultBackgroundColor;
                }
            }
        }
        bool _isOn = false;

        public Color defaultBackgroundColor = UICommon.backgroundColor;
        public Color activeBackgroundColor = UICommon.activeBackgroundColor;


        public override void LeftClick(UIMouseEvent evt)
        {
            Toggle();

            SoundEngine.PlaySound(SoundID.MenuTick);

            base.LeftClick(evt);
        }

        public void SetState(bool value)
        {
            IsOn = value;
        }

        public void Toggle()
        {
            IsOn = !IsOn;
        }
    }
}
