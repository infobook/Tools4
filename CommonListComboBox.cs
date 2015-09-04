using System;
using System.Collections;
using System.Diagnostics;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CommandAS.Tools
{
  public class CompareListPCItem : IComparer
  {
    int IComparer.Compare(Object a1, Object a2)
    {
      int ret = 0;

      _ListBoxPCItem lpc1 = a1 as _ListBoxPCItem;
      _ListBoxPCItem lpc2 = a2 as _ListBoxPCItem;

      if (lpc1 != null && lpc2 != null)
      {
        ret = lpc1.pText.CompareTo(lpc2.pText);
      }
      else
        throw new ArgumentException("Objects is not a _ListBoxItem");

      return ret;

    }
  }

  public class _ListBoxItem
  {
    private int _code;
    private string _text;

    public int code
    {
      [DebuggerStepThrough]
      get { return _code; }
      [DebuggerStepThrough]
      set { _code = value; }
    }
    public string text
    {
      [DebuggerStepThrough]
      get { return _text; }
      [DebuggerStepThrough]
      set { _text = value; }
    }
    public string pCodeText
    {
      get
      {
        return code.ToString() + PlaceCode.DELIM.ToString() + text;
      }
      set
      {
        string[] ss = value.Split(new char[] { PlaceCode.DELIM });
        try
        {
          code = CASTools.ConvertToInt32Or0(ss[0]);
          text = value.Substring(ss[0].Length + PlaceCode.DELIM.ToString().Length);
        }
        catch { }
      }
    }

    public _ListBoxItem() : this(0, string.Empty) { }
    public _ListBoxItem(int aCode, string aText)
    {
      _code = aCode;
      _text = aText;
    }
    public _ListBoxItem(string aCodeText) : this()
    {
      pCodeText = aCodeText;
    }

    public override string ToString()
    {
      return _text;
    }
  }

  public class _ListBoxDecimalItem
  {
    private decimal _decCode;
    private string _text;

    public decimal decCode
    {
      [DebuggerStepThrough]
      get { return _decCode; }
      [DebuggerStepThrough]
      set { _decCode = value; }
    }

    public string text
    {
      [DebuggerStepThrough]
      get { return _text; }
      [DebuggerStepThrough]
      set { _text = value; }
    }

    public string pDecCodeText
    {
      get
      {
        return decCode.ToString() + PlaceCode.DELIM.ToString() + text;
      }
      set
      {
        string[] ss = value.Split(new char[] { PlaceCode.DELIM });
        try
        {
          decCode = Convert.ToDecimal(ss[0]);
          text = value.Substring(ss[0].Length + PlaceCode.DELIM.ToString().Length);
        }
        catch { }
      }
    }

    public _ListBoxDecimalItem() : this(0, string.Empty) { }
    public _ListBoxDecimalItem(decimal aDecCode, string aText)
    {
      _decCode = aDecCode;
      _text = aText;
    }
    public _ListBoxDecimalItem(string aCodeText) : this()
    {
      pDecCodeText = aCodeText;
    }

    public override string ToString()
    {
      return _text;
    }

  }

  public class _ListBoxTextItem
  {
    public string text1;
    public string text2;

    public string TextValue
    {
      [DebuggerStepThrough]
      get { return text1; }
      [DebuggerStepThrough]
      set { text1 = value; }
    }

    public string TextDisplay
    {
      [DebuggerStepThrough]
      get { return text2; }
      [DebuggerStepThrough]
      set { text2 = value; }
    }

    public string pTextText
    {
      get
      {
        return text1 + PlaceCode.DELIM.ToString() + text2;
      }
      set
      {
        string[] ss = value.Split(new char[] { PlaceCode.DELIM });
        try
        {
          text1 = ss[0];
          text2 = value.Substring(ss[0].Length + PlaceCode.DELIM.ToString().Length);
        }
        catch { }
      }
    }

    public _ListBoxTextItem() : this(string.Empty, string.Empty) { }
    public _ListBoxTextItem(string aText1, string aText2)
    {
      text1 = aText1;
      text2 = aText2;
    }
    public _ListBoxTextItem(string aTextText)
      : this()
    {
      pTextText = aTextText;
    }

    public override string ToString()
    {
      return text2;
    }
  }

  public class _ListBoxPCItem
  {
    private PlaceCode _pc;
    private string _text;

    public PlaceCode pPC
    {
      [DebuggerStepThrough]
      get { return _pc; }
      [DebuggerStepThrough]
      set { _pc = value; }
    }
    public string pText
    {
      [DebuggerStepThrough]
      get { return _text; }
      [DebuggerStepThrough]
      set { _text = value; }
    }
    public string pPDC
    {
      [DebuggerStepThrough]
      get { return _pc.PlaceDelimCode; }
      [DebuggerStepThrough]
      set { _pc = PlaceCode.PDC2PlaceCode(value); }
    }
    public string pPCText
    {
      get
      {
        return
          _pc.place.ToString()
          + PlaceCode.DELIM.ToString()
          + _pc.code.ToString()
          + PlaceCode.DELIM.ToString()
          + _text;
      }
      set
      {
        string[] ss = value.Split(new char[] { PlaceCode.DELIM });
        try
        {
          _pc.place = CASTools.ConvertToInt32Or0(ss[0]);
          _pc.code = CASTools.ConvertToInt32Or0(ss[1]);
          _text = value.Substring(ss[0].Length + ss[1].Length + PlaceCode.DELIM.ToString().Length * 2);
        }
        catch { }
      }
    }

    public _ListBoxPCItem() : this(new PlaceCode(0, 0), string.Empty) { }
    public _ListBoxPCItem(int aPlace, int aCode, string aText) : this(new PlaceCode(aPlace, aCode), aText) { }
    public _ListBoxPCItem(PlaceCode aPC, string aText)
    {
      _pc = aPC;
      _text = aText;
    }
    public _ListBoxPCItem(string aPCText)
      : this()
    {
      pPCText = aPCText;
    }

    public override string ToString()
    {
      return _text;
    }

    public override bool Equals(object obj)
    {
      bool ret = false;
      _ListBoxPCItem li = obj as _ListBoxPCItem;
      if (li != null)
        ret = li.pPC.Equals(pPC);
      return ret;
    }

    public _ListBoxPCItem Clone()
    {
      _ListBoxPCItem li = new _ListBoxPCItem();
      li.pPC = pPC;
      li.pText = pText;
      return li;
    }
  }

  public class ArrayListBoxItems : ArrayList
  {
    public void FillTextFromDB(OleDbCommand aCmd)
    {
      FillTextFromDB(aCmd, true, false);
    }
    public void FillTextFromDB(OleDbCommand aCmd, bool aIsClearBeforeFill, bool aIsFirstEmpty)
    {
      try
      {
        OleDbDataReader dReader = null;
        try
        {
          dReader = aCmd.ExecuteReader();
          if (aIsClearBeforeFill)
            this.Clear();
          if (aIsFirstEmpty)
            this.Add(string.Empty);
          while (dReader.Read())
            this.Add(dReader[0].ToString());
        }
        finally
        {
          if (dReader != null)
            dReader.Close();
        }
      }
      catch { }
    }

    public void FillTextTextFromDB(OleDbCommand aCmd)
    {
      FillCodeTextFromDB(aCmd, true, false);
    }
    public void FillTextTextFromDB(OleDbCommand aCmd, bool aIsClearBeforeFill, bool aIsFirstEmpty)
    {
      try
      {
        OleDbDataReader dReader = null;
        try
        {
          dReader = aCmd.ExecuteReader();
          if (aIsClearBeforeFill)
            this.Clear();
          if (aIsFirstEmpty)
            this.Add(new _ListBoxTextItem());
          while (dReader.Read())
            this.Add(new _ListBoxTextItem(dReader[0].ToString(), dReader[1].ToString()));
        }
        finally
        {
          if (dReader != null)
            dReader.Close();
        }
      }
      catch { }
    }

    public void FillCodeTextFromDB(OleDbCommand aCmd)
    {
      FillCodeTextFromDB(aCmd, true, false);
    }
    public void FillCodeTextFromDB(OleDbCommand aCmd, bool aIsClearBeforeFill, bool aIsFirstEmpty)
    {
      try
      {
        OleDbDataReader dReader = null;
        try
        {
          dReader = aCmd.ExecuteReader();
          if (aIsClearBeforeFill)
            this.Clear();
          if (aIsFirstEmpty)
            this.Add(new _ListBoxItem());
          while (dReader.Read())
            this.Add(new _ListBoxItem(Convert.ToInt32(dReader[0]), dReader[1].ToString()));
        }
        finally
        {
          if (dReader != null)
            dReader.Close();
        }
      }
      catch { }
    }

    public void FillDecimalTextFromDB(OleDbCommand aCmd)
    {
      FillDecimalTextFromDB(aCmd, true, false);
    }

    public void FillDecimalTextFromDB(OleDbCommand aCmd, bool aIsClearBeforeFill, bool aIsFirstEmpty)
    {
      try
      {
        OleDbDataReader dReader = null;
        try
        {
          dReader = aCmd.ExecuteReader();
          if (aIsClearBeforeFill)
            this.Clear();
          if (aIsFirstEmpty)
            this.Add(new _ListBoxDecimalItem());
          while (dReader.Read())
            this.Add(new _ListBoxDecimalItem(Convert.ToDecimal(dReader[0]), dReader[1].ToString()));
        }
        finally
        {
          if (dReader != null)
            dReader.Close();
        }
      }
      catch { }
    }

    public void FillPCTextFromDB(OleDbCommand aCmd)
    {
      FillPCTextFromDB(aCmd, true, false);
    }
    public void FillPCTextFromDB(OleDbCommand aCmd, bool aIsClearBeforeFill, bool aIsFirstEmpty)
    {
      try
      {
        OleDbDataReader dReader = null;
        try
        {
          dReader = aCmd.ExecuteReader();
          if (aIsClearBeforeFill)
            this.Clear();
          if (aIsFirstEmpty)
            this.Add(new _ListBoxPCItem());
          while (dReader.Read())
            this.Add(new _ListBoxPCItem(new PlaceCode(Convert.ToInt32(dReader[0]),Convert.ToInt32(dReader[1])), dReader[2].ToString()));
        }
        finally
        {
          if (dReader != null)
            dReader.Close();
        }
      }
      catch { }
    }

    public String GetDisplayTextByTextValue(String aVal)
    {
      _ListBoxTextItem item = FindByTextValue(aVal);
      if (item != null)
        return item.TextDisplay;
      else
        return String.Empty;
    }
    public _ListBoxTextItem FindByTextValue(String aVal)
    {
      _ListBoxTextItem ret = null;
      foreach (_ListBoxTextItem item in this)
      {
        if (item.TextValue.Equals(aVal))
        {
          ret = item;
          break;
        }
      }
      return ret;
    }
  }
  /// <summary>
  /// 
  /// </summary>
  public class CommonListComboBox
  {

    /// <summary>
    /// «аполнение ComboBox из запроса следующего формата:
    /// SELECT [text field] FROM .... 
    /// </summary>
    /// <param name="aCbo">ComboBox</param>
    /// <param name="aCn">Connection with DB</param>
    /// <param name="aSQLText">SQL запрос</param>
    public static void FillListTextFromDB(ComboBox aCbo, OleDbConnection aCn, string aSQLText)
    {
      OleDbCommand cmd = new OleDbCommand(aSQLText, aCn);
      OleDbDataReader dReader = null;
      try
      {
        dReader = cmd.ExecuteReader();
        aCbo.Items.Clear();
        while (dReader.Read())
          aCbo.Items.Add(dReader[0]);
      }
      catch (Exception ex)
      {
        Error.ShowError(ex.Message + "\n" + ex.Source);
      }
      finally
      {
        if (dReader != null)
          dReader.Close();
      }
    }

    /// <summary>
    /// «аполнение ComboBox из запроса следующего формата:
    /// SELECT [text field], [text field] FROM ...
    /// </summary>
    /// <param name="aCbo"></param>
    /// <param name="aCn"></param>
    /// <param name="aSQLText"></param>
    public static void FillListTextTextFromDB(ComboBox aCbo, OleDbCommand aCmd, bool aIsClearBeforeFill)
    {
      try
      {
        OleDbDataReader dReader = null;
        try
        {
          dReader = aCmd.ExecuteReader();
          if (aIsClearBeforeFill)
            aCbo.Items.Clear();
          while (dReader.Read())
            aCbo.Items.Add(new _ListBoxTextItem(dReader[0].ToString(), dReader[1].ToString()));
        }
        finally
        {
          if (dReader != null)
            dReader.Close();
        }
      }
      catch { }
    }

    /// <summary>
    /// «аполнение ComboBox из запроса следующего формата:
    /// SELECT [code field], [text field] FROM .... 
    /// </summary>
    /// <param name="aCbo">ComboBox</param>
    /// <param name="aCn">Connection with DB</param>
    /// <param name="aSQLText">SQL запрос</param>
    #region FillListCodeTextFromDB
    public static void FillListCodeTextFromDB(ComboBox aCbo, OleDbConnection aCn, string aSQLText)
    {
      FillListCodeTextFromDB(aCbo, aCn, aSQLText, false);
    }
    public static void FillListCodeTextFromDB(ComboBox aCbo, OleDbConnection aCn, string aSQLText, bool aIsFirstEmpty)
    {
      FillListCodeTextFromDB(aCbo, new OleDbCommand(aSQLText, aCn), true, aIsFirstEmpty);
    }
    public static void FillListCodeTextFromDB(ComboBox aCbo, OleDbCommand aCmd, bool aIsClearBeforeFill, bool aIsFirstEmpty)
    {
      try
      {
        OleDbDataReader dReader = null;
        try
        {
          dReader = aCmd.ExecuteReader();
          if (aIsClearBeforeFill)
            aCbo.Items.Clear();
          if (aIsFirstEmpty)
            aCbo.Items.Add(new _ListBoxItem(0, string.Empty));
          while (dReader.Read())
            aCbo.Items.Add(new _ListBoxItem(Convert.ToInt32(dReader[0]), dReader[1].ToString()));
        }
#if DEBUG
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
#endif
        finally
        {
          if (dReader != null)
            dReader.Close();
        }
      }
      catch { }
    }
    #endregion

    /// <summary>
    /// «аполнение ComboBox из запроса следующего формата:
    /// SELECT [place field], [code field], [text field] FROM .... 
    /// </summary>
    /// <param name="aCbo">ComboBox</param>
    /// <param name="aCn">Connection with DB</param>
    /// <param name="aSQLText">SQL запрос</param>
    #region FillListPCTextFromDB
    public static void FillListPCTextFromDB(ComboBox aCbo, OleDbConnection aCn, string aSQLText)
    {
      FillListPCTextFromDB(aCbo, new OleDbCommand(aSQLText, aCn), true);
    }
    public static void FillListPCTextFromDB(ComboBox aCbo, OleDbConnection aCn, string aSQLText, bool aIsClearBeforeFill)
    {
      FillListPCTextFromDB(aCbo, new OleDbCommand(aSQLText, aCn), aIsClearBeforeFill);
    }
    public static void FillListPCTextFromDB(ComboBox aCbo, OleDbCommand aCmd)
    {
      FillListPCTextFromDB(aCbo, aCmd, true);
    }
    public static void FillListPCTextFromDB(ComboBox aCbo, OleDbCommand aCmd, bool aIsClearBeforeFill)
    {
      FillListPCTextFromDB(aCbo, aCmd, aIsClearBeforeFill, false);
    }
    public static void FillListPCTextFromDB(ComboBox aCbo, OleDbCommand aCmd, bool aIsClearBeforeFill, bool aIsFirstEmpty)
    {
      try
      {
        OleDbDataReader dReader = null;
        try
        {
          dReader = aCmd.ExecuteReader();
          if (aIsClearBeforeFill)
            aCbo.Items.Clear();
          if (aIsFirstEmpty)
            aCbo.Items.Add(new _ListBoxPCItem(PlaceCode.Empty, string.Empty));
          while (dReader.Read())
            aCbo.Items.Add(new _ListBoxPCItem(Convert.ToInt32(dReader[0]), Convert.ToInt32(dReader[1]), dReader[2].ToString()));
        }
#if DEBUG
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
#endif
        finally
        {
          if (dReader != null)
            dReader.Close();
        }
      }
      catch { }
    }
    #endregion

    public static void SelectText(ComboBox aCbo, string aText)
    {
      foreach (_ListBoxTextItem ci in aCbo.Items)
      {
        if (ci.text1.Equals(aText))
          aCbo.SelectedItem = ci;
      }
    }
    public static void SelectCode(ComboBox aCbo, int aCode)
    {
      foreach (_ListBoxItem ci in aCbo.Items)
      {
        if (ci.code == aCode)
          aCbo.SelectedItem = ci;
      }
    }
    public static void SelectPC(ComboBox aCbo, PlaceCode aPC)
    {
      foreach (_ListBoxPCItem pci in aCbo.Items)
      {
        if (pci.pPC.Equals(aPC))
          aCbo.SelectedItem = pci;
      }
    }

  }
}
