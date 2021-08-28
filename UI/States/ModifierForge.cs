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
using Terraria.Audio;
using PathOfModifiers.Affixes.Items.Constraints;

namespace PathOfModifiers.UI.States
{
    interface IMFSelectable
    {
        string Name { get; }
        string Description { get; }
        int Cost { get; }
    }
    interface IMFAction : IMFSelectable
    {
        void Execute(Item item, ItemItem modItem);
    }
    interface IMFAffixConstraint
    {
        Constraint ItemAffixConstraint { get; set; }
        Constraint DataManagerAffixConstraint { get; set; }
    }
    interface IMFTierConstraint
    {
        Constraint TierConstraint { get; set; }
    }
    class Action
    {
        public class Roll : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Name => "Roll";
            public string Description => "Reroll existing affixes into new ones";
            public int Cost => 1;

            public Constraint DataManagerAffixConstraint { get; set; }
            public Constraint ItemAffixConstraint { get; set; }
            public Constraint TierConstraint { get; set; }

            public void Execute(Item item, ItemItem modItem)
            {
                if (TierConstraint is None)
                {
                    modItem.RerollAffixes(item, ItemAffixConstraint.Then(TierConstraint), DataManagerAffixConstraint);
                }
                else
                {
                    modItem.TryRemoveRandomAffix(item, ItemAffixConstraint);
                    modItem.TryAddRandomAffix(item, DataManagerAffixConstraint);
                }
            }
        }
        public class RollRarity : IMFAction
        {
            public string Name => "Roll rarity";
            public string Description => "Randomly choose a rarity for the item\nIf lower — remove affixes until item fits into the new rarity";
            public int Cost => 1;

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.RerollRarity(item);
            }
        }
        public class Add : IMFAction, IMFAffixConstraint
        {
            public string Name => "Add";
            public string Description => "Add a random affix to the item";
            public int Cost => 10;

            public Constraint DataManagerAffixConstraint { get; set; }
            public Constraint ItemAffixConstraint { get; set; }

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.TryAddRandomAffix(item, DataManagerAffixConstraint);
            }
        }
        public class ImproveRarity : IMFAction
        {
            public string Name => "Improve rarity";
            public string Description => "Raises the rarity once";
            public int Cost => 100;

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.TryRaiseRarity(item);
            }
        }
    }
    abstract class SelectableConstraint : IMFSelectable
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract int Cost { get; }
        public abstract Constraint Constraint { get; }
    }
    class AffixConstraint
    {
        public class Any : SelectableConstraint
        {
            public override string Name => "Any";
            public override string Description => "Can affect all affixes";
            public override int Cost => 1;
            public override Constraint Constraint => new None();
        }
        public class Prefix : SelectableConstraint
        {
            public override string Name => "Prefix";
            public override string Description => "Can affect prefixes only";
            public override int Cost => 5;
            public override Constraint Constraint => new Prefixes();
        }
        public class Suffix : SelectableConstraint
        {
            public override string Name => "Suffix";
            public override string Description => "Can affect suffixes only";
            public override int Cost => 5;
            public override Constraint Constraint => new Suffixes();
        }
    }
    class TierConstraint
    {
        public class Any : SelectableConstraint
        {
            public override string Name => "Any";
            public override string Description => "Can affect all affixes";
            public override int Cost => 1;
            public override Constraint Constraint => new None();
        }
        public class Highest : SelectableConstraint
        {
            public override string Name => "Highest";
            public override string Description => "Only affects the affix with the highest tier";
            public override int Cost => 5;
            public override Constraint Constraint => new HighestTier();
        }
        public class Lowest : SelectableConstraint
        {
            public override string Name => "Lowest";
            public override string Description => "Only affects the affix with the lowest tier";
            public override int Cost => 5;
            public override Constraint Constraint => new LowestTier();
        }
    }


    class ModifierForge : UIState
	{
        public static bool IsOpen => Systems.UI.IsModifierForgeOpen;
        public static void Open(Tiles.ModifierForgeTE forge) => Systems.UI.OpenModifierForge(forge);
        public static void Close() => Systems.UI.CloseModifierForge();

        UIDraggablePanel panel;

        SelectList<ActionListEntry> actionList;
        ActionListEntry improveRarity;

        SelectList<ConstraintListEntry<SelectableConstraint>> affixConstraintList;

        SelectList<ConstraintListEntry<SelectableConstraint>> tierConstraintList;

        UIItemSlot itemSlot;
        FragmentCost fragmentCostText;

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
				closePanelX.OnClick += (UIMouseEvent evt, UIElement listeningElement) => Systems.UI.CloseModifierForge();
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
                        actionList = new();
                        actionList.MinWidth.Set(0f, 0.24f);
                        actionList.MinHeight.Set(0f, 1f);
                        actionList.ListPadding = 0f;
                        actionList.OnEntrySelected += OnActionSelected;
                        actionSection.Append(actionList);
                        {
                            ActionListEntry roll = new(new Action.Roll());
                            actionList.Add(roll);

                            ActionListEntry add = new(new Action.Add());
                            actionList.Add(add);

                            ActionListEntry rollRarity = new(new Action.RollRarity());
                            actionList.Add(rollRarity);

                            improveRarity = new(new Action.ImproveRarity());
                            actionList.Add(improveRarity);
                        }

                        affixConstraintList = new();
                        affixConstraintList.Left.Set(0f, actionList.MinWidth.Percent + 0.01f);
                        affixConstraintList.MinWidth.Set(0f, 0.24f);
                        affixConstraintList.MinHeight.Set(0f, 1f);
                        affixConstraintList.ListPadding = 0f;
                        affixConstraintList.OnEntrySelected += (ConstraintListEntry<SelectableConstraint> entry) => UpdateCost();
                        actionSection.Append(affixConstraintList);
                        {
                            ConstraintListEntry<SelectableConstraint> any = new("Any", new AffixConstraint.Any());
                            affixConstraintList.Add(any);

                            ConstraintListEntry<SelectableConstraint> prefix = new("Prefix", new AffixConstraint.Prefix());
                            affixConstraintList.Add(prefix);

                            ConstraintListEntry<SelectableConstraint> suffix = new("Suffix", new AffixConstraint.Suffix());
                            affixConstraintList.Add(suffix);
                        }

                        tierConstraintList = new();
                        tierConstraintList.Left.Set(0f, affixConstraintList.Left.Percent + affixConstraintList.MinWidth.Percent + 0.01f);
                        tierConstraintList.MinWidth.Set(0f, 0.24f);
                        tierConstraintList.MinHeight.Set(0f, 1f);
                        tierConstraintList.ListPadding = 0f;
                        tierConstraintList.OnEntrySelected += (ConstraintListEntry<SelectableConstraint> entry) => UpdateCost();
                        actionSection.Append(tierConstraintList);
                        {
                            ConstraintListEntry<SelectableConstraint> any = new("Any", new TierConstraint.Any());
                            tierConstraintList.Add(any);

                            ConstraintListEntry<SelectableConstraint> highest = new("Highest", new TierConstraint.Highest());
                            tierConstraintList.Add(highest);

                            ConstraintListEntry<SelectableConstraint> lowest = new("Lowest", new TierConstraint.Lowest());
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
                            var fragmentTexture = ModContent.Request<Texture2D>("PathOfModifiers/Items/ModifierFragment");
                            fragment.CanInsertItem = (Item item) => item.type == ModContent.ItemType<Items.ModifierFragment>();
                            fragment.OnDrawGhostItem += delegate (SpriteBatch sb, Vector2 position, float scale)
                            {
                                sb.Draw(fragmentTexture.Value, position, null, Color.White * 0.3f, 0f, fragmentTexture.Size() / 2f, scale, SpriteEffects.None, 0f);
                            };
                            actionParent.Append(fragment);

                            itemSlot = new();
                            itemSlot.MinWidth.Set(fragment.MinWidth.Pixels, 0f);
                            itemSlot.MinHeight.Set(fragment.MinWidth.Pixels, 0f);
                            itemSlot.Left.Set(-fragment.MinWidth.Pixels, 1f);
                            var itemTexture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ ItemID.GoldBroadsword }");
                            itemSlot.CanInsertItem = (Item item) => item.TryGetGlobalItem<ItemItem>(out var _);
                            itemSlot.OnDrawGhostItem += delegate (SpriteBatch sb, Vector2 position, float scale)
                            {
                                sb.Draw(itemTexture.Value, position, null, Color.White * 0.3f, 0f, itemTexture.Size() / 2f, scale, SpriteEffects.None, 0f);
                            };
                            itemSlot.OnItemChanged += OnItemChanged;
                            actionParent.Append(itemSlot);

                            UIButton forge = new();
                            forge.HAlign = 0.5f;
                            forge.Top.Set(0f, 0.6f);
                            forge.MinHeight.Set(0f, 0.4f);
                            forge.MinWidth.Set(0f, 1f);
                            forge.OnClick += OnForgeClicked;
                            actionParent.Append(forge);
                            {
                                UIElement cost = new();
                                cost.MinWidth.Set(0f, 1f);
                                cost.MinHeight.Set(0f, 0.3f);
                                forge.Append(cost);
                                {
                                    fragmentCostText = new("654456", 0.35f, true);
                                    fragmentCostText.HAlign = 0.5f;
                                    fragmentCostText.VAlign = 0.5f;
                                    fragmentCostText.IgnoresMouseInteraction = true;
                                    cost.Append(fragmentCostText);
                                }

                                UIText text = new("Forge", 0.55f, true);
                                text.Top.Set(cost.GetOuterDimensions().Height + UICommon.spacing * 2f, 0f);
                                text.HAlign = 0.5f;
                                text.IgnoresMouseInteraction = true;
                                forge.Append(text);
                            }
                        }
                    }

                    UIElement textSection = new();
                    textSection.HAlign = 0.5f;
                    textSection.Top.Set(0f, 0.43f);
                    textSection.MinWidth.Set(0f, 0.9f);
                    textSection.MinHeight.Set(0f, 0.57f);
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
                            affixTextItemName = new("         Rare Beads of Annahilatikon", 0.8f);
                            affixTextItemName.TextColor = Color.Orange;
                            itemText.Append(affixTextItemName);

                            float textScale = 0.7f;

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
                                    affixText.SetText($"{ Keyword.GetTextOrTag(KeywordType.Bleed) }gamepedia is soo bad on russian, literally 90% of in-game stuff just doesn't exists there as page, while fandom is 95% full", textScale);
                                }
                            }
                        }
                    }
                }
			}

            OnItemChanged(new Item());
        }

        private void OnForgeClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            var action = actionList.SelectedItem.action;
            var affixConstraint = affixConstraintList.SelectedItem?.constraint;
            var tierConstraint = tierConstraintList.SelectedItem?.constraint;

            if (action != null)
            {
                if (action is IMFAffixConstraint affixAction)
                {
                    affixAction.DataManagerAffixConstraint = affixConstraint.Constraint;
                    affixAction.ItemAffixConstraint = affixConstraint.Constraint;
                }
                if (action is IMFTierConstraint tierAction)
                {
                    tierAction.TierConstraint = tierConstraint.Constraint;
                }

                var item = itemSlot.Item;
                if (item.TryGetGlobalItem<ItemItem>(out var modItem))
                {
                    action.Execute(item, modItem);

                    PopupText.NewText(PopupTextContext.ItemReforge, item, item.stack, true, false);
                    SoundEngine.PlaySound(SoundID.Item37, -1, -1);
                    //UI.States.ModifierForge.SetItemSlots(modifiedItem.Clone(), modifierItem.Clone());
                    //SendModifiedItemToServer();
                    //SendModifierItemToServer();

                    UpdateItemText(itemSlot.Item);
                }
            }
        }

        void OnActionSelected(ActionListEntry entry)
        {
            bool isActioAffixConstrained = entry.action is IMFAffixConstraint;
            foreach (var constraint in affixConstraintList)
            {
                constraint.IsEnabled = isActioAffixConstrained;
            }

            if (isActioAffixConstrained)
            {
                foreach (var constraint in affixConstraintList)
                {
                    if (constraint.IsEnabled)
                    {
                        affixConstraintList.Select(constraint);
                        break;
                    }
                }
            }
            else
            {
                affixConstraintList.Deselect();
            }

            bool isActioTierConstrained = entry.action is IMFTierConstraint;
            foreach (var constraint in tierConstraintList)
            {
                constraint.IsEnabled = isActioTierConstrained;
            }

            if (isActioTierConstrained)
            {
                foreach (var constraint in tierConstraintList)
                {
                    if (constraint.IsEnabled)
                    {
                        tierConstraintList.Select(constraint);
                        break;
                    }
                }
            }
            else
            {
                tierConstraintList.Deselect();
            }

            UpdateCost();
        }

        void OnItemChanged(Item item)
        {
            bool isItemValid = item.TryGetGlobalItem<ItemItem>(out var modItem);
            foreach (var action in actionList)
            {
                action.IsEnabled = isItemValid;
            }
            foreach (var constraint in affixConstraintList)
            {
                constraint.IsEnabled = false;
            }
            foreach (var constraint in tierConstraintList)
            {
                constraint.IsEnabled = false;
            }
            actionList.Deselect();
            affixConstraintList.Deselect();
            tierConstraintList.Deselect();

            if (isItemValid)
            {
                improveRarity.IsEnabled = modItem.CanRaiseRarity(item);

                foreach (var action in actionList)
                {
                    if (action.IsEnabled)
                    {
                        actionList.Select(action);
                        break;
                    }
                }
            }
            UpdateCost();
            UpdateItemText(item);
        }

        void UpdateCost()
        {
            ItemItem modItem = null;
            itemSlot.Item?.TryGetGlobalItem(out modItem);
            int newCost = modItem?.rarity.forgeCost ?? 0;
            newCost *= actionList.SelectedItem?.action.Cost ?? 0;
            newCost *= affixConstraintList.SelectedItem?.constraint.Cost ?? 1;
            newCost *= tierConstraintList.SelectedItem?.constraint.Cost ?? 1;

            fragmentCostText.SetText(newCost.ToString());
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

                affixTextItemName.SetText($"         {item.Name}");
                affixTextItemName.TextColor = modItem.rarity.color;

                int i = 0;
                for (; i < modItem.prefixes.Count; i++)
                {
                    if (i >= affixTextList.Count) break;

                    var prefix = modItem.prefixes[i];
                    var text = affixTextList._items[i];

                    text.TextColor = prefix.Color;
                    text.SetText(prefix.GetForgeText());
                }
                int j = 0;
                for (; j < modItem.suffixes.Count; j++)
                {
                    int textIndex = i + j;
                    if (textIndex >= affixTextList.Count) break;

                    var suffix = modItem.suffixes[j];
                    var text = affixTextList._items[textIndex];

                    text.TextColor = suffix.Color;
                    text.SetText(suffix.GetForgeText());
                }

                affixTextList.Recalculate();
            }
            else
            {
                freeAffixes.SetText("[0/0]");
                freePrefixes.SetText("[0/0]");
                freeSuffixes.SetText("[0/0]");

                affixTextItemName.SetText("No valid item");
            }
        }
    }
}
