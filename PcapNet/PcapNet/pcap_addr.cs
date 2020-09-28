using System;

namespace PcapNet
{
	public class pcap_addr
	{
		public pcap_addr next;

		public sockaddr addr;

		public sockaddr netmask;

		public sockaddr broadaddr;

		public sockaddr dstaddr;

		public pcap_addr()
		{
		}
	}
}