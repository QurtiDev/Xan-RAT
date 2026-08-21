

using InvokedCommon.Messages;
using ProtoBuf;
using System;
using System.IO;


namespace InvokedCommon.Networking
{
	public class PayloadWriter : MemoryStream
	{
		private readonly Stream _innerStream;

		public bool LeaveInnerStreamOpen { get; }

		public PayloadWriter(Stream stream, bool leaveInnerStreamOpen)
		{
			this._innerStream = stream;
			this.LeaveInnerStreamOpen = leaveInnerStreamOpen;
		}

		public void WriteBytes(byte[] value) => this._innerStream.Write(value, 0, value.Length);

		public void WriteInteger(int value) => this.WriteBytes(BitConverter.GetBytes(value));

		public int WriteMessage(IMessage message)
		{
			using (MemoryStream destination = new MemoryStream())
			{
				Serializer.Serialize<IMessage>((Stream)destination, message);
				byte[] array = destination.ToArray();
				this.WriteInteger(array.Length);
				this.WriteBytes(array);
				return 4 + array.Length;
			}
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