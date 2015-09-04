using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CommandAS.Tools;
using CommandAS.Tools.Controls;
using CommandAS.Tools.TwainLib;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// Отображение коллекции образов с поддержкой основных функций.
	/// </summary>
	public class ucImageCollection : System.Windows.Forms.UserControl, IMessageFilter
	{
		private const int										ZOOM_MAX	=	5;

		private Panel												_pnl;
		private ucMoveNavigator							_nav;
		private TextBox											_txt;
		private Twain												_tw;
		private bool												_msgfilter;
		private bool												_isFill;
		private Image												_curImg;
		private DataRowState								_curState;
		private int													_zoom	=	0;

		protected StateCollection						mIC;

		public bool													pIsFill
		{
			get { return _isFill; }
		}

		public Image												pImage
		{
			get {return _curImg;}
			set 
			{
				_curImg = value;
				if (_isFill || _curImg == null)
					_pnl.AutoScrollMinSize=new Size(0,0);
				else
					_pnl.AutoScrollMinSize=_curImg.Size;
				_pnl.Refresh();
			}
		}

    public String                       pText
    {
      get { return _txt.Text;  }
      set { _txt.Text = value; }
    }

		public bool													pIsImageCollection
		{
			get { return _nav.Enabled;  }
			set	{	_nav.Enabled = value;	}
		}

		public bool													pIsModified
		{
			get
			{
				bool ret = (_curState != DataRowState.Unchanged);

				if (!ret && pIsImageCollection)
					foreach (PicItem pic in mIC)
					{
						ret = (pic.pState != DataRowState.Unchanged);
						if (ret)
							break;
					}
							
				return ret;
			}
		}

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public	bool												pIsNew
		{
			get { return pIsImageCollection;}
		}
		public	bool												pIsDelete
		{
			get { return pIsImageCollection;}
		}
		public	bool												pIsSave
		{
			get { return true;}
		}
		public	bool												pIsScan
		{
			get { return true;}
		}
		public	bool												pIsPaste
		{
			get { return true;}
		}
		public	bool												pIsZoom
		{
			get { return !_isFill;}
		}
		public	bool												pIsRotate
		{
      get { return _curImg != null; } //!_isFill;}
		}
		public	bool												pIsMoveForward
		{
			get { return pIsImageCollection && (mIC.pCurrentIndex < mIC.pCount-1);}
		}
		public	bool												pIsMoveBackward
		{
			get { return pIsImageCollection && (mIC.pCurrentIndex > 0);}
		}

    public int pMaxImageSize = 0;
    private string _sbText;
    public string pStatusBarText
    {
      get { return _sbText; }
      set
      {
        _sbText = value;

        if (ImageInformationChange != null)
          ImageInformationChange(this, new EventArgs());
      }
    }

		public event EventHandler					CollectionCurrentItemChange;
    public event EventHandler         ImageInformationChange;

		public StateCollection							pImageCollection
		{
			get { return mIC; }
			set
			{
				mIC = value;

				if (_nav != null && mIC != null)
				{
					if (mIC.Count == 0)
					{
						CI_New();
					}
					else
					{
						mIC.GoFirst();
						PicItem pic = mIC.pCurrentItem as PicItem;
						if (pic != null)
						{
							_curImg = pic.pImage;
							_txt.Text = pic.pText;
							_curState = pic.pState;
							_nav.pMaxValue = mIC.Count;
							_nav.pPosition = mIC.pCurrentIndex+1;
              pStatusBarText = ImageTools.ImageInformation(_curImg);
            }
					}
				}
			}
		}
    
		public ucImageCollection() : this (new StateCollection()) {}
		public ucImageCollection(StateCollection aIC)
		{
      pStatusBarText = string.Empty;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			_pnl = new Panel();
			_nav = new ucMoveNavigator();
			_txt = new TextBox();

			_nav.Location = new System.Drawing.Point(0, this.Height-_nav.Height);
			_txt.Location = new System.Drawing.Point(_nav.Width+10, _nav.Top);
			_txt.Width = Width - _txt.Left-10;
			_txt.BackColor = SystemColors.Control;
			_txt.TextAlign = HorizontalAlignment.Center;
			_pnl.Location = new System.Drawing.Point(5, 5);
			_pnl.Size = new System.Drawing.Size(this.Width-10, _nav.Top-10);

			_pnl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			_nav.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			_txt.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

			Controls.AddRange(new Control [] {_pnl, _nav, _txt});

			_pnl.Paint += new PaintEventHandler(_pnl_Paint);
			_nav.MoveNavigator += new EvH_MoveNavigator(_nav_MoveNavigator);
			_txt.TextChanged += new EventHandler(_txt_TextChanged);

			_nav.pMinValue = 1;
			pImageCollection = aIC;

			_isFill = true;
			_curImg = null;
			_curState = DataRowState.Unchanged;

			ResizeRedraw = true;

			_tw = new Twain();
			_tw.Init( this.Handle );
			_msgfilter = false;
		}

		public void EndEdit()
		{
			if (pIsImageCollection)
			{
				PicItem pic = mIC.pCurrentItem as PicItem;
				if (pic != null)
				{
					pic.pText		= _txt.Text;
					pic.pImage	= pImage;
					pic.pState	= _curState;
				}
			}
		}

		private void _pnl_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(this.BackColor);
			if (_curImg != null)
			{
				Rectangle rect=new Rectangle(0, 0, _pnl.Width, _pnl.Height);
				if (_isFill)
				{
					ImageTools.ScaleImageIsotropically(e.Graphics, _curImg, rect);
				}
				else
				{
					
					Point pointBegin=new Point(_pnl.AutoScrollPosition.X,_pnl.AutoScrollPosition.Y);

					Size add = _zoomSize();

					RectangleF R=new RectangleF(pointBegin,add);

					GraphicsUnit unit=GraphicsUnit.Pixel;
					RectangleF r=_curImg.GetBounds(ref unit);
					e.Graphics.DrawImage(_curImg,R,r,GraphicsUnit.Pixel);
				}
			}
		}

		private Size _zoomSize()
		{
			Size ret;

			int zoom=Math.Abs(_zoom)+1;
			if (_zoom>0)
				ret=new Size(_curImg.Width*zoom,_curImg.Height*zoom);
			else
				ret=new Size(_curImg.Width/zoom,_curImg.Height/zoom);

			return ret;
		}

		private void _nav_MoveNavigator(object sender, EvA_MoveNavigator e)
		{
			PicItem pic = mIC.pCurrentItem as PicItem;
			if (pic != null)
			{
				pic.pImage	= pImage;
				pic.pText		= _txt.Text;
				pic.pState	= _curState;
			}

			switch (e.pDirection)
			{
				case eDirection.First:
					mIC.GoFirst();
					break;
				case eDirection.Last:
					mIC.GoLast();
					break;
				case eDirection.Next:
					mIC.GoNext();
					break;
				case eDirection.Prev:
					mIC.GoPrev();
					break;
				case eDirection.Position:
					mIC.pCurrentIndex = e.pPosition - 1;
					break;
			}

			pic = mIC.pCurrentItem as PicItem;
			if (pic != null)
			{
				pImage					= pic.pImage;
				_txt.Text				= pic.pText;
				_curState				= pic.pState;
        pStatusBarText = ImageTools.ImageInformation(pImage);
			}

			if (CollectionCurrentItemChange != null)
				CollectionCurrentItemChange(this, new EventArgs());

		}

		private void _txt_TextChanged(object sender, EventArgs e)
		{
			_SetStatusCurrentImage();
		}

		private void _SetStatusCurrentImage()
		{
			if (_curState == DataRowState.Unchanged)
				_curState = DataRowState.Modified;
		}

		#region Do it for current image

		public void CI_New()
		{
			PicItem pic = new PicItem();
			pImage = pic.pImage;
			_txt.Text = pic.pText;
			mIC.Add(pic);
			_curState = pic.pState;
			_nav.pMaxValue = mIC.Count;
			_nav.pPosition = _nav.pMaxValue;
		}

		public void CI_Load()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = CommonConst.OPENFILEDIALOG_FILTER_IMAGE;
			dlg.Title="Загрузить файл с изображением";
			//dlg.RestoreDirectory=true;
			//dlg.InitialDirectory=mLocalPath;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				Image img = null;
				try{ img = Image.FromFile(dlg.FileName); }
				catch
				{
					MessageBox.Show ("Неизвестный формат файла с изображением!","Файл: "+dlg.FileName,MessageBoxButtons.OK, MessageBoxIcon.Error); 
				}
				if (img != null)
				{
					pImage = img;
          CI_CorrectImageSize();
          _SetStatusCurrentImage();
				}
			}
		}

		public void CI_Scan()
		{
			if( ! _msgfilter )
			{
				this.Enabled = false;
				_msgfilter = true;
				Application.AddMessageFilter( this );
			}

			_tw.Acquire();
		}

		public void CI_Save()
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg;*.jfif";
			if (dlg.ShowDialog() == DialogResult.OK)
				_curImg.Save(dlg.FileName);
		}

		public void CI_Delete()
		{
			mIC.RemoveCurrent();
			_nav.pMaxValue = mIC.pCount;
			
			PicItem pic = mIC.pCurrentItem as PicItem;
			if (pic != null)
			{
				pImage = pic.pImage;
				_txt.Text = pic.pText;
				_nav.pPosition = mIC.pCurrentIndex+1;
			}
			else
			{
				pImage = null;
				_txt.Text = string.Empty;
				//_nav.pPosition = mIC.pCurrentIndex+1;
			}
		}

		public void CI_Clear()
		{
			pImage = null;
		}

		public void CI_Copy()
		{
			try
			{
				CommandAS.Tools.MouseCursor.SetCursorWait();
				Clipboard.SetDataObject(pImage);
			}
			catch{}
			CommandAS.Tools.MouseCursor.SetCursorDefault();
		}

		public void CI_Paste()
		{
			IDataObject io=Clipboard.GetDataObject();
			if (io==null)
				return;
			Image img=io.GetData(typeof(Bitmap)) as Image;
			if (img != null)
			{
				pImage = img;
        CI_CorrectImageSize();
        _SetStatusCurrentImage();
			}
		}

		public bool CI_ZoomPlus()
		{
			bool ret = true;
			_zoom++;
			if (_zoom>ZOOM_MAX)
			{
				_zoom=ZOOM_MAX;
				ret = false;
			}
			else
			{
				_pnl.Refresh();
				if (!_isFill)
					_pnl.AutoScrollMinSize=_zoomSize();
			}
			return ret;
		}

		public bool CI_ZoomMinus()
		{
			bool ret = true;
			_zoom--;
			if (_zoom<ZOOM_MAX*(-1))
			{
				_zoom=ZOOM_MAX*(-1);
				ret = false;
			}
			else
			{
				_pnl.Refresh();
				if (!_isFill)
					_pnl.AutoScrollMinSize=_zoomSize();
			}
			return ret;
		}

		public void CI_RotateLeft()
		{
			_curImg.RotateFlip(RotateFlipType.Rotate90FlipXY);
			if (!_isFill)
				_pnl.AutoScrollMinSize=_curImg.Size;
			_pnl.Refresh();
      _SetStatusCurrentImage();

		}

		public void CI_RotateRight()
		{
			_curImg.RotateFlip(RotateFlipType.Rotate90FlipNone);
			if (!_isFill)
				_pnl.AutoScrollMinSize=_curImg.Size;
			_pnl.Refresh();
      _SetStatusCurrentImage();

		}

		public void CI_RealFillToggle()
		{
			_zoom=0;
			_isFill = !_isFill;
			if (_isFill)
			{
				_pnl.AutoScrollMinSize=new Size(0,0);
			}
			else
			{
				if (pImage != null)
					_pnl.AutoScrollMinSize=pImage.Size;
			}
			_pnl.Refresh();
		}

		public void CI_MoveForward()
		{
			mIC.MoveForward();
			_nav.pPosition = mIC.pCurrentIndex+1;
			PicItem pic = mIC.pCurrentItem as PicItem;
			if (pic != null)
				_curState				= pic.pState;
		}

		public void CI_MoveBackward()
		{
			mIC.MoveBackward();
			_nav.pPosition = mIC.pCurrentIndex+1;
			PicItem pic = mIC.pCurrentItem as PicItem;
			if (pic != null)
				_curState				= pic.pState;
		}

    public void CI_CorrectImageSize()
    {
      string ss = string.Empty;
      pImage = ImageTools.DecreaseImageSize(pImage, pMaxImageSize, out ss);
      pStatusBarText = ss;
    }

		#endregion

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				_tw.Finish();
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
			// 
			// ucImageCollection
			// 
			this.Name = "ucImageCollection";
			this.Size = new System.Drawing.Size(376, 328);

		}
		#endregion

		public void SelectScan()
		{
			_tw.Select();
		}

		bool IMessageFilter.PreFilterMessage( ref Message m )
		{
			TwainCommand cmd = _tw.PassMessage( ref m );
			if( cmd == TwainCommand.Not )
				return false;

			switch( cmd )
			{
				case TwainCommand.CloseRequest:
				{
					EndingScan();
					_tw.CloseSrc();
					break;
				}
				case TwainCommand.CloseOk:
				{
					EndingScan();
					_tw.CloseSrc();
					break;
				}
				case TwainCommand.DeviceEvent:
				{
					break;
				}
				case TwainCommand.TransferReady:
				{
					ArrayList pics = _tw.TransferPictures();
					EndingScan();
					_tw.CloseSrc();
					if (pics.Count > 0)
					{
						IntPtr img = (IntPtr) pics[0];
						TwainImage ti = new TwainImage(img);
						pImage = ti.GetImage2();
            //pImage = ti.GetImage();
            CI_CorrectImageSize();
            _SetStatusCurrentImage();
					}
					break;
				}
			}

			return true;
		}

		private void EndingScan()
		{
			if( _msgfilter )
			{
				Application.RemoveMessageFilter( this );
				_msgfilter = false;
				this.Enabled = true;
				//this.Activate();
			}
		}

  }
}
