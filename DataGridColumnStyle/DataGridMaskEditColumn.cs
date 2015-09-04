using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.DataGridColumnStyle
{
	public class DataGridMaskEditColumn : System.Windows.Forms.DataGridColumnStyle
	{
		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		private bool isEditing;

		protected MaskEdit						mME;

		public string									pEditMask
		{
			set { mME.EditMask = value; }
			get { return mME.EditMask;  }
		}

		public event EventHandler ValueChanged;

		public DataGridMaskEditColumn() : base() 
		{
			mME = new MaskEdit();
			//mME.EditMask = "A";
			mME.BorderStyle = BorderStyle.None;
			mME.Visible = false;
		}

		protected override void Abort(int rowNum)
		{
			isEditing = false;
			RemoveEventHandlerMaskEditChanged();
			Invalidate();
		}

		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
			mME.Bounds = Rectangle.Empty;
			RemoveEventHandlerMaskEditChanged();

			if (!isEditing)
				return true;

			isEditing = false;

			try 
			{
				SetColumnValueAtRow(dataSource, rowNum, mME.Text);
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
			if (value != null && value != System.DBNull.Value && value.ToString().Length > 0) 
				mME.Text = value.ToString();
			else 
				mME.Text = mME.EditMask; //string.Empty;

			if (cellIsVisible) 
			{
				mME.Bounds = new Rectangle(bounds.X+2, bounds.Y+2 , bounds.Width-2, bounds.Height-2);
				mME.Visible = true;
				isEditing = true;
				base.ColumnStartedEditing(mME);
				AddEventHandlerMaskEditChanged();
			} 
			else 
			{
				mME.Visible = false;
			}

			if (mME.Visible)
			{
				DataGridTableStyle.DataGrid.Invalidate(bounds);
				mME.Select(0,1);
				mME.Focus();
			}
		}

		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			return new Size(100, mME.PreferredHeight);
		}

		protected override int GetMinimumHeight() 
		{
			return mME.PreferredHeight;
		}

		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			return mME.PreferredHeight;
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
			mME.Visible = false;
			if (mME.Parent != null) 
			{
				mME.Parent.Controls.Remove (mME);
			}
			if (value != null) 
			{
				value.Controls.Add(mME);
			}
		}

		private void AddEventHandlerMaskEditChanged ()
		{
			mME.TextChanged += new EventHandler(DoMaskEditChanged);
		}

		private void RemoveEventHandlerMaskEditChanged ()
		{
			mME.TextChanged -= new EventHandler(DoMaskEditChanged);
		}

		private void DoMaskEditChanged(object sender, EventArgs e) 
		{
			if (!isEditing)
			{
				isEditing = true;
				base.ColumnStartedEditing(mME);
				if (ValueChanged != null)
					ValueChanged(this, new EventArgs());
			}
		}

//		protected override object GetColumnValueAtRow(CurrencyManager source, int rowNum)
//		{
//			object obj = base.GetColumnValueAtRow (source, rowNum);
//			string pdc = string.Empty;
//			try {pdc = obj.ToString();} catch {}
//			return pdc;
//		}
//
//		protected override void SetColumnValueAtRow (CurrencyManager source, int rowNum, object value)
//		{
//			string ss = value.ToString();
//			if (ss.Length > 0)
//				base.SetColumnValueAtRow(source, rowNum, ss);
//		}

	}
}
