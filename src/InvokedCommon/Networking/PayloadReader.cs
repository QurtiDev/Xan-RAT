

using InvokedCommon.Messages;
using ProtoBuf;
using System;
using System.IO;


namespace InvokedCommon.Networking
{
	public class PayloadReader : MemoryStream
	{
		private readonly Stream _innerStream;

		public bool LeaveInnerStreamOpen { get; }

		public PayloadReader(byte[] payload, int length, bool leaveInnerStreamOpen)
		{
			this._innerStream = (Stream)new MemoryStream(payload, 0, length, false, true);
			this.LeaveInnerStreamOpen = leaveInnerStreamOpen;
		}

		public PayloadReader(Stream stream, bool leaveInnerStreamOpen)
		{
			this._innerStream = stream;
			this.LeaveInnerStreamOpen = leaveInnerStreamOpen;
		}

		public int ReadInteger() => BitConverter.ToInt32(this.ReadBytes(4), 0);

		public byte[] ReadBytes(int length)
		{
			if (this._innerStream.Position + (long)length > this._innerStream.Length)
				throw new OverflowException(string.Format("Unable to read {0} bytes from stream", (object)length));
			byte[] buffer = new byte[length];
			this._innerStream.Read(buffer, 0, buffer.Length);
			return buffer;
		}

		public IMessage ReadMessage()
		{
			this.ReadInteger();
			return Serializer.Deserialize<IMessage>(this._innerStream);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this.LeaveInnerStreamOpen)
					this._innerStream.Flush();
				else
					this._innerStream.Close();
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
	}
}