// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Graphics;

namespace CivOne
{
	public interface IRuntime
	{
		event EventHandler Initialize, Draw;
		event UpdateEventHandler Update;
		event KeyboardEventHandler KeyboardUp, KeyboardDown;
		event ScreenEventHandler MouseUp, MouseDown, MouseMove;
		Platform CurrentPlatform { get; }
		string StorageFolder { get; }
		RuntimeSettings Settings { get; }
		IBitmap Bitmap { get; set; }
		IBitmap Cursor { set; }
		int CanvasWidth { get; }
		int CanvasHeight { get; }
		void Log(string text, params object[] parameters);
		string BrowseFolder(string caption = "");
		void PlaySound(string file);
		void StopSound();
		void Quit();
	}
}