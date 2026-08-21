

using System;
using System.Collections.Generic;
using System.Linq;
using InvokedClient.Extensions;
using InvokedCommon.Models;
using InvokedCommon.Utilities;
using Microsoft.Win32;


namespace InvokedClient.Helper
{
	public static class RegistryKeyHelper
	{
		private static string DEFAULT_VALUE = string.Empty;

		public static bool AddRegistryKeyValue(
		  RegistryHive hive,
		  string path,
		  string name,
		  string value,
		  bool addQuotes = false)
		{
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenWritableSubKeySafe(path))
				{
					if (registryKey == null)
						return false;
					if (addQuotes && !value.StartsWith("\"") && !value.EndsWith("\""))
						value = "\"" + value + "\"";
					registryKey.SetValue(name, (object)value);
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		public static RegistryKey OpenReadonlySubKey(RegistryHive hive, string path)
		{
			try
			{
				return RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(path, false);
			}
			catch
			{
				return (RegistryKey)null;
			}
		}

		public static bool DeleteRegistryKeyValue(RegistryHive hive, string path, string name)
		{
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenWritableSubKeySafe(path))
				{
					if (registryKey == null)
						return false;
					registryKey.DeleteValue(name, true);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsDefaultValue(string valueName) => string.IsNullOrEmpty(valueName);

		public static RegValueData[] AddDefaultValue(List<RegValueData> values)
		{
			if (!values.Any<RegValueData>((Func<RegValueData, bool>)(value => RegistryKeyHelper.IsDefaultValue(value.Name))))
				values.Add(RegistryKeyHelper.GetDefaultValue());
			return values.ToArray();
		}

		public static RegValueData[] GetDefaultValues()
		{
			return new RegValueData[1]
			{
				RegistryKeyHelper.GetDefaultValue()
			};
		}

		public static RegValueData CreateRegValueData(
		  string name,
		  RegistryValueKind kind,
		  object value = null)
		{
			RegValueData regValueData = new RegValueData()
			{
				Name = name,
				Kind = kind
			};
			if (value == null)
			{
				regValueData.Data = new byte[0];
			}
			else
			{
				switch (regValueData.Kind)
				{
					case RegistryValueKind.String:
					case RegistryValueKind.ExpandString:
						regValueData.Data = ByteConverter.GetBytes((string)value);
						break;
					case RegistryValueKind.Binary:
						regValueData.Data = (byte[])value;
						break;
					case RegistryValueKind.DWord:
						regValueData.Data = ByteConverter.GetBytes((uint)(int)value);
						break;
					case RegistryValueKind.MultiString:
						regValueData.Data = ByteConverter.GetBytes((string[])value);
						break;
					case RegistryValueKind.QWord:
						regValueData.Data = ByteConverter.GetBytes((ulong)(long)value);
						break;
				}
			}
			return regValueData;
		}

		private static RegValueData GetDefaultValue()
		{
			return RegistryKeyHelper.CreateRegValueData(RegistryKeyHelper.DEFAULT_VALUE, RegistryValueKind.String);
		}
	}
}
