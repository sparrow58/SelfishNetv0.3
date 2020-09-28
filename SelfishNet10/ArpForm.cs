using AdvancedDataGridView;
using DataGridViewNumericUpDownElements;
using PcapNet;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SelfishNet10
{
	public class ArpForm : Form
	{
		public Driver driver;

		public PcList pcs;

		public CArp cArp;

		public CAdapter cAdapter;

		public byte[] routerIP;

		public object[] resolvState;

		public NetworkInterface nicNet;

		public static ArpForm instance;

		private ToolStripMenuItem ViewMenuDownloadCap;

		private ToolStripMenuItem ViewMenuUploadCap;

		private ToolStripButton toolStripButton4;

		private TreeGridColumn ColPCName;

		private DataGridViewTextBoxColumn ColPCIP;

		private DataGridViewTextBoxColumn ColPCMac;

		private DataGridViewTextBoxColumn ColDownload;

		private DataGridViewTextBoxColumn ColUpload;

		private DataGridViewNumericUpDownColumn ColDownCap;

		private DataGridViewNumericUpDownColumn ColUploadCap;

		private DataGridViewCheckBoxColumn ColBlock;

		private DataGridViewCheckBoxColumn ColSpoof;

		private System.Windows.Forms.Timer timerDiscovery;

		public int timerStatCount;

		private System.Windows.Forms.Timer timer1;

		private System.Windows.Forms.Timer timer2;

		private System.Windows.Forms.Timer timerSpoof;

		private ToolStrip toolStrip1;

		private ToolStripButton toolStripButton1;

		private ToolStripButton toolStripButton2;

		private ToolStripButton toolStripButton3;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripSeparator toolStripSeparator2;

		private TreeGridView treeGridView1;

		private System.Windows.Forms.ContextMenuStrip ContextMenuViews;

		private ToolStripMenuItem ViewMenuIP;

		private ToolStripMenuItem ViewMenuMAC;

		private ToolStripMenuItem ViewMenuDownload;

		private ToolStripMenuItem ViewMenuUpload;

		private ToolStripMenuItem ViewMenuBlock;

		private ToolStripMenuItem ViewMenuSpoof;

		private ImageList imageList1;

		private System.Windows.Forms.Timer timer3;

		private NotifyIcon SelfishNetTrayIcon;

		private IContainer components;

		static ArpForm()
		{
		}

		public ArpForm()
		{
			try
			{
				this.InitializeComponent();
				this.timerStatCount = 0;
				this.driver = new Driver();
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

		private void ArpForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (MessageBox.Show(this, "Quit the application ?", "Quit", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		private unsafe void ArpForm_Load(object sender, EventArgs e)
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

		private void ArpForm_Resize(object sender, EventArgs e)
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
			ArpForm arpForm = this;
			arpForm.Invoke(new ArpForm.delegateOnNewPC(arpForm.AddPc), objArray);
			Dns.BeginResolve(pc.ip.ToString(), new AsyncCallback(this.EndResolvCallBack), pc);
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

		protected override void Dispose(bool flag)
		{
			if (!flag)
			{
				base.Dispose(false);
			}
			else
			{
				try
				{
					//this.ArpForm();
				}
				finally
				{
					base.Dispose(true);
				}
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
				hostName = Dns.EndResolve(re).HostName;
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
			ArpForm arpForm = this;
			arpForm.Invoke(new ArpForm.DelUpdateName(arpForm.updateTreeViewNameCallBack), this.resolvState);
		}

		public void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ArpForm));
			TreeGridNode treeGridNode = new TreeGridNode();
			this.toolStrip1 = new ToolStrip();
			this.toolStripButton1 = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.toolStripButton2 = new ToolStripButton();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.toolStripButton3 = new ToolStripButton();
			this.toolStripButton4 = new ToolStripButton();
			this.treeGridView1 = new TreeGridView();
			this.ColPCName = new TreeGridColumn();
			this.ColPCIP = new DataGridViewTextBoxColumn();
			this.ColPCMac = new DataGridViewTextBoxColumn();
			this.ColDownload = new DataGridViewTextBoxColumn();
			this.ColUpload = new DataGridViewTextBoxColumn();
			this.ColDownCap = new DataGridViewNumericUpDownColumn();
			this.ColUploadCap = new DataGridViewNumericUpDownColumn();
			this.ColBlock = new DataGridViewCheckBoxColumn();
			this.ColSpoof = new DataGridViewCheckBoxColumn();
			ArpForm contextMenuStrip = this;
			contextMenuStrip.ContextMenuViews = new System.Windows.Forms.ContextMenuStrip(contextMenuStrip.components);
			this.ViewMenuIP = new ToolStripMenuItem();
			this.ViewMenuMAC = new ToolStripMenuItem();
			this.ViewMenuDownload = new ToolStripMenuItem();
			this.ViewMenuUpload = new ToolStripMenuItem();
			this.ViewMenuDownloadCap = new ToolStripMenuItem();
			this.ViewMenuUploadCap = new ToolStripMenuItem();
			this.ViewMenuBlock = new ToolStripMenuItem();
			this.ViewMenuSpoof = new ToolStripMenuItem();
			ArpForm imageList = this;
			imageList.imageList1 = new ImageList(imageList.components);
			ArpForm timer = this;
			timer.timer1 = new System.Windows.Forms.Timer(timer.components);
			ArpForm arpForm = this;
			arpForm.timer2 = new System.Windows.Forms.Timer(arpForm.components);
			ArpForm timer1 = this;
			timer1.timerSpoof = new System.Windows.Forms.Timer(timer1.components);
			ArpForm notifyIcon = this;
			notifyIcon.SelfishNetTrayIcon = new NotifyIcon(notifyIcon.components);
			ArpForm arpForm1 = this;
			arpForm1.timerDiscovery = new System.Windows.Forms.Timer(arpForm1.components);
			this.toolStrip1.SuspendLayout();
			((ISupportInitialize)this.treeGridView1).BeginInit();
			this.ContextMenuViews.SuspendLayout();
			base.SuspendLayout();
			System.Drawing.Size size = new System.Drawing.Size(32, 32);
			this.toolStrip1.ImageScalingSize = size;
			ToolStripItem[] toolStripItemArray = new ToolStripItem[] { this.toolStripButton1, this.toolStripSeparator1, this.toolStripButton2, this.toolStripSeparator2, this.toolStripButton3, this.toolStripButton4 };
			this.toolStrip1.Items.AddRange(toolStripItemArray);
			Point point = new Point(0, 0);
			this.toolStrip1.Location = point;
			this.toolStrip1.Name = "toolStrip1";
			System.Drawing.Size size1 = new System.Drawing.Size(703, 39);
			this.toolStrip1.Size = size1;
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = (Image)componentResourceManager.GetObject("toolStripButton1.Image");
			Color magenta = Color.Magenta;
			this.toolStripButton1.ImageTransparentColor = magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			System.Drawing.Size size2 = new System.Drawing.Size(36, 36);
			this.toolStripButton1.Size = size2;
			this.toolStripButton1.Text = "Network Discovery";
			this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			System.Drawing.Size size3 = new System.Drawing.Size(6, 39);
			this.toolStripSeparator1.Size = size3;
			this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Image = (Image)componentResourceManager.GetObject("toolStripButton2.Image");
			Color color = Color.Magenta;
			this.toolStripButton2.ImageTransparentColor = color;
			this.toolStripButton2.Name = "toolStripButton2";
			System.Drawing.Size size4 = new System.Drawing.Size(36, 36);
			this.toolStripButton2.Size = size4;
			this.toolStripButton2.Text = "Start redirecting-spoofing";
			this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			System.Drawing.Size size5 = new System.Drawing.Size(6, 39);
			this.toolStripSeparator2.Size = size5;
			this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Image = (Image)componentResourceManager.GetObject("toolStripButton3.Image");
			Color magenta1 = Color.Magenta;
			this.toolStripButton3.ImageTransparentColor = magenta1;
			this.toolStripButton3.Name = "toolStripButton3";
			System.Drawing.Size size6 = new System.Drawing.Size(36, 36);
			this.toolStripButton3.Size = size6;
			this.toolStripButton3.Text = "stop redirecting- spoofing";
			this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
			this.toolStripButton4.Alignment = ToolStripItemAlignment.Right;
			this.toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.toolStripButton4.Image = (Image)componentResourceManager.GetObject("toolStripButton4.Image");
			Color color1 = Color.Magenta;
			this.toolStripButton4.ImageTransparentColor = color1;
			this.toolStripButton4.Name = "toolStripButton4";
			System.Drawing.Size size7 = new System.Drawing.Size(36, 36);
			this.toolStripButton4.Size = size7;
			this.toolStripButton4.Text = "Help";
			this.toolStripButton4.Click += new EventHandler(this.toolStripButton4_Click);
			this.treeGridView1.AllowUserToAddRows = false;
			this.treeGridView1.AllowUserToDeleteRows = false;
			this.treeGridView1.AllowUserToOrderColumns = true;
			this.treeGridView1.AllowUserToResizeRows = false;
			this.treeGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.treeGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			this.treeGridView1.BorderStyle = BorderStyle.Fixed3D;
			this.treeGridView1.CellBorderStyle = DataGridViewCellBorderStyle.RaisedHorizontal;
			this.treeGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			this.treeGridView1.ColumnHeadersHeight = 35;
			DataGridViewColumn[] colPCName = new DataGridViewColumn[] { this.ColPCName, this.ColPCIP, this.ColPCMac, this.ColDownload, this.ColUpload, this.ColDownCap, this.ColUploadCap, this.ColBlock, this.ColSpoof };
			this.treeGridView1.Columns.AddRange(colPCName);
			this.treeGridView1.ContextMenuStrip = this.ContextMenuViews;
			this.treeGridView1.Dock = DockStyle.Fill;
			this.treeGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
			this.treeGridView1.ImageList = this.imageList1;
			Point point1 = new Point(0, 39);
			this.treeGridView1.Location = point1;
			this.treeGridView1.Name = "treeGridView1";
			treeGridNode.Height = 19;
			treeGridNode.ImageIndex = 1;
			this.treeGridView1.Nodes.Add(treeGridNode);
			this.treeGridView1.RowHeadersVisible = false;
			this.treeGridView1.ShowCellErrors = false;
			this.treeGridView1.ShowCellToolTips = false;
			this.treeGridView1.ShowEditingIcon = false;
			this.treeGridView1.ShowRowErrors = false;
			System.Drawing.Size size8 = new System.Drawing.Size(703, 363);
			this.treeGridView1.Size = size8;
			this.treeGridView1.TabIndex = 1;
			this.treeGridView1.CellPainting += new DataGridViewCellPaintingEventHandler(this.treeGridView1_CellPainting);
			this.treeGridView1.CurrentCellDirtyStateChanged += new EventHandler(this.treeGridView1_CurrentCellDirtyStateChanged);
			this.treeGridView1.CellValueChanged += new DataGridViewCellEventHandler(this.treeGridView1_CellValueChanged);
			this.ColPCName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.ColPCName.DefaultNodeImage = null;
			this.ColPCName.FillWeight = 180.4366f;
			this.ColPCName.HeaderText = "PC Name";
			this.ColPCName.MinimumWidth = 40;
			this.ColPCName.Name = "ColPCName";
			this.ColPCName.ReadOnly = true;
			this.ColPCName.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.ColPCIP.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.ColPCIP.FillWeight = 119.7174f;
			this.ColPCIP.HeaderText = "IP";
			this.ColPCIP.MinimumWidth = 35;
			this.ColPCIP.Name = "ColPCIP";
			this.ColPCIP.ReadOnly = true;
			this.ColPCIP.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.ColPCMac.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.ColPCMac.FillWeight = 106.599f;
			this.ColPCMac.HeaderText = "MAC";
			this.ColPCMac.MinimumWidth = 35;
			this.ColPCMac.Name = "ColPCMac";
			this.ColPCMac.ReadOnly = true;
			this.ColPCMac.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.ColDownload.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.ColDownload.FillWeight = 74.57431f;
			this.ColDownload.HeaderText = "Download KB/s";
			this.ColDownload.MinimumWidth = 20;
			this.ColDownload.Name = "ColDownload";
			this.ColDownload.ReadOnly = true;
			this.ColDownload.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.ColUpload.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.ColUpload.FillWeight = 70.42757f;
			this.ColUpload.HeaderText = "Upload KB/s";
			this.ColUpload.MinimumWidth = 20;
			this.ColUpload.Name = "ColUpload";
			this.ColUpload.ReadOnly = true;
			this.ColUpload.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.ColDownCap.HeaderText = "Download Cap";
			int[] numArray = new int[] { 10000, 0, 0, 0 };
			decimal num = new decimal(numArray);
			this.ColDownCap.Maximum = num;
			this.ColDownCap.Name = "ColDownCap";
			this.ColDownCap.ThousandsSeparator = true;
			this.ColUploadCap.HeaderText = "Upload Cap";
			int[] numArray1 = new int[] { 10000, 0, 0, 0 };
			decimal num1 = new decimal(numArray1);
			this.ColUploadCap.Maximum = num1;
			this.ColUploadCap.Name = "ColUploadCap";
			this.ColUploadCap.ThousandsSeparator = true;
			this.ColBlock.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.ColBlock.FalseValue = "False";
			this.ColBlock.FillWeight = 48.2451f;
			this.ColBlock.HeaderText = "Block";
			this.ColBlock.MinimumWidth = 10;
			this.ColBlock.Name = "ColBlock";
			this.ColBlock.Resizable = DataGridViewTriState.True;
			this.ColBlock.TrueValue = "True";
			this.ColSpoof.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.ColSpoof.FalseValue = "False";
			this.ColSpoof.HeaderText = "Spoof";
			this.ColSpoof.MinimumWidth = 10;
			this.ColSpoof.Name = "ColSpoof";
			this.ColSpoof.Resizable = DataGridViewTriState.True;
			this.ColSpoof.TrueValue = "True";
			this.ColSpoof.Visible = false;
			ToolStripItem[] viewMenuIP = new ToolStripItem[] { this.ViewMenuIP, this.ViewMenuMAC, this.ViewMenuDownload, this.ViewMenuUpload, this.ViewMenuDownloadCap, this.ViewMenuUploadCap, this.ViewMenuBlock, this.ViewMenuSpoof };
			this.ContextMenuViews.Items.AddRange(viewMenuIP);
			this.ContextMenuViews.Name = "ContextMenuViews";
			System.Drawing.Size size9 = new System.Drawing.Size(155, 180);
			this.ContextMenuViews.Size = size9;
			this.ContextMenuViews.Text = "Columns Views";
			this.ViewMenuIP.Checked = true;
			this.ViewMenuIP.CheckOnClick = true;
			this.ViewMenuIP.CheckState = CheckState.Checked;
			this.ViewMenuIP.Name = "ViewMenuIP";
			System.Drawing.Size size10 = new System.Drawing.Size(154, 22);
			this.ViewMenuIP.Size = size10;
			this.ViewMenuIP.Text = "IP";
			this.ViewMenuIP.CheckStateChanged += new EventHandler(this.ViewMenuIP_CheckStateChanged);
			this.ViewMenuMAC.Checked = true;
			this.ViewMenuMAC.CheckOnClick = true;
			this.ViewMenuMAC.CheckState = CheckState.Checked;
			this.ViewMenuMAC.Name = "ViewMenuMAC";
			System.Drawing.Size size11 = new System.Drawing.Size(154, 22);
			this.ViewMenuMAC.Size = size11;
			this.ViewMenuMAC.Text = "MAC";
			this.ViewMenuMAC.CheckStateChanged += new EventHandler(this.ViewMenuMAC_CheckStateChanged);
			this.ViewMenuDownload.Checked = true;
			this.ViewMenuDownload.CheckOnClick = true;
			this.ViewMenuDownload.CheckState = CheckState.Checked;
			this.ViewMenuDownload.Name = "ViewMenuDownload";
			System.Drawing.Size size12 = new System.Drawing.Size(154, 22);
			this.ViewMenuDownload.Size = size12;
			this.ViewMenuDownload.Text = "Download";
			this.ViewMenuDownload.CheckStateChanged += new EventHandler(this.ViewMenuDownload_CheckStateChanged);
			this.ViewMenuUpload.Checked = true;
			this.ViewMenuUpload.CheckOnClick = true;
			this.ViewMenuUpload.CheckState = CheckState.Checked;
			this.ViewMenuUpload.Name = "ViewMenuUpload";
			System.Drawing.Size size13 = new System.Drawing.Size(154, 22);
			this.ViewMenuUpload.Size = size13;
			this.ViewMenuUpload.Text = "Upload";
			this.ViewMenuUpload.CheckStateChanged += new EventHandler(this.ViewMenuUpload_CheckStateChanged);
			this.ViewMenuDownloadCap.Checked = true;
			this.ViewMenuDownloadCap.CheckOnClick = true;
			this.ViewMenuDownloadCap.CheckState = CheckState.Checked;
			this.ViewMenuDownloadCap.Name = "ViewMenuDownloadCap";
			System.Drawing.Size size14 = new System.Drawing.Size(154, 22);
			this.ViewMenuDownloadCap.Size = size14;
			this.ViewMenuDownloadCap.Text = "Download Cap";
			this.ViewMenuDownloadCap.CheckStateChanged += new EventHandler(this.downloadCapToolStripMenuItem_CheckStateChanged);
			this.ViewMenuDownloadCap.Click += new EventHandler(this.downloadCapToolStripMenuItem_Click);
			this.ViewMenuUploadCap.Checked = true;
			this.ViewMenuUploadCap.CheckState = CheckState.Checked;
			this.ViewMenuUploadCap.Name = "ViewMenuUploadCap";
			System.Drawing.Size size15 = new System.Drawing.Size(154, 22);
			this.ViewMenuUploadCap.Size = size15;
			this.ViewMenuUploadCap.Text = "Upload Cap";
			this.ViewMenuUploadCap.CheckStateChanged += new EventHandler(this.uploadCapToolStripMenuItem_CheckStateChanged);
			this.ViewMenuBlock.Checked = true;
			this.ViewMenuBlock.CheckOnClick = true;
			this.ViewMenuBlock.CheckState = CheckState.Checked;
			this.ViewMenuBlock.Name = "ViewMenuBlock";
			System.Drawing.Size size16 = new System.Drawing.Size(154, 22);
			this.ViewMenuBlock.Size = size16;
			this.ViewMenuBlock.Text = "Block";
			this.ViewMenuBlock.CheckStateChanged += new EventHandler(this.ViewMenuBlock_CheckStateChanged);
			this.ViewMenuSpoof.CheckOnClick = true;
			this.ViewMenuSpoof.Name = "ViewMenuSpoof";
			System.Drawing.Size size17 = new System.Drawing.Size(154, 22);
			this.ViewMenuSpoof.Size = size17;
			this.ViewMenuSpoof.Text = "Spoofed";
			this.ViewMenuSpoof.CheckStateChanged += new EventHandler(this.ViewMenuSpoof_CheckStateChanged);
			this.imageList1.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
			Color white = Color.White;
			this.imageList1.TransparentColor = white;
			this.imageList1.Images.SetKeyName(0, "pc.png");
			this.imageList1.Images.SetKeyName(1, "Vista-network_local.png");
			this.timer1.Tick += new EventHandler(this.timer1_Tick);
			this.timer2.Tick += new EventHandler(this.timer2_Tick);
			this.timerSpoof.Tick += new EventHandler(this.timerSpoof_Tick);
			this.SelfishNetTrayIcon.BalloonTipIcon = ToolTipIcon.Info;
			this.SelfishNetTrayIcon.BalloonTipText = "SelfishNet is minimized";
			this.SelfishNetTrayIcon.BalloonTipTitle = "SelfishNet";
			this.SelfishNetTrayIcon.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("SelfishNetTrayIcon.Icon");
			this.SelfishNetTrayIcon.Text = "SelfishNet";
			this.SelfishNetTrayIcon.MouseDoubleClick += new MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
			this.timerDiscovery.Interval = 600000;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(703, 402);
			base.Controls.Add(this.treeGridView1);
			base.Controls.Add(this.toolStrip1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.MaximizeBox = false;
			base.Name = "ArpForm";
			this.Text = "SelfishNet v0.1 Beta";
			ArpForm arpForm2 = this;
			arpForm2.Resize += new EventHandler(arpForm2.ArpForm_Resize);
			ArpForm arpForm3 = this;
			arpForm3.FormClosing += new FormClosingEventHandler(arpForm3.ArpForm_FormClosing);
			ArpForm arpForm4 = this;
			arpForm4.Load += new EventHandler(arpForm4.ArpForm_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((ISupportInitialize)this.treeGridView1).EndInit();
			this.ContextMenuViews.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
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
			this.pcs = new PcList();
			this.pcs.SetCallBackOnNewPC(new delegateOnNewPC(callbackOnNewPC));
			this.pcs.SetCallBackOnPCRemove(new delegateOnNewPC(callbackOnPCRemove));
			this.nicNet = nic;
			/* modopt(System.Runtime.CompilerServices.IsConst) */ CArp cArp = new CArp(nic, this.pcs);
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
			if (MessageBox.Show(this, "Quit ?", "Quit", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes && this != null)
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

		public delegate void delegateOnNewPC(PC pc);

		public delegate void DelUpdateName(PC pc, string str);
	}
}