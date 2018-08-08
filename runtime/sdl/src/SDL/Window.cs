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

			private void Log(string message) => OnLog?.Invoke(message);

			protected event Action<string> OnLog;

			protected Texture CreateTexture(IBitmap bitmap) => new Texture(_renderer, bitmap?.Palette, bitmap?.Bitmap);
			protected Texture CreateTexture(Palette palette, Bytemap bytemap) => new Texture(_renderer, palette, bytemap);

			protected void Clear(Color color)
			{
				_redraw = true;

				SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
				SDL_RenderClear(_renderer);
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

			private bool _pixelScale = false;
			protected bool PixelScale
			{
				get => _pixelScale;
				set
				{
					if (!SDL_SetHint("SDL_RENDER_SCALE_QUALITY", value ? "1" : "0"))
						return;
					_pixelScale = value;
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
					HandleSound();

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

			private string _title;
			public string Title
			{
				get => _title;
				set
				{
					if (value == _title) return;
					Log($@"Changing window title changed from ""{_title}"" to ""{value}""");
					_title = value;
					SDL_SetWindowTitle(_handle, _title);
				}
			}

			public IBitmap Icon
			{
				set
				{
					int width = value.Width(), height = value.Height();
					byte[] bytes = new byte[width * height * 4];

					int i = 0;
					for (int yy = 0; yy < width; yy++)
					for (int xx = 0; xx < width; xx++)
					{
						Colour colour = value.Palette[value.Bitmap[xx, yy]];
						bytes[i++] = colour.A;
						bytes[i++] = colour.R;
						bytes[i++] = colour.G;
						bytes[i++] = colour.B;
					}

					IntPtr pixels = Marshal.AllocHGlobal(bytes.Length);
					Marshal.Copy(bytes, 0, pixels, bytes.Length);
					
					IntPtr surface = SDL_CreateRGBSurfaceFrom(pixels, width, height, 32, width * 4, 0x0000ff00, 0x00ff0000, 0xff000000, 0x000000ff);

					SDL_SetWindowIcon(_handle, surface);

					SDL_FreeSurface(surface);

					Marshal.FreeHGlobal(pixels);
				}
			}

			public Window(string title, int width, int height, bool fullscreen, bool softwareRender = false)
			{
				_title = title;

				SDL_Init(SDL_INIT.VIDEO | SDL_INIT.AUDIO);

				SDL_WINDOW flags = SDL_WINDOW.RESIZABLE;
				if (_fullscreen = fullscreen) flags |= SDL_WINDOW.FULLSCREEN_DESKTOP;
				_handle = SDL_CreateWindow(title, 100, 100, width, height, flags);
				_renderer = softwareRender ? IntPtr.Zero : SDL_CreateRenderer(_handle, -1, SDL_RENDERER_FLAGS.SDL_RENDERER_ACCELERATED);
				if (_renderer == null || _renderer == IntPtr.Zero)
				{
					_renderer = SDL_CreateRenderer(_handle, -1, SDL_RENDERER_FLAGS.SDL_RENDERER_SOFTWARE);
				}

				if (_handle == null)
				{
					Log("Something is wrong");
					return;
				}

				// Should be default, just to be sure
				PixelScale = false;

				// Run OS native functions for initialization
				Native.Init(_handle);
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