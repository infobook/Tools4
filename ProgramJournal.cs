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
  /// 
  ///     created - 15.07.2015
  /// last update - 16.09.2015
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
    private string _fileName;
    private StreamWriter _sw;
    private bool _isConsleOutput;

    public TextBox OutputTextBox
    {
      set { _txb = value; }
    }

    public string OutputFileName
    {
      set
      {
        _fileName = value;
      }
    }

    public bool IsConsoleOutput
    {
      get { return _isConsleOutput;  }
      set { _isConsleOutput = value; }
    }
    /// <summary>
    /// Output debug information to format:
    /// [_current_date _current_time] _text
    /// Not write to the log file, to display only.
    /// </summary>
    public string Debug
    {
      set { Write(null, value, Type.Debug); }
    }

    /// <summary>
    /// Output trace information to format:
    /// {_current_time>} _text
    /// Not write to the log file, to display only.
    /// </summary>
    public string Trace
    {
      set { Write(null, value, Type.Trace); }
    }

    /// <summary>
    /// Output trace information to format:
    /// _text
    /// Write to the log file and show to display.
    /// </summary>
    public string Info
    {
      set { Write(null, value, Type.Info); }
    }

    /// <summary>
    /// Output message information to format:
    /// [_current_date _current_time] _text
    /// Write to the log file and show to display.
    /// </summary>
    public string Message
    {
      set { Write(string.Empty, value, Type.Message); }
    }

    /// <summary>
    /// Output warning information to format:
    /// WARNING {_current_time>} _text
    /// Write to the log file and show to display.
    /// </summary>
    public string Warning
    {
      set { Write("warning", value, Type.Warning); }
    }

    /// <summary>
    /// Output error information to format:
    /// ERROR [_current_date _current_time] _text
    /// Write to the log file and show to display.
    /// </summary>
    public string Error
    {
      set { Write("error", value, Type.Error); }
    }

    public ProgramJournal()
    {
      _txb = null;
      _sw = null;
      _isConsleOutput = false;
    }

    /// <summary>
    /// Clear TextBox contents, if it's defined.
    /// </summary>
    public void ClearDisplay()
    {
      if (_txb != null)
      {
        _txb.Text = string.Empty;
        _txb.Refresh();
      }
    }

    public void RefreshDisplay()
    {
      if (_txb != null)
      {
        _txb.Select(_txb.Text.Length, 0);
        _txb.Refresh();
      }
    }

    /// <summary>
    /// Begin write block to log file.
    /// If need opened file.
    /// </summary>
    /// <param name="aMessage">message text</param>
    public void Begin(string aMessage)
    {
      Begin();
      Message = aMessage;
    }
    /// <summary>
    /// Begin write block to log file.
    /// If need opened file.
    /// </summary>
    public void Begin()
    {
      if (_fileName != null && _fileName.Length > 0 && _sw == null)
        _sw = new StreamWriter(_fileName, true);
    }

    /// <summary>
    /// Commit write block - close log file.
    /// </summary>
    /// <param name="aMessage">message text</param>
    public void Commit(string aMessage)
    {
      Message = aMessage;
      Commit();
    }
    /// <summary>
    /// Commit write block - close log file.
    /// </summary>
    public void Commit()
    {
      if (_sw != null)
      {
        _sw.Close();
        _sw = null;
      }
    }

    /// <summary>
    /// Show output information to the display and write to the log file,
    /// if they defined.
    /// </summary>
    /// <param name="aTitle"></param>
    /// <param name="aText"></param>
    /// <param name="aType"></param>
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
          case Type.Message:
            dtStr = "[" + dt.ToShortDateString()
                + " " + dt.ToShortTimeString() + "]";
            break;
          case Type.Trace:
          case Type.Warning:
            dtStr = "{" + dt.ToLongTimeString() + "}";
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
      {
        _sw.WriteLine(aText);
      }

      if (_isConsleOutput && aType != Type.Debug && aType != Type.Trace)
      {
        System.Console.WriteLine(aText);
      }

    }
  }
}
