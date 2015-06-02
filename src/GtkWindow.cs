// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

#if GTK
using Gtk;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne
{
	internal class GtkWindow : IDisposable
	{
		private readonly Window _window;
		private readonly GtkGraphics _graphics;
		
		private Picture _canvas = null;
		
		private uint _gameTick = 0;
		private Thread TickThread;
		
		private AutoResetEvent _tickWaiter = new AutoResetEvent(true);
		
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
				int sizeW, sizeH;
				_window.GetSize(out sizeW, out sizeH);
				return (sizeW - CanvasWidth) / 2;
			}
		}
		
		private int CanvasY
		{
			get
			{
				int sizeW, sizeH;
				_window.GetSize(out sizeW, out sizeH);
				return (sizeH - CanvasHeight) / 2;
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
		
		private int ScaleX
		{
			get
			{
				int sizeW, sizeH;
				_window.GetSize(out sizeW, out sizeH);
				return (int)Math.Floor((float)sizeW / 320);
			}
		}
		
		private int ScaleY
		{
			get
			{
				int sizeW, sizeH;
				_window.GetSize(out sizeW, out sizeH);
				return (int)Math.Floor((float)sizeH / 200);
			}
		}
		
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
		
		private void ScreenUpdate()
		{
			_graphics.Refresh();
			//Graphics graphics = Gtk.DotNet.Graphics.FromDrawable(_graphics);
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
			if (Common.Screens.Count(x => x.HasUpdate(_gameTick)) > 0) ScreenUpdate();
		}
		
		private void OnPaint(object sender, System.Windows.Forms.PaintEventArgs args)
		{
			//args.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			//args.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			
			if (Common.Screens.Length == 0) return;
			
			_canvas = new Picture(320, 200, TopScreen.Canvas.Image.Palette.Entries);
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				// This is a workaround/fix for Linux. It causes artifacts on Windows.
				_canvas.FillRectangle(5, 0, 0, 320, 200);
			}
			foreach (IScreen screen in Common.Screens)
			{
				_canvas.AddLayer(screen.Canvas.Image, 0, 0);
			}
			
			args.Graphics.Clear(Color.Black);
			args.Graphics.DrawImage(_canvas.Image, CanvasX, CanvasY, CanvasWidth, CanvasHeight);
		}
		
		private void OnDelete(object sender, EventArgs args)
		{
			Common.Quit();
		}
		
		internal GtkWindow(string screen)
		{
			// Set Window properties
			_window = new Window("CivOne");
			_window.Resize(320 * Settings.Instance.ScaleX, 200 * Settings.Instance.ScaleY);
			
			_graphics = new GtkGraphics();
			_window.Add(_graphics);
			
			// Set Window/Canvas events
			_window.DeleteEvent += OnDelete;
			_graphics.Paint += OnPaint;
						
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
			
			_window.ShowAll();
			
			// Start tick thread
			TickThread = new Thread(new ThreadStart(SetGameTick));
			TickThread.Start();
		}
		
		public void Dispose()
		{
			if (TickThread.IsAlive && Common.EndGame)
			{
				TickThread.Abort();
			}
			
			Gtk.Application.Quit();
		}
	}
}
#endif