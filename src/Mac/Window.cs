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
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace CivOne
{
	internal class Window : NSApplicationDelegate
	{
		private static NSApplication _app;
		private NSWindow _window;
		
		private void CreateMenu()
		{
			NSMenu mainMenu = new NSMenu();
			NSMenuItem appMenuItem = new NSMenuItem();
			mainMenu.AddItem(appMenuItem);
			
			NSMenu appMenu = new NSMenu();
			NSMenuItem quitMenuItem = new NSMenuItem("Quit", "q", delegate { NSApplication.SharedApplication.Terminate(mainMenu); });
			appMenu.AddItem(quitMenuItem);
			
			appMenuItem.Submenu = appMenu;
			
			NSApplication.SharedApplication.MainMenu = mainMenu;
		}
		
		public override void FinishedLaunching(NSObject notification)
		{
			CreateMenu();
			NSWindow _window = new NSWindow(new RectangleF(0, 0, 640, 400), NSWindowStyle.Titled, NSBackingStore.Buffered, false)
			{
				Title = "CivOne"
			};
			
			_window.CascadeTopLeftFromPoint(new PointF(20, 20));
			_window.MakeKeyAndOrderFront(null);
		}
		
		public static void CreateWindow(string screen)
		{
			NSApplication.Init();
			_app = NSApplication.SharedApplication;
			
			_app.Delegate = new Window(screen);
			_app.Run();
		}
		
		private Window(string screen)
		{
			
		}
	}
}