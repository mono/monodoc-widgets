// KitchenSinkItem.cs: KitchenSink DocView Item.
//
// Author:  Mike Kestner  <mkestner@novell.com>
//
// Copyright (c) 2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN 
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.


namespace Monodoc.Widgets {

	using System;
	using System.Collections.Generic;
	using System.Xml;

	[Flags]
	internal enum EditingFlags {
		NoPara,
	}

	internal class KitchenSinkItem : DocViewItem {

		DocView view;
		XmlElement elem;
		List<DocViewItem> items = new List<DocViewItem> ();
		MarkupItem markup_item;
		EditingFlags flags;

		public KitchenSinkItem (DocView view, XmlElement elem, EditingFlags flags) : this (view, elem)
		{
			this.flags = flags;
		}

		public KitchenSinkItem (DocView view, XmlElement elem)
		{
			this.elem = elem;
			this.view = view;
			ParseElem ();
		}

		MarkupItem MarkupItem {
			get {
				if (markup_item == null) {
					markup_item = new MarkupItem (view);
					items.Add (markup_item);
				}
				return markup_item;
			}
		}

		void ParseElem ()
		{
			foreach (XmlNode node in elem) {
				switch (node.Name) {
				case "#text":
				case "paramref":
				case "see":
				case "typeparamref":
					MarkupItem.AddNode (node);
					break;
				case "para":
					markup_item = null;
					items.Add (new KitchenSinkItem (view, node as XmlElement, EditingFlags.NoPara));
					break;
				case "block":
				case "c":
				case "code":
				case "ul":
				case "example":
				case "list":
				case "SPAN":
				case "sub":
				case "sup":
				case "onequarter":
					markup_item = null;
					throw new NotImplementedException (node.Name);
				default:
					markup_item = null;
					Console.WriteLine ("unexpected node: " + node.Name);
					break;
				}
			}
		}

		public override void Update (int width)
		{
			sz = Gdk.Size.Empty;
			if (items.Count == 0)
				return;

			sz.Width = width;
			foreach (DocViewItem item in items) {
				item.Update (width);
				item.Location = new Gdk.Point (0, sz.Height);
				sz.Height += item.Size.Height + view.LineHeight;
			}
			sz.Height -= view.LineHeight;
		}

		protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
		{
			Gdk.Point adj = Globalize (offset);
			foreach (DocViewItem item in items)
				item.Paint (win, adj, clip);
		}

		public override void Event (EventInfo info)
		{
			info.Position = Localize (info.Position);
			foreach (DocViewItem item in items)
				item.Event (info);
		}
	}
}
