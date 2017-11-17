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
	internal static class TextureExtensions
	{
		public static void Draw(this SDL.Texture texture, int x, int y, int width, int height)
		{
			if (texture == null || texture.IsEmpty) return;
			texture.Draw(x, y, width, height);
		}
	}
}