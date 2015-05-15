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
			int scale = Settings.Instance.Scale;
			cursor = new Cursor[scale, scale];
			Bitmap img = Resources.Instance.GetPart("SP257", x, y, 16, 16);

			for (int cx = 0; cx < scale; cx++)
				for (int cy = 0; cy < scale; cy++)
				{
					Bitmap res = new Bitmap(32 * scale, 32 * scale, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					Graphics gfx = Graphics.FromImage(res);
					gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
					gfx.PixelOffsetMode = PixelOffsetMode.Half;
					gfx.DrawImage(img, (15 * scale) - cx, (15 * scale) - cy, 16 * scale, 16 * scale);
					cursor[cx, cy] = new Cursor(res.GetHicon());
				}
		}
		
		private void OnLoad(object sender, EventArgs args)
		{
			// Load the demo
			Screens.Add(new Demo());
			
			// Start tick thread
			TickThread = new Thread(new ThreadStart(SetGameTick));
			TickThread.Start();
			
			// Load cursors
			_hiddenCursor = new Cursor(new Bitmap(16, 16).GetHicon());
			LoadCursor(ref _cursorPointer, 112, 32);
			LoadCursor(ref _cursorGoto, 32, 32);
		}
		
		private void OnPaint(object sender, PaintEventArgs args)
		{
			args.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			args.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			
			if (Screens.Count == 0) return;
			
			_canvas = new Picture(320, 200, Screens[Screens.Count - 1].Canvas.Image.Palette.Entries);
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
			int s = Settings.Instance.Scale;
			if (args.X < 0 || args.Y < 0) return;
			switch (_currentCursor)
			{
				case MouseCursor.Pointer:
					Cursor = _cursorPointer[args.X % s, args.Y % s];
					break;
				case MouseCursor.Goto:
					Cursor = _cursorGoto[args.X % s, args.Y % s];
					break;
			}
		}
		
		public Window()
		{
			SuspendLayout();
			
			// Set Window properties
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			ClientSize = new Size(320 * Settings.Instance.Scale, 200 * Settings.Instance.Scale);
			Text = "CivOne";
			
			// Set Window events
			Load += OnLoad;
			Paint += OnPaint;
			MouseMove += OnMouseMove;
			
			ResumeLayout(false);
		}
		
		protected override void Dispose(bool disposing)
		{
			TickThread.Abort();
			
			base.Dispose(disposing);
		}
	}
}