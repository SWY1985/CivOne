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
using System.Linq;
using CivOne.Graphics;

namespace CivOne.Screens.Dialogs
{
	internal class OverwritePlugin : BaseDialog
	{
		private readonly string _source, _destination, _filename;

		private void ConfirmOverwrite(object sender, EventArgs args)
		{
			Plugin plugin = Reflect.Plugins().FirstOrDefault(x => x.Filename == _filename && !x.Deleted);
			if (plugin != null)
			{
				plugin.Delete();
			}
			File.Copy(_source, _destination);
			Reflect.LoadPlugin(_destination);

			Destroy();
		}

		protected override void FirstUpdate()
		{
			Menu menu = new Menu(Palette, Selection(3, 20, 160, 16))
			{
				X = 73,
				Y = 100,
				MenuWidth = 160,
				ActiveColour = 11,
				TextColour = 5,
				FontId = 0
			};
			foreach (string choice in new [] { "No, keep existing", "Yes, overwrite" })
			{
				menu.Items.Add(choice);
			}
			menu.Items[0].Selected += Cancel;
			menu.Items[1].Selected += ConfirmOverwrite;

			menu.MissClick += Cancel;
			menu.Cancel += Cancel;
			AddMenu(menu);
		}

		public OverwritePlugin(string source, string destination) : base(70, 80, 164, 39)
		{
			_source = source;
			_destination = destination;
			_filename = Path.GetFileName(destination);

			DialogBox.DrawText("Overwrite existing plugin", 0, 15, 5, 5);
			DialogBox.DrawText($"file {_filename}?", 0, 15, 5, 13);
		}
	}
}