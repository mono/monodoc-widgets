// DocViewItem.cs: DocView item base class 
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
	using Gtk;

	internal struct EventInfo {
		public Gdk.Point Position;
		public uint Button;
		public uint Clicks;

		public EventInfo (Gdk.Point pos, uint button, uint clicks)
		{
			Position = pos;
			Button = button;
			Clicks = clicks;
		}
	}

	internal abstract class DocViewItem {

		protected const int Padding = 6;

		Gdk.Rectangle Bounds {
			get { return new Gdk.Rectangle (loc, sz); }
		}

		Gdk.Point loc;
		public Gdk.Point Location {
			get { return loc; }
			set { loc = value; }
		}

		protected Gdk.Size sz;
		public Gdk.Size Size { 
			get { return sz; }
		}

		public abstract void Event (EventInfo info);

		protected abstract void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip);

		public abstract void Update (int width);

		public bool Contains (Gdk.Point pt)
		{
			return Bounds.Contains (pt);
		}

		bool Intersects (Gdk.Point offset, Gdk.Rectangle clip)
		{
			Gdk.Rectangle bounds = Bounds;
			bounds.X += offset.X;
			bounds.Y += offset.Y;
			return bounds.IntersectsWith (clip);
		}

		public void Paint (Gdk.Drawable drawable, Gdk.Point offset, Gdk.Rectangle clip)
		{
			if (Intersects (offset, clip))
				OnPaint (drawable, offset, clip);
		}

		public Gdk.Point Localize (Gdk.Point pt)
		{
			return new Gdk.Point (pt.X - loc.X, pt.Y - loc.Y);
		}

		public Gdk.Point Globalize (Gdk.Point pt)
		{
			return new Gdk.Point (pt.X + loc.X, pt.Y + loc.Y);
		}
	}
}
