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
        public delegate void EntryEvent(T entry);

        public event EntryEvent OnEntrySelected;

        public T SelectedItem { get; private set; }

        public override void Add(T item)
        {
            item.OnClick += (UIMouseEvent evt, UIElement listeningElement) => Select((T)listeningElement);

            base.Add(item);
        }

        public void Select(T toggledAction)
        {
            SelectedItem?.SetState(false);
            SelectedItem = toggledAction;
            toggledAction.SetState(true);

            OnEntrySelected?.Invoke(toggledAction);
        }

        public void Deselect()
        {
            SelectedItem?.SetState(false);
            SelectedItem = null;
        }
    }
}
