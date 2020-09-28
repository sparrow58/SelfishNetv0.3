using u003cCppImplementationDetailsu003e;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace PcapNet
{
	public class CPcapNet : IDisposable
	{
		public IntPtr nicHandle;

		public CPcapNet()
		{
		}

		protected virtual void Dispose(bool flag)
		{
			if (!flag)
			{
				//this.Finalize();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public unsafe ArrayList getAllDevsID()
		{
			pcap_if* pcapIfPointer = null;
			$ArrayType$$$BY0BAA@D _u0024ArrayTypeu0024u0024u0024BY0BAAu0040D = new $ArrayType$$$BY0BAA@D();
			<Module>.pcap_findalldevs(ref pcapIfPointer, ref _u0024ArrayTypeu0024u0024u0024BY0BAAu0040D);
			pcap_if* pcapIfPointer1 = pcapIfPointer;
			ArrayList arrayLists = new ArrayList();
			if (pcapIfPointer1 != null)
			{
				do
				{
					arrayLists.Add(new string((int)(*(pcapIfPointer1 + 4))));
					pcapIfPointer1 = (pcap_if*)((int)(*pcapIfPointer1));
				}
				while (pcapIfPointer1 != null);
			}
			return arrayLists;
		}

		public void pcapnet_close()
		{
			<Module>.pcap_close(this.nicHandle.ToPointer());
		}

		public unsafe int pcapnet_findalldevs(out pcapnet_if pcap_int, out string errbuf)
		{
			pcap_int = null;
			errbuf = null;
			pcap_if* pcapIfPointer = null;
			$ArrayType$$$BY0BAA@D _u0024ArrayTypeu0024u0024u0024BY0BAAu0040D = new $ArrayType$$$BY0BAA@D();
			int num = <Module>.pcap_findalldevs(ref pcapIfPointer, ref _u0024ArrayTypeu0024u0024u0024BY0BAAu0040D);
			if (num == 0)
			{
				/* modopt(System.Runtime.CompilerServices.IsConst) */ pcapnet_if pcapnetIf = new pcapnet_if();
				pcap_int = pcapnetIf;
				pcapnet_if str = pcapnetIf;
				if ((IntPtr)pcapIfPointer != IntPtr.Zero)
				{
					do
					{
						str.name = new string((int)(*(pcapIfPointer + 4)));
						str.description = new string((int)(*(pcapIfPointer + 8)));
						str.flags = (uint)(*(pcapIfPointer + 16));
						/* modopt(System.Runtime.CompilerServices.IsConst) */ pcap_addr pcapAddr = new pcap_addr();
						str.addresses = pcapAddr;
						pcap_addr _sockaddr = pcapAddr;
						if ((IntPtr)((int)(*(pcapIfPointer + 12))) != IntPtr.Zero)
						{
							do
							{
								_sockaddr.addr = new sockaddr();
								IntPtr intPtr = (IntPtr)(*((int)(*(pcapIfPointer + 12)) + 4));
								_sockaddr.addr = (sockaddr)Marshal.PtrToStructure(intPtr, typeof(sockaddr));
								IntPtr intPtr1 = (IntPtr)(*((int)(*(pcapIfPointer + 12)) + 12));
								_sockaddr.broadaddr = (sockaddr)Marshal.PtrToStructure(intPtr1, typeof(sockaddr));
								IntPtr intPtr2 = (IntPtr)(*((int)(*(pcapIfPointer + 12)) + 16));
								_sockaddr.dstaddr = (sockaddr)Marshal.PtrToStructure(intPtr2, typeof(sockaddr));
								IntPtr intPtr3 = (IntPtr)(*((int)(*(pcapIfPointer + 12)) + 8));
								_sockaddr.netmask = (sockaddr)Marshal.PtrToStructure(intPtr3, typeof(sockaddr));
								if ((IntPtr)(*(int)(*(pcapIfPointer + 12))) != IntPtr.Zero)
								{
									_sockaddr.next = new pcap_addr();
								}
								_sockaddr = _sockaddr.next;
								pcap_if* pcapIfPointer1 = pcapIfPointer + 12;
								*pcapIfPointer1 = *(int)(*pcapIfPointer1);
							}
							while ((IntPtr)((int)(*(pcapIfPointer + 12))) != IntPtr.Zero);
						}
						if ((IntPtr)((int)(*pcapIfPointer)) != IntPtr.Zero)
						{
							str.next = new pcapnet_if();
						}
						str = str.next;
						pcapIfPointer = (pcap_if*)((int)(*pcapIfPointer));
					}
					while ((IntPtr)pcapIfPointer != IntPtr.Zero);
				}
			}
			return num;
		}

		public void pcapnet_freeAlldevs(IntPtr nics)
		{
			<Module>.pcap_freealldevs(nics.ToPointer());
		}

		public unsafe int pcapnet_next_ex(out byte[] packet)
		{
			packet = 0;
			byte* numPointer = null;
			pcap_pkthdr* pcapPkthdrPointer = null;
			int num = <Module>.pcap_next_ex(this.nicHandle.ToPointer(), ref pcapPkthdrPointer, ref numPointer);
			if (num == 1)
			{
				packet = new byte[(int)(*(pcapPkthdrPointer + 8))];
				IntPtr intPtr = new IntPtr(numPointer);
				byte[] numArray = packet;
				Marshal.Copy(intPtr, numArray, 0, numArray.Length);
			}
			return num;
		}

		public unsafe int pcapnet_next_ex(out pcap_packet packet)
		{
			packet = null;
			pcap_pkthdr* pcapPkthdrPointer = null;
			byte* numPointer = null;
			int num = <Module>.pcap_next_ex(this.nicHandle.ToPointer(), ref numPointer, ref pcapPkthdrPointer);
			if (num == 1 && (IntPtr)pcapPkthdrPointer != IntPtr.Zero && (IntPtr)numPointer != IntPtr.Zero)
			{
				packet = new pcap_packet();
				IntPtr intPtr = (IntPtr)numPointer;
				packet.pkt_hdr = (packet_headers)Marshal.PtrToStructure(intPtr, typeof(packet_headers));
				pcap_packet pcapPacket = packet;
				pcapPacket.pkt_data = new byte[pcapPacket.pkt_hdr.caplen];
				pcap_packet pcapPacket1 = packet;
				Marshal.Copy((IntPtr)pcapPkthdrPointer, pcapPacket1.pkt_data, 0, (int)pcapPacket1.pkt_hdr.caplen);
			}
			return num;
		}

		public unsafe int pcapnet_next_ex(out packet_headers pkt_hdr, out byte[] pkt_data)
		{
			pkt_hdr = null;
			pkt_data = 0;
			void* voidPointer = null;
			void* voidPointer1 = null;
			int num = <Module>.pcap_next_ex(this.nicHandle.ToPointer(), ref voidPointer1, ref voidPointer);
			if (num == 1 && (IntPtr)voidPointer1 != IntPtr.Zero && (IntPtr)voidPointer != IntPtr.Zero)
			{
				pkt_hdr = new packet_headers();
				packet_headers structure = (packet_headers)Marshal.PtrToStructure((IntPtr)voidPointer1, typeof(packet_headers));
				pkt_hdr = structure;
				pkt_data = new byte[structure.caplen];
				Marshal.Copy((IntPtr)voidPointer, pkt_data, 0, (int)pkt_hdr.caplen);
			}
			return num;
		}

		public bool pcapnet_openLive(string name, int sizeofpacket, int options, int timeout, string err)
		{
			$ArrayType$$$BY0BAA@D _u0024ArrayTypeu0024u0024u0024BY0BAAu0040D = new $ArrayType$$$BY0BAA@D();
			if (!name.StartsWith("\\Device"))
			{
				name = string.Concat("\\Device\\NPF_", name);
			}
			IntPtr hGlobalAnsi = Marshal.StringToHGlobalAnsi(name);
			IntPtr intPtr = (IntPtr)<Module>.pcap_open_live(hGlobalAnsi.ToPointer(), sizeofpacket, options, timeout, ref _u0024ArrayTypeu0024u0024u0024BY0BAAu0040D);
			this.nicHandle = intPtr;
			return (this.nicHandle == IntPtr.Zero ? false : true);
		}

		public unsafe int pcapnet_sendpacket(byte[] packet)
		{
			byte* numPointer = (byte*)<Module>.@new((uint)packet.Length);
			IntPtr intPtr = new IntPtr(numPointer);
			Marshal.Copy(packet, 0, intPtr, packet.Length);
			/* modopt(System.Runtime.CompilerServices.CallConvCdecl) */ int int32u0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eCallConvCdeclu0029 = <Module>.pcap_sendpacket(this.nicHandle.ToPointer(), numPointer, packet.Length);
			<Module>.delete(numPointer);
			return int32u0020modoptu0028Systemu002eRuntimeu002eCompilerServicesu002eCallConvCdeclu0029;
		}

		public unsafe int pcapnet_setFilter(string filter, uint netmask)
		{
			bpf_program bpfProgram = new bpf_program();
			/* modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte* pointer = (/* modopt(System.Runtime.CompilerServices.IsSignUnspecifiedByte) */ sbyte*)Marshal.StringToHGlobalAnsi(filter).ToPointer();
			if (<Module>.pcap_compile(this.nicHandle.ToPointer(), ref bpfProgram, pointer, 1, netmask) < 0)
			{
				return -1;
			}
			return (<Module>.pcap_setfilter(this.nicHandle.ToPointer(), ref bpfProgram) < 0 ? -2 : 0);
		}
	}
}