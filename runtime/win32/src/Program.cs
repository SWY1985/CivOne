// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Windows.Forms;

namespace CivOne
{
	internal class Program
	{
		private static void Main(params string[] args)
		{
			using (Runtime runtime = new Runtime())
			using (Window window = new Window(runtime))
			{
				RuntimeHandler.Register(runtime);
				Application.Run(window);
			}
		}
	}
}