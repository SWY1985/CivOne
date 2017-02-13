// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Linq;
using CivOne.Events;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens.Dialogs;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityBuildings : BaseScreen
	{
		private readonly City _city;
		private IProduction[] _improvements;

		private readonly Picture _background;
		
		private bool _update = true;
		
		public event EventHandler BuildingUpdate;

		private int _page = 0;

		private void DrawWonder(IWonder wonder, int offset)
		{
			int xx = (offset % 2 == 0) ? 21 : 1;
			int yy = -1 + (6 * offset);
			if (yy < 0)
				AddLayer(wonder.SmallIcon.GetPart(0, Math.Abs(yy), wonder.SmallIcon.Width, wonder.SmallIcon.Height + yy), xx, 0);
			else
				AddLayer(wonder.SmallIcon, xx, yy);
			
			string name = wonder.Name;
			while (Resources.Instance.GetTextSize(1, name).Width > 62)
			{
				name = $"{name.Substring(0, name.Length - 2)}.";
			}
			_canvas.DrawText(name, 1, 15, 42, 3 + (6 * offset));
		}

		private void DrawBuilding(IBuilding building, int offset)
		{
			int xx = (offset % 2 == 0) ? 21 : 1;
			int yy = -1 + (6 * offset);
			if (yy < 0)
				AddLayer(building.SmallIcon.GetPart(0, Math.Abs(yy), building.SmallIcon.Width, building.SmallIcon.Height + yy), xx, 0);
			else
				AddLayer(building.SmallIcon, xx, yy);

			string name = building.Name;
			while (Resources.Instance.GetTextSize(1, name).Width > 54)
			{
				name = $"{name.Substring(0, name.Length - 1)}";
			}
			_canvas.DrawText(name, 1, 15, 42, 3 + (6 * offset));
			AddLayer(Icons.SellButton, 98, 2 + (6 * offset));
		}

		private IEnumerable<IProduction> GetImprovements
		{
			get
			{
				foreach (IWonder wonder in _city.Wonders)
					yield return wonder;
				foreach (IBuilding building in _city.Buildings)
					yield return building;
			}
		}
		
		public override bool HasUpdate(uint gameTick)
		{
			if (_update)
			{
				_canvas.FillLayerTile(_background);
				_canvas.FillRectangle(0, 107, 0, 1, 97);

				for (int i = (_page * 14); i < _improvements.Length && i < ((_page + 1) * 14); i++)
				{
					if (_improvements[i] is IWonder)
					{
						DrawWonder((_improvements[i] as IWonder), i % 14);
						continue;
					}
					DrawBuilding((_improvements[i] as IBuilding), i % 14);
					continue;
				}

				if (_improvements.Length > 14)
				{
					DrawButton("More", 9, 1, 76, 87, 29);
				}

				_canvas.AddBorder(1, 1, 0, 0, 107, 97);
				
				_update = false;
			}
			return true;
		}

		private void SellBuilding(object sender, EventArgs args)
		{
			_city.SellBuilding((sender as ConfirmSell).Building);
			_page = 0;
			_improvements = GetImprovements.ToArray();
			_update = true;
			if (BuildingUpdate != null)
				BuildingUpdate(this, null);
		}

		public override bool MouseDown(ScreenEventArgs args)
		{
			if (!_city.BuildingSold && args.X > 97 && args.X < 105)
			{
				int yy = 2;
				for (int i = (_page * 14); i < _improvements.Length && i < ((_page + 1) * 14); i++)
				{
					if (args.Y >= yy && args.Y < yy + 8 && _improvements[i] is IBuilding)
					{
						ConfirmSell confirmSell = new ConfirmSell(_improvements[i] as IBuilding);
						confirmSell.Sell += SellBuilding;
						Common.AddScreen(confirmSell);
						return true;
					}
					yy += 6;
				}
			}

			if (args.X > 75 && args.X < 105 && args.Y > 86 && args.Y < 96)
			{
				_page++;
				if ((_page * 14) > _improvements.Length) _page = 0;
				_update = true;
				return true;
			}
			return false;
		}

		public void Close()
		{
			Destroy();
		}

		public CityBuildings(City city, Picture background)
		{
			_city = city;
			_improvements = GetImprovements.ToArray();
			_background = background;

			_canvas = new Picture(108, 97, background.Palette);
		}
	}
}