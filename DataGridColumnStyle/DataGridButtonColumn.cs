using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using CommandAS.Tools.Controls;

//по мотивам от --- (оригинал на VB)
//Автор кода Конунов Дмитрий Владимирович
//e-mail:cktro@hotmail.com,Самара ТеррНИИГражданпроект www.stri.ru

namespace CommandAS.Tools.DataGridColumnStyle
{
  public delegate void EventHandlerGridButtonClick(object sender, int row, int col);
  /// <summary>
	/// DataGridButtonColumn - колонка с кнопкой.
	/// </summary>
  public class DataGridColumnButton : DataGridTextBoxColumn
  {
		#region Public Events

		public event EventHandlerGridButtonClick DataGridButtonClick ;
		public event EventHandler OnControlChanged;
 
		#endregion --Public Events
			
		#region Local vars

    private Bitmap _buttonFace;
    private Bitmap _buttonFacePressed;
		private int _pressedRow = -1;

		protected bool  mIsLock=false;
    protected bool								setEvents	=true; 
    protected bool			setEventsChangeGrid	=true;
    protected Rectangle							saveFrm	=Rectangle.Empty;
    protected Form											frm;
    protected Label									lblHead;
    protected bool							mIsTVVisible=false;
		protected CurrencyManager						mCm	=	null;
		protected CurrencyManager						pCm
		{
			set
			{
				if (value==null)
					return;
				if (mCm==null)
				{
					mCm=value;
					mCm.ItemChanged+=new ItemChangedEventHandler(on_ItemChanged);
					//mCm.CurrentChanged+=new EventHandler(mCm_CurrentChanged);
				}
			}
		}

		private System.ComponentModel.Container components = null;

		#endregion --Local vars

		#region Public Property

		public string HeaderLabel
		{
			set
			{
				if (lblHead!=null)
				{
					lblHead.Text=value;
					lblHead.Refresh();
				}
			}
			get
			{
				string ret=string.Empty;
				if (lblHead!=null)
					ret=lblHead.Text;
				return ret;
			}
		}

    /// <summary>
    /// Возвращаем ссылку на выпадающую форму
    /// </summary>
    public Form ChildForm
    {
      get
      {
        if (frm==null)
          FormInit();
        return frm;
      }
    }

		
		/// <summary>
		/// Возвращаем коллекцию записей в Grid
		/// </summary>
		/// <returns></returns>
		public DataRow GetCurrentRow
		{
			get
			{
				DataRow dRow = null;
				DataGrid dg=this.DataGridTableStyle.DataGrid;
				try
				{
					CurrencyManager cm = (CurrencyManager) dg.BindingContext[dg.DataSource];
					DataRowView drv= (DataRowView) cm.Current;
					dRow = drv.Row;
				}
				catch {}
				return dRow;
			}
		}

		#endregion --Public Property

		#region Constructor

		public DataGridColumnButton()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

      _buttonFace         = new Bitmap(typeof(DataGridColumnButton),"Images.ButtonFace.bmp");
      _buttonFacePressed  = new Bitmap(typeof(DataGridColumnButton),"Images.ButtonPressed.bmp");

      //this.TextBox.ParentChanged+=  new EventHandler(onChangeParent);
      this.TextBox.KeyDown      +=  new KeyEventHandler(FormKeyDown);
      this.TextBox.TextChanged  +=  new EventHandler(onChangeText);
      this.TextBox.LostFocus    +=  new EventHandler(onLostFocus);
      this.DataGridButtonClick  +=  new EventHandlerGridButtonClick(onClick);
      this.TextBox.BorderStyle  = BorderStyle.None;
		}

		#endregion --Constructor

    #region Графическая поддержка 

    protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Brush backBrush, System.Drawing.Brush foreBrush, bool alignToRight)
    {
      Rectangle Rect = Rectangle.Empty;
      Bitmap bm = _buttonFace;
      Rect = bounds;
      Rect.Width -= bm.Width;
      if (this.ReadOnly)
      {
        //backBrush=new SolidBrush(SystemColors.InactiveBorder);
        foreBrush=new SolidBrush(SystemColors.InactiveCaptionText);
      }
      g.FillRectangle(backBrush, bounds);

      string s= this.GetColumnValueAtRow(source, rowNum).ToString();
      
      if (s!=null)
        g.DrawString(s, DataGridTableStyle.DataGrid.Font, foreBrush, bounds.X, bounds.Y);
      /*
       * позднее надо будет самому нарисовать кнопку !!!
       * 
      Rect=new Rectangle(bounds.Right,bounds.Y,bm.Width,bounds.Height);
      g.FillRectangle(new SolidBrush(Color.Red),Rect);
      Font fnt=new Font("Marlett",9,FontStyle.Bold);
      if( _pressedRow == rowNum)
      {
        Rect.Offset(1,1);
        //Rect.Height-=1;
        //Rect.Width-=1;
      }
      g.DrawString("6",fnt,foreBrush,Rect);
      */      
      if( _pressedRow == rowNum)
        bm = _buttonFacePressed ;
      g.DrawImage(bm, bounds.Right - bm.Width, bounds.Y);
    }

    #endregion
		
		#region обработка по событиям

    private void onChangeText(object sender,EventArgs e)
    {
      if (mIsLock)
        return;
      if (this.TextBox.Visible)
        this.TextBox.Tag=this.TextBox.Text;
      else
        this.TextBox.Tag=null;
			Change();
    }

		private void Change()
		{
			if (OnControlChanged!=null && !this.DataGridTableStyle.DataGrid.ReadOnly)
				OnControlChanged(this,EventArgs.Empty);
		}
		/// <summary>
		/// Перезагрузка записи в Grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void onChangeSource(object sender,EventArgs e)
		{
			this.TextBox.Tag=null;  //если правка - загрузкой данных !!!
			ConcedeFocus();
		}
		private void onLostFocus(object sender,EventArgs e)
		{
			ConcedeFocus();
		}

		protected void onClick(object sender,int row,int col)
		{
			FormInit();
			FormShow();
		}

		private void on_ItemChanged(object sender, ItemChangedEventArgs e)
		{
			DataRowCollection rows=Rows;
			if (e.Index==-1 || rows==null || e.Index >=rows.Count)
				return;
			DataRow drow=rows[e.Index];
			if (drow.RowState!=DataRowState.Unchanged)
			{
				//if (drow.
				Change();
			}
		}

		#endregion --обработка по событиям

    #region Поддержка protected операций

		protected override void SetDataGridInColumn(DataGrid value) 
		{
			base.SetDataGridInColumn(value);
   
			if (value!=null && setEvents)
			{
				DataGrid dg   =value;
				dg.MouseDown += new MouseEventHandler(onMouseDown);
				dg.MouseUp   += new MouseEventHandler(onMouseUp);
				dg.MouseMove += new MouseEventHandler(onMouseMove);
				dg.MouseHover+= new EventHandler(onMouseHover);
				dg.KeyDown  += new KeyEventHandler(FormKeyDown);
				dg.DataSourceChanged+=new EventHandler(onChangeSource);
				setEvents=false;
			}
		}
		
		protected override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
		{
			pCm=source;
			DataRowView drv= (DataRowView) source.Current;
			if (drv!=null)
				drv.BeginEdit();
			Rectangle Rect = bounds;
			Rect.Width -= _buttonFace.Width;

			base.Edit(source, rowNum, Rect, readOnly, instantText, cellIsVisible);
			TextBox.Select(0,0);
		}

    protected override bool Commit(CurrencyManager source, int rowNum)
    {
      mIsLock=true;
      try
      {
        DataRowView drv= (DataRowView) source.Current;
        string bases=(string)drv.Row[this.MappingName];
        SetColumnValueAtRow(source, rowNum, bases);

        drv.EndEdit();
        ConcedeFocus();
      }
      catch
      {
        Abort(rowNum);
        mIsLock=false;
        return false;
      }

      return true;
    }

    protected override void Abort(int rowNum)
    {
      Invalidate();
      this.TextBox.Tag=null;
    }

    /// <summary>
    /// При потери фокуса контролом - гасим TextBox
    /// </summary>
    protected override void ConcedeFocus()
    {
      this.TextBox.Visible=false;
      //base.ConcedeFocus();
      mIsLock=false;
    }
    #endregion

		#region Поддержка мыши
    protected void onMouseHover(object sender, EventArgs e)
    {
      CommandAS.Tools.MouseCursor.SetCursorHand();
    }
    protected void onMouseDown(object sender, MouseEventArgs e)
    { 

      DataGrid dg = DataGridTableStyle.DataGrid;
      DataGrid.HitTestInfo hti = dg.HitTest(new Point(e.X, e.Y));
      Rectangle rect = Rectangle.Empty;
      int clickColumn = this.DataGridTableStyle.GridColumnStyles.IndexOf( this );

      if ( hti.Column != clickColumn || hti.Row == -1 )
        return;

      bool isClickInCell = true;
      rect = dg.GetCellBounds(hti.Row, hti.Column);
      isClickInCell = e.X > (rect.Right - _buttonFace.Width);
      if (rect.Left < 0)
        isClickInCell = false;

      if (isClickInCell)
      {
        CommandAS.Tools.MouseCursor.SetCursorHand();
        Graphics g = Graphics.FromHwnd(dg.Handle);
        g.DrawImage(_buttonFacePressed, rect.Right - _buttonFacePressed.Width, rect.Y);
        rect.X+=1;
        rect.Width-=1;
        rect.Y-=1;
        rect.Height-=1;
        //g.DrawRectangle(new Pen(new SolidBrush(Color.Green)) ,rect);
        _pressedRow = hti.Row;
        g.Dispose();
      }
    }
    protected void onMouseMove(object sender, MouseEventArgs e)
    {
      DataGrid dg= DataGridTableStyle.DataGrid;
      DataGrid.HitTestInfo hti = dg.HitTest(new Point(e.X, e.Y));
      int clickColumn = this.DataGridTableStyle.GridColumnStyles.IndexOf( this );

      bool isClickInCell = (hti.Column == clickColumn && hti.Row > -1 && hti.Row==_pressedRow);
      //отжимаем
      if (!isClickInCell && e.Button==MouseButtons.Left)
      {
        onMouseUp(sender,e);
      }
    }
    protected void onMouseUp(object sender, MouseEventArgs e)
    {
      DataGrid dg= DataGridTableStyle.DataGrid;
      DataGrid.HitTestInfo hti = dg.HitTest(new Point(e.X, e.Y));
      int clickColumn = this.DataGridTableStyle.GridColumnStyles.IndexOf( this );

      if (hti.Column>-1 && hti.Row>-1)
      {
        bool isClickInCell = (hti.Column == clickColumn && hti.Row==_pressedRow);
        Rectangle rect = Rectangle.Empty;
        if (isClickInCell)
        {
          rect = dg.GetCellBounds(hti.Row, hti.Column);
          isClickInCell = e.X >= (rect.Right - _buttonFace.Width);
        }
        Graphics g = Graphics.FromHwnd(dg.Handle);
        Bitmap bm;
        if (isClickInCell)
        {
          bm= _buttonFace;
          if (DataGridButtonClick!=null)
            DataGridButtonClick(this, _pressedRow, clickColumn);
        }
        else
        {
          bm=_buttonFacePressed;
        }
        g.DrawImage(bm, rect.Right - bm.Width, rect.Y);
        g.Dispose();
        _pressedRow = -1;
      }
      CommandAS.Tools.MouseCursor.SetCursorDefault();
    }

    #endregion

    #region Работаем с формой - выводом

    /// <summary>
    /// Создаем новое окно, если оно еще не создано!
    /// </summary>
    private void FormInit()
    {
      if (frm==null)
      {
        frm=new System.Windows.Forms.Form();
        Graphics g=frm.CreateGraphics();
        frm.Font=this.DataGridTableStyle.DataGrid.Font;
        lblHead=new Label();
        lblHead.TextAlign=ContentAlignment.MiddleCenter;
        lblHead.Parent=frm;
        HeaderLabel="Справочник объектов";
        lblHead.ForeColor=SystemColors.ActiveCaptionText;
        lblHead.BackColor=SystemColors.ActiveCaption;
        lblHead.Font=new Font(frm.Font.Name,frm.Font.Size,FontStyle.Bold);
        lblHead.Height=g.MeasureString(lblHead.Text,lblHead.Font).ToSize().Height*3/2;
        lblHead.Dock=DockStyle.Top;

        frm.StartPosition=FormStartPosition.Manual;
        frm.FormBorderStyle= FormBorderStyle.SizableToolWindow; //SizableToolWindow
        frm.ControlBox=false;
        if (saveFrm.IsEmpty)
          frm.Size=new Size(this.Width,350);
        else
          frm.Bounds=saveFrm;

        frm.ShowInTaskbar=false;
        frm.KeyPreview=true;

        //tv.LostFocus				+= new EventHandler (OnLostFocus);
        //tv.AfterSelect			+= new TreeViewEventHandler(OnTreeItemSelected);
        //tv.OnDoCommand			+= new EvH_CasTVCommand(OnTreeItemDoCommand);

        frm.ControlAdded+= new ControlEventHandler(onControlAdd);
        frm.KeyDown   +=new KeyEventHandler(FormKeyDown);
        //оптимизируем и не закрываем окно !!!
        frm.Closing+=new CancelEventHandler(onFormClosing);
        frm.Disposed+=new EventHandler(onFormDisposed);
        frm.Deactivate+=new EventHandler(onFormHide);
        g.Dispose();
      }
    }
    private void onControlAdd(object sender,ControlEventArgs e)
    {
//      frm.SuspendLayout();
      frm.Controls.AddRange(new Control[]{e.Control,lblHead});
      e.Control.Dock=DockStyle.Fill;
			e.Control.Visible=true;
      /*
      Control[] controls=new Control[]{};
      int y=0;
      for(int i=frm.Controls.Count-1;i>-1;i--)
      {
        frm.Controls[i].con;
      }
      if (controls.Length>0)
        frm.Controls.AddRange(controls);
*/        
//      frm.ResumeLayout(false);
    }
    /// <summary>
    /// Обрабатываем спец. клавиши
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FormKeyDown(object sender,KeyEventArgs e)
    {
      TextBox tx=sender as TextBox;
      if (tx!=null)
      {
        //по F4 вызываем дерево !!!
        if (e.KeyCode==Keys.F4)
        {
          DataGrid dg=this.DataGridTableStyle.DataGrid;
          DataGrid.HitTestInfo hti = dg.HitTest(dg.GetCurrentCellBounds().Location);
          //по нажатию генерим такое-же событие !!!
          if (DataGridButtonClick!=null)
            DataGridButtonClick(this,hti.Row,hti.Column);
        }
        return;
      }
      //если esc - выходим!
      if (e.KeyCode == Keys.Escape)
      {
        FormHide();
      }
    }
    /// <summary>
    /// Позиционируем окно в координатах
    /// </summary>
    private void FormShow()
    {
      if (frm!=null)
      {
        DataGrid dg=this.DataGridTableStyle.DataGrid;
        //узнаем координаты в экране txt
        Rectangle rect=dg.GetCurrentCellBounds();
        rect=dg.RectangleToScreen(rect);
        frm.Bounds= CASTools.GetBoundsControl(rect,frm);
        if (saveFrm.IsEmpty) //первоначально
        {
          frm.Width=this.Width;
        }
        else
        {
          frm.Height=saveFrm.Height;
          frm.Width=saveFrm.Width;
        }
        /*
        if (tv.Parent==null)
        {
          tv.Parent=frm;
          tv.Dock=DockStyle.Fill;
        }
        tv.Visible = true;
        tv.Focus();
*/        
        frm.BringToFront();
        frm.Visible=true;
      }
    }
    //оптимизируем и не закрываем окно !!!
    private void onFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      e.Cancel=true;
      FormHide();
    }
    private void onFormDisposed(object sender, EventArgs e)
    {
      if (frm.Parent!=null)
        frm.Parent=null;
    }
    /// <summary>
    /// Программно убираем окно и выполняем отмену
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void onFormHide(object sender,EventArgs e)
    {
      FormHide();
    }
    private void FormHide()
    {
      if (frm!=null)
      {
        saveFrm = frm.Bounds;
        frm.Visible=false;
        MouseEventArgs mouse =new MouseEventArgs(Form.MouseButtons ,0,Form.MousePosition.X,Form.MousePosition.Y,0);
        //отрабатываем для отрисовки отжатия клавиши (или нажатия в другом !)
        onMouseUp(this, mouse);
      }
      mIsTVVisible=false;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		#endregion --Staff .Net

		#region local property

		private CurrencyManager CurrMan
		{
			get
			{				
				return mCm;
			}
		}

		private DataRowCollection Rows
		{
			get
			{
				DataRowCollection ret=null;
				CurrencyManager cm=CurrMan;
				if (cm==null)
					return ret;
				DataView dv = (DataView)cm.List;
				if (dv!=null)
					ret=dv.Table.Rows;
				return ret;
			}
		}


		#endregion --local property
	}
}



