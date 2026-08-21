

using AForge.Video.DirectShow.Internals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;


namespace AForge.Video.DirectShow
{
	public class VideoCaptureDevice : IVideoSource
	{
		private string deviceMoniker;
		private int framesReceived;
		private long bytesReceived;
		private AForge.Video.DirectShow.VideoCapabilities videoResolution;
		private AForge.Video.DirectShow.VideoCapabilities snapshotResolution;
		private bool provideSnapshots;
		private Thread thread;
		private ManualResetEvent stopEvent;
		private AForge.Video.DirectShow.VideoCapabilities[] videoCapabilities;
		private AForge.Video.DirectShow.VideoCapabilities[] snapshotCapabilities;
		private bool needToSetVideoInput;
		private bool needToSimulateTrigger;
		private bool needToDisplayPropertyPage;
		private bool needToDisplayCrossBarPropertyPage;
		private IntPtr parentWindowForPropertyPage = IntPtr.Zero;
		private object sourceObject;
		private DateTime startTime;
		private object sync = new object();
		private bool? isCrossbarAvailable;
		private VideoInput[] crossbarVideoInputs;
		private VideoInput crossbarVideoInput = VideoInput.Default;
		private static Dictionary<string, AForge.Video.DirectShow.VideoCapabilities[]> cacheVideoCapabilities = new Dictionary<string, AForge.Video.DirectShow.VideoCapabilities[]>();
		private static Dictionary<string, AForge.Video.DirectShow.VideoCapabilities[]> cacheSnapshotCapabilities = new Dictionary<string, AForge.Video.DirectShow.VideoCapabilities[]>();
		private static Dictionary<string, VideoInput[]> cacheCrossbarVideoInputs = new Dictionary<string, VideoInput[]>();

		public VideoInput CrossbarVideoInput
		{
			get => this.crossbarVideoInput;
			set
			{
				this.needToSetVideoInput = true;
				this.crossbarVideoInput = value;
			}
		}

		public VideoInput[] AvailableCrossbarVideoInputs
		{
			get
			{
				if (this.crossbarVideoInputs == null)
				{
					lock (VideoCaptureDevice.cacheCrossbarVideoInputs)
					{
						if (!string.IsNullOrEmpty(this.deviceMoniker))
						{
							if (VideoCaptureDevice.cacheCrossbarVideoInputs.ContainsKey(this.deviceMoniker))
								this.crossbarVideoInputs = VideoCaptureDevice.cacheCrossbarVideoInputs[this.deviceMoniker];
						}
					}
					if (this.crossbarVideoInputs == null)
					{
						if (!this.IsRunning)
						{
							this.WorkerThread(false);
						}
						else
						{
							for (int index = 0; index < 500 && this.crossbarVideoInputs == null; ++index)
								Thread.Sleep(10);
						}
					}
				}
				return this.crossbarVideoInputs == null ? new VideoInput[0] : this.crossbarVideoInputs;
			}
		}

		public bool ProvideSnapshots
		{
			get => this.provideSnapshots;
			set => this.provideSnapshots = value;
		}

		public event NewFrameEventHandler NewFrame;

		public event NewFrameEventHandler SnapshotFrame;

		public event VideoSourceErrorEventHandler VideoSourceError;

		public event PlayingFinishedEventHandler PlayingFinished;

		public virtual string Source
		{
			get => this.deviceMoniker;
			set
			{
				this.deviceMoniker = value;
				this.videoCapabilities = (AForge.Video.DirectShow.VideoCapabilities[]) null;
				this.snapshotCapabilities = (AForge.Video.DirectShow.VideoCapabilities[]) null;
				this.crossbarVideoInputs = (VideoInput[]) null;
				this.isCrossbarAvailable = new bool?();
			}
		}

		public int FramesReceived
		{
			get
			{
				int framesReceived = this.framesReceived;
				this.framesReceived = 0;
				return framesReceived;
			}
		}

		public long BytesReceived
		{
			get
			{
				long bytesReceived = this.bytesReceived;
				this.bytesReceived = 0L;
				return bytesReceived;
			}
		}

