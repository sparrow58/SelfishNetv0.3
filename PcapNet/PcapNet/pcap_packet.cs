using System;

namespace PcapNet
{
	public class pcap_packet
	{
		public packet_headers pkt_hdr;

		public byte[] pkt_data;

		public pcap_packet()
		{
			this.pkt_hdr = new packet_headers();
		}
	}
}