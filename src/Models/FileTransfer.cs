

using InvokedCommon.IO;
using InvokedCommon.Utilities;
using InvokedServer.Enums;
using System;


namespace InvokedServer.Models
{
	public class FileTransfer : IEquatable<FileTransfer>
	{
		private static readonly SafeRandom Random = new SafeRandom();

		public int Id { get; set; }

		public TransferType Type { get; set; }

		public long Size { get; set; }

		public long TransferredSize { get; set; }

		public string LocalPath { get; set; }

		public string RemotePath { get; set; }

		public string Status { get; set; }

		public FileSplit FileSplit { get; set; }

		public bool Equals(FileTransfer other)
		{
			if ((object)other == null)
				return false;
			if ((object)this == (object)other)
				return true;
			return this.Id == other.Id && this.Type == other.Type && this.Size == other.Size && this.TransferredSize == other.TransferredSize && string.Equals(this.LocalPath, other.LocalPath) && string.Equals(this.RemotePath, other.RemotePath) && string.Equals(this.Status, other.Status);
		}

		public FileTransfer Clone()
		{
			return new FileTransfer()
			{
				Id = this.Id,
				Type = this.Type,
				Size = this.Size,
				TransferredSize = this.TransferredSize,
				LocalPath = this.LocalPath,
				RemotePath = this.RemotePath,
				Status = this.Status,
				FileSplit = this.FileSplit
			};
		}

		public static bool operator ==(FileTransfer f1, FileTransfer f2)
		{
			return (object)f1 == null ? (object)f2 == null : f1.Equals(f2);
		}

		public static bool operator !=(FileTransfer f1, FileTransfer f2) => !(f1 == f2);

		public override bool Equals(object obj) => this.Equals(obj as FileTransfer);

		public override int GetHashCode() => this.Id;

		public static int GetRandomTransferId() => FileTransfer.Random.Next(0, int.MaxValue);
	}
}