		public bool IsRunning
		{
			get
			{
				if (this.thread != null)
				{
					if (!this.thread.Join(0))
						return true;
					this.Free();
				}
				return false;
			}
		}

		[Obsolete]
		public Size DesiredFrameSize
		{
			get => Size.Empty;
			set
			{
			}
		}

		[Obsolete]
		public Size DesiredSnapshotSize
		{
			get => Size.Empty;
			set
			{
			}
		}

		[Obsolete]
		public int DesiredFrameRate
		{
			get => 0;
			set
			{
			}
		}

		public AForge.Video.DirectShow.VideoCapabilities VideoResolution
		{
			get => this.videoResolution;
			set => this.videoResolution = value;
		}

		public AForge.Video.DirectShow.VideoCapabilities SnapshotResolution
		{
			get => this.snapshotResolution;
			set => this.snapshotResolution = value;
		}

		public AForge.Video.DirectShow.VideoCapabilities[] VideoCapabilities
		{
			get
			{
				if (this.videoCapabilities == null)
				{
					lock (VideoCaptureDevice.cacheVideoCapabilities)
					{
						if (!string.IsNullOrEmpty(this.deviceMoniker))
						{
							if (VideoCaptureDevice.cacheVideoCapabilities.ContainsKey(this.deviceMoniker))
								this.videoCapabilities = VideoCaptureDevice.cacheVideoCapabilities[this.deviceMoniker];
						}
					}
					if (this.videoCapabilities == null)
					{
						if (!this.IsRunning)
						{
							this.WorkerThread(false);
						}
						else
						{
							for (int index = 0; index < 500 && this.videoCapabilities == null; ++index)
								Thread.Sleep(10);
						}
					}
				}
				return this.videoCapabilities == null ? new AForge.Video.DirectShow.VideoCapabilities[0] : this.videoCapabilities;
			}
		}

		public AForge.Video.DirectShow.VideoCapabilities[] SnapshotCapabilities
		{
			get
			{
				if (this.snapshotCapabilities == null)
				{
					lock (VideoCaptureDevice.cacheSnapshotCapabilities)
					{
						if (!string.IsNullOrEmpty(this.deviceMoniker))
						{
							if (VideoCaptureDevice.cacheSnapshotCapabilities.ContainsKey(this.deviceMoniker))
								this.snapshotCapabilities = VideoCaptureDevice.cacheSnapshotCapabilities[this.deviceMoniker];
						}
					}
					if (this.snapshotCapabilities == null)
					{
						if (!this.IsRunning)
						{
							this.WorkerThread(false);
						}
						else
						{
							for (int index = 0; index < 500 && this.snapshotCapabilities == null; ++index)
								Thread.Sleep(10);
						}
					}
				}
				return this.snapshotCapabilities == null ? new AForge.Video.DirectShow.VideoCapabilities[0] : this.snapshotCapabilities;
			}
		}

		public object SourceObject => this.sourceObject;

		public VideoCaptureDevice()
		{
		}

		public VideoCaptureDevice(string deviceMoniker) => this.deviceMoniker = deviceMoniker;

		public void Start()
		{
			if (this.IsRunning)
				return;
			if (string.IsNullOrEmpty(this.deviceMoniker))
				throw new ArgumentException("Video source is not specified.");
			this.framesReceived = 0;
			this.bytesReceived = 0L;
			this.isCrossbarAvailable = new bool?();
			this.needToSetVideoInput = true;
			this.stopEvent = new ManualResetEvent(false);
			lock (this.sync)
			{
				this.thread = new Thread(new ThreadStart(this.WorkerThread));
				this.thread.Name = this.deviceMoniker;
				this.thread.Start();
			}
		}

		public void SignalToStop()
		{
			if (this.thread == null)
				return;
			this.stopEvent.Set();
		}

		public void WaitForStop()
		{
			if (this.thread == null)
				return;
			this.thread.Join();
			this.Free();
		}

		public void Stop()
		{
			if (!this.IsRunning)
				return;
			this.thread.Abort();
			this.WaitForStop();
		}

		private void Free()
		{
			this.thread = (Thread) null;
			this.stopEvent.Close();
			this.stopEvent = (ManualResetEvent) null;
		}

