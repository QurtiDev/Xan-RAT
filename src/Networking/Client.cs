

using InvokedCommon.Messages;
using InvokedCommon.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Threading;


namespace InvokedServer.Networking
{
	public class Client : IEquatable<Client>, ISender
	{
		private readonly SslStream _stream;
		private readonly BufferPool _bufferPool;
		private readonly Queue<IMessage> _sendBuffers = new Queue<IMessage>();
		private bool _sendingMessages;
		private readonly object _sendingMessagesLock = new object();
		private readonly Queue<byte[]> _readBuffers = new Queue<byte[]>();
		private bool _readingMessages;
		private readonly object _readingMessagesLock = new object();
		private int _readOffset;
		private int _writeOffset;
		private int _readableDataLen;
		private int _payloadLen;
		private Client.ReceiveType _receiveState;
		private readonly byte[] _readBuffer;
		private byte[] _payloadBuffer;
		private const int HeaderSize = 4;
		private const int MaxMessageSize = 5242880;
		private readonly Mutex _singleWriteMutex = new Mutex();

		public event Client.ClientStateEventHandler ClientState;

		private void OnClientState(bool connected)
		{
			if (this.Connected == connected)
				return;
			this.Connected = connected;
			Client.ClientStateEventHandler clientState = this.ClientState;
			if (clientState == null)
				return;
			clientState(this, connected);
		}

		public event Client.ClientReadEventHandler ClientRead;

		private void OnClientRead(IMessage message, int messageLength)
		{
			Client.ClientReadEventHandler clientRead = this.ClientRead;
			if (clientRead == null)
				return;
			clientRead(this, message, messageLength);
		}

		public event Client.ClientWriteEventHandler ClientWrite;

		private void OnClientWrite(IMessage message, int messageLength)
		{
			Client.ClientWriteEventHandler clientWrite = this.ClientWrite;
			if (clientWrite == null)
				return;
			clientWrite(this, message, messageLength);
		}

		public static bool operator ==(Client c1, Client c2)
		{
			return (object)c1 == null ? (object)c2 == null : c1.Equals(c2);
		}

		public static bool operator !=(Client c1, Client c2) => !(c1 == c2);

		public bool Equals(Client other)
		{
			if ((object)other == null)
				return false;
			if ((object)this == (object)other)
				return true;
			try
			{
				return this.EndPoint.Port.Equals(other.EndPoint.Port);
			}
			catch
			{
				return false;
			}
		}

		public override bool Equals(object obj) => this.Equals(obj as Client);

		public bool PEquals(object obj)
		{
			return this.EndPoint.Address.Equals((object)(obj as Client).EndPoint.Address);
		}

		public override int GetHashCode() => this.EndPoint.GetHashCode();

		public DateTime ConnectedTime { get; }

		public bool Connected { get; private set; }

		public bool Identified { get; set; }

		public UserState Value { get; set; }

		public IPEndPoint EndPoint { get; }

		public Client(BufferPool bufferPool, SslStream stream, IPEndPoint endPoint)
		{
			try
			{
				this.Identified = false;
				this.Value = new UserState();
				this.EndPoint = endPoint;
				this.ConnectedTime = DateTime.UtcNow;
				this._stream = stream;
				this._bufferPool = bufferPool;
				this._readBuffer = this._bufferPool.GetBuffer();
				this._stream.BeginRead(this._readBuffer, 0, this._readBuffer.Length, new AsyncCallback(this.AsyncReceive), (object)null);
				this.OnClientState(true);
			}
			catch
			{
				this.Disconnect();
			}
		}

		private void AsyncReceive(IAsyncResult result)
		{
			int length;
			try
			{
				length = this._stream.EndRead(result);
				if (length <= 0)
					throw new Exception("no bytes transferred");
			}
			catch (NullReferenceException)
			{
				return;
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			catch
			{
				this.Disconnect();
				return;
			}
			byte[] destinationArray = new byte[length];
			try
			{
				Array.Copy((Array)this._readBuffer, (Array)destinationArray, destinationArray.Length);
			}
			catch
			{
				this.Disconnect();
				return;
			}
			lock (this._readBuffers)
				this._readBuffers.Enqueue(destinationArray);
			lock (this._readingMessagesLock)
			{
				if (!this._readingMessages)
				{
					this._readingMessages = true;
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsyncReceive));
				}
			}
			try
			{
				this._stream.BeginRead(this._readBuffer, 0, this._readBuffer.Length, new AsyncCallback(this.AsyncReceive), (object)null);
			}
			catch (ObjectDisposedException)
			{
			}
			catch
			{
				this.Disconnect();
			}
		}

