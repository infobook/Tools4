using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using CommandAS.Tools;
using CommandAS.Tools.Controls;
using System.Reflection;

namespace CommandAS.Tools.DataGridColumnStyle
{
	public delegate void EvH_DataGridColumnStyle(object sender, EvA_DataGridColumnStyle e);

	public class EvA_DataGridColumnStyle: EventArgs
	{
		private int			_row;

		public int			pRow
		{
			get { return _row; }
		}

		public EvA_DataGridColumnStyle(int aRow)
		{
			_row = aRow;
		}
	}

	public class DataGridCASSparrow : CASSparrow
	{
		private DataGridTextBox				_dgrtxt;

		public DataGridTextBox				pDtgTxt
		{
			get { return _dgrtxt; }
		}

#if DEBUG
		public bool								pIsTraceMessageOn = false;
#endif

		public bool pIsInEditOrNavigateMode 
		{
			get { return _dgrtxt.IsInEditOrNavigateMode;  }
			set {	_dgrtxt.IsInEditOrNavigateMode = value;	}
		}

		public DataGridCASSparrow() : base(true)
		{
			_dgrtxt = (DataGridTextBox)txt;
			//_dgrtxt.GotFocus += new EventHandler (DoTxtGotFocus);
			//txt.Validated += new EventHandler(OnTextValidated);
		}

//		private void OnTextValidated(object sender, System.EventArgs e)
//		{
//			try
//			{
//				if (OnSelectedTreeItem != null)
//					OnSelectedTreeItem (this, new EvA_SelectedTreeItem(mPC,txt.Text)); 
//			}
//			catch {}
//		}

//		private void DoTxtGotFocus(object sender, EventArgs e)
//		{
//#if LOC_TRACE1
//			Helper.Trace( "DoTxtGotFocus()");
//#endif
//		}

		public void  SetDataGrid(System.Windows.Forms.DataGrid aParentGrid)
		{
			_dgrtxt.SetDataGrid(aParentGrid);
		}
	}

	public class DataGridCASSparrowColumnBase : System.Windows.Forms.DataGridColumnStyle
	{
		private CurrencyManager					_curSrcMan;
		private int											_curRowNum;

		public DataGridCASSparrow				pSparrow;
		/// <summary>
		/// ‘ормат записи данных в €чейки:
		/// pIsPDCFormat = true		- формат <place><delimiter><code>
		/// pIsPDCFormat = false	- формат <code> (default)
		/// </summary>
		public bool											pIsPDCFormat;

		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		protected bool isEditing;
		protected bool isValueChange;


		public event EvH_DataGridColumnStyle	OnEdit;
		public event EvH_DataGridColumnStyle	OnCommit;
		public event EvH_DataGridColumnStyle	OnAbort;

		public DataGridCASSparrowColumnBase() : base() 
		{
			pSparrow = new DataGridCASSparrow();
			pSparrow.Visible = false;
			pIsPDCFormat = false;
			_curSrcMan = null;
			_curRowNum = -1;
			isEditing	= false;
			isValueChange = false;
			//this.DataGridTableStyle.DataGrid.KeyPress += new KeyPressEventHandler(DoDGRKeyPerss);
		}

		protected override void Abort(int rowNum)
		{
#if DEBUG && TRACE
			Helper.Trace( this.GetType().Name+".Abort", "rowNum="+rowNum, "isEditing="+isEditing, "isValueChange="+isValueChange);
#endif
			isEditing = false;
			isValueChange = false;
			RemoveEventHandlerSparrowChange();
			//ConcedeFocus();
			Invalidate();
			_curSrcMan = null;
			_curRowNum = -1;
			if (OnAbort != null)
				OnAbort(this, new EvA_DataGridColumnStyle(rowNum));
		}

		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
#if DEBUG && TRACE
			Helper.Trace( this.GetType().Name+".Commit", "rowNum="+rowNum, "isEditing="+isEditing, "isValueChange="+isValueChange);
#endif
			//pSparrow.Bounds = Rectangle.Empty;
			pSparrow.Hide();
			RemoveEventHandlerSparrowChange();

			if (!isEditing)
				return true;
			isEditing = false;

			if (!isValueChange)
				return true;
			isValueChange = false;


			try 
			{
				string ss = pSparrow.pItemText;
				SetColumnValueAtRow(dataSource, rowNum, ss);
			} 
			catch //(Exception ex) 
			{
				Abort(rowNum);
				return false;
			}

			//ConcedeFocus();
			Invalidate();
			_curSrcMan = null;
			_curRowNum = -1;

			if (OnCommit != null)
				OnCommit(this, new EvA_DataGridColumnStyle(rowNum));

			return true;
		}

		//protected override void ConcedeFocus() 
		//{
		//	// Hide the control when conceding focus.
		//	if(!isEditing)
		//		pSparrow.Visible = false;
		//}

		protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly,	string instantText, bool cellIsVisible)
		{
			if (readOnly || this.DataGridTableStyle.DataGrid.ReadOnly)
			{
				this.DataGridTableStyle.DataGrid.Select(rowNum);
				return;
			}
#if DEBUG && TRACE
			Helper.Trace( this.GetType().Name+".Edit", "rowNum="+rowNum, "isEditing="+isEditing, "isValueChange="+isValueChange);
#endif
			//if (pSparrow.Visible)
			//	return;
			_curSrcMan = source;
			_curRowNum = rowNum;

			string value = GetColumnValueAtRow(source, rowNum).ToString();
			pSparrow.pItemText = value;
			if (cellIsVisible) 
			{
				pSparrow.Bounds = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 4);
				pSparrow.Visible = true;
				pSparrow.pDtgTxt.Focus();
				if (!isEditing)
					AddEventHandlerSparrowChange();
			}
			else
			{
				pSparrow.Visible = false;
			}

			if (pSparrow.Visible)
			{
				DataGridTableStyle.DataGrid.Invalidate(bounds);
				isEditing = true;
				isValueChange = false;
				if (OnEdit != null)
					OnEdit(this, new EvA_DataGridColumnStyle(rowNum));
			}
		}

		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			return new Size(100, pSparrow.Height + 4);
		}

		protected override int GetMinimumHeight() 
		{
			return pSparrow.Height + 4;
		}

		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			return pSparrow.Height + 4;
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

			bool isSelect=(backBrush==SystemBrushes.ActiveCaption);
			Rectangle rect = bounds;
			bool isReadOnly=ReadOnly || this.DataGridTableStyle.DataGrid.ReadOnly;
			if (isReadOnly)
			{
				//режим выделени€
				if (isSelect)
				{
					backBrush=SystemBrushes.ActiveCaption; //new HatchBrush(HatchStyle.Percent10, SystemColors.ActiveCaption);
				}
				else
				{
					backBrush=SystemBrushes.Control; //new HatchBrush(HatchStyle.Percent10, SystemColors.ActiveCaption,SystemColors.ActiveCaptionText);
				}
			}
			g.FillRectangle(backBrush,rect);
			if (isReadOnly)
				g.DrawRectangle(SystemPens.ControlDark,rect);
			rect.Offset(2, 1);
			rect.Height += 2;
			g.DrawString(text, this.DataGridTableStyle.DataGrid.Font, foreBrush, rect,StringFormat.GenericTypographic);
		}

		protected override void SetDataGridInColumn(DataGrid value) 
		{
			base.SetDataGridInColumn(value);

			if (pSparrow.Parent != null) 
			{
				pSparrow.Parent.Controls.Remove (pSparrow);
			}
			if (value != null) 
			{
				value.Controls.Add(pSparrow);
			}
			pSparrow.SetDataGrid(value);
		}
		protected void AddEventHandlerSparrowChange ()
		{
			pSparrow.pDtgTxt.TextChanged += new EventHandler(DoSparrowChange);
			pSparrow.OnSelectedTreeItem += new CASSparrow.SelectedTreeItemEventHandler(DoSparrowChange2);
		}
		protected void RemoveEventHandlerSparrowChange ()
		{
			pSparrow.pDtgTxt.TextChanged -= new EventHandler(DoSparrowChange);
			pSparrow.OnSelectedTreeItem -= new CASSparrow.SelectedTreeItemEventHandler(DoSparrowChange2);
		}
		private void DoSparrowChange(object sender, EventArgs e) 
		{
			isValueChange = true;
#if DEBUG && TRACE
			Helper.Trace( "OnTextChanged", "IsInEditOrNavigateMode="+pSparrow.pIsInEditOrNavigateMode);
#endif
		}
		private void DoSparrowChange2(object sender, EvA_SelectedTreeItem e) 
		{
			isValueChange = true;
			if (_curSrcMan != null && _curRowNum >= 0)
				SetColumnValueAtRow(_curSrcMan, _curRowNum, e.pText);
			base.ColumnStartedEditing(pSparrow);
		}
	}

	public class DataGridCASSparrowColumn : DataGridCASSparrowColumnBase
	{
		protected override object GetColumnValueAtRow(CurrencyManager source, int rowNum)
		{
			object obj = base.GetColumnValueAtRow (source, rowNum);

			string id = string.Empty;
			try {id = obj.ToString();} 
			catch {}
			if (id.Length > 0)
			{
				TreeNode tn = null;
				if (pIsPDCFormat)
					tn = pSparrow.pTreeView.SearchByCode(PlaceCode.PDC2PlaceCode(id));
				else
					tn = pSparrow.pTreeView.SearchByCode(CommandAS.Tools.CASTools.ConvertToInt32Or0(id));
				if (tn != null)
					return tn.Text;
			}

			return string.Empty;
		}

		protected override void SetColumnValueAtRow (CurrencyManager source, int rowNum, object value)
		{
			if (this.DataGridTableStyle.DataGrid.ReadOnly)
				return;
			object s = value;

			string ss = s.ToString().Trim();
			if (ss.Length > 0)
			{
				TreeNode tn = pSparrow.pTreeView.SearchByText(s.ToString(), true, true);
 
				if (tn != null)
				{
					if (pIsPDCFormat)
						s = ((CASTreeItemData)tn.Tag).pPC.PlaceDelimCode;
					else
						s = ((CASTreeItemData)tn.Tag).pCode;
					//base.SetColumnValueAtRow(source, rowNum, s);
				}
			}
			else
			{
				if (pIsPDCFormat)
					s = string.Empty;
				else
					s = 0;
			}
			base.SetColumnValueAtRow(source, rowNum, s);
		}
	}
}