		public void DisplayPropertyPage(IntPtr parentWindow)
		{
			if (this.deviceMoniker == null || this.deviceMoniker == string.Empty)
				throw new ArgumentException("Video source is not specified.");
			lock (this.sync)
			{
				if (this.IsRunning)
				{
					this.parentWindowForPropertyPage = parentWindow;
					this.needToDisplayPropertyPage = true;
				}
				else
				{
					object filter;
					try
					{
						filter = FilterInfo.CreateFilter(this.deviceMoniker);
					}
					catch
					{
						throw new ApplicationException("Failed creating device object for moniker.");
					}
					if (!(filter is ISpecifyPropertyPages))
						throw new NotSupportedException("The video source does not support configuration property page.");
					this.DisplayPropertyPage(parentWindow, filter);
					Marshal.ReleaseComObject(filter);
				}
			}
		}

		public void DisplayCrossbarPropertyPage(IntPtr parentWindow)
		{
			lock (this.sync)
			{
				for (int index = 0; index < 500 && !this.isCrossbarAvailable.HasValue && this.IsRunning; ++index)
					Thread.Sleep(10);
				if (!this.IsRunning || !this.isCrossbarAvailable.HasValue)
					throw new ApplicationException("The video source must be running in order to display crossbar property page.");
				if (!this.isCrossbarAvailable.Value)
					throw new NotSupportedException("Crossbar configuration is not supported by currently running video source.");
				this.parentWindowForPropertyPage = parentWindow;
				this.needToDisplayCrossBarPropertyPage = true;
			}
		}

		public bool CheckIfCrossbarAvailable()
		{
			lock (this.sync)
			{
				if (!this.isCrossbarAvailable.HasValue)
				{
					if (!this.IsRunning)
					{
						this.WorkerThread(false);
					}
					else
					{
						for (int index = 0; index < 500 && !this.isCrossbarAvailable.HasValue; ++index)
							Thread.Sleep(10);
					}
				}
				return this.isCrossbarAvailable.HasValue && this.isCrossbarAvailable.Value;
			}
		}

		public void SimulateTrigger() => this.needToSimulateTrigger = true;

		public bool SetCameraProperty(
			CameraControlProperty property,
			int value,
			CameraControlFlags controlFlags)
		{
			bool flag = true;
			if (this.deviceMoniker == null || string.IsNullOrEmpty(this.deviceMoniker))
				throw new ArgumentException("Video source is not specified.");
			lock (this.sync)
			{
				object filter;
				try
				{
					filter = FilterInfo.CreateFilter(this.deviceMoniker);
				}
				catch
				{
					throw new ApplicationException("Failed creating device object for moniker.");
				}
				if (!(filter is IAMCameraControl))
					throw new NotSupportedException("The video source does not support camera control.");
				flag = ((IAMCameraControl) filter).Set(property, value, controlFlags) >= 0;
				Marshal.ReleaseComObject(filter);
			}
			return flag;
		}

		public bool GetCameraProperty(
			CameraControlProperty property,
			out int value,
			out CameraControlFlags controlFlags)
		{
			bool cameraProperty = true;
			if (this.deviceMoniker == null || string.IsNullOrEmpty(this.deviceMoniker))
				throw new ArgumentException("Video source is not specified.");
			lock (this.sync)
			{
				object filter;
				try
				{
					filter = FilterInfo.CreateFilter(this.deviceMoniker);
				}
				catch
				{
					throw new ApplicationException("Failed creating device object for moniker.");
				}
				if (!(filter is IAMCameraControl))
					throw new NotSupportedException("The video source does not support camera control.");
				cameraProperty = ((IAMCameraControl) filter).Get(property, out value, out controlFlags) >= 0;
				Marshal.ReleaseComObject(filter);
			}
			return cameraProperty;
		}