		private void AsyncReceive(object state)
		{
			while (true)
			{
				byte[] sourceArray;
				lock (this._readBuffers)
				{
					if (this._readBuffers.Count == 0)
					{
						lock (this._readingMessagesLock)
						{
							this._readingMessages = false;
							break;
						}
					}
					else
						sourceArray = this._readBuffers.Dequeue();
				}
				this._readableDataLen += sourceArray.Length;
				bool flag = true;
				while (flag)
				{
					switch (this._receiveState)
					{
						case Client.ReceiveType.Header:
							if (this._payloadBuffer == null)
								this._payloadBuffer = new byte[4];
							if (this._readableDataLen + this._writeOffset >= 4)
							{
								int length = 4 - this._writeOffset;
								try
								{
									Array.Copy((Array)sourceArray, this._readOffset, (Array)this._payloadBuffer, this._writeOffset, length);
									this._payloadLen = BitConverter.ToInt32(this._payloadBuffer, this._readOffset);
									if (this._payloadLen <= 0 || this._payloadLen > 5242880)
										throw new Exception("invalid header");
									if (this._payloadBuffer.Length <= this._payloadLen + 4)
										Array.Resize<byte>(ref this._payloadBuffer, this._payloadLen + 4);
								}
								catch
								{
									flag = false;
									this.Disconnect();
									continue;
								}
								this._readableDataLen -= length;
								this._writeOffset += length;
								this._readOffset += length;
								this._receiveState = Client.ReceiveType.Payload;
								continue;
							}
							try
							{
								Array.Copy((Array)sourceArray, this._readOffset, (Array)this._payloadBuffer, this._writeOffset, this._readableDataLen);
							}
							catch
							{
								flag = false;
								this.Disconnect();
								continue;
							}
							this._readOffset += this._readableDataLen;
							this._writeOffset += this._readableDataLen;
							flag = false;
							continue;
						case Client.ReceiveType.Payload:
							int length1 = this._writeOffset - 4 + this._readableDataLen >= this._payloadLen ? this._payloadLen - (this._writeOffset - 4) : this._readableDataLen;
							try
							{
								Array.Copy((Array)sourceArray, this._readOffset, (Array)this._payloadBuffer, this._writeOffset, length1);
							}
							catch
							{
								flag = false;
								this.Disconnect();
								continue;
							}
							this._writeOffset += length1;
							this._readOffset += length1;
							this._readableDataLen -= length1;
							if (this._writeOffset - 4 == this._payloadLen)
							{
								try
								{
									using (PayloadReader payloadReader = new PayloadReader(this._payloadBuffer, this._payloadLen + 4, false))
										this.OnClientRead(payloadReader.ReadMessage(), this._payloadBuffer.Length);
								}
								catch
								{
									flag = false;
									this.Disconnect();
									continue;
								}
								this._receiveState = Client.ReceiveType.Header;
								this._payloadLen = 0;
								this._writeOffset = 0;
							}
							if (this._readableDataLen == 0)
							{
								flag = false;
								continue;
							}
							continue;
						default:
							continue;
					}
				}
				this._readOffset = 0;
				this._readableDataLen = 0;
			}
		}

		public void Send<T>(T message) where T : IMessage
		{
			if (!this.Connected || (object)message == null)
				return;
			lock (this._sendBuffers)
			{
				this._sendBuffers.Enqueue((IMessage)message);
				lock (this._sendingMessagesLock)
				{
					if (this._sendingMessages)
						return;
					this._sendingMessages = true;
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcessSendBuffers));
				}
			}
		}

		public void SendBlocking<T>(T message) where T : IMessage
		{
			if (!this.Connected || (object)message == null)
				return;
			this.SafeSendMessage((IMessage)message);
		}

		private void SafeSendMessage(IMessage message)
		{
			try
			{
				this._singleWriteMutex.WaitOne();
				using (PayloadWriter payloadWriter = new PayloadWriter((Stream)this._stream, true))
					this.OnClientWrite(message, payloadWriter.WriteMessage(message));
			}
			catch
			{
				this.Disconnect();
				this.SendCleanup(true);
			}
			finally
			{
				this._singleWriteMutex.ReleaseMutex();
			}
		}

		private void ProcessSendBuffers(object state)
		{
			while (this.Connected)
			{
				IMessage message;
				lock (this._sendBuffers)
				{
					if (this._sendBuffers.Count == 0)
					{
						this.SendCleanup();
						return;
					}
					message = this._sendBuffers.Dequeue();
				}
				this.SafeSendMessage(message);
			}
			this.SendCleanup(true);
		}

		private void SendCleanup(bool clear = false)
		{
			lock (this._sendingMessagesLock)
				this._sendingMessages = false;
			if (!clear)
				return;
			lock (this._sendBuffers)
				this._sendBuffers.Clear();
		}

		public void Disconnect()
		{
			if (this._stream != null)
			{
				this._stream.Close();
				this._readOffset = 0;
				this._writeOffset = 0;
				this._readableDataLen = 0;
				this._payloadLen = 0;
				this._payloadBuffer = (byte[])null;
				this._receiveState = Client.ReceiveType.Header;
				this._singleWriteMutex.Dispose();
				this._bufferPool.ReturnBuffer(this._readBuffer);
			}
			this.OnClientState(false);
		}

		public delegate void ClientStateEventHandler(Client s, bool connected);

		public delegate void ClientReadEventHandler(Client s, IMessage message, int messageLength);

		public delegate void ClientWriteEventHandler(Client s, IMessage message, int messageLength);

		public enum ReceiveType
		{
			Header,
			Payload,
		}
	}
}