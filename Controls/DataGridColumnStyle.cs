using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace CommandAS.Tools.Controls
{

  public abstract class DataGridComboColumn: System.Windows.Forms.DataGridColumnStyle
  {
		#region PROPERTY:
    // UI constants    
    protected int														xMargin = 0; //2;
    protected int														yMargin = -1; //1;
    protected System.Windows.Forms.Control  mControl;
		
    // Used to track editing state
    protected string OldVal=string.Empty; //new string(string.Empty.ToCharArray());
    protected bool InEdit= false;
		#endregion

    public DataGridComboColumn(System.Windows.Forms.Control  aControl)
    {
      mControl = aControl;
    }


		#region  Methods overridden from DataGridColumnStyle

    // Abort Changes
    //	protected override void Abort(int RowNum){}

    // Commit Changes
    //	protected override bool Commit(CurrencyManager DataSource,int RowNum)

    //
    // Remove focus
    //
    protected override void ConcedeFocus()
    {
      //HideCombo();
      mControl.Visible=false;
    }

    // Edit Grid
    //protected override void Edit(CurrencyManager Source, int Rownum, Rectangle Bounds, bool ReadOnly, string InstantText, bool CellIsVisible)
		
    // Set the minimum height 
    //protected override int GetMinimumHeight()

    protected override int GetPreferredHeight(Graphics g ,object Value)
    {
      System.Diagnostics.Debug.WriteLine("GetPreferredHeight()");
      int NewLineIndex  = 0;
      int NewLines = 0;
      string ValueString = this.GetText(Value);
      do
      {
        NewLineIndex = ValueString.IndexOf("r\n", NewLineIndex + 1);
        NewLines += 1;
      }while(NewLineIndex != -1);
      return FontHeight * NewLines + yMargin;
    }
    protected override Size GetPreferredSize(Graphics g, object Value)
    {
      Size Extents = Size.Ceiling(g.MeasureString(GetText(Value), this.DataGridTableStyle.DataGrid.Font));
      Extents.Width += xMargin * 2 + DataGridTableGridLineWidth ;
      Extents.Height += yMargin;
      return Extents;
    }
    protected override void Paint(Graphics g,Rectangle Bounds,CurrencyManager Source,int RowNum)
    {
      Paint(g, Bounds, Source, RowNum, false);
    }
    protected override void Paint(Graphics g,Rectangle Bounds,CurrencyManager Source,int RowNum,bool AlignToRight)
    {
      string Text = GetText(GetColumnValueAtRow(Source, RowNum));
      PaintText(g, Bounds, Text, AlignToRight);
    }
    protected override void Paint(Graphics g,Rectangle Bounds,CurrencyManager Source,int RowNum,Brush BackBrush ,Brush ForeBrush ,bool AlignToRight)
    {
      string Text = GetText(GetColumnValueAtRow(Source, RowNum));
      PaintText(g, Bounds, Text, BackBrush, ForeBrush, AlignToRight);
    }
    protected override void SetDataGridInColumn(DataGrid Value)
    {
      base.SetDataGridInColumn(Value);
      if(mControl.Parent!=Value)
      {
        if(mControl.Parent!=null)
        {
          mControl.Parent.Controls.Remove(mControl);
        }
      }
      if(Value!=null) 
      {
        Value.Controls.Add(mControl);
      }
    }
    //protected override void UpdateUI(CurrencyManager Source,int RowNum, string InstantText)
		#endregion

		#region Helper Methods 
    protected int DataGridTableGridLineWidth
    {
      get
      {
        if(this.DataGridTableStyle.GridLineStyle == DataGridLineStyle.Solid) 
        { 
          return 1;
        }
        else
        {
          return 0;
        }
      }
    }
    public void EndEdit()
    {
      InEdit = false;
      Invalidate();
    }
    protected string GetText(object Value)
    {
      if(Value==System.DBNull.Value)
      {
        return NullText;
      }
      if(Value!=null)
      {
        return Value.ToString();
      }
      else
      {
        return string.Empty;
      }
    }
    protected void HideCombo()
    {
      if(mControl.Focused)
      {
        this.DataGridTableStyle.DataGrid.Focus();
      }
      mControl.Visible = false;
    }
    protected void PaintText(Graphics g ,Rectangle Bounds,string Text,bool AlignToRight)
    {
      Brush BackBrush = new SolidBrush(this.DataGridTableStyle.BackColor);
      Brush ForeBrush= new SolidBrush(this.DataGridTableStyle.ForeColor);
      PaintText(g, Bounds, Text, BackBrush, ForeBrush, AlignToRight);
    }
    protected void PaintText(Graphics g , Rectangle TextBounds, string Text, Brush BackBrush,Brush ForeBrush,bool AlignToRight)
    {	
      Rectangle Rect = TextBounds;
      RectangleF RectF  = Rect; 
      StringFormat Format = new StringFormat();
      if(AlignToRight)
      {
        Format.FormatFlags = StringFormatFlags.DirectionRightToLeft;
      }
      switch(this.Alignment)
      {
        case HorizontalAlignment.Left:
          Format.Alignment = StringAlignment.Near;
          break;
        case HorizontalAlignment.Right:
          Format.Alignment = StringAlignment.Far;
          break;
        case HorizontalAlignment.Center:
          Format.Alignment = StringAlignment.Center;
          break;
      }
      Format.FormatFlags =Format.FormatFlags;
      Format.FormatFlags =StringFormatFlags.NoWrap;
      g.FillRectangle(BackBrush, Rect);
      Rect.Offset(0, yMargin);
      Rect.Height -= yMargin;
      g.DrawString(Text, this.DataGridTableStyle.DataGrid.Font, ForeBrush, RectF, Format);
      Format.Dispose();
    }
		#endregion
  }


  public class DataGridComboBoxColumn2: DataGridComboColumn
  {
    //
    // Creates a combo box column on a data grid
    // all cells in the column have the same data source
    private System.Windows.Forms.ComboBox		mCombo;

    public DataGridComboBoxColumn2(System.Windows.Forms.ComboBox aComboBox):
      base (aComboBox)
    {
      mCombo = aComboBox;
      mCombo.Visible=false;
      mCombo.DropDownStyle = ComboBoxStyle.DropDownList;
    }

		#region Methods overridden from DataGridColumnStyle
    //
    // Abort Changes
    //
    protected override void Abort(int RowNum)
    {
      System.Diagnostics.Debug.WriteLine("Abort()");
      RollBack();
      HideCombo();
      EndEdit();
    }
    //
    // Commit Changes
    //
    protected override bool Commit(CurrencyManager DataSource,int RowNum)
    {
      HideCombo();
      if(!InEdit)
      {
        return true;
      }

      try
      {
        object Value = mCombo.SelectedValue;
        if(NullText.Equals(Value))
        {
          Value = System.Convert.DBNull; 
        }
        SetColumnValueAtRow(DataSource, RowNum, Value);
      }
      catch
      {
        RollBack();
        return false;	
      }
			
      this.EndEdit();
      return true;
    }

    //
    // Edit Grid
    //
    protected override void Edit(
      CurrencyManager Source ,
      int Rownum,
      Rectangle Bounds, 
      bool ReadOnly,
      string InstantText, 
      bool CellIsVisible
      )
    {

      mCombo.Text = string.Empty;
      Rectangle OriginalBounds = Bounds;
      OldVal = mCombo.Text;
	
      if(CellIsVisible)
      {
        Bounds.Offset(xMargin, yMargin);
        Bounds.Width -= xMargin * 2;
        Bounds.Height -= yMargin;
        mCombo.Bounds = Bounds;
        mCombo.Visible = true;
      }
      else
      {
        mCombo.Bounds = OriginalBounds;
        mCombo.Visible = false;
      }
      try  
      {
        mCombo.SelectedValue = GetText(GetColumnValueAtRow(Source, Rownum));
      }
      catch {}
			
      if(InstantText!=null)
      {
        mCombo.SelectedValue = InstantText;
      }
      mCombo.RightToLeft = this.DataGridTableStyle.DataGrid.RightToLeft;
      //			Combo.Focus();
			
      if(InstantText==null)
      {
        mCombo.SelectAll();
				
        // Pre-selects an item in the combo when user tries to add
        // a new record by moving the columns using tab.

        // Combo.SelectedIndex = 0;
      }
      else
      {
        int End = mCombo.Text.Length;
        mCombo.Select(End, 0);
      }
      if(mCombo.Visible)
      {
        DataGridTableStyle.DataGrid.Invalidate(OriginalBounds);
      }

      InEdit = true;
    }
    protected override int GetMinimumHeight()
    {
      //
      // Set the minimum height to the height of the combobox
      //
      return mCombo.PreferredHeight + yMargin;
    }

    protected override void UpdateUI(CurrencyManager Source,int RowNum, string InstantText)
    {
      mCombo.Text = GetText(GetColumnValueAtRow(Source, RowNum));
      if(InstantText!=null)
      {
        mCombo.Text = InstantText;
      }
    }	
		#endregion												 

		#region Helper Methods 
    private void RollBack()
    {
      mCombo.Text = OldVal;
      //EndEdit();
    }
		#endregion

  }


  /// <summary>
  /// Summary description for DataGridColumnStyle.
  /// </summary>
  //public class DataGridComboTreeColumn: DataGridComboColumn
//  public class DataGridComboTreeColumn2: DataGridComboColumn
//  {
//    private SelectFromTree mSelTree;
//
//    public DataGridComboTreeColumn2(SelectFromTree aSelTree):
//      base (aSelTree)
//    {
//      mSelTree = aSelTree;
//      mSelTree.Visible = false;
//    }
//
//		#region Methods overridden from DataGridColumnStyle
//    //
//    // Abort Changes
//    //
//    protected override void Abort(int RowNum)
//    {
//      System.Diagnostics.Debug.WriteLine("Abort()");
//      RollBack();
//      HideCombo();
//      EndEdit();
//    }
//    //
//    // Commit Changes
//    //
//    protected override bool Commit(CurrencyManager DataSource,int RowNum)
//    {
//      HideCombo();
//      if(!InEdit)
//      {
//        return true;
//      }
//
//      try
//      {
//        object Value = mSelTree.pItemText;
//        if(NullText.Equals(Value))
//        {
//          Value = System.Convert.DBNull; 
//        }
//        SetColumnValueAtRow(DataSource, RowNum, Value);
//      }
//      catch
//      {
//        RollBack();
//        return false;	
//      }
//			
//      this.EndEdit();
//      return true;
//    }
//
//    //
//    // Edit Grid
//    //
//    protected override void Edit(
//      CurrencyManager Source ,
//      int Rownum,
//      Rectangle Bounds, 
//      bool ReadOnly,
//      string InstantText, 
//      bool CellIsVisible
//      )
//    {
//
//      mSelTree.pItemText = string.Empty;
//      Rectangle OriginalBounds = Bounds;
//      OldVal = mSelTree.pItemText;
//
//      if(CellIsVisible)
//      {
//        Bounds.Offset(xMargin, yMargin);
//        Bounds.Width -= xMargin * 2;
//        Bounds.Height -= yMargin;
//        mSelTree.Bounds = Bounds;
//        mSelTree.Visible = true;
//        mSelTree.Focus(); 
//      }
//      else
//      {
//        mSelTree.Bounds = OriginalBounds;
//        mSelTree.Visible = false;
//      }
//
//      //			try  
//      //			{
//      //				mCombo.SelectedValue = GetText(GetColumnValueAtRow(Source, Rownum));
//      //			}
//      //			catch {}
//			
//      //			if(InstantText!=null)
//      //			{
//      //				mCombo.SelectedValue = InstantText;
//      //			}
//      mSelTree.RightToLeft = this.DataGridTableStyle.DataGrid.RightToLeft;
//			
//      //			if(InstantText==null)
//      //			{
//      //				mCombo.SelectAll();
//      //				
//      //				// Pre-selects an item in the combo when user tries to add
//      //				// a new record by moving the columns using tab.
//      //
//      //				// Combo.SelectedIndex = 0;
//      //			}
//      //			else
//      //			{
//      //				int End = mCombo.Text.Length;
//      //				mCombo.Select(End, 0);
//      //			}
//
//      if(mSelTree.Visible)
//      {
//        DataGridTableStyle.DataGrid.Invalidate(OriginalBounds);
//      }
//
//      InEdit = true;
//    }
//    protected override int GetMinimumHeight()
//    {
//      //
//      // Set the minimum height to the height of the combobox
//      //
//      return mSelTree.pTextHeight + yMargin;
//    }
//
//    protected override void UpdateUI(CurrencyManager Source,int RowNum, string InstantText)
//    {
//      mSelTree.pItemText = GetText(GetColumnValueAtRow(Source, RowNum));
//      if(InstantText!=null)
//      {
//        mSelTree.pItemText = InstantText;
//      }
//    }	
//		#endregion												 
//
//		#region Helper Methods 
//    private void RollBack()
//    {
//      mSelTree.pItemText = OldVal;
//    }
//		#endregion
//
//  }


  // Step 1. Derive a custom column style from DataGridTextBoxColumn
  //	a) add a ComboBox member
  //  b) track when the combobox has focus in Enter and Leave events
  //  c) override Edit to allow the ComboBox to replace the TextBox
  //  d) override Commit to save the changed data
  public class DataGridComboBoxColumnOld : DataGridTextBoxColumn
  {
    public System.Windows.Forms.ComboBox ColumnComboBox;
    private System.Windows.Forms.CurrencyManager _source;
    private int _rowNum;
    private bool _isEditing;
    public static int _RowCount;
    //private string mFNameValue;
    //private string mFNameDisplay;
		
    public event EventHandler OnBeforeDropDown;
    public event EventHandler OnAfterSelectedItem;

    public DataGridComboBoxColumnOld(System.Windows.Forms.ComboBox aComboBox)
      //string aFNameValue, 
      //string aFNameDisplay
      //)
      : base()
    {
      _source = null;
      _isEditing = false;
      _RowCount = -1;
      //mFNameValue = aFNameValue;
      //mFNameDisplay = aFNameDisplay;
	
      ColumnComboBox = aComboBox;
      ColumnComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		
      ColumnComboBox.Leave += new EventHandler(LeaveComboBox);
      //		ColumnComboBox.Enter += new EventHandler(ComboMadeCurrent);
      ColumnComboBox.SelectionChangeCommitted += new EventHandler(ComboStartEditing);
		
    }
    private void ComboStartEditing(object sender, EventArgs e)
    {
      _isEditing = true;
      base.ColumnStartedEditing((Control) sender);
      if (OnAfterSelectedItem != null)
        OnAfterSelectedItem(this, new EventArgs()); 
    }

    private void HandleScroll(object sender, EventArgs e)
    {
      if(ColumnComboBox.Visible)
        ColumnComboBox.Hide();

    }
    //		private void ComboMadeCurrent(object sender, EventArgs e)
    //		{
    //			_isEditing = true; 	
    //		}
    //		
    private void LeaveComboBox(object sender, EventArgs e)
    {
      if(_isEditing)
      {
        SetColumnValueAtRow(_source, _rowNum, ColumnComboBox.Text);
        _isEditing = false;
        Invalidate();
      }
      ColumnComboBox.Hide();
      this.DataGridTableStyle.DataGrid.Scroll -= new EventHandler(HandleScroll);
			
    }
    protected override  int GetPreferredHeight ( Graphics g,object value )
    {
      return ColumnComboBox.Height;
    }
    protected override  int GetMinimumHeight()
    {
      return ColumnComboBox.Height;
    }
    protected override  Size GetPreferredSize ( Graphics g, object value )
    {
      return ColumnComboBox.Size;
    }
    protected override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
    {
			

      base.Edit(source,rowNum, bounds, readOnly, instantText , cellIsVisible);

      _rowNum = rowNum;
      _source = source;
		
      if (OnBeforeDropDown != null)
        OnBeforeDropDown(this, new EventArgs()); 
      ColumnComboBox.Parent = this.TextBox.Parent;
      //ColumnComboBox.Location = this.TextBox.Location;
      //ColumnComboBox.Size = new Size(this.TextBox.Size.Width, ColumnComboBox.Size.Height);
      ColumnComboBox.Bounds = bounds; 
      ColumnComboBox.SelectedIndex = ColumnComboBox.FindStringExact(this.TextBox.Text);
      ColumnComboBox.Text =  this.TextBox.Text;
      this.TextBox.Visible = false;
      ColumnComboBox.Visible = true;
      this.DataGridTableStyle.DataGrid.Scroll += new EventHandler(HandleScroll);
			
      ColumnComboBox.BringToFront();
      ColumnComboBox.Focus();	
    }

    protected override bool Commit(System.Windows.Forms.CurrencyManager dataSource, int rowNum)
    {
		
      if(_isEditing)
      {
        _isEditing = false;
        SetColumnValueAtRow(dataSource, rowNum, ColumnComboBox.Text);
      }
      return true;
    }

    protected override void ConcedeFocus()
    {
      //Console.WriteLine("ConcedeFocus");
      base.ConcedeFocus();
    }

    /*protected override object GetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum)
    {

      object s =  base.GetColumnValueAtRow(source, rowNum);
      DataView dv = (DataView)this.ColumnComboBox.DataSource;
      int rowCount = dv.Count;
      int i = 0;

      //if things are slow, you could order your dataview
      //& use binary search instead of this linear one
      while (i < rowCount)
      {
        if( s.Equals( dv[i][this.ColumnComboBox.ValueMember]))
          break;
        ++i;
      }
			
      if(i < rowCount)
        return dv[i][this.ColumnComboBox.DisplayMember];
			
      return DBNull.Value;
    }

    protected override void SetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum, object value)
    {
      object s = value;

      DataView dv = (DataView)this.ColumnComboBox.DataSource;
      int rowCount = dv.Count;
      int i = 0;

      //if things are slow, you could order your dataview
      //& use binary search instead of this linear one
      while (i < rowCount)
      {
        if( s.Equals( dv[i][this.ColumnComboBox.DisplayMember]))
          break;
        ++i;
      }
      if(i < rowCount)
        s =  dv[i][this.ColumnComboBox.ValueMember];
      else
        s = DBNull.Value;
      base.SetColumnValueAtRow(source, rowNum, s);

    }*/
  }

//  public class DataGridComboTreeColumn: DataGridTextBoxColumn
//  {
//    private SelectFromTree												mSelTree;
//    private bool																	mIsEditing;
//    private System.Windows.Forms.CurrencyManager	mSource;
//    private int																		mRowNum;
//    //private string																mFNamePlace;
//    //private string																mFNameCode;
//    //private string																mFNameDisplay;
//
//    //public SelectFromTree													pSelTree
//    //{
//    //	get { return mSelTree; }
//    //}
//    public class SelEventArgs: EventArgs
//    {
//      private SelectFromTree	mSt;
//      private	DataRowView			mDrv;
//      public SelectFromTree		pSt
//      { 
//        get { return mSt;}
//      }
//      public DataRowView			pDrv
//      {
//        get { return mDrv;}
//      }
//
//      public SelEventArgs(SelectFromTree aSt, DataRowView	aDrv)
//      {
//        mSt = aSt;
//        mDrv = aDrv;
//      }
//    }
//
//    public delegate void SelectEHandler(object sender, SelEventArgs e);
//    public event SelectEHandler OnBegSelect;
//    //public delegate void EndSelectEHandler(object sender, SelEventArgs e);
//    public event SelectEHandler OnEndSelect;
//
//    //public event AfterSelectedTreeItemEventHandler OnStartSelecteTreeItem;
//    public event EventHandler OnAfterSelectedTreeItem;
//
//    public DataGridComboTreeColumn(
//      SelectFromTree aSelTree 
//      //string aFNamePlace, 
//      //string aFNameCode, 
//      //string aFNameDisplay
//      ) 
//      : base()
//    {
//      mSource = null;
//      mIsEditing = false;
//      //mFNamePlace = aFNamePlace;
//      //mFNameCode = aFNameCode;
//      //mFNameDisplay = aFNameDisplay;
//
//      mSelTree = aSelTree;
//      mSelTree.OnLeaveSelecteFromTree += new EventHandler(LeaveComboBox);
//      mSelTree.OnSelectedTreeItem += new SelectFromTree.SelectedTreeItemEventHandler(ComboStartEditing);
//    }
//
//		#region Methods overridden from DataGridColumnStyle
//
//    private void ComboStartEditing(object sender, SelectedTreeItemEventArgs e)
//    {
//      mIsEditing = true;
//      base.ColumnStartedEditing((Control) sender);
//    }
//
//    private void LeaveComboBox(object sender, EventArgs e)
//    {
//      if(mIsEditing)
//      {
//        SetColumnValueAtRow(mSource, mRowNum, mSelTree);
//        mIsEditing = false;
//        Invalidate();
//      }
//      //mSelTree.Parent = null;
//      mSelTree.Hide();
//      this.DataGridTableStyle.DataGrid.Scroll -= new EventHandler(HandleScroll);
//      //this.TextBox.Visible = true;
//      //this.DataGridTableStyle.DataGrid.Focus(); 
//    }
//    private void HandleScroll(object sender, EventArgs e)
//    {
//      if(mSelTree.Visible)
//        mSelTree.Hide();
//    }
//
//    protected override void ConcedeFocus()
//    {
//      //Console.WriteLine("ConcedeFocus");
//      base.ConcedeFocus();
//    }
//
//    protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly,	string instantText, bool cellIsVisible)
//    {
//      base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible);
//
//      mRowNum = rowNum;
//      mSource = source;
//
//      mSelTree.Parent = this.TextBox.Parent;
//      //mSelTree.Location = this.TextBox.Location;
//      //mSelTree.Size = new Size(this.TextBox.Size.Width, mSelTree.Size.Height);
//      mSelTree.Bounds = bounds;
//      DataRowView dt = (DataRowView) source.Current;
//      if (OnBegSelect != null)
//        OnBegSelect(this, new SelEventArgs(mSelTree, dt)); 
//      //if(dt[mFNameCode] == System.DBNull.Value)
//      //	mSelTree.SetItem(0, 0); 
//      //else
//      //	mSelTree.SetItem(Convert.ToInt32(dt[mFNamePlace]), Convert.ToInt32(dt[mFNameCode]));
//      this.TextBox.Visible = false;
//      mSelTree.Visible = true;
//      this.DataGridTableStyle.DataGrid.Scroll += new EventHandler(HandleScroll);
//
//      //if (mSelTree.IsFocused() == false) 
//      //{
//      mSelTree.BringToFront(); 
//      mSelTree.FocusTvTxt();
//      //}
//
//      //DataGridTableStyle.DataGrid.Invalidate(bounds);
//    }
//
//    protected override bool Commit(System.Windows.Forms.CurrencyManager dataSource, int rowNum)
//    {
//      if(mIsEditing)
//      {
//        mIsEditing = false;
//        SetColumnValueAtRow(dataSource, rowNum, mSelTree);
//        if (OnAfterSelectedTreeItem != null)
//          OnAfterSelectedTreeItem (this, new EventArgs()); 
//      }
//      return true;
//    }
//
//    protected override int GetMinimumHeight()
//    {	// Set the minimum height to the height of the combobox
//      return mSelTree.pTextHeight;
//    }
//
//    /*protected override object GetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum)
//    {
//      DataRowView dt = (DataRowView) source.Current;
//      return dt["aCDep"];
//    }*/
//
//    protected override void SetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum, object aValue)
//    {
//      SelectFromTree  sft = (SelectFromTree) aValue;
//      DataRowView dt = (DataRowView) source.Current;
//      if (OnEndSelect != null)
//        OnEndSelect(this, new SelEventArgs(mSelTree, dt)); 
//      //dt[mFNamePlace] =  sft.pItemPlace; 
//      //dt[mFNameCode] =  sft.pItemCode;
//      //dt[mFNameDisplay] =  sft.pItemText;
//    }
//
//		#endregion												 
//
//		#region Helper Methods 
//		#endregion
//
//  }

  public class DataGridComboTreeColumn3: DataGridTextBoxColumn
  {
    private System.Windows.Forms.CurrencyManager _source;
    public event EventHandler OnSelect;

    public DataGridComboTreeColumn3()
    {
      this.TextBox.DoubleClick += new EventHandler(OnSelectTreeItem);
      this.TextBox.ReadOnly = true;
    }

    private void OnSelectTreeItem (object sender, EventArgs e)
    {
      if (OnSelect != null)
        OnSelect (this, new EventArgs());
    }

    public void SetText(int aRow, string aText)
    {
      base.ColumnStartedEditing((Control) TextBox);
      TextBox.Text = aText;
      SetColumnValueAtRow(_source, aRow, aText);
      Invalidate();
    }

    protected override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
    {
      base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible);
      _source = source;
    }

  }


  public class DataGridComboTreeColumn4: DataGridTextBoxColumn
  {
    //private	bool isLockInputLetter;
    private System.Windows.Forms.CurrencyManager	_source;
    //		private int																		_rowNum; 
    //		private System.Drawing.Rectangle							_bounds; 
    //		private bool																	_readOnly; 
    //		private string																_instantText; 
    //		private bool																	_cellIsVisible;

    public event EvH_Text OnSelect;
    public event EvH_Text OnAddWord;
    public event EvH_FindByLetters OnFindByLetters;
    public event EvH_FindByLetters OnLeaveTextBox;

    public DataGridComboTreeColumn4()
    {
      //isLockInputLetter = false;
      this.TextBox.DoubleClick += new EventHandler(OnSelectTreeItem);
      this.TextBox.KeyDown  += new KeyEventHandler(OnKeyDown);
      this.TextBox.KeyPress += new KeyPressEventHandler(OnKeyPress);
      this.TextBox.Leave  += new EventHandler(OnTextLeave);
      this.TextBox.ReadOnly = true;
    }

    private void OnSelectTreeItem (object sender, EventArgs e)
    {
      if (OnSelect != null)
        OnSelect (this, new EvA_Text(TextBox.Text));
    }

    private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {
      if (e.KeyCode == Keys.F3)
      {
        //MessageBox.Show("e.KeyCode == Keys.F3");   
        if (OnSelect != null)
          OnSelect (this, new EvA_Text(TextBox.Text));
      }
      else if (e.KeyCode == Keys.Insert)
      {
        //MessageBox.Show("e.KeyCode == Keys.Insert");   
        if (OnAddWord != null)
        {
          EvA_Text evt = new EvA_Text(TextBox.Text);
          OnAddWord (this, evt);
          if (evt.pOk)
          {
            TextBox.Text = evt.pText; 
            TextBox.Select (evt.pText.Length, 0);
          }
        }
      }
    }

    private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
      if (OnFindByLetters == null)
        return;

      //if (TextBox.Text.Length > 0 && ((int)e.KeyChar) > 0x1F )
      if (((int)e.KeyChar) > 0x1F )
      {
        string newTxt;
        int selectionStart;
        int selectionLength;
        if (TextBox.SelectionStart > 0 && TextBox.SelectionStart < TextBox.Text.Length)
          newTxt = TextBox.Text.Substring(0,TextBox.SelectionStart) + e.KeyChar.ToString(); 
        else
          newTxt = TextBox.Text + e.KeyChar.ToString(); 
        EvA_FindText ft = new EvA_FindText(newTxt);
        OnFindByLetters(this, ft);
        if (ft.pDestTetx.Length > 0 )
        {
          selectionStart = newTxt.Length;
          selectionLength = ft.pDestTetx.Length - newTxt.Length;
          TextBox.Text = ft.pDestTetx;
          TextBox.Select (selectionStart, selectionLength);
          //TextBox.SelectionStart = selectionStart;
          //TextBox.SelectionLength = selectionLength;
        }
        else
        {
          TextBox.Text = newTxt; 
          TextBox.Select (newTxt.Length, 0);
        }
        e.Handled = true;
      }
    }

    private void OnTextLeave(object sender, EventArgs e)
    {
      EvA_FindText ft = new EvA_FindText(TextBox.Text);
      if (OnLeaveTextBox != null)
        OnLeaveTextBox(this, ft);
/*      if (ft.pDestTetx.Length > 0 )
        TextBox.Text = ft.pDestTetx;
      else
        TextBox.Text = string.Empty;
*/        
      TextBox.Text = ft.pDestTetx;
    }

    public void SetText(int aRow, string aText)
    {
      base.ColumnStartedEditing((Control) TextBox);
      TextBox.Text = aText;
      SetColumnValueAtRow(_source, aRow, aText);
      Invalidate();
    }

    protected override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
    {
      //_isEditing = true;
      base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible);
      _source = source;
      //			_rowNum = rowNum; 
      //			_bounds = bounds; 
      //			_readOnly = readOnly; 
      //			_instantText = instantText; 
      //			_cellIsVisible = cellIsVisible;
    }


  }

  public class DataGridDateTimePickerColumn:System.Windows.Forms.DataGridColumnStyle
  {
    #region Req 4 drawing

    private SolidBrush _fcbr; // fore color brush
    private SolidBrush _bcbr; // back color brush
    private Font _fnt;        //

    #endregion

    protected DataGridDateTimePicker m_DateTime ;
				
    public DataGridDateTimePickerColumn()
    {
			
    }
		
    #region		Init datagrid Column class
		
    protected override void SetDataGridInColumn ( DataGrid datagrid )
    {
      m_DateTime = new DataGridDateTimePicker();
      this.m_DateTime.Visible = false;
      datagrid.Controls.Add(this.m_DateTime  );
    }
    //override Size Methods
    protected override  int GetPreferredHeight ( Graphics g,object value )
    {
      return m_DateTime.Height;
    }
    protected override  int GetMinimumHeight()
    {
      return m_DateTime.Height;
    }
    protected override  Size GetPreferredSize ( Graphics g, object value )
    {
      return m_DateTime.Size;
    }
    protected new void ConcedeFocus() 
    {
      // Hide the control when conceding focus.
      this.m_DateTime.Visible = false;
    }


    #endregion

    #region Control value change events

    protected override  void Abort ( int rowNum )
    {
      this.ConcedeFocus();
    }
    protected override  bool Commit ( CurrencyManager dataSource, int rowNum )
    {
      this.SetColumnValueAtRow(dataSource,rowNum,this.m_DateTime.Value);
      this.ConcedeFocus();
      return true;
    }


    #endregion
    // More in edit mode
    protected override  void Edit ( CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible )
    {
      if(this.m_DateTime.DataBindings.Count ==0)
        this.m_DateTime.DataBindings.Add(
          new System.Windows.Forms.Binding("Text"
          ,this.DataGridTableStyle.DataGrid.DataSource
          ,this.DataGridTableStyle.MappingName +"."+ this.MappingName));
      try
      {
        this.m_DateTime.SetBounds(bounds.X,bounds.Y,bounds.Width,bounds.Height);
        this.m_DateTime.Show();
        this.m_DateTime.Focus();
      }
      catch(System.Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex.Message);
        System.Diagnostics.Debug.WriteLine(ex.Source);
      }

    }
	  #region Paint overloads

    protected override  void Paint ( Graphics g 
      , Rectangle bounds 
      , CurrencyManager source 
      , int rowNum 
      , bool alignToRight )
    {
      if(true)
      {
        this._bcbr = new System.Drawing.SolidBrush( this.DataGridTableStyle.BackColor);
        this._fcbr = new System.Drawing.SolidBrush(this.DataGridTableStyle.ForeColor);
        this._fnt = this.DataGridTableStyle.DataGrid.Font;
      }
      //this code should be modified 2 make row selection effect
      // yeh... where is double buffering
      g.FillRectangle(_bcbr,bounds);
      g.DrawString(
        this.GetColumnValueAtRow(source,rowNum).ToString()
        , _fnt
        ,_fcbr
        ,bounds.X + 2
        ,bounds.Y + 2 );
    }
    protected override  void Paint ( Graphics g, Rectangle bounds, CurrencyManager source, int rowNum )
    {
      //I can`t catch 4 what....
    }
    #endregion
  }

  public class DataGridDateTimePicker:System.Windows.Forms.DateTimePicker
  {
    public DataGridDateTimePicker()
    {
      this.Width = 80;
      this.TabIndex = 0; // if else it never get keyboard focus
      this.Format =System.Windows.Forms.DateTimePickerFormat.Short;
		
    }
  }

  //Автор кода Конунов Дмитрий Владимирович
  //e-mail:cktro@hotmail.com,Самара ТеррНИИГражданпроект www.stri.ru
  //адаптация AndryC
  //
  public class GridStyleButton: DataGridTextBoxColumn
  {

    public delegate void EventHandlerColumnClick(object sender,EventClick e);
//    public delegate void EventHandlerColumnClick(object sender, SelectedTreeItemEventArgs e);

    public event EventHandlerColumnClick OnPushButton;

    private int _columnNum ;
    private int _pressedRow = -1;
    private Label bt;
    private DataGrid dg;
    private Rectangle Rbt=new Rectangle(0,0,0,0);
    private Rectangle textBound=new Rectangle(0,0,0,0);
    private Rectangle Bound;
    public GridStyleButton(int columnNum)
    {

      bt=new Label();
      bt.Visible=false;
      bt.BackColor=SystemColors.Control;
      bt.FlatStyle = FlatStyle.Flat;
      bt.Text="6"; // v стрелка вниз
//      bt.Click+=new EventHandler(onClick);
      bt.MouseDown+=new MouseEventHandler(onMouseDown);
      bt.MouseUp+=new MouseEventHandler(onMouseUp);
      bt.Visible=true;
      _columnNum=columnNum;
    }
    #region Overrides
    //передается DataGrid при добавлении в коллекцию колонок
    protected override void SetDataGridInColumn ( DataGrid Dg )
    {
      bt.Visible = false;
      dg=Dg;
      dg.Controls.Add(bt);
    }
    protected override  int GetPreferredHeight ( Graphics g,object value )
    {
      return bt.Height;
    }
    protected override  int GetMinimumHeight()
    {
      return this.TextBox.Height;
    }
    protected override  Size GetPreferredSize ( Graphics g, object value )
    {
      return bt.Size;
    }
    private void paintButton(Graphics g,Font fnt,Rectangle rb,SolidBrush sb,string s)
    {
      StringFormat drawFormat = new StringFormat(StringFormatFlags.DirectionVertical);
      g.DrawString(s, fnt, sb, Rbt.Left, Rbt.Top,drawFormat);
      //рисуем окантовку кнопки
      g.DrawRectangle(new Pen(new SolidBrush(SystemColors.ControlDark),1), rb);
    }
    #endregion
    protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
    {

      Rectangle Rect = new Rectangle();
      //      Bitmap bm = this._buttonFace;
      //      bm.h
      Rect = bounds;
      Bound = bounds;
      Font fnt = new Font("Marlett" ,this.DataGridTableStyle.DataGrid.Font.Size);
      int wi=(int)g.MeasureString("6",fnt,this.FontHeight).Width+2;

      //размеры для TextBox
      Rect.Width -= wi;
      base.Paint(g,Rect, source,rowNum, backBrush, foreBrush, alignToRight);
      //размеры для кнопки      
      Rbt=new Rectangle(Rect.Left+Rect.Width,Rect.Top,wi,Rect.Height);
      //делаем все для кнопки!
      bt.Font=fnt;
      bt.Bounds=Rbt;
//      g.FillRectangle(new SolidBrush(SystemColors.Window), bounds);
//      g.FillRectangle(new SolidBrush(SystemColors.Control), Rbt);

//      SolidBrush sb=new SolidBrush(this.DataGridTableStyle.DataGrid.ForeColor);
/*      if (_pressedRow == rowNum)
        bt.Text="5";
//        paintButton(g,fnt,Rbt,sb,"5"); //^
      else
        bt.Text="6";
*/        
//        paintButton(g,fnt,Rbt,sb,"6"); //V
//      string s= this.GetColumnValueAtRow(source, rowNum).ToString();
      //выводим текст который там был
//      g.DrawString(s, this.DataGridTableStyle.DataGrid.Font, new SolidBrush(this.DataGridTableStyle.DataGrid.ForeColor), bounds.X, bounds.Y);
//      if (this.TextBox.Focused)
//        g.DrawRectangle(new Pen(new SolidBrush(SystemColors.HotTrack),1), Rect);
    }    

    protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly , string instantText, bool cellIsVisible)
    {
      Bound=bounds;
      Rectangle Rect =new Rectangle(bounds.X+1,bounds.Y+1,bounds.Width-Rbt.Width-1,this.TextBox.Height-5);
//      _pressedRow=rowNum;
      this.bt.Bounds=Rect;
      this.bt.Visible=true;
      base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible);
