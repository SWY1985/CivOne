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
using OpenTK.Audio.OpenAL;

namespace CivOne
{
	internal class WaveFile : IDisposable
	{
		private readonly short _channels, _bps;
		private readonly int _sampleRate;
		private readonly byte[] _data;

		private readonly ALFormat _format;

		public bool Valid { get; private set; }

		public int ALBuffer { get; private set; }
		public int ALSource { get; private set; }

		public WaveFile(string filename)
		{
			Valid = false;

			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			using (BinaryReader br = new BinaryReader(fs))
			{
				if (new string(br.ReadChars(4)) != "RIFF") return;

				int riffSize = br.ReadInt32();

				if (new string(br.ReadChars(4)) != "WAVE") return;
				if (new string(br.ReadChars(4)) != "fmt ") return;

				int chunkSize = br.ReadInt32();
				short audioFormat = br.ReadInt16();
				_channels = br.ReadInt16();
				_sampleRate = br.ReadInt32();
				int byteRate = br.ReadInt32();
				short blockAlign = br.ReadInt16();
				_bps = br.ReadInt16();

				if (new string(br.ReadChars(4)) != "data") return;

				int dataChunkSize = br.ReadInt32();

				_data = br.ReadBytes((int)br.BaseStream.Length);

				switch (_channels)
				{
					case 1:
						if (_bps == 8) _format = ALFormat.Mono8;
						else if (_bps == 16) _format = ALFormat.Mono16;
						break;
					case 2:
						if (_bps == 8) _format = ALFormat.Stereo8;
						else if (_bps == 16) _format = ALFormat.Stereo16;
						break;
					default:
						Valid = false;
						return;
				}

				ALBuffer = AL.GenBuffer();
				ALSource = AL.GenSource();

				AL.BufferData(ALBuffer, _format, _data, _data.Length, _sampleRate);

				AL.Source(ALSource, ALSourcei.Buffer, ALBuffer);
				AL.Source(ALSource, ALSourceb.Looping, false);
				AL.GenSources(ALSource);

				Valid = true;
			}
		}

		public void Dispose()
		{
			AL.DeleteSource(ALSource);
			AL.DeleteBuffer(ALBuffer);
		}
	}
}