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
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using CivOne.Enums;

namespace CivOne
{
	internal class Sounds : IDisposable
	{
		private void Log(string value, params object[] formatArgs) => _runtime.Log(value, formatArgs);

		private readonly Runtime _runtime;

		private static AudioContext _audioContext = null;
		private static WaveFile _waveFile = null;
		private static bool _initError = false;

		internal void PlaySound(string filename)
		{
			if (_initError) return;

			StopSound();

			if (!File.Exists(filename))
			{
				Log($"File not found: {filename}");
				return;
			}
			
			_waveFile = new WaveFile(filename);
			if (!_waveFile.Valid)
			{
				Log($"Invalid wave file: {filename}");
				_waveFile.Dispose();
				return;
			}
				
			AL.SourcePlay(_waveFile.ALSource);
		}
		
		internal void StopSound()
		{
			if (_initError || _waveFile == null) return;
			
			AL.SourceStop(_waveFile.ALSource);
			_waveFile.Dispose();
			_waveFile = null;
		}

		public Sounds(Runtime runtime)
		{
			_runtime = runtime;

			if (Native.Platform != Platform.Windows)
			{
				_initError = true;
				Log($"Sound is not supported on {Native.Platform}.");
				return;
			}
			
			try
			{
				_audioContext = new AudioContext();
			}
			catch(TypeInitializationException)
			{
				_initError = true;
				Log("Could not initialize audio, is OpenAL installed?");
			}
			catch
			{
				_initError = true;
				Log("Could not initialize audio device.");
			}
		}

		public void Dispose()
		{
			_audioContext?.Dispose();
		}
	}
}