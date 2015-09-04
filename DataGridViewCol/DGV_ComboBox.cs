using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;
using CommandAS.Tools;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.DataGridViewCol
{
  public class DGV_ComboBoxColumn : DataGridViewColumn
  {
    public ArrayList List
    {
      get
      {
        ArrayList ret = null;
        if (this.ComboBoxCellTemplate != null)
          ret = this.ComboBoxCellTemplate.List;
        return ret;
      }
      set
      {
        if (this.ComboBoxCellTemplate == null)
          return;
        this.ComboBoxCellTemplate.List = value;
      }
    }

    public string DisplayMember
    {
      get
      {
        string ret = string.Empty;
        if (this.ComboBoxCellTemplate != null)
          ret = this.ComboBoxCellTemplate.DisplayMember;
        return ret;
      }
      set
      {
        if (this.ComboBoxCellTemplate == null)
          return;
        this.ComboBoxCellTemplate.DisplayMember = value;
      }
    }

    public string ValueMember
    {
      get
      {
        string ret = string.Empty;
        if (this.ComboBoxCellTemplate != null)
          ret = this.ComboBoxCellTemplate.ValueMember;
        return ret;
      }
      set
      {
        if (this.ComboBoxCellTemplate == null)
          return;
        this.ComboBoxCellTemplate.ValueMember = value;
      }
    }

    [DefaultValue(0)]
    public int MaxDropDownItems
    {
      get
      {
        int ret = 0;
        if (this.ComboBoxCellTemplate != null)
          ret = this.ComboBoxCellTemplate.MaxDropDownItems;
        return ret;
      }
      set
      {
        if (this.ComboBoxCellTemplate == null)
          return;
        this.ComboBoxCellTemplate.MaxDropDownItems = value;
      }
    }
    [DefaultValue(0)]
    public int DropDownWidth
    {
      get
      {
        int ret = 0;
        if (this.ComboBoxCellTemplate != null)
          ret = this.ComboBoxCellTemplate.DropDownWidth;
        return ret;
      }
      set
      {
        if (this.ComboBoxCellTemplate == null)
          return;
        this.ComboBoxCellTemplate.DropDownWidth = value;
      }
    }

    [DefaultValue(0)]
    public int FindByLettersStart
    {
      get
      {
        int ret = 0;
        if (this.ComboBoxCellTemplate != null)
          ret = this.ComboBoxCellTemplate.FindByLettersStart;
        return ret;
      }
      set
      {
        if (this.ComboBoxCellTemplate == null)
          return;
        this.ComboBoxCellTemplate.FindByLettersStart = value;
      }
    }

    [DefaultValue(0)]
    public int FindByLettersLength
    {
      get
      {
        int ret = 0;
        if (this.ComboBoxCellTemplate != null)
          ret = this.ComboBoxCellTemplate.FindByLettersLength;
        return ret;
      }
      set
      {
        if (this.ComboBoxCellTemplate == null)
          return;
        this.ComboBoxCellTemplate.FindByLettersLength = value;
      }
    }

    [DefaultValue(false)]
    public bool FindIsCaseSensitive
    {
      get
      {
        bool ret = false;
        if (this.ComboBoxCellTemplate != null)
          ret = this.ComboBoxCellTemplate.FindIsCaseSensitive;
        return ret;
      }
      set
      {
        if (this.ComboBoxCellTemplate == null)
          return;
        this.ComboBoxCellTemplate.FindIsCaseSensitive = value;
      }
    }

    public DGV_ComboBoxCell ComboBoxCellTemplate
    {
      get
      {
        return (DGV_ComboBoxCell)base.CellTemplate;
      }
    }

    public override DataGridViewCell CellTemplate
    {
      get
      {
        return base.CellTemplate;
      }
      set
      {
        DGV_ComboBoxCell cboCell = value as DGV_ComboBoxCell;
        if (value != null && cboCell == null)
          throw new InvalidCastException("Cell must be a DGV_ComboBoxCell");
        base.CellTemplate = cboCell;
        if (value != null)
        {
          cboCell.TemplateComboBoxColumn = this;
        }
      }
    }

    public event EvH_Text SelectedIndexChanged;
    public event EvH_Text EnterNewText;

    public DGV_ComboBoxColumn(DGV_ComboBoxCell cell)
      : base(cell)
    {
    }

    public DGV_ComboBoxColumn()
      : base(new DGV_ComboBoxCell())
    {
      ((DGV_ComboBoxCell)this.CellTemplate).TemplateComboBoxColumn = this;
    }

    public void OnSelectedIndexChanged(string aValue)
    {
      if (SelectedIndexChanged != null)
        SelectedIndexChanged(this, new EvA_Text(aValue));
    }

    public void OnEnterNewText(string aNewText)
    {
      if (EnterNewText != null)
        EnterNewText(this, new EvA_Text(aNewText));
    }

    public override object Clone()
    {
      DGV_ComboBoxColumn col = (DGV_ComboBoxColumn)base.Clone();
      col.CellTemplate = (DGV_ComboBoxCell)this.CellTemplate.Clone();
      return col;
    }

  }

  public class DGV_ComboBoxCell : DataGridViewTextBoxCell
  {

    //static int PropComboBoxCellDataSource;

    protected ArrayList _al;
    private string _displayMember;
    private string _valueMember;
    private DGV_ComboBoxColumn _templateComboBoxColumn;
    private DGV_ComboBoxEditingControl _editingComboBox;

    private int _ddMaxItems;
    private int _ddWidth;

    private int _findByLettersStart;
    private int _findByLettersLength;
    private bool _findIsCaseSensitive;

    public ArrayList List
    {
      get { return _al; }
      set
      {
        if (_al != value)
        {
          _al = value;
          if (OwnsEditingComboBox(this.RowIndex))
          {
            _editingComboBox.DataSource = value;
            return;
          }
        }
      }
    }
    public string DisplayMember
    {
      get { return _displayMember; }
      set { _displayMember = value; }
    }
    public string ValueMember
    {
      get { return _valueMember; }
      set { _valueMember = value; }
    }
    public DGV_ComboBoxColumn TemplateComboBoxColumn
    {
      get { return _templateComboBoxColumn; }
      set { _templateComboBoxColumn = value; }
    }

    public int MaxDropDownItems
    {
      get { return _ddMaxItems; }
      set { _ddMaxItems = value; }
    }
    public int DropDownWidth
    {
      get { return _ddWidth; }
      set { _ddWidth = value; }
    }

    public int FindByLettersStart
    {
      get { return _findByLettersStart; }
      set { _findByLettersStart = value; }
    }
    public int FindByLettersLength
    {
      get { return _findByLettersLength; }
      set { _findByLettersLength = value; }
    }
    public bool FindIsCaseSensitive
    {
      get { return _findIsCaseSensitive; }
      set { _findIsCaseSensitive = value; }
    }

    /// <summary>
    /// скрытое значение €чейки
    /// added by DSY 24.10.2007
    /// </summary>
    private string _hiddenVal;

    public DGV_ComboBoxCell()
      : base()
    {
      _al = null;
      _displayMember = string.Empty;
      _valueMember = string.Empty;
      _editingComboBox = null;

      _ddMaxItems = 0;
      _ddWidth = 0;
    }

    static DGV_ComboBoxCell()
    {
    }

    public override void InitializeEditingControl(int rowIndex, object
        initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
      // Set the value of the editing control to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

      DGV_ComboBoxEditingControl ctl = DataGridView.EditingControl as DGV_ComboBoxEditingControl;
      if (ctl != null)
      {
        ctl.SelectedIndexChanged -= new EventHandler(DoSelectedIndexChanged);

        ctl.DataSource = null;
        //ctl.Items.Clear();

        ctl.DropDownStyle = ComboBoxStyle.DropDown;
        if (_ddMaxItems > 0)
          ctl.MaxDropDownItems = _ddMaxItems;
        if (_ddWidth > 0)
          ctl.DropDownWidth = _ddWidth;
        ctl.DataSource = _al;
        if (ctl.Items.Count > 0)
        {
          ctl.DisplayMember = _displayMember;
          ctl.ValueMember = _valueMember;
        }
        ctl.pFindByLettersStart = FindByLettersStart;
        ctl.pFindByLettersLength = FindByLettersLength;
        ctl.pIsCaseSensitive = FindIsCaseSensitive;

        string s = initialFormattedValue as string;
        if (s == null)
          s = string.Empty;
        ctl.Text = s;

        _editingComboBox = ctl;
        _editingComboBox.EditingControlRowIndex = rowIndex;

        _editingComboBox.SelectedIndexChanged += new EventHandler(DoSelectedIndexChanged);
      }
    }

    void DoSelectedIndexChanged(object sender, EventArgs e)
    {
      if (_templateComboBoxColumn != null && _editingComboBox != null && _editingComboBox.SelectedValue != null)
      {
        _templateComboBoxColumn.OnSelectedIndexChanged(_editingComboBox.SelectedValue.ToString());
      }
      else
        _templateComboBoxColumn.OnSelectedIndexChanged(string.Empty);
    }

    public override void DetachEditingControl()
    {
      ComboBox cbo = this.DataGridView.EditingControl as ComboBox;
      if (cbo != null && cbo.Text.Length > 0)
      {
        bool flg = true;
        foreach (object oi in cbo.Items)
        {
          string si = oi as string;
          if (si != null && si.Equals(cbo.Text))
          {
            flg = false;
            break;
          }
        }

        if (flg && _templateComboBoxColumn != null)
        {
          _templateComboBoxColumn.OnEnterNewText(cbo.Text);
        }

      }

      if(cbo != null)
        cbo.SelectedIndexChanged -= new EventHandler(DoSelectedIndexChanged);
      
      _editingComboBox = null;
      base.DetachEditingControl();
    }
    
    public override Type EditType
    {
      // Return the type of the editing contol that DGV_NodeCell uses.
      get { return typeof(DGV_ComboBoxEditingControl); }
    }

    public override Type ValueType
    {
      // Return the type of the value that this contains.
      get { return typeof(object); }
    }

    public override Type FormattedValueType
    {
      get
      {
        return typeof(string);
      }
    }

    public override object DefaultNewRowValue
    {
      get { return string.Empty; }
    }

    protected override object GetFormattedValue(
      object val, int rowIndex, ref DataGridViewCellStyle cellStyle,
      System.ComponentModel.TypeConverter valueTypeConverter,
      System.ComponentModel.TypeConverter formattedValueTypeConverter,
      DataGridViewDataErrorContexts context)
    {

//#if DEBUG
//      if (val != null)
//        Helper.Trace("GetFormattedValue formattedValue = " + val.ToString());
//      else
//        Helper.Trace("GetFormattedValue formattedValue = NULL");
//#endif
//      cellStyle.ForeColor = Color.Coral;

      object ret = GetFmtValue(val);
      if (ret == null)
        ret = base.GetFormattedValue(val, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);

//#if DEBUG
//      if (ret != null)
//        Helper.Trace("GetFormattedValue ret = " + ret.ToString());
//      else
//        Helper.Trace("GetFormattedValue ret = NULL");
//#endif
      return ret;
    }

    protected object GetFmtValue(object val)
    {
      object ret = null;

      if (_al.Count > 0)
      {
        Type t = (_al[0]).GetType();
        switch (t.Name)
        {
          case "_ListBoxItem":
            ret = _GetIntItemDisplay(val);
            break;
          case "_ListBoxPCItem":
            ret = _GetPCItemDisplay(val);
            break;
          case "_ListBoxTextItem":
            ret = _GetTextItemDisplay(val);
            break;
          case "_ListBoxDecimalItem":
            ret = _GetDecimalItemDisplay(val);
            break;
        }
      }
      return ret;
    }

    public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
    {
//#if DEBUG
//      if (formattedValue != null)
//        Helper.Trace("ParseFormattedValue formattedValue = " + formattedValue.ToString());
//      else
//        Helper.Trace("ParseFormattedValue formattedValue = NULL");
//#endif
      object ret = null;

      if (_al.Count > 0)
      {
        Type t = (_al[0]).GetType();
        switch (t.Name)
        {
          case "_ListBoxItem":
            ret = _GetIntItemValue(formattedValue);
            break;
          case "_ListBoxPCItem":
            ret = _GetPCItemValue(formattedValue);
            break;
          case "_ListBoxTextItem":
            ret = _GetTextItemValue(formattedValue);
            break;
          case "_ListBoxDecimalItem":
            ret = _GetDecimalItemValue(formattedValue);
            break;
          default:
            ret = base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
            break;
        }
      }

//#if DEBUG
//      if (ret != null)
//        Helper.Trace("ParseFormattedValue ret = " + ret.ToString());
//      else
//        Helper.Trace("ParseFormattedValue ret = NULL");
//#endif
      return ret;
    }

    protected int _GetIntItemValue(object formattedValue)
    {
      int ret = 0;
      foreach (_ListBoxItem item in _al)
      {
        if (item.text.Equals(formattedValue))
        {
          ret = item.code;
          break;
        }
      }
      return ret;
    }
    protected decimal _GetDecimalItemValue(object formattedValue)
    {
      decimal ret = decimal.Zero;
      foreach (_ListBoxDecimalItem item in _al)
      {
        ret = item.decCode;
        break;
      }
      return ret;
    }
    protected string _GetPCItemValue(object formattedValue)
    {
      string ret = PlaceCode.Empty.PlaceDelimCode;
      foreach (_ListBoxPCItem item in _al)
      {
        if (item.pText.Equals(formattedValue))
        {
          ret = item.pPDC;
          break;
        }
      }
      return ret;
    }
    protected string _GetTextItemValue(object formattedValue)
    {
      string ret = PlaceCode.Empty.PlaceDelimCode;
      foreach (_ListBoxTextItem item in _al)
      {
        if (item.TextDisplay.Equals(formattedValue))
        {
          ret = item.TextValue;
          break;
        }
      }
      return ret;
    }

    protected string _GetIntItemDisplay(object aVal)
    {
      string ret = string.Empty;
      int iVal = CASTools.ConvertToInt32Or0(aVal);
      foreach (_ListBoxItem item in _al)
      {
        if (item.code == iVal)
        {
          ret = item.text;
          break;
        }
      }
      return ret;
    }
    protected string _GetDecimalItemDisplay(object aVal)
    {
      string ret = string.Empty;
      decimal iVal = decimal.Zero;

      try {iVal = Convert.ToDecimal(aVal);}
      catch { }

      foreach (_ListBoxDecimalItem item in _al)
      {
        if (item.decCode == iVal)
        {
          ret = item.text;
          break;
        }
      }
      return ret;
    }
    protected string _GetPCItemDisplay(object aVal)
    {
      string ret = string.Empty;
      foreach (_ListBoxPCItem item in _al)
      {
        if (item.pPDC.Equals(aVal))
        {
          ret = item.pText;
          break;
        }
      }
      return ret;
    }
    protected string _GetTextItemDisplay(object aVal)
    {
      string ret = string.Empty;
      foreach (_ListBoxTextItem item in _al)
      {
        if (item.TextValue.Equals(aVal))
        {
          ret = item.TextDisplay;
          break;
        }
      }
      return ret;
    }

    public override object Clone()
    {
      DGV_ComboBoxCell cell = (DGV_ComboBoxCell)base.Clone();
      cell.TemplateComboBoxColumn = this.TemplateComboBoxColumn;
      cell.DisplayMember = this.DisplayMember;
      cell.ValueMember = this.ValueMember;
      cell.List = this.List;
      cell.MaxDropDownItems = this.MaxDropDownItems;
      cell.DropDownWidth = this.DropDownWidth;
      cell.FindByLettersStart = this.FindByLettersStart;
      cell.FindByLettersLength = this.FindByLettersLength;
      cell.FindIsCaseSensitive = this.FindIsCaseSensitive;
      return cell;
    }

    private bool OwnsEditingComboBox(int rowIndex)
    {
      if (rowIndex != -1 && _editingComboBox != null)
        return rowIndex == _editingComboBox.EditingControlRowIndex;
      return false;
    }
  }

  //public class DGV_ComboVEditingControl : DataGridViewComboBoxEditingControl
  //public class DGV_ComboBoxEditingControl : ComboBox, IDataGridViewEditingControl
  public class DGV_ComboBoxEditingControl : ComboBoxSearchInRange, IDataGridViewEditingControl
  {

    private DataGridView _dgv;
    private int _rowIndex;
    private bool _valueChanged = false;

    public DGV_ComboBoxEditingControl()
    {
      this.TabStop = false;
    }

    // Implements the IDataGridViewEditingControl
    // .EditingControlDataGridView property.
    public virtual DataGridView EditingControlDataGridView
    {
      get { return _dgv; }
      set { _dgv = value; }
    }


    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
    // property.
    public virtual object EditingControlFormattedValue
    {
      get
      {
        return this.Text;
      }
      set
      {
        string ssVal;

        ssVal = value as String;
        if (ssVal != null)
        {
          this.Text = ssVal;
          if (String.Compare(ssVal, this.Text, true, CultureInfo.CurrentCulture) != 0)
            this.SelectedIndex = -1;
        }
      }
    }

    // Implements the 
    // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
    public object GetEditingControlFormattedValue(
        DataGridViewDataErrorContexts context)
    {
      return EditingControlFormattedValue;
    }

    // Implements the 
    // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
    public void ApplyCellStyleToEditingControl(
        DataGridViewCellStyle dataGridViewCellStyle)
    {
      Color local0;
      Color local1;

      this.Font = dataGridViewCellStyle.Font;
      local1 = dataGridViewCellStyle.BackColor;
      if (local1.A < 255)
      {
        local0 = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
        this.BackColor = local0;
        _dgv.EditingPanel.BackColor = local0;
      }
      else
        this.BackColor = dataGridViewCellStyle.BackColor;
      this.ForeColor = dataGridViewCellStyle.ForeColor;
//      this.ForeColor = Color.Brown;
    }

    // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
    // property.
    public virtual int EditingControlRowIndex
    {
      get { return _rowIndex; }
      set { _rowIndex = value; }
    }

    // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
    // method.
    public bool EditingControlWantsInputKey(
        Keys key, bool dataGridViewWantsInputKey)
    {
      switch ((Keys)(key & Keys.KeyCode))
      {
        case Keys.Right:
          //if (((RightToLeft == RightToLeft.No) && ((SelectionLength != 0) || (SelectionStart != Text.Length))) || ((RightToLeft == RightToLeft.Yes) && ((SelectionLength != 0) || (SelectionStart == 0))))
            return true;
          break;
        case Keys.Left:
          //if (((RightToLeft == RightToLeft.No) && ((SelectionLength != 0) || (SelectionStart != 0))) || ((RightToLeft == RightToLeft.Yes) && ((SelectionLength != 0) || (SelectionStart == Text.Length))))
            return true;
          break;
        //case Keys.Down:
        //  int i = SelectionStart + SelectionLength;
        //  if (Text.IndexOf("\r\n", i) == -1)
        //    goto label_1;
        //  return true;

        //case Keys.Up:
        //  if ((Text.IndexOf("\r\n") < 0) || ((SelectionStart + SelectionLength) < Text.IndexOf("\r\n")))
        //    goto label_1;
        //  return true;

        case Keys.End:
        case Keys.Home:
          //if (SelectionLength == Text.Length)
            return true;
          break;
        //case Keys.Prior:
        //case Keys.Next:
        //  if (!_valueChanged)
        //    goto label_1;
        //  return true;

        case Keys.Delete:
          if ((SelectionLength <= 0) && (SelectionStart >= Text.Length))
            return true;
          break;
        //case Keys.Return:
        //  if (((key & (Keys.Shift | Keys.Control | Keys.Alt)) != Keys.Shift))
        //    goto label_1;
        //  return true;

        //case Keys.Select:
        //case Keys.Print:
        //case Keys.Execute:
        //case Keys.Snapshot:
        //case Keys.Insert:
        //label_1:
        //  return !dataGridViewWantsInputKey;

        //default:
        //  if ((int)(key & Keys.KeyCode) > 0x1F)
        //    return true;
        //  break;
      }
      

      return false;
    }

    // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
    // method.
    public virtual void PrepareEditingControlForEdit(bool selectAll)
    {
      if (selectAll)
        this.SelectAll();
    }

    // Implements the IDataGridViewEditingControl
    // .RepositionEditingControlOnValueChange property.
    public virtual bool RepositionEditingControlOnValueChange
    {
      get
      {
        return false;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingControlValueChanged property.
    public virtual bool EditingControlValueChanged
    {
      get
      {
        return _valueChanged;
      }
      set
      {
        _valueChanged = value;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingPanelCursor property.
    public Cursor EditingPanelCursor
    {
      get
      {
        return Cursors.Default;
      }
    }

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      this.NotifyDataGridViewOfValueChange();
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
      if (this.SelectedIndex != -1)
        this.NotifyDataGridViewOfValueChange();
      // »справлено 12.01.2009 by M.Tor, раньше было первой строчкой
       base.OnSelectedIndexChanged(e);
   }

    private void NotifyDataGridViewOfValueChange()
    {
      _valueChanged = true;
      _dgv.NotifyCurrentCellDirty(true);
      //DataGridView dgv = this.EditingControlDataGridView;
      //if (dgv != null)
      //  dgv.NotifyCurrentCellDirty(true);

    }
  }

}
