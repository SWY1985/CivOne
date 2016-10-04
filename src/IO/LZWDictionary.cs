// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

namespace CivOne.IO
{
	/// <remarks>
	/// This code is based on JCivED source code by darkpanda. <http://sourceforge.net/projects/jcived/>
	/// </remarks>
	internal class LZWDictionary
	{
		private const int MAX_BITS = 11;

		private int[][] _dictionary;
		private int _cursorPosition;

		public int AddEntry(int[] entry)
		{
			if (_cursorPosition < _dictionary.Length)
				_dictionary[_cursorPosition++] = entry;
			return _cursorPosition - 1;
		}

		public int[] GetEntry(int position)
		{
			return (position < _cursorPosition) ? _dictionary[position] : null;
		}

		public int CursorPosition
		{
			get
			{
				return _cursorPosition;
			}
		}

		public bool IsFull
		{
			get
			{
				return (_cursorPosition == _dictionary.Length);
			}
		}

		public LZWDictionary()
		{
			int dictionarySize = (1 << MAX_BITS);
			_dictionary = new int[dictionarySize][];

			for (int i = 0; i < dictionarySize; i++)
			{
				if (i > 255)
				{
					_dictionary[i] = null;
					continue;
				}
				_dictionary[i] = new int[] { i };
			}

			_cursorPosition = 257;
		}
	}
}