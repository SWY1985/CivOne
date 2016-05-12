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
using System.Drawing.Imaging;
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
		
		private void GameTick()
		{
			RefreshGame();
			_gameTick++;
			_tickWaiter.Set();
		}
		
		private void SetGameTick()
		{
			while (true)
			{
				// if the previous tick is still busy, step out... this will cause the game to slow down a bit
				if (!_tickWaiter.WaitOne(25)) continue;
				_tickWaiter.Reset();
				
				new Thread(new ThreadStart(GameTick)).Start();
				Thread.Sleep(1000 / Settings.Instance.FramesPerSecond);
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
		
		private void Init(string screen)
		{
			if (!FileSystem.DataFilesExist())
			{
				FileSystem.CopyDataFiles(BrowseDataFolder());
				Environment.Exit(0);
			}
			
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
		}
	}
}