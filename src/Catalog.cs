// Catalog.cs: Documentation Catalog.
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
	using System.IO;
	using System.Xml;
	using System.Xml.Serialization;

	internal class Catalog : DocNode {

		string path;
		XmlDocument index_doc;

		public Catalog () : base (null, "Monodoc Catalog") {}

		public override DocViewItem CreateItem (DocView view)
		{
			return new ViewItem (view, this);
		}

		protected override void LoadChildren ()
		{
			
			foreach (XmlElement ns in index_doc.DocumentElement.SelectNodes ("Types/Namespace")) {
				string ns_dir = Path.Combine (path, ns.GetAttribute ("Name"));
				string file = Path.Combine (path, "ns-" + ns.GetAttribute ("Name") + ".xml");
				if (!File.Exists (file))
					file = ns_dir + ".xml";
				if (!File.Exists (file))
					continue;
				AddChild (new NamespaceNode (this, file, ns_dir, ns));
			}
		}

		public static Catalog Load (string directory_path)
		{
			if (!Directory.Exists (directory_path))
				return null;

			string index_file = System.IO.Path.Combine (directory_path, "index.xml");
			if (!File.Exists (index_file))
				return null;

			XmlDocument doc = new XmlDocument ();
			using (XmlTextReader rdr = new XmlTextReader (index_file))
				doc.Load (rdr);

			Catalog result = new Catalog ();
			result.path = directory_path;
			result.index_doc = doc;
			return result;
		}

		public void Save ()
		{
			try {
				if (path == null)
					throw new Exception ("Path not set");

				string cat_file = System.IO.Path.Combine (path, ".catalog.xml");
				XmlSerializer serializer = new XmlSerializer (typeof (Catalog));
				using (FileStream fs = File.Create (cat_file))
					serializer.Serialize (fs, this);
			} catch (Exception e) {
				Console.Error.WriteLine ("Error saving settings: " + e);
			}
		}


		class NSItem : DocViewItem {

			LinkItem link;
			KitchenSinkItem summ;

			public NSItem (DocView view, NamespaceNode ns)
			{
				link = new LinkItem (view, ns.Caption, ns);
				summ = new KitchenSinkItem (view, ns.Docs.Summary);
			}

			public override void Event (EventInfo info)
			{
				info.Position = Localize (info.Position);
				if (link.Contains (info.Position))
					link.Event (info);
				else if (summ.Contains (info.Position))
					summ.Event (info);
			}

			protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
			{
				Gdk.Point local = Globalize (offset);
				link.Paint (win, local, clip);
				summ.Paint (win, local, clip);
			}

			public override void Update (int width)
			{
				sz.Width = width;
				link.Update (width);
				if (link.Size.Width > sz.Width)
					sz.Width = link.Size.Width;
				link.Location = Gdk.Point.Zero;
				sz.Height = link.Size.Height + Padding;
				summ.Update (width);
				summ.Location = new Gdk.Point (0, sz.Height);
				sz.Height += summ.Size.Height;
			}
		}

		class ViewItem : DocViewItem {
			NSItem[] namespaces;
			BannerItem banner;
			LabelItem ns_label;

			public ViewItem (DocView view, Catalog catalog)
			{
				banner = new BannerItem (view, catalog);
				ns_label = new LabelItem (view, "Namespaces:");
				namespaces = new NSItem [catalog.ChildCount];
				for (int i = 0; i < catalog.ChildCount; i++)
					namespaces [i] = new NSItem (view, catalog [i] as NamespaceNode);
			}

			const int ns_indent = 20;

			public override void Update (int width)
			{
				sz.Width = width;
				banner.Update (width);
				banner.Location = Gdk.Point.Zero;
				int height = banner.Size.Height + 2 * Padding;
				ns_label.Update (width);
				int line_height = ns_label.Size.Height;
				height += line_height + Padding;
				ns_label.Location = new Gdk.Point (Padding, height);
				height += line_height + Padding;
				width -= ns_indent;
				for (int i = 0; i < namespaces.Length; i++) {
					namespaces [i].Update (width);
					namespaces [i].Location = new Gdk.Point (ns_indent, height + Padding);
					height += namespaces [i].Size.Height + line_height;
				}
				sz.Height = height;
			}

			protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
			{
				banner.Paint (win, offset, clip);
				ns_label.Paint (win, offset, clip);
				foreach (NSItem item in namespaces)
					item.Paint (win, offset, clip);
			}

			public override void Event (EventInfo info)
			{
				foreach (NSItem ns in namespaces) {
					if (ns.Contains (info.Position))
						ns.Event (info);
				}
			}
		}
	}
}
