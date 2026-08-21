

using ProtoBuf;


namespace InvokedCommon.Structs
{
	[ProtoContract]
	public struct NgrokInfo
	{
		[ProtoMember(1)]
		public string authToken;

		public NgrokInfo(string _authToken) => this.authToken = _authToken;

		public override string ToString() => "AUTHTOKEN: " + this.authToken;
	}
}