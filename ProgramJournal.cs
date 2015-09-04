using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CommandAS.Tools
{
  /// <summary>
  /// Class output imformation for various program events, so as 
  /// error, message, trace, debug, warning and simple info.
  /// 
  /// M.Tor
  /// 15.07.2015
  /// </summary>
  public class ProgramJournal
  {
    public enum Type
    {
      Trace = 1,
      Debug = 2,
      Info = 3,
      Message = 4,
      Warning = 5,
      Error = 6
    }

    private TextBox _txb;
    private StreamWriter _sw;

    public TextBox OutputTextBox
    {
      set { _txb = value; }
    }

    public string OutputFileName
    {
      set
      {
        if (_sw != null)
          _sw.Close();
        if (value != null && value.Length > 0)
          _sw = new StreamWriter(value, true);
      }
    }

    /// <summary>
    /// Output debug information to format:
    /// [<current date> <current time>] <text>
    /// </summary>
    public string Debug
    {
      set { Write(null, value, Type.Debug); }
    }
    public string Trace
    {
      set { Write(null, value, Type.Trace); }
    }

    public string Info
    {
      set { Write(null, value, Type.Info); }
    }

    public string Message
    {
      set { Write(string.Empty, value, Type.Message); }
    }

    public string Warning
    {
      set { Write("warning", value, Type.Warning); }
    }

    public string Error
    {
      set { Write("error", value, Type.Error); }
    }

    public ProgramJournal()
    {
      _txb = null;
      _sw = null;
    }

    public void Init()
    {
      if (_txb != null)
      {
        _txb.Text = string.Empty;
        _txb.Refresh();
      }
    }

    public void Write(string aTitle, string aText, ProgramJournal.Type aType)
    {

      if (aTitle != null)
      {
        DateTime dt = DateTime.Now;
        string dtStr = string.Empty;

        switch (aType)
        {
          case Type.Debug:
          case Type.Error:
            dtStr = "[" + dt.ToShortDateString()
                + " " + dt.ToShortTimeString() + "]";
            break;
          case Type.Trace:
          case Type.Warning:
          case Type.Message:
            dtStr = "{" + dt.ToShortTimeString() + "}";
            break;
        }


        if (aTitle.Length > 0)
        {
          aText = aTitle.ToUpper() + " " + dtStr + " - " + aText;
        }
        else
        {
          aText = dtStr + " " + aText;
        }
      }

      if (_txb != null)
      {
        _txb.Text += Environment.NewLine + aText;
        _txb.Refresh();
        //_txb.Parent.Refresh();
      }

      if (_sw != null && aType != Type.Debug && aType != Type.Trace)
        _sw.WriteLine(aText);


    }
  }
}
