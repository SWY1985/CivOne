// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
using CivOne.GFX;
using CivOne.Interfaces;
using CivOne.Screens;

namespace CivOne
{
	internal class View : NSView
	{
		public event MouseEventHandler OnMouseDown, OnMouseUp;
		
		private int Height
		{
			get
			{
				return Convert.ToInt32(Window.ContentRectFor(Window.Frame).Height);
			}
		}
		
		private int Width
		{
			get
			{
				return Convert.ToInt32(Window.ContentRectFor(Window.Frame).Width);
			}
		}
		
		private static CGImage ConvertImage(Image input)
		{
			byte[] byteMap;
			using (MemoryStream ms = new MemoryStream())
			{
				input.Save(ms, ImageFormat.Png);
				byteMap = ms.ToArray();
			}
			
			if (byteMap == null)
				return null;
			
			CGDataProvider dataProvider = new CGDataProvider(byteMap, 0, byteMap.Length);
			return CGImage.FromPNG(dataProvider, null, false, CGColorRenderingIntent.Default);
		}
		
		public override void DrawRect(RectangleF area)
		{
			if (Common.Screens.Length == 0) return;
			
			Color[] colours = Common.Screens.LastOrDefault().Canvas.Image.Palette.Entries;
			colours[0] = Color.Black;
			
			Picture _canvas = new Picture(320, 200, colours);
			foreach (IScreen screen in Common.Screens)
			{
				_canvas.AddLayer(screen.Canvas.Image, 0, 0);
			}
			
			CGImage image = ConvertImage(_canvas.Image);
			NSGraphicsContext.CurrentContext.GraphicsPort.DrawImage(new RectangleF(0, 0, 640, 400), image);
		}
		
		private MouseEventArgs MouseEvent(NSEvent theEvent)
		{
			MouseButtons buttons;
			switch (theEvent.ButtonNumber)
			{
				case 0: buttons = MouseButtons.Left; break;
				case 1: buttons = MouseButtons.Right; break;
				default: buttons = MouseButtons.None; break;
			}
			
			return new MouseEventArgs(buttons, 1, (int)theEvent.LocationInWindow.X, Height - (int)theEvent.LocationInWindow.Y - 1, 0);
		}

		public override void MouseDown(NSEvent theEvent)
		{
			if (OnMouseDown == null) return;
			OnMouseDown.Invoke(this, MouseEvent(theEvent));
		}

		public override void MouseUp(NSEvent theEvent)
		{
			if (OnMouseUp == null) return;
			OnMouseUp.Invoke(this, MouseEvent(theEvent));
		}

		public override void RightMouseDown(NSEvent theEvent)
		{
			MouseDown(theEvent);
		}

		public override void RightMouseUp(NSEvent theEvent)
		{
			MouseUp(theEvent);
		}
	}
}