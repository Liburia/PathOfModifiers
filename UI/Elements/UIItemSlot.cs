using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI.Elements
{
	internal class UIItemSlot : UIElement
    {
        public delegate bool CheckDelegate(Item item);
        public delegate void ChangedDelegate(Item item);


        protected Asset<Texture2D> _texture;
        protected float _scale = 1f;

        Item _item;
        public Item Item {
            get => _item;
            private set
            {
                _item = value ?? new Item();
            }
        }

        public CheckDelegate CanInsertItem;
        public event ChangedDelegate OnItemChanged;
        public event Action<SpriteBatch, Vector2, float> OnDrawGhostItem;

        public UIItemSlot()
        {
            Item = new Item();
            _texture = Terraria.GameContent.TextureAssets.InventoryBack9;
            CanInsertItem = (Item _) => true;
        }

        public override void Recalculate()
        {
            base.Recalculate();
            _scale = GetDimensions().Width / _texture.Width();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var dims = GetDimensions();
            Rectangle dest = dims.ToRectangle();

            spriteBatch.Draw(_texture.Value, dest, Color.White);

            Vector2 position = dims.Center();

            if (Item.IsAir)
            {
                OnDrawGhostItem?.Invoke(spriteBatch, position, _scale);
            }
            else
            {
                var itemTexture = PoMUtil.GetItemTexture(Item.type).Value;
                Rectangle itemsource = (Main.itemAnimations[Item.type] == null) ? itemTexture.Frame() : Main.itemAnimations[Item.type].GetFrame(itemTexture);

                Color newColor = Color.White;
                float pulseScale = 1f;
                ItemSlot.GetItemLight(ref newColor, ref pulseScale, Item, false);
                int height = itemsource.Height;
                int width = itemsource.Width;
                float drawScale = 1f;
                float availableWidth = (float)_texture.Width() * _scale;
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
                drawScale *= _scale;
                Vector2 vector = _texture.Size() * _scale;
                Vector2 position2 = dims.Position() + vector / 2f - itemsource.Size() * drawScale / 2f;
                Vector2 origin = itemsource.Size() * (pulseScale / 2f - 0.5f);

                if (ItemLoader.PreDrawInInventory(Item, spriteBatch, position2, itemsource, Item.GetAlpha(newColor), Item.GetColor(Color.White), origin, drawScale * pulseScale))
                {
                    spriteBatch.Draw(itemTexture, position2, new Rectangle?(itemsource), Item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
                    if (Item.color != Color.Transparent)
                    {
                        spriteBatch.Draw(itemTexture, position2, new Rectangle?(itemsource), Item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
                    }
                }
                ItemLoader.PostDrawInInventory(Item, spriteBatch, position2, itemsource, Item.GetAlpha(newColor),
                    Item.GetColor(Color.White), origin, drawScale * pulseScale);
                if (ItemID.Sets.TrapSigned[Item.type])
                {
                    spriteBatch.Draw(Terraria.GameContent.TextureAssets.Wire.Value, dims.Position() + new Vector2(40f, 40f) * _scale, new Rectangle?(new Rectangle(4, 58, 8, 8)), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
                }
                if (Item.stack > 1)
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Terraria.GameContent.FontAssets.ItemStack.Value, Item.stack.ToString(), dims.Position() + new Vector2(10f, 26f) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);
                }

                if (IsMouseHovering)
                {
                    Main.HoverItem = Item.Clone();
                    Main.hoverItemName = Main.HoverItem.Name;
                }
            }
        }

        public override void Click(UIMouseEvent evt)
        {
            base.Click(evt);

            if (TryInsertItem(Main.mouseItem, true, out var oldItem))
            {
                Main.mouseItem = oldItem;
            }
        }

        /// <param name="item">Item to insert</param>
        /// <param name="oldItem">Item that was replaced if successful</param>
        /// <returns>If the insertion was successful</returns>
        public bool TryInsertItem(Item item, bool playSound, out Item oldItem)
        {
            if (!(Item.IsAir && item.IsAir) && (item.IsAir || CanInsertItem(item)))
            {
                oldItem = Item;
                Item = item;

                OnItemChanged?.Invoke(item);

                if (playSound)
                    SoundEngine.PlaySound(SoundID.Grab);

                return true;
            }

            oldItem = null;
            return false;
        }
        public void SetItem(Item item)
        {
            Item = item;
        }
    }
}
