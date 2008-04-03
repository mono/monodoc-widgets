// OverloadNode.cs: Overloaded member node.
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

	internal class OverloadNode : DocNode {

		XmlElement member;

		public OverloadNode (DocNode parent, string caption) : base (parent, caption)
		{
		}

		string GetChildCaption (XmlElement child)
		{
			string result = Caption + " (";
			bool comma = false;
			foreach (XmlElement param in child.SelectNodes ("Parameters/Parameter")) {
				result += (comma ? ", " : "" ) + param.GetAttribute ("Type");
				comma = true;
			}
			return result += ")";
		}

		public void AddOverload (XmlElement child)
		{
			if (member == null) {
				member = child;
				return;
			}

			if (ChildCount == 0)
				AddChild (new MemberNode (this, GetChildCaption (member), member));
			AddChild (new MemberNode (this, GetChildCaption (child), child));
		}

		public override DocViewItem CreateItem (DocView view)
		{
			return null;
		}
	}
}
