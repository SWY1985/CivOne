/* CivOne
 *
 * To the extent possible under law, the person who associated CC0 with
 * CivOne has waived all copyright and related or neighboring rights
 * to CivOne.
 *
 * You should have received a copy of the CC0 legalcode along with this
 * work.  If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace CivOne
{
	internal class Window : Form
	{
		private void OnPaint(object sender, PaintEventArgs args)
		{
			args.Graphics.Clear(Color.Black);
		}
		
		public Window()
		{
			SuspendLayout();
			
			// Set Window properties
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			ClientSize = new Size(320 * 2, 200 * 2);
			Text = "CivOne";
			
			// Set Window events
			Paint += OnPaint;
			
			ResumeLayout(false);
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}