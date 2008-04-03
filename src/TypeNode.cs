// TypeNode.cs: Type node.
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

	internal class TypeNode : DocNode, ITableSummary {

		string path;

		public TypeNode (DocNode parent, string dir_path, XmlElement index_elem) : base (parent)
		{
			name = index_elem.GetAttribute ("Name");
			this.path = Path.Combine (dir_path, name + ".xml");
			caption = name;
			kind = index_elem.GetAttribute ("Kind");
			if (kind == null || kind == String.Empty) {
				try {
					XmlDocument doc = new XmlDocument ();
					using (XmlTextReader rdr = new XmlTextReader (path))
						doc.Load (rdr);
					XmlElement elem = doc.SelectSingleNode ("Type/Base/BaseTypeName") as XmlElement;
					kind = GetKind (elem);
				} catch (Exception) {
				}
			}
			caption = name + " " + kind;
		}

		static string GetKind (XmlElement elem)
		{
			if (elem == null)
				return "Interface";

			switch (elem.InnerXml) {
			case "System.Delegate":
				return "Delegate";
			case "System.ValueType":
				return "Structure";
			case "System.Enum":
				return "Enumeration";
			case "System.Object":
			default:
				return "Class";
			}
		}

		string kind;
		public string Kind {
			get { return kind; }
		}

		string name;
		public string Name {
			get { return name; }
		}

		string caption;
		public override string Caption {
			get { return caption; }
		}

		XmlDocument doc;
		public XmlDocument Doc {
			get {
				if (doc == null) {
					doc = new XmlDocument ();
					using (XmlTextReader rdr = new XmlTextReader (path))
						doc.Load (rdr);
				}
				return doc;
			}
		}

		public XmlElement Summary {
			get { return Docs.Summary; }
		}

		protected override int CompareNodes (DocNode a, DocNode b)
		{
			if (a is MembersNode)
				return -1;
			else if (b is MembersNode)
				return 1;
			else
				return base.CompareNodes (a, b);
		}

		protected override void LoadChildren ()
		{
			switch (kind) {
			case "Delegate":
			case "Enumeration":
				break;
			default:
				AddChild (new MembersNode (this, path));
				break;
			}
		}

		public override DocViewItem CreateItem (DocView view)
		{
			return null;
		}

		DocsElement docs;
		public DocsElement Docs {
			get {
				if (docs == null)
					docs = new DocsElement (Doc.SelectSingleNode ("Type/Docs") as XmlElement);
				return docs;
			}
		}
	}
}
