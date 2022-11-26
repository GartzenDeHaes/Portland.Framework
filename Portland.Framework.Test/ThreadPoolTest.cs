//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Threading;
//using System.Diagnostics.CodeAnalysis;

//using NUnit.Framework;

//namespace Portland.Threading
//{
//	[ExcludeFromCodeCoverage]
//	[TestFixture()]
//	public class Test
//	{
//		#region Constants
//		private const int DefaultAssertWait = 5000;
//		#endregion

//		#region Types
//		private delegate bool WaitFunction();
//		#endregion

//		#region Private methods
//		private static bool WaitFor(WaitFunction func, int timeout)
//		{
//			var status = false;
//			var end = DateTime.UtcNow.AddMilliseconds(timeout);

//			while (!(status = func()) && DateTime.UtcNow < end)
//			{
//				Thread.Sleep(5);
//			}

//			return status;
//		}

//		private static void AssertWaitFor(WaitFunction func, int timeout = DefaultAssertWait)
//		{
//			Assert.IsTrue(WaitFor(func, timeout));
//		}
//		#endregion

//		#region Tests
//		[Test()]
//		public void TestProcessorCount()
//		{
//			Assert.AreEqual(Environment.ProcessorCount, ApplicationThreadPool.ProcessorCount);
//			Assert.AreEqual(Environment.ProcessorCount, ApplicationThreadPool.CalculateThreadCount(100));
//			Assert.AreEqual(Math.Max(1, Environment.ProcessorCount / 2), ApplicationThreadPool.CalculateThreadCount(50));
//			Assert.AreEqual(Math.Max(1, Environment.ProcessorCount / 4), ApplicationThreadPool.CalculateThreadCount(25));
//			Assert.AreEqual(1, ApplicationThreadPool.CalculateThreadCount(0));
//		}

//		[Test()]
//		public void TestDefaultThreadPool()
//		{
//			using (var pool = new SystemThreadPool())
//			{
//				Assert.IsTrue(pool.MaxThreads > 0);
//				Assert.IsTrue(pool.AvailableThreads > 0);
//				Assert.IsTrue(pool.AvailableThreads <= pool.MaxThreads);

//				var beforeStartAvailable = pool.AvailableThreads;
//				var startedEvent = new ManualResetEvent(false);
//				var pauseEvent = new ManualResetEvent(false);
//				pool.QueueWorkItem(() => { startedEvent.Set(); pauseEvent.WaitOne(); });

//				startedEvent.WaitOne();
//				var execurtingAvailable = pool.AvailableThreads;
//				pauseEvent.Set();

//				Assert.IsTrue(execurtingAvailable < beforeStartAvailable);
//			}
//		}

//		[Test()]
//		public void TestInitialState()
//		{
//			using (var pool = new ApplicationThreadPool("test", 2, 8, true))
//			{
//				Assert.AreEqual(0, pool.ActiveThreads);
//				Assert.AreEqual(2, pool.MaxThreads);
//				Assert.AreEqual(2, pool.AvailableThreads);
//				Assert.AreEqual(8, pool.MaxQueueLength);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);
//				Assert.AreEqual(0, pool.TotalQueueLength);
//				Assert.AreEqual(0, pool.CompletedItems);
//			}
//		}

//		[Test()]
//		public void TestSingleThread()
//		{
//			using (var pool = new ApplicationThreadPool("test", 2, 8, true))
//			{
//				var finished = false;
//				pool.QueueWorkItem(() => { finished = true; });

//				AssertWaitFor(() => pool.CompletedItems == 1);

//				Assert.IsTrue(finished);
//				Assert.AreEqual(0, pool.ActiveThreads);
//				Assert.AreEqual(2, pool.AvailableThreads);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);
//				Assert.AreEqual(0, pool.TotalQueueLength);
//			}
//		}

//		[Test()]
//		public void TestSingleThreadWithValue()
//		{
//			using (var pool = new ApplicationThreadPool("test", 2, 8, true))
//			{
//				var finished = false;
//				object value = null;
//				pool.QueueWorkItem(() => { value = "Hello world"; finished = true; });

//				AssertWaitFor(() => pool.CompletedItems == 1);

//				Assert.IsTrue(finished);
//				Assert.AreEqual(typeof(string), value.GetType());
//				Assert.AreEqual("Hello world", (string)value);

//				Assert.AreEqual(0, pool.ActiveThreads);
//				Assert.AreEqual(2, pool.AvailableThreads);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);
//				Assert.AreEqual(0, pool.TotalQueueLength);
//			}
//		}

//		[Test()]
//		public void TestFourThreads()
//		{
//			using (var pool = new ApplicationThreadPool("test", 2, 8, true))
//			{
//				var finished = 0;
//				pool.QueueWorkItem(() => { Interlocked.Increment(ref finished); });
//				pool.QueueWorkItem(() => { Interlocked.Increment(ref finished); });
//				pool.QueueWorkItem(() => { Interlocked.Increment(ref finished); });
//				pool.QueueWorkItem(() => { Interlocked.Increment(ref finished); });

//				AssertWaitFor(() => pool.CompletedItems == 4);

//				Assert.AreEqual(4, finished);
//				Assert.AreEqual(0, pool.ActiveThreads);
//				Assert.AreEqual(2, pool.AvailableThreads);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);
//				Assert.AreEqual(0, pool.TotalQueueLength);
//			}
//		}

//		[Test()]
//		public void TestMultipleThreads()
//		{
//			const int maxWorkItems = 1024;
//			using (var pool = new ApplicationThreadPool("test", 2, maxWorkItems, true))
//			{
//				var finished = 0;
//				for (var i = 0; i < maxWorkItems; ++i)
//				{
//					pool.QueueWorkItem(() => { Interlocked.Increment(ref finished); });
//				}

