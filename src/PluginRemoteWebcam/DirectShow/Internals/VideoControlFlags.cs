

using System;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow.Internals
{
	[Flags]
	[ComVisible(false)]
	internal enum VideoControlFlags
	{
		FlipHorizontal = 1,
		FlipVertical = 2,
		ExternalTriggerEnable = 4,
		Trigger = 8,
	}
}
