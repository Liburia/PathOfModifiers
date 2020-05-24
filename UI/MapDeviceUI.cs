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
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.ModNet.PacketHandlers;

namespace PathOfModifiers.UI
{
    class MapDeviceUI : UIState
    {
        public static MapDeviceUI Instance { get; set; }

        public static void ShowUI(MapDeviceTE md)
        {
            if (MapDevice.activeMD != null)
                Main.PlaySound(SoundID.MenuTick);
            else
                Main.PlaySound(SoundID.MenuOpen);
            MapDevice.activeMD = md;
            Main.playerInventory = true;
            Instance.IsVisible = true;
            Instance.UpdateText();
        }
        public static void HideUI()
        {
            if (MapDevice.activeMD != null)
                Main.PlaySound(SoundID.MenuClose);
            MapDevice.activeMD = null;
            Instance.IsVisible = false;
        }

        public UIPanel mapDevicePanel;
        public UIItemSlot mapSlot;
        public UIText timeLeftText;

        bool isVisible = false;
        public bool IsVisible
        {
            get { return isVisible; }
            private set
            {
                isVisible = value;
                if (isVisible)
                {
                    if (MapDevice.activeMD != null)
                    {
                        SetItemSlot(MapDevice.activeMD.mapItem.Clone());
                    }
                    PathOfModifiers.mapDeviceUI.SetState(this);
                }
                else
                {
                    SetItemSlot(new Item());
                    PathOfModifiers.mapDeviceUI.SetState(null);
                }
            }
        }

        public bool IsLocked => MapDevice.activeMD.timeLeft > 0;


        public Vector2 position = new Vector2(100, 400);

        public override void OnInitialize()
        {
            Instance = this;

            #region Panel definition
            mapDevicePanel = new UIPanel();
            mapDevicePanel.SetPadding(0);
            mapDevicePanel.Left.Set(position.X, 0f);
            mapDevicePanel.Top.Set(position.Y, 0f);
            mapDevicePanel.Width.Set(UIItemSlot.defaultBackgroundTexture.Width + (10 * 4) + (100 * 2), 0f);
            mapDevicePanel.Height.Set(UIItemSlot.defaultBackgroundTexture.Height + 30 + (10 * 2) + 45, 0f);
            mapDevicePanel.BackgroundColor = new Color(73, 94, 171);

            mapDevicePanel.OnMouseDown += new MouseEvent(DragStart);
            mapDevicePanel.OnMouseUp += new MouseEvent(DragEnd);
            #endregion
            #region Close button
            UIImageButton closeButton = new UIImageButton(ModContent.GetTexture("PathOfModifiers/UI/CloseButton"));
            closeButton.Left.Set(mapDevicePanel.Width.Pixels - 26, 0);
            closeButton.Top.Set(10, 0);
            closeButton.OnClick += OnCloseButtonClicked;
            mapDevicePanel.Append(closeButton);
            #endregion
            #region Map slot
            mapSlot = new UIItemSlot(new Item(), null, 1);
            mapSlot.Left.Set(10, 0f);
            mapSlot.Top.Set(30, 0f);
            mapSlot.CheckCanPutIntoSlot += MapCanPutIntoSlot;
            mapSlot.CheckIsLocked += delegate (ref bool isLocked) { if (!isLocked) isLocked = IsLocked; };
            mapSlot.OnItemChange += MapItemChanged;
            mapSlot.OnItemChange += OnSlotItemChange;
            mapDevicePanel.Append(mapSlot);
            #endregion
            #region Action buttons
            UITextButton beginButton = new UITextButton("Begin", Color.White);
            beginButton.Left.Set(UIItemSlot.defaultBackgroundTexture.Width + (10 * 2), 0f);
            beginButton.Top.Set(30, 0f);
            beginButton.Width.Set(100, 0f);
            beginButton.Height.Set(UIItemSlot.defaultBackgroundTexture.Height, 0f);
            beginButton.OnClick += new MouseEvent(BeginButtonClicked);
            mapDevicePanel.Append(beginButton);
            UITextButton endButton = new UITextButton("End", Color.White);
            endButton.Left.Set(UIItemSlot.defaultBackgroundTexture.Width + (10 * 3) + (100 * 1), 0f);
            endButton.Top.Set(30, 0f);
            endButton.Width.Set(100, 0f);
            endButton.Height.Set(UIItemSlot.defaultBackgroundTexture.Height, 0f);
            endButton.OnClick += new MouseEvent(EndButtonClicked);
            mapDevicePanel.Append(endButton);
            #endregion
            UIPanelConditioned beginConditionPanel = new UIPanelConditioned();
            beginConditionPanel.Left.Set(UIItemSlot.defaultBackgroundTexture.Width + (10 * 2), 0f);
            beginConditionPanel.Top.Set(30, 0f);
            beginConditionPanel.Width.Set(100, 0f);
            beginConditionPanel.Height.Set(UIItemSlot.defaultBackgroundTexture.Height, 0f);
            beginConditionPanel.drawableCondition = delegate () { return MapDevice.activeMD.mapItem.IsAir || MapDevice.activeMD.timeLeft > 0 || !(MapDevice.activeMD.mapItem.modItem is Items.Map); };
            mapDevicePanel.Append(beginConditionPanel);
            UIPanelConditioned endConditionPanel = new UIPanelConditioned();
            endConditionPanel.Left.Set(UIItemSlot.defaultBackgroundTexture.Width + (10 * 3) + (100 * 1), 0f);
            endConditionPanel.Top.Set(30, 0f);
            endConditionPanel.Width.Set(100, 0f);
            endConditionPanel.Height.Set(UIItemSlot.defaultBackgroundTexture.Height, 0f);
            endConditionPanel.drawableCondition = delegate () { return MapDevice.activeMD.timeLeft == 0; };
            mapDevicePanel.Append(endConditionPanel);

            UIText timeLeftLabelText = new UIText("Time Left", 0.6f, true);
            timeLeftLabelText.Left.Set(10, 0);
            timeLeftLabelText.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + 30 + (10 * 1) + 15, 0f);
            mapDevicePanel.Append(timeLeftLabelText);