//				AssertWaitFor(() => pool.CompletedItems == maxWorkItems);

//				Assert.AreEqual(maxWorkItems, finished);
//				Assert.AreEqual(0, pool.ActiveThreads);
//				Assert.AreEqual(2, pool.AvailableThreads);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);
//				Assert.AreEqual(0, pool.TotalQueueLength);
//			}
//		}

//		[Test()]
//		public void TestSingleThreadException()
//		{
//			using (var pool = new ApplicationThreadPool("test", 2, 8, true))
//			{
//				var finished = false;
//				pool.QueueWorkItem(() => { finished = true; throw new Exception(); });

//				AssertWaitFor(() => pool.CompletedItems == 1);

//				Assert.IsTrue(finished);
//				Assert.AreEqual(0, pool.ActiveThreads);
//				Assert.AreEqual(2, pool.AvailableThreads);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(1, pool.TotalExceptions);
//				Assert.AreEqual(0, pool.TotalQueueLength);
//			}
//		}

//		[Test()]
//		public void TestFourBlockingThreads()
//		{
//			using (var pool = new ApplicationThreadPool("test", 4, 8, true))
//			{
//				using (var block = new ManualResetEvent(false))
//				{
//					pool.QueueWorkItem(() => { block.WaitOne(); });
//					pool.QueueWorkItem(() => { block.WaitOne(); });
//					pool.QueueWorkItem(() => { block.WaitOne(); });
//					pool.QueueWorkItem(() => { block.WaitOne(); });

//					AssertWaitFor(() => pool.ActiveThreads == 4);
//					Assert.AreEqual(0, pool.AvailableThreads);
//					Assert.AreEqual(0, pool.QueueLength);
//					Assert.AreEqual(0, pool.TotalExceptions);
//					Assert.AreEqual(4, pool.TotalQueueLength);
//					Assert.AreEqual(0, pool.CompletedItems);

//					block.Set();

//					AssertWaitFor(() => pool.ActiveThreads == 0);
//					Assert.AreEqual(4, pool.AvailableThreads);
//					Assert.AreEqual(0, pool.QueueLength);
//					Assert.AreEqual(0, pool.TotalExceptions);
//					Assert.AreEqual(0, pool.TotalQueueLength);
//					Assert.AreEqual(4, pool.CompletedItems);
//				}
//			}
//		}

//		[Test()]
//		public void TestFourBlockingThreadsCountdown()
//		{
//			using (var pool = new ApplicationThreadPool("test", 2, 8, true))
//			{
//				var eventIndex = 0;
//				var activeWorkerEvents = new ConcurrentQueue<int>();

//				var workerEvents = new ManualResetEvent[4];
//				for (var i = 0; i < workerEvents.Length; ++i)
//				{
//					workerEvents[i] = new ManualResetEvent(false);
//				}

//				for (var i = 0; i < workerEvents.Length; ++i)
//				{
//					int n = i;

//					pool.QueueWorkItem(() => {
//						activeWorkerEvents.Enqueue(n); 
//						workerEvents[n].WaitOne(); 
//					});
//				}

//				AssertWaitFor(() => pool.ActiveThreads == 2);
//				Assert.AreEqual(0, pool.AvailableThreads);
//				//Assert.AreEqual(2, pool.QueueLength);
//				Assert.AreEqual(4, pool.TotalQueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);
//				Assert.AreEqual(0, pool.CompletedItems);

//				//AssertWaitFor(() => activeWorkerEvents.Count == 4);
//				Assert.IsTrue(activeWorkerEvents.TryDequeue(out eventIndex));
//				workerEvents[eventIndex].Set();

//				AssertWaitFor(() => pool.CompletedItems == 1);
//				AssertWaitFor(() => pool.ActiveThreads == 2);
//				Assert.AreEqual(0, pool.AvailableThreads);
//				Assert.AreEqual(1, pool.CompletedItems);
//				Assert.AreEqual(1, pool.QueueLength);
//				Assert.AreEqual(3, pool.TotalQueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);

//				AssertWaitFor(() => activeWorkerEvents.Count > 0);
//				Assert.IsTrue(activeWorkerEvents.TryDequeue(out eventIndex));
//				workerEvents[eventIndex].Set();

//				AssertWaitFor(() => pool.CompletedItems == 2);
//				AssertWaitFor(() => pool.ActiveThreads == 2);
//				Assert.AreEqual(0, pool.AvailableThreads);
//				Assert.AreEqual(2, pool.CompletedItems);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(2, pool.TotalQueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);

//				AssertWaitFor(() => activeWorkerEvents.Count > 0);
//				Assert.IsTrue(activeWorkerEvents.TryDequeue(out eventIndex));
//				workerEvents[eventIndex].Set();

//				AssertWaitFor(() => pool.CompletedItems == 3);
//				AssertWaitFor(() => pool.ActiveThreads == 1);
//				Assert.AreEqual(1, pool.AvailableThreads);
//				Assert.AreEqual(3, pool.CompletedItems);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(1, pool.TotalQueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);

//				AssertWaitFor(() => activeWorkerEvents.Count > 0);
//				Assert.IsTrue(activeWorkerEvents.TryDequeue(out eventIndex));
//				workerEvents[eventIndex].Set();

//				AssertWaitFor(() => pool.CompletedItems == 4);
//				Assert.AreEqual(0, pool.ActiveThreads);
//				Assert.AreEqual(2, pool.AvailableThreads);
//				Assert.AreEqual(4, pool.CompletedItems);
//				Assert.AreEqual(0, pool.QueueLength);
//				Assert.AreEqual(0, pool.TotalQueueLength);
//				Assert.AreEqual(0, pool.TotalExceptions);
//			}
//		}
//		#endregion
//	}
//}
