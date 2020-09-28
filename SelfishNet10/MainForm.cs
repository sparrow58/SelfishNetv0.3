using AdvancedDataGridView;
using PcapNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SelfishNet10
{
    public partial class MainForm : Form
	{
		public delegate void delegateOnNewPC(PC pc);

		public delegate void DelUpdateName(PC pc, string str);
		public Driver driver;

		public PcList pcs;

		public CArp cArp;

		public CAdapter cAdapter;

		public byte[] routerIP;

		public object[] resolvState;

		public NetworkInterface nicNet;
		public static MainForm instance;

		public MainForm()
		{
			try
			{
				this.InitializeComponent();
                //this.imageList1.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
                //this.imageList1.Images.SetKeyName(0, "pc.png");
                //this.imageList1.Images.SetKeyName(1, "Vista-network_local.png");
                this.timerStatCount = 0;
				this.driver = new Driver();
				instance = this;
			}
			catch
			{

			}
		}

		//private void ArpForm()
		//{
		//	IDisposable disposable = this.cArp;
		//	if (disposable != null)
		//	{
		//		disposable.Dispose();
		//	}
		//	IContainer container = this.components;
		//	if (container != null)
		//	{
		//		container.Dispose();
		//	}
		//}

		private void AddPc(PC pc)
		{
			TreeGridNode treeGridNode;
			if (pc.isGateway)
			{
				this.treeGridView1.Nodes[0].Cells[1].Value = pc.ip.ToString();
				this.treeGridView1.Nodes[0].Cells[2].Value = pc.mac.ToString();
				this.treeGridView1.Nodes[0].Cells[5].ReadOnly = true;
				this.treeGridView1.Nodes[0].Cells[6].ReadOnly = true;
				this.treeGridView1.Nodes[0].Cells[7].ReadOnly = true;
				this.treeGridView1.Nodes[0].Cells[8].ReadOnly = true;
				this.treeGridView1.Nodes[0].Cells[5].Value = 0;
				this.treeGridView1.Nodes[0].Cells[6].Value = 0;
				this.treeGridView1.Nodes[0].Cells[7].ReadOnly = true;
				this.treeGridView1.Nodes[0].Cells[8].ReadOnly = true;
			}
			else if (!pc.isLocalPc)
			{
				object[] objArray = new object[] { "", pc.ip, pc.mac.ToString(), "", "", 0, 0, false, true };
				treeGridNode = this.treeGridView1.Nodes[0].Nodes.Add(objArray);
				treeGridNode.ImageIndex = 0;
				treeGridNode.Cells[5].ReadOnly = false;
				treeGridNode.Cells[6].ReadOnly = false;
			}
			else
			{
				object[] objArray1 = new object[] { "Your PC", pc.ip, pc.mac.ToString() };
				treeGridNode = this.treeGridView1.Nodes[0].Nodes.Add(objArray1);
				treeGridNode.ImageIndex = 0;
				treeGridNode.Cells[5].Value = 0;
				treeGridNode.Cells[6].Value = 0;
				treeGridNode.Cells[5].ReadOnly = true;
				treeGridNode.Cells[6].ReadOnly = true;
				treeGridNode.Cells[7].Value = false;
				treeGridNode.Cells[8].Value = false;
				treeGridNode.Cells[7].ReadOnly = true;
				treeGridNode.Cells[8].ReadOnly = true;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (MessageBox.Show(this, "Quit the application ?", "Quit", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		private unsafe void MainForm_Load(object sender, EventArgs e)
		{
			IntPtr hGlobalAnsi = Marshal.StringToHGlobalAnsi("npf");
			if (driver.openDeviceDriver((sbyte*)hGlobalAnsi) != null)
			{
				MessageBox.Show("Driver WinPcap already installed");
			}
			else if (!File.Exists("license.txt"))
			{
				this.licenseAccepted();
			}
			else
			{
				CWizard cWizard = new CWizard();
				cWizard.Show(this);
				Decoder decoder = Encoding.UTF7.GetDecoder();
				FileStream fileStream = File.OpenRead("license.txt");
				byte[] numArray = new byte[(int)fileStream.Length];
				fileStream.Read(numArray, 0, (int)fileStream.Length);
				char[] chrArray = new char[decoder.GetCharCount(numArray, 0, numArray.Length)];
				decoder.GetChars(numArray, 0, numArray.Length, chrArray, 0);
				cWizard.richTextBox1.Text = new string(chrArray);
				fileStream.Close();
			}
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (base.WindowState == FormWindowState.Minimized)
			{
				base.Visible = false;
				this.SelfishNetTrayIcon.Visible = true;
				this.SelfishNetTrayIcon.ShowBalloonTip(2000);
			}
		}

		private void callbackOnNewPC(PC pc)
		{
			object[] objArray = new object[] { pc };
			Invoke(new delegateOnNewPC(AddPc), objArray);
			Dns.BeginGetHostEntry(pc.ip.ToString(), new AsyncCallback(this.EndResolvCallBack), pc);
		}

		private void callbackOnPCRemove(PC pc)
		{
			int num = 1;
			if (1 < this.treeGridView1.Nodes[0].Nodes.Count)
			{
				while (this.treeGridView1.Nodes[0].Nodes[num].Cells[1].Value.ToString().CompareTo(pc.ip.ToString()) != 0)
				{
					num++;
					if (num < this.treeGridView1.Nodes[0].Nodes.Count)
					{
						continue;
					}
					return;
				}
				this.treeGridView1.Nodes[0].Nodes.RemoveAt(num);
			}
		}
		private void downloadCapToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			this.ColDownCap.Visible = this.ViewMenuDownloadCap.Checked;
		}

		private void downloadCapToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void EndResolvCallBack(IAsyncResult re)
		{
			string hostName = null;
			PC asyncState = (PC)re.AsyncState;
			try
			{
				hostName = Dns.EndGetHostEntry(re).HostName;
			}
			catch (Exception exception)
			{
			}
			if (hostName == null)
			{
				hostName = "noname";
			}
			object[] objArray = new object[2];
			this.resolvState = objArray;
			objArray[0] = asyncState;
			this.resolvState[1] = hostName;
			Invoke(new DelUpdateName(updateTreeViewNameCallBack), this.resolvState);
		}

		public void licenseAccepted()
		{
			if (driver.create())
			{
				/* modopt(System.Runtime.CompilerServices.IsConst) */
				CAdapter cAdapter = new CAdapter();
				this.cAdapter = cAdapter;
				cAdapter.show(this);
			}
			else
			{
				MessageBox.Show("problem installing the drivers, do you have administrator privileges?");
				if (this != null)
				{
					((IDisposable)this).Dispose();
				}
			}
		}

		public void NicIsSelected(NetworkInterface nic)
		{
			pcs = new PcList();
			pcs.SetCallBackOnNewPC(callbackOnNewPC);
			pcs.SetCallBackOnPCRemove(callbackOnPCRemove);
			nicNet = nic;
			/* modopt(System.Runtime.CompilerServices.IsConst) */
			CArp cArp = new CArp(nic, this.pcs);
			this.cArp = cArp;
			cArp.startArpListener();
			this.cArp.findMacRouter();
			PC pC = new PC()
			{
				ip = new IPAddress(this.cArp.localIP),
				mac = new PhysicalAddress(this.cArp.localMAC),
				capDown = 0,
				capUp = 0,
				isLocalPc = true,
				name = "",
				nbPacketReceivedSinceLastReset = 0,
				nbPacketSentSinceLastReset = 0,
				redirect = false,
				timeSinceLastRarp = DateTime.Now,
				totalPacketReceived = 0,
				totalPacketSent = 0,
				isGateway = false
			};
			this.pcs.addPcToList(pC);
			this.timer2.Interval = 5000;
			this.timer2.Start();
			this.treeGridView1.Nodes[0].Expand();

			
		}

		private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			base.Visible = true;
			base.WindowState = FormWindowState.Normal;
			this.SelfishNetTrayIcon.Visible = false;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.timerStatCount++;
			for (int i = 0; i < this.treeGridView1.Nodes[0].Nodes.Count; i++)
			{
				try
				{
					IPAddress ipAddress = tools.getIpAddress(this.treeGridView1.Nodes[0].Nodes[i].Cells[1].Value.ToString());
					PC pCFromIP = pcs.getPCFromIP(ipAddress.GetAddressBytes());
					double num = (float)pCFromIP.nbPacketReceivedSinceLastReset * 0.0009765625;
					float interval = (float)(num / (float)(timer1.Interval / 1000) / (float)timerStatCount);
					string str = interval.ToString();
					double num1 = (float)pCFromIP.nbPacketSentSinceLastReset * 0.0009765625;
					float single = (float)(num1 / (float)(timer1.Interval / 1000) / (float)timerStatCount);
					string str1 = single.ToString();
					if (str.Length - str.IndexOf(".") > 1)
					{
						int num2 = -2 - str.IndexOf(".");
						string str2 = str;
						str = str2.Remove(str2.IndexOf(".") + 1, str.Length + num2);
					}
					if (str1.Length - str1.IndexOf(".") > 1)
					{
						int num3 = -2 - str1.IndexOf(".");
						string str3 = str1;
						str1 = str3.Remove(str3.IndexOf(".") + 1, str1.Length + num3);
					}
					this.treeGridView1.Nodes[0].Nodes[i].Cells[3].Value = str;
					this.treeGridView1.Nodes[0].Nodes[i].Cells[4].Value = str1;
				}
				catch (Exception)
				{
				}
			}
			this.pcs.ResetAllPacketsCount();
			this.timerStatCount = 0;
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			int num = 0;
			if (0 < this.treeGridView1.Nodes[0].Nodes.Count)
			{
				do
				{
					IPAddress ipAddress = tools.getIpAddress(this.treeGridView1.Nodes[0].Nodes[num].Cells[1].Value.ToString());
					PC pCFromIP = this.pcs.getPCFromIP(ipAddress.GetAddressBytes());
					if (pCFromIP != null && !pCFromIP.isGateway && !pCFromIP.isLocalPc && DateTime.Now.Ticks - ((DateTime)pCFromIP.timeSinceLastRarp).Ticks > 3500000000L)
					{
						this.pcs.removePcFromList(pCFromIP);
						num = 0;
					}
					num++;
				}
				while (num < this.treeGridView1.Nodes[0].Nodes.Count);
			}
			int num1 = 0;
			if (0 < this.treeGridView1.Nodes[0].Nodes.Count)
			{
				do
				{
					IPAddress pAddress = tools.getIpAddress(this.treeGridView1.Nodes[0].Nodes[num1].Cells[1].Value.ToString());
					PC pC = this.pcs.getPCFromIP(pAddress.GetAddressBytes());
					if (pC != null && DateTime.Now.Ticks - ((DateTime)pC.timeSinceLastRarp).Ticks > (long)200000000)
					{
						this.cArp.findMac(pC.ip.ToString());
					}
					num1++;
				}
				while (num1 < this.treeGridView1.Nodes[0].Nodes.Count);
			}
		}

		private void timerSpoof_Tick(object sender, EventArgs e)
		{
			this.timerSpoof.Interval = 5000;
			int num = 0;
			if (0 < this.treeGridView1.Nodes[0].Nodes.Count)
			{
				do
				{
					if (this.treeGridView1.Nodes[0].Nodes[num].Cells[8].Value.ToString().CompareTo("True") == 0)
					{
						IPAddress ipAddress = tools.getIpAddress(this.treeGridView1.Nodes[0].Nodes[num].Cells[1].Value.ToString());
						PC pCFromIP = this.pcs.getPCFromIP(ipAddress.GetAddressBytes());
						if (!pCFromIP.isLocalPc)
						{
							this.cArp.Spoof(pCFromIP.ip, new IPAddress(this.cArp.routerIP));
						}
					}
					num++;
				}
				while (num < this.treeGridView1.Nodes[0].Nodes.Count);
			}
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			this.cArp.startArpDiscovery();
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			if (!this.toolStripButton2.Checked)
			{
				this.cArp.startRedirector();
				this.toolStripButton2.Checked = true;
				this.timer1.Interval = 1000;
				this.timer1.Start();
				this.timerSpoof.Start();
				this.timerSpoof.Interval = 1000;
				this.toolStripButton2.Checked = true;
				this.toolStripButton2.Enabled = false;
				this.timerDiscovery.Start();
			}
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			if (this.toolStripButton2.Checked)
			{
				this.cArp.stopRedirector();
				this.cArp.completeUnspoof();
				this.timer1.Stop();
				this.timerSpoof.Stop();
				int num = 0;
				if (0 < this.treeGridView1.Nodes[0].Nodes.Count)
				{
					do
					{
						this.treeGridView1.Nodes[0].Nodes[num].Cells[3].Value = "";
						this.treeGridView1.Nodes[0].Nodes[num].Cells[4].Value = "";
						num++;
					}
					while (num < this.treeGridView1.Nodes[0].Nodes.Count);
				}
				this.toolStripButton2.Checked = false;
				this.toolStripButton2.Enabled = true;
				this.timerDiscovery.Stop();
			}
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			if (!File.Exists("hlpindex.html"))
			{
				MessageBox.Show("help file is missing !");
			}
			else
			{
				Process.Start("hlpindex.html");
			}
		}

		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Quit ?", "Quit", MessageBoxButtons.YesNo) == DialogResult.Yes && this != null)
			{
				((IDisposable)this).Dispose();
			}
		}

		private void treeGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
		}

		private void treeGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if (e.ColumnIndex == 5 && e.RowIndex >= 2)
				{
					IPAddress ipAddress = tools.getIpAddress(this.treeGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
					if (this.treeGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().CompareTo("") != 0)
					{
						PC pCFromIP = this.pcs.getPCFromIP(ipAddress.GetAddressBytes());
						if (pCFromIP != null)
						{
							Monitor.Enter(pCFromIP);
							pCFromIP.capDown = Convert.ToInt32(this.treeGridView1.Rows[e.RowIndex].Cells[5].Value) * 1024;
							Monitor.Exit(pCFromIP);
						}
					}
				}
				if (e.ColumnIndex == 6 && e.RowIndex >= 2)
				{
					IPAddress pAddress = tools.getIpAddress(this.treeGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
					if (this.treeGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().CompareTo("") != 0)
					{
						PC num = this.pcs.getPCFromIP(pAddress.GetAddressBytes());
						if (num != null)
						{
							Monitor.Enter(num);
							num.capUp = Convert.ToInt32(this.treeGridView1.Rows[e.RowIndex].Cells[6].Value) * 1024;
							Monitor.Exit(num);
						}
					}
				}
				if (e.ColumnIndex == 7 && e.RowIndex >= 2)
				{
					IPAddress ipAddress1 = tools.getIpAddress(this.treeGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
					if (this.treeGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().CompareTo("") != 0)
					{
						PC pC = this.pcs.getPCFromIP(ipAddress1.GetAddressBytes());
						if (pC != null)
						{
							Monitor.Enter(pC);
							pC.redirect = !pC.redirect;
							Monitor.Exit(pC);
						}
					}
				}
				if (e.ColumnIndex == 8 && e.RowIndex >= 2 && this.treeGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().CompareTo("") != 0)
				{
					IPAddress pAddress1 = tools.getIpAddress(this.treeGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
					if (this.treeGridView1.Rows[e.RowIndex].Cells[8].Value.ToString().CompareTo("False") == 0)
					{
						for (int i = 0; i < 35; i++)
						{
							this.cArp.UnSpoof(pAddress1, new IPAddress(this.cArp.routerIP));
						}
					}
				}
			}
			catch (Exception exception)
			{
			}
		}

		private void treeGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (this.treeGridView1.CurrentCell.ColumnIndex >= 7 || this.treeGridView1.CurrentCell.ColumnIndex >= 8 && this.treeGridView1.IsCurrentCellDirty)
			{
				this.treeGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}

		private void updateTreeViewNameCallBack(PC pc, string str)
		{
			if (!pc.isGateway)
			{
				int num = 1;
				if (1 < this.treeGridView1.Nodes[0].Nodes.Count)
				{
					while (this.treeGridView1.Nodes[0].Nodes[num].Cells[1].Value.ToString().CompareTo(pc.ip.ToString()) != 0)
					{
						num++;
						if (num < this.treeGridView1.Nodes[0].Nodes.Count)
						{
							continue;
						}
						return;
					}
					this.treeGridView1.Nodes[0].Nodes[num].Cells[0].Value = str;
				}
			}
			else
			{
				this.treeGridView1.Nodes[0].Cells[0].Value = str;
				this.treeGridView1.Nodes[0].ImageIndex = 1;
			}
		}

		private void uploadCapToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			this.ColUploadCap.Visible = this.ViewMenuUploadCap.Checked;
		}

		private void ViewMenuBlock_CheckStateChanged(object sender, EventArgs e)
		{
			this.ColBlock.Visible = this.ViewMenuBlock.Checked;
		}

		private void ViewMenuDownload_CheckStateChanged(object sender, EventArgs e)
		{
			this.ColDownload.Visible = this.ViewMenuDownload.Checked;
		}

		private void ViewMenuIP_CheckStateChanged(object sender, EventArgs e)
		{
			this.ColPCIP.Visible = this.ViewMenuIP.Checked;
		}

		private void ViewMenuMAC_CheckStateChanged(object sender, EventArgs e)
		{
			this.ColPCMac.Visible = this.ViewMenuMAC.Checked;
		}

		private void ViewMenuSpoof_CheckStateChanged(object sender, EventArgs e)
		{
			this.ColSpoof.Visible = this.ViewMenuSpoof.Checked;
		}

		private void ViewMenuUpload_CheckStateChanged(object sender, EventArgs e)
		{
			this.ColUpload.Visible = this.ViewMenuUpload.Checked;
		}
	}
}