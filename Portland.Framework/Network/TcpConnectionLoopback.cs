using System;
using System.Net;

using Portland.CheckedEvents;

namespace Portland.Network
{
	public class TcpConnectionLoopback : ITcpConnection
	{
		public bool Connected { get; private set; }

		public Notify OnConnecting { get; private set; } = new Notify();
		public Notify OnConnected { get; private set; } = new Notify();
		public Notify OnConnectFailed { get; private set; } = new Notify();
		public Notify OnDisconnected { get; private set; } = new Notify();
		public Notify<string> OnError { get; private set; } = new Notify<string>();
		public Notify<byte[], int> OnRecv { get; private set; } = new Notify<byte[], int>();

		public IPAddress RemoteAddress { get { return IPAddress.Loopback; } }

		public Action<byte[], int, int> OnSend;

		public void Close()
		{
			Dispose();
		}

		public void Connect(string host, int port)
		{
			OnConnecting.Send();
			Connected = true;
			OnConnected.Send();
		}

		public void Dispose()
		{
			if (Connected)
			{
				Connected = false;
				OnDisconnected.Send();
			}
		}

		public void PollRecv()
		{
		}

		public void Send(byte[] buf, int start, int len)
		{
			OnSend?.Invoke(buf, start, len);
		}
	}
}
