using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CommandAS.Tools.Forms
{
	/// <summary>
	/// Summary description for dlgCalendar.
	/// </summary>
	public class dlgCalendar : System.Windows.Forms.Form
	{
		public	DateTime		pCurrentDate
		{
			set 
			{ 
				calendar.SelectionStart = value;
				calendar.SelectionEnd		= value;
			}
			get { return calendar.SelectionStart; }
		}
    public event EventHandler onDateSelect;
		private System.Windows.Forms.MonthCalendar calendar;
    private System.ComponentModel.IContainer components=null;

		public dlgCalendar()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

      this.KeyPreview=true;
      this.ShowInTaskbar=false;

      this.KeyDown += new KeyEventHandler(this.OnCancel);
      this.MouseDown+=new MouseEventHandler(onMouse);
      this.Closed+=new EventHandler(onClose);

      calendar.Dock=DockStyle.Fill;
      onResize(this,EventArgs.Empty);
		}
    private void OnCancel(object sender, KeyEventArgs e)
    {
      if (e.KeyCode==Keys.Escape)
        DialogResult = DialogResult.Cancel;
    }

    private void onMouse(object sender, MouseEventArgs e)
    {
      if (!this.Bounds.Contains(e.X,e.Y))
        this.Hide();
    }
    private void onClose(object sender, System.EventArgs e)
    {
      this.Hide();
    }
    private void onResize(object sender, System.EventArgs e)
    {
      this.Width=calendar.Left+calendar.Width; // ????
      this.Height=calendar.Top+ calendar.Height;
    }

		private void OnDateSelected(object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			DialogResult = DialogResult.OK;
      if (onDateSelect!=null)
        onDateSelect(calendar,EventArgs.Empty);
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
      this.calendar = new System.Windows.Forms.MonthCalendar();
      this.SuspendLayout();
      // 
      // calendar
      // 
      this.calendar.Dock = System.Windows.Forms.DockStyle.Fill;
      this.calendar.MaxSelectionCount = 1;
      this.calendar.Name = "calendar";
      this.calendar.TabIndex = 0;
      this.calendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.OnDateSelected);
      // 
      // dlgCalendar
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(193, 153);
      this.ControlBox = false;
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.calendar});
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "dlgCalendar";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Resize += new System.EventHandler(this.onResize);
      this.ResumeLayout(false);

    }
		#endregion
  }
}
