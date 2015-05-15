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
		
		private List<IScreen> Screens
		{
			get
			{
				return Common.Screens;
			}
		}
		
		private IScreen TopScreen
		{
			get
			{
				return Screens.LastOrDefault();
			}
		}
		
		private void SetGameTick()
		{
			while (true)
			{
				// if the previous tick is still busy, step out... this will cause the game to slow down a bit
				if (!_tickWaiter.WaitOne(25)) return;
				_tickWaiter.Reset();
				
				RefreshGame();
				_gameTick++;
				Thread.Sleep(1000 / 30);
				
				_tickWaiter.Set();
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
			if (Screens.Count > 0 && _currentCursor != Screens[0].Cursor)
			{
				OnMouseMove(this, new MouseEventArgs(MouseButtons.None, 0, Cursor.Position.X, Cursor.Position.Y, 0));
			}
			
			// Refresh the screen if there's an update
			if (Screens.Count(x => x.HasUpdate(_gameTick)) > 0) Refresh();
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
		
		private void OnLoad(object sender, EventArgs args)
		{
			// Load the demo
			Screens.Add(new Demo());
			
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
			
			if (Screens.Count == 0) return;
			
			_canvas = new Picture(320, 200, TopScreen.Canvas.Image.Palette.Entries);
			foreach (IScreen screen in Screens)
			{
				_canvas.AddLayer(screen.Canvas.Image, 0, 0);
			}
			
			args.Graphics.DrawImage(_canvas.Image, 0, 0, ClientSize.Width, ClientSize.Height);
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
		}
		
		public Window()
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
			MouseMove += OnMouseMove;
			ResizeEnd += OnResizeEnd;
			
			ResumeLayout(false);
		}
		
		protected override void Dispose(bool disposing)
		{
			TickThread.Abort();
			
			base.Dispose(disposing);
		}
	}
}