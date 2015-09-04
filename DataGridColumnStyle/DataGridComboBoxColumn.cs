#define TRACE

using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CommandAS.Tools.DataGridColumnStyle
{
	#region IControlChange

	/// <summary>
	/// Событие об изменении содержания контрола!
	/// </summary>
	[Guid("E8B5EC54-B65A-4d73-82EF-09EBA4FF89BC")]
	public interface IControlChange
	{
		event EventHandler OnControlChanged;
	}

	#endregion --IControlChange

	public class DataGridComboBox: System.Windows.Forms.ComboBox
	{

		public int FindByLettersBegin = -1;
		public bool MayAddNewItem = false;

		public DataGridComboBox()
		{
			// initializing constructor

			//this.KeyPress += new KeyPressEventHandler(DoKeyPress);
			this.Sorted = true;
		}

		//private void DoKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			if (FindByLettersBegin == -1)
				return;

			if (((int)e.KeyChar) > 0x1F && this.Text.Length > FindByLettersBegin)
			{
				string newTxt;
				if (this.SelectionStart > 0 && this.SelectionStart <= this.Text.Length)
					newTxt = this.Text.Substring(0,SelectionStart) + e.KeyChar.ToString();
				else
					newTxt = this.Text;
				int ii = this.FindString(newTxt);
				if (ii >=0 )
				{
					this.SelectedIndex = ii;
					this.Select (newTxt.Length, this.Text.Length - newTxt.Length);
					e.Handled = true;
				}
			}

			//if (!e.Handled)
			base.OnKeyPress(e);
		}
	}

	public class DataGridComboBoxColumn : System.Windows.Forms.DataGridTextBoxColumn, IControlChange //System.Windows.Forms.DataGridColumnStyle
	{
		#region vars

		public DataGridComboBox					pCbo;
		protected DataGrid							dgr;
		protected bool									mLock=false;
		protected CurrencyManager				cm;
		private bool isEditing;
		public event EventHandler OnControlChanged;
		public event EventHandler OnControlAfterChanged;

		#endregion --vars

		#region constructors

		public DataGridComboBoxColumn() : base() 
		{
			pCbo = new DataGridComboBox();
			//pCbo.LostFocus+=new EventHandler(pCbo_LostFocus);
			pCbo.Visible = false;
		}

		#endregion --constructors


		protected override void Abort(int rowNum)
		{
			mLock=true;
			isEditing = false;
			RemoveEventHandlerComboBoxChanged();
			cm.CancelCurrentEdit();
			mLock=false;
			Invalidate();
		}

		protected override bool Commit(CurrencyManager source, int rowNum) 
		{
#if DEBUG && TRACE
			Helper.Trace( "Commit(...)", rowNum, "isEditing="+isEditing, "pCbo.Visible=" + pCbo.Visible);
#endif
			RemoveEventHandlerComboBoxChanged();

			cm=source;
			if (!isEditing)
			{
				//getRowView.EndEdit();
				//return true;
			}
			isEditing = false;
			pCbo.Visible=false;
			mLock=true;
			try 
			{
				string str=pCbo.Text.Trim();
				SetColumnValueAtRow(source, rowNum, str);
				//если нет в коллекции - добавляем
				if (pCbo.MayAddNewItem && pCbo.FindStringExact(str)==-1 && str.Length > 0)
					pCbo.Items.Add(str);
				if (OnControlAfterChanged != null)
					OnControlAfterChanged(this, new EventArgs());
			}
			catch //(Exception ex) 
			{
				Abort(rowNum);
				return false;
			}
			Invalidate();
			mLock=false;
			return true;
		}

		protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly,	string instantText, bool cellIsVisible)
		{
			if (isEditing)
			{
				pCbo.BringToFront();
				pCbo.Focus();
				return;
			}

#if DEBUG && TRACE
			Helper.Trace( "Edit(...)", rowNum, "isEditing="+isEditing, "cellIsVisible="+cellIsVisible);
#endif

			if (rowNum==-1)
			{
				Error.ShowError("DataGridComboBox : Edit : rowNum==-1 ???");
			}
			cm=source;

			mLock=true;
			string value = GetColumnValueAtRow(source, rowNum) as string;
			if (value != null) 
				pCbo.Text = value;
			else 
				pCbo.Text = this.NullText;

			if (cellIsVisible) 
			{
				pCbo.Bounds = new Rectangle(bounds.X, bounds.Y , bounds.Width, bounds.Height - 2);
				isEditing = true;
				AddEventHandlerComboBoxChanged();
				this.ColumnStartedEditing(pCbo);
			} 
				/*
			else 
			{
				pCbo.Visible = false;
			}*/
			if (pCbo.Visible)
				DataGridTableStyle.DataGrid.Invalidate(bounds);
			mLock=false;
		}

		#region необходимые размеры

		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			return new Size(100, pCbo.PreferredHeight + 1);
		}

		protected override int GetMinimumHeight() 
		{
			return pCbo.PreferredHeight + 1;
		}

		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			return pCbo.PreferredHeight + 1;
		}


		#endregion --необходимые размеры

		#region Paints

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
			string value = this.GetColumnValueAtRow( source, rowNum ) as string;
			if ( value == null) 
				value = this.NullText;

			Rectangle rect = bounds;
			g.FillRectangle(backBrush,rect);
			rect.Offset(0, 0);
			rect.Height -= 1;
			g.DrawString(value, this.DataGridTableStyle.DataGrid.Font, foreBrush, rect);
		}

		#endregion --Paints

		/// Не надо ничего оптимизировать!!!
		/// Это не работает !!!
		//protected override void SetColumnValueAtRow (CurrencyManager source, int rowNum, object value)
		//{
		//	string oldValue =GetColumnValueAtRow(source,rowNum) as string;
		//	string newValue=value as string;
		//	if (!oldValue.Equals(newValue))
		//	{
		//		base.SetColumnValueAtRow(source, rowNum, newValue);
		//		getRowView.EndEdit();
		//	}
		//}

		protected override void ColumnStartedEditing(Control editingControl)
		{
			base.ColumnStartedEditing (editingControl);
			try 
			{
				editingControl.Visible=true;
				editingControl.BringToFront();
				editingControl.Focus();
			}
			catch {}
		}

		protected override void SetDataGridInColumn(DataGrid value) 
		{
#if DEBUG && TRACE
			Helper.Trace( "SetDataGridInColumn ...");
#endif
			base.SetDataGridInColumn(value);
			pCbo.Visible = false;
			if (pCbo.Parent != null) 
			{
				pCbo.Parent.Controls.Remove (pCbo);
			}
			if (value != null) 
			{
				value.Controls.Add(pCbo);
			}
			//поддерживаем горящие клавиши - пока только F2
			this.pCbo.KeyDown-=new KeyEventHandler(Combo_KeyDown);
			this.pCbo.KeyDown+=new KeyEventHandler(Combo_KeyDown);
			this.dgr=value;
			//this.dgr.KeyDown+=new KeyEventHandler(dgr_KeyDown);
			//this.dgr.LostFocus+=new EventHandler(pCbo_LostFocus);
		}

		#region local events

		private void Combo_KeyDown(object sender, KeyEventArgs e)
		{
			//поддерживаем горящие клавиши - пока только F2
			switch(e.KeyCode)
			{
				case Keys.F2:
					pCbo.DroppedDown=true;
					break;
				case Keys.Escape:
					pCbo.DroppedDown=false;
					break;
			}
		}

		//отрабатываем выбор из списка
		private void DoComboBoxChanged(object sender, EventArgs e) 
		{
			RaiseChangeControl();
		}

		private void pCbo_LostFocus(object sender, EventArgs e)
		{/*
			if (cm!=null)
			 getRowView.EndEdit();*/
		}

		#endregion --local events

		#region local functions

		protected void RaiseChangeControl()
		{
			//try 
			//{
			//	string str=pCbo.Text.Trim();
			//	SetColumnValueAtRow(source, rowNum, str);
			//	//если нет в коллекции - добавляем
			//	if (pCbo.MayAddNewItem && pCbo.FindStringExact(str)==-1 && str.Length > 0)
			//		pCbo.Items.Add(str);
			//}
			//catch //(Exception ex) 
			//{
			//	Abort(rowNum);
			//	return false;
			//}

			if (OnControlChanged!=null && 
						!this.ReadOnly && 
						!this.DataGridTableStyle.DataGrid.ReadOnly && 
						!mLock)
				OnControlChanged(this,EventArgs.Empty);
		}
		protected DataRowView getRowView
		{
			get
			{
				DataRowView ret=null;
				if (cm!=null)
					ret=cm.Current as DataRowView;
				return ret;
			}
		}

		private void AddEventHandlerComboBoxChanged ()
		{
			if (pCbo.DropDownStyle == ComboBoxStyle.DropDownList)
				pCbo.SelectedValueChanged += new EventHandler(DoComboBoxChanged);
			else
				pCbo.TextChanged += new EventHandler(DoComboBoxChanged);
		}

		private void RemoveEventHandlerComboBoxChanged ()
		{
			if (pCbo.DropDownStyle == ComboBoxStyle.DropDownList)
				pCbo.SelectedValueChanged -= new EventHandler(DoComboBoxChanged);
			else
				pCbo.TextChanged -= new EventHandler(DoComboBoxChanged);
		}

		#endregion --local functions

	}

	public class DataGridComboBoxColumnByCode : DataGridComboBoxColumn
	{
		protected override object GetColumnValueAtRow(CurrencyManager source, int rowNum)
		{
			object obj = base.GetColumnValueAtRow (source, rowNum);

			int code = 0;
			try {code = (int)obj;} 
			catch {}
			//if (code > 0)
			//{
				foreach (_ListBoxItem item in pCbo.Items)
				{
					if (item.code == code)
						return item.text;
				}
			//}

			return string.Empty;
		}

		protected override void SetColumnValueAtRow (CurrencyManager source, int rowNum, object value)
		{
			object s = value;

			if (s.ToString().Trim().Length > 0)
			{
				int ii = pCbo.FindStringExact(s.ToString());
 
				if (ii == -1)
					s = null; // DBNull.Value;
				else
					s = ((_ListBoxItem)pCbo.Items[ii]).code;
				
				base.SetColumnValueAtRow(source, rowNum, s);
			}
			else
				base.SetColumnValueAtRow(source, rowNum, 0);

		}

	}

	public class DataGridComboBoxColumnByPC : DataGridComboBoxColumn
	{

		protected override object GetColumnValueAtRow(CurrencyManager source, int rowNum)
		{
			object obj = base.GetColumnValueAtRow (source, rowNum);

			string pc = string.Empty;
			try {pc = obj.ToString();} 
			catch {}
			if (pc.Length > 0)
			{
				foreach (_ListBoxPCItem item in pCbo.Items)
				{
					if (item.pPC == PlaceCode.PDC2PlaceCode(pc))
						return item.pText;
				}
			}

			return string.Empty;
		}

		protected override void SetColumnValueAtRow (CurrencyManager source, int rowNum, object value)
		{
			object s = value;
			if (s.ToString().Trim().Length > 0)
			{
				int ii = pCbo.FindStringExact(s.ToString());
 
				if (ii == -1)
					s = null; //DBNull.Value;
				else
					s = ((_ListBoxPCItem)pCbo.Items[ii]).pPC.PlaceDelimCode;
					
				base.SetColumnValueAtRow(source, rowNum, s);
			}
			else
				base.SetColumnValueAtRow(source, rowNum, 0);
		}

	}
}
