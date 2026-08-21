

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace InvokedClient.Extensions
{
	public static class KeyExtensions
	{
		public static bool ContainsModifierKeys(this List<Keys> pressedKeys)
		{
			return pressedKeys.Any<Keys>((Func<Keys, bool>)(x => x.IsModifierKey()));
		}

		public static bool IsModifierKey(this Keys key)
		{
			return key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.LMenu || key == Keys.RMenu || key == Keys.LWin || key == Keys.RWin || key == Keys.Control || key == Keys.Alt;
		}

		public static bool ContainsKeyChar(this List<Keys> pressedKeys, char c)
		{
			return pressedKeys.Contains((Keys)char.ToUpper(c));
		}

		public static bool IsExcludedKey(this Keys k)
		{
			return k >= Keys.A && k <= Keys.Z || k >= Keys.NumPad0 && k <= Keys.Divide || k >= Keys.D0 && k <= Keys.D9 || k >= Keys.OemSemicolon && k <= Keys.OemClear || k >= Keys.LShiftKey && k <= Keys.RShiftKey || k == Keys.Capital || k == Keys.Space;
		}

		public static string GetDisplayName(this Keys key)
		{
			string str = key.ToString();
			if (str.Contains("ControlKey"))
				return "Control";
			if (str.Contains("Menu"))
				return "Alt";
			if (str.Contains("Win"))
				return "Win";
			return str.Contains("Shift") ? "Shift" : str;
		}
	}
}