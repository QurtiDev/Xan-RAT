

using InvokedCommon.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace InvokedCommon.IO
{
	public class FileSplit : IEnumerable<FileChunk>, IEnumerable, IDisposable
	{
		public readonly int MaxChunkSize = (int) ushort.MaxValue;
		private readonly FileStream _fileStream;

		public string FilePath => this._fileStream.Name;

		public long FileSize => this._fileStream.Length;

		public FileSplit(string filePath, FileAccess fileAccess)
		{
			if (fileAccess != FileAccess.Read)
			{
				if (fileAccess != FileAccess.Write)
					throw new ArgumentException("fileAccess must be either Read or Write.");
				this._fileStream = File.OpenWrite(filePath);
			}
			else
				this._fileStream = File.OpenRead(filePath);
		}

		public void WriteChunk(FileChunk chunk)
		{
			this._fileStream.Seek(chunk.Offset, SeekOrigin.Begin);
			this._fileStream.Write(chunk.Data, 0, chunk.Data.Length);
		}

		public FileChunk ReadChunk(long offset)
		{
			this._fileStream.Seek(offset, SeekOrigin.Begin);
			byte[] buffer = new byte[(this._fileStream.Length - this._fileStream.Position < (long)this.MaxChunkSize) ? (this._fileStream.Length - this._fileStream.Position) : ((long)this.MaxChunkSize)];
            this._fileStream.Read(buffer, 0, buffer.Length);
			return new FileChunk()
			{
				Data = buffer,
				Offset = this._fileStream.Position - (long) buffer.Length
			};
		}

		public IEnumerator<FileChunk> GetEnumerator()
		{
			for (long currentChunk = 0; currentChunk <= this._fileStream.Length / (long) this.MaxChunkSize; ++currentChunk)
				yield return this.ReadChunk(currentChunk * (long) this.MaxChunkSize);
		}

		IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			this._fileStream.Dispose();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object) this);
		}
	}
}
