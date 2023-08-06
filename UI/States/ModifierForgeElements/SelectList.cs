using PathOfModifiers.UI.Elements;
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
            item.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => Select((T)listeningElement);

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