		public bool GetCameraPropertyRange(
			CameraControlProperty property,
			out int minValue,
			out int maxValue,
			out int stepSize,
			out int defaultValue,
			out CameraControlFlags controlFlags)
		{
			bool cameraPropertyRange = true;
			if (this.deviceMoniker == null || string.IsNullOrEmpty(this.deviceMoniker))
				throw new ArgumentException("Video source is not specified.");
			lock (this.sync)
			{
				object filter;
				try
				{
					filter = FilterInfo.CreateFilter(this.deviceMoniker);
				}
				catch
				{
					throw new ApplicationException("Failed creating device object for moniker.");
				}
				if (!(filter is IAMCameraControl))
					throw new NotSupportedException("The video source does not support camera control.");
				cameraPropertyRange = ((IAMCameraControl) filter).GetRange(property, out minValue, out maxValue, out stepSize, out defaultValue, out controlFlags) >= 0;
				Marshal.ReleaseComObject(filter);
			}
			return cameraPropertyRange;
		}

		private void WorkerThread() => this.WorkerThread(true);

		private void WorkerThread(bool runGraph)
		{
			ReasonToFinishPlaying reason = ReasonToFinishPlaying.StoppedByUser;
			bool flag = false;
			VideoCaptureDevice.Grabber callback1 = new VideoCaptureDevice.Grabber(this, false);
			VideoCaptureDevice.Grabber callback2 = new VideoCaptureDevice.Grabber(this, true);
			object o1 = (object) null;
			object o2 = (object) null;
			object o3 = (object) null;
			object o4 = (object) null;
			object retInterface = (object) null;
			ICaptureGraphBuilder2 captureGraphBuilder2 = (ICaptureGraphBuilder2) null;
			IFilterGraph2 filterGraph2_1 = (IFilterGraph2) null;
			IBaseFilter baseFilter1 = (IBaseFilter) null;
			IBaseFilter baseFilter2 = (IBaseFilter) null;
			IBaseFilter baseFilter3 = (IBaseFilter) null;
			ISampleGrabber sampleGrabber1 = (ISampleGrabber) null;
			ISampleGrabber sampleGrabber2 = (ISampleGrabber) null;
			IMediaControl mediaControl1 = (IMediaControl) null;
			IAMVideoControl amVideoControl = (IAMVideoControl) null;
			IMediaEventEx mediaEventEx1 = (IMediaEventEx) null;
			IPin pin = (IPin) null;
			IAMCrossbar amCrossbar = (IAMCrossbar) null;
			try
			{
				Type typeFromClsid1 = Type.GetTypeFromCLSID(Clsid.CaptureGraphBuilder2);
				o1 = !(typeFromClsid1 == (Type) null) ? Activator.CreateInstance(typeFromClsid1) : throw new ApplicationException("Failed creating capture graph builder");
				ICaptureGraphBuilder2 graphBuilder = (ICaptureGraphBuilder2) o1;
				Type typeFromClsid2 = Type.GetTypeFromCLSID(Clsid.FilterGraph);
				o2 = !(typeFromClsid2 == (Type) null) ? Activator.CreateInstance(typeFromClsid2) : throw new ApplicationException("Failed creating filter graph");
				IFilterGraph2 filterGraph2_2 = (IFilterGraph2) o2;
				graphBuilder.SetFiltergraph((IGraphBuilder) filterGraph2_2);
				this.sourceObject = FilterInfo.CreateFilter(this.deviceMoniker);
				IBaseFilter baseFilter4 = this.sourceObject != null ? (IBaseFilter) this.sourceObject : throw new ApplicationException("Failed creating device object for moniker");
				try
				{
					amVideoControl = (IAMVideoControl) this.sourceObject;
				}
				catch
				{
				}
				Type typeFromClsid3 = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
				o3 = !(typeFromClsid3 == (Type) null) ? Activator.CreateInstance(typeFromClsid3) : throw new ApplicationException("Failed creating sample grabber");
				ISampleGrabber sampleGrabber3 = (ISampleGrabber) o3;
				IBaseFilter baseFilter5 = (IBaseFilter) o3;
				o4 = Activator.CreateInstance(typeFromClsid3);
				ISampleGrabber sampleGrabber4 = (ISampleGrabber) o4;
				IBaseFilter baseFilter6 = (IBaseFilter) o4;
				filterGraph2_2.AddFilter(baseFilter4, "source");
				filterGraph2_2.AddFilter(baseFilter5, "grabber_video");
				filterGraph2_2.AddFilter(baseFilter6, "grabber_snapshot");
				AMMediaType mediaType = new AMMediaType();
				mediaType.MajorType = MediaType.Video;
				mediaType.SubType = MediaSubType.RGB24;
				sampleGrabber3.SetMediaType(mediaType);
				sampleGrabber4.SetMediaType(mediaType);
				graphBuilder.FindInterface(FindDirection.UpstreamOnly, Guid.Empty, baseFilter4, typeof (IAMCrossbar).GUID, out retInterface);
				if (retInterface != null)
					amCrossbar = (IAMCrossbar) retInterface;
				this.isCrossbarAvailable = new bool?(amCrossbar != null);
				this.crossbarVideoInputs = this.ColletCrossbarVideoInputs(amCrossbar);
				if (amVideoControl != null)
				{
					graphBuilder.FindPin(this.sourceObject, PinDirection.Output, PinCategory.StillImage, MediaType.Video, false, 0, out pin);
					if (pin != null)
					{
						VideoControlFlags flags;
						amVideoControl.GetCaps(pin, out flags);
						flag = (flags & VideoControlFlags.ExternalTriggerEnable) != 0;
					}
				}
				sampleGrabber3.SetBufferSamples(false);
				sampleGrabber3.SetOneShot(false);
				sampleGrabber3.SetCallback((ISampleGrabberCB) callback1, 1);
				sampleGrabber4.SetBufferSamples(true);
				sampleGrabber4.SetOneShot(false);
				sampleGrabber4.SetCallback((ISampleGrabberCB) callback2, 1);
				this.GetPinCapabilitiesAndConfigureSizeAndRate(graphBuilder, baseFilter4, PinCategory.Capture, this.videoResolution, ref this.videoCapabilities);
				if (flag)
					this.GetPinCapabilitiesAndConfigureSizeAndRate(graphBuilder, baseFilter4, PinCategory.StillImage, this.snapshotResolution, ref this.snapshotCapabilities);
				else
					this.snapshotCapabilities = new AForge.Video.DirectShow.VideoCapabilities[0];
				lock (VideoCaptureDevice.cacheVideoCapabilities)
				{
					if (this.videoCapabilities != null)
					{
						if (!VideoCaptureDevice.cacheVideoCapabilities.ContainsKey(this.deviceMoniker))
							VideoCaptureDevice.cacheVideoCapabilities.Add(this.deviceMoniker, this.videoCapabilities);
					}
				}
				lock (VideoCaptureDevice.cacheSnapshotCapabilities)
				{
					if (this.snapshotCapabilities != null)
					{
						if (!VideoCaptureDevice.cacheSnapshotCapabilities.ContainsKey(this.deviceMoniker))
							VideoCaptureDevice.cacheSnapshotCapabilities.Add(this.deviceMoniker, this.snapshotCapabilities);
					}
				}
				if (runGraph)
				{
					graphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, (object) baseFilter4, (IBaseFilter) null, baseFilter5);
					if (sampleGrabber3.GetConnectedMediaType(mediaType) == 0)
					{
						VideoInfoHeader structure = (VideoInfoHeader) Marshal.PtrToStructure(mediaType.FormatPtr, typeof (VideoInfoHeader));
						callback1.Width = structure.BmiHeader.Width;
						callback1.Height = structure.BmiHeader.Height;
						mediaType.Dispose();
					}
					if (flag && this.provideSnapshots)
					{
						graphBuilder.RenderStream(PinCategory.StillImage, MediaType.Video, (object) baseFilter4, (IBaseFilter) null, baseFilter6);
						if (sampleGrabber4.GetConnectedMediaType(mediaType) == 0)
						{
							VideoInfoHeader structure = (VideoInfoHeader) Marshal.PtrToStructure(mediaType.FormatPtr, typeof (VideoInfoHeader));
							callback2.Width = structure.BmiHeader.Width;
							callback2.Height = structure.BmiHeader.Height;
							mediaType.Dispose();
						}
					}
					IMediaControl mediaControl2 = (IMediaControl) o2;
					IMediaEventEx mediaEventEx2 = (IMediaEventEx) o2;
					mediaControl2.Run();
					if (flag && this.provideSnapshots)
					{
						this.startTime = DateTime.Now;
						amVideoControl.SetMode(pin, VideoControlFlags.ExternalTriggerEnable);
					}
					do
					{
						DsEvCode lEventCode;
						IntPtr lParam1;
						IntPtr lParam2;
						if (mediaEventEx2 != null && mediaEventEx2.GetEvent(out lEventCode, out lParam1, out lParam2, 0) >= 0)
						{
							mediaEventEx2.FreeEventParams(lEventCode, lParam1, lParam2);
							if (lEventCode == DsEvCode.DeviceLost)
							{
								reason = ReasonToFinishPlaying.DeviceLost;
								break;
							}
						}
						if (this.needToSetVideoInput)
						{
							this.needToSetVideoInput = false;
							if (this.isCrossbarAvailable.Value)
							{
								this.SetCurrentCrossbarInput(amCrossbar, this.crossbarVideoInput);
								this.crossbarVideoInput = this.GetCurrentCrossbarInput(amCrossbar);
							}
						}
						if (this.needToSimulateTrigger)
						{
							this.needToSimulateTrigger = false;
							if (flag && this.provideSnapshots)
								amVideoControl.SetMode(pin, VideoControlFlags.Trigger);
						}
						if (this.needToDisplayPropertyPage)
						{
							this.needToDisplayPropertyPage = false;
							this.DisplayPropertyPage(this.parentWindowForPropertyPage, this.sourceObject);
							if (amCrossbar != null)
								this.crossbarVideoInput = this.GetCurrentCrossbarInput(amCrossbar);
						}
						if (this.needToDisplayCrossBarPropertyPage)
						{
							this.needToDisplayCrossBarPropertyPage = false;
							if (amCrossbar != null)
							{
								this.DisplayPropertyPage(this.parentWindowForPropertyPage, (object) amCrossbar);
								this.crossbarVideoInput = this.GetCurrentCrossbarInput(amCrossbar);
							}
						}
					}
					while (!this.stopEvent.WaitOne(100, false));
					mediaControl2.Stop();
				}
			}
			catch (Exception ex)
			{
				if (this.VideoSourceError != null)
					this.VideoSourceError((object) this, new VideoSourceErrorEventArgs(ex.Message));
			}
			finally
			{
				captureGraphBuilder2 = (ICaptureGraphBuilder2) null;
				filterGraph2_1 = (IFilterGraph2) null;
				baseFilter1 = (IBaseFilter) null;
				mediaControl1 = (IMediaControl) null;
				mediaEventEx1 = (IMediaEventEx) null;
				baseFilter2 = (IBaseFilter) null;
				baseFilter3 = (IBaseFilter) null;
				sampleGrabber1 = (ISampleGrabber) null;
				sampleGrabber2 = (ISampleGrabber) null;
				if (o2 != null)
					Marshal.ReleaseComObject(o2);
				if (this.sourceObject != null)
				{
					Marshal.ReleaseComObject(this.sourceObject);
					this.sourceObject = (object) null;
				}
				if (o3 != null)
					Marshal.ReleaseComObject(o3);
				if (o4 != null)
					Marshal.ReleaseComObject(o4);
				if (o1 != null)
					Marshal.ReleaseComObject(o1);
				if (retInterface != null)
					Marshal.ReleaseComObject(retInterface);
			}
			if (this.PlayingFinished == null)
				return;
			this.PlayingFinished((object) this, reason);
		}

