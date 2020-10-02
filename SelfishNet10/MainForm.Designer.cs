using AdvancedDataGridView;
using DataGridViewNumericUpDownElements;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SelfishNet
{
    partial class MainForm
    {
		/// <summary>
		/// Required designer variable.
		/// </summary>
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

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		public void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            AdvancedDataGridView.TreeGridNode treeGridNode1 = new AdvancedDataGridView.TreeGridNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.treeGridView1 = new AdvancedDataGridView.TreeGridView();
            this.ColPCName = new AdvancedDataGridView.TreeGridColumn();
            this.ColPCIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColPCMac = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDownload = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColUpload = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDownCap = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.ColUploadCap = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.ColBlock = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColSpoof = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ContextMenuViews = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ViewMenuIP = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenuMAC = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenuDownload = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenuUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenuDownloadCap = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenuUploadCap = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenuBlock = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenuSpoof = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timerSpoof = new System.Windows.Forms.Timer(this.components);
            this.SelfishNetTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.timerDiscovery = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).BeginInit();
            this.ContextMenuViews.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2,
            this.toolStripSeparator2,
            this.toolStripButton3,
            this.toolStripButton4});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(703, 39);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Properties.Resources.search;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton1.Text = "Network Discovery";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Properties.Resources.play;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton2.Text = "Start redirecting-spoofing";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::Properties.Resources.pause;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton3.Text = "stop redirecting- spoofing";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 36);
            this.toolStripButton4.Text = "Help";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // treeGridView1
            // 
            this.treeGridView1.AllowUserToAddRows = false;
            this.treeGridView1.AllowUserToDeleteRows = false;
            this.treeGridView1.AllowUserToOrderColumns = true;
            this.treeGridView1.AllowUserToResizeRows = false;
            this.treeGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.treeGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.treeGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.treeGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.RaisedHorizontal;
            this.treeGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.treeGridView1.ColumnHeadersHeight = 35;
            this.treeGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColPCName,
            this.ColPCIP,
            this.ColPCMac,
            this.ColDownload,
            this.ColUpload,
            this.ColDownCap,
            this.ColUploadCap,
            this.ColBlock,
            this.ColSpoof});
            this.treeGridView1.ContextMenuStrip = this.ContextMenuViews;
            this.treeGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.treeGridView1.ImageList = this.imageList1;
            this.treeGridView1.Location = new System.Drawing.Point(0, 39);
            this.treeGridView1.Name = "treeGridView1";
            treeGridNode1.Height = 20;
            treeGridNode1.ImageIndex = 1;
            this.treeGridView1.Nodes.Add(treeGridNode1);
            this.treeGridView1.RowHeadersVisible = false;
            this.treeGridView1.ShowCellErrors = false;
            this.treeGridView1.ShowCellToolTips = false;
            this.treeGridView1.ShowEditingIcon = false;
            this.treeGridView1.ShowRowErrors = false;
            this.treeGridView1.Size = new System.Drawing.Size(703, 363);
            this.treeGridView1.TabIndex = 1;
            this.treeGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.treeGridView1_CellPainting);
            this.treeGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.treeGridView1_CellValueChanged);
            this.treeGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.treeGridView1_CurrentCellDirtyStateChanged);
            // 
            // ColPCName
            // 
            this.ColPCName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColPCName.DefaultNodeImage = null;
            this.ColPCName.FillWeight = 180.4366F;
            this.ColPCName.HeaderText = "PC Name";
            this.ColPCName.MinimumWidth = 40;
            this.ColPCName.Name = "ColPCName";
            this.ColPCName.ReadOnly = true;
            this.ColPCName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColPCIP
            // 
            this.ColPCIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColPCIP.FillWeight = 119.7174F;
            this.ColPCIP.HeaderText = "IP";
            this.ColPCIP.MinimumWidth = 35;
            this.ColPCIP.Name = "ColPCIP";
            this.ColPCIP.ReadOnly = true;
            this.ColPCIP.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColPCMac
            // 
            this.ColPCMac.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColPCMac.FillWeight = 106.599F;
            this.ColPCMac.HeaderText = "MAC";
            this.ColPCMac.MinimumWidth = 35;
            this.ColPCMac.Name = "ColPCMac";
            this.ColPCMac.ReadOnly = true;
            this.ColPCMac.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColDownload
            // 
            this.ColDownload.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColDownload.FillWeight = 74.57431F;
            this.ColDownload.HeaderText = "Download KB/s";
            this.ColDownload.MinimumWidth = 20;
            this.ColDownload.Name = "ColDownload";
            this.ColDownload.ReadOnly = true;
            this.ColDownload.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColUpload
            // 
            this.ColUpload.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColUpload.FillWeight = 70.42757F;
            this.ColUpload.HeaderText = "Upload KB/s";
            this.ColUpload.MinimumWidth = 20;
            this.ColUpload.Name = "ColUpload";
            this.ColUpload.ReadOnly = true;
            this.ColUpload.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColDownCap
            // 
            this.ColDownCap.HeaderText = "Download Cap";
            this.ColDownCap.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ColDownCap.Name = "ColDownCap";
            this.ColDownCap.ThousandsSeparator = true;
            // 
            // ColUploadCap
            // 
            this.ColUploadCap.HeaderText = "Upload Cap";
            this.ColUploadCap.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ColUploadCap.Name = "ColUploadCap";
            this.ColUploadCap.ThousandsSeparator = true;
            // 
            // ColBlock
            // 
            this.ColBlock.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColBlock.FalseValue = "False";
            this.ColBlock.FillWeight = 48.2451F;
            this.ColBlock.HeaderText = "Block";
            this.ColBlock.MinimumWidth = 10;
            this.ColBlock.Name = "ColBlock";
            this.ColBlock.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColBlock.TrueValue = "True";
            // 
            // ColSpoof
            // 
            this.ColSpoof.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColSpoof.FalseValue = "False";
            this.ColSpoof.HeaderText = "Spoof";
            this.ColSpoof.MinimumWidth = 10;
            this.ColSpoof.Name = "ColSpoof";
            this.ColSpoof.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColSpoof.TrueValue = "True";
            this.ColSpoof.Visible = false;
            // 
            // ContextMenuViews
            // 
            this.ContextMenuViews.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewMenuIP,
            this.ViewMenuMAC,
            this.ViewMenuDownload,
            this.ViewMenuUpload,
            this.ViewMenuDownloadCap,
            this.ViewMenuUploadCap,
            this.ViewMenuBlock,
            this.ViewMenuSpoof});
            this.ContextMenuViews.Name = "ContextMenuViews";
            this.ContextMenuViews.Size = new System.Drawing.Size(153, 180);
            this.ContextMenuViews.Text = "Columns Views";
            // 
            // ViewMenuIP
            // 
            this.ViewMenuIP.Checked = true;
            this.ViewMenuIP.CheckOnClick = true;
            this.ViewMenuIP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewMenuIP.Name = "ViewMenuIP";
            this.ViewMenuIP.Size = new System.Drawing.Size(152, 22);
            this.ViewMenuIP.Text = "IP";
            this.ViewMenuIP.CheckStateChanged += new System.EventHandler(this.ViewMenuIP_CheckStateChanged);
            // 
            // ViewMenuMAC
            // 
            this.ViewMenuMAC.Checked = true;
            this.ViewMenuMAC.CheckOnClick = true;
            this.ViewMenuMAC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewMenuMAC.Name = "ViewMenuMAC";
            this.ViewMenuMAC.Size = new System.Drawing.Size(152, 22);
            this.ViewMenuMAC.Text = "MAC";
            this.ViewMenuMAC.CheckStateChanged += new System.EventHandler(this.ViewMenuMAC_CheckStateChanged);
            // 
            // ViewMenuDownload
            // 
            this.ViewMenuDownload.Checked = true;
            this.ViewMenuDownload.CheckOnClick = true;
            this.ViewMenuDownload.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewMenuDownload.Name = "ViewMenuDownload";
            this.ViewMenuDownload.Size = new System.Drawing.Size(152, 22);
            this.ViewMenuDownload.Text = "Download";
            this.ViewMenuDownload.CheckStateChanged += new System.EventHandler(this.ViewMenuDownload_CheckStateChanged);
            // 
            // ViewMenuUpload
            // 
            this.ViewMenuUpload.Checked = true;
            this.ViewMenuUpload.CheckOnClick = true;
            this.ViewMenuUpload.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewMenuUpload.Name = "ViewMenuUpload";
            this.ViewMenuUpload.Size = new System.Drawing.Size(152, 22);
            this.ViewMenuUpload.Text = "Upload";
            this.ViewMenuUpload.CheckStateChanged += new System.EventHandler(this.ViewMenuUpload_CheckStateChanged);
            // 
            // ViewMenuDownloadCap
            // 
            this.ViewMenuDownloadCap.Checked = true;
            this.ViewMenuDownloadCap.CheckOnClick = true;
            this.ViewMenuDownloadCap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewMenuDownloadCap.Name = "ViewMenuDownloadCap";
            this.ViewMenuDownloadCap.Size = new System.Drawing.Size(152, 22);
            this.ViewMenuDownloadCap.Text = "Download Cap";
            this.ViewMenuDownloadCap.CheckStateChanged += new System.EventHandler(this.DownloadCapToolStripMenuItem_CheckStateChanged);
            this.ViewMenuDownloadCap.Click += new System.EventHandler(this.DownloadCapToolStripMenuItem_Click);
            // 
            // ViewMenuUploadCap
            // 
            this.ViewMenuUploadCap.Checked = true;
            this.ViewMenuUploadCap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewMenuUploadCap.Name = "ViewMenuUploadCap";
            this.ViewMenuUploadCap.Size = new System.Drawing.Size(152, 22);
            this.ViewMenuUploadCap.Text = "Upload Cap";
            this.ViewMenuUploadCap.CheckStateChanged += new System.EventHandler(this.uploadCapToolStripMenuItem_CheckStateChanged);
            // 
            // ViewMenuBlock
            // 
            this.ViewMenuBlock.Checked = true;
            this.ViewMenuBlock.CheckOnClick = true;
            this.ViewMenuBlock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ViewMenuBlock.Name = "ViewMenuBlock";
            this.ViewMenuBlock.Size = new System.Drawing.Size(152, 22);
            this.ViewMenuBlock.Text = "Block";
            this.ViewMenuBlock.CheckStateChanged += new System.EventHandler(this.ViewMenuBlock_CheckStateChanged);
            // 
            // ViewMenuSpoof
            // 
            this.ViewMenuSpoof.CheckOnClick = true;
            this.ViewMenuSpoof.Name = "ViewMenuSpoof";
            this.ViewMenuSpoof.Size = new System.Drawing.Size(152, 22);
            this.ViewMenuSpoof.Text = "Spoofed";
            this.ViewMenuSpoof.CheckStateChanged += new System.EventHandler(this.ViewMenuSpoof_CheckStateChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            this.imageList1.Images.SetKeyName(0, "circle.png");
            this.imageList1.Images.SetKeyName(1, "router.png");
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timerSpoof
            // 
            this.timerSpoof.Tick += new System.EventHandler(this.timerSpoof_Tick);
            // 
            // SelfishNetTrayIcon
            // 
            this.SelfishNetTrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.SelfishNetTrayIcon.BalloonTipText = "SelfishNet is minimized";
            this.SelfishNetTrayIcon.BalloonTipTitle = "SelfishNet";
            this.SelfishNetTrayIcon.Text = "SelfishNet";
            this.SelfishNetTrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // timerDiscovery
            // 
            this.timerDiscovery.Interval = 600000;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 402);
            this.Controls.Add(this.treeGridView1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "SelfishNetSabsab v0.1 Beta";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).EndInit();
            this.ContextMenuViews.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
        #endregion
    }
}