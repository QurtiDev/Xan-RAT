

using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;


namespace InvokedClient.Helper
{
	public static class JsonHelper
	{
		public static string Serialize<T>(T o)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new DataContractJsonSerializer(typeof(T)).WriteObject((Stream)memoryStream, (object)o);
				return Encoding.UTF8.GetString(memoryStream.ToArray());
			}
		}

		public static T Deserialize<T>(string json)
		{
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
				return (T)new DataContractJsonSerializer(typeof(T)).ReadObject((Stream)memoryStream);
		}

		public static T Deserialize<T>(Stream stream)
		{
			return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
		}
	}
}