		private void SetResolution(IAMStreamConfig streamConfig, AForge.Video.DirectShow.VideoCapabilities resolution)
		{
			if (resolution == (AForge.Video.DirectShow.VideoCapabilities) null)
				return;
			int count = 0;
			int size = 0;
			AMMediaType mediaType = (AMMediaType) null;
			VideoStreamConfigCaps streamConfigCaps = new VideoStreamConfigCaps();
			streamConfig.GetNumberOfCapabilities(out count, out size);
			for (int index = 0; index < count; ++index)
			{
				try
				{
					AForge.Video.DirectShow.VideoCapabilities videoCapabilities = new AForge.Video.DirectShow.VideoCapabilities(streamConfig, index);
					if (resolution == videoCapabilities)
					{
						if (streamConfig.GetStreamCaps(index, out mediaType, streamConfigCaps) == 0)
							break;
					}
				}
				catch
				{
				}
			}
			if (mediaType == null)
				return;
			streamConfig.SetFormat(mediaType);
			mediaType.Dispose();
		}

		private void GetPinCapabilitiesAndConfigureSizeAndRate(
			ICaptureGraphBuilder2 graphBuilder,
			IBaseFilter baseFilter,
			Guid pinCategory,
			AForge.Video.DirectShow.VideoCapabilities resolutionToSet,
			ref AForge.Video.DirectShow.VideoCapabilities[] capabilities)
		{
			object retInterface;
			graphBuilder.FindInterface(pinCategory, MediaType.Video, baseFilter, typeof (IAMStreamConfig).GUID, out retInterface);
			if (retInterface != null)
			{
				IAMStreamConfig amStreamConfig = (IAMStreamConfig) null;
				try
				{
					amStreamConfig = (IAMStreamConfig) retInterface;
				}
				catch (InvalidCastException ex)
				{
				}
				if (amStreamConfig != null)
				{
					if (capabilities == null)
					{
						try
						{
							capabilities = AForge.Video.DirectShow.VideoCapabilities.FromStreamConfig(amStreamConfig);
						}
						catch
						{
						}
					}
					if (resolutionToSet != (AForge.Video.DirectShow.VideoCapabilities) null)
						this.SetResolution(amStreamConfig, resolutionToSet);
				}
			}
			if (capabilities != null)
				return;
			capabilities = new AForge.Video.DirectShow.VideoCapabilities[0];
		}

