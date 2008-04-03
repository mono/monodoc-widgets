// RemarksItem.cs: Remarks DocView Item.
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
	using System.Xml;
	using Pango;

	internal class RemarksItem : DocViewItem {

		LabelItem label;
		KitchenSinkItem contents;

		public RemarksItem (DocView view, XmlElement elem)
		{
			label = new LabelItem (view, "Remarks:");
			contents = new KitchenSinkItem (view, elem);
		}

		const int text_padding = 6;
		const int item_offset = 14;

		public override void Update (int width)
		{
			sz.Width = width;
			label.Update (width);
			label.Location = Gdk.Point.Zero;
			width -= item_offset;
			int height = label.Size.Height + text_padding;
			contents.Update (width);
			contents.Location = new Gdk.Point (item_offset, height);
			sz.Height = height + contents.Size.Height;
		}

		protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
		{
			Gdk.Point adj = Globalize (offset);
			label.Paint (win, adj, clip);
			contents.Paint (win, adj, clip);
		}

		public override void Event (EventInfo info)
		{
			info.Position = Localize (info.Position);
			if (contents.Contains (info.Position))
				contents.Event (info);
		}
	}
}
