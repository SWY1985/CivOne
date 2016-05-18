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
using CivOne.Enums;
using CivOne.Events;
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
		
		private void RefreshWindow()
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
			if (HasUpdate || _forceUpdate) InvokeOnMainThread(new NSAction(ScreenUpdate));
			_forceUpdate = false;
		}
		
		private void ScaleMouseEventArgs(ref ScreenEventArgs args)
		{
			int xx = args.X - CanvasX, yy = args.Y - CanvasY;
			args = new ScreenEventArgs((int)Math.Floor((float)xx / ScaleX), (int)Math.Floor((float)yy / ScaleY), args.Buttons);
		}
		
		private void MouseDown(object sender, ScreenEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseDown(args);
		}
		
		private void MouseUp(object sender, ScreenEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseUp(args);
		}
		
		private static string BrowseDataFolder()
		{
			Init();
			NSOpenPanel openPanel = new NSOpenPanel()
			{
				ReleasedWhenClosed = true,
				Prompt = "Select",
				CanChooseDirectories = true,
				CanChooseFiles = false,
				Title = "Select the folder containing the original Civilization data files."
			};
			if (openPanel.RunModal() == 1)
			{
				return openPanel.Url.Path;
			}
			return Settings.Instance.DataDirectory;
		}
		
		public static void CreateWindow(string screen)
		{
			Init();
			_app = new Application(screen);
		}
		
		private static bool _init = false;
		private static void Init()
		{
			if (_init) return;
			
			NSApplication.Init();
			_init = true;
		}
		
		public Window(string screen) : base(new RectangleF(0, 0, 640, 400), NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable, NSBackingStore.Buffered, false)
		{
			// Load the first screen
			Init(screen);
			
			// Setup the application window
			Title = "CivOne";
			CascadeTopLeftFromPoint(new PointF(20, 20));
			MakeKeyAndOrderFront(null);
			ContentView = new View();
			
			// Set View events
			(ContentView as View).OnMouseDown += MouseDown;
			(ContentView as View).OnMouseUp += MouseUp;
			
			// Start tick thread
			TickThread = new Thread(new ThreadStart(SetGameTick));
			TickThread.Start();
		}
	}
}