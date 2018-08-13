// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.IO;
using CivOne.Enums;
using CivOne.Graphics;
using CivOne.Players;
using CivOne.UserInterface;

namespace CivOne
{
	public abstract class BaseInstance
	{
		private protected static Game Game => Game.Instance;
		private protected static Map Map => Map.Instance;
		private protected static Player Human => Game.Instance.HumanPlayer;
		private protected static Resources Resources => Resources.Instance;
		private protected static IRuntime Runtime => RuntimeHandler.Runtime;
		private protected static Settings Settings => Settings.Instance;
		private protected static MenuCollection Menus => MenuCollection.Instance;

		private protected static void Log(string text, params object[] parameters) => Runtime.Log(text, parameters);
		private protected static void PlaySound(string filename)
		{
			if (!(Game.Started && Game.Sound) || Settings.Sound == GameOption.Off || !File.Exists(filename = filename.GetSoundFile())) return;
			Runtime.PlaySound(filename);
		}

		private protected bool GFX256 => (Settings.GraphicsMode == GraphicsMode.Graphics256);
	}
}