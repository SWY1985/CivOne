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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
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
		
		private void ScaleMouseEventArgs(ref MouseEventArgs args)
		{
			int xx = args.X - CanvasX, yy = args.Y - CanvasY;
			args = new MouseEventArgs(args.Button, args.Clicks, (int)Math.Floor((float)xx / ScaleX), (int)Math.Floor((float)yy / ScaleY), args.Delta);
		}
		
		private void SendMouseDown(MouseEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			TopScreen.MouseDown(args);
		}
		
		private void SendMouseUp(MouseEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			TopScreen.MouseUp(args);
		}
		
		private void SendKeyDown(System.Windows.Forms.Keys keys)
		{
			System.Windows.Forms.KeyEventArgs args = new System.Windows.Forms.KeyEventArgs(keys);
			
			if (args.Alt || args.Control)
			{
				if (args.KeyCode == System.Windows.Forms.Keys.Enter)
				{
					Console.WriteLine("TODO: Toggle full screen");
					//ToggleFullScreen();
				}
				if (args.Control && args.KeyCode == System.Windows.Forms.Keys.F5)
				{
					Console.WriteLine("TODO: Save screen");
					//SaveScreen();
				}
				args.SuppressKeyPress = true;
				return;
			}
			
			if (TopScreen != null && TopScreen.KeyDown(args)) ;// ScreenUpdate();
			
			if (args.KeyCode == System.Windows.Forms.Keys.F10)
			{
				args.SuppressKeyPress = true;
			}
		}
		
		private void OnDelete(object sender, EventArgs args)
		{
			Common.Quit();
		}
		
		private void OnPaint(object sender, System.Windows.Forms.PaintEventArgs args)
		{
			args.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			args.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			
			if (Common.Screens.Length == 0) return;
			
			Color[] colours = TopScreen.Canvas.Image.Palette.Entries;
			colours[0] = Color.Black;
			
			_canvas = new Picture(320, 200, colours);
			foreach (IScreen screen in Common.Screens)
			{
				_canvas.AddLayer(screen.Canvas.Image, 0, 0);
			}
			
			args.Graphics.Clear(Color.Black);
			args.Graphics.DrawImage(_canvas.Image, CanvasX, CanvasY, CanvasWidth, CanvasHeight);
		}
		
		private void OnMouseDown(object sender, ButtonPressEventArgs args)
		{
			MouseButtons buttons = MouseButtons.None;
			switch (args.Event.Button)
			{
				case 1: buttons = MouseButtons.Left; break;
				case 3: buttons = MouseButtons.Right; break;
			}
			SendMouseDown(new MouseEventArgs(buttons, 1, (int)args.Event.X, (int)args.Event.Y, 0));
		}
		
		private void OnMouseUp(object sender, ButtonReleaseEventArgs args)
		{
			MouseButtons buttons = MouseButtons.None;
			switch (args.Event.Button)
			{
				case 1: buttons = MouseButtons.Left; break;
				case 3: buttons = MouseButtons.Right; break;
			}
			SendMouseUp(new MouseEventArgs(buttons, 1, (int)args.Event.X, (int)args.Event.Y, 0));
		}
		
		[GLib.ConnectBefore()]
		private void OnKeyPress(object sender, Gtk.KeyPressEventArgs args)
		{
			switch (args.Event.Key)
			{
				case Gdk.Key.Return:
				case Gdk.Key.KP_Enter:
					SendKeyDown(System.Windows.Forms.Keys.Enter);
					return;
				case Gdk.Key.space:
				case Gdk.Key.KP_Space:
					SendKeyDown(System.Windows.Forms.Keys.Space);
					return;
				case Gdk.Key.Up:
					SendKeyDown(System.Windows.Forms.Keys.Up);
					return;
				case Gdk.Key.Down:
					SendKeyDown(System.Windows.Forms.Keys.Down);
					return;
				case Gdk.Key.Left:
					SendKeyDown(System.Windows.Forms.Keys.Left);
					return;
				case Gdk.Key.Right:
					SendKeyDown(System.Windows.Forms.Keys.Right);
					return;
			}
		}
		
		internal GtkWindow(string screen)
		{
			// Set Window properties
			_window = new Window("CivOne");
			_window.Resize(320 * Settings.Instance.ScaleX, 200 * Settings.Instance.ScaleY);
			
			_graphics = new GtkGraphics();
			_graphics.AddEvents((int)(Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.ButtonMotionMask));
			_window.Add(_graphics);
			_window.AddEvents((int)(Gdk.EventMask.KeyPressMask));
			
			// Set Window/Canvas events
			_window.DeleteEvent += OnDelete;
			_graphics.Paint += OnPaint;
			_graphics.ButtonPressEvent += OnMouseDown;
			_graphics.ButtonReleaseEvent += OnMouseUp;
			_window.KeyPressEvent += OnKeyPress;
			
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