

using InvokedCommon.Networking;
using System;
using System.Threading;


namespace InvokedCommon.Messages
{
	public abstract class MessageProcessorBase<T> : IMessageProcessor, IProgress<T>
	{
		protected readonly SynchronizationContext SynchronizationContext;
		private readonly SendOrPostCallback _invokeReportProgressHandlers;

		public event MessageProcessorBase<T>.ReportProgressEventHandler ProgressChanged;

		protected virtual void OnReport(T value)
		{
			if (this.ProgressChanged == null)
				return;
			this.SynchronizationContext.Post(this._invokeReportProgressHandlers, (object) value);
		}

		protected MessageProcessorBase(bool useCurrentContext)
		{
			this._invokeReportProgressHandlers = new SendOrPostCallback(this.InvokeReportProgressHandlers);
			this.SynchronizationContext = useCurrentContext ? SynchronizationContext.Current : ProgressStatics.DefaultContext;
		}

		private void InvokeReportProgressHandlers(object state)
		{
			MessageProcessorBase<T>.ReportProgressEventHandler progressChanged = this.ProgressChanged;
			if (progressChanged == null)
				return;
			progressChanged((object) this, (T) state);
		}

		public abstract bool CanExecute(IMessage message);

		public abstract bool CanExecuteFrom(ISender sender);

		public abstract void Execute(ISender sender, IMessage message);

		void IProgress<T>.Report(T value) => this.OnReport(value);

		public delegate void ReportProgressEventHandler(object sender, T value);
	}
}
