using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;
using PathOfModifiers.Tiles;
using Terraria.DataStructures;
using Terraria.Graphics;
using System.Collections.Generic;
using PathOfModifiers.Affixes;

namespace PathOfModifiers.UI
{
    class ModifierForgeUI : UIState
    {
        enum SelectedAction
        {
            None = -1,
            Reforge = 0,
            Rarify = 1,
            AddAffix = 2,
            AddPrefix = 3,
            AddSuffix = 4,
            RemoveAll = 5,
            RemovePrefixes = 6,
            RemoveSuffixes = 7,
            RollAffixes = 8,
            RollPrefixes = 9,
            RollSuffixes = 10,
        }

        public static ModifierForgeUI Instance { get; set; }

		public UIPanel modifierForgePanel;
        public UIItemSlot modifiedItemSlot;
        public UIItemSlot modifierItemSlot;
        public UIText modifierCostText;

        bool visible = false;
        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
                if (visible)
                {
                    if (ModifierForge.activeForge != null)
                    {
                        SetItemSlots(ModifierForge.activeForge.modifiedItem.Clone(), ModifierForge.activeForge.modifierItem.Clone());
                    }
                    PathOfModifiers.modifierForgeUI.SetState(this);
                }
                else
                {
                    SetItemSlots(new Item(), new Item());
                    PathOfModifiers.modifierForgeUI.SetState(null);
                }
            }
        }

        public Vector2 position = new Vector2(100, 400);

        SelectedAction selectedAction = SelectedAction.None;
        UIPanelButton[] toggleElements;

        UIText freeAffixCount;
        UIText freePrefixCount;
        UIText freeSuffixCount;
        UIText[] itemInfoText;

		public override void OnInitialize()
		{
            Instance = this;
            toggleElements = new UIPanelButton[11];
            itemInfoText = new UIText[9];

            #region Panel definition
            modifierForgePanel = new UIPanel();
			modifierForgePanel.SetPadding(0);
			modifierForgePanel.Left.Set(position.X, 0f);
			modifierForgePanel.Top.Set(position.Y, 0f);
			modifierForgePanel.Width.Set(500f, 0f);
            modifierForgePanel.Height.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 5) + (10 * 10) + 46, 0f);
            modifierForgePanel.BackgroundColor = new Color(73, 94, 171);

			modifierForgePanel.OnMouseDown += new MouseEvent(DragStart);
			modifierForgePanel.OnMouseUp += new MouseEvent(DragEnd);
            #endregion
            #region Item slots
            modifiedItemSlot = new UIItemSlot(new Item(), null, 1);
            modifiedItemSlot.Left.Set(10, 0f);
            modifiedItemSlot.Top.Set(10, 0f);
            modifiedItemSlot.CanPutIntoSlot += ModifiedCanPutIntoSlot;
            modifiedItemSlot.OnItemChange += ModifiedItemChange;
            modifiedItemSlot.OnItemChange += OnSlotItemChange;
            modifierForgePanel.Append(modifiedItemSlot);

            modifierItemSlot = new UIItemSlot(new Item(), null, 1);
            modifierItemSlot.Left.Set(UIItemSlot.defaultBackgroundTexture.Width + 20, 0f);
            modifierItemSlot.Top.Set(10, 0f);
            modifierItemSlot.CanPutIntoSlot += ModifierCanPutIntoSlot;
            modifierItemSlot.OnItemChange += ModifierItemChange;
            modifierItemSlot.OnItemChange += OnSlotItemChange;
            modifierForgePanel.Append(modifierItemSlot);
            #endregion
            #region Close button
            UIImageButton closeButton = new UIImageButton(ModLoader.GetTexture("PathOfModifiers/UI/CloseButton"));
            closeButton.Left.Set(474, 0);
            closeButton.Top.Set(10, 0);
            closeButton.OnClick += OnCloseButtonClicked;
            modifierForgePanel.Append(closeButton);
            #endregion
            #region Item info
            freeAffixCount = new UIText("[-]", 0.75f);
            freeAffixCount.Left.Set((UIItemSlot.defaultBackgroundTexture.Width * 2) + 30, 0);
            freeAffixCount.Top.Set(10, 0f);
            freeAffixCount.TextColor = PoMDataLoader.affixes[PoMDataLoader.affixMap[typeof(Affix)]].color;
            modifierForgePanel.Append(freeAffixCount);

            freePrefixCount = new UIText("[-]", 0.75f);
            freePrefixCount.Left.Set((UIItemSlot.defaultBackgroundTexture.Width * 2) + 30, 0);
            freePrefixCount.Top.Set(UIItemSlot.defaultBackgroundTexture.Height / 2 + 3, 0f);
            freePrefixCount.TextColor = PoMDataLoader.affixes[PoMDataLoader.affixMap[typeof(Prefix)]].color;
            modifierForgePanel.Append(freePrefixCount);

            freeSuffixCount = new UIText("[-]", 0.75f);
            freeSuffixCount.Left.Set((UIItemSlot.defaultBackgroundTexture.Width * 2) + 30, 0);
            freeSuffixCount.Top.Set(UIItemSlot.defaultBackgroundTexture.Height / 2 + 22, 0f);
            freeSuffixCount.TextColor = PoMDataLoader.affixes[PoMDataLoader.affixMap[typeof(Suffix)]].color;
            modifierForgePanel.Append(freeSuffixCount);

            for (int i = 0; i < itemInfoText.Length; i++)
            {
                UIText iItext = new UIText("TEST123", 0.75f);
                iItext.Left.Set((UIItemSlot.defaultBackgroundTexture.Width * 2) + 55, 0);
                iItext.Top.Set(10 + (i * 16), 0f);
                modifierForgePanel.Append(iItext);
                itemInfoText[i] = iItext;
            }
            #endregion
            #region Affix scope text
            UIText addText = new UIText("Add", 0.65f, true);
            addText.Left.Set(10, 0);
            addText.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            modifierForgePanel.Append(addText);
            UIText removeText = new UIText("Remove", 0.65f, true);
            removeText.Left.Set(10, 0);
            removeText.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            modifierForgePanel.Append(removeText);
            UIText rollText = new UIText("Roll", 0.65f, true);
            rollText.Left.Set(10, 0);
            rollText.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            modifierForgePanel.Append(rollText);
            #endregion
            #region Reforge/Enchance
            UITextButton reforgeToggle = new UITextButton("Reforge", Color.White);
            reforgeToggle.isToggle = true;
            reforgeToggle.Left.Set(10, 0f);
            reforgeToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + 20, 0f);
            reforgeToggle.Width.Set(UIItemSlot.defaultBackgroundTexture.Width * 2 + 10, 0f);
            reforgeToggle.Height.Set(32, 0f);
            reforgeToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(reforgeToggle);
            toggleElements[0] = reforgeToggle;

            UITextButton rarifyToggle = new UITextButton("Rarify", Color.White);
            rarifyToggle.isToggle = true;
            rarifyToggle.Left.Set(10, 0f);
            rarifyToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + 32 + 30, 0f);
            rarifyToggle.Width.Set(UIItemSlot.defaultBackgroundTexture.Width * 2 + 10, 0f);
            rarifyToggle.Height.Set(32, 0f);
            rarifyToggle.OnClick += new MouseEvent(ButtonToggled);
            rarifyToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.Rarify); };
            modifierForgePanel.Append(rarifyToggle);
            toggleElements[1] = rarifyToggle;
            #endregion
            #region Affix scope selectors
            UITextButton addAffixToggle = new UITextButton("Affix", Color.White);
            addAffixToggle.isToggle = true;
            addAffixToggle.Left.Set(140, 0f);
            addAffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            addAffixToggle.Width.Set(110, 0f);
            addAffixToggle.Height.Set(32, 0f);
            addAffixToggle.OnClick += new MouseEvent(ButtonToggled);
            addAffixToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.AddAffix); };
            modifierForgePanel.Append(addAffixToggle);
            toggleElements[2] = addAffixToggle;

            UITextButton addPrefixToggle = new UITextButton("Prefix", Color.White);
            addPrefixToggle.isToggle = true;
            addPrefixToggle.Left.Set(140 + ((110 + 10) * 1), 0f);
            addPrefixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            addPrefixToggle.Width.Set(110, 0f);
            addPrefixToggle.Height.Set(32, 0f);
            addPrefixToggle.OnClick += new MouseEvent(ButtonToggled);
            addPrefixToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.AddPrefix); };
            modifierForgePanel.Append(addPrefixToggle);
            toggleElements[3] = addPrefixToggle;

            UITextButton addSuffixToggle = new UITextButton("Suffix", Color.White);
            addSuffixToggle.isToggle = true;
            addSuffixToggle.Left.Set(140 + ((110 + 10) * 2), 0f);
            addSuffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            addSuffixToggle.Width.Set(110, 0f);
            addSuffixToggle.Height.Set(32, 0f);
            addSuffixToggle.OnClick += new MouseEvent(ButtonToggled);
            addSuffixToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.AddSuffix); };
            modifierForgePanel.Append(addSuffixToggle);
            toggleElements[4] = addSuffixToggle;


            UITextButton removeAllToggle = new UITextButton("All", Color.White);
            removeAllToggle.isToggle = true;
            removeAllToggle.Left.Set(140, 0f);
            removeAllToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removeAllToggle.Width.Set(110, 0f);
            removeAllToggle.Height.Set(32, 0f);
            removeAllToggle.OnClick += new MouseEvent(ButtonToggled);
            removeAllToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.RemoveAll); };
            modifierForgePanel.Append(removeAllToggle);
            toggleElements[5] = removeAllToggle;

            UITextButton removePrefixToggle = new UITextButton("Prefixes", Color.White);
            removePrefixToggle.isToggle = true;
            removePrefixToggle.Left.Set(140 + ((110 + 10) * 1), 0f);
            removePrefixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removePrefixToggle.Width.Set(110, 0f);
            removePrefixToggle.Height.Set(32, 0f);
            removePrefixToggle.OnClick += new MouseEvent(ButtonToggled);
            removePrefixToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.RemovePrefixes); };
            modifierForgePanel.Append(removePrefixToggle);
            toggleElements[6] = removePrefixToggle;

            UITextButton removeSuffixToggle = new UITextButton("Suffixes", Color.White);
            removeSuffixToggle.isToggle = true;
            removeSuffixToggle.Left.Set(140 + ((110 + 10) * 2), 0f);
            removeSuffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removeSuffixToggle.Width.Set(110, 0f);
            removeSuffixToggle.Height.Set(32, 0f);
            removeSuffixToggle.OnClick += new MouseEvent(ButtonToggled);
            removeSuffixToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.RemoveSuffixes); };
            modifierForgePanel.Append(removeSuffixToggle);
            toggleElements[7] = removeSuffixToggle;


            UITextButton rollAffixToggle = new UITextButton("Affix", Color.White);
            rollAffixToggle.isToggle = true;
            rollAffixToggle.Left.Set(140, 0f);
            rollAffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollAffixToggle.Width.Set(110, 0f);
            rollAffixToggle.Height.Set(32, 0f);
            rollAffixToggle.OnClick += new MouseEvent(ButtonToggled);
            rollAffixToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.RollAffixes); };
            modifierForgePanel.Append(rollAffixToggle);
            toggleElements[8] = rollAffixToggle;

            UITextButton rollPrefixToggle = new UITextButton("Prefix", Color.White);
            rollPrefixToggle.isToggle = true;
            rollPrefixToggle.Left.Set(140 + ((110 + 10) * 1), 0f);
            rollPrefixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollPrefixToggle.Width.Set(110, 0f);
            rollPrefixToggle.Height.Set(32, 0f);
            rollPrefixToggle.OnClick += new MouseEvent(ButtonToggled);
            rollPrefixToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.RollPrefixes); };
            modifierForgePanel.Append(rollPrefixToggle);
            toggleElements[9] = rollPrefixToggle;

            UITextButton rollSuffixToggle = new UITextButton("Suffix", Color.White);
            rollSuffixToggle.isToggle = true;
            rollSuffixToggle.Left.Set(140 + ((110 + 10) * 2), 0f);
            rollSuffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollSuffixToggle.Width.Set(110, 0f);
            rollSuffixToggle.Height.Set(32, 0f);
            rollSuffixToggle.OnClick += new MouseEvent(ButtonToggled);
            rollSuffixToggle.interactiveCondition = delegate () { return ActionCondition(SelectedAction.RollSuffixes); };
            modifierForgePanel.Append(rollSuffixToggle);
            toggleElements[10] = rollSuffixToggle;
            #endregion
            #region Button Unlock Conditions
            #region Main Actions
            UIPanelConditioned rarifyConditionPanel = new UIPanelConditioned();
            rarifyConditionPanel.Left.Set(10, 0f);
            rarifyConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + 32 + 30, 0f);
            rarifyConditionPanel.Width.Set(UIItemSlot.defaultBackgroundTexture.Width * 2 + 10, 0f);
            rarifyConditionPanel.Height.Set(32, 0f);
            rarifyConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.Rarify); };
            UIImage rarifyCondition = new UIImage(ActionConditionImage(SelectedAction.Rarify));
            rarifyCondition.Left.Set(-12 + UIItemSlot.defaultBackgroundTexture.Width + 5 - 13, 0);
            rarifyCondition.Top.Set(-12 + 16 - 15, 0);
            rarifyConditionPanel.Append(rarifyCondition);
            modifierForgePanel.Append(rarifyConditionPanel);
            #endregion
            #region Add
            UIPanelConditioned addAffixConditionPanel = new UIPanelConditioned();
            addAffixConditionPanel.Left.Set(140, 0f);
            addAffixConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            addAffixConditionPanel.Width.Set(110, 0f);
            addAffixConditionPanel.Height.Set(32, 0f);
            addAffixConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.AddAffix); };
            UIImage addAffixCondition = new UIImage(ActionConditionImage(SelectedAction.AddAffix));
            addAffixCondition.Left.Set(-12 + 42, 0);
            addAffixCondition.Top.Set(-12 + 2, 0);
            addAffixConditionPanel.Append(addAffixCondition);
            modifierForgePanel.Append(addAffixConditionPanel);

            UIPanelConditioned addPrefixConditionPanel = new UIPanelConditioned();
            addPrefixConditionPanel.Left.Set(140 + ((110 + 10) * 1), 0f);
            addPrefixConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            addPrefixConditionPanel.Width.Set(110, 0f);
            addPrefixConditionPanel.Height.Set(32, 0f);
            addPrefixConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.AddPrefix); };
            UIImage addPrefixCondition = new UIImage(ActionConditionImage(SelectedAction.AddPrefix));
            addPrefixCondition.Left.Set(-12 + 55 - 15, 0);
            addPrefixCondition.Top.Set(-12 + 16 - 15, 0);
            addPrefixConditionPanel.Append(addPrefixCondition);
            modifierForgePanel.Append(addPrefixConditionPanel);

            UIPanelConditioned addSuffixConditionPanel = new UIPanelConditioned();
            addSuffixConditionPanel.Left.Set(140 + ((110 + 10) * 2), 0f);
            addSuffixConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            addSuffixConditionPanel.Width.Set(110, 0f);
            addSuffixConditionPanel.Height.Set(32, 0f);
            addSuffixConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.AddSuffix); };
            UIImage addSuffixCondition = new UIImage(ActionConditionImage(SelectedAction.AddSuffix));
            addSuffixCondition.Left.Set(-12 + 55 - 15, 0);
            addSuffixCondition.Top.Set(-12 + 16 - 15, 0);
            addSuffixConditionPanel.Append(addSuffixCondition);
            modifierForgePanel.Append(addSuffixConditionPanel);
            #endregion
            #region Remove
            UIPanelConditioned removeAllConditionPanel = new UIPanelConditioned();
            removeAllConditionPanel.Left.Set(140, 0f);
            removeAllConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removeAllConditionPanel.Width.Set(110, 0f);
            removeAllConditionPanel.Height.Set(32, 0f);
            removeAllConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.RemoveAll); };
            UIImage removeAllCondition = new UIImage(ActionConditionImage(SelectedAction.RemoveAll));
            removeAllCondition.Left.Set(-12 + 55 - 13, 0);
            removeAllCondition.Top.Set(-12 + 16 - 15, 0);
            removeAllConditionPanel.Append(removeAllCondition);
            modifierForgePanel.Append(removeAllConditionPanel);

            UIPanelConditioned removePrefixesConditionPanel = new UIPanelConditioned();
            removePrefixesConditionPanel.Left.Set(140 + ((110 + 10) * 1), 0f);
            removePrefixesConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removePrefixesConditionPanel.Width.Set(110, 0f);
            removePrefixesConditionPanel.Height.Set(32, 0f);
            removePrefixesConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.RemovePrefixes); };
            UIImage removePrefixesCondition = new UIImage(ActionConditionImage(SelectedAction.RemovePrefixes));
            removePrefixesCondition.Left.Set(-12 + 55 - 15, 0);
            removePrefixesCondition.Top.Set(-12 + 16 - 15, 0);
            removePrefixesConditionPanel.Append(removePrefixesCondition);
            modifierForgePanel.Append(removePrefixesConditionPanel);

            UIPanelConditioned removeSuffixesConditionPanel = new UIPanelConditioned();
            removeSuffixesConditionPanel.Left.Set(140 + ((110 + 10) * 2), 0f);
            removeSuffixesConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removeSuffixesConditionPanel.Width.Set(110, 0f);
            removeSuffixesConditionPanel.Height.Set(32, 0f);
            removeSuffixesConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.RemoveSuffixes); };
            UIImage removeSuffixesCondition = new UIImage(ActionConditionImage(SelectedAction.RemoveSuffixes));
            removeSuffixesCondition.Left.Set(-12 + 55 - 15, 0);
            removeSuffixesCondition.Top.Set(-12 + 16 - 15, 0);
            removeSuffixesConditionPanel.Append(removeSuffixesCondition);
            modifierForgePanel.Append(removeSuffixesConditionPanel);
            #endregion
            #region Roll
            UIPanelConditioned rollAffixesConditionPanel = new UIPanelConditioned();
            rollAffixesConditionPanel.Left.Set(140, 0f);
            rollAffixesConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollAffixesConditionPanel.Width.Set(110, 0f);
            rollAffixesConditionPanel.Height.Set(32, 0f);
            rollAffixesConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.RollAffixes); };
            UIImage rollAffixesCondition = new UIImage(ActionConditionImage(SelectedAction.RollAffixes));
            rollAffixesCondition.Left.Set(-12 + 55 - 15, 0);
            rollAffixesCondition.Top.Set(-12 + 16 - 15, 0);
            rollAffixesConditionPanel.Append(rollAffixesCondition);
            modifierForgePanel.Append(rollAffixesConditionPanel);

            UIPanelConditioned rollPrefixesConditionPanel = new UIPanelConditioned();
            rollPrefixesConditionPanel.Left.Set(140 + ((110 + 10) * 1), 0f);
            rollPrefixesConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollPrefixesConditionPanel.Width.Set(110, 0f);
            rollPrefixesConditionPanel.Height.Set(32, 0f);
            rollPrefixesConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.RollPrefixes); };
            UIImage rollPrefixesCondition = new UIImage(ActionConditionImage(SelectedAction.RollPrefixes));
            rollPrefixesCondition.Left.Set(-12 + 55 - 15, 0);
            rollPrefixesCondition.Top.Set(-12 + 16 - 15, 0);
            rollPrefixesConditionPanel.Append(rollPrefixesCondition);
            modifierForgePanel.Append(rollPrefixesConditionPanel);

            UIPanelConditioned rollSuffixesConditionPanel = new UIPanelConditioned();
            rollSuffixesConditionPanel.Left.Set(140 + ((110 + 10) * 2), 0f);
            rollSuffixesConditionPanel.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollSuffixesConditionPanel.Width.Set(110, 0f);
            rollSuffixesConditionPanel.Height.Set(32, 0f);
            rollSuffixesConditionPanel.drawableCondition = delegate () { return !ActionCondition(SelectedAction.RollSuffixes); };
            UIImage rollSuffixesCondition = new UIImage(ActionConditionImage(SelectedAction.RollSuffixes));
            rollSuffixesCondition.Left.Set(-12 + 55 - 15, 0);
            rollSuffixesCondition.Top.Set(-12 + 16 - 15, 0);
            rollSuffixesConditionPanel.Append(rollSuffixesCondition);
            modifierForgePanel.Append(rollSuffixesConditionPanel);
            #endregion
            #endregion
            UITextButton forgeButton = new UITextButton("Forge", Color.White);
            forgeButton.Left.Set(280, 0f);
            forgeButton.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 5) + (10 * 9), 0f);
            forgeButton.Width.Set(190, 0f);
            forgeButton.Height.Set(46, 0f);
            forgeButton.OnClick += new MouseEvent(ForgeButtonClicked);
            modifierForgePanel.Append(forgeButton);

            UIText costText = new UIText("Cost:", 1, true);
            costText.Left.Set(50, 0);
            costText.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 5) + (10 * 9), 0f);
            modifierForgePanel.Append(costText);

            UIImage modifierCostImage = new UIImage(ModLoader.GetTexture("PathOfModifiers/Items/ModifierFragment"));
            modifierCostImage.Left.Set(160, 0);
            modifierCostImage.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 5) + (10 * 9), 0f);
            modifierForgePanel.Append(modifierCostImage);

            UIText modifierCostXText = new UIText("x", 0.8f, true);
            modifierCostXText.Left.Set(190, 0);
            modifierCostXText.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 5) + (10 * 9), 0f);
            modifierForgePanel.Append(modifierCostXText);

            modifierCostText = new UIText("0", 1, true);
            modifierCostText.Left.Set(210, 0);
            modifierCostText.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 5) + (10 * 9), 0f);
            modifierForgePanel.Append(modifierCostText);

            Append(modifierForgePanel);
		}

        bool ModifiedCanPutIntoSlot(Item item)
        {
            return PoMItem.IsRollable(item);
        }
        bool ModifierCanPutIntoSlot(Item item)
        {
            //TODO: Actual system
            return item.type == PathOfModifiers.Instance.ItemType<Items.ModifierFragment>();
        }
        void ModifiedItemChange(Item oldItem, Item newItem)
        {
            ModifierForge.activeForge.modifiedItem = newItem.Clone();
            ModifierForge.activeForge.Sync(ModifierForge.activeForge.ID);
        }
        void ModifierItemChange(Item oldItem, Item newItem)
        {
            ModifierForge.activeForge.modifierItem = newItem.Clone();
            ModifierForge.activeForge.Sync(ModifierForge.activeForge.ID);
        }

        void ButtonToggled(UIMouseEvent evt, UIElement listeningElement)
        {
            UIPanelButton element;
            for (int i = 0; i < toggleElements.Length; i++)
            {
                element = toggleElements[i];
                if (element == listeningElement)
                {
                    if (element.toggleState)
                    {
                        selectedAction = SelectedAction.None;
                    }
                    else
                    {
                        selectedAction = (SelectedAction)i;
                    }
                    UpdateText();
                }
                else
                    element.toggleState = false;
            }
        }
        bool ActionCondition(SelectedAction action)
        {
            if (action == SelectedAction.AddAffix)
                return NPC.downedBoss1;
            else if (action == SelectedAction.Rarify || action == SelectedAction.RemoveAll)
                return NPC.downedBoss3;
            else if (action == SelectedAction.AddPrefix || action == SelectedAction.AddSuffix || action == SelectedAction.RemovePrefixes || action == SelectedAction.RemoveSuffixes || action == SelectedAction.RollAffixes)
                return NPC.downedMechBossAny;
            else if (action == SelectedAction.RollPrefixes || action == SelectedAction.RollSuffixes)
                return NPC.downedPlantBoss;

            return true;
        }
        Texture2D ActionConditionImage(SelectedAction action)
        {
            if (action == SelectedAction.AddAffix)
                return TextureManager.Load("Images/NPC_Head_Boss_0");
            else if (action == SelectedAction.Rarify || action == SelectedAction.RemoveAll)
                return TextureManager.Load("Images/NPC_Head_Boss_19");
            else if (action == SelectedAction.AddPrefix || action == SelectedAction.AddSuffix || action == SelectedAction.RemovePrefixes || action == SelectedAction.RemoveSuffixes || action == SelectedAction.RollAffixes)
                return ModLoader.GetTexture("PathOfModifiers/UI/ActionConditionMechAny");
            else if (action == SelectedAction.RollPrefixes || action == SelectedAction.RollSuffixes)
                return TextureManager.Load("Images/NPC_Head_Boss_11");

            return ModLoader.GetTexture("PathOfModifiers/UI/ActionConditionUnknown");
        }
        void ForgeButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            switch (selectedAction)
            {
                case SelectedAction.Reforge:
                    ModifierForge.activeForge.RerollAffixes();
                    break;
                case SelectedAction.Rarify:
                    ModifierForge.activeForge.Rarify();
                    break;
                case SelectedAction.AddAffix:
                    ModifierForge.activeForge.AddAffix();
                    break;
                case SelectedAction.AddPrefix:
                    ModifierForge.activeForge.AddPrefix();
                    break;
                case SelectedAction.AddSuffix:
                    ModifierForge.activeForge.AddSuffix();
                    break;
                case SelectedAction.RemoveAll:
                    ModifierForge.activeForge.RemoveAll();
                    break;
                case SelectedAction.RemovePrefixes:
                    ModifierForge.activeForge.RemovePrefixes();
                    break;
                case SelectedAction.RemoveSuffixes:
                    ModifierForge.activeForge.RemoveSuffixes();
                    break;
                case SelectedAction.RollAffixes:
                    ModifierForge.activeForge.RollAffixes();
                    break;
                case SelectedAction.RollPrefixes:
                    ModifierForge.activeForge.RollPrefixes();
                    break;
                case SelectedAction.RollSuffixes:
                    ModifierForge.activeForge.RollSuffixes();
                    break;
            }
            UpdateText();
        }
        void OnSlotItemChange(Item oldItem, Item newItem)
        {
            UpdateText();
        }
        void OnCloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            ModifierForge.HideUI();
        }

        Vector2 offset;
		public bool dragging = false;
		void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(evt.Target is UIImageButton) && !(evt.Target is UIPanelButton) && !(evt.Target is UIItemSlot))
            {
                offset = new Vector2(evt.MousePosition.X - modifierForgePanel.Left.Pixels, evt.MousePosition.Y - modifierForgePanel.Top.Pixels);
                dragging = true;
            }
		}
		void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(evt.Target is UIImageButton) && !(evt.Target is UIPanelButton) && !(evt.Target is UIItemSlot))
            {
                Vector2 end = evt.MousePosition;
                dragging = false;

                modifierForgePanel.Left.Set(end.X - offset.X, 0f);
                modifierForgePanel.Top.Set(end.Y - offset.Y, 0f);

                Recalculate();
            }
		}

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (modifierForgePanel.ContainsPoint(MousePosition))
			{
				Main.LocalPlayer.mouseInterface = true;
			}
			if (dragging)
			{
				modifierForgePanel.Left.Set(MousePosition.X - offset.X, 0f);
				modifierForgePanel.Top.Set(MousePosition.Y - offset.Y, 0f);
				Recalculate();
			}
		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Visible)
            {
                Player player = Main.LocalPlayer;
                Point playerPos = new Point((int)(player.MountedCenter.X / 16), (int)(player.MountedCenter.Y / 16));
                if (playerPos.X < ModifierForge.activeForge.Position.X - Player.tileRangeX || playerPos.X > ModifierForge.activeForge.Position.X + Player.tileRangeX + 1 ||
                    playerPos.Y < ModifierForge.activeForge.Position.Y - Player.tileRangeY || playerPos.Y > ModifierForge.activeForge.Position.Y + Player.tileRangeY + 1)
                {
                    ModifierForge.HideUI();
                }
            }
        }

        public void SetItemSlots(Item modifiedItem, Item modifierItem)
        {
            modifiedItemSlot.item = modifiedItem;
            modifierItemSlot.item = modifierItem;
            UpdateText();
        }
        /// <summary>
        /// Update any text that depends on the item stats
        /// </summary>
        public void UpdateText()
        {
            if (!Visible)
                return;

            if (selectedAction == SelectedAction.None)
                modifierCostText.SetText("0");
            else
                modifierCostText.SetText(ModifierForge.activeForge.CalculateCost((TEModifierForge.ForgeAction)selectedAction).ToString());
            modifierCostText.Recalculate();

            int i = 0;
            if (!modifiedItemSlot.item.IsAir)
            {
                PoMItem pomItem = modifiedItemSlot.item.GetGlobalItem<PoMItem>();

                freeAffixCount.SetText($"[{pomItem.FreeAffixes}]");
                freePrefixCount.SetText($"[{pomItem.FreePrefixes}]");
                freeSuffixCount.SetText($"[{pomItem.FreeSuffixes}]");

                foreach (Prefix prefix in pomItem.prefixes)
                {
                    if (i >= itemInfoText.Length)
                        break;
                    UIText iiText = itemInfoText[i];
                    iiText.SetText(prefix.GetTolltipText(modifiedItemSlot.item));
                    iiText.TextColor = prefix.color;
                    iiText.Recalculate();
                    i++;
                }
                foreach (Suffix suffix in pomItem.suffixes)
                {
                    if (i >= itemInfoText.Length)
                        break;
                    UIText iiText = itemInfoText[i];
                    iiText.SetText(suffix.GetTolltipText(modifiedItemSlot.item));
                    iiText.TextColor = suffix.color;
                    iiText.Recalculate();
                    i++;
                }
            }
            else
            {
                freeAffixCount.SetText($"[-]");
                freePrefixCount.SetText($"[-]");
                freeSuffixCount.SetText($"[-]");
            }
            for (; i < itemInfoText.Length; i++)
            {
                itemInfoText[i].SetText(string.Empty);
                itemInfoText[i].Recalculate();
            }
        }
    }
}
