

using System;
using System.Security.Cryptography;


namespace InvokedCommon.Utilities
{
	public class SafeRandom
	{
		private static readonly RandomNumberGenerator GlobalCryptoProvider = RandomNumberGenerator.Create();
		[ThreadStatic]
		private static Random _random;

		private static Random GetRandom()
		{
			if (SafeRandom._random == null)
			{
				byte[] data = new byte[4];
				SafeRandom.GlobalCryptoProvider.GetBytes(data);
				SafeRandom._random = new Random(BitConverter.ToInt32(data, 0));
			}
			return SafeRandom._random;
		}

		public int Next() => SafeRandom.GetRandom().Next();

		public int Next(int maxValue) => SafeRandom.GetRandom().Next(maxValue);

		public int Next(int minValue, int maxValue) => SafeRandom.GetRandom().Next(minValue, maxValue);

		public void NextBytes(byte[] buffer) => SafeRandom.GetRandom().NextBytes(buffer);

		public double NextDouble() => SafeRandom.GetRandom().NextDouble();
	}
}