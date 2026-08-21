

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Plugin.Helper
{
	public static class RemoteDesktopHelper
	{
		public const int SRCCOPY = 13369376;
		private const int CURSOR_SHOWING = 1;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetThreadDesktop(IntPtr hDesktop);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr OpenDesktop(
			string lpszDesktop,
			int dwFlags,
			bool fInherit,
			uint dwDesiredAccess);

		[DllImport("user32.dll")]
		private static extern bool GetCursorInfo(out RemoteDesktopHelper.CURSORINFO pci);

		[DllImport("user32.dll")]
		private static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

		public static Rectangle GetBounds(int screenNumber) => Screen.AllScreens[screenNumber].Bounds;

		public static Bitmap CaptureScreen(int Scrn, bool captureCursor)
		{
			Rectangle bounds = Screen.AllScreens[Scrn].Bounds;
			try
			{
				Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
				using (Graphics graphics = Graphics.FromImage((Image) bitmap))
				{
					graphics.PageUnit = GraphicsUnit.Pixel;
					graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, new Size(bitmap.Width, bitmap.Height), CopyPixelOperation.SourceCopy);
					if (captureCursor)
					{
						RemoteDesktopHelper.CURSORINFO pci;
						pci.cbSize = Marshal.SizeOf(typeof (RemoteDesktopHelper.CURSORINFO));
						if (RemoteDesktopHelper.GetCursorInfo(out pci) && pci.flags == 1)
						{
							RemoteDesktopHelper.GetCursorInfo(out pci);
							RemoteDesktopHelper.DrawIcon(graphics.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
							graphics.ReleaseHdc();
						}
					}
					return bitmap;
				}
			}
			catch
			{
				return new Bitmap(bounds.Width, bounds.Height);
			}
		}

		public enum DESKTOP_ACCESS : uint
		{
			DESKTOP_NONE = 0,
			DESKTOP_READOBJECTS = 1,
			DESKTOP_CREATEWINDOW = 2,
			DESKTOP_CREATEMENU = 4,
			DESKTOP_HOOKCONTROL = 8,
			DESKTOP_JOURNALRECORD = 16, // 0x00000010
			DESKTOP_JOURNALPLAYBACK = 32, // 0x00000020
			DESKTOP_ENUMERATE = 64, // 0x00000040
			DESKTOP_WRITEOBJECTS = 128, // 0x00000080
			DESKTOP_SWITCHDESKTOP = 256, // 0x00000100
			GENERIC_ALL = 511, // 0x000001FF
		}

		private struct CURSORINFO
		{
			public int cbSize;
			public int flags;
			public IntPtr hCursor;
			public RemoteDesktopHelper.POINTAPI ptScreenPos;
		}

		private struct POINTAPI
		{
			public int x;
			public int y;
		}
	}
}
