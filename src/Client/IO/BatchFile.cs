

using InvokedCommon.Helpers;
using System.IO;
using System.Text;


namespace InvokedClient.IO
{
	public static class BatchFile
	{
		public static string CreateUninstallBatch(string currentFilePath)
		{
			string tempFilePath = FileHelper.GetTempFilePath(".bat");
			string contents = "@echo off\r\nchcp 65001\r\necho DONT CLOSE THIS WINDOW!\r\nping -n 10 localhost > nul\r\ndel /a /q /f \"" + currentFilePath + "\"\r\ndel /a /q /f \"" + tempFilePath + "\"";
			File.WriteAllText(tempFilePath, contents, (Encoding)new UTF8Encoding(false));
			return tempFilePath;
		}

		public static string CreateUpdateBatch(string currentFilePath, string newFilePath)
		{
			string tempFilePath = FileHelper.GetTempFilePath(".bat");
			string contents = "@echo off\r\nchcp 65001\r\necho DONT CLOSE THIS WINDOW!\r\nping -n 10 localhost > nul\r\ndel /a /q /f \"" + currentFilePath + "\"\r\nmove /y \"" + newFilePath + "\" \"" + currentFilePath + "\"\r\nstart \"\" \"" + currentFilePath + "\"\r\ndel /a /q /f \"" + tempFilePath + "\"";
			File.WriteAllText(tempFilePath, contents, (Encoding)new UTF8Encoding(false));
			return tempFilePath;
		}

		public static string CreateRestartBatch(string currentFilePath)
		{
			string tempFilePath = FileHelper.GetTempFilePath(".bat");
			string contents = "@echo off\r\nchcp 65001\r\necho DONT CLOSE THIS WINDOW!\r\nping -n 10 localhost > nul\r\nstart \"\" \"" + currentFilePath + "\"\r\ndel /a /q /f \"" + tempFilePath + "\"";
			File.WriteAllText(tempFilePath, contents, (Encoding)new UTF8Encoding(false));
			return tempFilePath;
		}
	}
}
