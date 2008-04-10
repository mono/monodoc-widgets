// SummaryTableItem.cs: SummaryTable DocView Item.
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
	using Pango;

	internal interface ITableSummary {

		string Name { get; }

		System.Xml.XmlElement Summary { get; }
	}

	internal class SummaryTableItem : DocViewItem {

		class RowItem : DocViewItem {

			DocView view;
			DocViewItem caption;
			DocViewItem summ;
			int cap_width;

			public RowItem (DocView view, ITableSummary node)
			{
				this.view = view;
				caption = new LinkItem (view, node.Name, node as DocNode);
				summ = new KitchenSinkItem (view, node.Summary);
			}

			public DocViewItem CaptionItem {
				get { return caption; }
			}

			public int CaptionWidth {
				set { cap_width = value; }
			}

			public override void Update (int width)
			{
				sz.Width = width;
				summ.Update (width - cap_width - 2 * Padding);
				if (summ.Size.Height > caption.Size.Height)
					sz.Height = summ.Size.Height;
				else
					sz.Height = caption.Size.Height;
				sz.Height += 2 * Padding;
				caption.Location = new Gdk.Point (Padding, Padding);
				summ.Location = new Gdk.Point (cap_width + Padding, Padding);
			}

			protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
			{
				Gdk.Rectangle r = new Gdk.Rectangle (offset.X + Location.X, offset.Y + Location.Y, cap_width, sz.Height);
				if (r.IntersectsWith (clip)) {
					win.DrawRectangle (view.Style.MidGC (0), false, r);
					caption.Paint (win, Globalize (offset), clip);
				}
				r.X += cap_width;
				r.Width = sz.Width - cap_width - 1;
				if (r.IntersectsWith (clip)) {
					win.DrawRectangle (view.Style.MidGC (0), false, r);
					summ.Paint (win, Globalize (offset), clip);
				}
			}

			public override void Event (EventInfo info)
			{
				info.Position = Localize (info.Position);
				if (caption.Contains (info.Position))
					caption.Event (info);
				else if (summ.Contains (info.Position))
					summ.Event (info);
			}
		}

		DocView view;
		List<RowItem> rows = new List <RowItem> ();
		LabelItem label;
		string column_heading;

		public SummaryTableItem (DocView view, string label, string caption_heading, List<ITableSummary> rows)
		{
			this.view = view;
			this.label = new LabelItem (view, label);
			column_heading = caption_heading;
			foreach (ITableSummary node in rows)
				this.rows.Add (new RowItem (view, node));
		}

		const int table_offset = 20;
		int cap_width;
		Gdk.Rectangle heading_area;
		Gdk.Rectangle expander_area;
		bool expanded = true;

		public override void Update (int width)
		{
			expander_area = new Gdk.Rectangle (Padding, 0, view.LineHeight, view.LineHeight);
			label.Update (-1);
			label.Location = new Gdk.Point (table_offset, 0);

			if (!expanded) {
				sz = new Gdk.Size (width, label.Size.Height);
				return;
			}

			cap_width = 0;
			foreach (RowItem row in rows) {
				row.CaptionItem.Update (-1);
				if (row.CaptionItem.Size.Width > cap_width)
					cap_width = row.CaptionItem.Size.Width;
			}
			cap_width += 2 * Padding;
			width -= table_offset + Padding;
			int table_width;
			if (cap_width > (width / 2))
				table_width = 2 * cap_width;
			else
				table_width = width;
			sz.Width = table_width + table_offset + Padding;

			int height = label.Size.Height + Padding;
			heading_area = new Gdk.Rectangle (table_offset, height, table_width, view.LineHeight + Padding);
			height += heading_area.Height;
			foreach (RowItem row in rows) {
				row.CaptionWidth = cap_width;
				row.Update (table_width);
				row.Location = new Gdk.Point (table_offset, height);
				height += row.Size.Height;
			}
			sz.Height = height;
		}

		protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
		{
			Gdk.Point adj = Globalize (offset);
			Gtk.Style.PaintExpander (view.Style, win, Gtk.StateType.Normal, clip, view, null, adj.X + 10, adj.Y + 6, expanded ? Gtk.ExpanderStyle.Expanded : Gtk.ExpanderStyle.Collapsed);
			label.Paint (win, adj, clip);

			if (!expanded)
				return;

			Gdk.Rectangle r = heading_area;
			r.X += adj.X;
			r.Y += adj.Y;
			if (r.IntersectsWith (clip)) {
				win.DrawRectangle (view.Style.DarkGC (0), true, r);
				Pango.Layout layout = view.Layout;
				layout.Width = -1;
				layout.Alignment = Alignment.Left;
				layout.SetText (column_heading);
				win.DrawLayout (view.Style.TextGC (0), r.X + Padding, r.Y + Padding, layout);
				layout.SetText ("Description");
				win.DrawLayout (view.Style.TextGC (0), r.X + cap_width + Padding, r.Y + Padding, layout);
			}
				
			foreach (RowItem row in rows)
				row.Paint (win, adj, clip);
		}

		public override void Event (EventInfo info)
		{
			info.Position = Localize (info.Position);
			if (expander_area.Contains (info.Position)) {
				expanded = !expanded;
				view.Reflow ();
				return;
			}
			foreach (RowItem row in rows)
				if (row.Contains (info.Position))
					row.Event (info);
		}
	}
}
