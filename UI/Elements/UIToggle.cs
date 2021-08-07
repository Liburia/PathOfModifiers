using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
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
					SoundEngine.PlaySound(SoundID.MenuOpen);
				}
                else
				{
					BackgroundColor = defaultBackgroundColor;
					SoundEngine.PlaySound(SoundID.MenuClose);
				}
            }
        }
        bool _isOn = false;

		public Color defaultBackgroundColor = UICommon.backgroundColor;
		public Color activeBackgroundColor = UICommon.activeBackgroundColor;

		public override void MouseDown(UIMouseEvent evt)
		{
			base.MouseDown(evt);

			Toggle();
		}

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
			SoundEngine.PlaySound(SoundID.MenuTick);
		}

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }

        public void Toggle()
        {
			IsOn = !IsOn;
        }
		public void ToggleOn()
        {
			IsOn = true;
        }
		public void ToggleOff()
        {
			IsOn = false;
        }
	}
}
