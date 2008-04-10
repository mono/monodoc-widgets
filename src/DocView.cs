// DocView.cs: Document View widget.
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
	using Gdk;
	using Gtk;

	public class DocView : DrawingArea {

		public DocView (DocTree tree)
		{
			this.tree = tree;
			CanFocus = true;
			Events = EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.ExposureMask | EventMask.KeyPressMask | EventMask.PointerMotionMask | EventMask.LeaveNotifyMask;
			layout = CreatePangoLayout (String.Empty);
			tree.NodeSelected += new NodeSelectedEventHandler (OnNodeSelected);
		}

		void OnNodeSelected (object o, NodeSelectedEventArgs args)
		{
			Item = args.Node.CreateItem (this);
			QueueDraw ();
		}

		Pango.Layout layout;
		internal Pango.Layout Layout {
			get { return layout; }
		}

		DocViewItem item;
		internal DocViewItem Item {
			get { return item; }
			set {
				if (item == value)
					return;
				item = value;
				if (item != null) {
					item.Update (Allocation.Width);
					SetSizeRequest (item.Size.Width, item.Size.Height);
				}
				QueueDraw ();
			}
		}

		int line_height = -1;
		internal int LineHeight {
			get {
				if (line_height == -1) {
					layout.Width = -1;
					layout.SetText ("Testing Height");
					Pango.Rectangle ink, log;
					layout.GetPixelExtents (out ink, out log);
					line_height = ink.Height + 6;
				}
				return line_height;
			}
		}

		DocTree tree;
		internal DocTree Tree {
			get { return tree; }
		}

		internal void Reflow ()
		{
			if (item == null)
				return;
			item.Update (Allocation.Width);
			QueueDraw ();
		}

		protected override bool OnExposeEvent (EventExpose ev)
		{
			base.OnExposeEvent (ev);

			if (item == null) {
				layout.SetText (Tree.SelectedNode.Caption + "\n\nNode renderer not implemented.");
				ev.Window.DrawLayout (Style.TextGC (0), 6, 6, layout);
			} else
				item.Paint (ev.Window, new Gdk.Point (0, 0), ev.Area);
			return true;
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			if (item != null)
				item.Update (allocation.Width);
		}

		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			if (Item == null)
				return;
			requisition.Width = Item.Size.Width;
			requisition.Height = Item.Size.Height;
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			if (item == null)
				return base.OnButtonPressEvent (ev);

			Point pt = new Point ((int)ev.X, (int)ev.Y);
			item.Event (new EventInfo (pt, ev.Button, 1));
			GrabFocus ();
			return base.OnButtonPressEvent (ev);
		}
	}
}
