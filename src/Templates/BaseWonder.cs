// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseWonder : IWonder
	{
		public string Name { get; protected set; }
		public virtual Picture Icon
		{
			get
			{
				return null;
			}
		}
		public byte PageCount
		{
			get
			{
				return 2;
			}
		}
		public Picture DrawPage(byte pageNumber)
		{
			string[] text = new string[0];
			switch (pageNumber)
			{
				case 1:
					text = Resources.Instance.GetCivilopediaText(Name.ToUpper());
					break;
				case 2:
					text = Resources.Instance.GetCivilopediaText(Name.ToUpper() + "2");
					break;
				default:
					Console.WriteLine("Invalid page number: {0}", pageNumber);
					break;
			}
			
			Picture output = new Picture(320, 200);
			
			int yy = 76;
			foreach (string line in text)
			{
				Console.WriteLine(line);
				output.DrawText(line, 6, 1, 12, yy);
				yy += 9;
			}
			
			if (pageNumber == 2)
			{
				yy += 8;
				string requiredTech = "";
				if (RequiredTech != null) requiredTech = RequiredTech.Name;
				output.DrawText(string.Format("Requires {0}", requiredTech), 6, 9, 12, yy); yy += 8;
				output.DrawText(string.Format("Cost: {0}0 shields.", Price), 6, 9, 12, yy); yy += 8;
				output.DrawText(string.Format("Maintenance: ${0}", 0), 6, 12, 12, yy);
			}
			
			return output;
		}
		
		public IAdvance RequiredTech { get; protected set; }
		public IAdvance ObsoleteTech { get; protected set; }
		public byte Price { get; protected set; }
		
		protected BaseWonder(byte price = 1)
		{
			Price = price;
		}
	}
}