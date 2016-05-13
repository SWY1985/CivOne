// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using Gtk;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne
{
	internal partial class Window : IDisposable
	{
		private readonly Gtk.Window _window;
		
		private bool _forceUpdate = false;
		private bool _fullScreen = false;
		
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
		
		private static Gdk.Pixbuf GetPixbuf(Bitmap image)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				image.Save(ms, ImageFormat.Bmp);
				ms.Position = 0;
				Gdk.Pixbuf output = new Gdk.Pixbuf(ms);
				return output;
			}
		}
		
		private void ScreenUpdate()
		{
			if (Common.Screens.Length == 0) return;
			
			Color[] colours = TopScreen.Canvas.Image.Palette.Entries;
			colours[0] = Color.Black;
			
			_canvas = new Picture(320, 200, colours);
			foreach (IScreen screen in Common.Screens)
			{
				_canvas.AddLayer(screen.Canvas.Image, 0, 0);
			}
			
			Gdk.Threads.Enter();
			_window.QueueDraw();
			Gdk.Threads.Leave();
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
			if (_forceUpdate || Common.Screens.Count(x => x.HasUpdate(_gameTick)) > 0) ScreenUpdate();
			_forceUpdate = false;
		}
		
		private void ToggleFullScreen()
		{
			if (_fullScreen)
			{
				Console.WriteLine("Full screen off");
				_window.Unfullscreen();
				_fullScreen = false;
				_forceUpdate = true;
				return;
			}
			Console.WriteLine("Full screen on");
			_window.Fullscreen();
			_fullScreen = true;
			_forceUpdate = true;
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
			_forceUpdate = TopScreen.MouseDown(args);
		}
		
		private void SendMouseUp(MouseEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseUp(args);
		}
		
		private void SendMouseDrag(MouseEventArgs args)
		{
			if (TopScreen == null);
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseDrag(args);
		}
		
		private void SendKeyDown(System.Windows.Forms.Keys keys)
		{
			System.Windows.Forms.KeyEventArgs args = new System.Windows.Forms.KeyEventArgs(keys);
			
			if (args.Alt || args.Control)
			{
				if (args.Alt && args.KeyCode == System.Windows.Forms.Keys.Enter)
				{
					ToggleFullScreen();
				}
				if (args.Control && args.KeyCode == System.Windows.Forms.Keys.F5)
				{
					SaveScreen();
				}
				if (args.Alt && args.KeyCode == System.Windows.Forms.Keys.Q)
				{
					Common.Quit();
					Dispose();
				}
				args.SuppressKeyPress = true;
				return;
			}
			
			if (TopScreen != null && TopScreen.KeyDown(args)) _forceUpdate = true;// ScreenUpdate();
			
			if (args.KeyCode == System.Windows.Forms.Keys.F10)
			{
				args.SuppressKeyPress = true;
			}
		}
		
		private void OnDelete(object sender, EventArgs args)
		{
			Common.Quit();
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
		
		private void OnMouseDrag(MotionNotifyEventArgs args)
		{
			MouseButtons buttons = MouseButtons.None;
			if ((args.Event.State & Gdk.ModifierType.Button1Mask) > 0) buttons |= MouseButtons.Left;
			if ((args.Event.State & Gdk.ModifierType.Button3Mask) > 0) buttons |= MouseButtons.Right;
			
			SendMouseDrag(new MouseEventArgs(buttons, 1, (int)args.Event.X, (int)args.Event.Y, 0));
		}
		
		private void OnMouseMove(object sender, MotionNotifyEventArgs args)
		{
			if ((args.Event.State & (Gdk.ModifierType.Button1Mask | Gdk.ModifierType.Button3Mask)) > 0) OnMouseDrag(args);
			
			// TODO: Implement cursor
		}
		
		[GLib.ConnectBefore()]
		private void OnKeyPress(object sender, Gtk.KeyPressEventArgs args)
		{
			System.Windows.Forms.Keys modifier = System.Windows.Forms.Keys.None;
			if ((args.Event.State & Gdk.ModifierType.ControlMask) > 0) modifier |= System.Windows.Forms.Keys.Control;
			if ((args.Event.State & Gdk.ModifierType.ShiftMask) > 0) modifier |= System.Windows.Forms.Keys.Shift;
			if ((args.Event.State & Gdk.ModifierType.Mod1Mask) > 0) modifier |= System.Windows.Forms.Keys.Alt;
			
			switch (args.Event.Key)
			{
				case Gdk.Key.Return:
				case Gdk.Key.KP_Enter:
					SendKeyDown(System.Windows.Forms.Keys.Enter | modifier);
					return;
				case Gdk.Key.space:
				case Gdk.Key.KP_Space:
					SendKeyDown(System.Windows.Forms.Keys.Space | modifier);
					return;
				case Gdk.Key.Up:
					SendKeyDown(System.Windows.Forms.Keys.Up | modifier);
					return;
				case Gdk.Key.Down:
					SendKeyDown(System.Windows.Forms.Keys.Down | modifier);
					return;
				case Gdk.Key.Left:
					SendKeyDown(System.Windows.Forms.Keys.Left | modifier);
					return;
				case Gdk.Key.Right:
					SendKeyDown(System.Windows.Forms.Keys.Right | modifier);
					return;
				case Gdk.Key.F1:
					SendKeyDown(System.Windows.Forms.Keys.F1 | modifier);
					return;
				case Gdk.Key.F2:
					SendKeyDown(System.Windows.Forms.Keys.F2 | modifier);
					return;
				case Gdk.Key.F3:
					SendKeyDown(System.Windows.Forms.Keys.F3 | modifier);
					return;
				case Gdk.Key.F4:
					SendKeyDown(System.Windows.Forms.Keys.F4 | modifier);
					return;
				case Gdk.Key.F5:
					SendKeyDown(System.Windows.Forms.Keys.F5 | modifier);
					return;
				case Gdk.Key.F6:
					SendKeyDown(System.Windows.Forms.Keys.F6 | modifier);
					return;
				case Gdk.Key.F7:
					SendKeyDown(System.Windows.Forms.Keys.F7 | modifier);
					return;
				case Gdk.Key.F8:
					SendKeyDown(System.Windows.Forms.Keys.F8 | modifier);
					return;
				case Gdk.Key.F9:
					SendKeyDown(System.Windows.Forms.Keys.F9 | modifier);
					return;
				case Gdk.Key.F10:
					SendKeyDown(System.Windows.Forms.Keys.F10 | modifier);
					return;
				default:
					SendKeyDown((System.Windows.Forms.Keys)char.ToUpper((char)args.Event.Key) | modifier);
					return;
			}
		}
		
		protected void OnExpose(object sender, ExposeEventArgs args)
		{
			//args.Graphics.DrawImage(_canvas.Image, CanvasX, CanvasY, CanvasWidth, CanvasHeight);
			
			if (_canvas == null) return;
			
			Gdk.Pixbuf canvas = GetPixbuf(_canvas.Image).ScaleSimple(CanvasWidth, CanvasHeight, Gdk.InterpType.Nearest);
			canvas.RenderToDrawable(args.Event.Window, _window.Style.BaseGC(StateType.Normal), 0, 0, CanvasX, CanvasY, -1, -1, Gdk.RgbDither.None, 0, 0);
		}
		
		internal void Run()
		{
			Gtk.Application.Run();
		}
		
		public string BrowseDataFolder()
		{
			FileChooserDialog folderBrowser = new FileChooserDialog("Select the folder containing the original Civilization data files.", null, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Select", ResponseType.Ok);
			
			if (folderBrowser.Run() == (int)ResponseType.Ok)
				return folderBrowser.Filename;
			return Settings.Instance.DataDirectory;
		}
		
		public static void CreateWindow(string screen)
		{
			using (Window window = new Window(screen))
			{
				window.Run();
			}
		}
		
		private Window(string screen)
		{
			Gdk.Threads.Init();
			Gtk.Application.Init();
			Gdk.Threads.Enter();
			
			// Set Window properties
			_window = new Gtk.Window("CivOne");
			_window.Resize(320 * Settings.Instance.ScaleX, 200 * Settings.Instance.ScaleY);
			_window.AddEvents((int)(Gdk.EventMask.KeyPressMask | Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.ButtonMotionMask | Gdk.EventMask.PointerMotionMask));
			_window.ModifyBg(StateType.Normal, Gdk.Color.Zero);
			
			// Set Window events
			_window.DeleteEvent += OnDelete;
			_window.ExposeEvent += OnExpose;
			_window.ButtonPressEvent += OnMouseDown;
			_window.ButtonReleaseEvent += OnMouseUp;
			_window.MotionNotifyEvent += OnMouseMove;
			_window.KeyPressEvent += OnKeyPress;
			
			// Load the first screen
			Init(screen);
			
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
			
			Gdk.Threads.Leave();
			Gtk.Application.Quit();
		}
	}
}