using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.DataGridColumnStyle
{
	public class DataGridCheckBoxColumn : System.Windows.Forms.DataGridBoolColumn
	{
		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		private bool isEditing;

		protected CheckBox										mCB;

		public CheckBox											pCheckBox
		{
			set { mCB = value; }
			get { return mCB;  }
		}

		public DataGridCheckBoxColumn()
		{
			mCB = new CheckBox();
		}

		protected override void Abort(int rowNum)
		{
			isEditing = false;
			RemoveEventHandlerCheckBoxChanged();
			Invalidate();
		}

		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
			mCB.Bounds = Rectangle.Empty;
			RemoveEventHandlerCheckBoxChanged();

			if (!isEditing)
				return true;

			isEditing = false;

			try 
			{
				SetColumnValueAtRow(dataSource, rowNum, mCB.Checked);
			} 
			catch (Exception) // ex) 
			{
				Abort(rowNum);
				return false;
			}

			Invalidate();
			return true;
		}

		protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly,	string instantText, bool cellIsVisible)
		{
			if (isEditing)
				return;


			object value = GetColumnValueAtRow(source, rowNum);
			if (value != null && value != System.DBNull.Value && value.ToString().Equals("+"))
					mCB.Checked = true;
			else 
					mCB.Checked = false;

			if (cellIsVisible) 
			{
				mCB.Bounds = new Rectangle(bounds.X+2, bounds.Y+2 , bounds.Width-2, bounds.Height-2);
				mCB.Visible = true;
				isEditing = true;
				base.ColumnStartedEditing(mCB);
				AddEventHandlerCheckBoxChanged();
			} 
			else 
			{
				mCB.Visible = false;
			}

			if (mCB.Visible)
			{
				DataGridTableStyle.DataGrid.Invalidate(bounds);
				mCB.Focus();
			}
		}

		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			return new Size(100, (int)mCB.Font.SizeInPoints+20);
		}

		protected override int GetMinimumHeight() 
		{
			return (int)mCB.Font.SizeInPoints+20;
		}

		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			return (int)mCB.Font.SizeInPoints+20;
		}

		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
		{
			Paint(g, bounds, source, rowNum, false);
		}
		protected override void Paint(Graphics g, Rectangle bounds,	CurrencyManager source, int rowNum,	bool alignToRight)
		{
			Paint(g, bounds, source, rowNum, Brushes.Red, Brushes.Blue, alignToRight);
		}
		protected override void Paint(Graphics g, Rectangle bounds,	CurrencyManager source, int rowNum,	Brush backBrush, Brush foreBrush, bool alignToRight)
		{
			object value = this.GetColumnValueAtRow( source, rowNum );
			string text;
			if ( value == null || value == DBNull.Value ) 
				text = this.NullText;
			else 
				text = value.ToString();

			Rectangle rect = bounds;
			g.FillRectangle(backBrush,rect);
			rect.Offset(2, 2);
			rect.Height -= 2;
			rect.Width -= 2;
			g.DrawString(text, this.DataGridTableStyle.DataGrid.Font, foreBrush, rect);
		}

		protected override void SetDataGridInColumn(DataGrid value) 
		{
			base.SetDataGridInColumn(value);
			mCB.Visible = false;
			if (mCB.Parent != null) 
			{
				mCB.Parent.Controls.Remove (mCB);
			}
			if (value != null) 
			{
				value.Controls.Add(mCB);
			}
		}

		private void AddEventHandlerCheckBoxChanged ()
		{
			mCB.CheckedChanged += new EventHandler(DoCheckBoxChanged);
		}

		private void RemoveEventHandlerCheckBoxChanged ()
		{
			mCB.CheckedChanged -= new EventHandler(DoCheckBoxChanged);
		}

		private void DoCheckBoxChanged(object sender, EventArgs e) 
		{
			if (!isEditing)
			{
				isEditing = true;
				base.ColumnStartedEditing(mCB);
			}
		}

		protected override object GetColumnValueAtRow(CurrencyManager source, int rowNum)
		{
			object obj = base.GetColumnValueAtRow (source, rowNum);
			string pdc = "-";
			try {
				if (Convert.ToBoolean(obj))
					pdc = "+";
			} 
			catch {}
			return pdc;
		}

		protected override void SetColumnValueAtRow (CurrencyManager source, int rowNum, object value)
		{
			string ss = value.ToString();
			if (ss.Length > 0 && ss.Equals("+"))
				base.SetColumnValueAtRow(source, rowNum, true);
		}
	}
}
