

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace InvokedCommon.Cryptography
{
	public class Aes256
	{
		private const int KeyLength = 32;
		private const int AuthKeyLength = 64;
		private const int IvLength = 16;
		private const int HmacSha256Length = 32;
		private readonly byte[] _key;
		private readonly byte[] _authKey;
		private static readonly byte[] Salt = new byte[32]
		{
			(byte) 175,
			(byte) 43,
			(byte) 158,
			(byte) 59,
			(byte) 247,
			(byte) 45,
			(byte) 119,
			(byte) 251,
			(byte) 242,
			(byte) 249,
			(byte) 35,
			(byte) 180,
			(byte) 160,
			(byte) 21,
			(byte) 184,
			(byte) 243,
			(byte) 32,
			(byte) 157,
			(byte) 22,
			(byte) 36,
			(byte) 162,
			(byte) 27,
			(byte) 111,
			(byte) 177,
			(byte) 196,
			(byte) 161,
			(byte) 35,
			(byte) 178,
			(byte) 166,
			(byte) 179,
			(byte) 41,
			(byte) 161
		};

		public Aes256(string masterKey)
		{
			if (string.IsNullOrEmpty(masterKey))
				throw new ArgumentException("masterKey can not be null or empty.");
			using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(masterKey, Aes256.Salt, 50000))
			{
				this._key = rfc2898DeriveBytes.GetBytes(32);
				this._authKey = rfc2898DeriveBytes.GetBytes(64);
			}
		}

		public string Encrypt(string input)
		{
			return Convert.ToBase64String(this.Encrypt(Encoding.UTF8.GetBytes(input)));
		}

		public byte[] Encrypt(byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException("input can not be null.");
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Position = 32L;
				using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
				{
					cryptoServiceProvider.KeySize = 256;
					cryptoServiceProvider.BlockSize = 128;
					cryptoServiceProvider.Mode = CipherMode.CBC;
					cryptoServiceProvider.Padding = PaddingMode.PKCS7;
					cryptoServiceProvider.Key = this._key;
					cryptoServiceProvider.GenerateIV();
					using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, cryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
					{
						memoryStream.Write(cryptoServiceProvider.IV, 0, cryptoServiceProvider.IV.Length);
						cryptoStream.Write(input, 0, input.Length);
						cryptoStream.FlushFinalBlock();
						using (HMACSHA256 hmacshA256 = new HMACSHA256(this._authKey))
						{
							byte[] hash = hmacshA256.ComputeHash(memoryStream.ToArray(), 32, memoryStream.ToArray().Length - 32);
							memoryStream.Position = 0L;
							memoryStream.Write(hash, 0, hash.Length);
						}
					}
				}
				return memoryStream.ToArray();
			}
		}

		public string Decrypt(string input)
		{
			return Encoding.UTF8.GetString(this.Decrypt(Convert.FromBase64String(input)));
		}

		public byte[] Decrypt(byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException("input can not be null.");
			using (MemoryStream memoryStream = new MemoryStream(input))
			{
				using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
				{
					cryptoServiceProvider.KeySize = 256;
					cryptoServiceProvider.BlockSize = 128;
					cryptoServiceProvider.Mode = CipherMode.CBC;
					cryptoServiceProvider.Padding = PaddingMode.PKCS7;
					cryptoServiceProvider.Key = this._key;
					using (HMACSHA256 hmacshA256 = new HMACSHA256(this._authKey))
					{
						byte[] hash = hmacshA256.ComputeHash(memoryStream.ToArray(), 32, memoryStream.ToArray().Length - 32);
						byte[] buffer = new byte[32];
						memoryStream.Read(buffer, 0, buffer.Length);
						byte[] a2 = buffer;
						if (!SafeComparison.AreEqual(hash, a2))
							throw new CryptographicException("Invalid message authentication code (MAC).");
					}
					byte[] buffer1 = new byte[16];
					memoryStream.Read(buffer1, 0, 16);
					cryptoServiceProvider.IV = buffer1;
					using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, cryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Read))
					{
						byte[] numArray = new byte[memoryStream.Length - 16L + 1L];
						byte[] dst = new byte[cryptoStream.Read(numArray, 0, numArray.Length)];
						Buffer.BlockCopy((Array) numArray, 0, (Array) dst, 0, dst.Length);
						return dst;
					}
				}
			}
		}
	}
}
