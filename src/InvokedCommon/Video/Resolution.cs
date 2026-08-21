

using ProtoBuf;
using System;


namespace InvokedCommon.Video
{
	[ProtoContract]
	public class Resolution : IEquatable<Resolution>
	{
		[ProtoMember(1)]
		public int Width { get; set; }

		[ProtoMember(2)]
		public int Height { get; set; }

		public bool Equals(Resolution other)
		{
			if ((object)other == null)
				return false;
			if ((object)this == (object)other)
				return true;
			return this.Width == other.Width && this.Height == other.Height;
		}

		public static bool operator ==(Resolution r1, Resolution r2)
		{
			return (object)r1 == null ? (object)r2 == null : r1.Equals(r2);
		}

		public static bool operator !=(Resolution r1, Resolution r2) => !(r1 == r2);

		public override bool Equals(object obj) => this.Equals(obj as Resolution);

		public override int GetHashCode() => this.Width ^ this.Height;

		public override string ToString()
		{
			return string.Format("{0}x{1}", (object)this.Width, (object)this.Height);
		}
	}
}