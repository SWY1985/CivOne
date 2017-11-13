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
using System.IO;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne
{
	internal class Runtime : IRuntime, IDisposable
	{
		internal static Size CanvasSize => new Size(320, 200);

		internal bool SignalQuit { get; private set; }

		internal void InvokeInitialize() => Initialize?.Invoke(this, EventArgs.Empty);
		internal void InvokeDraw() => Draw?.Invoke(this, EventArgs.Empty);
		internal void InvokeUpdate(ref UpdateEventArgs args) => Update?.Invoke(this, args);
		internal void InvokeKeyboardUp(KeyboardEventArgs args) => KeyboardUp?.Invoke(this, args);
		internal void InvokeKeyboardDown(KeyboardEventArgs args) => KeyboardDown?.Invoke(this, args);
		internal void InvokeMouseUp(ScreenEventArgs args) => MouseUp?.Invoke(this, args);
		internal void InvokeMouseDown(ScreenEventArgs args) => MouseDown?.Invoke(this, args);
		internal void InvokeMouseMove(ScreenEventArgs args) => MouseMove?.Invoke(this, args);

		public event EventHandler Initialize, Draw;
		public event UpdateEventHandler Update;
		public event KeyboardEventHandler KeyboardUp, KeyboardDown;
		public event ScreenEventHandler MouseUp, MouseDown, MouseMove;
		
		public RuntimeSettings Settings => new RuntimeSettings() { Demo = true, Setup = true };
		public IBitmap Bitmap { get; set; }
		public IBitmap Cursor { internal get; set; }
		public void Log(string value, params object[] formatArgs) => Console.WriteLine(value, formatArgs);

		Platform IRuntime.CurrentPlatform => Platform.Windows;
		string IRuntime.StorageFolder => Directory.GetCurrentDirectory();
		int IRuntime.CanvasWidth => CanvasSize.Width;
		int IRuntime.CanvasHeight => CanvasSize.Height;

		string IRuntime.BrowseFolder(string caption) => null;
		void IRuntime.PlaySound(string filename) => Console.WriteLine("PLAY SOUND NOT IMPLEMENTED");
		void IRuntime.StopSound() => Console.WriteLine("STOP SOUND NOT IMPLEMENTED");
		void IRuntime.Quit() => SignalQuit = true;

		public Runtime()
		{
			RuntimeHandler.Register(this);
		}

		public void Dispose()
		{

		}
	}
}