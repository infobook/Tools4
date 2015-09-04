using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using CommandAS.Tools;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.DataGridColumnStyle
{
	public class PicSwitchImage : Control
	{
		protected ImageCollection						mIC;

		public ImageCollection							pImageCollection
		{
			set { mIC = value; }
			get { return mIC;  }
		}

		public int													pPreferredHeight;
		public int													pPreferredWidth;

		public event EventHandler						Changed;

		public PicSwitchImage()
		{
			mIC = new ImageCollection();
			pPreferredHeight	= 0;
			pPreferredWidth		= 0;
			//DoubleClick += new EventHandler (DoDoubleClick);
			MouseUp += new MouseEventHandler(DoMouseUp);
			//KeyDown += new KeyEventHandler (DoKeyDown);
		}

		protected override void OnPaint (PaintEventArgs pea)
		{
			base.OnPaint(pea);
			if (mIC != null && mIC.pCurrentItem != null && mIC.pCurrentItem.pImage != null)
			{
				ImageTools.ScaleImageIsotropically(pea.Graphics, mIC.pCurrentItem.pImage, 
					new Rectangle(2, 2, this.ClientSize.Width-4, this.ClientSize.Height-4));
			}
		}

		private void DoMouseUp (object sender, MouseEventArgs e)
		{
			if (mIC == null)
				return;

			if (e.Button == MouseButtons.Left)
				mIC.RotateForward();
			else
				mIC.RotateBackward();
			Invalidate();

			if (Changed != null)
				Changed(this, new EventArgs());
		}
		
		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (mIC == null)
				return;

			if (e.KeyCode == Keys.Space)
			{
				//if (e.Shift)
				//	mIC.RotateBackward();
				//else
					mIC.RotateForward();
			}
			Invalidate();

			if (Changed != null)
				Changed(this, new EventArgs());
		}
	}

	public class DataGridPicSwitchColumn : System.Windows.Forms.DataGridColumnStyle
	{
		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		private bool isEditing;

		protected PicSwitchImage						mPSI;

		public PicSwitchImage								pPicSwitchImage
		{
			get { return mPSI; }
		}

		public int													pHeight;
		public int													pWidth;

		public DataGridPicSwitchColumn()
		{
			mPSI = new PicSwitchImage();
		}

		protected override void Abort(int rowNum)
		{
			isEditing = false;
			RemoveEventHandlerPicSwitchChanged();
			Invalidate();
		}

		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
			mPSI.Bounds = Rectangle.Empty;
			RemoveEventHandlerPicSwitchChanged();

			if (!isEditing)
				return true;

			isEditing = false;

			try 
			{
				SetColumnValueAtRow(dataSource, rowNum, mPSI.pImageCollection.pCurrentIndex);
			} 
			catch //(Exception ex) 
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
			if (value != null && value != System.DBNull.Value)
					mPSI.pImageCollection.pCurrentIndex = (int)value;
			else 
					mPSI.pImageCollection.pCurrentIndex = 0;

			if (cellIsVisible) 
			{
				mPSI.Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				mPSI.Visible = true;
				isEditing = true;
				base.ColumnStartedEditing(mPSI);
				AddEventHandlerPicSwitchChanged();
			} 
			else 
			{
				mPSI.Visible = false;
			}

			if (mPSI.Visible)
			{
				DataGridTableStyle.DataGrid.Invalidate(bounds);
				mPSI.Invalidate();
				mPSI.Focus();
			}
		}

		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			return new Size (mPSI.pPreferredWidth, mPSI.pPreferredHeight);
		}

		protected override int GetMinimumHeight() 
		{
			return mPSI.pPreferredHeight;
		}

		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			return mPSI.pPreferredHeight;
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
			object value = this.GetColumnValueAtRow(source, rowNum);
			int ind = 0;
			try
			{
				ind = (int)value;
			}
			catch {}
			Rectangle rect = bounds;
			g.FillRectangle(backBrush,rect);
			//rect.Offset(2, 2);
			//rect.Height -= 2;
			//rect.Width -= 2;
			try
			{
				ImageTools.ScaleImageIsotropically(g, ((IImageItem)mPSI.pImageCollection[ind]).pImage, rect);
			}
			catch {}
		}

		protected override void SetDataGridInColumn(DataGrid value) 
		{
			base.SetDataGridInColumn(value);
			mPSI.Visible = false;
			if (mPSI.Parent != null) 
			{
				mPSI.Parent.Controls.Remove (mPSI);
			}
			if (value != null) 
			{
				value.Controls.Add(mPSI);
			}
		}

		private void AddEventHandlerPicSwitchChanged ()
		{
			mPSI.Changed += new EventHandler(DoPicChanged);
		}

		private void RemoveEventHandlerPicSwitchChanged ()
		{
			mPSI.Changed -= new EventHandler(DoPicChanged);
		}

		private void DoPicChanged(object sender, EventArgs e) 
		{
			if (!isEditing)
			{
				isEditing = true;
				base.ColumnStartedEditing(mPSI);
			}
		}

		/*
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
		*/
		/*protected override void SetColumnValueAtRow (CurrencyManager source, int rowNum, object value)
		{
			int ii = -1;
			try { ii = (int)value; } 
			catch {}
			if (ii != -1)
				base.SetColumnValueAtRow(source, rowNum, ii);
		}*/
		
	}
}
