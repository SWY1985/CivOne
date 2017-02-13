// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.IO;
using CivOne.GFX;

namespace CivOne.IO
{
	internal class PicFile
	{
		private readonly byte[] _bytes;
		private readonly byte[,] _colourTable = null;
		private readonly Color[] _palette16 = Common.GetPalette16;
		private readonly Color[] _palette256 = new Color[256];
		private byte[,] _picture16;
		private byte[,] _picture256;

		public Color[] GetPalette16
		{
			get
			{
				return _palette16;
			}
		}

		public Color[] GetPalette256
		{
			get
			{
				return _palette256;
			}
		}

		public byte[,] GetPicture16
		{
			get
			{
				return _picture16;
			}
		}

		public byte[,] GetPicture256
		{
			get
			{
				return _picture256;
			}
		}

		/// <summary>
		/// Read the E0 colour replacement table from the PIC file.
		/// </summary>
		/// <param name="index">Current file reading index.</param>
		/// <returns>Returns a byte array containing the colour replacement table.</returns>
		private byte[,] ReadColourTable(ref int index)
		{
			// the 4-bit colour conversion table has 2 entries for each colour
			// that are painted in a chessboard-pattern, so one 8-bit colour can
			// be replaced by two different 4-bit colours
			byte[,] colourTable = new byte[256, 2];
			uint length = BitConverter.ToUInt16(_bytes, index); index += 2;
			byte firstIndex = _bytes[index++];
			byte lastIndex = _bytes[index++];
			
			// create all colour entries
			for (int i = 0; i < 256; i++)
			{
				// if the colour entries fall outside the first/last index range, they
				// will use colour 0 (transparent)
				// this never happens for any of the original Civilization resources
				if (i < firstIndex || i > lastIndex)
				{
					for (int j = 0; j < 2; j++)
					{
						colourTable[i, j] = 0;
					}
					continue;
				}
				
				// split the byte into two nibbles, each containing a colour number
				colourTable[i, 0] = (byte)((_bytes[index] & 0xF0) >> 4);
				colourTable[i, 1] = (byte)(_bytes[index] & 0x0F);
				index++;
			}
			
			// This is a fix for transparency in 16 colour mode
			colourTable[0, 0] = 0;
			colourTable[0, 1] = 0;
			
			return colourTable;
		}

		/// <summary>
		/// Read the M0 colour palette.
		/// </summary>
		/// <param name="index">Current file reading index.</param>
		private void ReadColourPalette(ref int index)
		{
			uint length = BitConverter.ToUInt16(_bytes, index); index += 2;
			byte firstIndex = _bytes[index++];
			byte lastIndex = _bytes[index++];
			for (int i = 0; i < 256; i++)
			{
				// if the colour entry fall outside the first/last index range, use
				// a transparent colour entry
				// this never happens for any of the original Civilization resources
				if (i < firstIndex || i > lastIndex)
				{
					_palette256[i] = Color.Transparent;
					continue;
				}
				byte red = _bytes[index++], green = _bytes[index++], blue = _bytes[index++];
				_palette256[i] = new Color(255, red * 4, green * 4, blue * 4);
			}
			
			// always set colour 0 to transparent
			_palette256[0] = Color.Transparent;
		}

		/// <summary>
		/// Extract/Decode the LZW/RLE encoded bytes.
		/// </summary>
		/// <param name="index">Current file reading index.</param>
		/// <param name="length">Number of bytes to decode.</param>
		/// <returns></returns>
		private byte[] DecodePicture(ref int index, uint length)
		{
			byte bits = _bytes[index++];
			byte[] img = new byte[length - 5];
			Array.Copy(_bytes, index, img, 0, (int)(length - 5));

			byte[] output;

			using (MemoryStream ms = new MemoryStream())
			{
				foreach (byte b in img) ms.WriteByte(b);
				ms.Flush();
				ms.Position = 0;
				using (BinaryReader br = new BinaryReader(ms))
				{
					int[] indexes = LZWDecoder.ConvertByteStream(br, img.Length, bits);
					int[] decoded = LZWDecoder.Decode(indexes);
					int[] bytes = RLECodec.Decode(decoded);

					output = new byte[bytes.Length];
					for (int i = 0; i < bytes.Length; i++)
					{
						output[i] = (byte)bytes[i];
					}
				}
			}
			
			index += (int)(length - 5);
			return output;
		}
		
		/// <summary>
		/// Read the 8-bit image into a 2D byte array.
		/// </summary>
		/// <param name="index">Current file reading index.</param>
		private void ReadPictureX0(ref int index)
		{
			uint length = BitConverter.ToUInt16(_bytes, index); index += 2;
			uint width = BitConverter.ToUInt16(_bytes, index); index += 2;
			uint height = BitConverter.ToUInt16(_bytes, index); index += 2;
			
			_picture256 = new byte[width, height];
			
			byte[] image = DecodePicture(ref index, length);
			int c = 0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					_picture256[x, y] = image[c++];
				}
			}
		}
		
		/// <summary>
		/// Read the 4-bit image into a 2D byte array.
		/// </summary>
		/// <param name="index">Current file reading index.</param>
		private void ReadPictureX1(ref int index)
		{
			uint length = BitConverter.ToUInt16(_bytes, index); index += 2;
			uint width = BitConverter.ToUInt16(_bytes, index); index += 2;
			uint height = BitConverter.ToUInt16(_bytes, index); index += 2;
			
			_picture16 = new byte[width, height];

			byte[] image = DecodePicture(ref index, length);
			int c = 0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					_picture16[x++, y] = (byte)(image[c] & 0x0F);
					_picture16[x, y] = (byte)((image[c++] & 0xF0) >> 4);
				}
			}
		}
		
		/// <summary>
		/// Generate a 4-bit image from the 8-bit image and colourtable.
		/// </summary>
		/// <param name="colourTable">Colour table that was generated</param>
		private void ConvertPictureX0(byte[,] colourTable)
		{
			if (colourTable == null) return;
			
			int width = _picture256.GetLength(0);
			int height = _picture256.GetLength(1);
			
			_picture16 = new byte[width, height];
			
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					byte col256 = _picture256[x, y];
					_picture16[x, y] = colourTable[col256, (y + x) % 2];
				}
			}
		}
		
		public PicFile(string filename)
		{
			if (!filename.ToLower().EndsWith(".map"))
			{
				// fix for case sensitive file systems
				foreach (string fileEntry in Directory.GetFiles(Settings.Instance.DataDirectory))
				{
					if (Path.GetFileName(fileEntry).ToLower() != string.Format("{0}.pic", filename.ToLower())) continue;
					filename = fileEntry;
				}
			}

			// generate an exception if the file is not found
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(string.Format("File not found: {0}.PIC", filename.ToUpper()));
			}

			// read all bytes into a byte array
			using (FileStream fs = new FileStream(filename, FileMode.Open))
			{
				_bytes = new byte[fs.Length];
				for (int i = 0; i < fs.Length; i++)
				{
					_bytes[i] = (byte)fs.ReadByte();
				}
			}
			
			int index = 0;
			while (index < (_bytes.Length - 1))
			{
				uint magicCode = BitConverter.ToUInt16(_bytes, index); index += 2;
				switch (magicCode)
				{
					case 0x3045:
						_colourTable = ReadColourTable(ref index);
						break;
					case 0x304D:
						ReadColourPalette(ref index);
						break;
					case 0x3058:
						ReadPictureX0(ref index);
						ConvertPictureX0(_colourTable);
						break;
					case 0x3158:
						ReadPictureX1(ref index);
						break;
				}
			}
		}
	}
}