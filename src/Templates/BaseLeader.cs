// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.GFX;
using CivOne.Interfaces;

namespace CivOne.Templates
{
	public abstract class BaseLeader : ILeader
	{
		private string _name;
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		private readonly Picture _picture;
		private readonly int _overlayX;
		private readonly int _overlayY;

		public Picture GetPortrait(FaceState state = FaceState.Neutral)
		{
			Picture output = _picture.GetPart(181, 67, 139, 133);
			// TODO: Add face states
			return output;
		}

		public BaseLeader(string name, string picFile, int overlayX, int overlayY)
		{
			Name = name;
			_picture = Resources.Instance.LoadPIC(picFile);
			_overlayX = overlayX;
			_overlayY = overlayY;
		}
	}
}