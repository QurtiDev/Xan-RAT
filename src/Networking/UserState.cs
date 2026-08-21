

using InvokedCommon.Cryptography;
using InvokedCommon.Helpers;
using System.IO;
using System.Windows.Forms;


namespace InvokedServer.Networking
{
	public class UserState
	{
		private string _downloadDirectory;
		private Aes256 _aesInstance;

		public string Version { get; set; }

		public string OperatingSystem { get; set; }

		public string AccountType { get; set; }

		public int ImageIndex { get; set; }

		public string Country { get; set; }

		public string CountryCode { get; set; }

		public string Id { get; set; }

		public string Username { get; set; }

		public string PcName { get; set; }

		public string UserAtPc => this.Username + "@" + this.PcName;

		public string CountryWithCode => this.Country + " [" + this.CountryCode + "]";

		public string Tag { get; set; }

		public string EncryptionKey { get; set; }

		public Aes256 AesInstance
		{
			get => this._aesInstance ?? (this._aesInstance = new Aes256(this.EncryptionKey));
		}

		public string DownloadDirectory
		{
			get
			{
				string downloadDirectory1 = this._downloadDirectory;
				if (downloadDirectory1 != null)
					return downloadDirectory1;
				string str;
				if (FileHelper.HasIllegalCharacters(this.UserAtPc))
					str = Path.Combine(Application.StartupPath, "Clients\\" + this.Id + "\\");
				else
					str = Path.Combine(Application.StartupPath, "Clients\\" + this.UserAtPc + "_" + this.Id.Substring(0, 7) + "\\");
				string downloadDirectory2 = str;
				this._downloadDirectory = str;
				return downloadDirectory2;
			}
		}
	}
}
