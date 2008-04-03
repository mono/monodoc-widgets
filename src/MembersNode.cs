// MembersNode.cs: Members node.
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
	using System.IO;
	using System.Xml;

	internal class MembersNode : DocNode {

		string path;

		public MembersNode (TypeNode parent, string path) : base (parent, parent.Name + " Members")
		{
			this.path = path;
		}

		TypeNode TypeNode {
			get { return Parent as TypeNode; }
		}

		OverloadNode ctors;
		OverloadNode Ctors {
			get {
				if (ctors == null) {
					ctors = new OverloadNode (Parent as TypeNode, (Parent as TypeNode).Name + " Constructor");
					Parent.AddChild (ctors);
				}
				return ctors;
			}
		}

		MemberSubsetNode events;
		MemberSubsetNode Events {
			get {
				if (events == null) {
					events = new MemberSubsetNode (TypeNode, TypeNode.Name + " Events");
					Parent.AddChild (events);
				}
				return events;
			}
		}

		MemberSubsetNode fields;
		MemberSubsetNode Fields {
			get {
				if (fields == null) {
					fields = new MemberSubsetNode (TypeNode, TypeNode.Name + " Fields");
					Parent.AddChild (fields);
				}
				return fields;
			}
		}

		MemberSubsetNode methods;
		MemberSubsetNode Methods {
			get {
				if (methods == null) {
					methods = new MemberSubsetNode (TypeNode, TypeNode.Name + " Methods");
					Parent.AddChild (methods);
				}
				return methods;
			}
		}

		MemberSubsetNode props;
		MemberSubsetNode Props {
			get {
				if (props == null) {
					props = new MemberSubsetNode (TypeNode, TypeNode.Name + " Properties");
					Parent.AddChild (props);
				}
				return props;
			}
		}


		protected override void LoadChildren ()
		{
			GLib.Idle.Add (new GLib.IdleHandler (InsertSiblings));
		}

		Dictionary <string, OverloadNode> method_hash;
		Dictionary <string, OverloadNode> MethodHash {
			get {
				if (method_hash == null)
					method_hash = new Dictionary <string, OverloadNode> ();
				return method_hash;
			}
		}

		bool InsertSiblings ()
		{
			XmlDocument doc = new XmlDocument ();
			using (XmlTextReader rdr = new XmlTextReader (path))
				doc.Load (rdr);

			foreach (XmlElement mtype in doc.DocumentElement.SelectNodes ("Members/Member/MemberType")) {
				XmlElement member = mtype.ParentNode as XmlElement;
				string mname = member.GetAttribute ("MemberName");
				switch (mtype.InnerXml) {
				case "Constructor":
					Ctors.AddOverload (member);
					break;
				case "Event":
					Events.AddChild (new MemberNode (Events, mname + " Event", member));
					break;
				case "Field":
					Fields.AddChild (new MemberNode (Fields, mname + " Field", member));
					break;
				case "Method":
					OverloadNode node;
					if (MethodHash.ContainsKey (mname))
						MethodHash [mname].AddOverload (member);
					else {
						node = new OverloadNode (Methods, mname + " Method");
						node.AddOverload (member);
						MethodHash [mname] = node;
						Methods.AddChild (node);
					}
					break;
				case "Property":
					Props.AddChild (new MemberNode (Props, mname + " Property", member));
					break;
				default:
					Console.WriteLine ("unexpected member type: " + mtype.InnerXml);
					break;
				}
			}

			return false;
		}

		public override DocViewItem CreateItem (DocView view)
		{
			return null;
		}
	}
}
