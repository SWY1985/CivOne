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
using System.Threading;
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.Events;

using FTimer = System.Windows.Forms.Timer;

namespace CivOne
{
	internal class Window : Form
	{
		private static Settings Settings => Settings.Instance;

		private AutoResetEvent _updateBusy = new AutoResetEvent(true);
		private AutoResetEvent _drawBusy = new AutoResetEvent(true);

		private readonly Runtime _runtime;
		private readonly FTimer _timer = new FTimer();

		private Canvas Canvas => new Canvas(_runtime.Bitmap);

		private int _mouseX, _mouseY;
		private MouseButton _mouseButtons;

		private void OnLoad(object sender, EventArgs args)
		{
			_runtime.InvokeInitialize();
		}

		private void OnUpdate(object sender, EventArgs args)
		{
			if (_runtime.SignalQuit)
			{
				_timer.Stop();
				_timer.Dispose();
				Close();
				return;
			}

			if (!_updateBusy.WaitOne(0)) return;
			
			UpdateEventArgs updateArgs = UpdateEventArgs.Empty;
			_runtime.InvokeUpdate(ref updateArgs);
			if (updateArgs.HasUpdate) Invalidate();
			_updateBusy.Set();
		}

		private void OnPaint(object sender, PaintEventArgs args)
		{
			if (!_drawBusy.WaitOne(0)) return;

			_runtime.InvokeDraw();
			args.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			args.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			args.Graphics.DrawImage(Canvas.Image, 0, 0, InitialWidth, InitialHeight);
			_drawBusy.Set();
		}

		private void OnKeyUp(object sender, KeyEventArgs args)
		{
			args.SuppressKeyPress = (args.Alt || args.Control || args.KeyCode == Keys.F10);

			_runtime.InvokeKeyboardUp(args.ConvertKeyboardEvents());
		}

		private void OnKeyDown(object sender, KeyEventArgs args)
		{
			args.SuppressKeyPress = (args.Alt || args.Control || args.KeyCode == Keys.F10);

			_runtime.InvokeKeyboardDown(args.ConvertKeyboardEvents());
		}

		private void OnMouseUp(object sender, MouseEventArgs args)
		{
			MouseButton buttons = MouseButton.None;
			if ((args.Button & MouseButtons.Left) > 0 && (_mouseButtons & MouseButton.Left) > 0) buttons |= MouseButton.Left;
			if ((args.Button & MouseButtons.Right) > 0 && (_mouseButtons & MouseButton.Right) > 0) buttons |= MouseButton.Right;

			_runtime.InvokeMouseUp(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		private void OnMouseDown(object sender, MouseEventArgs args)
		{
			MouseButton buttons = MouseButton.None;
			if ((args.Button & MouseButtons.Left) > 0) buttons |= MouseButton.Left;
			if ((args.Button & MouseButtons.Right) > 0) buttons |= MouseButton.Right;
			_mouseButtons = buttons;

			_runtime.InvokeMouseDown(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		private void OnMouseMove(object sender, MouseEventArgs args)
		{
			int x1 = 0, y1 = 0;

			bool mouseMove =
				(_mouseX != (_mouseX = (int)((args.X - x1) / Settings.Scale))) |
				(_mouseY != (_mouseY = (int)((args.Y - y1) / Settings.Scale)));
			
			if (!mouseMove) return;

			MouseButton buttons = MouseButton.None;
			if ((args.Button & MouseButtons.Left) > 0) buttons |= MouseButton.Left;
			if ((args.Button & MouseButtons.Right) > 0) buttons |= MouseButton.Right;

			_runtime.InvokeMouseMove(new ScreenEventArgs(_mouseX, _mouseY, buttons));
		}

		private static int InitialCanvasWidth => (Settings.AspectRatio == AspectRatio.Expand) ? Settings.ExpandWidth : 320;
		private static int InitialCanvasHeight => (Settings.AspectRatio == AspectRatio.Expand) ? Settings.ExpandHeight : 200;

		private static int InitialWidth => InitialCanvasWidth * Settings.Scale;
		private static int InitialHeight => InitialCanvasHeight * Settings.Scale;

		internal Window(Runtime runtime)
		{
			_runtime = runtime;
			
			SuspendLayout();
			
			// Set Window properties
			DoubleBuffered = true;
			MaximizeBox = false;
			ClientSize = new Size(InitialWidth, InitialHeight);
			MinimumSize = new Size(320, 200);
			FormBorderStyle = Settings.FullScreen ? FormBorderStyle.None : FormBorderStyle.FixedSingle;
			WindowState = Settings.FullScreen ? FormWindowState.Maximized : FormWindowState.Normal;
			Text = "CivOne";
			
			// Set Window events
			Load += OnLoad;
			Paint += OnPaint;
			KeyUp += OnKeyUp;
			KeyDown += OnKeyDown;
			MouseUp += OnMouseUp;
			MouseDown += OnMouseDown;
			MouseMove += OnMouseMove;
			
			// Initialize timer
			_timer.Tick += OnUpdate;
			_timer.Interval = (1000 / 60);
			_timer.Enabled = true;
			
			ResumeLayout(false);
		}
	}
}