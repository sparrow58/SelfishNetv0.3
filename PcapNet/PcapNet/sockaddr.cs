using System;

namespace PcapNet
{
	public class sockaddr
	{
		public uint sa_family;

		public byte[] sa_data;

		public sockaddr()
		{
		}
	}
}