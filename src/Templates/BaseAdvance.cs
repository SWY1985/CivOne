// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using System.Linq;
using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	internal abstract class BaseAdvance : IAdvance
	{
		private Advance[] _requiredTechs;
		private IEnumerable<IAdvance> GetRequiredTechs()
		{
			foreach (Advance advance in _requiredTechs)
			{
				yield return Common.Advances.Where(x => x.Id == (byte)advance).FirstOrDefault();
			}
		}
		
		public virtual Picture Icon
		{
			get
			{
				return null;
			}
		}
		
		public string Name { get; protected set; }
		public byte PageCount
		{
			get
			{
				return 2;
			}
		}
		public Picture DrawPage(byte pageNumber)
		{
			return new Picture(320, 200);
		}
		
		protected Advance Type { get; set; }
		
		public IAdvance[] RequiredTechs
		{
			get
			{
				return GetRequiredTechs().ToArray();
			}
		}
		
		public byte Id
		{
			get
			{
				return (byte)Type;
			}
		}
		
		protected BaseAdvance(params Advance[] requiredTechs)
		{
			_requiredTechs = requiredTechs;
		}
	}
}