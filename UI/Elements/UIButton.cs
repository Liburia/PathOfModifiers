﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
	internal class UIButton : UIPanel
	{
        public UIButton()
        {
            BackgroundColor = UICommon.backgroundColor;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            BackgroundColor = UICommon.hoverBackgroundColor;
			SoundEngine.PlaySound(SoundID.MenuTick);
        }
        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            BackgroundColor = UICommon.backgroundColor;
        }
        public override void Click(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
        public override void MouseDown(UIMouseEvent evt) { }
        public override void MouseUp(UIMouseEvent evt) { }
    }
}
