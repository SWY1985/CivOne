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
		private enum SDL_AudioFormat : ushort
		{
			AUDIO_S8,
			AUDIO_U8,
			AUDIO_S16LSB,
			AUDIO_S16MSB,
			AUDIO_S16SYS,
			AUDIO_S16,
			AUDIO_U16LSB,
			AUDIO_U16MSB,
			AUDIO_U16SYS,
			AUDIO_U16,
			AUDIO_S32LSB,
			AUDIO_S32MSB,
			AUDIO_S32SYS,
			AUDIO_S32,
			AUDIO_F32LSB,
			AUDIO_F32MSB,
			AUDIO_F32SYS,
			AUDIO_F32
		}
	}
}