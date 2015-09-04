using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CommandAS.Tools;
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.Forms
{
	/// <summary>
	/// Summary description for dlgImage.
	/// </summary>
	public class dlgImage : System.Windows.Forms.Form
	{
		protected ucImageCollection						mUCIC;

		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ToolBar _tb;
		private System.Windows.Forms.ToolBarButton _tbbFitReal;
		private System.Windows.Forms.ToolBarButton _tbbRotateL;
		private System.Windows.Forms.ToolBarButton _tbbRotateR;
		private System.Windows.Forms.ToolBarButton _tbbZoomPlus;
		private System.Windows.Forms.ToolBarButton _tbbZoomMinus;
		private System.Windows.Forms.StatusBar _sb;
		private System.Windows.Forms.Button _cmdCancel;
		private System.Windows.Forms.MainMenu _mnuMain;
		private System.Windows.Forms.MenuItem _mnuFile;
		private System.Windows.Forms.MenuItem _mnuLoad;
		private System.Windows.Forms.MenuItem _mnuSaveAs;
		private System.Windows.Forms.MenuItem _mnuClose;
		private System.Windows.Forms.MenuItem _mnuSep1;
		private System.Windows.Forms.MenuItem _mnuAcquire;
		private System.Windows.Forms.MenuItem _mnuSep2;
		private System.Windows.Forms.MenuItem _mnuImage;
		private System.Windows.Forms.MenuItem _mnuSizeReal;
		private System.Windows.Forms.MenuItem _mnuRotateL;
		private System.Windows.Forms.MenuItem _mnuRotateR;
		private System.Windows.Forms.MenuItem _mnuSizePlus;
		private System.Windows.Forms.MenuItem _mnuSizeMinus;
		private System.Windows.Forms.MenuItem _mnuSep5;
		private System.Windows.Forms.MenuItem _mnuSep4;
		private System.Windows.Forms.MenuItem _mnuCopy;
		private System.Windows.Forms.MenuItem _mnuPaste;
		private System.Windows.Forms.MenuItem _mnuClear;
		private System.Windows.Forms.MenuItem _mnuSep3;
		private System.Windows.Forms.ToolBarButton _tbbSep1;
		private System.Windows.Forms.ToolBarButton _tbbLoad;
		private System.Windows.Forms.ToolBarButton _tbbCopy;
		private System.Windows.Forms.ToolBarButton _tbbPaste;
		private System.Windows.Forms.ToolBarButton _tbbSep2;
		private System.Windows.Forms.ToolBarButton _tbbSep3;
		private System.Windows.Forms.MenuItem _mnuSelectScan;
		private System.Windows.Forms.MenuItem _mnuAdd;
		private System.Windows.Forms.MenuItem _mnuDel;
		private System.Windows.Forms.MenuItem _mnuForward;
		private System.Windows.Forms.MenuItem _mnuBackward;
		private System.Windows.Forms.MenuItem _mnuSep6;
		private System.Windows.Forms.ToolBarButton _tbbAdd;
		private System.Windows.Forms.ToolBarButton _tbbDel;
		private System.Windows.Forms.ToolBarButton _tbbSep0;
		private System.Windows.Forms.ToolBarButton _tbbSep4;
		private System.Windows.Forms.ToolBarButton _tbbForward;
		private System.Windows.Forms.ToolBarButton _tbbBackward;
    private MenuItem _mnuSizeCorrect;
		private IconCollection mCol;

		public bool														pIsImageFit
		{
			set
			{ 
				_tbbFitReal.Pushed = value;
			}
		}

		public Image													pImage
		{
			get { return mUCIC.pImage;  }
			set {	mUCIC.pImage = value; }
		}

    public String                         pText
    {
      get { return mUCIC.pText; }
      set { mUCIC.pText = value; }
    }

		public StateCollection								pImageCollection
		{
			get { return mUCIC.pImageCollection; }
			set
			{
				mUCIC.pImageCollection = value;
				mUCIC.pIsImageCollection = true;
			}
		}

		public bool														pIsModified
		{
			get { return mUCIC.pIsModified; }
		}

    public int pMaxImageSize
    {
      get { return mUCIC.pMaxImageSize;  }
      set { mUCIC.pMaxImageSize = value; }
    }

		public dlgImage():this(null){}

		public dlgImage(IconCollection iCol)
		{
			mCol=iCol;

			mUCIC = new ucImageCollection();
			mUCIC.pIsImageCollection = false;
			mUCIC.Dock = DockStyle.Fill;
			Controls.Add(mUCIC);

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			_cmdCancel.Width = 0;
			_cmdCancel.Height = 0;
			_cmdCancel.SendToBack();

			if (mCol!=null)
			{
				Icon = mCol.Icon(IconCollection.Foto);

				_tb.ImageList = mCol.pImageList;
				_tbbAdd.ImageIndex = mCol.Index(IconCollection.Additive);
				_tbbDel.ImageIndex = mCol.Index(IconCollection.Minus);
				_tbbLoad.ImageIndex = mCol.Index(IconCollection.FolderOpen);
				_tbbCopy.ImageIndex = mCol.Index(IconCollection.Copy);
				_tbbPaste.ImageIndex = mCol.Index(IconCollection.Paste);
				_tbbRotateL.ImageIndex = mCol.Index(IconCollection.ImageRotate_Left);
				_tbbRotateR.ImageIndex = mCol.Index(IconCollection.ImageRotate_Right);
				_tbbFitReal.ImageIndex = mCol.Index(IconCollection.ImageFillReal);
				_tbbZoomPlus.ImageIndex = mCol.Index(IconCollection.Zoom_Plus);
				_tbbZoomMinus.ImageIndex = mCol.Index(IconCollection.Zoom_Minus);
				_tbbForward.ImageIndex = mCol.Index(IconCollection.ArrowForward);
				_tbbBackward.ImageIndex = mCol.Index(IconCollection.ArrowBackward);

				_tbbAdd.Tag = _mnuAdd;
				_tbbDel.Tag = _mnuDel;
				_tbbLoad.Tag = _mnuLoad;
				_tbbCopy.Tag = _mnuCopy;
				_tbbPaste.Tag = _mnuPaste;
				_tbbRotateL.Tag = _mnuRotateL;
				_tbbRotateR.Tag = _mnuRotateR;
				_tbbFitReal.Tag = _mnuSizeReal;
				_tbbZoomPlus.Tag = _mnuSizePlus;
				_tbbZoomMinus.Tag = _mnuSizeMinus;
				_tbbForward.Tag = _mnuForward;
				_tbbBackward.Tag = _mnuBackward;
			}
			else 
				_tb.Visible = false;

			Closed += new EventHandler(dlgImage_Closed);
			ResizeRedraw = true;

			mUCIC.CollectionCurrentItemChange += new EventHandler(mUCIC_CollectionCurrentItemChange);
      mUCIC.ImageInformationChange += new EventHandler(mUCIC_ImageInformationChange);
		}

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
      this.components = new System.ComponentModel.Container();
      this._tb = new System.Windows.Forms.ToolBar();
      this._tbbAdd = new System.Windows.Forms.ToolBarButton();
      this._tbbDel = new System.Windows.Forms.ToolBarButton();
      this._tbbSep0 = new System.Windows.Forms.ToolBarButton();
      this._tbbLoad = new System.Windows.Forms.ToolBarButton();
      this._tbbSep1 = new System.Windows.Forms.ToolBarButton();
      this._tbbCopy = new System.Windows.Forms.ToolBarButton();
      this._tbbPaste = new System.Windows.Forms.ToolBarButton();
      this._tbbSep2 = new System.Windows.Forms.ToolBarButton();
      this._tbbRotateL = new System.Windows.Forms.ToolBarButton();
      this._tbbRotateR = new System.Windows.Forms.ToolBarButton();
      this._tbbSep3 = new System.Windows.Forms.ToolBarButton();
      this._tbbFitReal = new System.Windows.Forms.ToolBarButton();
      this._tbbZoomPlus = new System.Windows.Forms.ToolBarButton();
      this._tbbZoomMinus = new System.Windows.Forms.ToolBarButton();
      this._tbbSep4 = new System.Windows.Forms.ToolBarButton();
      this._tbbForward = new System.Windows.Forms.ToolBarButton();
      this._tbbBackward = new System.Windows.Forms.ToolBarButton();
      this._sb = new System.Windows.Forms.StatusBar();
      this._cmdCancel = new System.Windows.Forms.Button();
      this._mnuMain = new System.Windows.Forms.MainMenu(this.components);
      this._mnuFile = new System.Windows.Forms.MenuItem();
      this._mnuLoad = new System.Windows.Forms.MenuItem();
      this._mnuSaveAs = new System.Windows.Forms.MenuItem();
      this._mnuSep1 = new System.Windows.Forms.MenuItem();
      this._mnuSelectScan = new System.Windows.Forms.MenuItem();
      this._mnuAcquire = new System.Windows.Forms.MenuItem();
      this._mnuSep2 = new System.Windows.Forms.MenuItem();
      this._mnuClose = new System.Windows.Forms.MenuItem();
      this._mnuImage = new System.Windows.Forms.MenuItem();
      this._mnuAdd = new System.Windows.Forms.MenuItem();
      this._mnuDel = new System.Windows.Forms.MenuItem();
      this._mnuClear = new System.Windows.Forms.MenuItem();
      this._mnuSep3 = new System.Windows.Forms.MenuItem();
      this._mnuCopy = new System.Windows.Forms.MenuItem();
      this._mnuPaste = new System.Windows.Forms.MenuItem();
      this._mnuSep4 = new System.Windows.Forms.MenuItem();
      this._mnuRotateL = new System.Windows.Forms.MenuItem();
      this._mnuRotateR = new System.Windows.Forms.MenuItem();
      this._mnuSep5 = new System.Windows.Forms.MenuItem();
      this._mnuSizeReal = new System.Windows.Forms.MenuItem();
      this._mnuSizePlus = new System.Windows.Forms.MenuItem();
      this._mnuSizeMinus = new System.Windows.Forms.MenuItem();
      this._mnuSizeCorrect = new System.Windows.Forms.MenuItem();
      this._mnuSep6 = new System.Windows.Forms.MenuItem();
      this._mnuForward = new System.Windows.Forms.MenuItem();
      this._mnuBackward = new System.Windows.Forms.MenuItem();
      this.SuspendLayout();
      // 
      // _tb
      // 
      this._tb.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this._tbbAdd,
            this._tbbDel,
            this._tbbSep0,
            this._tbbLoad,
            this._tbbSep1,
            this._tbbCopy,
            this._tbbPaste,
            this._tbbSep2,
            this._tbbRotateL,
            this._tbbRotateR,
            this._tbbSep3,
            this._tbbFitReal,
            this._tbbZoomPlus,
            this._tbbZoomMinus,
            this._tbbSep4,
            this._tbbForward,
            this._tbbBackward});
      this._tb.DropDownArrows = true;
      this._tb.Location = new System.Drawing.Point(0, 0);
      this._tb.Name = "_tb";
      this._tb.ShowToolTips = true;
      this._tb.Size = new System.Drawing.Size(623, 28);
      this._tb.TabIndex = 0;
      this._tb.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.OnButtonClick);
      // 
      // _tbbAdd
      // 
      this._tbbAdd.Name = "_tbbAdd";
      this._tbbAdd.ToolTipText = "Добавить новое изображение в коллекцию";
      // 
      // _tbbDel
      // 
      this._tbbDel.Name = "_tbbDel";
      this._tbbDel.ToolTipText = "Удалить изображение из коллекции";
      // 
      // _tbbSep0
      // 
      this._tbbSep0.Name = "_tbbSep0";
      this._tbbSep0.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      // 
      // _tbbLoad
      // 
      this._tbbLoad.Name = "_tbbLoad";
      this._tbbLoad.ToolTipText = "Загрузить из файла";
      // 
      // _tbbSep1
      // 
      this._tbbSep1.Name = "_tbbSep1";
      this._tbbSep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      // 
      // _tbbCopy
      // 
      this._tbbCopy.Name = "_tbbCopy";
      this._tbbCopy.ToolTipText = "Копировать в буфер";
      // 
      // _tbbPaste
      // 
      this._tbbPaste.Name = "_tbbPaste";
      this._tbbPaste.ToolTipText = "Копировать из буфера";
      // 
      // _tbbSep2
      // 
      this._tbbSep2.Name = "_tbbSep2";
      this._tbbSep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      // 
      // _tbbRotateL
      // 
      this._tbbRotateL.Name = "_tbbRotateL";
      this._tbbRotateL.Tag = "cmdRotateL";
      this._tbbRotateL.ToolTipText = "Поворот против часовой стрелке";
      // 
      // _tbbRotateR
      // 
      this._tbbRotateR.Name = "_tbbRotateR";
      this._tbbRotateR.Tag = "cmdRotateR";
      this._tbbRotateR.ToolTipText = "Поворот по часовой стрелке";
      // 
      // _tbbSep3
      // 
      this._tbbSep3.Name = "_tbbSep3";
      this._tbbSep3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      // 
      // _tbbFitReal
      // 
      this._tbbFitReal.ImageIndex = 0;
      this._tbbFitReal.Name = "_tbbFitReal";
      this._tbbFitReal.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
      this._tbbFitReal.Tag = "cmdFitReal";
      // 
      // _tbbZoomPlus
      // 
      this._tbbZoomPlus.Name = "_tbbZoomPlus";
      this._tbbZoomPlus.ToolTipText = "Увеличить размер";
      // 
      // _tbbZoomMinus
      // 
      this._tbbZoomMinus.Name = "_tbbZoomMinus";
      this._tbbZoomMinus.ToolTipText = "Уменьшить размер";
      // 
      // _tbbSep4
      // 
      this._tbbSep4.Name = "_tbbSep4";
      this._tbbSep4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
      // 
      // _tbbForward
      // 
      this._tbbForward.Name = "_tbbForward";
      this._tbbForward.ToolTipText = "Переместить вперед";
      // 
      // _tbbBackward
      // 
      this._tbbBackward.Name = "_tbbBackward";
      this._tbbBackward.ToolTipText = "Переместить назад";
      // 
      // _sb
      // 
      this._sb.Location = new System.Drawing.Point(0, 396);
      this._sb.Name = "_sb";
      this._sb.Size = new System.Drawing.Size(623, 19);
      this._sb.TabIndex = 1;
      // 
      // _cmdCancel
      // 
      this._cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this._cmdCancel.Location = new System.Drawing.Point(440, 291);
      this._cmdCancel.Name = "_cmdCancel";
      this._cmdCancel.Size = new System.Drawing.Size(60, 27);
      this._cmdCancel.TabIndex = 2;
      this._cmdCancel.Click += new System.EventHandler(this.DoCommandCancel);
      // 
      // _mnuMain
      // 
      this._mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._mnuFile,
            this._mnuImage});
      // 
      // _mnuFile
      // 
      this._mnuFile.Index = 0;
      this._mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._mnuLoad,
            this._mnuSaveAs,
            this._mnuSep1,
            this._mnuSelectScan,
            this._mnuAcquire,
            this._mnuSep2,
            this._mnuClose});
      this._mnuFile.Text = "Файл";
      // 
      // _mnuLoad
      // 
      this._mnuLoad.Index = 0;
      this._mnuLoad.Text = "Загрузить из файла";
      this._mnuLoad.Click += new System.EventHandler(this._mnuLoad_Click);
      // 
      // _mnuSaveAs
      // 
      this._mnuSaveAs.Index = 1;
      this._mnuSaveAs.Text = "Сохранить в файл";
      this._mnuSaveAs.Click += new System.EventHandler(this._mnuSaveAs_Click);
      // 
      // _mnuSep1
      // 
      this._mnuSep1.Index = 2;
      this._mnuSep1.Text = "-";
      // 
      // _mnuSelectScan
      // 
      this._mnuSelectScan.Index = 3;
      this._mnuSelectScan.Text = "Выбрать источник";
      this._mnuSelectScan.Click += new System.EventHandler(this._mnuSelectScan_Click);
      // 
      // _mnuAcquire
      // 
      this._mnuAcquire.Index = 4;
      this._mnuAcquire.Text = "Сканировать";
      this._mnuAcquire.Click += new System.EventHandler(this._mnuAcquire_Click);
      // 
      // _mnuSep2
      // 
      this._mnuSep2.Index = 5;
      this._mnuSep2.Text = "-";
      // 
      // _mnuClose
      // 
      this._mnuClose.Index = 6;
      this._mnuClose.Text = "Выход";
      this._mnuClose.Click += new System.EventHandler(this._mnuClose_Click);
      // 
      // _mnuImage
      // 
      this._mnuImage.Index = 1;
      this._mnuImage.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this._mnuAdd,
            this._mnuDel,
            this._mnuClear,
            this._mnuSep3,
            this._mnuCopy,
            this._mnuPaste,
            this._mnuSep4,
            this._mnuRotateL,
            this._mnuRotateR,
            this._mnuSep5,
            this._mnuSizeReal,
            this._mnuSizePlus,
            this._mnuSizeMinus,
            this._mnuSizeCorrect,
            this._mnuSep6,
            this._mnuForward,
            this._mnuBackward});
      this._mnuImage.Text = "Изображение";
      // 
      // _mnuAdd
      // 
      this._mnuAdd.Index = 0;
      this._mnuAdd.Text = "Добавить";
      this._mnuAdd.Click += new System.EventHandler(this._mnuAdd_Click);
      // 
      // _mnuDel
      // 
      this._mnuDel.Index = 1;
      this._mnuDel.Text = "Удалить";
      this._mnuDel.Click += new System.EventHandler(this._mnuDel_Click);
      // 
      // _mnuClear
      // 
      this._mnuClear.Index = 2;
      this._mnuClear.Text = "Очистить";
      this._mnuClear.Click += new System.EventHandler(this._mnuClear_Click);
      // 
      // _mnuSep3
      // 
      this._mnuSep3.Index = 3;
      this._mnuSep3.Text = "-";
      // 
      // _mnuCopy
      // 
      this._mnuCopy.Index = 4;
      this._mnuCopy.Text = "Копировать в буфер";
      this._mnuCopy.Click += new System.EventHandler(this._mnuCopy_Click);
      // 
      // _mnuPaste
      // 
      this._mnuPaste.Index = 5;
      this._mnuPaste.Text = "Копировать из буфера";
      this._mnuPaste.Click += new System.EventHandler(this._mnuPaste_Click);
      // 
      // _mnuSep4
      // 
      this._mnuSep4.Index = 6;
      this._mnuSep4.Text = "-";
      // 
      // _mnuRotateL
      // 
      this._mnuRotateL.Index = 7;
      this._mnuRotateL.Text = "Поворот - лево";
      this._mnuRotateL.Click += new System.EventHandler(this._mnuRotateL_Click);
      // 
      // _mnuRotateR
      // 
      this._mnuRotateR.Index = 8;
      this._mnuRotateR.Text = "Поворот - право";
      this._mnuRotateR.Click += new System.EventHandler(this._mnuRotateR_Click);
      // 
      // _mnuSep5
      // 
      this._mnuSep5.Index = 9;
      this._mnuSep5.Text = "-";
      // 
      // _mnuSizeReal
      // 
      this._mnuSizeReal.Index = 10;
      this._mnuSizeReal.Text = "Реальный размер";
      this._mnuSizeReal.Click += new System.EventHandler(this._mnuSizeReal_Click);
      // 
      // _mnuSizePlus
      // 
      this._mnuSizePlus.Index = 11;
      this._mnuSizePlus.Text = "Увеличить";
      this._mnuSizePlus.Click += new System.EventHandler(this._mnuSizePlus_Click);
      // 
      // _mnuSizeMinus
      // 
      this._mnuSizeMinus.Index = 12;
      this._mnuSizeMinus.Text = "Уменьшить";
      this._mnuSizeMinus.Click += new System.EventHandler(this._mnuSizeMinus_Click);
      // 
      // _mnuSizeCorrect
      // 
      this._mnuSizeCorrect.Index = 13;
      this._mnuSizeCorrect.Text = "Корректировать размер";
      this._mnuSizeCorrect.Click += new System.EventHandler(this._mnuSizeCorrect_Click);
      // 
      // _mnuSep6
      // 
      this._mnuSep6.Index = 14;
      this._mnuSep6.Text = "-";
      // 
      // _mnuForward
      // 
      this._mnuForward.Index = 15;
      this._mnuForward.Text = "Переместить вперед";
      this._mnuForward.Click += new System.EventHandler(this._mnuForward_Click);
      // 
      // _mnuBackward
      // 
      this._mnuBackward.Index = 16;
      this._mnuBackward.Text = "Переместить назад";
      this._mnuBackward.Click += new System.EventHandler(this._mnuBackward_Click);
      // 
      // dlgImage
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this._cmdCancel;
      this.ClientSize = new System.Drawing.Size(623, 415);
      this.Controls.Add(this._cmdCancel);
      this.Controls.Add(this._sb);
      this.Controls.Add(this._tb);
      this.Menu = this._mnuMain;
      this.MinimizeBox = false;
      this.Name = "dlgImage";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Изображение";
      this.Load += new System.EventHandler(this.OnLoad);
      this.ResumeLayout(false);
      this.PerformLayout();

		}
		#endregion

		private void OnLoad(object sender, System.EventArgs e)
		{
			//StatusBarText();
			pIsImageFit = mUCIC.pIsFill;
			_ToolsBar_MenuView();
		}

		private void dlgImage_Closed(object sender, EventArgs e)
		{
			mUCIC.EndEdit();
		}

		private void mUCIC_CollectionCurrentItemChange(object sender, EventArgs e)
		{
			_ToolsBar_MenuView();
		}

		private void OnButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			ToolBarButton tbb = e.Button;
			MenuItem mi = tbb.Tag as MenuItem;
			if (mi!=null)
			{
				mi.PerformClick();
			}
		}

		private void DoCommandCancel(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
		private void _mnuAdd_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_New();
			//StatusBarText();
			_ToolsBar_MenuView();
		}

		private void _mnuDel_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_Delete();
			//StatusBarText();
			_ToolsBar_MenuView();
		}

		private void _mnuLoad_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_Load();
			//StatusBarText();
		}
		private void _mnuSaveAs_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_Save();
		}
		private void _mnuSelectScan_Click(object sender, System.EventArgs e)
		{
			mUCIC.SelectScan();
		}
		private void _mnuAcquire_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_Scan();
		}
		private void _mnuClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		private void _mnuClear_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_Clear();
			//StatusBarText();
		}
		private void _mnuCopy_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_Copy();
			_ToolsBar_MenuView();
		}
		private void _mnuPaste_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_Paste();
			//StatusBarText();
		}
		private void _mnuRotateL_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_RotateLeft();
			//StatusBarText();
		}
		private void _mnuRotateR_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_RotateRight();
			//StatusBarText();
		}
		private void _mnuSizeReal_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_RealFillToggle();
			_tbbFitReal.Pushed = mUCIC.pIsFill;
			_mnuSizeReal.Checked = !_tbbFitReal.Pushed;
			_ToolsBar_MenuView();
		}
		private void _mnuSizePlus_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_ZoomPlus();
			//StatusBarText();
		}
		private void _mnuSizeMinus_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_ZoomMinus();
			//StatusBarText();
		}
		private void _mnuForward_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_MoveForward();
			_ToolsBar_MenuView();
		}

		private void _mnuBackward_Click(object sender, System.EventArgs e)
		{
			mUCIC.CI_MoveBackward();
			_ToolsBar_MenuView();
		}

    private void _mnuSizeCorrect_Click(object sender, System.EventArgs e)
    {
      mUCIC.CI_CorrectImageSize();
    }


    //private void StatusBarText()
    //{
    //  if (mUCIC.pImage != null)
    //  {
    //    try
    //    {
    //      System.IO.MemoryStream ms = new System.IO.MemoryStream();
    //      mUCIC.pImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
    //      _sb.Text = "Размер изобр. = " + mUCIC.pImage.Width + "х" + mUCIC.pImage.Height + "; Размер файла = " + ms.ToArray().Length.ToString("# ###") + " байт";
    //    }
    //    catch (Exception ex)
    //    {
    //      _sb.Text = ex.Message;
    //    }
    //  }
    //}

    void mUCIC_ImageInformationChange(object sender, EventArgs e)
    {
      if (mUCIC.pImage != null)
      {
        _sb.Text = mUCIC.pStatusBarText;
      }
      else
        _sb.Text = string.Empty;
    }

		private void _ToolsBar_MenuView()
		{
			_tbbAdd.Enabled = mUCIC.pIsNew;
			_tbbDel.Enabled = mUCIC.pIsDelete;
			//_tbbLoad
			//_tbbCopy
			_tbbPaste.Enabled = mUCIC.pIsPaste;
			_tbbRotateL.Enabled = mUCIC.pIsRotate;
			_tbbRotateR.Enabled = _tbbRotateL.Enabled;
			//_tbbFitReal.Pushed = mUCIC
			_tbbZoomPlus.Enabled = mUCIC.pIsZoom;
			_tbbZoomMinus.Enabled = _tbbZoomPlus.Enabled;
			_tbbForward.Enabled = mUCIC.pIsMoveForward;
			_tbbBackward.Enabled = mUCIC.pIsMoveBackward;

			foreach (ToolBarButton tbb in _tb.Buttons)
			{
				MenuItem mnu = tbb.Tag as MenuItem;
				if (mnu != null)
					mnu.Enabled = tbb.Enabled;
			}
		}

	}
}
