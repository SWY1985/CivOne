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
using System.Runtime.InteropServices;

namespace CivOne
{
	internal static partial class SDL
	{
		internal unsafe class Wave : IDisposable
		{
			private SDL_AudioSpec _waveSpec;
			private uint _length;
			private IntPtr _buffer;

			public string Filename { get; }
			public bool Playing { get; private set; }

			public void Play()
			{
				if (Playing) return;

				Playing = true;

				Console.WriteLine($"Sound start: {Path.GetFileName(Filename)}");

				if (SDL_OpenAudio(ref _waveSpec, out _) < 0)
				{
					Console.WriteLine("Could not open audio");
					return;
				}

				SDL_QueueAudio(1, _buffer, _length);
				SDL_PauseAudio(0);
			}

			public Wave(string filename)
			{
				Filename = filename;

				SDL_LoadWAV_RW(Filename, 1, ref _waveSpec, out _buffer, out _length);
			}

			public void Dispose()
			{
				Console.WriteLine($"Sound stop: {Path.GetFileName(Filename)}");

				SDL_PauseAudio(1);
				SDL_CloseAudio();
				SDL_FreeWAV(_buffer);
			}
		}
	}
}