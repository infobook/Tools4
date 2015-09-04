using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.DataGridColumnStyle
{
  public class DataGridColumnCASSparrow : DataGridColumnBase
  {
		#region Public Property

		public CASSparrow				pSparrow
		{
			get
			{
				return pControl as CASSparrow;
			}
		}
		
		#endregion --Public Property

		#region constructor

    /// <summary>
    /// Формат записи данных в ячейки:
    /// pIsPDCFormat = true		- формат <place><delimiter><code>
    /// pIsPDCFormat = false	- формат <code> (default)
    /// </summary>
    //public bool											pIsPDCFormat;

    public DataGridColumnCASSparrow()
    {
      CASSparrow mSparrow = new CASSparrow(true);
      mSparrow.Visible = false;
      mSparrow.OnValueChanged+= new EventHandler(ControlChanged);
      mSparrow.OnSelectedTreeItem += new CASSparrow.SelectedTreeItemEventHandler(onSparrowChange);

			((DataGridTextBox)mSparrow.pTextBox).IsInEditOrNavigateMode=true;
      this.pControl=mSparrow;
    }


		#endregion --constructor

		//выбор из дерева
    private void onSparrowChange(object sender, EvA_SelectedTreeItem e) 
    {
      int curRowNum=getCurrencyManager.Position;
      if (curRowNum==-1)
        return;
      if (pIsPDCFormat)
        SetColumnValueAtRow(getCurrencyManager,curRowNum,e.pPC.PlaceDelimCode);
      else
        SetColumnValueAtRow(getCurrencyManager,curRowNum,e.pPC.code);

			_ControlChanged();
			this.ColumnStartedEditing(pSparrow);
    }

    private bool pIsPDCFormat
    {
      get
      {
        if (getTypeFieldMapping==typeof(int)) // - по умолчанию
          return false;
        else //if (type=="system.string")
          return true;
      }
    }
    #region Устанавливаем и считываем значение  контрола

    /// <summary>
    /// Переписываем сво-во установки и считывания значения с контрола !
    /// </summary>
    public override object ValueControl
    {
      get
      {
        object ret=null;
        try 
        {
					object value;
					if (pIsPDCFormat)
						value=pSparrow.pItemPC.PlaceDelimCode;
					else
						value=pSparrow.pItemCode;
					ret=value;
        } 
        catch {}
        return ret;
      }
      //устанавливаем значение в контроле
      set
      {
        try
        {
					if (value.GetType()==typeof(string))
					{
						pSparrow.pItemText = value.ToString();
					}
					else
					{
						//pSparrow.pItemText=string.Empty;
						pSparrow.pItemText = pSparrow.pTreeView.SearchByText(value.ToString()).Text;
					}
				}
        catch{}
      }
    }
    #endregion

    #region Set и Get для отображения значений

    protected override object GetColumnValueAtRow(CurrencyManager source, int rowNum) //override
    {
      string ret = NullText;
      try
      {
        string obj = base.GetColumnValueAtRow (source, rowNum).ToString();
        if (obj.Length > 0 && obj!="0")
        {
          if (pIsPDCFormat)
            ret = pSparrow.pTreeView.SearchByCode(PlaceCode.PDC2PlaceCode(obj)).Text;
          else
            ret = pSparrow.pTreeView.SearchByCode(CommandAS.Tools.CASTools.ConvertToInt32Or0(obj)).Text;
        }
      }
      catch {}
      return ret;
    }

		protected override void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value)
		{
			//Rectangle rect= this.DataGridTableStyle.DataGrid.GetCurrentCellBounds();
			if (rowNum!=this.DataGridTableStyle.DataGrid.CurrentCell.RowNumber)
			{
				//source.Position=this.DataGridTableStyle.DataGrid.CurrentCell.RowNumber;
				//if (this.DataGridTableStyle.DataGrid.CurrentCell.RowNumber>this.getDataTable.Rows.Count-1)
				source.AddNew();
			}
			base.SetColumnValueAtRow (source, rowNum, value);
			//pSparrow.pItemText = pSparrow.pTreeView.SearchByText(value.ToString()).Text;
		}

    #endregion

		#region вспомогательные ф-ции работы с DataGrid

    protected override void ColumnStartedEditing(Control editingControl)
    {
			if (this.DataGridTableStyle.DataGrid.ReadOnly)
				return;
      base.ColumnStartedEditing(editingControl);
    }

		protected override void SetDataGridInColumn(DataGrid value) //override
		{
			base.SetDataGridInColumn(value);
			if (pControl!=null)
			{
				DataGridTextBox dg=pSparrow.pTextBox as DataGridTextBox;
				if (dg!=null)
				{
					dg.SetDataGrid(value);
				}
			}
		}


		#endregion --вспомогательные ф-ции работы с DataGrid

    #region Размеры-с

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
    #endregion

    #region служебная часть Windows .Net

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
				pControl.Dispose();
      }
      base.Dispose( disposing );
    }

		#endregion

	}
  /// <summary>
  /// Класс для вставки в DataGrid столбца типа ComboPictures
  /// </summary>
  public class DataGridColumnPictures : DataGridColumnBase
  {
    private System.ComponentModel.Container components = null;

		private ucComboPicture								_cp
		{
			get
			{
				return (ucComboPicture) pControl;
			}
		}

		private ImageList mImageList=null;
    /// <summary>
    /// Получаем коллекцию иконок снаружи !!!
    /// </summary>
		public ImageList pImageList
		{
			get
			{
				return mImageList;
			}
			set
			{
				mImageList=value;
				_cp.ImageList=value;
			}
		}
		/*
    public IconCollection IconCollection
    {
      get
      {
        //if (ic==null) ic=new CommonIconCollection(16);
        return ic;
      }
      set
      {
        if (value==null)
          return;
        ic=value;
        _cp.pIcons=ic; //бросаем дельше !
      }
    }*/
    /// <summary>
    /// Добавление в коллекцию
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="indexImage"></param>
    /// <returns></returns>
    public int AddItem(EvA_CodeText obj,int IndexImage)
    {
      if (pImageList!=null)
        return _cp.AddItem(obj,IndexImage); //ic.GetIndexfromType(obj.pCode)
      else
        return -1;
    }
		
		private EvA_CodeText getObj(int type)
		{
			EvA_CodeText ret=null;
			foreach(EvA_CodeText et in _cp.ComboBox.Items)
			{
				if (et!=null && et.pCode==type)
				{
					return et;
				}
			}
			return ret;
		}

    public DataGridColumnPictures()  
    {
      InitializeComponent();
     
      ucComboPicture _cp = new ucComboPicture();
      _cp.Visible = false;
      _cp.ComboBox.SelectedIndexChanged += new EventHandler(ControlChanged);

      this.pControl=_cp;
      this.NullText=string.Empty;
    }

    public ComboBox ComboBox
    {
      get
      {
        return _cp.ComboBox;
      }
    }

		public override System.Drawing.Size MinimumSize
		{
			get
			{
				if (pImageList!=null)
					return pImageList.ImageSize;
				else
					return new System.Drawing.Size(32, 32);
			}
		}

    #region Установка размеров
    protected override System.Drawing.Size GetPreferredSize(Graphics g, object value) 
    {
      if (pImageList==null)
        return new Size(32, 32+32);
      else
        return new Size(pImageList.ImageSize.Width,pImageList.ImageSize.Height*2);
    }

    protected override int GetMinimumHeight() 
    {
      return pImageList.ImageSize.Height + 2;
    }

    protected override int GetPreferredHeight(Graphics g, object value) 
    {
      return pImageList.ImageSize.Height + 4;
    }
    #endregion

    #region Paints

    /// <summary>
    /// Рисуем все заново
    /// </summary>
    /// <param name="g"></param>
    /// <param name="bounds"></param>
    /// <param name="source"></param>
    /// <param name="rowNum"></param>
    /// <param name="backBrush"></param>
    /// <param name="foreBrush"></param>
    /// <param name="alignToRight"></param>
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
      if (ReadOnly)
      {
        //режим выделения
        if (!isSelect)
        {
          backBrush=SystemBrushes.ControlLightLight;
        }
      }
      g.FillRectangle(backBrush,rect);
      Image icon=null;
      int index=0;
      try
      {
        index=(int)value; //указан тип объекта
				EvA_CodeText ec= getObj(index);
        if (ec!=null && _cp.ImageList!=null)
        {

          index=_cp.IndexImageFromType(index);
          icon=_cp.ImageList.Images[index];
        }
      }
      catch{}
      if (pImageList!=null)
      {
        rect.Width=pImageList.ImageSize.Width;
        rect.Height=pImageList.ImageSize.Height;
      }
      //g.DrawString(text, this.DataGridTableStyle.DataGrid.Font, foreBrush, rect,StringFormat.GenericTypographic);
      if (icon!=null)
        g.DrawImage(icon,rect);
    }

    #endregion
		
		#region Staff .Net

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
    }	

		#endregion --Staff .Net
  }

  /// <summary>
  /// Класс для вставки в DataGrid столбца типа OpenDate
  /// </summary>
  public class DataGridColumnOpenDate : DataGridColumnBase
  {
    private System.ComponentModel.Container components = null;

		private ucOpenDate											_odt
		{
			get
			{
				return pControl as ucOpenDate;
			}
		}

    /// <summary>
    /// Используем для записи поле типа DateTime
    /// </summary>
    protected bool               pFieldIsDateTime=true;

    public DataGridColumnOpenDate()  
    {
      InitializeComponent();
     
      ucOpenDate _odt = new ucOpenDate();
      _odt.Visible = false;
      _odt.OnControlChanged += new EventHandler(ControlChanged);

      this.pControl=_odt;
    }


    #region Get и Set в ячейке

    protected override object GetColumnValueAtRow(CurrencyManager source, int rowNum) //override
    {
      pFieldIsDateTime=FieldIsDateTime();
      object obj=null;
      try
      {
        obj=base.GetColumnValueAtRow(source,rowNum);
        if (pFieldIsDateTime)
          obj=CommandAS.Tools.CASTools.DateWithFullYear((DateTime)obj);
        else
          obj=OpenDate.GetRusFromOpenDate(obj.ToString());
      }
      catch{}

      if (obj==null || obj==System.DBNull.Value)
        obj=NullText;
			//else
				//_odt.pText=obj.ToString();
      return obj;
    }
		/*
		public override object ValueControl
		{
			get
			{
				return base.ValueControl;
			}
			set
			{
				_odt.pText = value as string;
			}
		}
		*/
    protected override void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value) //override
    {
      if (ReadOnly)
        return;
      pFieldIsDateTime=FieldIsDateTime();
      value=NullText;
      try
      {
        if (pFieldIsDateTime)
          value=CommandAS.Tools.CASTools.DateWithFullYear((DateTime)_odt.pDateTime);
        else
          value=_odt.Text;
      }
      catch{}
      string sOld=GetColumnValueAtRow(source,rowNum).ToString();
			string newStr=value as string;
			bool newValue=false;
			if (!pFieldIsDateTime)
				newStr=OpenDate.GetRusFromOpenDate(newStr);
			newValue=!newStr.Equals(sOld);
      if (newValue)
      {
        //только если что-то изменилось !!!
        base.SetColumnValueAtRow(source,rowNum,value);
      }
    }

    /// <summary>
    /// Определяем тип поля и записываем его в локальную переменную pFieldIsDateTime
    /// </summary>
    /// 
    protected bool FieldIsDateTime()
    {
      try
      {
        return (getTypeFieldMapping.FullName=="System.DateTime");
      }
      catch{}
      return false;
    }

    #endregion

		#region Staff .Net

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
    }	

		#endregion --Staff .Net
  }

  /// <summary>
  /// Основной (базовый) класс для работы со столбцами в DataGrid
  /// </summary>
  public class DataGridColumnBase : System.Windows.Forms.DataGridColumnStyle //DataGridTextBoxColumn DataGridColumnStyle
  {
    public event EventHandler onControlChanged;

    /// <summary>
    /// Устанавливаем автоматически ширину столбца
    /// </summary>
    public virtual bool               pAutoWidth
    {
      set
      {
        if (value && pControl!=null)
        {
          Graphics g= this.pControl.CreateGraphics();
          this.Width=GetPreferredSize(g,this.pControl).Width;
          g.Dispose();
        }
      }
    }
    protected Control				          mControl;
    protected virtual Control				  pControl
    {
      get
      {
        return mControl;
      }
      set
      {

        if (value==null)
					return;
				mControl=value;
				mControl.TextChanged -= new EventHandler(ControlChanged);
        mControl.TextChanged += new EventHandler(ControlChanged);
      }
    }

    // The isEditing field tracks whether or not the user is
    // editing data with the hosted control.
    protected bool isEditing;
    protected bool IsLock;
		
		#region constructors

    public DataGridColumnBase() : this(null){}
    public DataGridColumnBase(Control contrl) : base() 
    {
      pControl=contrl;
    }

		#endregion --constructors

    #region Рабочие режимы

		/// <summary>
		/// Считываем и записываем значение с контрола !!!
		/// по умолчанию э то св-во  Text
		/// </summary>
		/// <returns></returns>
		public virtual object ValueControl
		{
			get
			{
				return pControl.Text;
			}
			set
			{
				if (value!=null)
					pControl.Text=value.ToString();
				else
					pControl.Text=string.Empty;
			}
		}

    protected override void Abort(int rowNum) //override 
    {
			ConcedeFocus();
      Invalidate();
    }

    protected override bool Commit(CurrencyManager source, int rowNum) //override
    {
#if DEBUG
      Helper.Trace( "Commit", "row="+rowNum, "isEditing="+isEditing, "pValue=" + pControl.Text);
#endif
			if (!isEditing)
				return true;

			isEditing = false;
			try 
      {
				DataRowView rw=(DataRowView) source.Current;
				int ps=source.Position;
				//rw.EndEdit();

				object obj=ValueControl;
				object old=GetColumnValueAtRow(source,rowNum);
				bool newValue= !obj.Equals(old);
        //только если что-то было выбрано !!!
        if (newValue)
        {
					SetColumnValueAtRow(source, rowNum, obj);
					//генерим событие !
          _ControlChanged();
					//rw.Row.AcceptChanges();
        }
        this.DataGridTableStyle.DataGrid.UnSelect(rowNum);
      }
      catch//(Exception) 
      {
        Abort(rowNum);
        return false;
      }

      ConcedeFocus();
      Invalidate();

      return true;
    }

    protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly,	string instantText, bool cellIsVisible) //override
    {
#if DEBUG && TRACE
      Helper.Trace( "-->Edit ", "rowNum="+rowNum, "isEditing="+isEditing, "cellIsVisible="+cellIsVisible);
#endif
      if (readOnly || this.ReadOnly || this.DataGridTableStyle.DataGrid.ReadOnly)
        return;
			/*
			if (isEditing)
			{
				Commit(source,rowNum);
				return;
			}
			*/
			//base.Edit(source,rowNum,bounds,readOnly,instantText,cellIsVisible);
			if (pControl!=null)
      {
				if (cellIsVisible) 
        {
					IsLock=true;
					isEditing=true;

					//делаем шире колонку (по контролу)
					if (pControl.Width>this.Width)
						this.Width=pControl.Width;

          pControl.Bounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
					
					string value = GetColumnValueAtRow(source, rowNum).ToString();
					ValueControl=value; //устанавливаем значение через object !!!
					//выделяем строку ??
          //this.DataGridTableStyle.DataGrid.Select(rowNum);
					//DataRowView rw=(DataRowView) source.Current;
					/*
					//если новый столбец !!!
					if (rw.IsNew)
						rw.EndEdit();
					else
						ValueControl=value; //устанавливаем значение через object !!!
					*/
					this.ColumnStartedEditing(pControl);
					IsLock=false;
				}
				pControl.Visible = cellIsVisible;
      }
    }

		protected override void ColumnStartedEditing(Control editingControl) //override
		{
			//isEditing=true;
#if DEBUG && TRACE
			Helper.Trace( "ColumnStartedEditing(...)","CanFocus="+editingControl.CanFocus, "editingControl.Visible=" + editingControl.Visible);
#endif
			//base.ColumnStartedEditing(editingControl);
			if (editingControl!=null)
			{
				editingControl.Visible=true;
				editingControl.Show();

				if (editingControl.CanFocus)
					editingControl.Focus();
			}
		}

    protected override void ConcedeFocus()  //
    {
			isEditing = false;
      // Hide the control when conceding focus.
      if (pControl!=null)
        pControl.Visible = false;
			//this.DataGridTableStyle.DataGrid.Focus();
			//удаляем все пустые строки !!!
			//CASTools.DataTableDeleteAllEmptyRow((DataView)getCurrencyManager.List, this.getDataTable);
    }

    #endregion

		#region Получение данных о минимальных размеров

		public int MinimumWidth
		{
			get
			{
				return MinimumSize.Width;
			}
		}

		public virtual System.Drawing.Size MinimumSize
		{
			get
			{
				return pControl.Size;
			}
		}
		#endregion --Получение данных о минимальных размеров

    #region Установка размеров

    protected override Size GetPreferredSize(Graphics g, object value) 
    {
      return new Size(pControl.Width, pControl.Height + 4);
    }

    protected override int GetMinimumHeight() 
    {
      return pControl.Height + 4;
    }

    protected override int GetPreferredHeight(Graphics g, object value) 
    {
      return pControl.Height + 4;
    }

    #endregion

    #region Прорисовка
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
        //режим выделения
        if (isSelect)
        {
          backBrush=new SolidBrush(SystemColors.ActiveCaption); //new HatchBrush(HatchStyle.Percent10, SystemColors.ActiveCaption);
        }
        else
        {
          backBrush=new SolidBrush(SystemColors.Control); //new HatchBrush(HatchStyle.Percent10, SystemColors.ActiveCaption,SystemColors.ActiveCaptionText);
        }
			}
			g.FillRectangle(backBrush,rect);
			if (isReadOnly)
				g.DrawRectangle(SystemPens.ControlDark,rect);
      //if (ReadOnly)
      //  g.DrawRectangle(new Pen(foreBrush),bounds);
      rect.Height += 2;
			rect.Offset(2,1);
      g.DrawString(text, this.DataGridTableStyle.DataGrid.Font, foreBrush, rect,StringFormat.GenericTypographic);
    }

    #endregion

		#region protected SetDataGridInColumn

    protected override void SetDataGridInColumn(DataGrid value) //override
    {
      base.SetDataGridInColumn(value);
      if (pControl != null && pControl.Parent != null) 
      {
        pControl.Parent.Controls.Remove (pControl);
      }
      if (value != null && pControl != null) 
      {
        value.Controls.Add(pControl);
      }
    }

		#endregion --protected ColumnStartedEditing-SetDataGridInColumn

		#region local events

    /// <summary>
    /// Для успешной работы класса ... (зачем ???)
    /// </summary>
    protected void ControlChanged(object sender, EventArgs e) 
    {
      isEditing = true;
      _ControlChanged();
    }

		#endregion --local events

    #region Вспомогательные ф-ции

		/// <summary>
		/// Генерим наружу событие !!! (если надо и мона) !
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void _ControlChanged()
		{
			if (ReadOnly || IsLock)
				return;

			if (onControlChanged!=null)
				onControlChanged(this,EventArgs.Empty);
		}

		/// <summary>
		/// Возвращаем тип поля прикрепленному к этому столбцу
		/// </summary>
		protected Type getTypeFieldMapping
		{
			get
			{
				return getDataTable.Columns[this.MappingName].DataType;
			}
		}

		/// <summary>
		/// Возвращает индекс в коллекции DataGridColumnStyle
		/// </summary>
		public int Index
		{
			get
			{
				return this.DataGridTableStyle.GridColumnStyles.IndexOf(this);
			}
		}


    protected CurrencyManager getCurrencyManager
    {
      get
      {
        CurrencyManager dCurrency =null;
        DataGrid dg=this.DataGridTableStyle.DataGrid;
        try
        {
          dCurrency = (CurrencyManager)dg.BindingContext[dg.DataSource, dg.DataMember];
        }
        catch{}
        return dCurrency;
      }
    }

    protected DataTable getDataTable
    {
      get
      {
        DataGrid dg=this.DataGridTableStyle.DataGrid;
        DataTable dTable =null;
        try
        {
          DataView dataView = (DataView)getCurrencyManager.List;
          if (dataView!=null)
            dTable=dataView.Table;
        }
        catch{}
        return dTable;
      }
    }

    protected DataRow getCurrentRow
    {
      get
      {
        DataRow dRow = null;
        try
        {
          CurrencyManager cm = getCurrencyManager;
          DataRowView drv= (DataRowView) cm.Current;
          dRow = drv.Row;
        }
        catch {}
        return dRow;
      }
    }


		#endregion
	}
}
