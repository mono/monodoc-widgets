// NamespaceNode.cs: Namespace node.
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

	using Pango;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Xml;

	internal class NamespaceNode : DocNode {

		string path;
		DirectoryInfo dir;
		XmlDocument ns_doc;
		XmlElement index_elem;

		public NamespaceNode (DocNode parent, string path, string dir_path, XmlElement index_elem) : base (parent)
		{
			this.path = path;
			this.dir = new DirectoryInfo (dir_path);
			this.index_elem = index_elem;
		}

		public override string Caption {
			get { return dir.Name + " Namespace"; }
		}

		DocsElement docs;
		public DocsElement Docs {
			get {
				if (docs == null) {
					ns_doc = new XmlDocument ();
					try {
						using (XmlTextReader rdr = new XmlTextReader (path))
							ns_doc.Load (rdr);
					} catch (Exception e) {
						Console.WriteLine ("Unable to load " + path);
						Console.WriteLine (e);
						return null;
					}
					docs = new DocsElement (ns_doc.SelectSingleNode ("Namespace/Docs") as XmlElement);
				}
				return docs;
			}
		}

		public string Name {
			get { return dir.Name; }
		}

		protected override void LoadChildren ()
		{
			foreach (XmlElement elem in index_elem.SelectNodes ("Type"))
				AddChild (new TypeNode (this, dir.FullName, elem));
		}

		public override DocViewItem CreateItem (DocView view)
		{
			return new ViewItem (view, this);
		}

		public void Save ()
		{
			try {
				using (FileStream fs = File.Create (path))
					ns_doc.Save (fs);
			} catch (Exception e) {
				Console.Error.WriteLine ("Error saving namespace: " + e);
			}
		}

		class ViewItem : DocViewItem {

			DocView view;
			BannerItem banner;
			KitchenSinkItem summary;
			RemarksItem remarks;
			List<SummaryTableItem> tables = new List<SummaryTableItem> ();

			public ViewItem (DocView view, NamespaceNode ns)
			{
				this.view = view;
				banner = new BannerItem (view, ns);
				summary = new KitchenSinkItem (view, ns.Docs.Summary);
				remarks = new RemarksItem (view, ns.Docs.Remarks);
				Dictionary<string, List<ITableSummary>> groups = CreateGroupHash ();
				for (int i = 0; i < ns.ChildCount; i++) {
					TypeNode t = ns [i] as TypeNode;
					groups [t.Kind].Add (t);
				}
				if (groups["Class"].Count > 0)
					tables.Add (new SummaryTableItem (view, "Classes", "Class", groups["Class"]));
				if (groups["Structure"].Count > 0)
					tables.Add (new SummaryTableItem (view, "Structures", "Structure", groups["Structure"]));
				if (groups["Interface"].Count > 0)
					tables.Add (new SummaryTableItem (view, "Interfaces", "Interface", groups["Interface"]));
				if (groups["Delegate"].Count > 0)
					tables.Add (new SummaryTableItem (view, "Delegates", "Delegate", groups["Delegate"]));
				if (groups["Enumeration"].Count > 0)
					tables.Add (new SummaryTableItem (view, "Enumerations", "Enumeration", groups["Enumeration"]));
			}

			Dictionary<string, List<ITableSummary>> CreateGroupHash ()
			{
				Dictionary<string, List<ITableSummary>> result = new Dictionary <string, List<ITableSummary>> ();
				result ["Class"] = new List<ITableSummary> ();
				result ["Structure"] = new List<ITableSummary> ();
				result ["Interface"] = new List<ITableSummary> ();
				result ["Delegate"] = new List<ITableSummary> ();
				result ["Enumeration"] = new List<ITableSummary> ();
				return result;
			}
			
			const int grp_indent = 20;

			public override void Update (int width)
			{
				sz.Width = width;
				banner.Update (width);
				banner.Location = Gdk.Point.Zero;
				int height = banner.Size.Height + Padding;
				summary.Update (width);
				summary.Location = new Gdk.Point (Padding, height);
				height += summary.Size.Height + view.LineHeight;
				remarks.Update (width);
				remarks.Location = new Gdk.Point (Padding, height);
				height += remarks.Size.Height + view.LineHeight;
				foreach (DocViewItem table in tables) {
					table.Update (width);
					table.Location = new Gdk.Point (0, height);
					height += table.Size.Height + view.LineHeight;
				}
				sz.Height = height;
			}

			protected override void OnPaint (Gdk.Drawable win, Gdk.Point offset, Gdk.Rectangle clip)
			{
				banner.Paint (win, offset, clip);
				summary.Paint (win, offset, clip);
				remarks.Paint (win, offset, clip);
				foreach (DocViewItem table in tables)
					table.Paint (win, offset, clip);
			}

			public override void Event (EventInfo info)
			{
				if (banner.Contains (info.Position))
					banner.Event (info);
				else if (summary.Contains (info.Position))
					summary.Event (info);
				else if (remarks.Contains (info.Position))
					remarks.Event (info);
				else
					foreach (DocViewItem table in tables)
						if (table.Contains (info.Position)) {
							table.Event (info);
							break;
						}
			}
		}
	}
}
