using System;

namespace CivOne
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using (Runtime runtime = new Runtime())
			using (GameWindow window = new GameWindow(runtime))
			{
				window.Run();
			}
		}
	}
}