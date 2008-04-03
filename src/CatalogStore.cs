// CatalogStore.cs - Documentation catalog tree model.
//
// Author: Mike Kestner  <mkestner@novell.com>
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
	using System.Collections;
	using System.Runtime.InteropServices;
	using Gtk;

	internal class CatalogStore : GLib.Object, TreeModelImplementor {

		DocNode root_node;

		public CatalogStore (DocNode root_node)
		{
			this.root_node = root_node;
		}

		DocNode GetNodeAtPath (TreePath path)
		{
			if (path.Indices.Length <= 0 || path.Indices [0] != 0)
				return null;

			DocNode result = root_node;
			for (int i = 1; i < path.Indices.Length; i++) {
				if (result.ChildCount <= path.Indices[i])
					return null;
				result = result [path.Indices [i]] as DocNode;
			}

			return result;
		}

		Hashtable node_hash = new Hashtable ();

		public TreeModelFlags Flags {
			get {
				return TreeModelFlags.ItersPersist;
			}
		}

		public int NColumns {
			get {
				return 1;
			}
		}

		public GLib.GType GetColumnType (int col)
		{
			return GLib.GType.String;
		}

		TreeIter IterFromNode (DocNode node)
		{
			GCHandle gch;
			if (node_hash [node] != null)
				gch = (GCHandle) node_hash [node];
			else {
				gch = GCHandle.Alloc (node);
				node_hash [node] = gch;
				node.Changed += new EventHandler (OnNodeChanged);
				node.ChildAdded += new NodeEventHandler (OnChildAdded);
				node.ChildRemoved += new NodeEventHandler (OnChildRemoved);
			}
			TreeIter result = TreeIter.Zero;
			result.UserData = (IntPtr) gch;
			return result;
		}

		void OnNodeChanged (object o, EventArgs args)
		{
			TreeModelAdapter adapter = new TreeModelAdapter (this);
			adapter.EmitRowChanged (PathFromNode (o as DocNode), IterFromNode (o as DocNode));
		}

		void OnChildAdded (object o, NodeEventArgs args)
		{
			TreeModelAdapter adapter = new TreeModelAdapter (this);
			adapter.EmitRowInserted (PathFromNode (args.Node), IterFromNode (args.Node));
		}

		void OnChildRemoved (object o, NodeEventArgs args)
		{
			TreeModelAdapter adapter = new TreeModelAdapter (this);
			adapter.EmitRowDeleted (PathFromNode (args.Node));
		}

		internal DocNode NodeFromIter (TreeIter iter)
		{
			GCHandle gch = (GCHandle) iter.UserData;
			return gch.Target as DocNode;
		}

		internal TreePath PathFromNode (DocNode node)
		{
			if (node == null)
				return new TreePath ();

			DocNode work = node;
			TreePath path = new TreePath ();
			while (work != root_node) {
				DocNode parent = work.Parent;
				path.PrependIndex (parent.IndexOf (work));
				work = parent;
			}
			path.PrependIndex (0);
			return path;
		}

		public bool GetIter (out TreeIter iter, TreePath path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");

			iter = TreeIter.Zero;

			DocNode node = GetNodeAtPath (path);
			if (node == null)
				return false;

			iter = IterFromNode (node);
			return true;
		}

		public TreePath GetPath (TreeIter iter)
		{
			DocNode node = NodeFromIter (iter);
			if (node == null) 
				throw new ArgumentException ("iter");

			return PathFromNode (node);
		}

		public void GetValue (TreeIter iter, int col, ref GLib.Value val)
		{
			DocNode node = NodeFromIter (iter);
			if (node == null)
				return;

			val = new GLib.Value (node.Caption);
		}

		public bool IterNext (ref TreeIter iter)
		{
			DocNode node = NodeFromIter (iter);
			if (node == null || node == root_node)
				return false;

			int idx = node.Parent.IndexOf (node) + 1;
			if (idx < node.Parent.ChildCount) {
				iter = IterFromNode (node.Parent [idx]);
				return true;
			}
			return false;
		}

		public bool IterChildren (out TreeIter child, TreeIter parent)
		{
			child = TreeIter.Zero;

			if (parent.UserData == IntPtr.Zero) {
				child = IterFromNode (root_node);
				return true;
			}

			DocNode node = NodeFromIter (parent);
			if (node == null || node.ChildCount <= 0)
				return false;

			child = IterFromNode (node [0]);
			return true;
		}

		public bool IterHasChild (TreeIter iter)
		{
			DocNode node = NodeFromIter (iter);
			if (node == null || node.ChildCount <= 0)
				return false;

			return true;
		}

		public int IterNChildren (TreeIter iter)
		{
			if (iter.UserData == IntPtr.Zero)
				return 1;
				
			DocNode node = NodeFromIter (iter);
			return node == null ? 0 : node.ChildCount;
		}

		public bool IterNthChild (out TreeIter child, TreeIter parent, int n)
		{
			child = TreeIter.Zero;

			if (parent.UserData == IntPtr.Zero) {
				if (n != 0)
					return false;
				child = IterFromNode (root_node);
				return true;
			}
				
			DocNode node = NodeFromIter (parent);
			if (node == null || node.ChildCount <= n)
				return false;

			child = IterFromNode (node [n]);
			return true;
		}

		public bool IterParent (out TreeIter parent, TreeIter child)
		{
			parent = TreeIter.Zero;
			DocNode node = NodeFromIter (child);
			if (node == null || node == root_node)
				return false;

			parent = IterFromNode (node.Parent);
			return true;
		}

		public void RefNode (TreeIter iter)
		{
		}

		public void UnrefNode (TreeIter iter)
		{
		}
	}
}
