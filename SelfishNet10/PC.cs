using System;
using System.Net;
using System.Net.NetworkInformation;

namespace SelfishNet
{
	public class PC
	{
		public IPAddress ip;

		public PhysicalAddress mac;

		public string name;

		public bool isGateway;

		public bool isLocalPc;

		public int CapDown { set; get; }

		public int CapUp { set; get; }

		public bool redirect;

		public /* modopt(System.Runtime.CompilerServices.IsLong) */ int totalPacketSent;

		public /* modopt(System.Runtime.CompilerServices.IsLong) */ int totalPacketReceived;

		public int nbPacketSentSinceLastReset;

		public int nbPacketReceivedSinceLastReset;

		public /* modopt(System.Runtime.CompilerServices.IsBoxed), modopt(System.DateTime) */ ValueType timeSinceLastRarp;

		public PC()
		{
		}
	}
}