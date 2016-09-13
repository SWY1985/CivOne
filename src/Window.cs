// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using CivOne.IO;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne
{
	internal partial class Window
	{
		private Picture _canvas = null;
		
		private uint _gameTick = 0;
		private Thread TickThread;
		private AutoResetEvent _tickWaiter = new AutoResetEvent(true);
		
		// Returns whether any changes have been made to the screen.
		private bool HasUpdate
		{
			get
			{
				if (!GameTask.Update() && (_gameTick % 2) > 0) return false;
				if (Common.Screens.Any(x => x is IModal))
					return Common.Screens.Last(x => x is IModal).HasUpdate(_gameTick / 2);
				return (Common.Screens.Count(x => x.HasUpdate(_gameTick / 2)) > 0);
			}
		}
		
		protected IScreen TopScreen
		{
			get
			{
				if (Common.Screens.Any(x => x is IModal))
					return Common.Screens.Last(x => x is IModal);
				return Common.Screens.LastOrDefault();
			}
		}
		
		private void GameTick()
		{
			RefreshWindow();
			_gameTick++;
			_tickWaiter.Set();
		}
		
		private void SetGameTick()
		{
			while (true)
			{
				// if the previous tick is still busy, step out... this will cause the game to slow down a bit
				if (!_tickWaiter.WaitOne(10)) continue;
				_tickWaiter.Reset();
				
				new Thread(new ThreadStart(GameTick)).Start();
				Thread.Sleep(1000 / 30);
			}
		}
		
		private int CanvasWidth
		{
			get
			{
				return ScaleX * 320;
			}
		}
		
		private int CanvasHeight
		{
			get
			{
				return ScaleY * 200;
			}
		}
		
		private void SaveScreen()
		{
			string filename = Common.CaptureFilename;
			if (filename == null) return;
			
			using (Bitmap capture = (Bitmap)_canvas.Image.Clone())
			{
				Picture.ReplaceColours(capture, 0, 5); 
				capture.Save(filename, ImageFormat.Png);
			}
		}
		
		public static bool CheckFiles()
		{
			if (FileSystem.DataFilesExist())
				return true;
			FileSystem.CopyDataFiles(BrowseDataFolder());
			return FileSystem.DataFilesExist();
		}
		
		private static void LoadResources()
		{
			ThreadStart civilopediaDelegate = new ThreadStart(Reflect.PreloadCivilopedia);
			Thread civilopedia = new Thread(new ThreadStart(Reflect.PreloadCivilopedia))
			{
				IsBackground = true
			};
			
			civilopedia.Start();
		}
		
		private void Init(string screen)
		{
			// Load the first screen
			IScreen startScreen;
			switch (screen)
			{
				case "demo":
					startScreen = new Demo();
					break;
				case "setup":
					startScreen = new Setup();
					break;
				default:
					startScreen = new Credits();
					break;
			}
			Common.AddScreen(startScreen);
			
			LoadResources();
		}
	}
}