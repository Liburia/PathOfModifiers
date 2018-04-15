using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI
{
    class UIPanelConditioned : UIPanel
    {
        public Func<bool> drawableCondition = delegate () { return true; };

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (drawableCondition())
                base.Draw(spriteBatch);
        }
    }
}