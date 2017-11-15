// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.


using System;
using System.IO;

namespace CivOne.GameSave
{
    public class SaveGameFile : BaseInstance
    {
        public bool ValidFile { get; private set; }
        public string SveFile { get; private set; }
        public string MapFile { get; private set; }
        public int Difficulty { get; private set; }

        public string Name { get; private set; }

        private ushort ReadUShort(BinaryReader reader, int position)
        {
            return Common.BinaryReadUShort(reader, position);
        }

        private string[] ReadStrings(BinaryReader reader, int position, int length, int itemLength)
        {
            return Common.BinaryReadStrings(reader, position, length, itemLength);
        }

        public SaveGameFile(string filename)
        {
            ValidFile = false;
            Name = "(EMPTY)";
            SveFile = string.Format("{0}.SVE", filename);
            MapFile = string.Format("{0}.MAP", filename);
            if (!File.Exists(SveFile) || !File.Exists(MapFile)) return;

            try
            {
                using (FileStream fs = new FileStream(SveFile, FileMode.Open))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    if (fs.Length != 37856)
                    {
                        Name = "(INCORRECT FILE SIZE)";
                        return;
                    }

                    string turn = Common.YearString(ReadUShort(br, 0));
                    ushort humanPlayer = ReadUShort(br, 2);
                    ushort difficultyLevel = ReadUShort(br, 10);
                    string leaderName = ReadStrings(br, 16, 112, 14)[humanPlayer];
                    string civName = ReadStrings(br, 128, 96, 12)[humanPlayer];
                    string tribeName = ReadStrings(br, 224, 88, 11)[humanPlayer];
                    string title = Common.DifficultyName(difficultyLevel);

                    Name = string.Format("{0} {1}, {2}/{3}", title, leaderName, civName, turn);
                    Difficulty = (int)difficultyLevel;
                }
                ValidFile = true;
            }
            catch (Exception ex)
            {
                Log($"Could not open .SVE file: {ex.InnerException}");
                Name = "(COULD NOT READ SAVE FILE HEADER)";
            }
        }
    }
}