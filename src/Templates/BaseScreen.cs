// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CivOne.Enums;
using CivOne.Interfaces;
using CivOne.GFX;

namespace CivOne.Templates
{
	internal abstract class BaseScreen : IScreen
	{
		protected readonly List<Screens.Menu> Menus = new List<Screens.Menu>();
		protected Picture _canvas = new Picture(320, 200);
		
		protected void AddLayer(IScreen screen, Point point)
		{
			AddLayer(screen, point.X, point.Y);
		}
		protected void AddLayer(IScreen screen, int x = 0, int y = 0)
		{
			_canvas.AddLayer(screen.Canvas.Image, x, y);
		}
		protected void AddLayer(Picture picture, Point point)
		{
			AddLayer(picture, point.X, point.Y);
		}
		protected void AddLayer(Picture picture, int x = 0, int y = 0)
		{
			_canvas.AddLayer(picture.Image, x, y);
		}
		protected void AddLayer(Bitmap bitmap, Point point)
		{
			AddLayer(bitmap, point.X, point.Y);
		}
		protected void AddLayer(Bitmap bitmap, int x = 0, int y = 0)
		{
			_canvas.AddLayer(bitmap, x, y);
		}
		
		protected void MouseArgsOffset(ref MouseEventArgs args, int offsetX, int offsetY)
		{
			args = new MouseEventArgs(args.Button, args.Clicks, args.X - offsetX, args.Y - offsetY, args.Delta);
		}
		
		public virtual Picture Canvas
		{
			get
			{
				return _canvas;
			}
		}
		public virtual MouseCursor Cursor { get; protected set; }
		public abstract bool HasUpdate(uint gameTick);
		public virtual bool KeyDown(KeyEventArgs args)
		{
			return false;
		}
		public virtual bool MouseDown(MouseEventArgs args)
		{
			return false;
		}
		public virtual bool MouseUp(MouseEventArgs args)
		{
			return false;
		}
		public virtual bool MouseDrag(MouseEventArgs args)
		{
			return false;
		}
		
		protected void CloseMenus()
		{
			System.Console.WriteLine("Close menus");
			foreach (Screens.Menu menu in Menus)
			{
				menu.Close();
			}
			Menus.Clear();
		}
		protected void Destroy()
		{
			CloseMenus();
			Common.DestroyScreen(this);
		}
	}
}