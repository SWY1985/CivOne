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
	internal class Application : NSApplicationDelegate
	{
		private readonly string _startScreen;
		
		private static NSApplication _app;
		private static NSWindow _window;
		
		private void CreateMenu()
		{
			NSMenu mainMenu = new NSMenu();
			NSMenuItem appMenuItem = new NSMenuItem();
			mainMenu.AddItem(appMenuItem);
			
			NSMenu appMenu = new NSMenu();
			NSMenuItem quitMenuItem = new NSMenuItem("Quit", "q", delegate { NSApplication.SharedApplication.Terminate(mainMenu); });
			appMenu.AddItem(quitMenuItem);
			
			appMenuItem.Submenu = appMenu;
			
			_app.MainMenu = mainMenu;
		}
		
		public override void FinishedLaunching(NSObject notification)
		{
			CreateMenu();
			
			_window = new Window(_startScreen);
		}
		
		public Application(string screen)
		{
			_startScreen = screen;
			
			_app = NSApplication.SharedApplication;
			_app.Delegate = this;
			_app.Run();
		}
	}
}