using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
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
using PathOfModifiers.UI.States.ModifierForgeElements;
using PathOfModifiers.UI.Chat;
using Terraria.ID;

namespace PathOfModifiers.UI.States
{
    enum AffixConstraint
    {
        Any,
        Prefix,
        Suffix,
    }
    enum TierConstraint
    {
        Any,
        Highest,
        Lowest,
    }

    interface IMFAction
    {
        string Text { get; }

        void Execute(ItemItem item);
    }
    interface IMFAffixConstraint
    {
        AffixConstraint AffixConstraint { get; set; }
    }
    interface IMFTierConstraint
    {
        TierConstraint TierConstraint { get; set; }
    }
    class Action
    {
        public class Add : IMFAction, IMFAffixConstraint
        {
            public string Text => "Add";

            AffixConstraint IMFAffixConstraint.AffixConstraint { get; set; }

            public void Execute(ItemItem item)
            {
            }
        }
        public class Remove : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Text => "Remove";

            public AffixConstraint AffixConstraint { get; set; }
            public TierConstraint TierConstraint { get; set; }

            public void Execute(ItemItem item)
            {
            }
        }
        public class Roll : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Text => "Roll";

            public AffixConstraint AffixConstraint { get; set; }
            public TierConstraint TierConstraint { get; set; }

