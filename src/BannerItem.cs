// BannerItem.cs: Banner DocView Item.
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

	internal class BannerItem : DocViewItem {

		DocView view;
		HeadingItem heading;
		List<LinkItem> links = new List <LinkItem> ();

		public BannerItem (DocView view, DocNode node)
		{
			this.view = view;
			heading = new HeadingItem (view, node.Caption);
			while (node.Parent != null) {
				links.Insert (0, new LinkItem (view, node.Parent.Caption, node.Parent));
				node = node.Parent;
			}
		}

		public override void Update (int width)
		{
			sz.Width = width;
			int x = Padding;
			int height = Padding;
			foreach (LinkItem link in links) {
				link.Update (width);
				if (x + link.Size.Width > width) {
					x = Padding;
					height += link.Size.Height + Padding;
				}
				link.Location = new Gdk.Point (x, height);
				x += link.Size.Width;
			}
			if (links.Count > 0)
				height += links [0].Size.Height + 2 * Padding;
			else
				height += view.LineHeight;
			heading.Update (width);
			heading.Location = new Gdk.Point (Padding, height);
			sz.Height = height + heading.Size.Height + Padding;
		}

		protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
		{
			Gtk.Style.PaintBox (view.Style, win, Gtk.StateType.Selected, Gtk.ShadowType.EtchedOut, clip, view, "MonodocWidgetsDocView", Location.X + offset.X, Location.Y + offset.Y, sz.Width, sz.Height);
			Gdk.Point adj = Globalize (offset);
			heading.Paint (win, adj, clip);
			foreach (LinkItem link in links)
				link.Paint (win, adj, clip);
		}

		public override void Event (EventInfo info)
		{
			info.Position = Localize (info.Position);
			foreach (LinkItem link in links)
				if (link.Contains (info.Position))
					link.Event (info);
		}
	}
}
