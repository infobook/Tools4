using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// Summary description for ucRtfHtmlView.
	/// </summary>
	public class ucRtfHtmlView : System.Windows.Forms.UserControl
	{
		private RichTextBox							mRTB;
		private HtmlControl							mHTML;


		public string										pVal
		{
			set 
			{
				try
				{
					mRTB.Rtf = value;
					mRTB.BringToFront();
					mRTB.TabStop = true;
					mHTML.TabStop = false;
					return;
				}
				catch{}

				try
				{
					mHTML.BringToFront();
					mHTML.Html = value;
					mHTML.TabStop = true;
					mRTB.TabStop = false;
				}
				catch{}
			}
			get
			{
				if (mRTB.TabStop)
					return mRTB.Rtf;
				else
					return string.Empty;
			}
		}

		public string										pText
		{
			set 
			{
				try
				{
					mRTB.Text = value;
					mRTB.BringToFront();
					mRTB.TabStop = true;
					mHTML.TabStop = false;
					return;
				}
				catch{}
			}
			get
			{
				if (mRTB.TabStop)
					return mRTB.Text;
				else
					return mHTML.Html;
			}
		}

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ucRtfHtmlView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			mRTB	= new RichTextBox();
			mHTML = new HtmlControl();

			mRTB.Dock = DockStyle.Fill;
			mRTB.ReadOnly = true;
			//mRTB.Visible = false;

			mHTML.Dock = DockStyle.Fill;
			//mHTML.Visible = false;

			Controls.AddRange(new Control[]{mRTB, mHTML});

			Load += new EventHandler (OnLoad);
		}

		private void OnLoad(object sender, EventArgs e)
		{
			//mHTML.RaiseNavigateBlank();
			mHTML.DelayedInitialize();
		}

		public void Clear()
		{
			try
			{
				mRTB.Text = string.Empty;
			}
			catch{}

			try
			{
				mHTML.Html = string.Empty;
			}
			catch{}
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
