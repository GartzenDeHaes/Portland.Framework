﻿using System;
using System.Diagnostics;
using System.Threading;

namespace Portland.Threading
{
	public sealed class ThreadService : IThreadService
	{
		private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

		private bool _running = false;

		private Action OnSignaled;
		private Action OnStarted;
		private Action OnShutdown;

		private Thread _thread;

		private readonly AutoResetEvent _messageReady = new AutoResetEvent(false);

		public int WaitTime { get; set; } = Timeout.Infinite;

		public bool IsRunning
		{
			get { return _running; }
		}

		//public IThreadPool ThreadPool { get; set; }

		public ThreadService
		(
			string serviceName,
			Action onSignaled,
			Action onStarted = null,
			Action onStopped = null,
			ThreadPriority pri = ThreadPriority.Normal
		)
		{
			//ThreadPool = new NoThreadPool();
			OnStarted = onStarted;
			OnShutdown = onStopped;
			OnSignaled = onSignaled;

			_thread = new Thread(Run);
			_thread.Name = serviceName;
			_thread.IsBackground = true;
			_thread.Priority = pri;
		}

		public void Start()
		{
			Debug.Assert(_running == false);

			_running = true;

			_thread.Start();
		}

		//// override this in child classes
		//protected virtual void OnStarted()
		//{
		//}

		public void Dispose()
		{
			Shutdown();

			_messageReady.Dispose();
		}

		public void Shutdown()
		{
			_running = false;
			WaitTime = 0;

			SignalRunState();
			SignalRunState();
			
			//_thread.Join();
			//_thread = null;
		}

		public void SignalRunState()
		{
			_messageReady.Set();
		}

		private void Run()
		{
			OnStarted?.Invoke();

			while (_running)
			{
				_messageReady.WaitOne(WaitTime);

				if (!_running)
				{
					break;
				}

				try
				{
					OnSignaled();
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
			}

			OnShutdown?.Invoke();
		}

		//protected virtual void RunServiceOne()
		//{
		//	Debug.WriteLine("BackgroundServiceBase.RunServiceOne not overridden");
		//}
	}
}
