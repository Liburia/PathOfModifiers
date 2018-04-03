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

namespace PathOfModifiers.UI
{
    class ModifierForgeUI : UIState
    {
        public static ModifierForgeUI Instance { get; set; }

		public UIPanel modifierForgePanel;
        public UIItemSlot modifiedItemSlot;
        public UIItemSlot modifierItemSlot;

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
                }
                else
                {
                    SetItemSlots(new Item(), new Item());
                }
            }
        }

        public Vector2 position = new Vector2(100, 400);

		public override void OnInitialize()
		{
            Instance = this;
            
            modifierForgePanel = new UIPanel();
			modifierForgePanel.SetPadding(0);
			modifierForgePanel.Left.Set(position.X, 0f);
			modifierForgePanel.Top.Set(position.Y, 0f);
			modifierForgePanel.Width.Set(200f, 0f);
			modifierForgePanel.Height.Set(70f, 0f);
			modifierForgePanel.BackgroundColor = new Color(73, 94, 171);

			modifierForgePanel.OnMouseDown += new MouseEvent(DragStart);
			modifierForgePanel.OnMouseUp += new MouseEvent(DragEnd);
            
            modifiedItemSlot = new UIItemSlot(new Item(), null, 1);
            modifiedItemSlot.Left.Set(10, 0f);
            modifiedItemSlot.Top.Set(10, 0f);
            modifiedItemSlot.CanPutIntoSlot += ModifiedCanPutIntoSlot;
            modifiedItemSlot.OnItemChange += ModifiedItemChange;
            modifierForgePanel.Append(modifiedItemSlot);

            modifierItemSlot = new UIItemSlot(new Item(), null, 1);
            modifierItemSlot.Left.Set(UIItemSlot.defaultBackgroundTexture.Width + 20, 0f);
            modifierItemSlot.Top.Set(10, 0f);
            modifierItemSlot.CanPutIntoSlot += ModifierCanPutIntoSlot;
            modifierItemSlot.OnItemChange += ModifierItemChange;
            modifierForgePanel.Append(modifierItemSlot);

            Texture2D buttonDeleteTexture = ModLoader.GetTexture("Terraria/UI/Reforge_0");
			UIImageButton reforgeButton = new UIImageButton(buttonDeleteTexture);
            reforgeButton.Left.Set(140, 0f);
            reforgeButton.Top.Set(10 + (UIItemSlot.defaultBackgroundTexture.Height / 2 - buttonDeleteTexture.Height / 2), 0f);
            reforgeButton.Width.Set(50, 0f);
            reforgeButton.Height.Set(50, 0f);
            reforgeButton.OnClick += new MouseEvent(ReforgeButtonClicked);
			modifierForgePanel.Append(reforgeButton);
            
			base.Append(modifierForgePanel);
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

        void ReforgeButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
            ModifierForge.activeForge.Reforge();
		}

		Vector2 offset;
		public bool dragging = false;
		private void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            if (evt.Target == modifierForgePanel)
            {
                offset = new Vector2(evt.MousePosition.X - modifierForgePanel.Left.Pixels, evt.MousePosition.Y - modifierForgePanel.Top.Pixels);
                dragging = true;
            }
		}
		private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
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
        }
    }
}
