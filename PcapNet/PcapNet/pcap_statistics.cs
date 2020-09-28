using System;

namespace PcapNet
{
	public class pcap_statistics
	{
		public uint ps_recv;

		public uint ps_drop;

		public uint ps_ifdrop;

		public uint bs_capt;

		public pcap_statistics()
		{
		}
	}
}