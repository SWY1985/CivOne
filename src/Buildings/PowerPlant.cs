// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Advances;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Templates;

namespace CivOne.Buildings
{
	internal class PowerPlant : BaseBuilding
	{
		private static Picture _iconCache = null;
		
		public PowerPlant() : base(16, 4, 160, 640)
		{
			Name = "Power Plant";
			RequiredTech = new Refining();
			if (_iconCache == null)
			{
				SetIcon(4, 1, false);
				Picture icon = new Picture(52, 50, Icon.Image.Palette.Entries);
				icon.AddLayer(Icon.GetPart(31, 0, 20, 50), 1);
				icon.AddLayer(Icon.GetPart(0, 0, 32, 50), 19);
				icon.FillRectangle(0, 50, 0, 2, 50);
				_iconCache = icon;
			}
			Icon = _iconCache;
			SetSmallIcon(3, 3);
			// TODO: Fix icon in patch, should be: SetSmallIcon(3, 4);
			Type = Building.PowerPlant;
		}
	}
}