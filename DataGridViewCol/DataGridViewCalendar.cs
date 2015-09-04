using System;
using System.Windows.Forms;
using System.Drawing;

namespace CommandAS.Tools.DataGridViewCol
{
  /// <summary>
  /// From MSDN 2005
  /// ms-help://MS.MSDNQTR.v80.en/MS.MSDN.v80/MS.VisualStudio.v80.en/dv_fxmclictl/html/e79a9d4e-64ec-41f5-93ec-f5492633cbb2.htm
  /// </summary>
  public class DataGridViewCalendarColumn : DataGridViewColumn
  {
    public DataGridViewCalendarColumn() : base(new DataGridViewCalendarCell())
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
            !value.GetType().IsAssignableFrom(typeof(DataGridViewCalendarCell)))
        {
          throw new InvalidCastException("Must be a DataGridViewCalendarCell");
        }
        base.CellTemplate = value;
      }
    }
  }

  public class DataGridViewCalendarCell : DataGridViewTextBoxCell
  {
    /// <summary>
    /// Бесконечность с точки зрения БД InfoBook
    /// </summary>
    public static DateTime DATE_INFINITY
    {
      get { return (new DateTime(2100, 1, 1)); }
    }

    public DataGridViewCalendarCell() : base()
    {
      // Use the short date format.
      //this.Style.Format = "d";
      //correct [M.Tor 25.10.2011]
      this.Style.Format = "dd.MM.yyyy";
    }

    protected override void Paint(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
    {
      DateTime date = DateTime.Today;
      try
      {
        date = (DateTime)value;
      }
      catch { }

      if (date > DATE_INFINITY)
        this.Value = new DateTime(2100, 1, 1);

      base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);


      Rectangle rect = cellBounds;
      rect.Height -= 1;
      rect.Width -= 1;
      if (date == DATE_INFINITY)
      {
        graphics.FillRectangle(new SolidBrush(Color.GhostWhite), rect);
        rect.Offset(30, -5);
        graphics.DrawString("Ґ", new Font("Symbol", 14), new SolidBrush(System.Drawing.Color.Black), rect);
      }
      //else
      //{
      //  graphics.FillRectangle(new SolidBrush(Color.White), rect);
      //  rect.Offset(0, 3);
      //  //rect.Height -= 2;
      //  graphics.DrawString(date.ToString("d"), cellStyle.Font, new SolidBrush(System.Drawing.Color.Black), rect);
      //}
    }

    public override void InitializeEditingControl(int rowIndex, object
        initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
      // Set the value of the editing control to the current cell value.
      base.InitializeEditingControl(rowIndex, initialFormattedValue,
          dataGridViewCellStyle);
      DataGridViewCalendarEditingControl ctl =
          DataGridView.EditingControl as DataGridViewCalendarEditingControl;
      if (this.Value.ToString().Length != 0)
      {
        if (this.Value.GetType() == typeof(DateTime))
          ctl.Value = (DateTime)this.Value;
        else
          ctl.Value = Convert.ToDateTime(this.Value);
      }
      else
        ctl.Value = DateTime.Today;
    }

    public override Type EditType
    {
      get
      {
        // Return the type of the editing contol that DataGridViewCalendarCell uses.
        return typeof(DataGridViewCalendarEditingControl);
      }
    }

    public override Type ValueType
    {
      get
      {
        // Return the type of the value that DataGridViewCalendarCell contains.
        return typeof(DateTime);
      }
    }

    //protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, System.ComponentModel.TypeConverter valueTypeConverter, System.ComponentModel.TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
    //{
    //  return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
    //}

    public override object DefaultNewRowValue
    {
      get
      {
        // Use the current date and time as the default value.
        // Изменено 08/10/2007 MaryM, чтобы не выводить текущую дату в новой строке
        return string.Empty; 
        // return DateTime.Now;
      }
    }
  }

  class DataGridViewCalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
  {
    DataGridView dataGridView;
    private bool valueChanged  = false;
    int rowIndex;

    public DataGridViewCalendarEditingControl()
    {
      //this.Format = DateTimePickerFormat.Short;
      //correct [M.Tor 25.10.2011]
      this.Format = DateTimePickerFormat.Custom;
      this.CustomFormat = "dd.MM.yyyy";
    }

    // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
    // property.
    public object EditingControlFormattedValue
    {
      get
      {
        //return this.Value.ToShortDateString();
        //correct [M.Tor 25.10.2011]
        return this.Value.ToString("dd.MM.yyyy");
        //return this.Value.ToLongDateString();
      }
      set
      {
        String newValue = value as String;
        if (newValue != null)
        {
          this.Value = DateTime.Parse(newValue);
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
      this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
      this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
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
