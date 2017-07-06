// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Events;
using CivOne.GFX;
using CivOne.IO;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class PalaceView : BaseScreen
	{
		private enum Stage
		{
			View,
			Message,
			SelectPart,
			SelectStyle
		}

		private readonly Picture _background;

		private Stage _currentStage = Stage.View;

		private bool _update = true;
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				AddLayer(_background);
				AddLayer(Resources.Instance["CASTLE0"].GetPart(0, 1, 52, 99), 135, 37);
				AddLayer(Resources.Instance["CASTLE0"].GetPart(53, 1, 26, 99), 185, 37);
				AddLayer(Resources.Instance["CASTLE0"].GetPart(78, 1, 24, 99), 114, 37);

				switch (_currentStage)
				{
					case Stage.Message:
						{
							Picture message = new Picture(269, 39);
							message.FillLayerTile(Patterns.PanelGrey);
							message.AddBorder(15, 8, 0, 0, 269, 39);
							int yy = 4;
							foreach (string line in TextFile.Instance.GetGameText("KING/PALACE"))
							{
								message.DrawText(line.Trim('^'), 0, 15, 4, yy);
								yy += 8;
							}
							_canvas.FillRectangle(5, 20, 16, 271, 41);
							AddLayer(message, 21, 17);
						}
						break;
					case Stage.SelectPart:
						{
							Picture message = new Picture(180, 15);
							message.FillLayerTile(Patterns.PanelGrey);
							message.AddBorder(15, 8, 0, 0, 180, 15);
							message.DrawText("Which section shall we improve?", 0, 15, 4, 4);
							_canvas.FillRectangle(5, 40, 16, 182, 17);
							AddLayer(message, 41, 17);

							for (int i = 0; i < 7; i++)
							{
								int xx = 12 + (48 * i);
								_canvas.DrawText($"{i + 1}", 0, 5, xx, 145);
								_canvas.DrawText($"{i + 1}", 0, 14, xx, 144);
							}
							for (int i = 0; i < 3; i++)
							{
								int xx = 40 + (120 * i);
								_canvas.DrawText($"{(char)('A' + i)}", 0, 5, xx, 161);
								_canvas.DrawText($"{(char)('A' + i)}", 0, 14, xx, 160);
							}
						}
						break;
					case Stage.SelectStyle:
						break;
				}

				_update = false;
				return true;
			}
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			switch (_currentStage)
			{
				case Stage.Message:
					_currentStage = Stage.SelectPart;
					_update = true;
					break;
				case Stage.SelectPart:
					_currentStage = Stage.View;
					_update = true;
					break;
				case Stage.View:
					Destroy();
					break;
			}
			return true;
		}
		
		public override bool MouseDown(ScreenEventArgs args)
		{
			switch (_currentStage)
			{
				case Stage.Message:
					_currentStage = Stage.SelectPart;
					_update = true;
					break;
				case Stage.SelectPart:
					_currentStage = Stage.View;
					_update = true;
					break;
				case Stage.View:
					Destroy();
					break;
			}
			return true;
		}
		
		public PalaceView(bool build = false)
		{
			_background = Resources.Instance.LoadPIC("CBACK");
			
			_canvas = new Picture(320, 200, _background.Palette);
			if (build) _currentStage = Stage.Message;
		}
	}
}