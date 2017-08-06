// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.IO;

namespace CivOne.IO
{
	public class FileSystem
	{
		private static void Log(string text, params object[] parameters) => RuntimeHandler.Runtime.Log(text, parameters);

		private static string[] DATA_FILES = new string[] { "FONTS.CV", "ADSCREEN.PIC", "ARCH.PIC", "BACK0A.PIC", "BACK0M.PIC", "BACK1A.PIC", "BACK1M.PIC", "BACK2A.PIC", "BACK2M.PIC", "BACK3A.PIC", "BIRTH0.PIC", "BIRTH1.PIC", "BIRTH2.PIC", "BIRTH3.PIC", "BIRTH4.PIC", "BIRTH5.PIC", "BIRTH6.PIC", "BIRTH7.PIC", "BIRTH8.PIC", "CASTLE0.PIC", "CASTLE1.PIC", "CASTLE2.PIC", "CASTLE3.PIC", "CASTLE4.PIC", "CBACK.PIC", "CBACKS1.PIC", "CBACKS2.PIC", "CBACKS3.PIC", "CBRUSH0.PIC", "CBRUSH1.PIC", "CBRUSH2.PIC", "CBRUSH3.PIC", "CBRUSH4.PIC", "CBRUSH5.PIC", "CITYPIX1.PIC", "CITYPIX2.PIC", "CITYPIX3.PIC", "CUSTOM.PIC", "DIFFS.PIC", "DISCOVR1.PIC", "DISCOVR2.PIC", "DOCKER.PIC", "GOVT0A.PIC", "GOVT0M.PIC", "GOVT1A.PIC", "GOVT1M.PIC", "GOVT2A.PIC", "GOVT2M.PIC", "GOVT3A.PIC", "HILL.PIC", "ICONPG1.PIC", "ICONPG2.PIC", "ICONPG3.PIC", "ICONPG4.PIC", "ICONPG5.PIC", "ICONPG6.PIC", "ICONPG7.PIC", "ICONPG8.PIC", "ICONPGA.PIC", "ICONPGB.PIC", "ICONPGC.PIC", "ICONPGD.PIC", "ICONPGE.PIC", "ICONPGT1.PIC", "ICONPGT2.PIC", "INVADER2.PIC", "INVADER3.PIC", "INVADERS.PIC", "KING00.PIC", "KING01.PIC", "KING02.PIC", "KING03.PIC", "KING04.PIC", "KING05.PIC", "KING06.PIC", "KING07.PIC", "KING08.PIC", "KING09.PIC", "KING10.PIC", "KING11.PIC", "KING12.PIC", "KING13.PIC", "KINK00.PIC", "KINK03.PIC", "LOGO.PIC", "LOVE1.PIC", "LOVE2.PIC", "MAP.PIC", "NUKE1.PIC", "PLANET1.PIC", "PLANET2.PIC", "POP.PIC", "RIOT.PIC", "RIOT2.PIC", "SAD.PIC", "SETTLERS.PIC", "SLAG2.PIC", "SLAM1.PIC", "SLAM2.PIC", "SP257.PIC", "SP299.PIC", "SPACEST.PIC", "SPRITES.PIC", "TER257.PIC", "TORCH.PIC", "WONDERS.PIC", "WONDERS2.PIC", "BACK0A.PAL", "BACK0M.PAL", "BACK1A.PAL", "BACK1M.PAL", "BACK2A.PAL", "BACK2M.PAL", "BACK3A.PAL", "BIRTH0.PAL", "BIRTH1.PAL", "BIRTH2.PAL", "BIRTH3.PAL", "BIRTH4.PAL", "BIRTH5.PAL", "BIRTH6.PAL", "BIRTH7.PAL", "BIRTH8.PAL", "DISCOVR1.PAL", "DISCOVR2.PAL", "HILL.PAL", "ICONPG1.PAL", "ICONPGA.PAL", "KING00.PAL", "KING01.PAL", "KING02.PAL", "KING03.PAL", "KING04.PAL", "KING05.PAL", "KING06.PAL", "KING07.PAL", "KING08.PAL", "KING09.PAL", "KING10.PAL", "KING11.PAL", "KING12.PAL", "KING13.PAL", "SLAM1.PAL", "SP256.PAL", "SP257.PAL", "BLURB0.TXT", "BLURB1.TXT", "BLURB2.TXT", "BLURB3.TXT", "BLURB4.TXT", "CREDITS.TXT", "ERROR.TXT", "HELP.TXT", "KING.TXT", "PRODUCE.TXT", "STORY.TXT", "CIV.EXE" };
		
		internal static string[] MouseCursorFiles
		{
			get
			{
				return new string[] { "SP257.PIC", "SP257.PAL" };
			}
		}

		public static bool DataFilesExist(params string[] files)
		{
			Log("Checking data files...");
			if (files.Length == 0) files = DATA_FILES;
			foreach (string filename in files)
			{
				if (Directory.GetFiles(Settings.Instance.DataDirectory, filename).Length > 0) continue;
				
				Log("- File not found: {0}", filename);
				return false;
			}
			Log("- Done, all files exist");
			return true;
		}
		
		public static bool CopyDataFiles(string folder)
		{
			Log("Copying data files...");
			foreach (string filename in DATA_FILES)
			{
				string[] files;
				if (File.Exists(Path.Combine(Settings.Instance.DataDirectory, filename))) continue;
				if ((files = Directory.GetFiles(folder, filename)).Length > 0)
				{
					File.Copy(Path.Combine(folder, files[0]), Path.Combine(Settings.Instance.DataDirectory, filename));
					continue;
				}
				Log("- File not found: {0}", filename);
				return false;
			}
			Log("- Done, all copied");
			return true;
		}
	} 
}