//      this.TextBox.Focus();
    }
/*
    protected override void EndEdit()
    {
      bt.Visible=false;     
    }
*/    
    protected override bool Commit(CurrencyManager source, int rowNum)
    {
      bt.Visible=false;
      return base.Commit(source, rowNum);
    }

    protected override void Abort(int rowNum)
    {
      bt.Visible=false;
      base.Abort(rowNum);
    }

    private void onClick(object sender, EventArgs e)
    {
      //Click
      onPushButton(sender,new EventClick(_pressedRow,_columnNum,this.Bound));
    }

    /// <summary>
    /// Отрабатываем нажатие мышкой кнопки
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void onMouseDown(object sender, MouseEventArgs e)// Handles DataGrid.MouseDown
    {

      if (_columnNum!=-1 && dg.CurrentRowIndex!=-1)
      {
        Rectangle rect = dg.GetCellBounds(dg.CurrentRowIndex, _columnNum);
//        isClickInCell = e.X > rect.Right - Rbt.Width;
        if (rect.Left >= 0 )
        {
          bt.Bounds=new Rectangle(bt.Bounds.X+1,bt.Bounds.Y+1,bt.Bounds.Width-1,bt.Bounds.Height-1);
          //        Graphics g = Graphics.FromHwnd(dg.Handle);
          //        Bitmap bm = this._buttonFacePressed;
          //        g.DrawImage(bm, rect.Right - bm.Width, rect.Y);
          //        g.Dispose();
          _pressedRow = dg.CurrentRowIndex;
          onPushButton(dg,new EventClick(dg.CurrentRowIndex,_columnNum,this.Bound));
        }
        bt.Refresh();
      }
    }
    private void onMouseUp(object sender, MouseEventArgs e) //HandleMouseUp
    {
      if (_columnNum!=-1 && dg.CurrentRowIndex!=-1)
      {
        Rectangle rect = dg.GetCellBounds(dg.CurrentRowIndex, _columnNum);
        if (e.X < rect.Right - Rbt.Width)
        {
          bt.Bounds=new Rectangle(bt.Bounds.X-1,bt.Bounds.Y-1,bt.Bounds.Width+1,bt.Bounds.Height+1);
//          Graphics g= Graphics.FromHwnd(dg.Handle);
          //        Bitmap bm = this._buttonFace;
          //        g.DrawImage(bm, rect.Right - bm.Width, rect.Y);
//          g.Dispose();
          //        _pressedRow = hti.Row;
        }
        bt.Refresh();
        _pressedRow = -1;
      }
    } 
    private void onPushButton(object sender, EventClick e)
    {
      if (OnPushButton!=null)
        OnPushButton(sender,e);
//      MessageBox.Show(e.ToString(),"Нажата кнопка!");
    }
  }
  /// <summary>
  /// Вспомогательный класс для передачи событий от мыши в DataGrid
  /// </summary>
  public class EventClick: EventArgs
  {
    private int mRow, mCol, mouseX, mouseY;
    private MouseButtons mouseButton;
    public Rectangle Rect;
    
    /// <summary>
    /// Класс для передачи параметров мыши и поля
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="r"></param>
    public EventClick(int row,int col,Rectangle r)
    {
      mRow=row;
      mCol=col;
      mouseX=Control.MousePosition.X;
      mouseY=Control.MousePosition.Y;
      mouseButton=Control.MouseButtons;
      Rect = r;
    }
    public override string ToString()
    {
      return "{ EventClick: row="+this.Row+", col="+this.Col+", mouseX="+this.mouseX+", mouseY="+this.mouseY+", mouseButton="+Control.MouseButtons+"}";
    }
    public MouseButtons MouseButton
    {
      get
      {
        return mouseButton;
      }
    }
    public int Row
    {
      get
      {
        return mRow;
      }
    }
    public int Col
    {
      get
      {
        return mCol;
      }
    }
    public Rectangle Bound
    {
      get
      {
        return Rect;
      }
    }
    public bool isEmpty
    {
      get
      {
        return mCol==-1 && mRow ==-1;
      }
    }
  }


	#region ПРОБЫ
	/*
	/// <summary>
	/// из журнала MSDN magazine август 2003 [№8(20)]
	/// стр.35-43
	/// by Kristy Saunders
	/// derive class from DataGridTextBoxColumn
	/// </summary>
	public class DataGridComboBoxColumnKS : DataGridTextBoxColumn
	{
		// Hosted ComboBox control
		private ComboBox comboBox;
		private CurrencyManager cm;
		private int iCurrentRow;
		
		// Constructor - create combobox, register selection change event handler,
		// register lose focus event handler
		public DataGridComboBoxColumnKS()
		{
			this.cm = null;

			// Create ComboBox and force DropDownList style
			this.comboBox = new ComboBox();
			this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			
			// Add event handler for notification of when ComboBox loses focus
			this.comboBox.Leave += new EventHandler(comboBox_Leave);
		}
		
		// Property to provide access to ComboBox
		public ComboBox ComboBox
		{
			get { return comboBox; }
		}																				 
																									
		// On edit, add scroll event handler, and display combo box
		protected override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
		{
			Debug.WriteLine(String.Format("Edit {0}", rowNum));
			base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible);

			if (!readOnly && cellIsVisible)
			{
				// Save current row in the datagrid and currency manager associated with
				// the data source for the datagrid
				this.iCurrentRow = rowNum;
				this.cm = source;
		
				// Add event handler for datagrid scroll notification
				this.DataGridTableStyle.DataGrid.Scroll += new EventHandler(DataGrid_Scroll);

				// Site the combo box control within the bounds of the current cell
				this.comboBox.Parent = this.TextBox.Parent;
				Rectangle rect = this.DataGridTableStyle.DataGrid.GetCurrentCellBounds();
				this.comboBox.Location = rect.Location;
				this.comboBox.Size = new Size(this.TextBox.Size.Width, this.comboBox.Size.Height);

				// Set combo box selection to given text
				this.comboBox.SelectedIndex = this.comboBox.FindStringExact(this.TextBox.Text);

				// Make the ComboBox visible and place on top text box control
				this.comboBox.Show();
				this.comboBox.BringToFront();
				this.comboBox.Focus();
			}
		}

		// Given a row, get the value member associated with a row.  Use the value
		// member to find the associated display member by iterating over bound datasource
		protected override object GetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum)
		{
			Debug.WriteLine(String.Format("GetColumnValueAtRow {0}", rowNum));
			// Given a row number in the datagrid, get the display member
			object obj =  base.GetColumnValueAtRow(source, rowNum);
			
			// Iterate through the datasource bound to the ColumnComboBox
			// Don't confuse this datasource with the datasource of the associated datagrid
			CurrencyManager cm = (CurrencyManager) 
				(this.DataGridTableStyle.DataGrid.BindingContext[this.comboBox.DataSource]);
			// Assumes the associated DataGrid is bound to a DataView, or DataTable that
			// implements a default DataView
			DataView dataview = ((DataView)cm.List);
								
			int i;

			for (i = 0; i < dataview.Count; i++)
			{
				if (obj.Equals(dataview[i][this.comboBox.ValueMember]))
					break;
			}
			
			if (i < dataview.Count)
				return dataview[i][this.comboBox.DisplayMember];
			
			return DBNull.Value;
		}

		// Given a row and a display member, iterating over bound datasource to find
		// the associated value member.  Set this value member.
		protected override void SetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum, object value)
		{
			Debug.WriteLine(String.Format("SetColumnValueAtRow {0} {1}", rowNum, value));
			object s = value;

			// Iterate through the datasource bound to the ColumnComboBox
			// Don't confuse this datasource with the datasource of the associated datagrid
			CurrencyManager cm = (CurrencyManager) 
				(this.DataGridTableStyle.DataGrid.BindingContext[this.comboBox.DataSource]);
			// Assumes the associated DataGrid is bound to a DataView, or DataTable that
			// implements a default DataView
			DataView dataview = ((DataView)cm.List);
			int i;

			for (i = 0; i < dataview.Count; i++)
			{
				if (s.Equals(dataview[i][this.comboBox.DisplayMember]))
					break;
			}

			// If set item was found return corresponding value, otherwise return DbNull.Value
			if(i < dataview.Count)
				s =  dataview[i][this.comboBox.ValueMember];
			else
				s = DBNull.Value;
			
			try
			{
				base.SetColumnValueAtRow(source, rowNum, s);
			}
			catch{}
		}

		// On datagrid scroll, hide the combobox
		private void DataGrid_Scroll(object sender, EventArgs e)
		{
			Debug.WriteLine("Scroll");
			this.comboBox.Hide();
		}

		// On combo box losing focus, set the column value, hide the combo box,
		// and unregister scroll event handler
		private void comboBox_Leave(object sender, EventArgs e)
		{
			DataRowView rowView = (DataRowView) this.comboBox.SelectedItem;
			string s = (string) rowView.Row[this.comboBox.DisplayMember];
			Debug.WriteLine(String.Format("Leave: {0} {1}", this.comboBox.Text, s));

			SetColumnValueAtRow(this.cm, this.iCurrentRow, s);
			Invalidate();

			this.comboBox.Hide();
			this.DataGridTableStyle.DataGrid.Scroll -= new EventHandler(DataGrid_Scroll);			
		}
	}

	
	public class DataGridComboBoxColumnDD : DataGridTextBoxColumn
	{
		private ComboBox					_cb;
		private CurrencyManager		_cm;
		private int								_curRow;

		private OleDbConnection		_cn;
		private string						_sqlSelect;

		public ComboBox						pComboBox
		{
			get { return _cb; }
		}																				 
																									

		public DataGridComboBoxColumnDD(OleDbConnection aCn, string aSQLSelect)
		{
			_cm					= null;
			_curRow			= -1;
			_cn					= aCn;
			_sqlSelect	= aSQLSelect;

			_cb = new ComboBox();
			_cb.DropDownStyle = ComboBoxStyle.DropDown;
			//_cb.DrawMode = DrawMode.OwnerDrawFixed;
			
			_cb.Leave += new EventHandler(OnComboBoxLeave);
		}

		protected override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
		{
			base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible);

			if (!readOnly && cellIsVisible)
			{
				//if (_curRow != -1)
				//	this.ColumnStartedEditing(_cb);
				_curRow = rowNum;
				_cm = source;
		
				this.DataGridTableStyle.DataGrid.Scroll += new EventHandler(OnDataGridScroll);

				FillComboBox();

				_cb.Parent = this.TextBox.Parent;
				Rectangle rect = this.DataGridTableStyle.DataGrid.GetCurrentCellBounds();
				_cb.Location = rect.Location;
				_cb.Size = new Size(this.TextBox.Size.Width, _cb.Size.Height);

				// Set combo box selection to given text
				_cb.Text = this.TextBox.Text;

				// Make the ComboBox visible and place on top text box control
				_cb.Show();
				_cb.BringToFront();
				_cb.Focus();
			}
		}

		protected override object GetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum)
		{
			object obj =  base.GetColumnValueAtRow(source, rowNum);
			Debug.WriteLine("GetColumnValueAtRow -> "+obj.ToString());
			return obj;
		}

		
		protected override void SetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum, object value)
		{
			object s = value;
			Debug.WriteLine("SetColumnValueAtRow -> "+s.ToString());
			base.SetColumnValueAtRow(source, rowNum, s);
		}
		

		protected override bool Commit(System.Windows.Forms.CurrencyManager dataSource, int rowNum)
		{
			//SetColumnValueAtRow(dataSource, rowNum, _cb.Text);
			TextBox.Text = _cb.Text;
			return base.Commit(dataSource, rowNum);
		}


		protected override int GetMinimumHeight()
		{	// Set the minimum height to the height of the combobox
			return _cb.Height+1;
		}

		private void OnDataGridScroll(object sender, EventArgs e)
		{
			_cb.Hide();
		}

		private void OnComboBoxLeave(object sender, EventArgs e)
		{
			try {SetColumnValueAtRow(_cm, _curRow, _cb.Text);} 
			catch {}
			Invalidate();

			_curRow	= -1;
			_cb.Hide();
			this.DataGridTableStyle.DataGrid.Scroll -= new EventHandler(OnDataGridScroll);
		}

		private void FillComboBox()
		{
			OleDbCommand cmd = new OleDbCommand(_sqlSelect,_cn);
			OleDbDataReader dReader = null;
			try
			{
				dReader = cmd.ExecuteReader();
				_cb.Items.Clear();
				while (dReader.Read())
					_cb.Items.Add(dReader[0]);
			}
			//catch {}
			finally
			{
				if (dReader != null)
					dReader.Close();
			}

		}
	}
	*/
	#endregion ПРОБЫ
}
