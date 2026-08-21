

using AForge.Video.DirectShow.Internals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;


namespace AForge.Video.DirectShow
{
	public class VideoCapabilities
	{
		public readonly Size FrameSize;
		public readonly int AverageFrameRate;
		public readonly int MaximumFrameRate;
		public readonly int BitCount;

		[Obsolete("No longer supported. Use AverageFrameRate instead.")]
		public int FrameRate => this.AverageFrameRate;

		internal VideoCapabilities()
		{
		}

		internal static VideoCapabilities[] FromStreamConfig(IAMStreamConfig videoStreamConfig)
		{
			int count;
			int size;
			int errorCode = videoStreamConfig != null ? videoStreamConfig.GetNumberOfCapabilities(out count, out size) : throw new ArgumentNullException(nameof (videoStreamConfig));
			if (errorCode != 0)
				Marshal.ThrowExceptionForHR(errorCode);
			if (count <= 0)
				throw new NotSupportedException("This video device does not report capabilities.");
			if (size > Marshal.SizeOf(typeof (VideoStreamConfigCaps)))
				throw new NotSupportedException("Unable to retrieve video device capabilities. This video device requires a larger VideoStreamConfigCaps structure.");
			Dictionary<uint, VideoCapabilities> dictionary = new Dictionary<uint, VideoCapabilities>();
			for (int index = 0; index < count; ++index)
			{
				try
				{
					VideoCapabilities videoCapabilities = new VideoCapabilities(videoStreamConfig, index);
					uint key = (uint) (videoCapabilities.FrameSize.Height | videoCapabilities.FrameSize.Width << 16);
					if (!dictionary.ContainsKey(key))
						dictionary.Add(key, videoCapabilities);
					else if (videoCapabilities.BitCount > dictionary[key].BitCount)
						dictionary[key] = videoCapabilities;
				}
				catch
				{
				}
			}
			VideoCapabilities[] array = new VideoCapabilities[dictionary.Count];
			dictionary.Values.CopyTo(array, 0);
			return array;
		}

		internal VideoCapabilities(IAMStreamConfig videoStreamConfig, int index)
		{
			AMMediaType mediaType = (AMMediaType) null;
			VideoStreamConfigCaps streamConfigCaps = new VideoStreamConfigCaps();
			try
			{
				int streamCaps = videoStreamConfig.GetStreamCaps(index, out mediaType, streamConfigCaps);
				if (streamCaps != 0)
					Marshal.ThrowExceptionForHR(streamCaps);
				if (mediaType.FormatType == FormatType.VideoInfo)
				{
					VideoInfoHeader structure = (VideoInfoHeader) Marshal.PtrToStructure(mediaType.FormatPtr, typeof (VideoInfoHeader));
					this.FrameSize = new Size(structure.BmiHeader.Width, structure.BmiHeader.Height);
					this.BitCount = (int) structure.BmiHeader.BitCount;
					this.AverageFrameRate = (int) (10000000L / structure.AverageTimePerFrame);
					this.MaximumFrameRate = (int) (10000000L / streamConfigCaps.MinFrameInterval);
				}
				else
				{
					if (!(mediaType.FormatType == FormatType.VideoInfo2))
						throw new ApplicationException("Unsupported format found.");
					VideoInfoHeader2 structure = (VideoInfoHeader2) Marshal.PtrToStructure(mediaType.FormatPtr, typeof (VideoInfoHeader2));
					this.FrameSize = new Size(structure.BmiHeader.Width, structure.BmiHeader.Height);
					this.BitCount = (int) structure.BmiHeader.BitCount;
					this.AverageFrameRate = (int) (10000000L / structure.AverageTimePerFrame);
					this.MaximumFrameRate = (int) (10000000L / streamConfigCaps.MinFrameInterval);
				}
				if (this.BitCount <= 12)
					throw new ApplicationException("Unsupported format found.");
			}
			finally
			{
				mediaType?.Dispose();
			}
		}

		public override bool Equals(object obj) => this.Equals(obj as VideoCapabilities);

		public bool Equals(VideoCapabilities vc2)
		{
			return (object) vc2 != null && this.FrameSize == vc2.FrameSize && this.BitCount == vc2.BitCount;
		}

		public override int GetHashCode() => this.FrameSize.GetHashCode() ^ this.BitCount;

		public static bool operator ==(VideoCapabilities a, VideoCapabilities b)
		{
			if ((object) a == (object) b)
				return true;
			return (object) a != null && (object) b != null && a.Equals(b);
		}

		public static bool operator !=(VideoCapabilities a, VideoCapabilities b) => !(a == b);
	}
}
