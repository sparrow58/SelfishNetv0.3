using System;

namespace PcapNet
{
	public class pcapnet_if
	{
		public pcapnet_if next;

		public string name;

		public string description;

		public pcap_addr addresses;

		public uint flags;

		public pcapnet_if()
		{
		}
	}
}