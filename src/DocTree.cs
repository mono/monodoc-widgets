// DocTree.cs: Mono documentation browser
//
// Author:  Mike Kestner  <mkestner@novell.com>
//
// Copyright (c) 2008 Novell, Inc.

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


using Gtk;
using System;
using System.Collections;

namespace Monodoc.Widgets {

	internal class NodeSelectedEventArgs : EventArgs {

		DocNode node;

		public NodeSelectedEventArgs (DocNode node)
		{
			this.node = node;
		}

		public DocNode Node {
			get { return node; }
		}
	}

	internal delegate void NodeSelectedEventHandler (object o, NodeSelectedEventArgs args);

	public class DocTree : TreeView  {

		CatalogStore store;

		public DocTree (string catalog_dir)
		{
			DocNode node = Catalog.Load (catalog_dir);
			if (node == null)
				throw new ArgumentException ("node");

			HeadersVisible = false;
			store = new CatalogStore (node);
			Model = new TreeModelAdapter (store);
			AppendColumn ("Name", new CellRendererText (), "text", 0);
			Selection.Changed += new EventHandler (OnSelectionChanged);
			ExpandToPath (new TreePath ("0"));
		}

		internal event NodeSelectedEventHandler NodeSelected;

		internal DocNode SelectedNode {
			get {
				Gtk.TreeIter iter;
				Gtk.TreeModel model;
				if (Selection.GetSelected (out model, out iter))
					return store.NodeFromIter (iter);
				else
					return null;
			}
			set {
				TreePath path = store.PathFromNode (value);
				ExpandToPath (path);
				ScrollToCell (path, null, true, 0.2f, 0.0f);
				Selection.SelectPath (path);
			}
		}

		void OnSelectionChanged  (object o, EventArgs args)
		{
			if (NodeSelected == null)
				return;

			Gtk.TreeIter iter;
			Gtk.TreeModel model;
			if (Selection.GetSelected (out model, out iter))
				NodeSelected (this, new NodeSelectedEventArgs (store.NodeFromIter (iter)));
		}
	}
}

