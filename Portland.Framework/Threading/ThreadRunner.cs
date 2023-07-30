using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Portland.Threading
{
	/// <summary>
	/// For use by ThreadPool, only runs one work item on a thread at a time
	/// </summary>
	public sealed class ThreadRunner : IDisposable
	{
		private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

		public Action<ThreadRunner> OnWorkComplete;

		IThreadService _thread;

		Action Work;
		Action WorkOnComplete;
		Action<Exception> WorkOnError;

		public ThreadRunner(bool useSlim)
		{
			if (useSlim)
			{
				_thread = new ThreadServiceSlim(nameof(ThreadWorker), RunServiceOne);
			}
			else
			{
				_thread = new ThreadService(nameof(ThreadWorker), RunServiceOne);
			}
			//_thread.WaitTime = 5000;
			_thread.Start();
		}

		public void EnqueueWork(Action work, Action oncomplete, Action<Exception> onError)
		{
			Debug.Assert(Work == null);

			Volatile.Write(ref Work, work);
			Volatile.Write(ref WorkOnComplete, oncomplete);
			Volatile.Write(ref WorkOnError, onError);

			_thread.SignalRunState();
		}

		void RunServiceOne()
		{
			try
			{
				var onComplete = Volatile.Read(ref WorkOnComplete);
				var work = Volatile.Read(ref Work);
				work();
				Work = null;
				WorkOnComplete = null;

				onComplete?.Invoke();

				OnWorkComplete?.Invoke(this);
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				var onerr = Volatile.Read(ref WorkOnError);
				onerr?.Invoke(ex);
			}
		}

		public void Shutdown()
		{
			_thread.Shutdown();
		}

		public void Dispose()
		{
			_thread.Dispose();
		}
	}
}
