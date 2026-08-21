

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace InvokedServer.Build
{
	internal class CrypterEncryption
	{
		public static byte[] Compress(byte[] data)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream((Stream)memoryStream, CompressionMode.Compress, true))
					gzipStream.Write(data, 0, data.Length);
				return memoryStream.ToArray();
			}
		}

		public static byte[] AESEnc(byte[] data, string key)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(key);
				aes.GenerateIV();
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cryptoStream.Write(data, 0, data.Length);
						cryptoStream.FlushFinalBlock();
					}
					return ((IEnumerable<byte>)aes.IV).Concat<byte>((IEnumerable<byte>)memoryStream.ToArray()).ToArray<byte>();
				}
			}
		}
	}
}