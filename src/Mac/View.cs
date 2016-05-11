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
			CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
			context.SetStrokeColor(new CGColor(1.0F, 0.0F, 0.0F));
			context.SetLineWidth(1.0F);
			context.StrokeEllipseInRect(new RectangleF(5, 5, 10, 10));
			
			//
			
			if (Common.Screens.Length == 0) return;
			
			Color[] colours = Common.Screens.LastOrDefault().Canvas.Image.Palette.Entries;
			colours[0] = Color.Black;
			
			Picture _canvas = new Picture(320, 200, colours);
			foreach (IScreen screen in Common.Screens)
			{
				_canvas.AddLayer(screen.Canvas.Image, 0, 0);
			}
			
			CGImage image = ConvertImage(_canvas.Image);
			context.DrawImage(new RectangleF(0, 0, 640, 400), image);
			//
		}
	}
}