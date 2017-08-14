// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CivOne.IO;

namespace CivOne.Graphics.ImageFormats
{
	internal class GifFile : IImageFormat, IDisposable
	{
		private Palette _palette;
		private Bytemap _pixels;

		private IEnumerable<byte> GetPixels
		{
			get
			{
				for (int yy = 0; yy < _pixels.Width; yy++)
				for (int xx = 0; xx < _pixels.Height; xx++)
				{
					yield return _pixels[xx, yy];
				}
			}
		}

		private IEnumerable<byte[]> ByteBlock(byte[] input)
		{
			for (int offset = 0; offset < input.Length; offset += 255)
			{
				int length = input.Length - offset;
				if (length > 255) length = 255;
				byte[] output = new byte[length];
				Array.Copy(input, offset, output, 0, length);
				yield return output;
			}
		}

		public byte[] GetBytes()
		{
			using (MemoryStream output = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(output))
			{
				// GIF header
				writer.Write("GIF89a".ToCharArray());

				// Width x Height
				writer.Write((ushort)_pixels.Width);
				writer.Write((ushort)_pixels.Height);

				// GCT Descriptor
				writer.Write((byte)0xF7);

				// Background colour #0
				writer.Write((byte)0x00);
				
				//Default pixel aspect ratio
				writer.Write((byte)0x00);

				// Write colour palette
				for (int i = 0; i < 256; i++)
				{
					byte r = 0, g = 0, b = 0;
					if (_palette.Length > i)
					{
						r = _palette[i].R;
						g = _palette[i].G;
						b = _palette[i].B;
					}
					writer.Write(new byte[] { r, g, b });
				}

				// Image Descriptor
				writer.Write((byte)0x2C);
				// NW Corner
				writer.Write((ushort)0);
				writer.Write((ushort)0);
				// Width x Height
				writer.Write((ushort)_pixels.Width);
				writer.Write((ushort)_pixels.Height);
				// No local colour table
				writer.Write((byte)0x00);

				byte[] encoded = LZW.Encode(GetPixels.ToArray(), true, false, 12);
				
				// Code length
				writer.Write((byte)0x08);
				foreach (byte[] byteBlock in ByteBlock(encoded))
				{
					writer.Write((byte)byteBlock.Length);
					writer.Write(byteBlock);
				}
				writer.Write((byte)0x00);

				// End of file
				writer.Write((byte)0x3B);

				return output.ToArray();
			}
		}

		public GifFile(IBitmap bitmap)
		{
			_palette = bitmap.Palette;
			_pixels = bitmap.Bitmap;
		}

		public void Dispose()
		{
		}
	}
}