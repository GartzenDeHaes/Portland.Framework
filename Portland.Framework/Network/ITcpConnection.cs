using System;
using System.Net;

using Portland.CheckedEvents;

namespace Portland.Network
{
	public interface ITcpConnection : IDisposable
	{
		bool Connected { get; }

		Notify OnConnecting { get; }
		Notify OnConnected { get; }
		Notify OnConnectFailed { get; }
		Notify OnDisconnected { get; }
		Notify<string> OnError { get; }
		Notify<byte[], int> OnRecv { get; }
		IPAddress RemoteAddress { get; }

		void Close();
		void Connect(string host, int port);
		void PollRecv();
		void Send(byte[] buf, int start, int len);
	}
}
