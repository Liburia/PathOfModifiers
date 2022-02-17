using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.Affixes.Items.Constraints;
using PathOfModifiers.Tiles;
using PathOfModifiers.UI.Chat;
using PathOfModifiers.UI.Elements;
using PathOfModifiers.UI.States.ModifierForgeElements;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

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
        public class RemoveAll : IMFAction, IMFAffixConstraint
        {
            public string Name => "Remove all";
            public string Description => "Remove all affixes";
            public int Cost => 5;

            public Constraint DataManagerAffixConstraint { get; set; }
            public Constraint ItemAffixConstraint { get; set; }

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.RemoveAllAffixes(item, ItemAffixConstraint);
            }
        }
        public class RollValues : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Name => "Roll values";
            public string Description => "Reroll affix values within their tier";
            public int Cost => 5;

            public Constraint DataManagerAffixConstraint { get; set; }
            public Constraint ItemAffixConstraint { get; set; }
            public Constraint TierConstraint { get; set; }

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.RollAffixTierMultipliers(item, ItemAffixConstraint.Then(TierConstraint));
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
        public class Remove : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Name => "Remove";
            public string Description => "Remove a random affix";
            public int Cost => 50;

            public Constraint DataManagerAffixConstraint { get; set; }
            public Constraint ItemAffixConstraint { get; set; }
            public Constraint TierConstraint { get; set; }

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.TryRemoveRandomAffix(item, ItemAffixConstraint.Then(TierConstraint));
            }
        }
        public class Exchange : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Name => "Exchange";
            public string Description => "Change an affix into a random one, keeping the tier";
            public int Cost => 50;

            public Constraint DataManagerAffixConstraint { get; set; }
            public Constraint ItemAffixConstraint { get; set; }
            public Constraint TierConstraint { get; set; }

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.ExchangeRandomAffix(item, ItemAffixConstraint.Then(TierConstraint));
            }
        }
        public class ImproveValue : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Name => "Improve value";
            public string Description => "Improve value of a random affix within its tier";
            public int Cost => 50;

            public Constraint DataManagerAffixConstraint { get; set; }
            public Constraint ItemAffixConstraint { get; set; }
            public Constraint TierConstraint { get; set; }

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.ImproveRandomAffixTierMultiplier(item, ItemAffixConstraint.Then(TierConstraint));
            }
        }
        public class ImproveTier : IMFAction, IMFAffixConstraint, IMFTierConstraint
        {
            public string Name => "Improve tier";
            public string Description => "Improve tier of a random affix";
            public int Cost => 500;

            public Constraint DataManagerAffixConstraint { get; set; }
            public Constraint ItemAffixConstraint { get; set; }
            public Constraint TierConstraint { get; set; }

            public void Execute(Item item, ItemItem modItem)
            {
                modItem.ImproveRandomAffixTier(item, ItemAffixConstraint.Then(TierConstraint));
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
        public class Worst : SelectableConstraint
        {
            public override string Name => "Worst";
            public override string Description => "Only affects the affix with the highest relative tier\nTier 2/3 is higher than 3/6. In case of multiple tiers — averages";
            public override int Cost => 5;
            public override Constraint Constraint => new LowestTier();
        }
        public class Best : SelectableConstraint
        {
            public override string Name => "Best";
            public override string Description => "Only affects the affix with the lowest tier\nTier 3/6 is lower than 2/3. In case of multiple tiers — averages";
            public override int Cost => 5;
            public override Constraint Constraint => new HighestTier();
        }
    }


    public class ModifierForge : UIState
    {
        public static bool IsOpen => Systems.UI.IsModifierForgeOpen;

        public static void Open(ModifierForgeTE forge)
        {
            Systems.UI.ModifierForgeState.CurrentForgeTE = forge;
            Systems.UI.ModifierForgeInterface?.SetState(Systems.UI.ModifierForgeState);
            Main.playerInventory = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
        }
        public static void Close()
        {
            Systems.UI.ModifierForgeState.CurrentForgeTE = null;
            Systems.UI.ModifierForgeInterface?.SetState(null);
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
        public static void UpdateItemsFromForge()
        {
            var state = Systems.UI.ModifierForgeState;
            var forge = state.currentForgeTE;
            var item = forge.ModifiedItem.Clone();
            state.itemSlot.SetItem(item);
            state.fragmentSlot.SetItem(forge.ModifierItem.Clone());
            state.UpdateCost();
            state.UpdateForgeButton();
            state.UpdateItemText(item);
        }

        ModifierForgeTE currentForgeTE;
        public ModifierForgeTE CurrentForgeTE
        {
            get => currentForgeTE;
            set
            {
                currentForgeTE = value;
                Tiles.ModifierForge.activeForge = value;
                itemSlot.TryInsertItem(CurrentForgeTE?.ModifiedItem ?? new Item(), false, out _);
                fragmentSlot.TryInsertItem(CurrentForgeTE?.ModifierItem ?? new Item(), false, out _);
            }
        }

        UIDraggablePanel panel;

        SelectList<ActionListEntry> actionList;
        ActionListEntry addAffix;
        ActionListEntry improveRarity;

        SelectList<ConstraintListEntry<SelectableConstraint>> affixConstraintList;

        SelectList<ConstraintListEntry<SelectableConstraint>> tierConstraintList;

        internal UIItemSlot itemSlot;
        internal UIItemSlot fragmentSlot;
        UIButton forgeButton;
        FragmentCost fragmentCostText;
        int currentCost;

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
                closePanelX.OnClick += (UIMouseEvent evt, UIElement listeningElement) => Close();
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
                        UIScrollbar actionListScrollBar = new();
                        actionListScrollBar.Left.Set(0f, 0.22f);
                        actionListScrollBar.MinHeight.Set(0f, 1f);
                        actionSection.Append(actionListScrollBar);

                        actionList = new();
                        actionList.MinWidth.Set(0f, 0.22f);
                        actionList.MinHeight.Set(0f, 1f);
                        actionList.SetScrollbar(actionListScrollBar);
                        actionList.ListPadding = 0f;
                        actionList.OnEntrySelected += OnActionSelected;
                        actionSection.Append(actionList);
                        {
                            ActionListEntry roll = new(new Action.Roll());
                            actionList.Add(roll);

                            ActionListEntry rollRarity = new(new Action.RollRarity());
                            actionList.Add(rollRarity);

                            addAffix = new(new Action.Add());
                            actionList.Add(addAffix);

                            ActionListEntry removeAll = new(new Action.RemoveAll());
                            actionList.Add(removeAll);

                            ActionListEntry rollValues = new(new Action.RollValues());
                            actionList.Add(rollValues);

                            improveRarity = new(new Action.ImproveRarity());
                            actionList.Add(improveRarity);

                            ActionListEntry remove = new(new Action.Remove());
                            actionList.Add(remove);

                            ActionListEntry exchange = new(new Action.Exchange());
                            actionList.Add(exchange);

                            ActionListEntry improveValue = new(new Action.ImproveValue());
                            actionList.Add(improveValue);

                            ActionListEntry improveTier = new(new Action.ImproveTier());
                            actionList.Add(improveTier);
                        }

                        affixConstraintList = new();
                        affixConstraintList.Left.Set(0f, actionList.MinWidth.Percent + 0.03f);
                        affixConstraintList.MinWidth.Set(0f, 0.22f);
                        affixConstraintList.MinHeight.Set(0f, 1f);
                        affixConstraintList.ListPadding = 0f;
                        affixConstraintList.OnEntrySelected += delegate (ConstraintListEntry<SelectableConstraint> entry)
                        {
                            UpdateCost();
                            UpdateForgeButton();
                        };
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
                        tierConstraintList.Left.Set(0f, affixConstraintList.Left.Percent + affixConstraintList.MinWidth.Percent + 0.03f);
                        tierConstraintList.MinWidth.Set(0f, 0.22f);
                        tierConstraintList.MinHeight.Set(0f, 1f);
                        tierConstraintList.ListPadding = 0f;
                        tierConstraintList.OnEntrySelected += delegate (ConstraintListEntry<SelectableConstraint> entry)
                        {
                            UpdateCost();
                            UpdateForgeButton();
                        };
                        actionSection.Append(tierConstraintList);
                        {
                            ConstraintListEntry<SelectableConstraint> any = new("Any", new TierConstraint.Any());
                            tierConstraintList.Add(any);

                            ConstraintListEntry<SelectableConstraint> highest = new("Worst", new TierConstraint.Worst());
                            tierConstraintList.Add(highest);

                            ConstraintListEntry<SelectableConstraint> lowest = new("Best", new TierConstraint.Best());
                            tierConstraintList.Add(lowest);
                        }

                        UIElement actionParent = new();
                        actionParent.Left.Set(0f, tierConstraintList.Left.Percent + tierConstraintList.MinWidth.Percent + 0.03f);
                        actionParent.MinWidth.Set(0f, 0.24f);
                        actionParent.MinHeight.Set(0f, 1f);
                        actionSection.Append(actionParent);
                        {

                            fragmentSlot = new();
                            fragmentSlot.MinWidth.Set(75f, 0f);
                            fragmentSlot.MinHeight.Set(fragmentSlot.MinWidth.Pixels, 0f);
                            var fragmentTexture = ModContent.Request<Texture2D>("PathOfModifiers/Items/ModifierFragment");
                            fragmentSlot.CanInsertItem = (Item item) => item.type == ModContent.ItemType<Items.ModifierFragment>();
                            fragmentSlot.OnDrawGhostItem += delegate (SpriteBatch sb, Vector2 position, float scale)
                            {
                                sb.Draw(fragmentTexture.Value, position, null, Color.White * 0.3f, 0f, fragmentTexture.Size() / 2f, scale, SpriteEffects.None, 0f);
                            };
                            fragmentSlot.OnItemChanged += OnFragmentChanged;
                            actionParent.Append(fragmentSlot);

                            itemSlot = new();
                            itemSlot.MinWidth.Set(fragmentSlot.MinWidth.Pixels, 0f);
                            itemSlot.MinHeight.Set(fragmentSlot.MinWidth.Pixels, 0f);
                            itemSlot.Left.Set(-fragmentSlot.MinWidth.Pixels, 1f);
                            var itemTexture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ ItemID.GoldBroadsword }");
                            itemSlot.CanInsertItem = (Item item) => item.TryGetGlobalItem<ItemItem>(out var _);
                            itemSlot.OnDrawGhostItem += delegate (SpriteBatch sb, Vector2 position, float scale)
                            {
                                sb.Draw(itemTexture.Value, position, null, Color.White * 0.3f, 0f, itemTexture.Size() / 2f, scale, SpriteEffects.None, 0f);
                            };
                            itemSlot.OnItemChanged += OnItemChanged;
                            actionParent.Append(itemSlot);

                            forgeButton = new();
                            forgeButton.HAlign = 0.5f;
                            forgeButton.Top.Set(0f, 0.6f);
                            forgeButton.MinHeight.Set(0f, 0.4f);
                            forgeButton.MinWidth.Set(0f, 1f);
                            forgeButton.OnClick += OnForgeClicked;
                            actionParent.Append(forgeButton);
                            {
                                UIElement cost = new();
                                cost.MinWidth.Set(0f, 1f);
                                cost.MinHeight.Set(0f, 0.3f);
                                forgeButton.Append(cost);
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
                                forgeButton.Append(text);
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsOpen)
            {
                Player player = Main.LocalPlayer;
                Point playerPos = player.MountedCenter.ToTileCoordinates();
                //TODO: Don't hardcode TE size?
                if (playerPos.X < Tiles.ModifierForge.activeForge.Position.X - Player.tileRangeX || playerPos.X > Tiles.ModifierForge.activeForge.Position.X + Player.tileRangeX + 2 ||
                    playerPos.Y < Tiles.ModifierForge.activeForge.Position.Y - Player.tileRangeY || playerPos.Y > Tiles.ModifierForge.activeForge.Position.Y + Player.tileRangeY + 1)
                {
                    Close();
                }
            }
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

                UpdateCost();
                var item = itemSlot.Item;
                if (item.TryGetGlobalItem<ItemItem>(out var modItem) && (!fragmentSlot.Item?.IsAir ?? false) && fragmentSlot.Item.stack >= currentCost)
                {
                    fragmentSlot.Item.stack -= System.Math.Min(fragmentSlot.Item.stack, currentCost);

                    action.Execute(item, modItem);

                    PopupText.NewText(PopupTextContext.ItemReforge, item, item.stack, true, false);
                    SoundEngine.PlaySound(SoundID.Item37, -1, -1);

                    OnItemChanged(item);
                    OnFragmentChanged(fragmentSlot.Item);
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
            UpdateForgeButton();
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
                addAffix.IsEnabled = modItem.FreeAffixes > 0;
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
            UpdateForgeButton();
            UpdateItemText(item);
            SyncItemWithForge();
        }
        void OnFragmentChanged(Item item)
        {
            UpdateCost();
            UpdateForgeButton();
            SyncFragmentWithForge();
        }

        void UpdateCost()
        {
            ItemItem modItem = null;
            itemSlot.Item?.TryGetGlobalItem(out modItem);
            currentCost = modItem?.rarity.forgeCost ?? 0;
            currentCost *= actionList.SelectedItem?.action.Cost ?? 0;
            currentCost *= affixConstraintList.SelectedItem?.constraint.Cost ?? 1;
            currentCost *= tierConstraintList.SelectedItem?.constraint.Cost ?? 1;
        }
        void UpdateForgeButton()
        {
            fragmentCostText.SetText(currentCost.ToString());

            forgeButton.IsEnabled = currentCost <= GetCurrentFragmentCount() && (!itemSlot.Item?.IsAir ?? false);
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

        int GetCurrentFragmentCount()
        {
            return fragmentSlot.Item?.stack ?? 0;
        }

        void SyncItemWithForge()
        {
            CurrentForgeTE?.SetItem(itemSlot.Item.Clone(), false);
            CurrentForgeTE?.SendItemToServer();
        }
        void SyncFragmentWithForge()
        {
            CurrentForgeTE?.SetFragment(fragmentSlot.Item.Clone(), false);
            CurrentForgeTE?.SendFragmentToServer();
        }
    }
}
