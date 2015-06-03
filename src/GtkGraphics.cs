// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

#if GTK
using Gtk;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CivOne
{
	internal class GtkGraphics : DrawingArea
	{
		public event PaintEventHandler Paint;
		
		public void Refresh()
		{
			Gdk.Threads.Enter();
			QueueDraw();
			Gdk.Threads.Leave();
		}
		
		protected override bool OnExposeEvent(Gdk.EventExpose args)
		{
			using (Graphics g = Gtk.DotNet.Graphics.FromDrawable(args.Window))
			{
				if (Paint == null) return false;
				Paint(this, new PaintEventArgs(g, new Rectangle(0, 0, 320, 200)));
			}
			return true;
		}
		
		internal GtkGraphics()
		{
			
		}
	}
}
#endif