// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.GFX;
//using CivOne.Interfaces;
using CivOne.Templates;

namespace CivOne.Screens.Dialogs
{
	internal class TribalHut : BaseDialog
	{
		public TribalHut(params string[] message) : base(100, 80, 9, 7, message)
		{
			for (int i = 0; i < TextLines.Length; i++)
			{
				DialogBox.AddLayer(TextLines[i], 5, (TextLines[i].Height * i) + 5);
			}
		}
	}
}