		private void DisplayPropertyPage(IntPtr parentWindow, object sourceObject)
		{
			try
			{
				CAUUID pPages;
				((ISpecifyPropertyPages) sourceObject).GetPages(out pPages);
				FilterInfo filterInfo = new FilterInfo(this.deviceMoniker);
				Win32.OleCreatePropertyFrame(parentWindow, 0, 0, filterInfo.Name, 1, ref sourceObject, pPages.cElems, pPages.pElems, 0, 0, IntPtr.Zero);
				Marshal.FreeCoTaskMem(pPages.pElems);
			}
			catch
			{
			}
		}

		private VideoInput[] ColletCrossbarVideoInputs(IAMCrossbar crossbar)
		{
			lock (VideoCaptureDevice.cacheCrossbarVideoInputs)
			{
				if (VideoCaptureDevice.cacheCrossbarVideoInputs.ContainsKey(this.deviceMoniker))
					return VideoCaptureDevice.cacheCrossbarVideoInputs[this.deviceMoniker];
				List<VideoInput> videoInputList = new List<VideoInput>();
				int inputPinCount;
				if (crossbar != null && crossbar.get_PinCounts(out int _, out inputPinCount) == 0)
				{
					for (int index = 0; index < inputPinCount; ++index)
					{
						PhysicalConnectorType physicalType;
						if (crossbar.get_CrossbarPinInfo(true, index, out int _, out physicalType) == 0 && physicalType < PhysicalConnectorType.AudioTuner)
							videoInputList.Add(new VideoInput(index, physicalType));
					}
				}
				VideoInput[] array = new VideoInput[videoInputList.Count];
				videoInputList.CopyTo(array);
				VideoCaptureDevice.cacheCrossbarVideoInputs.Add(this.deviceMoniker, array);
				return array;
			}
		}

