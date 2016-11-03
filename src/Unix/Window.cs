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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne
{
	internal partial class Window : IDisposable
	{
		private readonly Gtk.Window _window;

		private Picture _cursorPointer, _cursorGoto;
		private MouseCursor _currentCursor = MouseCursor.None;
		
		private bool _forceUpdate = false;
		private bool _fullScreen = false;

		private int _mouseX, _mouseY;
		
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
			if (Common.Screens.Length == 0 || Canvas == null) return;
			
			Gdk.Threads.Enter();
			_window.QueueDraw();
			Gdk.Threads.Leave();
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

			// Update cursor
			if (Common.Screens.Length > 0 && _currentCursor != TopScreen.Cursor)
			{
				_currentCursor = TopScreen.Cursor;
				_forceUpdate = true;
			}
			
			// Refresh the screen if there's an update
			if (HasUpdate || _forceUpdate) ScreenUpdate();
			_forceUpdate = false;
		}

		private void LoadCursors()
		{
			using (Gdk.Pixmap inv = new Gdk.Pixmap(null, 1, 1, 1))
			{
				_window.GdkWindow.Cursor = new Gdk.Cursor(inv, inv, Gdk.Color.Zero, Gdk.Color.Zero, 0, 0);
			}

			_cursorPointer = Resources.Instance.GetPart("SP257", 112, 32, 16, 16);
			_cursorGoto = Resources.Instance.GetPart("SP257", 32, 32, 16, 16);
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
		
		private void ScaleMouseEventArgs(ref ScreenEventArgs args)
		{
			int xx = args.X - CanvasX, yy = args.Y - CanvasY;
			args = new ScreenEventArgs((int)Math.Floor((float)xx / ScaleX), (int)Math.Floor((float)yy / ScaleY), args.Buttons);
		}
		
		private void SendMouseDown(ScreenEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseDown(args);
		}
		
		private void SendMouseUp(ScreenEventArgs args)
		{
			if (TopScreen == null) return;
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseUp(args);
		}
		
		private void SendMouseDrag(ScreenEventArgs args)
		{
			if (TopScreen == null);
			ScaleMouseEventArgs(ref args);
			_forceUpdate = TopScreen.MouseDrag(args);
		}
		
		private void SendKeyDown(Key key, KeyModifier modifier)
		{
			SendKeyDown(new KeyboardEventArgs(key, modifier));
		}
		
		private void SendKeyDown(char keyChar, KeyModifier modifier)
		{
			SendKeyDown(new KeyboardEventArgs(keyChar, modifier));
		}
		
		private void SendKeyDown(KeyboardEventArgs args)
		{
			if (args.Alt || args.Control)
			{
				if (args.Alt && args.Key == Key.Enter)
				{
					ToggleFullScreen();
				}
				if (args.Control && args.Key == Key.F5)
				{
					SaveScreen();
				}
				if (args.Alt && args.KeyChar == 'Q')
				{
					Common.Quit();
					Dispose();
				}
				return;
			}
			
			if (TopScreen != null && TopScreen.KeyDown(args))
				_forceUpdate = true;
		}
		
		private void OnDelete(object sender, EventArgs args)
		{
			Common.Quit();
			Dispose();
		}
		
		private void OnMouseDown(object sender, Gtk.ButtonPressEventArgs args)
		{
			MouseButton buttons = MouseButton.None;
			switch (args.Event.Button)
			{
				case 1: buttons = MouseButton.Left; break;
				case 3: buttons = MouseButton.Right; break;
			}
			SendMouseDown(new ScreenEventArgs((int)args.Event.X, (int)args.Event.Y, buttons));
		}
		
		private void OnMouseUp(object sender, Gtk.ButtonReleaseEventArgs args)
		{
			MouseButton buttons = MouseButton.None;
			switch (args.Event.Button)
			{
				case 1: buttons = MouseButton.Left; break;
				case 3: buttons = MouseButton.Right; break;
			}
			SendMouseUp(new ScreenEventArgs((int)args.Event.X, (int)args.Event.Y, buttons));
		}
		
		private void OnMouseDrag(Gtk.MotionNotifyEventArgs args)
		{
			MouseButton buttons = MouseButton.None;
			if ((args.Event.State & Gdk.ModifierType.Button1Mask) > 0) buttons |= MouseButton.Left;
			if ((args.Event.State & Gdk.ModifierType.Button3Mask) > 0) buttons |= MouseButton.Right;
			
			SendMouseDrag(new ScreenEventArgs((int)args.Event.X, (int)args.Event.Y, buttons));
		}
		
		private void OnMouseMove(object sender, Gtk.MotionNotifyEventArgs args)
		{
			if ((args.Event.State & (Gdk.ModifierType.Button1Mask | Gdk.ModifierType.Button3Mask)) > 0) OnMouseDrag(args);

			// Update cursor location and force an update
			_mouseX = (int)args.Event.X / ScaleX;
			_mouseY = (int)args.Event.Y / ScaleY;
			if (_currentCursor == null)
				return;
			_forceUpdate = true;
		}
		
		[GLib.ConnectBefore()]
		private void OnKeyPress(object sender, Gtk.KeyPressEventArgs args)
		{
			KeyModifier modifier = KeyModifier.None;
			if ((args.Event.State & Gdk.ModifierType.ControlMask) > 0) modifier |= KeyModifier.Control;
			if ((args.Event.State & Gdk.ModifierType.ShiftMask) > 0) modifier |= KeyModifier.Shift;
			if ((args.Event.State & Gdk.ModifierType.Mod1Mask) > 0) modifier |= KeyModifier.Alt;
			
			switch (args.Event.Key)
			{
				case Gdk.Key.F1: SendKeyDown(Key.F1, modifier); return;
				case Gdk.Key.F2: SendKeyDown(Key.F2, modifier); return;
				case Gdk.Key.F3: SendKeyDown(Key.F3, modifier); return;
				case Gdk.Key.F4: SendKeyDown(Key.F4, modifier); return;
				case Gdk.Key.F5: SendKeyDown(Key.F5, modifier); return;
				case Gdk.Key.F6: SendKeyDown(Key.F6, modifier); return;
				case Gdk.Key.F7: SendKeyDown(Key.F7, modifier); return;
				case Gdk.Key.F8: SendKeyDown(Key.F8, modifier); return;
				case Gdk.Key.F9: SendKeyDown(Key.F9, modifier); return;
				case Gdk.Key.F10: SendKeyDown(Key.F10, modifier); return;
				case Gdk.Key.F11: SendKeyDown(Key.F11, modifier); return;
				case Gdk.Key.F12: SendKeyDown(Key.F12, modifier); return;

				case Gdk.Key.KP_0: SendKeyDown(Key.NumPad0, modifier); return;
				case Gdk.Key.KP_1: SendKeyDown(Key.NumPad1, modifier); return;
				case Gdk.Key.KP_2: SendKeyDown(Key.NumPad2, modifier); return;
				case Gdk.Key.KP_3: SendKeyDown(Key.NumPad3, modifier); return;
				case Gdk.Key.KP_4: SendKeyDown(Key.NumPad4, modifier); return;
				case Gdk.Key.KP_5: SendKeyDown(Key.NumPad5, modifier); return;
				case Gdk.Key.KP_6: SendKeyDown(Key.NumPad6, modifier); return;
				case Gdk.Key.KP_7: SendKeyDown(Key.NumPad7, modifier); return;
				case Gdk.Key.KP_8: SendKeyDown(Key.NumPad8, modifier); return;
				case Gdk.Key.KP_9: SendKeyDown(Key.NumPad9, modifier); return;
				case Gdk.Key.Up: SendKeyDown(Key.Up, modifier); return;
				case Gdk.Key.Down: SendKeyDown(Key.Down, modifier); return;
				case Gdk.Key.Left: SendKeyDown(Key.Left, modifier); return;
				case Gdk.Key.Right: SendKeyDown(Key.Right, modifier); return;
				case Gdk.Key.Return:
				case Gdk.Key.KP_Enter: SendKeyDown(Key.Enter, modifier); return;
				case Gdk.Key.space:
				case Gdk.Key.KP_Space: SendKeyDown(Key.Space, modifier); return;
				case Gdk.Key.BackSpace: SendKeyDown(Key.Backspace, modifier); return;
				case Gdk.Key.period: SendKeyDown('.', modifier); return;
				case Gdk.Key.comma: SendKeyDown(',', modifier); return;
				case Gdk.Key.KP_Add:
				case Gdk.Key.plus: SendKeyDown(Key.Plus, modifier); return;
				case Gdk.Key.KP_Subtract:
				case Gdk.Key.minus: SendKeyDown(Key.Minus, modifier); return;
				default: SendKeyDown(char.ToUpper((char)args.Event.Key), modifier); return;
			}
		}

		private Bitmap CanvasCursor
		{
			get
			{
				if (_currentCursor == MouseCursor.None)
					return _canvas.Image;
				
				Picture canvas = new Picture(_canvas);
				switch (_currentCursor)
				{
					case MouseCursor.Pointer:
						canvas.AddLayer(_cursorPointer, _mouseX, _mouseY);
						break;
					case MouseCursor.Goto:
						canvas.AddLayer(_cursorGoto, _mouseX, _mouseY);
						break;
				}
				return canvas.Image;
			}
		}
		
		protected void OnExpose(object sender, Gtk.ExposeEventArgs args)
		{
			Gdk.Pixbuf canvas = GetPixbuf(CanvasCursor).ScaleSimple(CanvasWidth, CanvasHeight, Gdk.InterpType.Nearest);
			canvas.RenderToDrawable(args.Event.Window, _window.Style.BaseGC(Gtk.StateType.Normal), 0, 0, CanvasX, CanvasY, -1, -1, Gdk.RgbDither.None, 0, 0);
		}
		
		internal void Run()
		{
			Gtk.Application.Run();
		}
		
		private static string BrowseDataFolder()
		{
			Init();
			
			Gtk.FileChooserDialog folderBrowser = new Gtk.FileChooserDialog("Select the folder containing the original Civilization data files.", null, Gtk.FileChooserAction.SelectFolder, "Cancel", Gtk.ResponseType.Cancel, "Select", Gtk.ResponseType.Ok);
			
			bool responseOk = (folderBrowser.Run() == (int)Gtk.ResponseType.Ok);
			string directory = (responseOk ? folderBrowser.Filename : Settings.Instance.DataDirectory);
			folderBrowser.Destroy();
			
			return directory;
		}
		
		public static void CreateWindow(string screen)
		{
			using (Window window = new Window(screen))
			{
				window.Run();
			}
		}
		
		private static bool _init = false;
		private static void Init()
		{
			if (_init) return;
			
			Gdk.Threads.Init();
			Gtk.Application.Init();
			Gdk.Threads.Enter();
			_init = true;
		}
		
		private Window(string screen)
		{
			Init();
			
			// Set Window properties
			_window = new Gtk.Window("CivOne");
			_window.Resize(320 * Settings.Instance.ScaleX, 200 * Settings.Instance.ScaleY);
			_window.AddEvents((int)(Gdk.EventMask.KeyPressMask | Gdk.EventMask.ButtonPressMask | Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.ButtonMotionMask | Gdk.EventMask.PointerMotionMask));
			_window.ModifyBg(Gtk.StateType.Normal, Gdk.Color.Zero);
			
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
			
			// Load cursors
			LoadCursors();
		}
		
		public void Dispose()
		{
			if (TickThread.IsAlive && Common.EndGame)
			{
				TickThread.Abort();
			}
			
			Gdk.Threads.Leave();
			Gtk.Application.Quit();
			Environment.Exit(0);
		}
	}
}