

using InvokedCommon.Enums;


namespace InvokedCommon.Extensions
{
	public static class StringExtensions
	{
		public static ContentType ToContentType(this string fileExtension)
		{
			switch (fileExtension.ToLower())
			{
				case ".7z":
				case ".apk":
				case ".bz2":
				case ".cab":
				case ".gz":
				case ".rar":
				case ".s7z":
				case ".tar":
				case ".tgz":
				case ".zip":
				case ".zipx":
				case ".zz":
					return ContentType.Archive;
				case ".asc":
				case ".cfg":
				case ".conf":
				case ".log":
				case ".txt":
					return ContentType.Text;
				case ".avi":
				case ".flv":
				case ".m4v":
				case ".mkv":
				case ".mov":
				case ".mp4":
				case ".wmv":
					return ContentType.Video;
				case ".bmp":
				case ".gif":
				case ".ico":
				case ".jpeg":
				case ".jpg":
				case ".png":
					return ContentType.Image;
				case ".doc":
				case ".docx":
				case ".odt":
					return ContentType.Word;
				case ".exe":
					return ContentType.Application;
				case ".m3u":
				case ".m4a":
				case ".mp3":
				case ".pls":
				case ".wav":
					return ContentType.Audio;
				case ".pdf":
					return ContentType.Pdf;
				default:
					return ContentType.Blob;
			}
		}
	}
}