		private VideoInput GetCurrentCrossbarInput(IAMCrossbar crossbar)
		{
			VideoInput currentCrossbarInput = VideoInput.Default;
			int outputPinCount;
			if (crossbar.get_PinCounts(out outputPinCount, out int _) == 0)
			{
				int outputPinIndex = -1;
				int pinIndexRelated;
				for (int pinIndex = 0; pinIndex < outputPinCount; ++pinIndex)
				{
					PhysicalConnectorType physicalType;
					if (crossbar.get_CrossbarPinInfo(false, pinIndex, out pinIndexRelated, out physicalType) == 0 && physicalType == PhysicalConnectorType.VideoDecoder)
					{
						outputPinIndex = pinIndex;
						break;
					}
				}
				int inputPinIndex;
				if (outputPinIndex != -1 && crossbar.get_IsRoutedTo(outputPinIndex, out inputPinIndex) == 0)
				{
					PhysicalConnectorType physicalType;
					crossbar.get_CrossbarPinInfo(true, inputPinIndex, out pinIndexRelated, out physicalType);
					currentCrossbarInput = new VideoInput(inputPinIndex, physicalType);
				}
			}
			return currentCrossbarInput;
		}

		private void SetCurrentCrossbarInput(IAMCrossbar crossbar, VideoInput videoInput)
		{
			int outputPinCount;
			int inputPinCount;
			if (videoInput.Type == PhysicalConnectorType.Default || crossbar.get_PinCounts(out outputPinCount, out inputPinCount) != 0)
				return;
			int outputPinIndex = -1;
			int inputPinIndex = -1;
			int pinIndexRelated;
			PhysicalConnectorType physicalType;
			for (int pinIndex = 0; pinIndex < outputPinCount; ++pinIndex)
			{
				if (crossbar.get_CrossbarPinInfo(false, pinIndex, out pinIndexRelated, out physicalType) == 0 && physicalType == PhysicalConnectorType.VideoDecoder)
				{
					outputPinIndex = pinIndex;
					break;
				}
			}
			for (int pinIndex = 0; pinIndex < inputPinCount; ++pinIndex)
			{
				if (crossbar.get_CrossbarPinInfo(true, pinIndex, out pinIndexRelated, out physicalType) == 0 && physicalType == videoInput.Type && pinIndex == videoInput.Index)
				{
					inputPinIndex = pinIndex;
					break;
				}
			}
			if (inputPinIndex == -1 || outputPinIndex == -1 || crossbar.CanRoute(outputPinIndex, inputPinIndex) != 0)
				return;
			crossbar.Route(outputPinIndex, inputPinIndex);
		}

