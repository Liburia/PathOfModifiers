using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.Affixes;
using PathOfModifiers.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using PathOfModifiers.UI.States.DebugElements;

namespace PathOfModifiers.UI.States
{
    public class DebugPanel : UIState
	{
        public static void Toggle()
        {
            if (Systems.UI.DebugPanelInterface.CurrentState == null)
                Systems.UI.DebugPanelInterface?.SetState(Systems.UI.DebugUIState);
            else
                Systems.UI.DebugPanelInterface?.SetState(null);
        }

        Item _heldItem = new Item();
        Item HeldItem
        {
            get => _heldItem;
            set
            {
                if (value != _heldItem)
                {
                    _heldItem = value;
                    FilterAffixList(true);
                }
            }
        }

        class SelectedDebugAffix
        {
            public AffixListEntry affixElement;
            public IUIDrawable itemAffix;
        }
        SelectedDebugAffix _selectedAffix;
        SelectedDebugAffix SelectedAffix
        {
            get => _selectedAffix;
            set
            {
                _selectedAffix?.affixElement.selectToggle.SetState(false);

                _selectedAffix = value;

                affixEditingUI?.Remove();

                affixEditingUI = _selectedAffix?.itemAffix.CreateUI(affixUIParent);

                string name = _selectedAffix?.itemAffix.GetType().Name ?? "Affix";
                affixName.SetText(name);
            }
        }

        AffixListEntry[] allAffixes = Array.Empty<AffixListEntry>();

        UIDraggablePanel panel;

        UIToggle onItemFilterToggle;
        UIFocusInputTextField affixSearch;
        UIList<AffixListEntry> affixList;

        UIText affixName;
        UIElement affixUIParent;
        UIElement affixEditingUI;

        public override void OnInitialize()
        {
            panel = new();
			panel.Left.Set(200, 0);
			panel.Top.Set(100, 0);
			panel.MinWidth.Set(600, 0);
			panel.MinHeight.Set(400, 0);
			Append(panel);
			{
                Terraria.GameContent.UI.Elements.UIText title = new("PoM Debug", UICommon.textMedium);
                title.IgnoresMouseInteraction = true;
                title.Top.Set(0, 0);
                title.HAlign = 0.5f;
                panel.Append(title);

                UIImageButton closePanelX = new(ModContent.Request<Texture2D>(PoMGlobals.Path.Image.UI.CloseButton, ReLogic.Content.AssetRequestMode.ImmediateLoad));
				closePanelX.Top.Set(0, 0);
				closePanelX.Left.Set(550, 0);
				closePanelX.OnClick += (UIMouseEvent evt, UIElement listeningElement) => Toggle();
				panel.Append(closePanelX);

                UIElement content = new();
                content.Top.Set(title.Top.Pixels + title.GetDimensions().Height + UICommon.spacing, 0);
                content.MinWidth.Set(0, 1);
                content.MinHeight.Set(0, 1);
                panel.Append(content);
                {
                    UIElement selectionHalf = new();
                    selectionHalf.MinWidth.Set(0, 0.5f);
                    selectionHalf.MinHeight.Set(0, 1);
                    content.Append(selectionHalf);
                    {
                        onItemFilterToggle = new();
                        onItemFilterToggle.Top.Set(0, 0);
                        onItemFilterToggle.MinWidth.Set(120f, 0f);
                        onItemFilterToggle.MinHeight.Set(30, 0);
                        onItemFilterToggle.OnClick += (UIMouseEvent evt, UIElement listeningElement) => FilterAffixList();
                        selectionHalf.Append(onItemFilterToggle);
                        {
                            UIText text = new("On item", UICommon.textBig);
                            text.IgnoresMouseInteraction = true;
                            text.HAlign = 0.5f;
                            text.VAlign = 0.5f;
                            onItemFilterToggle.Append(text);
                        }

                        affixSearch = new("Search affix...");
                        affixSearch.Top.Set(onItemFilterToggle.Top.Pixels + onItemFilterToggle.GetDimensions().Height + UICommon.spacing, 0);
                        affixSearch.MinWidth.Set(0, 1);
                        affixSearch.MinHeight.Set(25, 0);
                        affixSearch.OnTextChange += (object sender, EventArgs e) => FilterAffixList();
                        selectionHalf.Append(affixSearch);

                        UIScrollbar affixListScrollbar = new();
                        affixListScrollbar.Top.Set(affixSearch.Top.Pixels + affixSearch.GetDimensions().Height + UICommon.spacing, 0);
                        affixListScrollbar.Left.Set(-affixListScrollbar.Width.Pixels, 1f);
                        affixListScrollbar.MinHeight.Set(300, 0);
                        selectionHalf.Append(affixListScrollbar);

                        affixList = new();
                        affixList.Top.Set(affixListScrollbar.Top.Pixels, 0);
                        affixList.MinWidth.Set(-affixListScrollbar.Width.Pixels, 1);
                        affixList.MinHeight.Set(affixListScrollbar.MinHeight.Pixels, 0);
                        selectionHalf.Append(affixList);
                        affixList.SetScrollbar(affixListScrollbar);
                    }

                    UIElement affixHalf = new();
                    affixHalf.Left.Set(0, 0.5f);
                    affixHalf.MinWidth.Set(0, 0.5f);
                    affixHalf.MinHeight.Set(0, 1);
                    content.Append(affixHalf);
                    {
                        affixName = new("Affix", UICommon.textBig);
                        affixName.IgnoresMouseInteraction = true;
                        affixHalf.Append(affixName);

                        affixUIParent = new();
                        affixUIParent.Top.Set(affixName.Top.Pixels + affixName.GetDimensions().Height + UICommon.spacing, 0f);
                        affixUIParent.MinWidth.Set(0f, 1f);
                        affixUIParent.MinHeight.Set(500f, 0f);
                        affixHalf.Append(affixUIParent);
                        {
                            affixEditingUI = null;
                        }
                    }
                }
			}
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            HeldItem = Main.player[Main.myPlayer].HeldItem;
        }

