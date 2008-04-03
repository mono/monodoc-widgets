// MarkupItem.cs: Markup DocView Item.
//
// Handles text with <see>, <paramref>, and <typeparamref> nodes.
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

	internal class MarkupItem : DocViewItem {

		DocView view;
		uint offset = 0;
		string text;

		public MarkupItem (DocView view)
		{
			this.view = view;
		}

		public void AddNode (XmlNode node)
		{
			uint start;
			switch (node.Name) {
			case "#text":
				string result = System.Text.RegularExpressions.Regex.Replace (node.Value, @"\n\s*", " ");
				text += result.TrimStart ();
				offset = (uint) System.Text.Encoding.UTF8.GetByteCount (text);
				break;
			case "paramref":
			case "typeparamref":
				start = offset;
				text += (node as XmlElement).GetAttribute ("name");
				offset = (uint) System.Text.Encoding.UTF8.GetByteCount (text);
				InsertPRefAttrs (start, offset);
				text += " ";
				offset++;
				break;
			case "see":
				start = offset;
				XmlElement see = node as XmlElement;
				if (see.HasAttribute ("cref"))
					text += see.GetAttribute ("cref");
				else if (see.HasAttribute ("langword"))
					text += see.GetAttribute ("langword");
				offset = (uint) System.Text.Encoding.UTF8.GetByteCount (text);
				InsertSeeAttrs (start, offset);
				text += " ";
				offset++;
				break;
			default:
				throw new ArgumentException ("node");
			}
		}

		AttrList attrs;

		void InsertPRefAttrs (uint start, uint end)
		{
			if (attrs == null)
				attrs = new AttrList ();
			Pango.Attribute attr = new AttrStyle (Style.Italic);
			attr.StartIndex = start;
			attr.EndIndex = end;
			attrs.Insert (attr);
			attr = new AttrForeground (32000, 32000, 32000);
			attr.StartIndex = start;
			attr.EndIndex = end;
			attrs.Insert (attr);
		}

		void InsertSeeAttrs (uint start, uint end)
		{
			if (attrs == null)
				attrs = new AttrList ();
			Pango.Attribute attr = new AttrUnderline (Underline.Single);
			attr.StartIndex = start;
			attr.EndIndex = end;
			attrs.Insert (attr);
			attr = new AttrForeground (0, 0, 65535);
			attr.StartIndex = start;
			attr.EndIndex = end;
			attrs.Insert (attr);
		}

		public override void Update (int width)
		{
			sz.Width = width;
			Layout layout = view.Layout;
			layout.Alignment = Pango.Alignment.Left;
			layout.Width = width * 1024;
			if (attrs != null)
				layout.Attributes = attrs;
			layout.SetText (text);
			Rectangle ink, log;
			layout.GetPixelExtents (out ink, out log);
			sz.Height = ink.Height;
			layout.Attributes = null;
		}

		protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
		{
			Pango.Layout layout = view.Layout;
			layout.Alignment = Pango.Alignment.Left;
			layout.Width = sz.Width * 1024;
			if (attrs != null)
				layout.Attributes = attrs;
			layout.SetText (text);
			win.DrawLayout (view.Style.TextGC (0), Location.X + offset.X, Location.Y + offset.Y, layout);
			layout.Attributes = null;
		}

		public override void Event (EventInfo info)
		{
		}
	}
}

