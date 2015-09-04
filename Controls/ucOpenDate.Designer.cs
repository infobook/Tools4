namespace CommandAS.Tools.Controls
{
  partial class ucOpenDate
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this._mtb = new System.Windows.Forms.MaskedTextBox();
      this._cmd = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // _mtb
      // 
      this._mtb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._mtb.Location = new System.Drawing.Point(0, 0);
      this._mtb.Mask = "00/00/0000";
      this._mtb.Name = "_mtb";
      this._mtb.Size = new System.Drawing.Size(93, 22);
      this._mtb.TabIndex = 0;
      this._mtb.ValidatingType = typeof(System.DateTime);
      // 
      // _cmd
      // 
      this._cmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._cmd.Font = new System.Drawing.Font("Marlett", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
      this._cmd.Location = new System.Drawing.Point(95, 0);
      this._cmd.Name = "_cmd";
      this._cmd.Size = new System.Drawing.Size(22, 22);
      this._cmd.TabIndex = 1;
      this._cmd.Text = "6";
      this._cmd.UseVisualStyleBackColor = true;
      // 
      // ucOpenDate
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._cmd);
      this.Controls.Add(this._mtb);
      this.Name = "ucOpenDate";
      this.Size = new System.Drawing.Size(117, 22);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MaskedTextBox _mtb;
    private System.Windows.Forms.Button _cmd;

  }
}