		private void OnNewFrame(Bitmap image)
		{
			++this.framesReceived;
			this.bytesReceived += (long) (image.Width * image.Height * (Image.GetPixelFormatSize(image.PixelFormat) >> 3));
			if (this.stopEvent.WaitOne(0, false) || this.NewFrame == null)
				return;
			this.NewFrame((object) this, new NewFrameEventArgs(image));
		}

		private void OnSnapshotFrame(Bitmap image)
		{
			if ((DateTime.Now - this.startTime).TotalSeconds < 4.0 || this.stopEvent.WaitOne(0, false) || this.SnapshotFrame == null)
				return;
			this.SnapshotFrame((object) this, new NewFrameEventArgs(image));
		}

		private class Grabber : ISampleGrabberCB
		{
			private VideoCaptureDevice parent;
			private bool snapshotMode;
			private int width;
			private int height;

			public int Width
			{
				get => this.width;
				set => this.width = value;
			}

			public int Height
			{
				get => this.height;
				set => this.height = value;
			}

			public Grabber(VideoCaptureDevice parent, bool snapshotMode)
			{
				this.parent = parent;
				this.snapshotMode = snapshotMode;
			}

			public int SampleCB(double sampleTime, IntPtr sample) => 0;

			public unsafe int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
			{
				if (this.parent.NewFrame != null)
				{
					Bitmap image = new Bitmap(this.width, this.height, PixelFormat.Format24bppRgb);
					BitmapData bitmapdata = image.LockBits(new Rectangle(0, 0, this.width, this.height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					int stride1 = bitmapdata.Stride;
					int stride2 = bitmapdata.Stride;
					byte* dst = (byte*) ((IntPtr) bitmapdata.Scan0.ToPointer() + stride2 * (this.height - 1));
					byte* pointer = (byte*) buffer.ToPointer();
					for (int index = 0; index < this.height; ++index)
					{
						Win32.memcpy(dst, pointer, stride1);
						dst -= stride2;
						pointer += stride1;
					}
					image.UnlockBits(bitmapdata);
					if (this.snapshotMode)
						this.parent.OnSnapshotFrame(image);
					else
						this.parent.OnNewFrame(image);
					image.Dispose();
				}
				return 0;
			}
		}
	}
}
