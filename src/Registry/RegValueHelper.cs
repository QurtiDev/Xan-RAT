

using InvokedCommon.Models;
using InvokedCommon.Utilities;
using Microsoft.Win32;
using System;


namespace InvokedServer.Registry
{
	public class RegValueHelper
	{
		private static string DEFAULT_REG_VALUE = "(Default)";

		public static bool IsDefaultValue(string valueName) => string.IsNullOrEmpty(valueName);

		public static string GetName(string valueName)
		{
			return !RegValueHelper.IsDefaultValue(valueName) ? valueName : RegValueHelper.DEFAULT_REG_VALUE;
		}

		public static string RegistryValueToString(RegValueData value)
		{
			switch (value.Kind)
			{
				case RegistryValueKind.String:
				case RegistryValueKind.ExpandString:
					return ByteConverter.ToString(value.Data);
				case RegistryValueKind.Binary:
					return value.Data.Length == 0 ? "(zero-length binary value)" : BitConverter.ToString(value.Data).Replace("-", " ").ToLower();
				case RegistryValueKind.DWord:
					uint uint32 = ByteConverter.ToUInt32(value.Data);
					return string.Format("0x{0:x8} ({1})", (object)uint32, (object)uint32);
				case RegistryValueKind.MultiString:
					return string.Join(" ", ByteConverter.ToStringArray(value.Data));
				case RegistryValueKind.QWord:
					ulong uint64 = ByteConverter.ToUInt64(value.Data);
					return string.Format("0x{0:x8} ({1})", (object)uint64, (object)uint64);
				default:
					return string.Empty;
			}
		}
	}
}