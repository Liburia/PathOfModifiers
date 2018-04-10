using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI
{
    class UIItemSlot : UIElement
    {
        public static Texture2D defaultBackgroundTexture = Main.inventoryBack9Texture;
        public Texture2D backgroundTexture = defaultBackgroundTexture;
        internal float scale = .75f;
        public Item item;

        internal event Action<Item, Item> OnItemChange;
        internal event Func<Item, bool> CanPutIntoSlot;

        public UIItemSlot(Item item, Texture2D backgroundTexture = null, float scale = 1)
        {
            this.scale = scale;
            this.item = item;
            this.Width.Set(defaultBackgroundTexture.Width * scale, 0f);
            this.Height.Set(defaultBackgroundTexture.Height * scale, 0f);
            if (backgroundTexture == null)
                this.backgroundTexture = defaultBackgroundTexture;

            OnMouseDown += MouseItemDown;
            CanPutIntoSlot = delegate (Item delItem) { return true; };
        }

        internal int frameCounter = 0;
        internal int frameTimer = 0;
        private const int frameDelay = 7;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (item != null)
            {
                CalculatedStyle dimensions = base.GetInnerDimensions();
                Rectangle rectangle = dimensions.ToRectangle();
                spriteBatch.Draw(backgroundTexture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                if (!item.IsAir)
                {
                    Texture2D itemTexture = Main.itemTexture[this.item.type];
                    Rectangle rectangle2;
                    if (Main.itemAnimations[item.type] != null)
                    {
                        rectangle2 = Main.itemAnimations[item.type].GetFrame(itemTexture);
                    }
                    else
                    {
                        rectangle2 = itemTexture.Frame(1, 1, 0, 0);
                    }
                    Color newColor = Color.White;
                    float pulseScale = 1f;
                    ItemSlot.GetItemLight(ref newColor, ref pulseScale, item, false);
                    int height = rectangle2.Height;
                    int width = rectangle2.Width;
                    float drawScale = 1f;
                    float availableWidth = (float)backgroundTexture.Width * scale;
                    if (width > availableWidth || height > availableWidth)
                    {
                        if (width > height)
                        {
                            drawScale = availableWidth / width;
                        }
                        else
                        {
                            drawScale = availableWidth / height;
                        }
                    }
                    drawScale *= scale;
                    Vector2 vector = backgroundTexture.Size() * scale;
                    Vector2 position2 = dimensions.Position() + vector / 2f - rectangle2.Size() * drawScale / 2f;
                    Vector2 origin = rectangle2.Size() * (pulseScale / 2f - 0.5f);

                    if (ItemLoader.PreDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(newColor), item.GetColor(Color.White), origin, drawScale * pulseScale))
                    {
                        spriteBatch.Draw(itemTexture, position2, new Rectangle?(rectangle2), item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
                        if (item.color != Color.Transparent)
                        {
                            spriteBatch.Draw(itemTexture, position2, new Rectangle?(rectangle2), item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
                        }
                    }
                    ItemLoader.PostDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(newColor),
                        item.GetColor(Color.White), origin, drawScale * pulseScale);
                    if (ItemID.Sets.TrapSigned[item.type])
                    {
                        spriteBatch.Draw(Main.wireTexture, dimensions.Position() + new Vector2(40f, 40f) * scale, new Rectangle?(new Rectangle(4, 58, 8, 8)), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
                    }
                    if (item.stack > 1)
                    {
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, item.stack.ToString(), dimensions.Position() + new Vector2(10f, 26f) * scale, Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
                    }

                    if (IsMouseHovering)
                    {
                        // TODO, should only need 2 of these 3 I think
                        Main.HoverItem = item.Clone();
                        Main.hoverItemName = Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.mod.Name + "]" : "");

                        //	Main.hoverItemName = this.item.name;
                        //	Main.toolTip = item.Clone();
                        Main.HoverItem.SetNameOverride(Main.HoverItem.Name + (Main.HoverItem.modItem != null ? " [" + Main.HoverItem.modItem.mod.Name + "]" : ""));
                    }
                }
            }
        }

        void MouseItemDown(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.LocalPlayer;
            if ((!item.IsAir || (!Main.mouseItem.IsAir && CanPutIntoSlot(Main.mouseItem))) && player.itemAnimation == 0 && player.itemTime == 0)
            {
                Item tempItem = Main.mouseItem.Clone();
                Main.mouseItem = item.Clone();
                if (!Main.mouseItem.IsAir)
                {
                    Main.playerInventory = true;
                }
                item = tempItem;
                Main.PlaySound(SoundID.Grab, Main.LocalPlayer.position);
                OnItemChange?.Invoke(Main.mouseItem, item);
            }
        }
        public override void Click(UIMouseEvent evt)
        {
        }
    }
}