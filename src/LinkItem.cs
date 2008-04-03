// LinkItem.cs: DocView Link Item.
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
	using Pango;

	internal class LinkItem : DocViewItem {

		string caption;
		DocNode target;
		DocView view;

		public LinkItem (DocView view, string caption, DocNode target)
		{
			this.caption = caption;
			this.target = target;
			this.view = view;
		}

		public override void Event (EventInfo info)
		{
			view.Tree.SelectedNode = target;
		}

		AttrList attrs;
		AttrList Attrs {
			get {
				if (attrs == null) {
					attrs = new AttrList ();
					Pango.Attribute attr = new AttrUnderline (Underline.Single);
					attr.EndIndex = UInt32.MaxValue;
					attrs.Insert (attr);
					attr = new AttrForeground (0, 0, 65535);
					attr.EndIndex = UInt32.MaxValue;
					attrs.Insert (attr);
				}
				return attrs;
			}
		}

		protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
		{
			Pango.Layout layout = view.Layout;
			layout.Alignment = Pango.Alignment.Left;
			layout.Width = -1;
			layout.SetText (caption);
			layout.Attributes = Attrs;
			win.DrawLayout (view.Style.TextGC (0), Location.X + offset.X, Location.Y + offset.Y, layout);
			layout.Attributes = null;
		}

		public override void Update (int width)
		{
			Layout layout = view.Layout;
			layout.Alignment = Pango.Alignment.Left;
			layout.Width = -1;
			layout.Attributes = Attrs;
			layout.SetText (caption);
			Rectangle ink, log;
			layout.GetPixelExtents (out ink, out log);
			sz.Width = ink.Width;
			sz.Height = ink.Height;
			layout.Attributes = null;
		}
	}
}
