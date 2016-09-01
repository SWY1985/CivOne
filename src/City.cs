// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.Units;

namespace CivOne
{
	internal class City
	{
		internal byte X;
		internal byte Y;
		internal byte Owner;
		internal string Name;
		internal byte Size;
		internal int Shields { get; private set; }
		internal IProduction CurrentProduction { get; private set; }

		internal void NewTurn()
		{
			// Temporary code
			Shields++;
			if (Shields == (int)CurrentProduction.Price * 10)
			{
				Shields = 0;
				if (CurrentProduction is IUnit)
				{
					Game.Instance.CreateUnit((CurrentProduction as IUnit).Type, X, Y, Owner);
				}
			}
		}

		internal City()
		{
			CurrentProduction = new Militia();
		}
	}
}