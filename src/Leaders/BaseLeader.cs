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
using System.Collections.Generic;
using System.Linq;

namespace CivOne.Leaders
{
	public abstract class BaseLeader : BaseInstance, ILeader
	{
		private string _defaultName;
		private string DefaultName
		{
			get
			{
				foreach (LeaderModification modification in Modifications)
				{
					if (!modification.Name.HasValue) continue;
					_defaultName = modification.Name.Value;
				}
				return _defaultName;
			}
			set => _defaultName = value;
		}
		
		public string Name { get; set; }

		private readonly string _picFile = null;
		private readonly int _overlayX;
		private readonly int _overlayY;
		private Picture _picture, _portraitSmall;

		protected abstract Leader Leader { get; }

		public Picture GetPortrait(FaceState state = FaceState.Neutral)
		{
			if (_picFile == null) return new Picture(139, 133, Common.GetPalette256);

			if (_picture == null)
			{
				_picture = Resources[_picFile];
			}

			Picture output = _picture[181, 67, 139, 133];
			switch (state)
			{
				case FaceState.Smiling:
					output.AddLayer(_picture[1, 51, 59, 49], _overlayX, _overlayY);
					break;
				case FaceState.Angry:
					output.AddLayer(_picture[1, 151, 59, 49], _overlayX, _overlayY);
					break;
				default:
					// TODO: Add other states
					break;
			}
			return output;
		}

		public Picture PortraitSmall => _portraitSmall;
		
		private AggressionLevel _aggression = AggressionLevel.Normal;
		public AggressionLevel Aggression
		{
			get
			{
				foreach (LeaderModification modification in Modifications)
				{
					if (!modification.Aggression.HasValue) continue;
					_aggression = modification.Aggression.Value;
				}
				return _aggression;
			}
			set => _aggression = value;
		}

		private DevelopmentLevel _development = DevelopmentLevel.Normal;
		public DevelopmentLevel Development
		{
			get
			{
				foreach (LeaderModification modification in Modifications)
				{
					if (!modification.Development.HasValue) continue;
					_development = modification.Development.Value;
				}
				return _development;
			}
			set => _development = value;
		}

		private MilitarismLevel _militarism = MilitarismLevel.Normal;
		public MilitarismLevel Militarism
		{
			get
			{
				foreach (LeaderModification modification in Modifications)
				{
					if (!modification.Militarism.HasValue) continue;
					_militarism = modification.Militarism.Value;
				}
				return _militarism;
			}
			set => _militarism = value;
		}
		
		private static Dictionary<Leader, List<LeaderModification>> _modifications = new Dictionary<Leader, List<LeaderModification>>();
		internal static void LoadModifications()
		{
			_modifications.Clear();

			LeaderModification[] unitModifications = Reflect.GetModifications<LeaderModification>().ToArray();
			if (unitModifications.Length == 0) return;

			Log("Applying leader modifications");

			foreach (LeaderModification modification in Reflect.GetModifications<LeaderModification>())
			{
				if (!_modifications.ContainsKey(modification.Leader))
					_modifications.Add(modification.Leader, new List<LeaderModification>());
				_modifications[modification.Leader].Add(modification);
			}

			Log("Finished applying leader modifications");
		}
		public IEnumerable<LeaderModification> Modifications => _modifications.ContainsKey(Leader) ? _modifications[Leader].ToArray() : new LeaderModification[0];

		protected BaseLeader(string name, string picFile, int overlayX, int overlayY)
		{
			DefaultName = name;
			Name = DefaultName;
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
					_portraitSmall = Resources["SLAM2"][(28 * col) + 1, 34 * row, 27, 33];
					_portraitSmall.ColourReplace(0, 191, 0, 0, 27, 33);
					return;
				}
			}
			_portraitSmall = new Picture(27, 33, Common.GetPalette256);

			Aggression = AggressionLevel.Normal;
			Development = DevelopmentLevel.Normal;
			Militarism = MilitarismLevel.Normal;
		}

		protected BaseLeader(string name)
		{
			DefaultName = name;
			Name = DefaultName;
			_portraitSmall = new Picture(27, 33, Common.GetPalette256);
		}
	}
}