

using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugin.Helper.Stealer
{
	public static class AesGcm
	{
		private static uint SUCCESS = 0;
		private static uint BCRYPT_KEY_DATA_BLOB_MAGIC = 1296188491;
		private static string BCRYPT_OBJECT_LENGTH = "ObjectLength";
		private static string BCRYPT_CHAIN_MODE_GCM = "ChainingModeGCM";
		private static string BCRYPT_AUTH_TAG_LENGTH = "AuthTagLength";
		private static string BCRYPT_CHAINING_MODE = "ChainingMode";
		private static string BCRYPT_AES_ALGORITHM = "AES";
		private static string MS_PRIMITIVE_PROVIDER = "Microsoft Primitive Provider";

		public static byte[] Decrypt(
			byte[] key,
			byte[] iv,
			byte[] aad,
			byte[] cipherText,
			byte[] authTag)
		{
			IntPtr hAlg;
			if (!AesGcm.OpenAlgorithmProvider(AesGcm.BCRYPT_AES_ALGORITHM, AesGcm.MS_PRIMITIVE_PROVIDER, AesGcm.BCRYPT_CHAIN_MODE_GCM, out hAlg))
				return (byte[]) null;
			IntPtr hKey;
			IntPtr keyDataBuffer;
			if (!AesGcm.ImportKey(hAlg, key, out hKey, out keyDataBuffer))
			{
				int num = (int) BCryptNativeMethods.BCryptCloseAlgorithmProvider(hAlg, 0U);
				return (byte[]) null;
			}
			uint MaxAuthTagSize;
			if (!AesGcm.GetMaxAuthTagSize(hAlg, out MaxAuthTagSize))
			{
				int num1 = (int) BCryptNativeMethods.BCryptDestroyKey(hKey);
				Marshal.FreeHGlobal(keyDataBuffer);
				int num2 = (int) BCryptNativeMethods.BCryptCloseAlgorithmProvider(hAlg, 0U);
				return (byte[]) null;
			}
			BCryptInternalStructs.BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO pPaddingInfo = new BCryptInternalStructs.BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO(iv, aad, authTag);
			byte[] pbIV = new byte[(int) MaxAuthTagSize];
			int pcbResult = 0;
			if ((int) BCryptNativeMethods.BCryptDecrypt(hKey, cipherText, (uint) cipherText.Length, ref pPaddingInfo, pbIV, (uint) pbIV.Length, (byte[]) null, 0U, ref pcbResult, 0U) != (int) AesGcm.SUCCESS)
			{
				int num3 = (int) BCryptNativeMethods.BCryptDestroyKey(hKey);
				Marshal.FreeHGlobal(keyDataBuffer);
				int num4 = (int) BCryptNativeMethods.BCryptCloseAlgorithmProvider(hAlg, 0U);
				pPaddingInfo.Dispose();
				return (byte[]) null;
			}
			byte[] pbOutput = new byte[pcbResult];
			int num5 = (int) BCryptNativeMethods.BCryptDecrypt(hKey, cipherText, (uint) cipherText.Length, ref pPaddingInfo, pbIV, (uint) pbIV.Length, pbOutput, (uint) pbOutput.Length, ref pcbResult, 0U);
			int num6 = (int) BCryptNativeMethods.BCryptDestroyKey(hKey);
			Marshal.FreeHGlobal(keyDataBuffer);
			int num7 = (int) BCryptNativeMethods.BCryptCloseAlgorithmProvider(hAlg, 0U);
			pPaddingInfo.Dispose();
			int success = (int) AesGcm.SUCCESS;
			return num5 != success ? (byte[]) null : pbOutput;
		}

		private static bool GetMaxAuthTagSize(IntPtr hAlg, out uint MaxAuthTagSize)
		{
			BCryptInternalStructs.BCRYPT_KEY_LENGTHS_STRUCT outData;
			if (AesGcm.GetProperty<BCryptInternalStructs.BCRYPT_KEY_LENGTHS_STRUCT>(hAlg, AesGcm.BCRYPT_AUTH_TAG_LENGTH, out outData))
			{
				MaxAuthTagSize = outData.dwMaxLength;
				return true;
			}
			MaxAuthTagSize = 0U;
			return false;
		}

		private static bool OpenAlgorithmProvider(
			string alg,
			string provider,
			string chainingMode,
			out IntPtr hAlg)
		{
			if ((int) BCryptNativeMethods.BCryptOpenAlgorithmProvider(out hAlg, alg, provider, 0U) != (int) AesGcm.SUCCESS)
				return false;
			byte[] bytes = Encoding.Unicode.GetBytes(chainingMode);
			if ((int) BCryptNativeMethods.BCryptSetProperty(hAlg, AesGcm.BCRYPT_CHAINING_MODE, bytes, (uint) bytes.Length, 0U) == (int) AesGcm.SUCCESS)
				return true;
			int num = (int) BCryptNativeMethods.BCryptCloseAlgorithmProvider(hAlg, 0U);
			return false;
		}

		private static bool ImportKey(
			IntPtr hAlg,
			byte[] key,
			out IntPtr hKey,
			out IntPtr keyDataBuffer)
		{
			hKey = IntPtr.Zero;
			keyDataBuffer = IntPtr.Zero;
			InternalStructs.UINTRESULT outData;
			if (!AesGcm.GetProperty<InternalStructs.UINTRESULT>(hAlg, AesGcm.BCRYPT_OBJECT_LENGTH, out outData))
				return false;
			uint num1 = outData.Value;
			keyDataBuffer = Marshal.AllocHGlobal((int) num1);
			BCryptInternalStructs.BCRYPT_KEY_DATA_BLOB_HEADER structure = new BCryptInternalStructs.BCRYPT_KEY_DATA_BLOB_HEADER();
			structure.dwMagic = AesGcm.BCRYPT_KEY_DATA_BLOB_MAGIC;
			structure.dwVersion = 1U;
			structure.cbKeyData = (uint) key.Length;
			uint num2 = (uint) (Marshal.SizeOf<BCryptInternalStructs.BCRYPT_KEY_DATA_BLOB_HEADER>(structure) + key.Length);
			IntPtr num3 = Marshal.AllocHGlobal((int) num2);
			Marshal.StructureToPtr<BCryptInternalStructs.BCRYPT_KEY_DATA_BLOB_HEADER>(structure, num3, false);
			Marshal.Copy(key, 0, num3 + Marshal.SizeOf<BCryptInternalStructs.BCRYPT_KEY_DATA_BLOB_HEADER>(structure), key.Length);
			int num4 = (int) BCryptNativeMethods.BCryptImportKey_KeyDataBlob(hAlg, IntPtr.Zero, out hKey, keyDataBuffer, num1, num3, num2);
			Marshal.FreeHGlobal(num3);
			int success = (int) AesGcm.SUCCESS;
			if (num4 == success)
				return true;
			Marshal.FreeHGlobal(keyDataBuffer);
			return false;
		}

		private static bool GetProperty<T>(IntPtr hAlg, string name, out T outData)
		{
			outData = default (T);
			uint pcbResult = 0;
			uint num1 = (uint) Marshal.SizeOf<T>();
			IntPtr num2 = Marshal.AllocHGlobal((int) num1);
			if ((int) BCryptNativeMethods.BCryptGetProperty(hAlg, name, num2, num1, ref pcbResult, 0U) != (int) AesGcm.SUCCESS)
			{
				Marshal.FreeHGlobal(num2);
				return false;
			}
			outData = Marshal.PtrToStructure<T>(num2);
			Marshal.FreeHGlobal(num2);
			return true;
		}
	}
}
