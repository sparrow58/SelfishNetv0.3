using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace SelfishNet10
{
	public class CAdapter : Form
	{
		private ComboBox comboBox1;

		private Button buttonOK;

		private Button buttonCancel;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label labelIpText;

		private Label labelGWText;

		private Label labelRedirectInfo;

		private Label labelIPINFO;

		private System.ComponentModel.Container components;

		private Label label4;

		private Label labelTypeText;

		private NetworkInterface[] nics;

		private IEnumerator nicsEnum;

		public NetworkInterface selectedNic;

		public bool packetsHaveToBeRedirected;

		public CAdapter()
		{
			try
			{
				this.InitializeComponent();
				this.nics = NetworkInterface.GetAllNetworkInterfaces();
				this.buttonOK.Enabled = false;
				this.packetsHaveToBeRedirected = false;
				this.buttonCancel.Text = "Quit";
			}
            catch { }
		}

		//private void CAdapter()
		//{
		//	System.ComponentModel.Container container = this.components;
		//	if (container != null)
		//	{
		//		((IDisposable)container).Dispose();
		//	}
		//}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			if (this.buttonCancel.Text.CompareTo("Quit") != 0)
			{
				MainForm.instance.Enabled = true;
				base.Hide();
			}
			else
			{
				IDisposable disposable = MainForm.instance;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			this.buttonCancel.Text = "Cancel";
			MainForm.instance.Enabled = true;
			base.Hide();
			MainForm.instance.NicIsSelected(this.selectedNic);
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			IEnumerator enumerator = this.nics.GetEnumerator();
			this.nicsEnum = enumerator;
			if (enumerator.MoveNext())
			{
				do
				{
					NetworkInterface current = (NetworkInterface)this.nicsEnum.Current;
					if (current.Description.CompareTo(this.comboBox1.SelectedItem.ToString()) != 0)
					{
						continue;
					}
					this.labelTypeText.Text = ((int)((NetworkInterfaceType)(object)current.NetworkInterfaceType)).ToString();
					int num = 0;
					if (0 < current.GetIPProperties().UnicastAddresses.Count)
					{
						while (Convert.ToString(current.GetIPProperties().UnicastAddresses[num].Address.AddressFamily).EndsWith("V6"))
						{
							num++;
							if (num < current.GetIPProperties().UnicastAddresses.Count)
							{
								continue;
							}
							goto Label1;
						}
						this.labelIpText.Text = current.GetIPProperties().UnicastAddresses[num].Address.ToString();
					}
				Label1:
					if (current.GetIPProperties().GatewayAddresses.Count <= 0 || current.GetIPProperties().GatewayAddresses[0].Address.ToString().CompareTo("0.0.0.0") == 0)
					{
						this.labelGWText.Text = "No Gateway !";
						this.buttonOK.Enabled = false;
						return;
					}
					else
					{
						this.labelGWText.Text = current.GetIPProperties().GatewayAddresses[0].Address.ToString();
						this.buttonOK.Enabled = true;
						this.selectedNic = current;
						return;
					}
				}
				while (this.nicsEnum.MoveNext());
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
					//this.~CAdapter();
				}
				finally
				{
					base.Dispose(true);
				}
			}
		}

		public void InitializeComponent()
		{
			this.comboBox1 = new ComboBox();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.labelIpText = new Label();
			this.labelGWText = new Label();
			this.labelRedirectInfo = new Label();
			this.labelIPINFO = new Label();
			this.label4 = new Label();
			this.labelTypeText = new Label();
			base.SuspendLayout();
			this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			Point point = new Point(15, 40);
			this.comboBox1.Location = point;
			this.comboBox1.Name = "comboBox1";
			System.Drawing.Size size = new System.Drawing.Size(328, 21);
			this.comboBox1.Size = size;
			this.comboBox1.TabIndex = 0;
			this.comboBox1.SelectedIndexChanged += new EventHandler(this.comboBox1_SelectedIndexChanged);
			Point point1 = new Point(106, 182);
			this.buttonOK.Location = point1;
			this.buttonOK.Name = "buttonOK";
			System.Drawing.Size size1 = new System.Drawing.Size(51, 33);
			this.buttonOK.Size = size1;
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "Ok";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new EventHandler(this.buttonOK_Click);
			Point point2 = new Point(198, 182);
			this.buttonCancel.Location = point2;
			this.buttonCancel.Name = "buttonCancel";
			System.Drawing.Size size2 = new System.Drawing.Size(51, 33);
			this.buttonCancel.Size = size2;
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
			this.label1.AutoSize = true;
			Point point3 = new Point(12, 24);
			this.label1.Location = point3;
			this.label1.Name = "label1";
			System.Drawing.Size size3 = new System.Drawing.Size(174, 13);
			this.label1.Size = size3;
			this.label1.TabIndex = 3;
			this.label1.Text = "Select the Network Interface Card :";
			this.label2.AutoSize = true;
			Point point4 = new Point(12, 85);
			this.label2.Location = point4;
			this.label2.Name = "label2";
			System.Drawing.Size size4 = new System.Drawing.Size(63, 13);
			this.label2.Size = size4;
			this.label2.TabIndex = 4;
			this.label2.Text = "Ip Address :";
			this.label3.AutoSize = true;
			Point point5 = new Point(12, 110);
			this.label3.Location = point5;
			this.label3.Name = "label3";
			System.Drawing.Size size5 = new System.Drawing.Size(55, 13);
			this.label3.Size = size5;
			this.label3.TabIndex = 5;
			this.label3.Text = "Gateway :";
			this.labelIpText.AutoSize = true;
			Point point6 = new Point(81, 85);
			this.labelIpText.Location = point6;
			this.labelIpText.Name = "labelIpText";
			System.Drawing.Size size6 = new System.Drawing.Size(40, 13);
			this.labelIpText.Size = size6;
			this.labelIpText.TabIndex = 6;
			this.labelIpText.Text = "0.0.0.0";
			this.labelGWText.AutoSize = true;
			Point point7 = new Point(81, 110);
			this.labelGWText.Location = point7;
			this.labelGWText.Name = "labelGWText";
			System.Drawing.Size size7 = new System.Drawing.Size(40, 13);
			this.labelGWText.Size = size7;
			this.labelGWText.TabIndex = 7;
			this.labelGWText.Text = "0.0.0.0";
			this.labelRedirectInfo.AutoEllipsis = true;
			this.labelRedirectInfo.AutoSize = true;
			Point point8 = new Point(12, 134);
			this.labelRedirectInfo.Location = point8;
			this.labelRedirectInfo.Name = "labelRedirectInfo";
			System.Drawing.Size size8 = new System.Drawing.Size(384, 13);
			this.labelRedirectInfo.Size = size8;
			this.labelRedirectInfo.TabIndex = 8;
			this.labelRedirectInfo.Text = "Windows does not redirect packet, however,internal redirection will be activated";
			this.labelIPINFO.AutoEllipsis = true;
			this.labelIPINFO.AutoSize = true;
			Point point9 = new Point(142, 85);
			this.labelIPINFO.Location = point9;
			this.labelIPINFO.Name = "labelIPINFO";
			System.Drawing.Size size9 = new System.Drawing.Size(0, 13);
			this.labelIPINFO.Size = size9;
			this.labelIPINFO.TabIndex = 9;
			this.label4.AutoSize = true;
			Point point10 = new Point(12, 64);
			this.label4.Location = point10;
			this.label4.Name = "label4";
			System.Drawing.Size size10 = new System.Drawing.Size(58, 13);
			this.label4.Size = size10;
			this.label4.TabIndex = 10;
			this.label4.Text = "NIC Type :";
			this.labelTypeText.AutoSize = true;
			Point point11 = new Point(81, 64);
			this.labelTypeText.Location = point11;
			this.labelTypeText.Name = "labelTypeText";
			System.Drawing.Size size11 = new System.Drawing.Size(47, 13);
			this.labelTypeText.Size = size11;
			this.labelTypeText.TabIndex = 11;
			this.labelTypeText.Text = "Ethernet";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(352, 244);
			base.ControlBox = false;
			base.Controls.Add(this.labelTypeText);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.labelIPINFO);
			base.Controls.Add(this.labelRedirectInfo);
			base.Controls.Add(this.labelGWText);
			base.Controls.Add(this.labelIpText);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.comboBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.Name = "CAdapter";
			this.Text = "Nic selection";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public void show(IWin32Window owner)
		{
			base.Show(owner);
			MainForm.instance.Enabled = false;
			IEnumerator enumerator = this.nics.GetEnumerator();
			this.nicsEnum = enumerator;
			enumerator.MoveNext();
			if (!((NetworkInterface)this.nicsEnum.Current).GetIPProperties().GetIPv4Properties().IsForwardingEnabled)
			{
				this.labelRedirectInfo.Text = "Windows does not redirect packet,\n internal redirection will be turned on";
				this.packetsHaveToBeRedirected = true;
			}
			else
			{
				this.labelRedirectInfo.Text = "Windows does redirect packet,\n internal redirection will be turned off";
				this.packetsHaveToBeRedirected = false;
			}
			this.nicsEnum.Reset();
			if (this.nicsEnum.MoveNext())
			{
				do
				{
					NetworkInterface current = (NetworkInterface)this.nicsEnum.Current;
					if (current.GetIPProperties().GatewayAddresses.Count <= 0 || current.OperationalStatus != OperationalStatus.Up)
					{
						continue;
					}
					this.comboBox1.Items.Add(((NetworkInterface)this.nicsEnum.Current).Description);
				}
				while (this.nicsEnum.MoveNext());
			}
			if (this.comboBox1.Items.Count > 1)
			{
				int num = 0;
				this.nicsEnum.Reset();
				if (this.nicsEnum.MoveNext())
				{
					do
					{
						NetworkInterface networkInterface = (NetworkInterface)this.nicsEnum.Current;
						if (networkInterface.GetIPProperties().GatewayAddresses.Count <= 0 || networkInterface.GetIPProperties().GatewayAddresses[0].Address.ToString().CompareTo("0.0.0.0") == 0)
						{
							num++;
						}
						else
						{
							this.comboBox1.SelectedIndex = num;
							return;
						}
					}
					while (this.nicsEnum.MoveNext());
				}
			}
			if (this.comboBox1.Items.Count != 1)
			{
				MessageBox.Show("No network card with a gateway has been found!");
				IDisposable disposable = MainForm.instance;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			else
			{
				this.comboBox1.SelectedIndex = 0;
			}
		}
	}
}