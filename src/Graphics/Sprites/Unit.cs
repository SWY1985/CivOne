// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using CivOne.Enums;
using CivOne.IO;
using CivOne.Units;

namespace CivOne.Graphics.Sprites
{
	public static class Unit
	{
		private static Free Free => Free.Instance;
		private static Resources Resources => Resources.Instance;
		private static Settings Settings => Settings.Instance;

		private static bool GFX256 => (Settings.GraphicsMode == GraphicsMode.Graphics256);

		private static Bytemap GetUnit((UnitType Type, byte PlayerNumber) unit)
		{
			byte colourDark = Common.ColourDark[unit.PlayerNumber];
			byte colourLight = Common.ColourLight[unit.PlayerNumber];
			int unitId = (int)unit.Type;

			string resFile = GFX256 ? "SP257" : "SPRITES";
			int xx = (unitId % 20) * 16;
			int yy = unitId < 20 ? 160 : 176;

			Bytemap output;
			if (Resources.Exists(resFile))
			{
				output = Resources[resFile].Bitmap[xx, yy, 16, 16]
					.FillRectangle(0, 0, 16, 1, 0)
					.FillRectangle(0, 1, 1, 15, 0);
			}
			else
			{
				output = Free.GetUnit(unit.Type);
			}
			
			if (colourLight == 15) output.ColourReplace((15, 11), (10, colourLight), (2, colourDark));
			else if (colourDark == 8) output.ColourReplace((7, 3), (10, colourLight), (2, colourDark));
			else output.ColourReplace((10, colourLight), (2, colourDark));
			
			return output;
		}

		private static Bytemap GetUnitSentry((UnitType Type, byte PlayerNumber) unit)
		{
			return new Picture(16, 16)
				.AddLayer(BaseUnit[(unit.Type, unit.PlayerNumber)].Bitmap)
				.ColourReplace((5, 7), (8, 7))
				.Bitmap;
		}

		private static Bytemap GetUnitFortify((UnitType Type, byte PlayerNumber) unit)
		{
			return new Picture(16, 16)
				.AddLayer(BaseUnit[(unit.Type, unit.PlayerNumber)].Bitmap)
				.AddLayer(Generic.Fortify)
				.Bitmap;
		}

		private static Bytemap GetUnitLetter((UnitType Type, char Letter, byte PlayerNumber) unit)
		{
			return new Picture(16, 16)
				.AddLayer(BaseUnit[(unit.Type, unit.PlayerNumber)].Bitmap)
				.DrawText($"{unit.Letter}", 8, 8, TextSettings.UnitText(unit.PlayerNumber))
				.Bitmap;
		}

		private static ISpriteCollection<(UnitType, byte)> BaseUnit = new CachedSpriteCollection<(UnitType, byte)>(GetUnit);
		private static ISpriteCollection<(UnitType, byte)> SentryUnit = new CachedSpriteCollection<(UnitType, byte)>(GetUnitSentry);
		private static ISpriteCollection<(UnitType, byte)> FortifyUnit = new CachedSpriteCollection<(UnitType, byte)>(GetUnitFortify);
		private static ISpriteCollection<(UnitType, char, byte)> LetterUnit = new CachedSpriteCollection<(UnitType, char, byte)>(GetUnitLetter);
		public static ISprite Base<T>(byte playerNumber) where T : IUnit, new() => BaseUnit[(new T().Type, playerNumber)];
		public static ISprite Base(UnitType type, byte playerNumber) => BaseUnit[(type, playerNumber)];
		public static ISprite Sentry<T>(byte playerNumber) where T : IUnit, new() => SentryUnit[(new T().Type, playerNumber)];
		public static ISprite Sentry(UnitType type, byte playerNumber) => SentryUnit[(type, playerNumber)];
		public static ISprite Fortify<T>(byte playerNumber) where T : IUnit, new() => FortifyUnit[(new T().Type, playerNumber)];
		public static ISprite Fortify(UnitType type, byte playerNumber) => FortifyUnit[(type, playerNumber)];
		public static ISprite Letter<T>(char letter, byte playerNumber) where T : IUnit, new() => LetterUnit[(new T().Type, letter, playerNumber)];
		public static ISprite Letter(UnitType type, char letter, byte playerNumber) => LetterUnit[(type, letter, playerNumber)];
		
		public static ISprite Settlers(byte playerNumber) => BaseUnit[(UnitType.Settlers, playerNumber)];
		public static ISprite Militia(byte playerNumber) => BaseUnit[(UnitType.Militia, playerNumber)];
		public static ISprite Phalanx(byte playerNumber) => BaseUnit[(UnitType.Phalanx, playerNumber)];
		public static ISprite Legion(byte playerNumber) => BaseUnit[(UnitType.Legion, playerNumber)];
		public static ISprite Musketeers(byte playerNumber) => BaseUnit[(UnitType.Musketeers, playerNumber)];
		public static ISprite Riflemen(byte playerNumber) => BaseUnit[(UnitType.Riflemen, playerNumber)];
		public static ISprite Cavalry(byte playerNumber) => BaseUnit[(UnitType.Cavalry, playerNumber)];
		public static ISprite Knights(byte playerNumber) => BaseUnit[(UnitType.Knights, playerNumber)];
		public static ISprite Catapult(byte playerNumber) => BaseUnit[(UnitType.Catapult, playerNumber)];
		public static ISprite Cannon(byte playerNumber) => BaseUnit[(UnitType.Cannon, playerNumber)];
		public static ISprite Chariot(byte playerNumber) => BaseUnit[(UnitType.Chariot, playerNumber)];
		public static ISprite Armor(byte playerNumber) => BaseUnit[(UnitType.Armor, playerNumber)];
		public static ISprite MechInf(byte playerNumber) => BaseUnit[(UnitType.MechInf, playerNumber)];
		public static ISprite Artillery(byte playerNumber) => BaseUnit[(UnitType.Artillery, playerNumber)];
		public static ISprite Fighter(byte playerNumber) => BaseUnit[(UnitType.Fighter, playerNumber)];
		public static ISprite Bomber(byte playerNumber) => BaseUnit[(UnitType.Bomber, playerNumber)];
		public static ISprite Trireme(byte playerNumber) => BaseUnit[(UnitType.Trireme, playerNumber)];
		public static ISprite Sail(byte playerNumber) => BaseUnit[(UnitType.Sail, playerNumber)];
		public static ISprite Frigate(byte playerNumber) => BaseUnit[(UnitType.Frigate, playerNumber)];
		public static ISprite Ironclad(byte playerNumber) => BaseUnit[(UnitType.Ironclad, playerNumber)];
		public static ISprite Cruiser(byte playerNumber) => BaseUnit[(UnitType.Cruiser, playerNumber)];
		public static ISprite Battleship(byte playerNumber) => BaseUnit[(UnitType.Battleship, playerNumber)];
		public static ISprite Submarine(byte playerNumber) => BaseUnit[(UnitType.Submarine, playerNumber)];
		public static ISprite Carrier(byte playerNumber) => BaseUnit[(UnitType.Carrier, playerNumber)];
		public static ISprite Transport(byte playerNumber) => BaseUnit[(UnitType.Transport, playerNumber)];
		public static ISprite Nuclear(byte playerNumber) => BaseUnit[(UnitType.Nuclear, playerNumber)];
		public static ISprite Diplomat(byte playerNumber) => BaseUnit[(UnitType.Diplomat, playerNumber)];
		public static ISprite Caravan(byte playerNumber) => BaseUnit[(UnitType.Caravan, playerNumber)];
	}
}