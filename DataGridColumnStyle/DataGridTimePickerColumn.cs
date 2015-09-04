using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace CommandAS.Tools.DataGridColumnStyle
{
	// This example shows how to create your own column style that
	// hosts a control, in this case, a DateTimePicker.
	public class DataGridTimePickerColumn : System.Windows.Forms.DataGridColumnStyle
	{
		private DateTimePicker myDateTimePicker = new DateTimePicker();
		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		private bool isEditing;

		public DateTimePickerFormat				pFormat
		{
			set {	myDateTimePicker.Format = value; }
			get	{	return myDateTimePicker.Format;	 }
		}

		public DataGridTimePickerColumn() : base() 
		{
			myDateTimePicker.Visible = false;
      myDateTimePicker.Format=DateTimePickerFormat.Custom;
      myDateTimePicker.CustomFormat="dd.MM.yyyy";
		}
		
		protected override void Abort(int rowNum)
		{
			isEditing = false;
			myDateTimePicker.ValueChanged -= new EventHandler(TimePickerValueChanged);
			Invalidate();
		}

		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
			myDateTimePicker.Bounds = Rectangle.Empty;
			myDateTimePicker.ValueChanged -= new EventHandler(TimePickerValueChanged);

			if (!isEditing)
				return true;

			isEditing = false;

			try 
			{
				DateTime value = myDateTimePicker.Value;
				SetColumnValueAtRow(dataSource, rowNum, value);
			} 
			catch (Exception) 
			{
				Abort(rowNum);
				return false;
			}

			Invalidate();
			return true;
		}

		protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly,	string instantText, bool cellIsVisible)
		{
			DateTime value = DateTime.Today;
			try
			{
				value = (DateTime) GetColumnValueAtRow(source, rowNum);
			}
			catch {}

      if (cellIsVisible) 
			{
				myDateTimePicker.Bounds = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 4);
				myDateTimePicker.Value = value;
				myDateTimePicker.Visible = true;
				myDateTimePicker.ValueChanged += new EventHandler(TimePickerValueChanged);
			} 
			else 
			{
				myDateTimePicker.Value = value;
				myDateTimePicker.Visible = false;
			}

			if (myDateTimePicker.Visible)
				DataGridTableStyle.DataGrid.Invalidate(bounds);
		}

    //protected override void ColumnStartedEditing(Control editingControl)
    //{
    //  editingControl.Visible=true;
    //  editingControl.BringToFront();
    //  editingControl.Focus();
    //}

		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			return new Size(100, myDateTimePicker.PreferredHeight + 4);
		}

		protected override int GetMinimumHeight() 
		{
			return myDateTimePicker.PreferredHeight + 4;
		}

		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			return myDateTimePicker.PreferredHeight + 4;
		}

		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum) 
		{
			Paint(g, bounds, source, rowNum, false);
		}
		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight) 
		{
			Paint(
				g,bounds, 
				source, 
				rowNum, 
				Brushes.Red, 
				Brushes.Blue, 
				alignToRight);
		}
		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight) 
		{
			DateTime date = DateTime.Today;
			try
			{
				date = (DateTime) GetColumnValueAtRow(source, rowNum);
			}
			catch {}

			Rectangle rect = bounds;
			g.FillRectangle(backBrush,rect);
			rect.Offset(0, 2);
			rect.Height -= 2;
			g.DrawString(date.ToString("d"), this.DataGridTableStyle.DataGrid.Font, foreBrush, rect);
		}

		protected override void SetDataGridInColumn(DataGrid value) 
		{
			base.SetDataGridInColumn(value);
			if (myDateTimePicker.Parent != null) 
			{
				myDateTimePicker.Parent.Controls.Remove 
					(myDateTimePicker);
			}
			if (value != null) 
			{
				value.Controls.Add(myDateTimePicker);
			}
		}

		private void TimePickerValueChanged(object sender, EventArgs e) 
		{
			this.isEditing = true;
			base.ColumnStartedEditing(myDateTimePicker);
		}
	}
}
