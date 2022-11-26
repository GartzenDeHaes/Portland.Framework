using System;
using System.Linq;
using System.Net;

namespace Portland.Net
{
	public static class NetHelper
	{
		public static IPEndPoint ParseEndPoint(string endpointstring)
		{
			return ParseEndPoint(endpointstring, -1);
		}

		public static IPEndPoint ParseEndPoint(string endpointstring, int defaultport)
		{
			if (string.IsNullOrEmpty(endpointstring) || endpointstring.Trim().Length == 0)
			{
				return new IPEndPoint(IPAddress.Any, defaultport);
			}

			if (defaultport != -1 &&
				 (defaultport < IPEndPoint.MinPort
				 || defaultport > IPEndPoint.MaxPort))
			{
				throw new ArgumentException(string.Format("Invalid default port '{0}'", defaultport));
			}

			string[] values = endpointstring.Split(new char[] { ':' });
			IPAddress ipaddy;
			int port;

			//check if we have an IPv6 or ports
			if (values.Length <= 2) // ipv4 or hostname
			{
				if (values.Length == 1)
					//no port is specified, default
					port = defaultport;
				else
					port = ParsePort(values[1]);

				//try to use the address as IPv4, otherwise get hostname
				if (!IPAddress.TryParse(values[0], out ipaddy))
				{
					ipaddy = GetIPfromHost(values[0]);
				}
			}
			else if (values.Length > 2) //ipv6
			{
				//could [a:b:c]:d
				if (values[0].StartsWith("[") && values[values.Length - 2].EndsWith("]"))
				{
					string ipaddressstring = string.Join(":", values.Take(values.Length - 1).ToArray());
					ipaddy = IPAddress.Parse(ipaddressstring);
					port = ParsePort(values[values.Length - 1]);
				}
				else //[a:b:c] or a:b:c
				{
					ipaddy = IPAddress.Parse(endpointstring);
					port = defaultport;
				}
			}
			else
			{
				throw new FormatException(string.Format("Invalid endpoint ipaddress '{0}'", endpointstring));
			}

			if (port == -1)
				throw new ArgumentException(string.Format("No port specified: '{0}'", endpointstring));

			return new IPEndPoint(ipaddy, port);
		}

		private static int ParsePort(string p)
		{
			int port;

			if (!int.TryParse(p, out port)
			 || port < IPEndPoint.MinPort
			 || port > IPEndPoint.MaxPort)
			{
				throw new FormatException(string.Format("Invalid end point port '{0}'", p));
			}

			return port;
		}

		private static IPAddress GetIPfromHost(string p)
		{
			var hosts = Dns.GetHostAddresses(p);

			if (hosts == null || hosts.Length == 0)
				throw new ArgumentException(string.Format("Host not found: {0}", p));

			return hosts[0];
		}
	}
}