            timeLeftText = new UIText(GetTimeLeftString(0), 1.2f, true);
            timeLeftText.Left.Set(150, 0);
            timeLeftText.Top.Set(UIItemSlot.defaultBackgroundTexture.Height + 30 + (10 * 1), 0f);
            mapDevicePanel.Append(timeLeftText);

            Append(mapDevicePanel);
        }

        void MapCanPutIntoSlot(Item item, ref bool canPut)
        {
            if (canPut)
                canPut = item.IsAir || PoMItem.IsMap(item);
        }
        void MapItemChanged(Item oldItem, Item newItem)
        {
            MapDevice.activeMD.mapItem = newItem.Clone();
            if (Main.netMode == NetmodeID.MultiplayerClient)
                MapPacketHandler.CMapDeviceMapItemChanged(MapDevice.activeMD.ID, MapDevice.activeMD.mapItem);
        }

        void BeginButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                MapDevice.activeMD.OpenMap();
            else if (Main.netMode == NetmodeID.MultiplayerClient)
                MapPacketHandler.CMapDeviceOpenMap(MapDevice.activeMD.ID);
            UpdateText();
        }
        void EndButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                MapDevice.activeMD.CloseMap();
            else if (Main.netMode == NetmodeID.MultiplayerClient)
                MapPacketHandler.CMapDeviceCloseMap(MapDevice.activeMD.ID);
            UpdateText();
        }
        void OnSlotItemChange(Item oldItem, Item newItem)
        {
        }
        void OnCloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            HideUI();
        }

        Vector2 offset;
        public bool dragging = false;
        void DragStart(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(evt.Target is UIImageButton) && !(evt.Target is UIPanelButton) && !(evt.Target is UIItemSlot))
            {
                offset = new Vector2(evt.MousePosition.X - mapDevicePanel.Left.Pixels, evt.MousePosition.Y - mapDevicePanel.Top.Pixels);
                dragging = true;
            }
        }
        void DragEnd(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(evt.Target is UIImageButton) && !(evt.Target is UIPanelButton) && !(evt.Target is UIItemSlot))
            {
                Vector2 end = evt.MousePosition;
                dragging = false;

                mapDevicePanel.Left.Set(end.X - offset.X, 0f);
                mapDevicePanel.Top.Set(end.Y - offset.Y, 0f);

                Recalculate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (mapDevicePanel.ContainsPoint(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (dragging)
            {
                mapDevicePanel.Left.Set(MousePosition.X - offset.X, 0f);
                mapDevicePanel.Top.Set(MousePosition.Y - offset.Y, 0f);
                Recalculate();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsVisible)
            {
                Player player = Main.LocalPlayer;
                Point16 playerPos = new Point16((int)(player.MountedCenter.X / 16), (int)(player.MountedCenter.Y / 16));
                //TODO: Don't hardcode TE size?
                if (playerPos.X < MapDevice.activeMD.Position.X - Player.tileRangeX || playerPos.X > MapDevice.activeMD.Position.X + Player.tileRangeX + 3 ||
                    playerPos.Y < MapDevice.activeMD.Position.Y - Player.tileRangeY || playerPos.Y > MapDevice.activeMD.Position.Y + Player.tileRangeY + 3)
                {
                    HideUI();
                }

            }
        }

        public void SetItemSlot(Item map)
        {
            mapSlot.item = map;
        }

        public string GetTimeLeftString(int ticks)
        {
            int seconds = ticks / 60;
            int minutes = seconds / 60;
            seconds %= 60;

            return $"{ (minutes > 9 ? string.Empty : "0") }{ minutes }:{(seconds > 9 ? string.Empty : "0")}{ seconds }";
        }

        public void UpdateText()
        {
            if (!IsVisible)
                return;

            timeLeftText.SetText(GetTimeLeftString(MapDevice.activeMD.timeLeft));
            timeLeftText.Recalculate();
        }
    }
}
