using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;
using PathOfModifiers.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PathOfModifiers.UI
{
	class DebugUIState : UIState
	{
		public override void OnInitialize()
		{
			UIDraggablePanel panel = new();
			panel.Left.Set(200, 0);
			panel.Top.Set(100, 0);
			panel.Width.Set(600, 0);
			panel.Height.Set(400, 0);
			Append(panel);
			{
                UIText title = new("PoM Debug", 0.7f);
                title.IgnoresMouseInteraction = true;
                title.Top.Set(0, 0);
                title.Height.Set(20, 0);
                title.HAlign = 0.5f;
                panel.Append(title);

                UIImageButton closePanelX = new(ModContent.Request<Texture2D>(PoMGlobals.Path.Image.UI.CloseButton));
				closePanelX.Top.Set(0, 0);
				closePanelX.Left.Set(550, 0);
				closePanelX.OnClick += (UIMouseEvent evt, UIElement listeningElement) => Systems.UI.ToggleDebugPanel();
				panel.Append(closePanelX);

                UIElement content = new();
                content.Top.Set(title.Top.Pixels + title.Height.Pixels + UICommon.spacing, 0);
                content.Width.Set(0, 1);
                content.Height.Set(0, 1);
                panel.Append(content);
                {
                    UIElement selectionHalf = new();
                    selectionHalf.Width.Set(0, 0.5f);
                    selectionHalf.Height.Set(0, 1);
                    content.Append(selectionHalf);
                    {
                        UIToggle itemFilterToggle = new();
                        itemFilterToggle.Top.Set(0, 0);
                        itemFilterToggle.Width.Set(0, 0.2f);
                        itemFilterToggle.Height.Set(30, 0);
                        selectionHalf.Append(itemFilterToggle);
                        {
                            UIText text = new("Item");
                            text.IgnoresMouseInteraction = true;
                            text.HAlign = 0.5f;
                            text.VAlign = 0.5f;
                            itemFilterToggle.Append(text);
                        }
                        //TODO: on toggle filter list

                        UIFocusInputTextField search = new("Search modifier...");
                        search.Top.Set(itemFilterToggle.Top.Pixels + itemFilterToggle.Height.Pixels + UICommon.spacing, 0);
                        search.Width.Set(0, 1);
                        search.Height.Set(30, 0);
                        selectionHalf.Append(search);
                        //TODO: on input filter list

                        UIList modifierList = new();
                        modifierList.Top.Set(search.Top.Pixels + search.Height.Pixels + UICommon.spacing, 0);
                        modifierList.Width.Set(0, 1);
                        modifierList.Height.Set(225, 0);
                        selectionHalf.Append(modifierList);
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                UIToggle modifier = new();
                                modifier.Width.Set(0, 1);
                                modifier.Height.Set(20, 0);
                                modifierList.Add(modifier);
                                {
                                    UIText text = new($"Poggers mod #{i}");
                                    text.IgnoresMouseInteraction = true;
                                    text.HAlign = 0.5f;
                                    text.VAlign = 0.5f;
                                    modifier.Append(text);
                                }
                            }
                        }
                    }

                    UIElement affixHalf = new();
                    affixHalf.Left.Set(0, 0.5f);
                    affixHalf.Width.Set(0, 0.5f);
                    affixHalf.Height.Set(0, 1);
                    content.Append(affixHalf);
                    {
                        UIText name = new("Affix", 1);
                        title.IgnoresMouseInteraction = true;
                        title.Top.Set(0, 0);
                        title.Height.Set(20, 0);
                        title.HAlign = 0.5f;
                        affixHalf.Append(title);

                        UIFloatRange range1 = new(0.1f, 2.2f, 0.3f);
                        range1.Top.Set(name.Top.Pixels + name.Height.Pixels + UICommon.spacing, 0);
                        range1.Width.Set(0, 1);
                        affixHalf.Append(range1);
                    }
                }
			}
		}
    }
}
