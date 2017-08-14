// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.Graphics;

namespace CivOne.Leaders
{
	public abstract class BaseLeader : ILeader
	{
		public string Name { get; set; }

		private readonly string _picFile = null;
		private readonly int _overlayX;
		private readonly int _overlayY;
		private Picture _picture, _portraitSmall;

		public Picture GetPortrait(FaceState state = FaceState.Neutral)
		{
			if (_picture == null)
			{
				_picture = Resources.Instance.LoadPIC(_picFile);
			}

			Picture output = _picture.GetPart(181, 67, 139, 133);
			switch (state)
			{
				case FaceState.Smiling:
					output.AddLayer(_picture.GetPart(1, 51, 59, 49), _overlayX, _overlayY);
					break;
				case FaceState.Angry:
					output.AddLayer(_picture.GetPart(1, 151, 59, 49), _overlayX, _overlayY);
					break;
				default:
					// TODO: Add other states
					break;
			}
			return output;
		}

		public Picture PortraitSmall => _portraitSmall;

		public BaseLeader(string name, string picFile, int overlayX, int overlayY)
		{
			Name = name;
			_picFile = picFile;
			_overlayX = overlayX;
			_overlayY = overlayY;
			if (picFile.StartsWith("KING"))
			{
				int id;
				if (int.TryParse(picFile.Substring(4), out id) && id >= 0 && id <= 13)
				{
					int col = (id % 7);
					int row = (id - col) / 7;
					_portraitSmall = Resources.Instance["SLAM2"].GetPart((28 * col) + 1, 34 * row, 27, 33);
					_portraitSmall.ColourReplace(0, 191, 0, 0, 27, 33);
					return;
				}
			}
			_portraitSmall = new Picture(27, 33, Common.GetPalette256);
		}
	}
}