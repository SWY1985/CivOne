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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne
{
	internal class Window : Form
	{
		private Picture _canvas = null;
		
		private uint _gameTick = 0;
		private Thread TickThread;
		private delegate void DelegateRefreshGame();
		private delegate void DelegateScreenUpdate();
		
		private Cursor _hiddenCursor;
		private Cursor[,] _cursorPointer,_cursorGoto;
		private MouseCursor _currentCursor = MouseCursor.Pointer;
		
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
				return (ClientSize.Width - CanvasWidth) / 2;
			}
		}
		
		private int CanvasY
		{
			get
			{
				return (ClientSize.Height - CanvasHeight) / 2;
			}
		}
		
		private int CanvasWidth
		{
			get
			{
				return (int)Math.Floor((float)ClientSize.Width / 320) * 320;
			}
		}
		
		private int CanvasHeight
		{
			get
			{
				return (int)Math.Floor((float)ClientSize.Height / 200) * 200;
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
			if (InvokeRequired)
			{
				Invoke(new DelegateScreenUpdate(ScreenUpdate));
				return;
			}

			Refresh();
		}
		
		private void RefreshGame()
		{
			if (InvokeRequired)
			{
				Invoke(new DelegateRefreshGame(RefreshGame));
				return;
			}
			
			// Update cursor
			if (Common.Screens.Length > 0 && _currentCursor != TopScreen.Cursor)
			{
				_currentCursor = TopScreen.Cursor;
				OnMouseMove(this, new MouseEventArgs(MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, 0));
			}
			
			// Refresh the screen if there's an update
			if (Common.Screens.Count(x => x.HasUpdate(_gameTick)) > 0) Refresh();
		}
		
		private void LoadCursor(ref Cursor[,] cursor, int x, int y)
		{
			int sx = (int)Math.Floor((float)ClientSize.Width / 320);
			int sy = (int)Math.Floor((float)ClientSize.Height / 200);
			cursor = new Cursor[sx, sy];
			Bitmap img = Resources.Instance.GetPart("SP257", x, y, 16, 16);
			
			for (int cx = 0; cx < sx; cx++)
			for (int cy = 0; cy < sy; cy++)
			{
				Bitmap res = new Bitmap(32 * sx, 32 * sy, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				Graphics gfx = Graphics.FromImage(res);
				gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
				gfx.PixelOffsetMode = PixelOffsetMode.Half;
				gfx.DrawImage(img, (15 * sx) - cx, (15 * sy) - cy, 16 * sx, 16 * sy);
				cursor[cx, cy] = new Cursor(res.GetHicon());
			}
		}
		
		private void LoadCursors()
		{
			_hiddenCursor = new Cursor(new Bitmap(16, 16).GetHicon());
			LoadCursor(ref _cursorPointer, 112, 32);
			LoadCursor(ref _cursorGoto, 32, 32);
		}
		
		private void ToggleFullScreen()
		{
			if (WindowState == FormWindowState.Maximized)
			{
				Console.WriteLine("Full screen off");
				WindowState = FormWindowState.Normal;
				FormBorderStyle = FormBorderStyle.Sizable;
				LoadCursors();
				return;
			}
			
			Console.WriteLine("Full screen on");
			WindowState = FormWindowState.Maximized;
			FormBorderStyle = FormBorderStyle.None;
			LoadCursors();
		}
		
		private void OnLoad(object sender, EventArgs args)
		{			
			// Start tick thread
			TickThread = new Thread(new ThreadStart(SetGameTick));
			TickThread.Start();
			
			// Load cursors
			LoadCursors();
		}
		
		private void OnPaint(object sender, PaintEventArgs args)
		{
			args.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			args.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			
			if (Common.Screens.Length == 0) return;
			
			_canvas = new Picture(320, 200, TopScreen.Canvas.Image.Palette.Entries);
			foreach (IScreen screen in Common.Screens)
			{
				_canvas.AddLayer(screen.Canvas.Image, 0, 0);
			}
			
			args.Graphics.Clear(Color.Black);
			args.Graphics.DrawImage(_canvas.Image, CanvasX, CanvasY, CanvasWidth, CanvasHeight);
		}
		
		private void OnKeyDown(object sender, KeyEventArgs args)
		{
			if (args.Alt || args.Control)
			{
				if (args.KeyCode == Keys.Enter)
				{
					ToggleFullScreen();
				}
				args.SuppressKeyPress = true;
				return;
			}
			
			if (TopScreen != null && TopScreen.KeyDown(args)) ScreenUpdate();
			
			if (args.KeyCode == Keys.F10)
			{
				args.SuppressKeyPress = true;
			}
		}
		
		private void OnMouseDown(object sender, MouseEventArgs args)
		{
			if (TopScreen != null && TopScreen.MouseDown(args)) ScreenUpdate();
		}
		
		private void OnMouseMove(object sender, MouseEventArgs args)
		{
			if (_currentCursor == MouseCursor.None)
			{
				Cursor = _hiddenCursor;
				return;
			}
			
			// apply cursor correction
			int sx = (int)Math.Floor((float)ClientSize.Width / 320);
			int sy = (int)Math.Floor((float)ClientSize.Height / 200);
			if (args.X < 0 || args.Y < 0) return;
			switch (_currentCursor)
			{
				case MouseCursor.Pointer:
					Cursor = _cursorPointer[args.X % sx, args.Y % sy];
					break;
				case MouseCursor.Goto:
					Cursor = _cursorGoto[args.X % sx, args.Y % sy];
					break;
			}
		}
		
		private void OnResizeEnd(object sender, EventArgs args)
		{
			int width = (int)Math.Round((float)ClientSize.Width / 320) * 320;
			int height = (int)Math.Round((float)ClientSize.Height / 200) * 200;
			
			ClientSize = new Size(width, height);
			LoadCursors();
			Refresh();
		}
		
		public Window(string screen)
		{
			SuspendLayout();
			
			// Set Window properties
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.Sizable;
			MaximizeBox = false;
			ClientSize = new Size(320 * Settings.Instance.ScaleX, 200 * Settings.Instance.ScaleY);
			Text = "CivOne";
			
			// Set Window events
			Load += OnLoad;
			Paint += OnPaint;
			KeyDown += OnKeyDown;
			MouseDown += OnMouseDown;
			MouseMove += OnMouseMove;
			ResizeEnd += OnResizeEnd;
			
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
			
			ResumeLayout(false);
		}
		
		protected override void Dispose(bool disposing)
		{
			TickThread.Abort();
			
			base.Dispose(disposing);
		}
	}
}