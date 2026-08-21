

using InvokedCommon.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace InvokedCommon.Helpers
{
	public static class FileHelper
	{
		private static readonly char[] IllegalPathChars = ((IEnumerable<char>) Path.GetInvalidPathChars()).Union<char>((IEnumerable<char>) Path.GetInvalidFileNameChars()).ToArray<char>();

		public static bool HasIllegalCharacters(string path)
		{
			return path.Any<char>((Func<char, bool>) (c => ((IEnumerable<char>) FileHelper.IllegalPathChars).Contains<char>(c)));
		}

		public static string GetRandomFilename(int length, string extension = "")
		{
			return StringHelper.GetRandomString(length) + extension;
		}

		public static string GetTempFilePath(string extension)
		{
			string path;
			do
			{
				path = Path.Combine(Path.GetTempPath(), FileHelper.GetRandomFilename(12, extension));
			}
			while (File.Exists(path));
			return path;
		}

		public static bool HasExecutableIdentifier(byte[] binary)
		{
			if (binary.Length < 2)
				return false;
			if (binary[0] == (byte) 77 && binary[1] == (byte) 90)
				return true;
			return binary[0] == (byte) 90 && binary[1] == (byte) 77;
		}

		public static bool DeleteZoneIdentifier(string filePath)
		{
			return NativeMethods.DeleteFile(filePath + ":Zone.Identifier");
		}

		public static void WriteLogFile(string filename, string appendText, Aes256 aes)
		{
			appendText = FileHelper.ReadLogFile(filename, aes) + appendText;
			using (FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write))
			{
				byte[] buffer = aes.Encrypt(Encoding.UTF8.GetBytes(appendText));
				fileStream.Seek(0L, SeekOrigin.Begin);
				fileStream.Write(buffer, 0, buffer.Length);
			}
		}

		public static string ReadLogFile(string filename, Aes256 aes)
		{
			return !File.Exists(filename) ? string.Empty : Encoding.UTF8.GetString(aes.Decrypt(File.ReadAllBytes(filename)));
		}
	}
}
