

using System.Security.Cryptography;
using System.Text;


namespace InvokedCommon.Cryptography
{
	public static class Sha256
	{
		public static string ComputeHash(string input)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(input);
			using (SHA256Managed shA256Managed = new SHA256Managed())
				buffer = shA256Managed.ComputeHash(buffer);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte num in buffer)
				stringBuilder.Append(num.ToString("X2"));
			return stringBuilder.ToString().ToUpper();
		}

		public static byte[] ComputeHash(byte[] input)
		{
			using (SHA256Managed shA256Managed = new SHA256Managed())
				return shA256Managed.ComputeHash(input);
		}
	}
}
