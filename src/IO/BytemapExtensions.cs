// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using CivOne.Graphics.Sprites;

namespace CivOne.IO
{
	public static class BytemapExtensions
	{
		public static Bytemap Crop(this Bytemap bytemap, int left, int top, int width, int height) => bytemap[left, top, width, height];

		public static Bytemap FromByteArray(this Bytemap bytemap, params byte[] bytes)
		{
			int i = 0;
			for (int yy = 0; yy < bytemap.Height; yy++)
			for (int xx = 0; xx < bytemap.Width; xx++)
			{
				bytemap[xx, yy] = bytes[i++];
				if (i >= bytemap.Length) return bytemap;
			}
			return bytemap;
		}

		public static Bytemap AddLayer(this Bytemap bytemap, ISprite layer, Point point) => AddLayer(bytemap, layer.Bitmap, point.X, point.Y);
		public static Bytemap AddLayer(this Bytemap bytemap, ISprite layer, int left = 0, int top = 0) => AddLayer(bytemap, layer.Bitmap, left, top);
		public static Bytemap AddLayer(this Bytemap bytemap, Bytemap layer, Point point) => AddLayer(bytemap, layer, point.X, point.Y);
		public static Bytemap AddLayer(this Bytemap bytemap, Bytemap layer, int left = 0, int top = 0)
		{
			if (layer == null) return bytemap;

			for (int yy = 0; yy < layer.Height; yy++)
			{
				if (yy + top > bytemap.Height) continue;
				for (int xx = 0; xx < layer.Width; xx++)
				{
					if (xx + left > bytemap.Width) continue;
					if (layer[xx, yy] == 0) continue;
					bytemap[xx + left, yy + top] = layer[xx, yy];
				}
			}
			return bytemap;
		}

		public static Bytemap ColourReplace(this Bytemap bytemap, byte from, byte to) => ColourReplace(bytemap, (from, to));
		public static Bytemap ColourReplace(this Bytemap bytemap, params (byte From, byte To)[] fromToColours)
		{
			if (fromToColours == null) return bytemap;

			for (int yy = 0; yy < bytemap.Height; yy++)
			for (int xx = 0; xx < bytemap.Width; xx++)
			foreach ((byte From, byte To) colour in fromToColours)
			{
				if (bytemap[xx, yy] != colour.From) continue;
				bytemap[xx, yy] = colour.To;
			}
			return bytemap;
		}

		public static Bytemap FillRectangle(this Bytemap bytemap, Rectangle rectangle, byte colour) => FillRectangle(bytemap, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, colour);
		public static Bytemap FillRectangle(this Bytemap bytemap, Point point, Size size, byte colour) => FillRectangle(bytemap, point.X, point.Y, size.Width, size.Height, colour);
		public static Bytemap FillRectangle(this Bytemap bytemap, int left, int top, int width, int height, byte colour)
		{
			bytemap.Fill(left, top, width, height, colour);
			return bytemap;
		}
	}
}