// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne
{
	internal static partial class SDL
	{
		private struct SDL_PixelFormat
		{
			public SDL_PixelFormatEnum format;
			public SDL_Palette palette;
			public byte BitsPerPixel;
			public byte BytesPerPixel;
			public uint Rmask;
			public uint Gmask;
			public uint Bmask;
			public uint Amask;

			public static SDL_PixelFormat SDL_PIXELFORMAT_RGBA8888
			{
				get
				{
					return new SDL_PixelFormat()
					{
						format = SDL_PixelFormatEnum.SDL_PIXELFORMAT_RGBA8888
					};
				}
			}
		}
	}
}