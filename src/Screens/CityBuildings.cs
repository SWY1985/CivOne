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
using System.Drawing;
using System.Linq;
using CivOne.Buildings;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens
{
	internal class CityBuildings : BaseScreen
	{
		private readonly City _city;
		private readonly IProduction[] _improvements;

		private readonly Bitmap _background;
		
		private bool _update = true;

		private int _page = 0;

		private void DrawWonder(IWonder wonder, int offset)
		{
			int xx = (offset % 2 == 0) ? 21 : 1;
			int yy = -1 + (6 * offset);
			if (yy < 0)
				AddLayer(wonder.SmallIcon.GetPart(0, Math.Abs(yy), wonder.SmallIcon.Image.Width, wonder.SmallIcon.Image.Height + yy), xx, 0);
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
				AddLayer(building.SmallIcon.GetPart(0, Math.Abs(yy), building.SmallIcon.Image.Width, building.SmallIcon.Image.Height + yy), xx, 0);
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
/*
		private void DrawWonders(int offset = 0)
		{
			IWonder[] wonders = _city.Wonders.ToArray();
			for (int i = 0; i < wonders.Length; i++)
			{
				int ii = (i + offset);
				int xx = (ii % 2 == 0) ? 21 : 1;
				int yy = -1 + (6 * ii);
				if (yy < 0)
					AddLayer(wonders[i].SmallIcon.GetPart(0, Math.Abs(yy), wonders[i].SmallIcon.Image.Width, wonders[i].SmallIcon.Image.Height + yy), xx, 0);
				else
					AddLayer(wonders[i].SmallIcon, xx, yy);
				
				string name = wonders[i].Name;
				while (Resources.Instance.GetTextSize(1, name).Width > 62)
				{
					name = $"{name.Substring(0, name.Length - 2)}.";
				}
				_canvas.DrawText(name, 1, 15, 42, 3 + (6 * ii));
			}
		}

		private void DrawBuildings(int offset = 0)
		{
			IBuilding[] buildings = _city.Buildings.ToArray();
			for (int i = 0; i < buildings.Length; i++)
			{
				int ii = (i + offset);
				int xx = (ii % 2 == 0) ? 21 : 1;
				int yy = -1 + (6 * ii);
				if (yy < 0)
					AddLayer(buildings[i].SmallIcon.GetPart(0, Math.Abs(yy), buildings[i].SmallIcon.Image.Width, buildings[i].SmallIcon.Image.Height + yy), xx, 0);
				else
					AddLayer(buildings[i].SmallIcon, xx, yy);
				
				string name = buildings[i].Name;
				while (Resources.Instance.GetTextSize(1, name).Width > 54)
				{
					name = $"{name.Substring(0, name.Length - 1)}";
				}
				_canvas.DrawText(name, 1, 15, 42, 3 + (6 * ii));
				AddLayer(Icons.SellButton, 98, 2 + (6 * ii));
			}
		}*/

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

		public void Close()
		{
			Destroy();
		}

		public CityBuildings(City city, Bitmap background)
		{
			_city = city;
			_improvements = GetImprovements.ToArray();
			_background = background;

			_canvas = new Picture(108, 97, background.Palette.Entries);
		}
	}
}