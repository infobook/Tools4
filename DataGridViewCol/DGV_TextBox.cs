using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommandAS.Tools.DataGridViewCol
{
  public class DGV_TextBoxColumn : DataGridViewColumn
  {

    public AutoCompleteStringCollection pAC
    {
      get
      {
        AutoCompleteStringCollection ret = null;
        if (this.TextACCellTemplate != null)
          ret = this.TextACCellTemplate.pAC;
        return ret;
      }
      set
      {
        if (this.TextACCellTemplate == null)
          return;
        this.TextACCellTemplate.pAC = value;
      }
    }

    public AutoCompleteMode pACMode
    {
      get
      {
        AutoCompleteMode ret = AutoCompleteMode.None;
        if (this.TextACCellTemplate != null)
          ret = this.TextACCellTemplate.pACMode;
        return ret;
      }
      set
      {
        if (this.TextACCellTemplate == null)
          return;
        this.TextACCellTemplate.pACMode = value;
      }
    }

    public AutoCompleteSource pACSource
    {
      get
      {
        AutoCompleteSource ret = AutoCompleteSource.None;
        if (this.TextACCellTemplate != null)
          ret = this.TextACCellTemplate.pACSource;
        return ret;
      }
      set
      {
        if (this.TextACCellTemplate == null)
          return;
        this.TextACCellTemplate.pACSource = value;
      }
    }

    public DGV_TextBoxCell TextACCellTemplate
    {
      get
      {
        return (DGV_TextBoxCell)base.CellTemplate;
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
        // Ensure that the cell used for the template is a DGV_TextBoxCell.
        DGV_TextBoxCell acCell = value as DGV_TextBoxCell;
        if (value != null && acCell == null)
          throw new InvalidCastException("Cell must be a DGV_ComboBoxCell");
        base.CellTemplate = acCell;
        //if (value != null)
        //  acCell.TemplateComboBoxColumn = this;
      }
    }

    public DGV_TextBoxColumn()
      : base(new DGV_TextBoxCell())
    {
      //((DGV_TextBoxCell)this.CellTemplate).TemplateComboBoxColumn = this;
    }

  }

  public class DGV_TextBoxCell : DataGridViewTextBoxCell
  {
    public AutoCompleteStringCollection pAC;
    public AutoCompleteMode pACMode;
    public AutoCompleteSource pACSource;

    public DGV_TextBoxCell()
      : base()
    {
      pAC = null;
      pACMode = AutoCompleteMode.None;
      pACSource = AutoCompleteSource.None;
    }

    public override void InitializeEditingControl(int rowIndex, object
        initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
      // Set the value of the editing control to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
          dataGridViewCellStyle);
      DGV_TextBoxEditingControl ctl =
          DataGridView.EditingControl as DGV_TextBoxEditingControl;

      if (ctl != null && pAC != null)
      {
        ctl.AutoCompleteCustomSource = pAC;
        ctl.AutoCompleteMode = pACMode;
        ctl.AutoCompleteSource = pACSource;
      }
    }

    public override Type EditType
    {
      // Return the type of the editing contol that DGV_NodeCell uses.
      get { return typeof(DGV_TextBoxEditingControl); }
    }

    public override object Clone()
    {
      DGV_TextBoxCell cell = (DGV_TextBoxCell)base.Clone();
      cell.pAC = this.pAC;
      cell.pACMode = this.pACMode;
      cell.pACSource = this.pACSource;
      return cell;
    }
  }

  class DGV_TextBoxEditingControl : TextBox, IDataGridViewEditingControl
  {
    DataGridView dataGridView;
    private bool valueChanged = false;
    int rowIndex;

    public DGV_TextBoxEditingControl()
    {
    }

    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
    // property.
    public object EditingControlFormattedValue
    {
      get
      {
        return this.Text;
      }
      set
      {
        this.Text = value.ToString();
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
      this.Font = dataGridViewCellStyle.Font;
      this.ForeColor = dataGridViewCellStyle.ForeColor;
      this.BackColor = dataGridViewCellStyle.BackColor;
    }

    // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
    // property.
    public int EditingControlRowIndex
    {
      get
      {
        return rowIndex;
      }
      set
      {
        rowIndex = value;
      }
    }

    // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
    // method.
    public bool EditingControlWantsInputKey(
        Keys key, bool dataGridViewWantsInputKey)
    {
      return true;

      // Let the DateTimePicker handle the keys listed.
      //switch (key & Keys.KeyCode)
      //{
      //  case Keys.Left:
      //  case Keys.Up:
      //  case Keys.Down:
      //  case Keys.Right:
      //  case Keys.Home:
      //  case Keys.End:
      //  case Keys.PageDown:
      //  case Keys.PageUp:
      //    return true;
      //  default:
      //    return false;
      //}
    }

    // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
    // method.
    public void PrepareEditingControlForEdit(bool selectAll)
    {
      // No preparation needs to be done.
    }

    // Implements the IDataGridViewEditingControl
    // .RepositionEditingControlOnValueChange property.
    public bool RepositionEditingControlOnValueChange
    {
      get
      {
        return false;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingControlDataGridView property.
    public DataGridView EditingControlDataGridView
    {
      get
      {
        return dataGridView;
      }
      set
      {
        dataGridView = value;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingControlValueChanged property.
    public bool EditingControlValueChanged
    {
      get
      {
        return valueChanged;
      }
      set
      {
        valueChanged = value;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingPanelCursor property.
    public Cursor EditingPanelCursor
    {
      get
      {
        return base.Cursor;
      }
    }

    protected override void OnTextChanged(EventArgs e)
    {
      // Notify the DataGridView that the contents of the cell
      // have changed.
      valueChanged = true;
      this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
      base.OnTextChanged(e);
    }
  }

}
