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

		public override void OnInitialize()
		{
            Instance = this;
            toggleElements = new UIPanelButton[11];

            modifierForgePanel = new UIPanel();
			modifierForgePanel.SetPadding(0);
			modifierForgePanel.Left.Set(position.X, 0f);
			modifierForgePanel.Top.Set(position.Y, 0f);
			modifierForgePanel.Width.Set(500f, 0f);
            modifierForgePanel.Height.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 5) + (10 * 10) + 46, 0f);
            modifierForgePanel.BackgroundColor = new Color(73, 94, 171);

			modifierForgePanel.OnMouseDown += new MouseEvent(DragStart);
			modifierForgePanel.OnMouseUp += new MouseEvent(DragEnd);

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
            modifierForgePanel.Append(addAffixToggle);
            toggleElements[2] = addAffixToggle;

            UITextButton addPrefixToggle = new UITextButton("Prefix", Color.White);
            addPrefixToggle.isToggle = true;
            addPrefixToggle.Left.Set(140 + ((110 + 10) * 1), 0f);
            addPrefixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            addPrefixToggle.Width.Set(110, 0f);
            addPrefixToggle.Height.Set(32, 0f);
            addPrefixToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(addPrefixToggle);
            toggleElements[3] = addPrefixToggle;

            UITextButton addSuffixToggle = new UITextButton("Suffix", Color.White);
            addSuffixToggle.isToggle = true;
            addSuffixToggle.Left.Set(140 + ((110 + 10) * 2), 0f);
            addSuffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 2) + (10 * 4), 0f);
            addSuffixToggle.Width.Set(110, 0f);
            addSuffixToggle.Height.Set(32, 0f);
            addSuffixToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(addSuffixToggle);
            toggleElements[4] = addSuffixToggle;


            UITextButton removeAllToggle = new UITextButton("All", Color.White);
            removeAllToggle.isToggle = true;
            removeAllToggle.Left.Set(140, 0f);
            removeAllToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removeAllToggle.Width.Set(110, 0f);
            removeAllToggle.Height.Set(32, 0f);
            removeAllToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(removeAllToggle);
            toggleElements[5] = removeAllToggle;

            UITextButton removePrefixToggle = new UITextButton("Prefixes", Color.White);
            removePrefixToggle.isToggle = true;
            removePrefixToggle.Left.Set(140 + ((110 + 10) * 1), 0f);
            removePrefixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removePrefixToggle.Width.Set(110, 0f);
            removePrefixToggle.Height.Set(32, 0f);
            removePrefixToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(removePrefixToggle);
            toggleElements[6] = removePrefixToggle;

            UITextButton removeSuffixToggle = new UITextButton("Suffixes", Color.White);
            removeSuffixToggle.isToggle = true;
            removeSuffixToggle.Left.Set(140 + ((110 + 10) * 2), 0f);
            removeSuffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 3) + (10 * 5), 0f);
            removeSuffixToggle.Width.Set(110, 0f);
            removeSuffixToggle.Height.Set(32, 0f);
            removeSuffixToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(removeSuffixToggle);
            toggleElements[7] = removeSuffixToggle;


            UITextButton rollAffixToggle = new UITextButton("Affix", Color.White);
            rollAffixToggle.isToggle = true;
            rollAffixToggle.Left.Set(140, 0f);
            rollAffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollAffixToggle.Width.Set(110, 0f);
            rollAffixToggle.Height.Set(32, 0f);
            rollAffixToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(rollAffixToggle);
            toggleElements[8] = rollAffixToggle;

            UITextButton rollPrefixToggle = new UITextButton("Prefix", Color.White);
            rollPrefixToggle.isToggle = true;
            rollPrefixToggle.Left.Set(140 + ((110 + 10) * 1), 0f);
            rollPrefixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollPrefixToggle.Width.Set(110, 0f);
            rollPrefixToggle.Height.Set(32, 0f);
            rollPrefixToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(rollPrefixToggle);
            toggleElements[9] = rollPrefixToggle;

            UITextButton rollSuffixToggle = new UITextButton("Suffix", Color.White);
            rollSuffixToggle.isToggle = true;
            rollSuffixToggle.Left.Set(140 + ((110 + 10) * 2), 0f);
            rollSuffixToggle.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + (32 * 4) + (10 * 6), 0f);
            rollSuffixToggle.Width.Set(110, 0f);
            rollSuffixToggle.Height.Set(32, 0f);
            rollSuffixToggle.OnClick += new MouseEvent(ButtonToggled);
            modifierForgePanel.Append(rollSuffixToggle);
            toggleElements[10] = rollSuffixToggle;
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
                    UpdateCost();
                }
                else
                    element.toggleState = false;
            }
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
            UpdateCost();
        }
        void OnSlotItemChange(Item oldItem, Item newItem)
        {
            UpdateCost();
        }

        Vector2 offset;
		public bool dragging = false;
		void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            if (evt.Target == modifierForgePanel)
            {
                offset = new Vector2(evt.MousePosition.X - modifierForgePanel.Left.Pixels, evt.MousePosition.Y - modifierForgePanel.Top.Pixels);
                dragging = true;
            }
		}
		void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            if (evt.Target == modifierForgePanel)
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
            UpdateCost();
        }
        public void UpdateCost()
        {
            if (selectedAction == SelectedAction.None)
                modifierCostText.SetText("0");
            else
                modifierCostText.SetText(ModifierForge.activeForge.CalculateCost((TEModifierForge.ForgeAction)selectedAction).ToString());
        }
    }
}
