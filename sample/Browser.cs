// Browser.cs - Monodoc editor application
//
// Author: Mike Kestner <mkestner@novell.com>
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


namespace Monodoc.Widgets.Samples {

	using System;
	using Gdk;
	using Gtk;
	using Monodoc.Widgets;

	public class Browser : Gtk.Window  {

		public static int Main (string[] args)
		{
			if (args.Length != 1) {
				Console.WriteLine ("Usage: browser <docdir>");
				return 1;
			}

			Application.Init ();
			Gtk.Window win = new Browser (args [0]);
			win.Show ();
			Application.Run ();
			return 0;
		}

		public Browser (string catalog_dir) : base ("Documentation Browser Sample")
		{
			DefaultSize = new Size (600, 400);
			Gtk.Paned paned = new Gtk.HPaned ();
			Gtk.ScrolledWindow sw = new ScrolledWindow ();
			DocTree tree = new DocTree (catalog_dir);
			sw.Add (tree);
			paned.Add1 (sw);
			sw = new ScrolledWindow ();
			DocView view = new DocView (tree);
			sw.AddWithViewport (view);
			paned.Add2 (sw);
			paned.Position = 250;
			paned.ShowAll ();
			Add (paned);
		}

		protected override bool OnDeleteEvent (Event ev)
		{
			Application.Quit ();
			return true;
		}
	}
}