            public void Execute(ItemItem item)
            {
            }
        }
        public class Roll2 : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Text => "Roll2";

            public AffixConstraint AffixConstraint { get; set; }
            public TierConstraint TierConstraint { get; set; }

            public void Execute(ItemItem item)
            {
            }
        }
        public class Roll3 : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Text => "Roll3";

            public AffixConstraint AffixConstraint { get; set; }
            public TierConstraint TierConstraint { get; set; }

            public void Execute(ItemItem item)
            {
            }
        }
    }


    class ModifierForge : UIState
	{
        UIDraggablePanel panel;

        UIText freeAffixes;
        UIText freePrefixes;
        UIText freeSuffixes;

        UIText affixTextItemName;
        UIList<AffixText> affixTextList;

        public override void OnInitialize()
        {
            panel = new();
			panel.Left.Set(200f, 0f);
			panel.Top.Set(100f, 0f);
			panel.MinWidth.Set(700f, 0f);
			panel.MinHeight.Set(400f, 0f);
			Append(panel);
			{
                UIText title = new("Modifier Forge", UICommon.textMedium);
                title.IgnoresMouseInteraction = true;
                title.Top.Set(0, 0);
                title.HAlign = 0.5f;
                panel.Append(title);

                UIImageButton closePanelX = new(ModContent.Request<Texture2D>(PoMGlobals.Path.Image.UI.CloseButton, ReLogic.Content.AssetRequestMode.ImmediateLoad));
				closePanelX.Top.Set(0f, 0f);
				closePanelX.Left.Set(650f, 0f);
				closePanelX.OnClick += (UIMouseEvent evt, UIElement listeningElement) => Systems.UI.ToggleModifierForgeState();
				panel.Append(closePanelX);

                UIElement content = new();
                content.Top.Set(closePanelX.Top.Pixels + closePanelX.GetDimensions().Height + UICommon.spacing, 0);
                content.MinWidth.Set(0, 1);
                content.MinHeight.Set(0, 1);
                panel.Append(content);
                {
                    UIElement actionSection = new();
                    actionSection.MinWidth.Set(0f, 1f);
                    actionSection.MinHeight.Set(0f, 0.4f);
                    content.Append(actionSection);
                    {
                        SelectList<ActionListEntry> actionList = new();
                        actionList.MinWidth.Set(0f, 0.24f);
                        actionList.MinHeight.Set(0f, 1f);
                        actionList.ListPadding = 0f;
                        actionSection.Append(actionList);
                        {
                            ActionListEntry add = new(new Action.Add());
                            actionList.Add(add);

                            ActionListEntry remove = new(new Action.Remove());
                            actionList.Add(remove);

                            ActionListEntry roll = new(new Action.Roll());
                            actionList.Add(roll);

                            ActionListEntry roll2 = new(new Action.Roll2());
                            actionList.Add(roll2);

                            ActionListEntry roll3 = new(new Action.Roll3());
                            actionList.Add(roll3);
                        }

                        SelectList<ConstraintListEntry<AffixConstraint>> affixConstraintList = new();
                        affixConstraintList.Left.Set(0f, actionList.MinWidth.Percent + 0.01f);
                        affixConstraintList.MinWidth.Set(0f, 0.24f);
                        affixConstraintList.MinHeight.Set(0f, 1f);
                        affixConstraintList.ListPadding = 0f;
                        actionSection.Append(affixConstraintList);
                        {
                            ConstraintListEntry<AffixConstraint> any = new("Any", AffixConstraint.Any);
                            affixConstraintList.Add(any);

                            ConstraintListEntry<AffixConstraint> prefix = new("Prefix", AffixConstraint.Prefix);
                            affixConstraintList.Add(prefix);

                            ConstraintListEntry<AffixConstraint> suffix = new("Suffix", AffixConstraint.Suffix);
                            affixConstraintList.Add(suffix);
                        }

                        SelectList<ConstraintListEntry<TierConstraint>> tierConstraintList = new();
                        tierConstraintList.Left.Set(0f, affixConstraintList.Left.Percent + affixConstraintList.MinWidth.Percent + 0.01f);
                        tierConstraintList.MinWidth.Set(0f, 0.24f);
                        tierConstraintList.MinHeight.Set(0f, 1f);
                        tierConstraintList.ListPadding = 0f;
                        actionSection.Append(tierConstraintList);
                        {
                            ConstraintListEntry<TierConstraint> any = new("Any", TierConstraint.Any);
                            tierConstraintList.Add(any);

                            ConstraintListEntry<TierConstraint> highest = new("Highest", TierConstraint.Highest);
                            tierConstraintList.Add(highest);

                            ConstraintListEntry<TierConstraint> lowest = new("Lowest", TierConstraint.Lowest);
                            tierConstraintList.Add(lowest);
                        }

                        UIElement actionParent = new();
                        actionParent.Left.Set(0f, tierConstraintList.Left.Percent + tierConstraintList.MinWidth.Percent + 0.01f);
                        actionParent.MinWidth.Set(0f, 0.24f);
                        actionParent.MinHeight.Set(0f, 1f);
                        actionSection.Append(actionParent);
                        {

                            UIItemSlot fragment = new();
                            fragment.MinWidth.Set(75f, 0f);
                            fragment.MinHeight.Set(fragment.MinWidth.Pixels, 0f);
                            var fragmentTexture = ModContent.Request<Texture2D>($"PathOfModifiers/Items/ModifierFragment");
                            fragment.CanInsertItem = (Item item) => item.type == ModContent.ItemType<Items.ModifierFragment>();
                            fragment.OnDrawGhostItem += delegate (SpriteBatch sb, Vector2 position, float scale)
                            {
                                sb.Draw(fragmentTexture.Value, position, null, Color.White * 0.3f, 0f, fragmentTexture.Size() / 2f, scale, SpriteEffects.None, 0f);
                            };
                            actionParent.Append(fragment);

                            UIItemSlot item = new();
                            item.MinWidth.Set(fragment.MinWidth.Pixels, 0f);
                            item.MinHeight.Set(fragment.MinWidth.Pixels, 0f);
                            item.Left.Set(-fragment.MinWidth.Pixels, 1f);
                            var itemTexture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ ItemID.GoldBroadsword }");
                            item.CanInsertItem = (Item item) => ItemItem.IsRollable(item);
                            item.OnDrawGhostItem += delegate (SpriteBatch sb, Vector2 position, float scale)
                            {
                                sb.Draw(itemTexture.Value, position, null, Color.White * 0.3f, 0f, itemTexture.Size() / 2f, scale, SpriteEffects.None, 0f);
                            };
                            item.OnItemInserted += delegate (Item item)
                            {
                                UpdateItemText(item);
                            };
                            actionParent.Append(item);

                            UIButton reforge = new();
                            reforge.HAlign = 0.5f;
                            reforge.Top.Set(0f, 0.6f);
                            reforge.MinHeight.Set(0f, 0.4f);
                            reforge.MinWidth.Set(0f, 1f);
                            actionParent.Append(reforge);
                            {
                                UIText text = new("Forge", 0.9f, true);
                                text.HAlign = 0.5f;
                                text.VAlign = 0.5f;
                                text.IgnoresMouseInteraction = true;
                                reforge.Append(text);
                            }
                        }
                    }

                    UIElement textSection = new();
                    textSection.HAlign = 0.5f;
                    textSection.Top.Set(0f, 0.5f);
                    textSection.MinWidth.Set(0f, 0.9f);
                    textSection.MinHeight.Set(0f, 0.5f);
                    content.Append(textSection);
                    {
                        UIElement freeAffixSection = new();
                        freeAffixSection.MinHeight.Set(0f, 1f);
                        textSection.Append(freeAffixSection);
                        {
                            freeAffixes = new("[3/7]");
                            freeAffixes.HAlign = 0.5f;
                            freeAffixes.OnUpdate += delegate (UIElement affectedElement)
                            {
                                if (freeAffixes.IsMouseHovering)
                                    Main.instance.MouseText("Total number of affixes on the item");
                            };
                            freeAffixSection.Append(freeAffixes);
                            freeAffixSection.MinWidth.Set(freeAffixes.GetDimensions().Width, 0f);

                            freePrefixes = new("[2/5]");
                            freePrefixes.TextColor = Affix.prefixColor;
                            freePrefixes.HAlign = 0.5f;
                            freePrefixes.Top.Set(freeAffixes.MinHeight.Pixels + UICommon.spacing * 3, 0f);
                            freePrefixes.OnUpdate += delegate (UIElement affectedElement)
                            {
                                if (freePrefixes.IsMouseHovering)
                                    Main.instance.MouseText("Number of prefixes on the item");
                            };
                            freeAffixSection.Append(freePrefixes);

                            freeSuffixes = new("[1/4]");
                            freeSuffixes.TextColor = Affix.suffixColor;
                            freeSuffixes.HAlign = 0.5f;
                            freeSuffixes.Top.Set(freePrefixes.Top.Pixels + freePrefixes.MinHeight.Pixels + UICommon.spacing * 3, 0f);
                            freeSuffixes.OnUpdate += delegate (UIElement affectedElement)
                            {
                                if (freeSuffixes.IsMouseHovering)
                                    Main.instance.MouseText("Number of suffixes on the item");
                            };
                            freeAffixSection.Append(freeSuffixes);
                        }

                        UIElement itemText = new();
                        itemText.Left.Set(freeAffixSection.MinWidth.Pixels + UICommon.spacing, 0f);
                        itemText.MinWidth.Set(-freeAffixSection.MinWidth.Pixels, 1f);
                        itemText.MinHeight.Set(0f, 1f);
                        textSection.Append(itemText);
                        {
                            affixTextItemName = new("Rare Beads of Annahilatikon", 0.8f);
                            affixTextItemName.TextColor = Color.Orange;
                            itemText.Append(affixTextItemName);

                            affixTextList = new();
                            affixTextList.Top.Set(affixTextItemName.GetDimensions().Height + UICommon.spacing, 0f);
                            affixTextList.MinWidth.Set(0f, 1f);
                            affixTextList.MinHeight.Set(-affixTextItemName.GetDimensions().Height, 1f);
                            affixTextList.ListPadding = 0f;
                            itemText.Append(affixTextList);
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    AffixText affixText = new();
                                    affixText.MaxWidth.Set(0f, 1f);
                                    affixText.TextColor = Color.Green;
                                    affixTextList.Add(affixText);
                                    affixText.SetText($"{ Keyword.Bleed.GenerateTag() }gamepedia is soo bad on russian, literally 90% of in-game stuff just doesn't exists there as page, while fandom is 95% full", 0.7f);
                                }
                            }
                        }
                    }
                }
			}
        }

        void UpdateItemText(Item item)
        {
            foreach (var text in affixTextList)
            {
                text.SetText("");
            }

            if (item.TryGetGlobalItem<ItemItem>(out var modItem))
            {
                freeAffixes.SetText($"[{modItem.affixes.Count}/{modItem.rarity.maxAffixes}]");
                freePrefixes.SetText($"[{modItem.prefixes.Count}/{modItem.rarity.maxPrefixes}]");
                freeSuffixes.SetText($"[{modItem.suffixes.Count}/{modItem.rarity.maxSuffixes}]");

                affixTextItemName.SetText(item.Name);

                int i = 0;
                for (; i < modItem.prefixes.Count; i++)
                {
                    if (i >= affixTextList.Count) break;

                    var prefix = modItem.prefixes[i];
                    var text = affixTextList._items[i];

                    text.TextColor = prefix.Color;
                    text.SetText(prefix.GetTolltipText());
                }
                int j = 0;
                for (; j < modItem.suffixes.Count; j++)
                {
                    int textIndex = i + j;
                    if (textIndex >= affixTextList.Count) break;

                    var suffix = modItem.suffixes[j];
                    var text = affixTextList._items[textIndex];

                    text.TextColor = suffix.Color;
                    text.SetText(suffix.GetTolltipText());
                }

                affixTextList.Recalculate();
            }
            else
            {
                freeAffixes.SetText("[0/0]");
                freePrefixes.SetText("[0/0]");
                freeSuffixes.SetText("[0/0]");

                affixTextItemName.SetText("No item");
            }
        }
    }
}
