using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.UI.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace PathOfModifiers.UI.States.ModifierForgeElements
{
    class SelectList<T> : UIList<T> where T : UIToggle
    {
        public delegate void OnEntryToggledOn(ActionListEntry entry);

        public T SelectedAction { get; private set; }

        public override void Add(T item)
        {
            if (SelectedAction == null)
            {
                SelectedAction = item;
                item.SetState(true);
            }

            item.OnClick += Item_OnClick;

            base.Add(item);
        }

        private void Item_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var toggledAction = (T)listeningElement;
            if (toggledAction.IsOn)
            {
                SelectedAction.SetState(false);

                SelectedAction = toggledAction;
            }
            else
            {
                toggledAction.SetState(true);
            }
        }
    }
}
