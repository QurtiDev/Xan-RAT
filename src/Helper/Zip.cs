

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;


namespace InvokedServer.Helper
{
	public static class Zip
	{
		public static byte[] Decompress(byte[] input)
		{
			using (MemoryStream memoryStream = new MemoryStream(input))
			{
				byte[] buffer1 = new byte[4];
				memoryStream.Read(buffer1, 0, 4);
				int int32 = BitConverter.ToInt32(buffer1, 0);
				using (GZipStream gzipStream = new GZipStream((Stream)memoryStream, CompressionMode.Decompress))
				{
					byte[] buffer2 = new byte[int32];
					gzipStream.Read(buffer2, 0, int32);
					return buffer2;
				}
			}
		}

		public static byte[] Compress(byte[] input)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] bytes = BitConverter.GetBytes(input.Length);
				memoryStream.Write(bytes, 0, 4);
				using (GZipStream gzipStream = new GZipStream((Stream)memoryStream, CompressionMode.Compress))
				{
					gzipStream.Write(input, 0, input.Length);
					gzipStream.Flush();
				}
				return memoryStream.ToArray();
			}
		}

		public static string GetChecksum(string file)
		{
			using (FileStream inputStream = File.OpenRead(file))
				return BitConverter.ToString(new SHA256Managed().ComputeHash((Stream)inputStream)).Replace("-", string.Empty);
		}
	}
}