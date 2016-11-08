// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CivOne.Enums;
using CivOne.Events;

namespace CivOne
{
	internal partial class Window : System.Windows.Window
	{
		private double ScaleWidth
		{
			get
			{
				return ((double)(Width - SystemParameters.ResizeFrameVerticalBorderWidth) / 320);
			}
		}

		private double ScaleHeight
		{
			get
			{
				return ((double)(Height - SystemParameters.ResizeFrameHorizontalBorderHeight - SystemParameters.CaptionHeight) / 200);
			}
		}

		private int CanvasX
		{
			get
			{
				double scaleX = ScaleWidth;
				double scaleY = ScaleHeight;
				if (scaleX > scaleY)
					return (int)(((Width - (320 * scaleX)) / 2));
				return 0;
			}
		}

		private int CanvasY
		{
			get
			{
				double scaleX = ScaleWidth;
				double scaleY = ScaleHeight;
				if (scaleY > scaleX)
					return (int)(((Height - (200 * scaleY)) / 2));
				return 0;
			}
		}

		private double ScaleX
		{
			get
			{
				double scaleX = ScaleWidth;
				double scaleY = ScaleHeight;
				if (scaleX > scaleY)
					return scaleY;
				return scaleX;
			}
		}

		private double ScaleY
		{
			get
			{
				double scaleX = ScaleWidth;
				double scaleY = ScaleHeight;
				if (scaleX > scaleY)
					return scaleY; 
				return scaleX;
			}
		}

		private delegate void SetBackgroundDelegate();
		private void SetBackground()
		{
			BitmapImage bitmap = new BitmapImage();
			using (MemoryStream ms = new MemoryStream())
			{
				Canvas.Image.Save(ms, ImageFormat.Bmp);
				ms.Position = 0;
				bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.StreamSource = ms;
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				bitmap.EndInit();
			}

			Background = new ImageBrush(bitmap)
			{
				Stretch = Stretch.Uniform
			};
		}

		private void RefreshWindow()
		{
			// Refresh the screen if there's an update
			if (!HasUpdate) return;

			Dispatcher.Invoke(new SetBackgroundDelegate(SetBackground));
		}

		private Events.KeyboardEventArgs ConvertKeyboardEvents(KeyEventArgs args)
		{
			KeyModifier modifier = KeyModifier.None;
			switch (args.Key)
			{
				case System.Windows.Input.Key.Enter: return new Events.KeyboardEventArgs(Enums.Key.Enter, modifier);
				case System.Windows.Input.Key.Space: return new Events.KeyboardEventArgs(Enums.Key.Space, modifier);
			}
			return new Events.KeyboardEventArgs(char.ToUpper((char)args.Key), modifier);
		}

		private ScreenEventArgs ScaleMouseEventArgs(MouseButtonEventArgs args)
		{
			int xx = (int)args.GetPosition(this).X;// - CanvasX;
			int yy = (int)args.GetPosition(this).Y;// - CanvasY;
			Enums.MouseButton buttons = Enums.MouseButton.None;
			switch (args.ChangedButton)
			{
				case System.Windows.Input.MouseButton.Left:
					buttons = Enums.MouseButton.Left;
					break;
				case System.Windows.Input.MouseButton.Right:
					buttons = Enums.MouseButton.Right;
					break;
			}
			return new ScreenEventArgs((int)Math.Floor((float)xx / ScaleX) + CanvasX, (int)Math.Floor((float)yy / ScaleY) + CanvasY, buttons);
		}

		protected override void OnKeyDown(KeyEventArgs args)
		{
			if (TopScreen != null && TopScreen.KeyDown(ConvertKeyboardEvents(args))) RefreshWindow();
		}

		protected override void OnMouseDown(MouseButtonEventArgs args)
		{
			ScreenEventArgs screenArgs = ScaleMouseEventArgs(args);
			if (TopScreen != null && TopScreen.MouseDown(screenArgs)) RefreshWindow();
		}

		protected override void OnMouseUp(MouseButtonEventArgs args)
		{
			ScreenEventArgs screenArgs = ScaleMouseEventArgs(args);
			if (TopScreen != null && TopScreen.MouseUp(screenArgs)) RefreshWindow();
		}

		protected override void OnInitialized(EventArgs args)
		{
			base.OnInitialized(args);

			// Start tick thread
			TickThread = new Thread(new ThreadStart(SetGameTick));
			TickThread.Start();
		}

		protected override void OnClosed(EventArgs args)
		{
			base.OnClosed(args);

			if (TickThread.IsAlive)
			{
				TickThread.Abort();
				Environment.Exit(0);
			}
		}

		private static string BrowseDataFolder()
		{
			return string.Empty;
		}

		public static void CreateWindow(string screen)
		{
			Window window = new Window(screen);
			Application application = new Application()
			{
				MainWindow = window,
				ShutdownMode = ShutdownMode.OnMainWindowClose
			};
			application.Run(window);
		}

		private Window(string screen)
		{
			// Set Window properties
			Title = "CivOne";
			Height = (200 * 2) + SystemParameters.ResizeFrameHorizontalBorderHeight + SystemParameters.CaptionHeight;
			Width = (320 * 2) + SystemParameters.ResizeFrameVerticalBorderWidth;
			
			// Load the first screen
			Init(screen);
		}
	}
}