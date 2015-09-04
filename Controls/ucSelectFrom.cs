using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class ucSelectFrom : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button _cmd;
		private System.Windows.Forms.TextBox _txt;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		protected PlaceCode 									mSelectedItem;

		public PlaceCode											pSelectedItem
		{
			get { return mSelectedItem; }
			set { mSelectedItem = value;}
		}

		public string													pText
		{
			get { return _txt.Text; }
			set { _txt.Text = value;}
		}

		public event EventHandler 	OnControlChanged;
		public event EventHandler 	OnCommandDo;

		public ucSelectFrom()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			mSelectedItem = new PlaceCode(0, 0);
			_txt.TextChanged += new EventHandler(TextBoxTextChanged);
			_txt.ReadOnly = true;
		}

		protected override void OnResize (EventArgs e)
		{
			base.OnResize(e);
			_txt.Width=this.Width-_cmd.Width;
		}

		private void TextBoxTextChanged(object sender, System.EventArgs e)
		{
			if (OnControlChanged != null)
				OnControlChanged(this, new EventArgs());
		}

		private void DoCommand(object sender, System.EventArgs e)
		{
			if (OnCommandDo != null)
				OnCommandDo(this, new EventArgs());
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
			this._cmd = new System.Windows.Forms.Button();
			this._txt = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _cmd
			// 
			this._cmd.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this._cmd.BackColor = System.Drawing.SystemColors.Control;
			this._cmd.Font = new System.Drawing.Font("Marlett", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this._cmd.Location = new System.Drawing.Point(176, 0);
			this._cmd.Name = "_cmd";
			this._cmd.Size = new System.Drawing.Size(24, 24);
			this._cmd.TabIndex = 0;
			this._cmd.Text = "n";
			this._cmd.Click += new System.EventHandler(this.DoCommand);
			// 
			// _txt
			// 
			this._txt.BackColor = System.Drawing.SystemColors.Window;
			this._txt.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._txt.Location = new System.Drawing.Point(2, 2);
			this._txt.Name = "_txt";
			this._txt.Size = new System.Drawing.Size(174, 15);
			this._txt.TabIndex = 1;
			this._txt.Text = "";
			// 
			// ucSelectFromTreeView
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this._txt,
																																	this._cmd});
			this.Name = "ucSelectFromTreeView";
			this.Size = new System.Drawing.Size(200, 24);
			this.ResumeLayout(false);

		}
		#endregion

	}
}
