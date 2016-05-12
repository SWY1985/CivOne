// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne
{
	internal partial class Window : NSWindow
	{
		private static NSApplicationDelegate _app;
		
		private bool _forceUpdate = false;
		
		private IScreen TopScreen
		{
			get
			{
				return Common.Screens.LastOrDefault();
			}
		}
		
		private int CanvasX
		{
			get
			{
				return 0;
			}
		}
		
		private int CanvasY
		{
			get
			{
				return 0;
			}
		}
		
		private int ScaleX
		{
			get
			{
				return 2;
			}
		}
		
		private int ScaleY
		{
			get
			{
				return 2;
			}
		}
		
		private void ScreenUpdate()
		{
			ContentView.NeedsDisplay = true;
		}
		
		private void RefreshGame()
		{
			if (TickThread.IsAlive && Common.EndGame)
			{
				TickThread.Abort();
			}
			
			if (!TickThread.IsAlive)
			{
				Dispose();
				return;
			}
			
			// Refresh the screen if there's an update
			if (_forceUpdate || Common.Screens.Count(x => x.HasUpdate(_gameTick)) > 0) InvokeOnMainThread(new NSAction(ScreenUpdate));
			_forceUpdate = false;
		}
		
		private void ScaleMouseEventArgs(ref MouseEventArgs args)
		{
			int xx = args.X - CanvasX, yy = args.Y - CanvasY;
			args = new MouseEventArgs(args.Button, args.Clicks, (int)Math.Floor((float)xx / ScaleX), (int)Math.Floor((float)yy / ScaleY), args.Delta);
		}
		
		private void MouseDown(object sender, MouseEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseDown(args);
		}
		
		private void MouseUp(object sender, MouseEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseUp(args);
		}
		
		public static void CreateWindow(string screen)
		{
			NSApplication.Init();
			_app = new Application(screen);
		}
		
		public Window(string screen) : base(new RectangleF(0, 0, 640, 400), NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable, NSBackingStore.Buffered, false)
		{
			// Setup the application window
			Title = "CivOne";
			CascadeTopLeftFromPoint(new PointF(20, 20));
			MakeKeyAndOrderFront(null);
			ContentView = new View();
			
			// Set View events
			(ContentView as View).OnMouseDown += MouseDown;
			(ContentView as View).OnMouseUp += MouseUp;
			
			// Load the first screen
			Init(screen);
			
			// Start tick thread
			TickThread = new Thread(new ThreadStart(SetGameTick));
			TickThread.Start();
		}
	}
}