// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Threading;
using CivOne.Interfaces;
using CivOne.Tiles;

namespace CivOne
{
	internal class Map
	{
		public const int WIDTH = 80;
		public const int HEIGHT = 50;
		
		private int _landMass, _temperature, _climate, _age;
		private ITile[,] _tiles;
		
		public bool Ready { get; private set; }
		
		private void GenerateThread()
		{
			Console.WriteLine("Generating map (Land Mass: {0}, Temperature: {1}, Climate: {2}, Age: {3})", _landMass, _temperature, _climate, _age);
			
			Ready = true;
			Console.WriteLine("DEBUG: Map generating not yet implemented...");
		}
		
		private void LoadThread()
		{
			Console.WriteLine("Loading MAP.PIC");
			
			Ready = true;
			Console.WriteLine("DEBUG: Map loading not yet implemented...");
		}
		
		public void Generate(int landMass = 1, int temperature = 1, int climate = 1, int age = 1)
		{
			if (Ready || _tiles != null)
			{
				Console.WriteLine("ERROR: Map is already generat{0}", (Ready ? "ed" : "ing"));
				return;
			}
			
			_landMass = landMass;
			_temperature = temperature;
			_climate = climate;
			_age = age;
			
			new Thread(new ThreadStart(GenerateThread)).Start();
		}
		
		public void LoadMap()
		{
			if (Ready || _tiles != null)
			{
				Console.WriteLine("ERROR: Map is already generat{0}", (Ready ? "ed" : "ing"));
				return;
			}
			
			_landMass = -1;
			_temperature = -1;
			_climate = -1;
			_age = -1;
			
			new Thread(new ThreadStart(LoadThread)).Start();
		}
		
		private static Map _instance;
		public static Map Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Map();
				return _instance;
			}
		}
		
		private Map()
		{
			Ready = false;
			
			Console.WriteLine("Map instance created");
		}
	}
}