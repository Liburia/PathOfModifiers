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
                            UIItemSlot item = new();
                            item.MinWidth.Set(75f, 0f);
                            item.MinHeight.Set(75f, 0f);
                            actionParent.Append(item);

                            UIItemSlot fragment = new();
                            fragment.MinWidth.Set(75f, 0f);
                            fragment.MinHeight.Set(75f, 0f);
                            fragment.Left.Set(-fragment.MinWidth.Pixels, 1f);
                            actionParent.Append(fragment);

                            UIButton reforge = new();
                            reforge.HAlign = 0.5f;
                            reforge.Top.Set(0f, 0.6f);
                            reforge.MinHeight.Set(0f, 0.4f);
                            reforge.MinWidth.Set(0f, 1f);
                            actionParent.Append(reforge);
                            {
                                UIText text = new("Forge", 1f, true);
                                text.HAlign = 0.5f;
                                text.VAlign = 0.5f;
                                text.IgnoresMouseInteraction = true;
                                reforge.Append(text);
                            }

                        }
                    }
                }
			}
        }
    }
}
