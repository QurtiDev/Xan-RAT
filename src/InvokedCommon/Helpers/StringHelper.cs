

using InvokedCommon.Utilities;
using System.Text;
using System.Text.RegularExpressions;


namespace InvokedCommon.Helpers
{
	public static class StringHelper
	{
		private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		private static readonly string[] Sizes = new string[6]
		{
			"B",
			"KB",
			"MB",
			"GB",
			"TB",
			"PB"
		};
		private static readonly SafeRandom Random = new SafeRandom();

		public static string GetRandomString(int length)
		{
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int index = 0; index < length; ++index)
				stringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"[StringHelper.Random.Next("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Length)]);
			return stringBuilder.ToString();
		}

		public static string GetHumanReadableFileSize(long size)
		{
			double num = (double) size;
			int index;
			for (index = 0; num >= 1024.0 && index + 1 < StringHelper.Sizes.Length; num /= 1024.0)
				++index;
			return string.Format("{0:0.##} {1}", (object) num, (object) StringHelper.Sizes[index]);
		}

		public static string GetFormattedMacAddress(string macAddress)
		{
			return macAddress.Length == 12 ? Regex.Replace(macAddress, "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})", "$1:$2:$3:$4:$5:$6") : "00:00:00:00:00:00";
		}

		public static string RemoveLastChars(string input, int amount = 2)
		{
			if (input.Length > amount)
				input = input.Remove(input.Length - amount);
			return input;
		}
	}
}
