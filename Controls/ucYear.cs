using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{
  public partial class ucYear : UserControl
  {
    private TableLayoutPanel _tlp;
    private ucMonth[] _m12;

    public System.Collections.ArrayList pSelectedDate
    {
      set
      {
        for (int ii = 0; ii < 12; ii++)
          _m12[ii].pSelectedDate = value;
      }
    }
    public DateTime[] pBoldDate
    {
      set
      {
        for (int ii = 0; ii < 12; ii++)
          _m12[ii].pBoldDate = value;
      }
    }

    public bool pIsDisplayDayPreviousMonth
    {
      get { return _m12[0].pIsDisplayDayPreviousMonth; }
      set
      {
        for (int ii = 0; ii < 12; ii++)
          _m12[ii].pIsDisplayDayPreviousMonth = value;
      }
    }
    public bool pIsDisplayDayNextMonth
    {
      get { return _m12[0].pIsDisplayDayNextMonth; }
      set
      {
        for (int ii = 0; ii < 12; ii++)
          _m12[ii].pIsDisplayDayNextMonth = value;
      }
    }
    public bool pIsSelectedWeekend
    {
      get { return _m12[0].pIsSelectedWeekend; }
      set
      {
        for (int ii = 0; ii < 12; ii++)
          _m12[ii].pIsSelectedWeekend = value;
      }
    }
    public bool pIsSelectedToday
    {
      get { return _m12[0].pIsSelectedToday; }
      set
      {
        for (int ii = 0; ii < 12; ii++)
          _m12[ii].pIsSelectedToday = value;
      }
    }

    /// <summary>
    /// Year user contol
    /// </summary>
    /// <param name="aYear">year</param>
    /// <param name="aColCount">column count integer from range from 1 to 12</param>
    public ucYear(int aYear, int aColCount)
    {
      InitializeComponent();

      _tlp = new System.Windows.Forms.TableLayoutPanel();
      _m12 = new ucMonth[12];
      for (int ii = 0; ii < 12; ii++)
      {
        _m12[ii] = new ucMonth(aYear, ii+1);
        //if (ii % 2 == 0)
          _m12[ii].BorderStyle = BorderStyle.FixedSingle;
      }

      int rowCount = 4;
      if (12 % aColCount == 0)
        rowCount = 12 / aColCount;
      else
        aColCount = 3;

      _init(aColCount, rowCount);
    }

    private void _init(int aColCount, int aRowCount)
    {
      float colWidth = _m12[0].MinimumSize.Width;
      float rowHeight = _m12[0].MinimumSize.Height;

      this.SuspendLayout();

      // 
      // _tlp
      // 
      _tlp.ColumnCount = aColCount;
      for (int ic = 0; ic < aColCount; ic++)
        _tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, colWidth+4F));

      _tlp.RowCount = aRowCount;
      for (int ir = 0; ir < aRowCount; ir++)
        _tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight+3F));

      _tlp.Location = new Point(3, 3);
      _tlp.Size = new Size(((int)colWidth+4) * aColCount, ((int)rowHeight+3) * aRowCount);
      
      for (int ir = 0; ir < _tlp.RowCount; ir++)
        for (int ic = 0; ic < aColCount; ic++)
          _tlp.Controls.Add(_m12[ir * aColCount + ic], ic, ir);

        // 
        // ucYear
        // 
        //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(_tlp);
        this.AutoScroll = true;
        //this.Name = "ucYear";
        //this.Size = new System.Drawing.Size(318, 274);

        this.ResumeLayout(false);
    }
  }
}
