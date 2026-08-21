

using InvokedClient.Extensions;
using InvokedClient.Helper;
using InvokedCommon.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;


namespace InvokedClient.Registry
{
	public class RegistrySeeker
	{
		private readonly List<RegSeekerMatch> _matches;

		public RegSeekerMatch[] Matches => this._matches?.ToArray();

		public RegistrySeeker() => this._matches = new List<RegSeekerMatch>();

		public void BeginSeeking(string rootKeyName)
		{
			if (!string.IsNullOrEmpty(rootKeyName))
			{
				using (RegistryKey rootKey1 = RegistrySeeker.GetRootKey(rootKeyName))
				{
					if (rootKey1 != null && rootKey1.Name != rootKeyName)
					{
						string name = rootKeyName.Substring(rootKey1.Name.Length + 1);
						using (RegistryKey rootKey2 = rootKey1.OpenReadonlySubKeySafe(name))
						{
							if (rootKey2 == null)
								return;
							this.Seek(rootKey2);
						}
					}
					else
						this.Seek(rootKey1);
				}
			}
			else
				this.Seek((RegistryKey)null);
		}

		private void Seek(RegistryKey rootKey)
		{
			if (rootKey == null)
			{
				foreach (RegistryKey rootKey1 in RegistrySeeker.GetRootKeys())
					this.ProcessKey(rootKey1, rootKey1.Name);
			}
			else
				this.Search(rootKey);
		}

		private void Search(RegistryKey rootKey)
		{
			foreach (string subKeyName in rootKey.GetSubKeyNames())
				this.ProcessKey(rootKey.OpenReadonlySubKeySafe(subKeyName), subKeyName);
		}

		private void ProcessKey(RegistryKey key, string keyName)
		{
			if (key != null)
			{
				List<RegValueData> values = new List<RegValueData>();
				foreach (string valueName in key.GetValueNames())
				{
					RegistryValueKind valueKind = key.GetValueKind(valueName);
					object obj = key.GetValue(valueName);
					values.Add(RegistryKeyHelper.CreateRegValueData(valueName, valueKind, obj));
				}
				this.AddMatch(keyName, RegistryKeyHelper.AddDefaultValue(values), key.SubKeyCount);
			}
			else
				this.AddMatch(keyName, RegistryKeyHelper.GetDefaultValues(), 0);
		}

		private void AddMatch(string key, RegValueData[] values, int subkeycount)
		{
			this._matches.Add(new RegSeekerMatch()
			{
				Key = key,
				Data = values,
				HasSubKeys = subkeycount > 0
			});
		}

		public static RegistryKey GetRootKey(string subkeyFullPath)
		{
			string[] strArray = subkeyFullPath.Split('\\');
			try
			{
				switch (strArray[0])
				{
					case "HKEY_CLASSES_ROOT":
						return RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);
					case "HKEY_CURRENT_USER":
						return RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
					case "HKEY_LOCAL_MACHINE":
						return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
					case "HKEY_USERS":
						return RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64);
					case "HKEY_CURRENT_CONFIG":
						return RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64);
					default:
						throw new Exception("Invalid rootkey, could not be found.");
				}
			}
			catch (SystemException ex)
			{
				throw new Exception("Unable to open root registry key, you do not have the needed permissions.");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static List<RegistryKey> GetRootKeys()
		{
			List<RegistryKey> rootKeys = new List<RegistryKey>();
			try
			{
				rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64));
				rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64));
				rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64));
				rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64));
				rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64));
			}
			catch (SystemException ex)
			{
				throw new Exception("Could not open root registry keys, you may not have the needed permission");
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return rootKeys;
		}
	}
}