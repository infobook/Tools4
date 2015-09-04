using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
//using CommandAS.Tools;

namespace CommandAS.Tools.Controls
{
  public partial class ComboBoxSearchInRange : ComboBox
  {

		/// <summary>
		/// Позиция в искомой строке. Начиная с 0.
		/// </summary>
		public int pFindByLettersStart = 0;
		/// <summary>
		/// Длина диапазона поиска (символов). Если = 0 - до конца строки.
		/// </summary>
		public int pFindByLettersLength = 0;
    /// <summary>
    /// Чуствительность поиска к регистру. По умолчанию - нет.
    /// </summary>
    public bool pIsCaseSensitive;

		private string _txt2Find = string.Empty;

    public ComboBoxSearchInRange()
    {
      InitializeComponent();
			//this.Sorted = true;
      pIsCaseSensitive = false;
    }
	
		//  поиск по номеру в составном названии объекта, 
		//  например, машину с номером У 456 ЛМ 77 будет искать по любым вхождениям цифр в номере
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{

      if (Text.Length == 0 || (Text.Length == SelectionLength && ((int)e.KeyChar) > 0x1F))
      {
        _txt2Find = string.Empty;
        if (this.Items.Count > 0)
          this.SelectedIndex = 0;
        this.Select(0, 0);
#if DEBUG
        Helper.Trace("Clear.....");
#endif
      }

      if (((int)e.KeyChar) > 0x1F) // && _txt2Find.Length > pFindByLettersStart)
      {
        //_txt2Find += e.KeyChar.ToString();
        int currPos = SelectionStart + SelectionLength;
        if (currPos > _txt2Find.Length)
          currPos = _txt2Find.Length;
        _txt2Find = _txt2Find.Substring(0, currPos) + e.KeyChar.ToString() + _txt2Find.Substring(currPos);

#if DEBUG
        Helper.Trace("Key press - " + _txt2Find);
#endif
        if (!findItem(_txt2Find, 0))
        {
          Text = _txt2Find;
          Select(currPos + 1, 0);
        }
        e.Handled = true;
      }
      else
      {
        base.OnKeyPress(e);
#if DEBUG
        Helper.Trace("Key press no handled -" + e.KeyChar);
#endif
      }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Home:
        case Keys.End:
        case Keys.Left:
        case Keys.Right:
          _txt2Find = Text;
          //int pos = SelectionStart == 0 ? 0 : SelectionStart - 1;
          //Select(pos, pos);
          //e.Handled = true;
          break;
        // Закомментировал M.Tor 20.07.2011
        // не понял, что это .....................
        //case Keys.Q:          
        //  {
        //      if (e.KeyData == Keys.Q)
        //      {
        //          char c = Convert.ToChar("q");
        //          System.Windows.Forms.KeyPressEventArgs ea = new KeyPressEventArgs(c);
        //          OnKeyPress(ea);
        //      }
        //  }
        //  break;
      }
#if DEBUG
      Helper.Trace("Key Down - " + _txt2Find);
#endif
      base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
      base.OnKeyUp(e);
      switch (e.KeyCode)
      {
        case Keys.Back:
        case Keys.Delete:
          _txt2Find = Text;
          break;
        //case Keys.ControlKey:
        //  Select(SelectionStart + SelectionLength, 0);
        //  _txt2Find = Text;
        //  //e.Handled = true;
        //  break;
        case Keys.F3:
          GotoNextItem();
          break;
      }
#if DEBUG
      Helper.Trace("Key Up - " + _txt2Find);
#endif
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      _txt2Find = Text;
    }

    //protected override void OnSelectedValueChanged(EventArgs e)
    //{
    //  base.OnSelectedValueChanged(e);
    //}
    //  переход к следующему элементу списка, в котором присутствует тоже введенное значение
    public bool GotoNextItem()
    {
      return findItem(SelectedText, this.SelectedIndex + 1);
    }

		private bool findItem(string aSS, int aStartIndex)
		{
      bool ret = false;
			int index = -1;
      int i;
      for (i = aStartIndex; i < this.Items.Count; i++)
			{
        string sSrc = this.Items[i].ToString();
        string sFnd = aSS;
        if (sSrc.Length == 0 || sSrc.Length < sFnd.Length)
          continue;
        if (!pIsCaseSensitive)
        {
          sSrc = sSrc.ToUpper();
          sFnd = sFnd.ToUpper();
        }
        index = -1;
        try
        {
          if (pFindByLettersLength > 0 && pFindByLettersLength < sSrc.Length)
            index = sSrc.IndexOf(sFnd, pFindByLettersStart, pFindByLettersLength);
          else
            index = sSrc.IndexOf(sFnd, pFindByLettersStart);
        }
        catch { }
        if (index != -1)
				{
          ret = true;
          break;
				}
			}
			if (ret)
			{
        this.SelectedIndex = i;
#if DEBUG
        Helper.Trace("found1 : " + SelectedText, Text);
#endif
        this.Select(index, aSS.Length);
#if DEBUG
        Helper.Trace("found2 : " + SelectedText);
#endif
      }
      else
        this.SelectedIndex = -1;


      return ret;
		}
  }
}
