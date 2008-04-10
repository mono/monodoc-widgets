// HeadingItem.cs: Page Heading DocView Item.
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

	internal class HeadingItem : DocViewItem {

		DocView view;
		string heading;

		public HeadingItem (DocView view, string heading)
		{
			this.heading = heading;
			this.view = view;
		}

		FontDescription desc;
		FontDescription FontDescription {
			get {
				if (desc == null) {
					desc = view.Layout.Context.FontDescription.Copy ();
					desc.Weight = Weight.Bold;
					desc.Size = (int) (desc.Size * Scale.Large);
				}
				return desc;
			}
		}

		Gdk.Point ink_offset;

		public override void Update (int width)
		{
			Layout layout = view.Layout;
			layout.FontDescription = FontDescription;
			layout.Alignment = Alignment.Left;
			layout.Width = -1;
			layout.SetText (heading);
			Rectangle ink, log;
			layout.GetPixelExtents (out ink, out log);
			ink_offset = new Gdk.Point (ink.X, ink.Y);
			layout.FontDescription = null;
			sz.Width = ink.Width;
			sz.Height = ink.Height;
		}

		protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
		{
			Pango.Layout layout = view.Layout;
			layout.FontDescription = FontDescription;
			layout.Alignment = Pango.Alignment.Left;
			layout.Width = -1;
			layout.SetText (heading);
			win.DrawLayout (view.Style.TextGC (0), Location.X + offset.X - ink_offset.X, Location.Y + offset.Y - ink_offset.Y, layout);
			layout.FontDescription = null;
		}

		public override void Event (EventInfo info)
		{
		}
	}
}
