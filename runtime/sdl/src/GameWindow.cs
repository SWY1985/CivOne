using System;
using System.Drawing;
using System.Linq;
using CivOne.Events;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne
{
	internal class GameWindow : SDL.Window
	{
		private readonly Runtime _runtime;

		private void Load(object sender, EventArgs args)
		{
			_runtime.InvokeInitialize();
		}

		private void Update(object sender, EventArgs args)
		{
			UpdateEventArgs updateArgs = UpdateEventArgs.Empty;
			_runtime.InvokeUpdate(ref updateArgs);
		}

		private void Draw(object sender, EventArgs args)
		{
			_runtime.InvokeDraw();

			if (_runtime.Bitmap == null) return;
			Bytemap bitmap = _runtime.Bitmap.Bitmap;
			Colour[] palette = _runtime.Bitmap.Palette.Entries.ToArray();
			for (int y = 0; y < 200; y++)
			for (int x = 0; x < 320; x++)
			{
				int entry = bitmap[x, y];
				Colour c = entry == 0 ? Colour.Black : palette[entry];
				Rectangle rect = new Rectangle(x * 2, y * 2, 2, 2);
				Color color = Color.FromArgb(255, c.R, c.G, c.B);

				DrawRectangle(rect, color);
			}
		}

		public GameWindow(Runtime runtime) : base("CivOne")
		{
			_runtime = runtime;

			OnLoad += Load;
			OnUpdate += Update;
			OnDraw += Draw;
		}
	}
}