


namespace AForge.Video.DirectShow
{
	public class VideoInput
	{
		public readonly int Index;
		public readonly PhysicalConnectorType Type;

		internal VideoInput(int index, PhysicalConnectorType type)
		{
			this.Index = index;
			this.Type = type;
		}

		public static VideoInput Default => new VideoInput(-1, PhysicalConnectorType.Default);
	}
}
