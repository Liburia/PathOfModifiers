using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace PathOfModifiers.UI.Elements
{
	public class UIList<T> : UIElement, IEnumerable<T>, IEnumerable
		where T: UIElement
	{
		public delegate bool ElementSearchMethod(UIElement element);

		private class UIInnerList : UIElement
		{
			public override bool ContainsPoint(Vector2 point) => true;

			protected override void DrawChildren(SpriteBatch spriteBatch)
			{
				Vector2 position = Parent.GetDimensions().Position();
				Vector2 dimensions = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
				foreach (UIElement element in Elements)
				{
					Vector2 position2 = element.GetDimensions().Position();
					Vector2 dimensions2 = new Vector2(element.GetDimensions().Width, element.GetDimensions().Height);
					if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
						element.Draw(spriteBatch);
				}
			}

			public override Rectangle GetViewCullingArea() => Parent.GetDimensions().ToRectangle();
		}

		internal List<T> _items = new();
		protected UIScrollbar _scrollbar;
		private UIInnerList _innerList = new();
		private float _innerListHeight;
		public float ListPadding = 5f;
		public Action<List<T>> ManualSortMethod;

		public float ViewPosition
		{
			get => _scrollbar.ViewPosition;
			set => _scrollbar.ViewPosition = value;
		}

		public int Count => _items.Count;

		public UIList()
		{
			_innerList.OverflowHidden = false;
			_innerList.Width.Set(0f, 1f);
			_innerList.Height.Set(0f, 1f);
			OverflowHidden = true;
			Append(_innerList);
		}

		public float GetTotalHeight() => _innerListHeight;

		public void Goto(ElementSearchMethod searchMethod)
		{
			int num = 0;
			while (true)
			{
				if (num < _items.Count)
				{
					if (searchMethod(_items[num]))
						break;

					num++;
					continue;
				}

				return;
			}

			_scrollbar.ViewPosition = _items[num].Top.Pixels;
		}

		public virtual void Add(T item)
		{
			_items.Add(item);
			_innerList.Append(item);
			UpdateOrder();
			_innerList.Recalculate();
		}

		public virtual void AddRange(IEnumerable<T> items)
		{
			_items.AddRange(items);
			foreach (var item in items)
				_innerList.Append(item);

			UpdateOrder();
			_innerList.Recalculate();
		}

		public virtual bool Remove(T item)
		{
			_innerList.RemoveChild(item);
			UpdateOrder();
			return _items.Remove(item);
		}

		public virtual void Clear()
		{
			_innerList.RemoveAllChildren();
			_items.Clear();
		}

		public override void Recalculate()
		{
			base.Recalculate();
			UpdateScrollbar();
		}

		public override void ScrollWheel(UIScrollWheelEvent evt)
		{
			if (_scrollbar != null)
				_scrollbar.ViewPosition -= evt.ScrollWheelValue;
		}

		public override void RecalculateChildren()
		{
			base.RecalculateChildren();
			float num = 0f;
			for (int i = 0; i < _items.Count; i++)
			{
				float num2 = (_items.Count == 1) ? 0f : ListPadding;
				_items[i].Top.Set(num, 0f);
				_items[i].Recalculate();
				CalculatedStyle outerDimensions = _items[i].GetOuterDimensions();
				num += outerDimensions.Height + num2;
			}

			_innerListHeight = num;
		}

		private void UpdateScrollbar()
		{
			if (_scrollbar != null)
			{
				float height = GetInnerDimensions().Height;
				_scrollbar.SetView(height, _innerListHeight);
			}
		}

		public void SetScrollbar(UIScrollbar scrollbar)
		{
			_scrollbar = scrollbar;
			UpdateScrollbar();
		}

		public void UpdateOrder()
		{
			if (ManualSortMethod != null)
				ManualSortMethod(_items);
			else
				_items.Sort(SortMethod);

			UpdateScrollbar();
		}

		public int SortMethod(UIElement item1, UIElement item2) => item1.CompareTo(item2);

		public override List<SnapPoint> GetSnapPoints()
		{
			List<SnapPoint> list = new List<SnapPoint>();
			if (GetSnapPoint(out SnapPoint point))
				list.Add(point);

			foreach (UIElement item in _items)
			{
				list.AddRange(item.GetSnapPoints());
			}

			return list;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (_scrollbar != null)
				_innerList.Top.Set(0f - _scrollbar.GetValue(), 0f);

			Recalculate();
		}

		public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();
	}
}
