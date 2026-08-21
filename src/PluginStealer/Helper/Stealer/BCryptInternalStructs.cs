

using System;
using System.Runtime.InteropServices;


namespace Plugin.Helper.Stealer
{
	public static class BCryptInternalStructs
	{
		private static uint BCRYPT_INIT_AUTH_MODE_INFO_VERSION = 1;

		public struct BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO : IDisposable
		{
			public uint cbSize;
			public uint dwInfoVersion;
			public IntPtr pbNonce;
			public uint cbNonce;
			public IntPtr pbAuthData;
			public uint cbAuthData;
			public IntPtr pbTag;
			public uint cbTag;
			public IntPtr pbMacContext;
			public uint cbMacContext;
			public uint cbAAD;
			public ulong cbData;
			public uint dwFlags;

			public BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO(byte[] iv, byte[] aad, byte[] tag)
				: this()
			{
				this.dwInfoVersion = BCryptInternalStructs.BCRYPT_INIT_AUTH_MODE_INFO_VERSION;
				this.cbSize = (uint) Marshal.SizeOf(typeof (BCryptInternalStructs.BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO));
				if (iv != null)
				{
					this.cbNonce = (uint) iv.Length;
					this.pbNonce = Marshal.AllocHGlobal((int) this.cbNonce);
					Marshal.Copy(iv, 0, this.pbNonce, (int) this.cbNonce);
				}
				if (aad != null)
				{
					this.cbAuthData = (uint) aad.Length;
					this.pbAuthData = Marshal.AllocHGlobal((int) this.cbAuthData);
					Marshal.Copy(aad, 0, this.pbAuthData, (int) this.cbAuthData);
				}
				if (tag == null)
					return;
				this.cbTag = (uint) tag.Length;
				this.pbTag = Marshal.AllocHGlobal((int) this.cbTag);
				Marshal.Copy(tag, 0, this.pbTag, (int) this.cbTag);
				this.cbMacContext = (uint) tag.Length;
				this.pbMacContext = Marshal.AllocHGlobal((int) this.cbMacContext);
			}

			public void Dispose()
			{
				if (this.pbNonce != IntPtr.Zero)
					Marshal.FreeHGlobal(this.pbNonce);
				if (this.pbTag != IntPtr.Zero)
					Marshal.FreeHGlobal(this.pbTag);
				if (this.pbAuthData != IntPtr.Zero)
					Marshal.FreeHGlobal(this.pbAuthData);
				if (!(this.pbMacContext != IntPtr.Zero))
					return;
				Marshal.FreeHGlobal(this.pbMacContext);
			}
		}

		public struct BCRYPT_KEY_LENGTHS_STRUCT
		{
			public uint dwMinLength;
			public uint dwMaxLength;
			public uint dwIncrement;
		}

		public struct BCRYPT_KEY_DATA_BLOB_HEADER
		{
			public uint dwMagic;
			public uint dwVersion;
			public uint cbKeyData;
		}
	}
}
