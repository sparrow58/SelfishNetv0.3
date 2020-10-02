using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SelfishNet
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

		private Label label4;

		private Label labelTypeText;

		private NetworkInterface[] nics;

		private IEnumerator nicsEnum;

		public NetworkInterface selectedNic;

		public bool packetsHaveToBeRedirected;
		private Container components;

		public CAdapter()
		{
			try
			{
				InitializeComponent();
				nics = NetworkInterface.GetAllNetworkInterfaces();
				buttonOK.Enabled = false;
				packetsHaveToBeRedirected = false;
				buttonCancel.Text = "Quit";
			}
            catch { }
		}

        private void buttonCancel_Click(object sender, EventArgs e)
		{
			if (buttonCancel.Text.CompareTo("Quit") != 0)
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
			buttonCancel.Text = "Cancel";
			MainForm.instance.Enabled = true;
			base.Hide();
			MainForm.instance.NicIsSelected(selectedNic);
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			IEnumerator enumerator = nics.GetEnumerator();
			nicsEnum = enumerator;
			if (enumerator.MoveNext())
			{
				do
				{
					NetworkInterface current = (NetworkInterface)nicsEnum.Current;
					if (current.Description.CompareTo(comboBox1.SelectedItem.ToString()) != 0)
					{
						continue;
					}
					labelTypeText.Text = ((int)((NetworkInterfaceType)(object)current.NetworkInterfaceType)).ToString();
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
						labelIpText.Text = current.GetIPProperties().UnicastAddresses[num].Address.ToString();
					}
				Label1:
					if (current.GetIPProperties().GatewayAddresses.Count <= 0 || current.GetIPProperties().GatewayAddresses[0].Address.ToString().CompareTo("0.0.0.0") == 0)
					{
						labelGWText.Text = "No Gateway !";
						buttonOK.Enabled = false;
						return;
					}
					else
					{
						labelGWText.Text = current.GetIPProperties().GatewayAddresses[0].Address.ToString();
						buttonOK.Enabled = true;
						selectedNic = current;
						return;
					}
				}
				while (nicsEnum.MoveNext());
			}
		}

		protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool P_0)
		{
			if (P_0)
			{
				try
				{
					_007ECAdapter();
				}
				finally
				{
					base.Dispose(disposing: true);
				}
			}
			else
			{
				base.Dispose(disposing: false);
			}
		}
		private void _007ECAdapter()
		{
			((IDisposable)components)?.Dispose();
		}

		public void InitializeComponent()
		{
			comboBox1 = new ComboBox();
			buttonOK = new Button();
			buttonCancel = new Button();
			label1 = new Label();
			label2 = new Label();
			label3 = new Label();
			labelIpText = new Label();
			labelGWText = new Label();
			labelRedirectInfo = new Label();
			labelIPINFO = new Label();
			label4 = new Label();
			labelTypeText = new Label();
			base.SuspendLayout();
			comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox1.FormattingEnabled = true;
			Point point = new Point(15, 40);
			comboBox1.Location = point;
			comboBox1.Name = "comboBox1";
			System.Drawing.Size size = new System.Drawing.Size(328, 21);
			comboBox1.Size = size;
			comboBox1.TabIndex = 0;
			comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
			Point point1 = new Point(106, 182);
			buttonOK.Location = point1;
			buttonOK.Name = "buttonOK";
			System.Drawing.Size size1 = new System.Drawing.Size(51, 33);
			buttonOK.Size = size1;
			buttonOK.TabIndex = 1;
			buttonOK.Text = "Ok";
			buttonOK.UseVisualStyleBackColor = true;
			buttonOK.Click += new EventHandler(buttonOK_Click);
			Point point2 = new Point(198, 182);
			buttonCancel.Location = point2;
			buttonCancel.Name = "buttonCancel";
			System.Drawing.Size size2 = new System.Drawing.Size(51, 33);
			buttonCancel.Size = size2;
			buttonCancel.TabIndex = 2;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			buttonCancel.Click += new EventHandler(buttonCancel_Click);
			label1.AutoSize = true;
			Point point3 = new Point(12, 24);
			label1.Location = point3;
			label1.Name = "label1";
			System.Drawing.Size size3 = new System.Drawing.Size(174, 13);
			label1.Size = size3;
			label1.TabIndex = 3;
			label1.Text = "Select the Network Interface Card :";
			label2.AutoSize = true;
			Point point4 = new Point(12, 85);
			label2.Location = point4;
			label2.Name = "label2";
			System.Drawing.Size size4 = new System.Drawing.Size(63, 13);
			label2.Size = size4;
			label2.TabIndex = 4;
			label2.Text = "Ip Address :";
			label3.AutoSize = true;
			Point point5 = new Point(12, 110);
			label3.Location = point5;
			label3.Name = "label3";
			System.Drawing.Size size5 = new System.Drawing.Size(55, 13);
			label3.Size = size5;
			label3.TabIndex = 5;
			label3.Text = "Gateway :";
			labelIpText.AutoSize = true;
			Point point6 = new Point(81, 85);
			labelIpText.Location = point6;
			labelIpText.Name = "labelIpText";
			System.Drawing.Size size6 = new System.Drawing.Size(40, 13);
			labelIpText.Size = size6;
			labelIpText.TabIndex = 6;
			labelIpText.Text = "0.0.0.0";
			labelGWText.AutoSize = true;
			Point point7 = new Point(81, 110);
			labelGWText.Location = point7;
			labelGWText.Name = "labelGWText";
			System.Drawing.Size size7 = new System.Drawing.Size(40, 13);
			labelGWText.Size = size7;
			labelGWText.TabIndex = 7;
			labelGWText.Text = "0.0.0.0";
			labelRedirectInfo.AutoEllipsis = true;
			labelRedirectInfo.AutoSize = true;
			Point point8 = new Point(12, 134);
			labelRedirectInfo.Location = point8;
			labelRedirectInfo.Name = "labelRedirectInfo";
			System.Drawing.Size size8 = new System.Drawing.Size(384, 13);
			labelRedirectInfo.Size = size8;
			labelRedirectInfo.TabIndex = 8;
			labelRedirectInfo.Text = "Windows does not redirect packet, however,internal redirection will be activated";
			labelIPINFO.AutoEllipsis = true;
			labelIPINFO.AutoSize = true;
			Point point9 = new Point(142, 85);
			labelIPINFO.Location = point9;
			labelIPINFO.Name = "labelIPINFO";
			System.Drawing.Size size9 = new System.Drawing.Size(0, 13);
			labelIPINFO.Size = size9;
			labelIPINFO.TabIndex = 9;
			label4.AutoSize = true;
			Point point10 = new Point(12, 64);
			label4.Location = point10;
			label4.Name = "label4";
			System.Drawing.Size size10 = new System.Drawing.Size(58, 13);
			label4.Size = size10;
			label4.TabIndex = 10;
			label4.Text = "NIC Type :";
			labelTypeText.AutoSize = true;
			Point point11 = new Point(81, 64);
			labelTypeText.Location = point11;
			labelTypeText.Name = "labelTypeText";
			System.Drawing.Size size11 = new System.Drawing.Size(47, 13);
			labelTypeText.Size = size11;
			labelTypeText.TabIndex = 11;
			labelTypeText.Text = "Ethernet";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(352, 244);
			base.ControlBox = false;
			base.Controls.Add(labelTypeText);
			base.Controls.Add(label4);
			base.Controls.Add(labelIPINFO);
			base.Controls.Add(labelRedirectInfo);
			base.Controls.Add(labelGWText);
			base.Controls.Add(labelIpText);
			base.Controls.Add(label3);
			base.Controls.Add(label2);
			base.Controls.Add(label1);
			base.Controls.Add(buttonCancel);
			base.Controls.Add(buttonOK);
			base.Controls.Add(comboBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.Name = "CAdapter";
			Text = "Nic selection";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public void show(IWin32Window owner)
		{
			base.Show(owner);
			MainForm.instance.Enabled = false;
			IEnumerator enumerator = nics.GetEnumerator();
			nicsEnum = enumerator;
			enumerator.MoveNext();
			if (!((NetworkInterface)nicsEnum.Current).GetIPProperties().GetIPv4Properties().IsForwardingEnabled)
			{
				labelRedirectInfo.Text = "Windows does not redirect packet,\n internal redirection will be turned on";
				packetsHaveToBeRedirected = true;
			}
			else
			{
				labelRedirectInfo.Text = "Windows does redirect packet,\n internal redirection will be turned off";
				packetsHaveToBeRedirected = false;
			}
			nicsEnum.Reset();
			if (nicsEnum.MoveNext())
			{
				do
				{
					NetworkInterface current = (NetworkInterface)nicsEnum.Current;
					if (current.GetIPProperties().GatewayAddresses.Count <= 0 || current.OperationalStatus != OperationalStatus.Up)
					{
						continue;
					}
					comboBox1.Items.Add(((NetworkInterface)nicsEnum.Current).Description);
				}
				while (nicsEnum.MoveNext());
			}
			if (comboBox1.Items.Count > 1)
			{
				int num = 0;
				nicsEnum.Reset();
				if (nicsEnum.MoveNext())
				{
					do
					{
						NetworkInterface networkInterface = (NetworkInterface)nicsEnum.Current;
						if (networkInterface.GetIPProperties().GatewayAddresses.Count <= 0 || networkInterface.GetIPProperties().GatewayAddresses[0].Address.ToString().CompareTo("0.0.0.0") == 0)
						{
							num++;
						}
						else
						{
							comboBox1.SelectedIndex = num;
							return;
						}
					}
					while (nicsEnum.MoveNext());
				}
			}
			if (comboBox1.Items.Count != 1)
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
				comboBox1.SelectedIndex = 0;
			}
		}
	}
}