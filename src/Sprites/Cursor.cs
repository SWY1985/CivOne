// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Graphics;
using CivOne.IO;

using static CivOne.Enums.CursorType;

namespace CivOne.Sprites
{
	public static class Cursor
	{
		private static Resources Resources => Resources.Instance;
		private static Settings Settings => Settings.Instance;

		private static ISprite CursorPointer()
		{
			switch(Settings.CursorType)
			{
				case Default:
					return new CachedSprite(() => Resources["SP257"][113, 33, 15, 15].Bitmap);
				case Builtin:
					return new CachedSprite(() => new Bytemap(11, 16).FromByteArray(new byte[] {
						 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0,
						 5,15, 5, 0, 0, 0, 0, 0, 0, 0, 0,
						 5,15,15, 5, 0, 0, 0, 0, 0, 0, 0,
						 5,15,15,15, 5, 0, 0, 0, 0, 0, 0,
						 5,15,15,15,15, 5, 0, 0, 0, 0, 0,
						 5,15,15,15,15,15, 5, 0, 0, 0, 0,
						 5,15,15,15,15,15,15, 5, 0, 0, 0,
						 5,15,15,15,15,15,15,15, 5, 0, 0,
						 5,15,15,15,15,15,15,15,15, 5, 0,
						 5, 5, 5, 5,15,15, 5, 5, 5, 5, 5,
						 0, 0, 0, 0, 5,15,15, 5, 0, 0, 0,
						 0, 0, 0, 0, 5,15,15, 5, 0, 0, 0,
						 0, 0, 0, 0, 0, 5,15,15, 5, 0, 0,
						 0, 0, 0, 0, 0, 5,15,15, 5, 0, 0,
						 0, 0, 0, 0, 0, 0, 5, 5, 0, 0, 0
					}));
				default:
					return null;
			}
		}

		private static ISprite CursorGoto()
		{
			switch(Settings.CursorType)
			{
				case Default:
					return new CachedSprite(() => Resources["SP257"][33, 33, 15, 15].Bitmap);
				case Builtin:
					return new CachedSprite(() => new Bytemap(13, 16).FromByteArray(new byte[] {
						 5, 0, 0, 0, 0,15,15, 0, 0, 0,15,15, 0,
						 5, 5, 0, 0,15, 0, 0, 0, 0,15, 0, 0,15,
						 5,15, 5, 0,15, 0,15,15, 0,15, 0, 0,15,
						 5,15,15, 5,15, 0, 0,15, 0,15, 0, 0,15,
						 5,15,15,15, 5,15,15, 0, 0, 0,15,15, 0,
						 5,15,15,15,15, 5, 0, 0, 0, 0, 0, 0, 0,
						 5,15,15,15,15,15, 5, 0, 0, 0, 0, 0, 0,
						 5,15,15,15,15,15,15, 5, 0, 0, 0, 0, 0,
						 5,15,15,15,15,15,15,15, 5, 0, 0, 0, 0,
						 5,15,15,15,15,15,15,15,15, 5, 0, 0, 0,
						 5, 5, 5, 5,15,15, 5, 5, 5, 5, 5, 0, 0,
						 0, 0, 0, 0, 5,15,15, 5, 0, 0, 0, 0, 0,
						 0, 0, 0, 0, 5,15,15, 5, 0, 0, 0, 0, 0,
						 0, 0, 0, 0, 0, 5,15,15, 5, 0, 0, 0, 0,
						 0, 0, 0, 0, 0, 5,15,15, 5, 0, 0, 0, 0,
						 0, 0, 0, 0, 0, 0, 5, 5, 0, 0, 0, 0, 0
					}));
				default:
					return null;
			}
		}

		public static ISprite Pointer = CursorPointer();
		public static ISprite Goto = CursorGoto();
		public static ISprite Current
		{
			get
			{
				switch (Common.TopScreen?.Cursor)
				{
					case MouseCursor.Pointer:
						return Pointer;
					case MouseCursor.Goto:
						return Goto;
					default:
						return null;
				}
			}
		}

		public static void ClearCache()
		{
			Pointer = CursorPointer();
			Goto = CursorGoto();
		}
	}
}