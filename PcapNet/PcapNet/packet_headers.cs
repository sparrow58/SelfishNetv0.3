using System;

namespace PcapNet
{
	public class packet_headers
	{
		public timeVal timeval;

		public uint caplen;

		public uint len;

		public packet_headers()
		{
			this.timeval = new timeVal();
		}
	}
}