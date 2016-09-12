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
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne.Templates
{
	internal abstract class BaseUnitAir : BaseUnit
	{
		protected BaseUnitAir(byte price = 1, byte attack = 1, byte defense = 1, byte move = 1) : base(price, attack, defense, move)
		{
			Class = UnitClass.Land;
		}
	}
}