// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.IO;

namespace CivOne.UserInterface
{
	public interface IElement : IDisposable
	{
		int Left { get; }
		int Top { get; }
		Bytemap Bitmap { get; }
		void Click();
	}
}