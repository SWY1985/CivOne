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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CivOne.Graphics;
using CivOne.IO;

namespace CivOne
{
	internal static partial class SDL
	{
		internal abstract partial class Window : IDisposable
		{
			private readonly IntPtr _handle, _renderer;

			private bool _running = true, _redraw = false;

			private void FillRectangle(Rectangle rectangle, Color color)
			{
				_redraw = true;

				SDL_Rect rect = new SDL_Rect() { X = rectangle.X, Y = rectangle.Y, W = rectangle.Width, H = rectangle.Height };

				SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
				SDL_RenderFillRect(_renderer, ref rect);
			}

			protected void Clear(Color color)
			{
				_redraw = true;

				SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
				SDL_RenderClear(_renderer);
			}

			protected void DrawBitmap(IBitmap bitmap, int x, int y, int pixelWidth, int pixelHeight)
			{
				if (bitmap == null) return;
				Bytemap bytemap = bitmap.Bitmap;
				Colour[] palette = bitmap.Palette.Entries.ToArray();

				for (int yy = 0; yy < bytemap.Height; yy++)
				for (int xx = 0; xx < bytemap.Width; xx++)
				{
					byte entry = bytemap[xx, yy];
					if (entry == 0) continue;
					Colour c = palette[entry];
					Rectangle rect = new Rectangle(x + (xx * pixelWidth), y + (yy * pixelHeight), pixelWidth, pixelHeight);
					Color color = Color.FromArgb(255, c.R, c.G, c.B);

					FillRectangle(rect, color);
				}
			}

			protected void StopRunning()
			{
				_running = false;
			}

			private T CastToStruct<T>(object source) where T : struct
			{
				IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(source.GetType()));
				Marshal.StructureToPtr(source, ptr, false);
				T output = Marshal.PtrToStructure<T>(ptr);
				Marshal.FreeHGlobal(ptr);
				return output;
			}
			
			private void HandleEvent(SDL_Event sdlEvent)
			{
				switch (sdlEvent.SDL_EventType)
				{
					case SDL_EventType.SDL_WINDOWEVENT:
						HandleEventWindow(CastToStruct<SDL_WindowEvent>(sdlEvent));
						break;
					case SDL_EventType.SDL_KEYDOWN:
					case SDL_EventType.SDL_KEYUP:
						HandleEventKeyboard(CastToStruct<SDL_KeyboardEvent>(sdlEvent));
						break;
				}
			}

			public void Run()
			{
				OnLoad?.Invoke(this, EventArgs.Empty);
				
				while (_running)
				{
					if (SDL_PollEvent(out SDL_Event sdlEvent) == 1)
					{
						HandleEvent(sdlEvent);
					}

					OnUpdate?.Invoke(this, EventArgs.Empty);
					OnDraw?.Invoke(this, EventArgs.Empty);

					HandleMouse();

					if (!_redraw)
					{
						Wait(1);
						continue;
					}

					SDL_RenderPresent(_renderer);
					_redraw = false;
				}
			}

			public void Wait(uint time)
			{
				SDL_Delay(time);
			}

			private static uint DefinePixelformat(SDL_PixelType type, SDL_PixelOrder order, SDL_PixelLayout layout, byte bits, byte bytes)
			{
				return (uint) (
					(1 << 28) |
					(((byte) type) << 24) |
					(((byte) order) << 20) |
					(((byte) layout) << 16) |
					(bits << 8) |
					(bytes)
				);
			}
			
			private static readonly uint SDL_PIXELFORMAT_RGBA8888 = DefinePixelformat(SDL_PixelType.SDL_PIXELTYPE_PACKED32, SDL_PixelOrder.SDL_PACKEDORDER_RGBA, SDL_PixelLayout.SDL_PACKEDLAYOUT_8888, 32, 4);

			protected int Width
			{
				get
				{
					SDL_GetWindowSize(_handle, out int width, out _);
					return width;
				}
			}

			protected int Height
			{
				get
				{
					SDL_GetWindowSize(_handle, out _, out int width);
					return width;
				}
			}

			public Window(string title, int width, int height, bool fullscreen)
			{
				SDL_Init(SDL_INIT.VIDEO);

				SDL_WINDOW flags = SDL_WINDOW.RESIZABLE;
				if (_fullscreen = fullscreen) flags |= SDL_WINDOW.FULLSCREEN_DESKTOP;
				_handle = SDL_CreateWindow(title, 100, 100, width, height, flags);
				_renderer = SDL_CreateRenderer(_handle, -1, SDL_RENDERER_FLAGS.SDL_RENDERER_ACCELERATED);

				if (_handle == null)
				{
					Console.WriteLine("Something is wrong");
					return;
				}
			}

			public void Dispose()
			{
				SDL_DestroyRenderer(_renderer);
				SDL_DestroyWindow(_handle);
				SDL_Quit();
			}
		}
	}
}