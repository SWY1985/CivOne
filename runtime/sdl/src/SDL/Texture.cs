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
		internal class Texture : IDisposable
		{
			private static uint SDL_PIXELFORMAT_ABGR8888 => DefinePixelformat(SDL_PixelType.SDL_PIXELTYPE_PACKED32, SDL_PixelOrder.SDL_PACKEDORDER_ABGR, SDL_PixelLayout.SDL_PACKEDLAYOUT_8888, 32, 4);

			private readonly IntPtr _renderer, _handle;

			public int Width { get; private set; }
			public int Height { get; private set; }

			private int[] PaletteArray(Palette palette)
			{
				int[] output = new int[palette.Length];
				IntPtr ptr = Marshal.AllocHGlobal(palette.Length * 4);
				for (int i = 0; i < output.Length; i++)
				{
					Colour colour = palette[i];
					Marshal.WriteInt32(ptr, (i * 4), ((int)colour.A << 24) + ((int)colour.B << 16) + ((int)colour.G << 8) + ((int)colour.R));
				}
				Marshal.Copy(ptr, output, 0, output.Length);
				Marshal.FreeHGlobal(ptr);
				return output;
			}

			private bool HasAlpha(Palette palette) => palette.Entries.Any(x => x.A != 255);

			public bool IsEmpty => (_handle == IntPtr.Zero);

			public void Draw(int x, int y, int width, int height)
			{
				if (IsEmpty) return;

				SDL_Rect src = new SDL_Rect() { X = 0, Y = 0, W = Width, H = Height };
				SDL_Rect dst = new SDL_Rect() { X = x, Y = y, W = width, H = height };

				SDL_RenderCopy(_renderer, _handle, ref src, ref dst);
			}

			internal Texture(IntPtr renderer, IBitmap bitmap)
			{
				if (bitmap == null)
				{
					// Do not load empty bitmap
					_handle = IntPtr.Zero;
					return;
				}

				Width = bitmap.Width();
				Height = bitmap.Height();

				_renderer = renderer;
				_handle = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ABGR8888, SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, Width, Height);
				SDL_Rect rect = new SDL_Rect() { X = 0, Y = 0, W = Width, H = Height };
				int[] palette = PaletteArray(bitmap.Palette);
				bool hasAlpha = bitmap.Palette.Entries.Any(x => x.A != 255);
				if (HasAlpha(bitmap.Palette)) SDL_SetTextureBlendMode(_handle, SDL_BlendMode.SDL_BLENDMODE_BLEND);
				if (SDL_LockTexture(_handle, ref rect, out IntPtr pixels, out int pitch) == 0)
				{
					int[] src = bitmap.Bitmap.ToColourMap(palette);
					Marshal.Copy(bitmap.Bitmap.ToColourMap(palette), 0, pixels, Width * Height);
					SDL_UnlockTexture(_handle);
				}
			}

			public void Dispose()
			{
				if (IsEmpty) return;
				SDL_DestroyTexture(_handle);
			}
		}
	}
}