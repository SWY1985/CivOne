// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class Conquest : BaseScreen
	{
		private struct Enemy
		{
			public string DestroyYear;
			public ILeader Leader;
			public ICivilization Civilization;
		}

		private const int NOISE_COUNT = 64;
		private int _noiseCounter;
		private readonly byte[,] _noiseMap;
		private bool _update = true;

		private readonly Enemy[] _enemies;

		private int _enemy = 0;
		private int _step = 0;

		private int _timer = 0;

		private Picture _background, _overlay;
		

		private void SetPalette()
		{
			Color[] palette = _enemies[_enemy].Leader.GetPortrait().Palette;
			for (int i = 64; i < 144; i++)
			{
				_canvas.Palette[i] = palette[i];
			}
		}

		private Point GetPoint(int number)
		{
			switch(number)
			{
				case 0: return new Point(8, 49);
				case 1: return new Point(284, 49);
				case 2: return new Point(54, 49);
				case 3: return new Point(238, 49);
				case 4: return new Point(100, 49);
				case 5: return new Point(192, 49);
				case 6: return new Point(146, 49);
			}
			return new Point(8, 49);
		}

		public override bool HasUpdate(uint gameTick)
		{
			if (++_timer > NOISE_COUNT)
			{
				_timer = 0;
				_step++;
				if (_step == 2)
				{
					_overlay = new Picture(_background);
					_overlay.AddLayer(_enemies[_enemy].Leader.GetPortrait(FaceState.Angry), 90, 0);
					_noiseCounter = NOISE_COUNT + 2;
					_background.AddLayer(_enemies[_enemy].Leader.PortraitSmall, GetPoint(_enemy));
				}
				if (_step == 3)
				{
					_step = 0;
					_enemy++;
					if (_enemy > _enemies.GetUpperBound(0))
					{
						Destroy();
						return true;
					}
					SetPalette();
				}
			}

			switch (_step)
			{
				case 0:
					AddLayer(_background);
					AddLayer(_enemies[_enemy].Leader.GetPortrait(FaceState.Smiling), 90, 0);
					break;
				case 1:
					AddLayer(_background);
					AddLayer(_enemies[_enemy].Leader.GetPortrait(FaceState.Angry), 90, 0);
					_canvas.DrawText($"{_enemies[_enemy].DestroyYear}: {Human.Civilization.NamePlural} destroy", 5, 20, 159, 152, TextAlign.Center);
					_canvas.DrawText($"{_enemies[_enemy].DestroyYear}: {Human.Civilization.NamePlural} destroy", 5, 23, 159, 151, TextAlign.Center);
					_canvas.DrawText($"{_enemies[_enemy].Civilization.Name} civilization!", 5, 20, 159, 168, TextAlign.Center);
					_canvas.DrawText($"{_enemies[_enemy].Civilization.Name} civilization!", 5, 23, 159, 167, TextAlign.Center);
					break;
				case 2:
					_overlay.ApplyNoise(_noiseMap, --_noiseCounter);
					if (_noiseCounter < -2) _timer = 90;
					AddLayer(_background);
					AddLayer(_overlay);
					_canvas.DrawText($"{_enemies[_enemy].DestroyYear}: {Human.Civilization.NamePlural} destroy", 5, 20, 159, 152, TextAlign.Center);
					_canvas.DrawText($"{_enemies[_enemy].DestroyYear}: {Human.Civilization.NamePlural} destroy", 5, 23, 159, 151, TextAlign.Center);
					_canvas.DrawText($"{_enemies[_enemy].Civilization.Name} civilization!", 5, 20, 159, 168, TextAlign.Center);
					_canvas.DrawText($"{_enemies[_enemy].Civilization.Name} civilization!", 5, 23, 159, 167, TextAlign.Center);
					break;
			}

			if (_update) return false;
			_update = false;
			return true;
		}
		
		public override bool KeyDown(KeyboardEventArgs args)
		{
			if (_step < 1)
			{
				_timer = NOISE_COUNT;
				_step = 1;
			}
			return true;
		}
		
		public Conquest()
		{
			_background = Resources.Instance.LoadPIC("SLAM1");
			
			_canvas = new Picture(320, 200, _background.Palette);
			
			AddLayer(_background);
			
			_noiseMap = new byte[320, 200];
			for (int x = 0; x < 320; x++)
			for (int y = 0; y < 200; y++)
			{
				_noiseMap[x, y] = (byte)Common.Random.Next(1, NOISE_COUNT);
			}

			_enemies = Game.Players.Where(x => x != 0 && x != Game.Human).OrderBy(x => x.DestroyTurn).Select(x =>
			new Enemy()
			{
				DestroyYear = Common.YearString((ushort)x.DestroyTurn),
				Leader = x.Civilization.Leader,
				Civilization = x.Civilization
			}).ToArray();

			SetPalette();
		}
	}
}