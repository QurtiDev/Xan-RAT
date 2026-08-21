

using System;
using System.Collections.Generic;


namespace InvokedServer.Controls.HexEditor
{
	public class ByteCollection
	{
		private List<byte> _bytes;

		public int Length => this._bytes.Count;

		public ByteCollection() => this._bytes = new List<byte>();

		public ByteCollection(byte[] bytes) => this._bytes = new List<byte>((IEnumerable<byte>)bytes);

		public void Add(byte item) => this._bytes.Add(item);

		public void Insert(int index, byte item) => this._bytes.Insert(index, item);

		public void Remove(byte item) => this._bytes.Remove(item);

		public void RemoveAt(int index) => this._bytes.RemoveAt(index);

		public void RemoveRange(int startIndex, int count)
		{
			this._bytes.RemoveRange(startIndex, count);
		}

		public byte GetAt(int index) => this._bytes[index];

		public void SetAt(int index, byte item) => this._bytes[index] = item;

		public char GetCharAt(int index) => Convert.ToChar(this._bytes[index]);

		public byte[] ToArray() => this._bytes.ToArray();
	}
}