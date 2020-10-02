using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;

namespace SelfishNet
{
	public class PcList : IDisposable
	{
		private delegateOnNewPC delOnNewPC;

		private delegateOnNewPC delOnPCRemove;

		public ArrayList pclist;
        private bool disposedValue;
        private bool disposedValue1;

        public PcList()
		{
			this.pclist = new ArrayList();
		}

		//private void ~PcList()
		//{
		//}

		public bool addPcToList(PC pc)
		{
			Monitor.Enter(pclist.SyncRoot);
			foreach (PC item in pclist)
			{
				if (item.ip.ToString().CompareTo(pc.ip.ToString()) == 0)
				{
					DateTime now = DateTime.Now;
					item.timeSinceLastRarp = now;
					Monitor.Exit(pclist.SyncRoot);
					return false;
				}
			}
			ArrayList.Synchronized(pclist).Add(pc);
			delOnNewPC?.Invoke(pc);
			Monitor.Exit(pclist.SyncRoot);
			return true;
		}

		//protected virtual void Dispose(bool flag)
		//{
		//	if (!flag)
		//	{
		//		this.Finalize();
		//	}
		//}

		//public sealed override void Dispose()
		//{
		//	this.Dispose(true);
		//	GC.SuppressFinalize(this);
		//}

		public PC getLocalPC()
		{
			Monitor.Enter(this.pclist.SyncRoot);
			IEnumerator enumerator = this.pclist.GetEnumerator();
			if (enumerator.MoveNext())
			{
				while (!((PC)enumerator.Current).isLocalPc)
				{
					if (enumerator.MoveNext())
					{
						continue;
					}
					Monitor.Exit(this.pclist.SyncRoot);
					return null;
				}
				Monitor.Exit(this.pclist.SyncRoot);
				return (PC)enumerator.Current;
			}
			Monitor.Exit(this.pclist.SyncRoot);
			return null;
		}

		public PC getPCFromIP(byte[] ip)
		{
			Monitor.Enter(this.pclist.SyncRoot);
			IEnumerator enumerator = this.pclist.GetEnumerator();
			if (enumerator.MoveNext())
			{
				while (!tools.areValuesEqual(((PC)enumerator.Current).ip.GetAddressBytes(), ip))
				{
					if (enumerator.MoveNext())
					{
						continue;
					}
					Monitor.Exit(this.pclist.SyncRoot);
					return null;
				}
				Monitor.Exit(this.pclist.SyncRoot);
				return (PC)enumerator.Current;
			}
			Monitor.Exit(this.pclist.SyncRoot);
			return null;
		}

		public PC getPCFromMac(byte[] Mac)
		{
			Monitor.Enter(this.pclist.SyncRoot);
			IEnumerator enumerator = this.pclist.GetEnumerator();
			if (enumerator.MoveNext())
			{
				while (!tools.areValuesEqual(((PC)enumerator.Current).mac.GetAddressBytes(), Mac))
				{
					if (enumerator.MoveNext())
					{
						continue;
					}
					Monitor.Exit(this.pclist.SyncRoot);
					return null;
				}
				Monitor.Exit(this.pclist.SyncRoot);
				return (PC)enumerator.Current;
			}
			Monitor.Exit(this.pclist.SyncRoot);
			return null;
		}

		public PC getRouter()
		{
			Monitor.Enter(this.pclist.SyncRoot);
			IEnumerator enumerator = this.pclist.GetEnumerator();
			if (enumerator.MoveNext())
			{
				while (!((PC)enumerator.Current).isGateway)
				{
					if (enumerator.MoveNext())
					{
						continue;
					}
					Monitor.Exit(this.pclist.SyncRoot);
					return null;
				}
				Monitor.Exit(this.pclist.SyncRoot);
				return (PC)enumerator.Current;
			}
			Monitor.Exit(this.pclist.SyncRoot);
			return null;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public bool removePcFromList(PC pc)
		{
			Monitor.Enter(pclist.SyncRoot);
			foreach (PC item in pclist)
			{
				if (item.ip.ToString().CompareTo(pc.ip.ToString()) == 0)
				{
					delOnPCRemove?.Invoke(pc);
					pclist.Remove(pc);
					Monitor.Exit(pclist.SyncRoot);
					return true;
				}
			}
			Monitor.Exit(pclist.SyncRoot);
			return false;
		}

		public void ResetAllPacketsCount()
		{
			Monitor.Enter(this.pclist.SyncRoot);
			IEnumerator enumerator = this.pclist.GetEnumerator();
			if (enumerator.MoveNext())
			{
				do
				{
					((PC)enumerator.Current).nbPacketReceivedSinceLastReset = 0;
					((PC)enumerator.Current).nbPacketSentSinceLastReset = 0;
				}
				while (enumerator.MoveNext());
			}
			Monitor.Exit(this.pclist.SyncRoot);
		}

		public void SetCallBackOnNewPC(delegateOnNewPC callback)
		{
			this.delOnNewPC = callback;
		}

		public void SetCallBackOnPCRemove(delegateOnNewPC callback)
		{
			this.delOnPCRemove = callback;
		}

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue1)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue1 = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PcList()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // TODO: dispose managed state (managed objects)
        //        }

        //        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        //        // TODO: set large fields to null
        //        disposedValue = true;
        //    }
        //}

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PcList()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        //public void Dispose()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}
    }
}