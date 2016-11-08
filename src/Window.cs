// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

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
		private uint _gameTick = 0;
		private Thread TickThread;
		private AutoResetEvent _tickWaiter = new AutoResetEvent(true);
		
		// Returns whether any changes have been made to the screen.
		private bool HasUpdate
		{
			get
			{
				if (!GameTask.Update() && (_gameTick % 4) > 0) return false;
				if (Common.Screens.Any(x => x is IModal))
					return Common.Screens.Last(x => x is IModal).HasUpdate(_gameTick / 4);
				return (Common.Screens.Count(x => x.HasUpdate(_gameTick / 4)) > 0);
			}
		}
		
		protected IScreen TopScreen
		{
			get
			{
				return Common.TopScreen;
			}
		}

		private readonly Picture _canvas = new Picture(320, 200);
		internal Picture Canvas
		{
			get
			{
				if (Common.Screens.Length == 0) return _canvas;

				Color[] palette = TopScreen.Canvas.Image.Palette.Entries;
				palette[0] = Color.Black;
				_canvas.FillRectangle(0, 0, 0, _canvas.Width, _canvas.Height);
				_canvas.SetPalette(palette);

				if (TopScreen is IModal)
				{
					_canvas.AddLayer(TopScreen.Canvas, 0, 0);
				}
				else
				{
					foreach (IScreen screen in Common.Screens)
					{
						_canvas.AddLayer(screen.Canvas, 0, 0);
					}
				}
				return _canvas;
			}
		}
		
		private void GameTick()
		{
			while (true)
			{
				_tickWaiter.WaitOne();
				RefreshWindow();
				_gameTick += (uint)(GameTask.Fast ? 1 : 4);
			}
		}
		
		private void SetGameTick()
		{
			new Thread(new ThreadStart(GameTick)).Start();
			while (true)
			{
				Thread.Sleep(1000 / (GameTask.Fast ? 60 : 15));
				_tickWaiter.Set();
			}
		}
		
		private int CanvasWidth
		{
			get
			{
				return (int)(ScaleX * 320);
			}
		}
		
		private int CanvasHeight
		{
			get
			{
				return (int)(ScaleY * 200);
			}
		}
		
		private void SaveScreen()
		{
			string filename = Common.CaptureFilename;
			if (filename == null) return;
			
			Picture capture = new Picture(_canvas);
			Picture.ReplaceColours(capture, 0, 5); 
			capture.Image.Save(filename, ImageFormat.Png);
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