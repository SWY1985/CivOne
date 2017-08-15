// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.IO;

namespace CivOne.Graphics
{
	public interface IFont
	{
		int FontHeight { get; }
		byte FirstChar { get; }
		byte LastChar { get; }
		Bytemap GetLetter(char character, byte colour);
	}
}