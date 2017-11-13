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
using System.Runtime.InteropServices;
using System.Text;

namespace CivOne
{
	internal static partial class SDL
	{
		internal abstract partial class Window : IDisposable
		{
			private readonly IntPtr _handle, _renderer;
			private SDL_Event _event;

			private bool _running = true;

			protected readonly int[,] Canvas = new int[320, 200];

			protected void FillRectangle(Rectangle rectangle, Color color)
			{
				SDL_Rect rect = new SDL_Rect() { X = rectangle.X, Y = rectangle.Y, W = rectangle.Width, H = rectangle.Height };

				SDL_SetRenderDrawColor(_renderer, color.R, color.G, color.B, color.A);
				SDL_RenderFillRect(_renderer, ref rect);
			}

			private void HandleEvent(SDL_Event sdlEvent)
			{
				if (sdlEvent.SDL_WindowEvent.Event != SDL_WindowEventID.SDL_WINDOWEVENT_NONE) HandleEventWindow(sdlEvent.SDL_WindowEvent);
			}

			public void Run()
			{
				OnLoad?.Invoke(this, EventArgs.Empty);

				int i = 0;
				while (_running)
				{
					switch (SDL_PollEvent(ref _event))
					{
						case 1:
							HandleEvent(_event);
							break;
						default:
							// Nothing is happening
							break;
					}

					OnUpdate?.Invoke(this, EventArgs.Empty);
					OnDraw?.Invoke(this, EventArgs.Empty);

					SDL_RenderPresent(_renderer);
					Wait(1);
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

			public Window(string title)
			{
				SDL_Init(SDL_INIT.VIDEO);

				_handle = SDL_CreateWindow(title, 100, 100, 640, 400, 0);
				_renderer = SDL_CreateRenderer(_handle, -1, SDL_RENDERER_FLAGS.SDL_RENDERER_ACCELERATED);

				_event = new SDL_Event();

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