        public void RecreateAffixElements()
        {
            var allLoadedAffixes = DataManager.Item.GetAllAffixesRef();
            allAffixes = new AffixListEntry[allLoadedAffixes.Length];
            for (int i = 0; i < allLoadedAffixes.Length; i++)
            {
                var affix = allLoadedAffixes[i];

                var entry = new AffixListEntry(affix);

                entry.selectToggle.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
                {
                    if (entry.selectToggle.IsOn)
                    {
                        if (HeldItem.TryGetGlobalItem<ItemItem>(out var modItem))
                        {
                            if (!IsAffixOnItem(affix, modItem, out var itemAffix))
                            {
                                itemAffix = entry.affix.Clone();
                                modItem.TryAddAffix(itemAffix, HeldItem);
                                FilterAffixList();
                            }

                            entry.selectToggle.SetState(true);
                            SelectedAffix = new SelectedDebugAffix { affixElement = entry, itemAffix = itemAffix };
                        }
                    }
                    else
                    {
                        entry.selectToggle.SetState(true);
                    }
                };
                entry.removeButton.OnClick += delegate (UIMouseEvent evt, UIElement listeningElement)
                {
                    if (HeldItem.TryGetGlobalItem<ItemItem>(out var modItem))
                    {
                        if (IsAffixOnItem(entry.affix, modItem, out var itemAffix))
                        {
                            modItem.RemoveAffix(itemAffix, HeldItem);
                            bool selectAfterFilter = entry == SelectedAffix.affixElement;
                            FilterAffixList(selectAfterFilter);
                        }
                    }
                };
                entry.removeButton.OnMouseUp += (UIMouseEvent evt, UIElement listeningElement) => panel.MouseUp(evt);

                allAffixes[i] = entry;
            }
            FilterAffixList();
        }

        void FilterAffixList(bool selectFirstAffixOnItem = false)
        {
            affixList.Clear();
            if (HeldItem.TryGetGlobalItem<ItemItem>(out var modItem))
            {
                var displayedAffixes = allAffixes
                    .Where(delegate (AffixListEntry entry)
                    {
                        bool onItem = IsAffixOnItem(entry.affix, modItem, out _);
                        return entry.affix.CanRoll(modItem, HeldItem)
                            && (onItem ||
                                (!onItemFilterToggle.IsOn && entry.affix.AffixSpaceAvailable(modItem)))
                            && (string.IsNullOrEmpty(affixSearch.CurrentString)
                                || entry.affix.GetType().Name.Contains(affixSearch.CurrentString, StringComparison.CurrentCultureIgnoreCase));
                    }).OrderBy((entry) => entry.affix.GetType().Name).ToArray();

                if (selectFirstAffixOnItem)
                    SelectedAffix = null;

                foreach (var entry in displayedAffixes)
                {
                    bool onItem = IsAffixOnItem(entry.affix, modItem, out var itemAffix);

                    entry.selectToggle.SetState(false);

                    if (onItem)
                    {
                        entry.ShowRemoveButton();

                        if (selectFirstAffixOnItem && SelectedAffix == null)
                        {
                            entry.selectToggle.SetState(true);
                            SelectedAffix = new SelectedDebugAffix { affixElement = entry, itemAffix = itemAffix }; ;
                        }
                    }
                    else
                    {
                        entry.HideRemoveButton();
                    }
                }

                affixList.AddRange(displayedAffixes);
            }
        }

        bool IsAffixOnItem(Affix affix, ItemItem item, out Affix itemAffix)
        {
            itemAffix = item.affixes.FirstOrDefault((Affix affix2) => affix.GetType() == affix2.GetType());
            return itemAffix != null;
        }
    }
}
