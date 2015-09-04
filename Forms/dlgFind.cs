using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CommandAS.Tools.Forms
{
	/// <summary>
	/// Summary description for dlgFind.
	/// </summary>
	public class dlgFind : System.Windows.Forms.Form
	{
    //  для того, чтобы окно не закрывалось
    public event EventHandler toSearch;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _cboFindText;
		private System.Windows.Forms.Button cmdFind;
    private System.Windows.Forms.Button cmdCancel;
		private System.Windows.Forms.GroupBox _grpDirection;
		private System.Windows.Forms.RadioButton _rdbForward;
		private System.Windows.Forms.RadioButton _rdbBack;
    private GroupBox groupBox1;
    private CheckBox _chkIsWordWhole;
    private CheckBox _chkIsMatchCase;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

    public DialogResult pCmdDR
    {
      set { cmdFind.DialogResult = value; }
    }

		public string									pFindText
		{
			get {return _cboFindText.Text; }
			set {_cboFindText.Text = value;}
		}
		public bool										pIsMatchCase
		{
			get {return _chkIsMatchCase.Checked; }
			set {_chkIsMatchCase.Checked = value;}
		}
		public bool										pIsWordWhole
		{
			get {return _chkIsWordWhole.Checked; }
			set {_chkIsWordWhole.Checked = value;}
		}
		public bool										pIsDirectionForward
		{
			get {return _rdbForward.Checked; }
			set {_rdbForward.Checked = value;}
		}

		public dlgFind()
		{
			InitializeComponent();
      cmdFind.Click += new EventHandler(OnCmdFind);
		}

    void OnCmdFind(object sender, EventArgs e)
    {
      if (toSearch != null)
        toSearch(this, e);
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
      this.label1 = new System.Windows.Forms.Label();
      this._cboFindText = new System.Windows.Forms.ComboBox();
      this.cmdFind = new System.Windows.Forms.Button();
      this.cmdCancel = new System.Windows.Forms.Button();
      this._grpDirection = new System.Windows.Forms.GroupBox();
      this._rdbBack = new System.Windows.Forms.RadioButton();
      this._rdbForward = new System.Windows.Forms.RadioButton();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this._chkIsWordWhole = new System.Windows.Forms.CheckBox();
      this._chkIsMatchCase = new System.Windows.Forms.CheckBox();
      this._grpDirection.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(7, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(53, 21);
      this.label1.TabIndex = 0;
      this.label1.Text = "Что:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // _cboFindText
      // 
      this._cboFindText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._cboFindText.Location = new System.Drawing.Point(53, 7);
      this._cboFindText.Name = "_cboFindText";
      this._cboFindText.Size = new System.Drawing.Size(473, 21);
      this._cboFindText.TabIndex = 1;
      // 
      // cmdFind
      // 
      this.cmdFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdFind.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.cmdFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.cmdFind.Location = new System.Drawing.Point(419, 42);
      this.cmdFind.Name = "cmdFind";
      this.cmdFind.Size = new System.Drawing.Size(107, 34);
      this.cmdFind.TabIndex = 4;
      this.cmdFind.Text = "Искать далее";
      // 
      // cmdCancel
      // 
      this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cmdCancel.Location = new System.Drawing.Point(419, 82);
      this.cmdCancel.Name = "cmdCancel";
      this.cmdCancel.Size = new System.Drawing.Size(107, 35);
      this.cmdCancel.TabIndex = 5;
      this.cmdCancel.Text = "Закрыть";
      // 
      // _grpDirection
      // 
      this._grpDirection.Controls.Add(this._rdbBack);
      this._grpDirection.Controls.Add(this._rdbForward);
      this._grpDirection.Location = new System.Drawing.Point(302, 35);
      this._grpDirection.Name = "_grpDirection";
      this._grpDirection.Size = new System.Drawing.Size(107, 83);
      this._grpDirection.TabIndex = 6;
      this._grpDirection.TabStop = false;
      this._grpDirection.Text = "Направление";
      // 
      // _rdbBack
      // 
      this._rdbBack.Location = new System.Drawing.Point(13, 49);
      this._rdbBack.Name = "_rdbBack";
      this._rdbBack.Size = new System.Drawing.Size(87, 21);
      this._rdbBack.TabIndex = 1;
      this._rdbBack.Text = "назад";
      // 
      // _rdbForward
      // 
      this._rdbForward.Checked = true;
      this._rdbForward.Location = new System.Drawing.Point(13, 22);
      this._rdbForward.Name = "_rdbForward";
      this._rdbForward.Size = new System.Drawing.Size(87, 21);
      this._rdbForward.TabIndex = 0;
      this._rdbForward.TabStop = true;
      this._rdbForward.Text = "вперед";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this._chkIsWordWhole);
      this.groupBox1.Controls.Add(this._chkIsMatchCase);
      this.groupBox1.Location = new System.Drawing.Point(12, 34);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(284, 83);
      this.groupBox1.TabIndex = 7;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Настройки";
      // 
      // _chkIsWordWhole
      // 
      this._chkIsWordWhole.Location = new System.Drawing.Point(15, 49);
      this._chkIsWordWhole.Name = "_chkIsWordWhole";
      this._chkIsWordWhole.Size = new System.Drawing.Size(254, 21);
      this._chkIsWordWhole.TabIndex = 5;
      this._chkIsWordWhole.Text = "Полное совпадение";
      // 
      // _chkIsMatchCase
      // 
      this._chkIsMatchCase.Location = new System.Drawing.Point(15, 23);
      this._chkIsMatchCase.Name = "_chkIsMatchCase";
      this._chkIsMatchCase.Size = new System.Drawing.Size(254, 20);
      this._chkIsMatchCase.TabIndex = 4;
      this._chkIsMatchCase.Text = "Различать заглавные и прописные буквы";
      // 
      // dlgFind
      // 
      this.AcceptButton = this.cmdFind;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.cmdCancel;
      this.ClientSize = new System.Drawing.Size(538, 131);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this._grpDirection);
      this.Controls.Add(this.cmdCancel);
      this.Controls.Add(this.cmdFind);
      this.Controls.Add(this._cboFindText);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "dlgFind";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Поиск";
      this._grpDirection.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

		}
		#endregion
	}
}
