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
		internal abstract partial class Window
		{
			private Wave _currentSound = null;

			private void HandleSound()
			{
				if (_currentSound == null || SDL_GetQueuedAudioSize(1) > 0) return;

				StopSound();
			}

			protected void PlaySound(string filename)
			{
				if (_currentSound != null) StopSound();

				_currentSound = new Wave(filename);
				_currentSound.Play();
			}

			protected void StopSound()
			{
				_currentSound.Dispose();
				_currentSound = null;
			}
		}
	}
}