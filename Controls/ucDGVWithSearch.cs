using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommandAS.Tools.Forms;

namespace CommandAS.Tools.Controls
{
  public partial class ucDGVWithSearch : DataGridView
  {
//    public event EventHandler SearchByWord;
    private dlgFind _wndSearch;
    private int _position;
    /// <summary>
    /// Выделять ли строку при смене позиции в таблице - !!ЛИБО ВЫДЕЛЯТЬ СТРОКУ, ЛИБО СТОЛБЕЦ!!
    /// </summary>
    public bool pSelectRow
    {
      set
      {
        if (value) this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
//        else this.SelectionMode = DataGridViewSelectionMode.CellSelect;
      }
    }
    /// <summary>
    /// Выделять ли столбец при смене позиции в таблице - !!ЛИБО ВЫДЕЛЯТЬ СТРОКУ, ЛИБО СТОЛБЕЦ!!
    /// </summary>
    public bool pSelectCol
    {
      set 
      {
        if (value) this.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
//        else this.SelectionMode = DataGridViewSelectionMode.CellSelect;
      }
    }

    public ucDGVWithSearch()
    {
      _position = -1;
      pSelectCol = false;
      pSelectRow = false;
      InitializeComponent();

      this.KeyPress += new KeyPressEventHandler(OnKeyPress);
      this.CurrentCellChanged += new EventHandler(OnCurrentCellChanged);
    }

    public void ShowDialogForm()
    {
      if (_wndSearch == null)
      {
        _wndSearch = new dlgFind();
        _wndSearch.toSearch += new EventHandler(OnSearch);
      }
      _wndSearch.pCmdDR = DialogResult.None;
      _wndSearch.ShowDialog();
    }

    void OnCurrentCellChanged(object sender, EventArgs e)
    {
      try
      {
//        this.Rows[this.CurrentRow.Index].Selected = pSelectRow;
        this._position = this.CurrentRow.Index;
      }
      catch
      {
        this._position = 0;
      }
    }

    void OnKeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)6)
      {
        if (_wndSearch == null)
        {
          _wndSearch = new dlgFind();
          _wndSearch.toSearch += new EventHandler(OnSearch);
        }
        _wndSearch.pCmdDR = DialogResult.None;
        _wndSearch.ShowDialog();
      }
    }

    void OnSearch(object sender, EventArgs e)
    {
      bool flag = false;
      int index = this._position;
      string txt2Find = _wndSearch.pFindText;
      bool isMatchCase = _wndSearch.pIsMatchCase;
      if (!isMatchCase) txt2Find = txt2Find.ToLower();

      switch (_wndSearch.pIsDirectionForward)
      {
        case true:
          for (int i = index + 1; i < this.Rows.Count; i++)
          {
            if (Find(i, txt2Find, isMatchCase, _wndSearch.pIsWordWhole))
            {
              index = i;
              flag = true;
              this._position = index;
              this.Rows[index].Selected = true;

              this.FirstDisplayedScrollingRowIndex = index;
              //  или можно вот так:
//              this.CurrentCell = this.Rows[index].Cells[3];
              break;
            }
          }
          break;
        default:
          for (int i = index - 1; i >= 0; i--)    //_dgvMain.Rows.Count - 
          {
            if (Find(i, txt2Find, isMatchCase, _wndSearch.pIsWordWhole))
            {
              index = i;
              flag = true;
              this._position = index;
              this.Rows[index].Selected = true;
              this.FirstDisplayedScrollingRowIndex = index;
              break;
            }
          }
          break;
      }
      if (!flag && index == 0)
        MessageBox.Show("Ничего не найдено!");
      if (!flag && index > 0)
        MessageBox.Show("Поиск закончен");
    }

    private bool Find(int aIndex, string aTxt2Find, bool aIsMatchCase, bool aIsTotalEq)
    {
      string aCurStr = string.Empty;

      foreach (DataGridViewCell dgvc in this.Rows[aIndex].Cells)
      {
        if (!aIsMatchCase)
          aCurStr = dgvc.FormattedValue.ToString().ToLower();
        else
          aCurStr = dgvc.FormattedValue.ToString();

        if (aIsTotalEq)
        {
          if (aCurStr.Equals(aTxt2Find))
            return true;
        }
        else
        {
          if (aCurStr.Contains(aTxt2Find))
            return true;
        }
      }
      return false;
    }

  }
}
