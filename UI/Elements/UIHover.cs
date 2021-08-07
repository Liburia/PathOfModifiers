using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
	internal class UIHover : UIPanel
	{
		public Color defaultBackgroundColor = UICommon.backgroundColor;
		public Color hoverBackgroundColor = UICommon.hoverBackgroundColor;

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
            BackgroundColor = hoverBackgroundColor;
        }
        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            BackgroundColor = defaultBackgroundColor;
        }
    }
}
