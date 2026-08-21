

using System;
using System.Collections.Generic;
using System.Text;


namespace InvokedCommon.Utilities
{
	public class ByteConverter
	{
		private static byte NULL_BYTE;

		public static byte[] GetBytes(int value) => BitConverter.GetBytes(value);

		public static byte[] GetBytes(long value) => BitConverter.GetBytes(value);

		public static byte[] GetBytes(uint value) => BitConverter.GetBytes(value);

		public static byte[] GetBytes(ulong value) => BitConverter.GetBytes(value);

		public static byte[] GetBytes(string value) => ByteConverter.StringToBytes(value);

		public static byte[] GetBytes(string[] value) => ByteConverter.StringArrayToBytes(value);

		public static int ToInt32(byte[] bytes) => BitConverter.ToInt32(bytes, 0);

		public static long ToInt64(byte[] bytes) => BitConverter.ToInt64(bytes, 0);

		public static uint ToUInt32(byte[] bytes) => BitConverter.ToUInt32(bytes, 0);

		public static ulong ToUInt64(byte[] bytes) => BitConverter.ToUInt64(bytes, 0);

		public static string ToString(byte[] bytes) => ByteConverter.BytesToString(bytes);

		public static string[] ToStringArray(byte[] bytes) => ByteConverter.BytesToStringArray(bytes);

		private static byte[] GetNullBytes()
		{
			return new byte[2]
			{
		ByteConverter.NULL_BYTE,
		ByteConverter.NULL_BYTE
			};
		}

		private static byte[] StringToBytes(string value)
		{
			byte[] dst = new byte[value.Length * 2];
			Buffer.BlockCopy((Array)value.ToCharArray(), 0, (Array)dst, 0, dst.Length);
			return dst;
		}

		private static byte[] StringArrayToBytes(string[] strings)
		{
			List<byte> byteList = new List<byte>();
			foreach (string str in strings)
			{
				byteList.AddRange((IEnumerable<byte>)ByteConverter.StringToBytes(str));
				byteList.AddRange((IEnumerable<byte>)ByteConverter.GetNullBytes());
			}
			return byteList.ToArray();
		}

		private static string BytesToString(byte[] bytes)
		{
			char[] dst = new char[(int)Math.Ceiling((double)bytes.Length / 2.0)];
			Buffer.BlockCopy((Array)bytes, 0, (Array)dst, 0, bytes.Length);
			return new string(dst);
		}

		private static string[] BytesToStringArray(byte[] bytes)
		{
			List<string> stringList = new List<string>();
			int index1 = 0;
			StringBuilder stringBuilder = new StringBuilder(bytes.Length);
			while (index1 < bytes.Length)
			{
				for (int index2 = 0; index1 < bytes.Length && index2 < 3; ++index1)
				{
					if ((int)bytes[index1] == (int)ByteConverter.NULL_BYTE)
					{
						++index2;
					}
					else
					{
						stringBuilder.Append(Convert.ToChar(bytes[index1]));
						index2 = 0;
					}
				}
				stringList.Add(stringBuilder.ToString());
				stringBuilder.Clear();
			}
			return stringList.ToArray();
		}
	}
}