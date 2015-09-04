using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.DataGridViewCol
{
  public partial class DGV_OpenDateCol : DataGridViewColumn
  {
    public DGV_OpenDateCol()
      : base(new DGV_OpenDateCell())
    {
    }

    public override DataGridViewCell CellTemplate
    {
      get
      {
        return base.CellTemplate;
      }
      set
      {
        // Ensure that the cell used for the template is a DataGridViewCalendarCell.
        if (value != null &&
            !value.GetType().IsAssignableFrom(typeof(DGV_OpenDateCell)))
        {
          throw new InvalidCastException("Must be a DGV_OpenDateCell");
        }
        base.CellTemplate = value;
      }
    }
  }

  public class DGV_OpenDateCell : DataGridViewTextBoxCell
  {
    /// <summary>
    /// Бесконечность с точки зрения БД InfoBook
    /// </summary>
    //public static DateTime DATE_INFINITY
    //{
    //  get { return (new DateTime(2100, 1, 1)); }
    //}

    public DGV_OpenDateCell()
      : base()
    {
    }

    public override void InitializeEditingControl(int rowIndex, object
        initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
      // Set the value of the editing control to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
      DGV_OpenEditEditingControl ctl = DataGridView.EditingControl as DGV_OpenEditEditingControl;

      if (this.Value.ToString().Length != 0)
      {
        ctl.Text = this.Value.ToString();
        ctl.SetCursorToBegin();
      }
    }

    public override Type EditType
    {
      get
      {
        // Return the type of the editing contol that DataGridViewCalendarCell uses.
        return typeof(DGV_OpenEditEditingControl);
      }
    }

    public override Type ValueType
    {
      get
      {
        // Return the type of the value that DataGridViewCalendarCell contains.
        return typeof(string);
      }
    }

    public override object DefaultNewRowValue
    {
      get
      {
        // Use the current date and time as the default value.
        //return OpenDate.GetString(DateTime.Today);
        return string.Empty;
      }
    }

    protected override object GetFormattedValue(
      object val, int rowIndex, ref DataGridViewCellStyle cellStyle,
      System.ComponentModel.TypeConverter valueTypeConverter,
      System.ComponentModel.TypeConverter formattedValueTypeConverter,
      DataGridViewDataErrorContexts context)
    {
      object ret = base.GetFormattedValue(val, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
      ret = OpenDate.GetRusFromOpenDate(ret.ToString());
      return ret;
    }
    public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
    {
      object ret = base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
      ret = OpenDate.GetString(ret.ToString());
      return ret;
    }

  }

  class DGV_OpenEditEditingControl : ucOpenDate, IDataGridViewEditingControl
  {
    DataGridView dataGridView;
    private bool valueChanged = false;
    int rowIndex;

    public DGV_OpenEditEditingControl()
    {
      this.pCalendarButtonTabStop = false;
    }

    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
    // property.
    public object EditingControlFormattedValue
    {
      get
      {
        return this.pFormatString;
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
      //this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
      //this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
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

    protected override void OnValueChanged(EventArgs eventargs)
    {
      // Notify the DataGridView that the contents of the cell
      // have changed.
      valueChanged = true;
      this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
      base.OnValueChanged(eventargs);
    }
  }
}
