using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{
  public partial class ucMonth : UserControl
  {

    private DateTime _firstMonthDay;

    private DayOfWeek _firstWeekDay = DayOfWeek.Monday;

    private TableLayoutPanel _tlp;

    private Color _defaultLabelBackColor;

    public ArrayList pSelectedDate = null;
    public DateTime[] pBoldDate = null;

    public bool pIsDisplayDayPreviousMonth = false;
    public bool pIsDisplayDayNextMonth = false;
    public bool pIsSelectedWeekend = true;
    public bool pIsSelectedToday = true;

    public ucMonth(int aYear, int aMonth)
    {
      _firstMonthDay = new DateTime(aYear, aMonth, 1);

      InitializeComponent();

      _tlp = new TableLayoutPanel();
      SuspendLayout();
      // 
      // _tlp
      // 
      _tlp.Dock = System.Windows.Forms.DockStyle.Fill;
      _tlp.Name = "_tlp";

      _tlp.ColumnCount = 7;
      float percent = 100F / _tlp.ColumnCount;
      for (int ii = 0; ii < _tlp.ColumnCount; ii ++ )
        _tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, percent));

      _tlp.RowCount = 8;
      _tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      _tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      percent = 100F / (_tlp.RowCount-2F);
      for (int ii = 2; ii < _tlp.RowCount; ii++)
        _tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, percent));


      Controls.Add(this._tlp);

      ResumeLayout(false);

      this.Load += new EventHandler(ucMonth_Load);

      _defaultLabelBackColor = (new Label()).BackColor;

      this.MinimumSize = new Size(206, 201);
    }

    void ucMonth_Load(object sender, EventArgs e)
    {
      initMonth();
    }

    private void initMonth()
    {
      Label lbl = new Label();
      lbl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      lbl.TextAlign = ContentAlignment.MiddleCenter;
      lbl.Text = _firstMonthDay.ToString("MMMM");
      lbl.Font = new Font(lbl.Font.FontFamily, lbl.Font.Size, FontStyle.Bold);
      lbl.BorderStyle = BorderStyle.Fixed3D;
      _tlp.Controls.Add(lbl, 0, 0);
      _tlp.SetColumnSpan(lbl, 7);
      
      DateTime dt = DateTime.Today;
      while (dt.DayOfWeek != _firstWeekDay)
        dt = dt.AddDays(1);
      int ic;
      DayOfWeek [] week = new DayOfWeek[7];
      for (ic=0; ic < 7; ic++)
      {
        lbl = new Label();
        lbl.Anchor = AnchorStyles.None;
        lbl.TextAlign = ContentAlignment.MiddleCenter;
        //lbl.BorderStyle = BorderStyle.FixedSingle;
        lbl.Text = dt.ToString("ddd");
        if (pIsSelectedWeekend && (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday))
          lbl.ForeColor = Color.Red;
        _tlp.Controls.Add(lbl, ic, 1);
        week[ic] = dt.DayOfWeek;
        dt = dt.AddDays(1);
      }

      DateTime cd = _firstMonthDay;
      bool is1 = false;
      int offsetWeek = -1;
      for (ic = 0; ic < 7; ic++)
      {
        if (!is1)
        {
          is1 = week[ic] == cd.DayOfWeek;
          offsetWeek++;
        }

        if (is1)
        {
          _tlp.Controls.Add(_getDayView(cd), ic, 2);
          cd = cd.AddDays(1);
        }
      }

      if (pIsDisplayDayPreviousMonth)
      {
        dt = _firstMonthDay;
        while (offsetWeek > 0)
        {
          offsetWeek--;
          dt = dt.AddDays(-1);
          _tlp.Controls.Add(_getDayView(dt), offsetWeek, 2);
        }
      }

      for (int ir = 3; ir < 8; ir++)
      {
        for (ic = 0; ic < 7; ic++)
        {
          if (cd.Month == _firstMonthDay.Month || pIsDisplayDayNextMonth)
          {
            _tlp.Controls.Add(_getDayView(cd), ic, ir);
            cd = cd.AddDays(1);
            //if (cd.AddDays(-1).Month != _firstMonthDay.Month && cd.DayOfWeek == _firstWeekDay)
            if (cd.Month != _firstMonthDay.Month && cd.DayOfWeek == _firstWeekDay)
              return;
          }
        }
      }

    }

    private Label _getDayView(DateTime aDT)
    {
      Label lbl = new Label();
      lbl.Anchor = AnchorStyles.None;
      lbl.TextAlign = ContentAlignment.MiddleCenter;
      lbl.BorderStyle = BorderStyle.Fixed3D;
      lbl.Text = aDT.ToString("dd");
      lbl.Padding = new Padding(0, 0, 0, 0);

      if (aDT.Month == _firstMonthDay.Month)
      {
        if (pSelectedDate != null)
          lbl.Click += new EventHandler(lbl_Click);

        if (pIsSelectedWeekend && (aDT.DayOfWeek == DayOfWeek.Saturday || aDT.DayOfWeek == DayOfWeek.Sunday))
          lbl.ForeColor = Color.Red;

        if (pBoldDate != null)
        {
          for (int ii=0; ii<pBoldDate.Length; ii++)
            if (aDT.Equals(pBoldDate[ii]))
              lbl.Font = new Font(lbl.Font.FontFamily, lbl.Font.Size, FontStyle.Bold);
        }

        if (pIsSelectedToday && aDT.Equals(DateTime.Today))
        {
          lbl.Font = new Font(lbl.Font.FontFamily, lbl.Font.Size, FontStyle.Bold | FontStyle.Underline);
          lbl.ForeColor = Color.Blue;
        }
      }
      else
      {
        if (pIsSelectedWeekend && (aDT.DayOfWeek == DayOfWeek.Saturday || aDT.DayOfWeek == DayOfWeek.Sunday))
          lbl.ForeColor = Color.RosyBrown;
        else
          lbl.ForeColor = SystemColors.GrayText;
      }

      return lbl;
    }

    void lbl_Click(object sender, EventArgs e)
    {
      Label lbl = (Label)sender;
      DateTime dt = new DateTime(_firstMonthDay.Year, _firstMonthDay.Month, Convert.ToInt32(lbl.Text));
      if (lbl.BackColor == _defaultLabelBackColor)
      {
        lbl.BackColor = SystemColors.Highlight;
        lbl.Tag = lbl.ForeColor;
        lbl.ForeColor = SystemColors.HighlightText;
        pSelectedDate.Add(dt);
      }
      else
      {
        lbl.BackColor = _defaultLabelBackColor;
        lbl.ForeColor = (Color)lbl.Tag;
        pSelectedDate.Remove(dt);
      }
    }
  }
}
