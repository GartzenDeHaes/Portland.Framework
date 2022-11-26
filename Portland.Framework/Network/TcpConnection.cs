using System;
using System.Net;
using System.Net.Sockets;

using Portland.CheckedEvents;

namespace Portland.Network
{
	public class TcpConnection : ITcpConnection
	{
		public enum Mode
		{
			BeginEnd,
			Poll
		}

		private TcpClient _sock;
		private byte[] _buf = new byte[2048];

		public bool Connected { get; private set; }

		public Notify OnConnecting { get; protected set; }
		public Notify OnConnected { get; protected set; }
		public Notify OnConnectFailed { get; protected set; }
		public Notify OnDisconnected { get; protected set; }
		public Notify<string> OnError { get; protected set; }
		public Notify<byte[], int> OnRecv { get; protected set; }
		public IPAddress RemoteAddress { get { return ((IPEndPoint)_sock.Client.RemoteEndPoint).Address; } }

		private Mode _mode;

		public TcpConnection(Mode mode)
		{
			_mode = mode;

			OnConnecting = new Notify();
			OnConnected = new Notify();
			OnConnectFailed = new Notify();
			OnDisconnected = new Notify();
			OnError = new Notify<string>();
			OnRecv = new Notify<byte[], int>();
		}

		public TcpConnection(Mode mode, TcpClient acceptedSock)
		{
			_mode = mode;
			_sock = acceptedSock;

			OnConnected.Send();

			if (_mode == Mode.BeginEnd)
			{
				ReceiveData();
			}
		}

		public void Connect(string host, int port)
		{
			Connected = false;

			_sock = new TcpClient(AddressFamily.InterNetworkV6);
			_sock.NoDelay = true;
			_sock.Client.NoDelay = true;
			_sock.Client.DualMode = true;

			try
			{
				OnConnecting.Send();
				_sock.BeginConnect(host, port, EndConnect, null);
			}
			catch (Exception ex)
			{
				OnError.Send($"TcpBehavior.Connect {ex.Message} {ex.StackTrace}");
				OnConnectFailed.Send();
			}
		}

		public void PollRecv()
		{
			if (Connected && _sock.Connected && _sock.Available > 0)
			{
				int size = _sock.Available > _buf.Length ? _buf.Length : _sock.Available;
				int len = _sock.Client.Receive(_buf, size, SocketFlags.None);

				if (len > 0)
				{
					byte[] bufc = new byte[len];
					Array.Copy(_buf, bufc, len);
					OnRecv.Send(bufc, len);
				}
				else
				{
					OnError.Send($"TcpBehavior.EndReceive: {len} length data packet");
				}
			}
		}

		void EndConnect(IAsyncResult result)
		{
			try
			{
				_sock.Client.EndConnect(result);

				Connected = true;

				OnConnected.Send();

				if (_mode == Mode.BeginEnd)
				{
					ReceiveData();
				}
			}
			catch (Exception ex)
			{
				OnError.Send($"TcpBehavior.EndConnect {ex.Message} {ex.StackTrace}");
			}
		}

		public void Close()
		{
			if (Connected)
			{
				Connected = false;
				OnDisconnected.Send();

				_sock.Close();
				_sock.Dispose();
			}

			_sock = null;
		}

		public void Send(byte[] buf, int start, int len)
		{
			if (_mode == Mode.BeginEnd)
			{
				_sock.Client.BeginSend(buf, start, len, SocketFlags.None, EndSend, null);
			}
			else
			{
				_sock.Client.Send(buf, start, len, SocketFlags.None);
			}
		}

		void EndSend(IAsyncResult result)
		{
			try
			{
				int len = _sock.Client.EndSend(result);
				if (len < 1)
				{
					OnError.Send($"TcpBehavior.Send returned {len}");
				}
			}
			catch (Exception ex)
			{
				OnError.Send($"TcpBehavior.EndSend {ex.Message} {ex.StackTrace}");
			}
		}

		void ReceiveData()
		{
			if (!Connected)
			{
				return;
			}
			try
			{
				_sock.Client.BeginReceive(_buf, 0, _buf.Length, SocketFlags.None, EndReceive, null);
			}
			catch (Exception ex)
			{
				OnError.Send($"TcpBehavior.ReceiveData {ex.Message} {ex.StackTrace}");
			}
		}

		void EndReceive(IAsyncResult result)
		{
			if (!Connected)
			{
				return;
			}
			try
			{
				// TODO: pool

				int len = _sock.Client.EndReceive(result);
				if (len > 0)
				{
					byte[] bufc = new byte[len];
					Array.Copy(_buf, bufc, len);
					OnRecv.Send(bufc, len);
				}
				else
				{
					OnError.Send($"TcpBehavior.EndReceive: {len} length data packet");
					Close();
				}
			}
			catch (Exception ex)
			{
				OnError.Send($"TcpBehavior.EndReceive {ex.Message} {ex.StackTrace}");
				Close();
			}

			ReceiveData();
		}

		public void Dispose()
		{
			Close();
		}
	}
}
