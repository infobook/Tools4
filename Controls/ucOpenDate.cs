using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommandAS.Tools;
using CommandAS.Tools.Forms;

namespace CommandAS.Tools.Controls
{
  public partial class ucOpenDate : UserControl
  {
    private dlgCalendar _dlg;
    protected OpenDate mODate;

    //public override string Text
    //{
    //  get
    //  {
    //    mODate.pFormatString = _mtb.Text;
    //    return _mtb.Text;
    //  }
    //  set
    //  {
    //    _mtb.Text = value;
    //  }
    //}

    public override Color ForeColor
    {
      get
      {
        return _mtb.ForeColor;
      }
      set
      {
        _mtb.ForeColor = value;
      }
    }

    public bool pCalendarButtonTabStop
    {
      get { return _cmd.TabStop;  }
      set { _cmd.TabStop = value; }
    }

    public override string Text //string pTextOpenFormat
    {
      get
      {
        mODate.pFormatString = _mtb.Text;
        return mODate.pString;
      }
      set
      {
        mODate.pString = value;
        _mtb.Text = mODate.pFormatString;
      }
    }

    public string pFormatString
    {
      get
      {
        mODate.pFormatString = _mtb.Text;
        return _mtb.Text;
      }
      set
      {
        mODate.pFormatString = value;
        _mtb.Text = value;
      }
    }

    public string pSep
    {
      get { return _mtb.Mask.Substring(2,1); }
    }

    public object pDateTime
    {
      get
      {
        mODate.pFormatString = _mtb.Text;
        return mODate.pDateTime;
      }
      set
      {
        try
        {
          DateTime dt = (DateTime)value;
          dt = CommandAS.Tools.CASTools.DateWithoutTime(dt);
          mODate.pDateTime = dt;
          _mtb.Text = mODate.pFormatString;
        }
        catch { }
      }
    }

    public event EventHandler OnControlChanged;

    public ucOpenDate()
    {
      _dlg = null;
      mODate = new OpenDate();

      InitializeComponent();
      _mtb.InsertKeyMode = InsertKeyMode.Overwrite;  
      _mtb.TextChanged += new EventHandler(_mtb_TextChanged);
      _mtb.Validating += new CancelEventHandler(_mtb_Validating);
      _cmd.Click += new EventHandler(_cmd_Click);
    }

    void _mtb_Validating(object sender, CancelEventArgs e)
    {
      mODate.pFormatString = _mtb.Text;
      if (!mODate.CheckAndCorrect())
        _mtb.Text = mODate.pFormatString;
      e.Cancel = false;
    }

    public void Init()
    {
      mODate.Init();
    }

    public new bool Focus()
    {
      return _mtb.Focus();
    }

    public void SetCursorToBegin()
    {
      _mtb.SelectionStart = 1;
      _mtb.SelectionLength = 0;
    }

    protected virtual void OnValueChanged(EventArgs eventargs)
    {
      OnTextChanged(eventargs);
    }
    protected override void OnTextChanged(EventArgs e)
    {
      if (OnControlChanged != null)
        OnControlChanged(this, new EventArgs());

      base.OnTextChanged(e);
    }

    private void _mtb_TextChanged(object sender, EventArgs e)
    {
      OnValueChanged(e);
    }

    private void _cmd_Click(object sender, EventArgs e)
    {
      if (_dlg == null)
      {
        _dlg = new dlgCalendar();
        _dlg.Font = this.Font;
        _dlg.StartPosition = FormStartPosition.Manual;
        _dlg.Closing += new CancelEventHandler(onCloseDlg);
        _dlg.Deactivate += new EventHandler(onDeactiveteDlg);
        _dlg.onDateSelect += new EventHandler(onDeactiveteDlg);
      }
      _dlg.Bounds = CASTools.GetBoundsControl(_cmd, _dlg);
      _dlg.Left -= _dlg.Width + _cmd.Width;
      try
      {
        mODate.pFormatString = _mtb.Text;
        _dlg.pCurrentDate = new DateTime(mODate.pYear, mODate.pMonth, mODate.pDay);
      }
#if DEBUG
      catch (Exception ex)
      {
        string s = ex.Message;
      }
#else
			catch{}
#endif
      _dlg.Show();
    }

    private void onCloseDlg(object sender, CancelEventArgs e)
    {
      e.Cancel = true;
    }
    private void onDeactiveteDlg(object sender, EventArgs e)
    {
      if (_dlg.DialogResult == DialogResult.OK) //только если надо менять !
        pDateTime = _dlg.pCurrentDate;
      _dlg.Hide();
    }

  
  }
}
