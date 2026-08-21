

using Microsoft.Win32;
using System;


namespace InvokedServer.Extensions
{
	public static class RegistryKeyExtensions
	{
		public static string RegistryTypeToString(this RegistryValueKind valueKind, object valueData)
		{
			if (valueData == null)
				return "(value not set)";
			switch (valueKind)
			{
				case RegistryValueKind.String:
				case RegistryValueKind.ExpandString:
					return valueData.ToString();
				case RegistryValueKind.Binary:
					return ((byte[])valueData).Length == 0 ? "(zero-length binary value)" : BitConverter.ToString((byte[])valueData).Replace("-", " ").ToLower();
				case RegistryValueKind.DWord:
					return string.Format("0x{0} ({1})", (object)((uint)(int)valueData).ToString("x8"), (object)((uint)(int)valueData).ToString());
				case RegistryValueKind.MultiString:
					return string.Join(" ", (string[])valueData);
				case RegistryValueKind.QWord:
					return string.Format("0x{0} ({1})", (object)((ulong)(long)valueData).ToString("x8"), (object)((ulong)(long)valueData).ToString());
				default:
					return string.Empty;
			}
		}

		public static string RegistryTypeToString(this RegistryValueKind valueKind)
		{
			switch (valueKind)
			{
				case RegistryValueKind.Unknown:
					return "(Unknown)";
				case RegistryValueKind.String:
					return "REG_SZ";
				case RegistryValueKind.ExpandString:
					return "REG_EXPAND_SZ";
				case RegistryValueKind.Binary:
					return "REG_BINARY";
				case RegistryValueKind.DWord:
					return "REG_DWORD";
				case RegistryValueKind.MultiString:
					return "REG_MULTI_SZ";
				case RegistryValueKind.QWord:
					return "REG_QWORD";
				default:
					return "REG_NONE";
			}
		}
	}
}