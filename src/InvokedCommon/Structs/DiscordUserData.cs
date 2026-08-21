

using ProtoBuf;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct DiscordUserData
	{
		[ProtoMember(1)]
		public string token;
		[ProtoMember(2)]
		public string username;
		[ProtoMember(3)]
		public string email;
		[ProtoMember(4)]
		public string phoneNumber;
		[ProtoMember(5)]
		public string id;
		[ProtoMember(6)]
		public bool hasNitro;

		public DiscordUserData(
		    string _token,
		    string _username,
		    string _email,
		    string _phoneNumber,
		    string _id,
		    bool _hasNitro)
		{
			this.token = _token;
			this.username = _username;
			this.email = _email;
			this.phoneNumber = _phoneNumber;
			this.id = _id;
			this.hasNitro = _hasNitro;
		}
	}
}
