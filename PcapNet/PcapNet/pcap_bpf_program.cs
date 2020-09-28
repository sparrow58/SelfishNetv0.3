using System;

namespace PcapNet
{
	public class pcap_bpf_program
	{
		public uint bf_len;

		public IntPtr bf_insns;

		public pcap_bpf_program()
		{
		}
	}
}