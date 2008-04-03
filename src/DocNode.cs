// DocNode.cs: Documentation node abstract base class.
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

	internal delegate void NodeEventHandler (object o, NodeEventArgs args);

	internal class NodeEventArgs : EventArgs {

		DocNode node;

		public NodeEventArgs (DocNode node)
		{
			this.node = node;
		}

		public DocNode Node {
			get { return node; }
		}
	}

	internal abstract class DocNode {

		public abstract DocViewItem CreateItem (DocView view);

		List <DocNode> children;
		DocNode parent;

		protected DocNode (DocNode parent)
		{
			this.parent = parent;
		}

		protected DocNode (DocNode parent, string caption) : this (parent)
		{
			this.caption = caption;
		}

		public event EventHandler Changed;
		public event NodeEventHandler ChildAdded;
		public event NodeEventHandler ChildRemoved;

		string caption;
		public virtual string Caption {
			get { return caption; }
		}

		bool loaded = false;

		public int ChildCount { 
			get {
				if (!loaded)
					LazyLoad ();
				return children == null ? 0 : children.Count;
			}
		}

		public DocNode this [int index] { 
			get {
				if (!loaded)
					LazyLoad ();
				if (children == null || index > children.Count || index < 0)
					throw new ArgumentOutOfRangeException ("value");

				return children [index];
			}
		}

		public DocNode Parent {
			get { return parent; }
		}

		protected void OnChanged ()
		{
			if (Changed == null)
				return;
			Changed (this, EventArgs.Empty);
		}

		bool loading = false;

		void LazyLoad ()
		{
			loaded = true;
			loading = true;
			LoadChildren ();
			loading = false;
		}

		protected virtual int CompareNodes (DocNode a, DocNode b)
		{
			return String.Compare (a.Caption, b.Caption);
		}

		protected virtual void LoadChildren ()
		{
			loaded = true;
		}

		public int IndexOf (DocNode child)
		{
			if (!loaded)
				LazyLoad ();
			return children == null ? -1 : children.IndexOf (child);
		}

		public void AddChild (DocNode child)
		{
			if (children == null)
				children = new List <DocNode> ();
			int i = 0;
			while (i < children.Count)
				if (CompareNodes (children [i], child) > 0)
					break;
				else
					i++;
			children.Insert (i, child);
			if (ChildAdded == null || loading)
				return;
			ChildAdded (this, new NodeEventArgs (child));
		}

		public void RemoveChild (DocNode child)
		{
			children.Remove (child);
			if (ChildRemoved == null)
				return;
			ChildRemoved (this, new NodeEventArgs (child));
		}
	}
}

