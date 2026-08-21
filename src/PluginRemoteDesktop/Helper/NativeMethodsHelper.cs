

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugin.Helper
{
	public static class NativeMethodsHelper
	{
		private const int INPUT_MOUSE = 0;
		private const int INPUT_KEYBOARD = 1;
		private const uint MOUSEEVENTF_LEFTDOWN = 2;
		private const uint MOUSEEVENTF_LEFTUP = 4;
		private const uint MOUSEEVENTF_RIGHTDOWN = 8;
		private const uint MOUSEEVENTF_RIGHTUP = 16;
		private const uint MOUSEEVENTF_WHEEL = 2048;
		private const uint KEYEVENTF_KEYDOWN = 0;
		private const uint KEYEVENTF_KEYUP = 2;
		private const int SPI_GETSCREENSAVERRUNNING = 114;
		private const uint DESKTOP_WRITEOBJECTS = 128;
		private const uint DESKTOP_READOBJECTS = 1;
		private const int WM_CLOSE = 16;
		private const uint SPI_SETSCREENSAVEACTIVE = 17;
		private const uint SPIF_SENDWININICHANGE = 2;

		public static uint GetLastInputInfoTickCount()
		{
			NativeMethods.LASTINPUTINFO plii = new NativeMethods.LASTINPUTINFO();
			plii.cbSize = (uint) Marshal.SizeOf<NativeMethods.LASTINPUTINFO>(plii);
			plii.dwTime = 0U;
			NativeMethods.GetLastInputInfo(ref plii);
			return plii.dwTime;
		}

		public static void DoMouseLeftClick(Point p, bool isMouseDown)
		{
			NativeMethods.INPUT[] pInputs = new NativeMethods.INPUT[1]
			{
				new NativeMethods.INPUT()
				{
					type = 0U,
					u = new NativeMethods.InputUnion()
					{
						mi = new NativeMethods.MOUSEINPUT()
						{
							dx = p.X,
							dy = p.Y,
							mouseData = 0,
							dwFlags = isMouseDown ? 2U : 4U,
							time = 0U,
							dwExtraInfo = NativeMethods.GetMessageExtraInfo()
						}
					}
				}
			};
			int num = (int) NativeMethods.SendInput((uint) pInputs.Length, pInputs, Marshal.SizeOf(typeof (NativeMethods.INPUT)));
		}

		public static void DoMouseRightClick(Point p, bool isMouseDown)
		{
			NativeMethods.INPUT[] pInputs = new NativeMethods.INPUT[1]
			{
				new NativeMethods.INPUT()
				{
					type = 0U,
					u = new NativeMethods.InputUnion()
					{
						mi = new NativeMethods.MOUSEINPUT()
						{
							dx = p.X,
							dy = p.Y,
							mouseData = 0,
							dwFlags = isMouseDown ? 8U : 16U,
							time = 0U,
							dwExtraInfo = NativeMethods.GetMessageExtraInfo()
						}
					}
				}
			};
			int num = (int) NativeMethods.SendInput((uint) pInputs.Length, pInputs, Marshal.SizeOf(typeof (NativeMethods.INPUT)));
		}

		public static void DoMouseMove(Point p) => NativeMethods.SetCursorPos(p.X, p.Y);

		public static void DoMouseScroll(Point p, bool scrollDown)
		{
			NativeMethods.INPUT[] pInputs = new NativeMethods.INPUT[1]
			{
				new NativeMethods.INPUT()
				{
					type = 0U,
					u = new NativeMethods.InputUnion()
					{
						mi = new NativeMethods.MOUSEINPUT()
						{
							dx = p.X,
							dy = p.Y,
							mouseData = scrollDown ? -120 : 120,
							dwFlags = 2048U,
							time = 0U,
							dwExtraInfo = NativeMethods.GetMessageExtraInfo()
						}
					}
				}
			};
			int num = (int) NativeMethods.SendInput((uint) pInputs.Length, pInputs, Marshal.SizeOf(typeof (NativeMethods.INPUT)));
		}

		public static void DoKeyPress(byte key, bool keyDown)
		{
			NativeMethods.INPUT[] pInputs = new NativeMethods.INPUT[1]
			{
				new NativeMethods.INPUT()
				{
					type = 1U,
					u = new NativeMethods.InputUnion()
					{
						ki = new NativeMethods.KEYBDINPUT()
						{
							wVk = (ushort) key,
							wScan = (ushort) 0,
							dwFlags = keyDown ? 0U : 2U,
							dwExtraInfo = NativeMethods.GetMessageExtraInfo()
						}
					}
				}
			};
			int num = (int) NativeMethods.SendInput((uint) pInputs.Length, pInputs, Marshal.SizeOf(typeof (NativeMethods.INPUT)));
		}

		public static bool IsScreensaverActive()
		{
			IntPtr zero = IntPtr.Zero;
			NativeMethods.SystemParametersInfo(114U, 0U, ref zero, 0U);
			return zero != IntPtr.Zero;
		}

		public static void DisableScreensaver()
		{
			IntPtr hDesktop = NativeMethods.OpenDesktop("Screen-saver", 0, false, 129U);
			if (hDesktop != IntPtr.Zero)
			{
				NativeMethods.EnumDesktopWindows(hDesktop, (NativeMethods.EnumDesktopWindowsProc) ((hWnd, lParam) =>
				{
					if (NativeMethods.IsWindowVisible(hWnd))
						NativeMethods.PostMessage(hWnd, 16, IntPtr.Zero, IntPtr.Zero);
					return true;
				}), IntPtr.Zero);
				NativeMethods.CloseDesktop(hDesktop);
			}
			else
				NativeMethods.PostMessage(NativeMethods.GetForegroundWindow(), 16, IntPtr.Zero, IntPtr.Zero);
			IntPtr zero = IntPtr.Zero;
			NativeMethods.SystemParametersInfo(17U, 1U, ref zero, 2U);
		}

		public static string GetForegroundWindowTitle()
		{
			StringBuilder lpString = new StringBuilder(1024);
			NativeMethods.GetWindowText(NativeMethods.GetForegroundWindow(), lpString, lpString.Capacity);
			return lpString.ToString();
		}
	}
}
