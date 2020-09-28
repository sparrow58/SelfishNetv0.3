using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SelfishNet10
{
	public class CWizard : Form
	{
		public RichTextBox richTextBox1;

		private Button button1;

		private RadioButton radioButton1;

		private RadioButton radioButton2;

		private Label label1;

        public CWizard()
		{
			try
			{
				this.InitializeComponent();
				ArpForm.instance.Enabled = false;
            }
            catch { }
		}

		//private void ~CWizard()
		//{
		//	System.ComponentModel.Container container = this.components;
		//	if (container != null)
		//	{
		//		((IDisposable)container).Dispose();
		//	}
		//}

		private void button1_Click(object sender, EventArgs e)
		{
			if (this.button1.Text.CompareTo("Quit") != 0)
			{
				if (File.Exists("license.txt"))
				{
					File.Move("license.txt", "LicenseYouAccepted.txt");
				}
				ArpForm.instance.Enabled = true;
				ArpForm.instance.licenseAccepted();
			}
			else
			{
				IDisposable disposable = ArpForm.instance;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			if (this != null)
			{
				((IDisposable)this).Dispose();
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
					//this.~CWizard();
				}
				finally
				{
					base.Dispose(true);
				}
			}
		}

		private void InitializeComponent()
		{
			this.richTextBox1 = new RichTextBox();
			this.button1 = new Button();
			this.radioButton1 = new RadioButton();
			this.radioButton2 = new RadioButton();
			this.label1 = new Label();
			base.SuspendLayout();
			Point point = new Point(7, 25);
			this.richTextBox1.Location = point;
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			System.Drawing.Size size = new System.Drawing.Size(439, 279);
			this.richTextBox1.Size = size;
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "this app use wpcap\\n Do u agree to use their driver";
			Point point1 = new Point(377, 348);
			this.button1.Location = point1;
			this.button1.Name = "button1";
			System.Drawing.Size size1 = new System.Drawing.Size(69, 29);
			this.button1.Size = size1;
			this.button1.TabIndex = 1;
			this.button1.Text = "Quit";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.button1_Click);
			this.radioButton1.AutoSize = true;
			Point point2 = new Point(7, 310);
			this.radioButton1.Location = point2;
			this.radioButton1.Name = "radioButton1";
			System.Drawing.Size size2 = new System.Drawing.Size(154, 17);
			this.radioButton1.Size = size2;
			this.radioButton1.TabIndex = 2;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "I agree, install me the driver";
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new EventHandler(this.radioButton1_CheckedChanged);
			this.radioButton2.AutoSize = true;
			this.radioButton2.Checked = true;
			Point point3 = new Point(7, 333);
			this.radioButton2.Location = point3;
			this.radioButton2.Name = "radioButton2";
			System.Drawing.Size size3 = new System.Drawing.Size(91, 17);
			this.radioButton2.Size = size3;
			this.radioButton2.TabIndex = 3;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "I do not agree";
			this.radioButton2.UseVisualStyleBackColor = true;
			this.label1.AutoSize = true;
			Point point4 = new Point(195, 9);
			this.label1.Location = point4;
			this.label1.Name = "label1";
			System.Drawing.Size size4 = new System.Drawing.Size(49, 13);
			this.label1.Size = size4;
			this.label1.TabIndex = 5;
			this.label1.Text = "Licenses";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(454, 385);
			base.ControlBox = false;
			base.Controls.Add(this.label1);
			base.Controls.Add(this.radioButton2);
			base.Controls.Add(this.radioButton1);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.richTextBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			base.Name = "CWizard";
			this.Text = "Wizard";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.radioButton1.Checked)
			{
				this.button1.Text = "Quit";
			}
			else
			{
				this.button1.Text = "Next";
			}
		}
	}
}