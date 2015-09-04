using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.DataGridColumnStyle
{
	/// <summary>
	/// 
	/// </summary>
	public class DataGridSelectFromColumn : System.Windows.Forms.DataGridColumnStyle
	{
		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		private bool isEditing;

		protected ucSelectFrom				mSel;

		public PlaceCode							pPCSelectedItem
		{
			get { return mSel.pSelectedItem; }
		}

		public event EventHandler ValueChanged;

		public DataGridSelectFromColumn() : base()
		{
			mSel = new ucSelectFrom();
			mSel.Visible = false;
			isEditing = false;
		}

		protected override void Abort(int rowNum)
		{
			isEditing = false;
			mSel.OnControlChanged -= new EventHandler(OpenDateValueChanged);
			Invalidate();
		}

		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
			mSel.Bounds = Rectangle.Empty;
         
			mSel.OnControlChanged -= new EventHandler(OpenDateValueChanged);

			if (!isEditing)
				return true;

			isEditing = false;

			try 
			{
				SetColumnValueAtRow(dataSource, rowNum, mSel.pSelectedItem.PlaceDelimCode);
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
			if (isEditing)
				return;
			
			if (cellIsVisible) 
			{
				mSel.Bounds = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width, bounds.Height);
				mSel.Visible = true;
				mSel.OnControlChanged += new EventHandler(OpenDateValueChanged);
				object value = base.GetColumnValueAtRow( source, rowNum );
				if ( value == null ) 
					mSel.pSelectedItem = new PlaceCode(0, 0);
				else
					mSel.pSelectedItem = PlaceCode.PDC2PlaceCode(value.ToString());
				value = this.GetColumnValueAtRow( source, rowNum );
				if ( value == null ) 
					mSel.pText = string.Empty;
				else
					mSel.pText = value.ToString();
			} 
			else 
			{
				mSel.Visible = false;
			}


			if (mSel.Visible)
			{
				isEditing = true;
				base.ColumnStartedEditing(mSel);
				DataGridTableStyle.DataGrid.Invalidate(bounds);
			}
		}

		protected override Size GetPreferredSize(Graphics g, object value)
		{
			return new Size(100, mSel.Height + 6);
		}

		protected override int GetMinimumHeight() 
		{
			return mSel.Height + 6;
		}

		protected override int GetPreferredHeight(Graphics g, object value)
		{
			return mSel.Height + 6;
		}

		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
		{
			Paint(g, bounds, source, rowNum, false);
		}
		protected override void Paint(Graphics g, Rectangle bounds,	CurrencyManager source, int rowNum,	bool alignToRight)
		{
			Paint(g, bounds, source, rowNum, Brushes.Red, Brushes.Blue, alignToRight);
		}
		protected override void Paint(Graphics g, Rectangle bounds,	CurrencyManager source, int rowNum,	Brush backBrush, Brush foreBrush,	bool alignToRight)
		{
			object value = this.GetColumnValueAtRow( source, rowNum );
			string text;
			if ( value == null || value == DBNull.Value ) 
				text = this.NullText;
			else 
				text = value.ToString();

			Rectangle rect = bounds;
			g.FillRectangle(backBrush,rect);
			rect.Offset(0, 2);
			rect.Height += 4;
			g.DrawString(text, this.DataGridTableStyle.DataGrid.Font, foreBrush, rect);
		}

		protected override void SetDataGridInColumn(DataGrid value) 
		{
			base.SetDataGridInColumn(value);
			if (mSel.Parent != null)
			{
				mSel.Parent.Controls.Remove (mSel);
			}
			if (value != null)
			{
				value.Controls.Add(mSel);
			}
		}

		private void OpenDateValueChanged(object sender, EventArgs e) 
		{
			if (!isEditing)
			{
				isEditing = true;
				base.ColumnStartedEditing(mSel);
				if (ValueChanged != null)
					ValueChanged(this, new EventArgs());
			}
		}
	}
}
