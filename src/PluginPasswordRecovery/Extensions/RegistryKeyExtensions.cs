

using InvokedCommon.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;


namespace InvokedClient.Extensions
{
	public static class RegistryKeyExtensions
	{
		private static bool IsNameOrValueNull(this string keyName, RegistryKey key)
		{
			return string.IsNullOrEmpty(keyName) || key == null;
		}

		public static string GetValueSafe(this RegistryKey key, string keyName, string defaultValue = "")
		{
			try
			{
				return key.GetValue(keyName, (object)defaultValue).ToString();
			}
			catch
			{
				return defaultValue;
			}
		}

		public static RegistryKey OpenReadonlySubKeySafe(this RegistryKey key, string name)
		{
			try
			{
				return key.OpenSubKey(name, false);
			}
			catch
			{
				return (RegistryKey)null;
			}
		}

		public static RegistryKey OpenWritableSubKeySafe(this RegistryKey key, string name)
		{
			try
			{
				return key.OpenSubKey(name, true);
			}
			catch
			{
				return (RegistryKey)null;
			}
		}

		public static RegistryKey CreateSubKeySafe(this RegistryKey key, string name)
		{
			try
			{
				return key.CreateSubKey(name);
			}
			catch
			{
				return (RegistryKey)null;
			}
		}

		public static bool DeleteSubKeyTreeSafe(this RegistryKey key, string name)
		{
			try
			{
				key.DeleteSubKeyTree(name, true);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool RenameSubKeySafe(this RegistryKey key, string oldName, string newName)
		{
			try
			{
				key.CopyKey(oldName, newName);
				key.DeleteSubKeyTree(oldName);
				return true;
			}
			catch
			{
				key.DeleteSubKeyTreeSafe(newName);
				return false;
			}
		}

		public static void CopyKey(this RegistryKey key, string oldName, string newName)
		{
			using (RegistryKey subKey = key.CreateSubKey(newName))
			{
				using (RegistryKey sourceKey = key.OpenSubKey(oldName, true))
					RegistryKeyExtensions.RecursiveCopyKey(sourceKey, subKey);
			}
		}

		private static void RecursiveCopyKey(RegistryKey sourceKey, RegistryKey destKey)
		{
			foreach (string valueName in sourceKey.GetValueNames())
			{
				object obj = sourceKey.GetValue(valueName);
				RegistryValueKind valueKind = sourceKey.GetValueKind(valueName);
				destKey.SetValue(valueName, obj, valueKind);
			}
			foreach (string subKeyName in sourceKey.GetSubKeyNames())
			{
				using (RegistryKey sourceKey1 = sourceKey.OpenSubKey(subKeyName))
				{
					using (RegistryKey subKey = destKey.CreateSubKey(subKeyName))
						RegistryKeyExtensions.RecursiveCopyKey(sourceKey1, subKey);
				}
			}
		}

		public static bool SetValueSafe(
		  this RegistryKey key,
		  string name,
		  object data,
		  RegistryValueKind kind)
		{
			try
			{
				if (kind != RegistryValueKind.Binary && data.GetType() == typeof(byte[]))
				{
					switch (kind)
					{
						case RegistryValueKind.String:
						case RegistryValueKind.ExpandString:
							data = (object)ByteConverter.ToString((byte[])data);
							break;
						case RegistryValueKind.DWord:
							data = (object)ByteConverter.ToUInt32((byte[])data);
							break;
						case RegistryValueKind.MultiString:
							data = (object)ByteConverter.ToStringArray((byte[])data);
							break;
						case RegistryValueKind.QWord:
							data = (object)ByteConverter.ToUInt64((byte[])data);
							break;
					}
				}
				key.SetValue(name, data, kind);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool DeleteValueSafe(this RegistryKey key, string name)
		{
			try
			{
				key.DeleteValue(name);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool RenameValueSafe(this RegistryKey key, string oldName, string newName)
		{
			try
			{
				key.CopyValue(oldName, newName);
				key.DeleteValue(oldName);
				return true;
			}
			catch
			{
				key.DeleteValueSafe(newName);
				return false;
			}
		}

		public static void CopyValue(this RegistryKey key, string oldName, string newName)
		{
			RegistryValueKind valueKind = key.GetValueKind(oldName);
			object obj = key.GetValue(oldName);
			key.SetValue(newName, obj, valueKind);
		}

		public static bool ContainsSubKey(this RegistryKey key, string name)
		{
			foreach (string subKeyName in key.GetSubKeyNames())
			{
				if (subKeyName == name)
					return true;
			}
			return false;
		}

		public static bool ContainsValue(this RegistryKey key, string name)
		{
			foreach (string valueName in key.GetValueNames())
			{
				if (valueName == name)
					return true;
			}
			return false;
		}

		public static IEnumerable<Tuple<string, string>> GetKeyValues(this RegistryKey key)
		{
			if (key != null)
			{
				foreach (string keyName in ((IEnumerable<string>)key.GetValueNames()).Where<string>((Func<string, bool>)(keyVal => !keyVal.IsNameOrValueNull(key))).Where<string>((Func<string, bool>)(k => !string.IsNullOrEmpty(k))))
					yield return new Tuple<string, string>(keyName, key.GetValueSafe(keyName));
			}
		}

		public static object GetDefault(this RegistryValueKind valueKind)
		{
			switch (valueKind)
			{
				case RegistryValueKind.String:
				case RegistryValueKind.ExpandString:
					return (object)"";
				case RegistryValueKind.Binary:
					return (object)new byte[0];
				case RegistryValueKind.DWord:
					return (object)0;
				case RegistryValueKind.MultiString:
					return (object)new string[0];
				case RegistryValueKind.QWord:
					return (object)0L;
				default:
					return (object)null;
			}
		}
	}
}