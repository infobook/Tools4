using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CommandAS.Tools.Forms
{
	/// <summary>
	/// Summary description for dlgPleaseWait.
	/// </summary>
	public class dlgPleaseWait : System.Windows.Forms.Form
	{
		private string										mText;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public string											pText
		{
			set 
			{ 
				mText = value;
			}
		}

		public dlgPleaseWait()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mText = "Пожалуйста, подождите!\nВыполняется инициализация и загрузка ...";
		}

		private void OnLoad(object sender, System.EventArgs e)
		{
		}

		private void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;

			e.Graphics.DrawString(mText, 
				new Font("Arial",10,FontStyle.Bold), 
				new SolidBrush(SystemColors.ActiveCaption),
				new Rectangle(new Point(0,0), Size),
				sf
			);

		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// dlgPleaseWait
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(400, 140);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "dlgPleaseWait";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "dlgPleaseWait";
			this.Load += new System.EventHandler(this.OnLoad);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);

		}
		#endregion

	}
}
