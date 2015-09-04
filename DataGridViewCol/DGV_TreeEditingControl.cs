using System;
using System.Windows.Forms;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.DataGridViewCol
{
  public class DGV_TreeEditingControl : CASSelectFromTV, IDataGridViewEditingControl
  {
    private DataGridView _dgv;
    private bool _valueChanged = false;
    private int _rowIndex;

    public DGV_TreeEditingControl()
    {
      //--this.Format = DateTimePickerFormat.Short;
    }

    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
    // property.
    public virtual object EditingControlFormattedValue
    {
      get
      {
        return pItemPC.PlaceDelimCode;
      }
      set
      {
        String newValue = value as String;
        if (newValue != null)
        {
          this.SetItem(PlaceCode.PDC2PlaceCode(newValue));

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
      this.Font = dataGridViewCellStyle.Font;
      //this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
      //this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
    }

    // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
    // property.
    public int EditingControlRowIndex
    {
      get
      {
        return _rowIndex;
      }
      set
      {
        _rowIndex = value;
      }
    }

    // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
    // method.
    public bool EditingControlWantsInputKey(
        Keys key, bool dataGridViewWantsInputKey)
    {
      // Let the DateTimePicker handle the keys listed.
      switch (key & Keys.KeyCode)
      {
        case Keys.Left:
        case Keys.Up:
        case Keys.Down:
        case Keys.Right:
        case Keys.Home:
        case Keys.End:
        case Keys.PageDown:
        case Keys.PageUp:
          return true;
        default:
          return false;
      }
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
        return _dgv;
      }
      set
      {
        _dgv = value;
      }
    }

    // Implements the IDataGridViewEditingControl
    // .EditingControlValueChanged property.
    public bool EditingControlValueChanged
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
        return base.Cursor;
      }
    }

    protected override void onSelectedItem(object sender, System.EventArgs e)
    {
      if (tv == null)
        return;

      // Notify the DataGridView that the contents of the cell
      // have changed.
      _valueChanged = true;
      this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
      base.onSelectedItem(sender, e);
    }

    protected override void OnTextChanged(EventArgs e)
    {
      if (tv != null)
      {
        _valueChanged = true;
        this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
      }
      base.OnTextChanged(e);
    }
  }
}
