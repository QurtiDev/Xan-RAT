

using System;
using System.Collections.Generic;


namespace InvokedServer.Networking
{
	public class BufferPool
	{
		private readonly int _bufferLength;
		private int _bufferCount;
		private readonly Stack<byte[]> _buffers;

		public event EventHandler NewBufferAllocated;

		protected virtual void OnNewBufferAllocated(EventArgs e)
		{
			EventHandler newBufferAllocated = this.NewBufferAllocated;
			if (newBufferAllocated == null)
				return;
			newBufferAllocated((object)this, e);
		}

		public event EventHandler BufferRequested;

		protected virtual void OnBufferRequested(EventArgs e)
		{
			EventHandler bufferRequested = this.BufferRequested;
			if (bufferRequested == null)
				return;
			bufferRequested((object)this, e);
		}

		public event EventHandler BufferReturned;

		protected virtual void OnBufferReturned(EventArgs e)
		{
			EventHandler bufferReturned = this.BufferReturned;
			if (bufferReturned == null)
				return;
			bufferReturned((object)this, e);
		}

		public int BufferLength => this._bufferLength;

		public int MaxBufferCount => this._bufferCount;

		public int BuffersAvailable => this._buffers.Count;

		public bool ClearOnReturn { get; set; }

		public BufferPool(int baseBufferLength, int baseBufferCount)
		{
			if (baseBufferLength <= 0)
				throw new ArgumentOutOfRangeException(nameof(baseBufferLength), (object)baseBufferLength, "Buffer length must be a positive integer value.");
			if (baseBufferCount <= 0)
				throw new ArgumentOutOfRangeException(nameof(baseBufferCount), (object)baseBufferCount, "Buffer count must be a positive integer value.");
			this._bufferLength = baseBufferLength;
			this._bufferCount = baseBufferCount;
			this._buffers = new Stack<byte[]>(baseBufferCount);
			for (int index = 0; index < baseBufferCount; ++index)
				this._buffers.Push(new byte[baseBufferLength]);
		}

		public byte[] GetBuffer()
		{
			lock (this._buffers)
			{
				if (this._buffers.Count > 0)
					return this._buffers.Pop();
			}
			return this.AllocateNewBuffer();
		}

		private byte[] AllocateNewBuffer()
		{
			byte[] numArray = new byte[this._bufferLength];
			++this._bufferCount;
			this.OnNewBufferAllocated(EventArgs.Empty);
			return numArray;
		}

		public bool ReturnBuffer(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));
			if (buffer.Length != this._bufferLength)
				return false;
			if (this.ClearOnReturn)
				Array.Clear((Array)buffer, 0, buffer.Length);
			lock (this._buffers)
			{
				if (!this._buffers.Contains(buffer))
					this._buffers.Push(buffer);
			}
			return true;
		}

		public void IncreaseBufferCount(int buffersToAdd)
		{
			List<byte[]> numArrayList = buffersToAdd > 0 ? new List<byte[]>(buffersToAdd) : throw new ArgumentOutOfRangeException(nameof(buffersToAdd), (object)buffersToAdd, "The number of buffers to add must be a nonnegative, nonzero integer.");
			for (int index = 0; index < buffersToAdd; ++index)
				numArrayList.Add(new byte[this._bufferLength]);
			lock (this._buffers)
			{
				this._bufferCount += buffersToAdd;
				for (int index = 0; index < buffersToAdd; ++index)
					this._buffers.Push(numArrayList[index]);
			}
		}

		public int DecreaseBufferCount(int buffersToRemove)
		{
			if (buffersToRemove <= 0)
				throw new ArgumentOutOfRangeException(nameof(buffersToRemove), (object)buffersToRemove, "The number of buffers to remove must be a nonnegative, nonzero integer.");
			int num = 0;
			lock (this._buffers)
			{
				for (int index = 0; index < buffersToRemove; ++index)
				{
					if (this._buffers.Count > 0)
					{
						this._buffers.Pop();
						++num;
						--this._bufferCount;
					}
					else
						break;
				}
			}
			return num;
		}
	}
}