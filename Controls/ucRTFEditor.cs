using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// Summary description for ucRTFEditor.
	/// </summary>
	public class ucRTFEditor : System.Windows.Forms.UserControl
	{

		private RichTextBox															mRtb;

		private System.Windows.Forms.Panel panelTB;
		private System.Windows.Forms.ToolBar tb;
		private System.Windows.Forms.ToolBarButton tbbBold;
		private ucComboFonts cboFont;
		private System.Windows.Forms.ComboBox cboFontSize;
		private System.Windows.Forms.ToolBarButton tbbItalic;
		private System.Windows.Forms.ToolBarButton tbbUnderline;
		private System.Windows.Forms.ToolBarButton tbbColor;
		private System.Windows.Forms.ToolBarButton tbbSep1;
		private System.Windows.Forms.ToolBarButton tbbLeft;
		private System.Windows.Forms.ToolBarButton tbbCenter;
		private System.Windows.Forms.ToolBarButton tbbRight;
		private System.Windows.Forms.ContextMenu mnuColor;
		private System.Windows.Forms.ImageList iml;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBarButton tbbSep2;
		private System.Windows.Forms.ToolBarButton tbbCut;
		private System.Windows.Forms.ToolBarButton tbbCopy;
		private System.Windows.Forms.ToolBarButton tbbPaste;
		private System.Windows.Forms.ToolBarButton tbbSep3;
		private System.Windows.Forms.ToolBarButton tbbInsPicFile;
		private System.Windows.Forms.ToolBarButton tbbUndo;


		public RichTextBox															pRichTB
		{
			get { return mRtb; }
			set
			{
				if (mRtb != null)
				{
					Controls.Remove(mRtb);
					mRtb.TextChanged -= new EventHandler(OnTextChanged);
					mRtb.SelectionChanged -= new EventHandler(OnTextSelectionChanged);
				}
				//mRtb.Dock = DockStyle.Fill;
				value.Location = new System.Drawing.Point(0, cboFont.Top+cboFont.Height+5);
				value.Size = new System.Drawing.Size(Width, Height-value.Location.Y);
				value.Anchor = AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Bottom|AnchorStyles.Right;
				mRtb = value;
				mRtb.TextChanged += new EventHandler(OnTextChanged);
				mRtb.SelectionChanged += new EventHandler(OnTextSelectionChanged);
				Controls.Add(mRtb);
			}
		}

		public string																	pRTF
		{
			set 
			{
				if (mRtb != null)
				{
					bool err = false;
					try {	mRtb.Rtf = value;} 
					catch {err = true;}
					if (err)
					{
						try 
						{
							mRtb.Clear();
							mRtb.AppendText(value);
						} 
						catch {err = true;}
					}
				}
			}
			get 
			{
				if (mRtb != null)
					return mRtb.Rtf;
				else
					return string.Empty;
			}
		}

		public StatusBarPanel													pSBPanel;

		public event EventHandler OnControlChanged;

		public ucRTFEditor() : this (null) {}
		public ucRTFEditor(RichTextBox rtb)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			if (rtb == null)
				pRichTB = new RichTextBox();
			else
				pRichTB = rtb;

			pSBPanel = null;

			pRichTB.HideSelection = false;

			#region Color menu
			MenuItemColor mic = new MenuItemColor(Color.Black);
			mic.Click += new EventHandler(OnTextColorChanged);
			mnuColor.MenuItems.Add(mic);
			mic = new MenuItemColor(Color.Blue);
			mic.Click += new EventHandler(OnTextColorChanged);
			mnuColor.MenuItems.Add(mic);
			mic = new MenuItemColor(Color.Green);
			mic.Click += new EventHandler(OnTextColorChanged);
			mnuColor.MenuItems.Add(mic);
			mic = new MenuItemColor(Color.Red);
			mic.Click += new EventHandler(OnTextColorChanged);
			mnuColor.MenuItems.Add(mic);
			mic = new MenuItemColor(Color.Yellow);
			mic.Click += new EventHandler(OnTextColorChanged);
			mnuColor.MenuItems.Add(mic);
			#endregion Color menu.
      this.EnabledChanged+=new EventHandler(onEnabled);
		}
    private void onEnabled (object sender, EventArgs e)
    {
      if (this.Enabled)
      {
        cboFont.BackColor=SystemColors.Window;
      }
      else
      {
        cboFont.BackColor=SystemColors.Control;
      }
    }

		private void OnTextChanged (object sender, EventArgs e)
		{
			if (OnControlChanged != null)
				OnControlChanged(this, new EventArgs());
		}

		private void OnTextSelectionChanged (object sender, EventArgs e)
		{
			if (pSBPanel != null)
				pSBPanel.Text = "SelectionStart = "+mRtb.SelectionStart;
			
			SetCurrentFont(mRtb.SelectionFont);
			SetCurrentAlignment(mRtb.SelectionAlignment);
			SetCutCopyPasteEnabled();
		}

		private void OnFontChanged(object sender, System.EventArgs e)
		{
			try
			{
				mRtb.SelectionFont = 
					new Font(cboFont.SelectedItem.ToString(),
					mRtb.SelectionFont.Size,
					mRtb.SelectionFont.Style);
		
				SetCurrentFont(mRtb.SelectionFont);
			}
			catch {}
		}

		private void OnFontSizeChanged(object sender, System.EventArgs e)
		{
			try
			{
				mRtb.SelectionFont = 
					new Font(mRtb.SelectionFont.Name,
					Convert.ToInt32(cboFontSize.SelectedItem.ToString()),
					mRtb.SelectionFont.Style);
		
				SetCurrentFont(mRtb.SelectionFont);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void OnTBButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbBold)
				mRtb.SelectionFont = 
					new Font(mRtb.SelectionFont.Name,mRtb.SelectionFont.Size,
					tbbBold.Pushed ? mRtb.SelectionFont.Style|FontStyle.Bold : mRtb.SelectionFont.Style&(~FontStyle.Bold));
			else if (e.Button == tbbItalic)
				mRtb.SelectionFont = 
					new Font(mRtb.SelectionFont.Name,mRtb.SelectionFont.Size,
					tbbItalic.Pushed ? mRtb.SelectionFont.Style|FontStyle.Italic : mRtb.SelectionFont.Style&(~FontStyle.Italic));
			else if (e.Button == tbbUnderline)
				mRtb.SelectionFont = 
					new Font(mRtb.SelectionFont.Name,mRtb.SelectionFont.Size,
					tbbUnderline.Pushed ? mRtb.SelectionFont.Style|FontStyle.Underline : mRtb.SelectionFont.Style&(~FontStyle.Underline));
			else if (e.Button == tbbLeft)
				mRtb.SelectionAlignment = HorizontalAlignment.Left;
			else if (e.Button == tbbCenter)
				mRtb.SelectionAlignment = HorizontalAlignment.Center;
			else if (e.Button == tbbRight)
				mRtb.SelectionAlignment = HorizontalAlignment.Right;
			else if (e.Button == tbbCut)
				mRtb.Cut();
			else if (e.Button == tbbCopy)
				mRtb.Copy();
			else if (e.Button == tbbPaste)
				mRtb.Paste();
			else if (e.Button == tbbUndo)
			{
				if (pSBPanel != null)
					pSBPanel.Text = mRtb.UndoActionName;
				mRtb.Undo();
			}
			else if (e.Button == tbbInsPicFile)
				InsertPicFile();

			SetCurrentFont(mRtb.SelectionFont);
			SetCurrentAlignment(mRtb.SelectionAlignment);
		}

		private void OnTextColorChanged (object sender, EventArgs e)
		{
				mRtb.SelectionColor = Color.FromName(((MenuItemColor)sender).Text);
		}

		private void SetCurrentFont(Font aFont)
		{
			if (aFont != null)
			{
				cboFont.SelectedItem = aFont.Name;
				cboFontSize.SelectedItem = ((int)aFont.SizeInPoints).ToString();

				tbbBold.Pushed = aFont.Bold;
				tbbItalic.Pushed = aFont.Italic;
				tbbUnderline.Pushed = aFont.Underline;
			}
		}

		private void SetCurrentAlignment(HorizontalAlignment aHA)
		{
			tbbLeft.Pushed = (aHA == HorizontalAlignment.Left);
			tbbCenter.Pushed = (aHA == HorizontalAlignment.Center);
			tbbRight.Pushed = (aHA == HorizontalAlignment.Right);
		}

		private void SetCutCopyPasteEnabled()
		{
			tbbCut.Enabled = (mRtb.SelectionLength > 0);
			tbbCopy.Enabled = (mRtb.SelectionLength > 0);
		}

		private void InsertPicFile()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			//dlg.Filter = "Файлы с изображением|*.jpg;*.jpeg;*.jfif;*.pnp;*.tif;*.tiff";
			dlg.Filter = "Все файлы с изображением|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.jfif;*.png;*.tif;*.tiff|" +
				"Windows Bitmap (*.bmp)|*.bmp|" +
				"Windows Icon (*.ico)|*.ico|" +
				"Graphics Interchange Format (*.gif)|*.gif|" +
				"JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg;*.jfif|" +
				"Portable Network Graphics (*.png)|*.png|" +
				"Tag Image File Format (*.tif)|*.tif;*.tiff|" +
				"All Files (*.*)|*.*";

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				Image img = Image.FromFile(dlg.FileName);
				if (img != null)
				{
					Clipboard.SetDataObject(img);
					mRtb.Paste();
				}
			}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ucRTFEditor));
			this.panelTB = new System.Windows.Forms.Panel();
			this.tb = new System.Windows.Forms.ToolBar();
			this.tbbBold = new System.Windows.Forms.ToolBarButton();
			this.tbbItalic = new System.Windows.Forms.ToolBarButton();
			this.tbbUnderline = new System.Windows.Forms.ToolBarButton();
			this.tbbColor = new System.Windows.Forms.ToolBarButton();
			this.mnuColor = new System.Windows.Forms.ContextMenu();
			this.tbbSep1 = new System.Windows.Forms.ToolBarButton();
			this.tbbLeft = new System.Windows.Forms.ToolBarButton();
			this.tbbCenter = new System.Windows.Forms.ToolBarButton();
			this.tbbRight = new System.Windows.Forms.ToolBarButton();
			this.iml = new System.Windows.Forms.ImageList(this.components);
			this.cboFont = new CommandAS.Tools.Controls.ucComboFonts();
			this.cboFontSize = new System.Windows.Forms.ComboBox();
			this.tbbSep2 = new System.Windows.Forms.ToolBarButton();
			this.tbbCut = new System.Windows.Forms.ToolBarButton();
			this.tbbCopy = new System.Windows.Forms.ToolBarButton();
			this.tbbPaste = new System.Windows.Forms.ToolBarButton();
			this.tbbSep3 = new System.Windows.Forms.ToolBarButton();
			this.tbbInsPicFile = new System.Windows.Forms.ToolBarButton();
			this.tbbUndo = new System.Windows.Forms.ToolBarButton();
			this.panelTB.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTB
			// 
			this.panelTB.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.panelTB.Controls.AddRange(new System.Windows.Forms.Control[] {
																																					this.tb});
			this.panelTB.Location = new System.Drawing.Point(264, 0);
			this.panelTB.Name = "panelTB";
			this.panelTB.Size = new System.Drawing.Size(352, 28);
			this.panelTB.TabIndex = 0;
			// 
			// tb
			// 
			this.tb.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																																					this.tbbBold,
																																					this.tbbItalic,
																																					this.tbbUnderline,
																																					this.tbbColor,
																																					this.tbbSep1,
																																					this.tbbLeft,
																																					this.tbbCenter,
																																					this.tbbRight,
																																					this.tbbSep2,
																																					this.tbbCut,
																																					this.tbbCopy,
																																					this.tbbPaste,
																																					this.tbbUndo,
																																					this.tbbSep3,
																																					this.tbbInsPicFile});
			this.tb.DropDownArrows = true;
			this.tb.ImageList = this.iml;
			this.tb.Name = "tb";
			this.tb.ShowToolTips = true;
			this.tb.Size = new System.Drawing.Size(352, 25);
			this.tb.TabIndex = 1;
			this.tb.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.OnTBButtonClick);
			// 
			// tbbBold
			// 
			this.tbbBold.ImageIndex = 0;
			this.tbbBold.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbBold.ToolTipText = "Жирный";
			// 
			// tbbItalic
			// 
			this.tbbItalic.ImageIndex = 1;
			this.tbbItalic.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbItalic.ToolTipText = "Курсив";
			// 
			// tbbUnderline
			// 
			this.tbbUnderline.ImageIndex = 2;
			this.tbbUnderline.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbUnderline.ToolTipText = "Подчеркнутый";
			// 
			// tbbColor
			// 
			this.tbbColor.DropDownMenu = this.mnuColor;
			this.tbbColor.ImageIndex = 3;
			this.tbbColor.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
			this.tbbColor.ToolTipText = "Цвет";
			// 
			// tbbSep1
			// 
			this.tbbSep1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbLeft
			// 
			this.tbbLeft.ImageIndex = 4;
			this.tbbLeft.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbLeft.ToolTipText = "По левому краю";
			// 
			// tbbCenter
			// 
			this.tbbCenter.ImageIndex = 5;
			this.tbbCenter.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbCenter.ToolTipText = "По центру";
			// 
			// tbbRight
			// 
			this.tbbRight.ImageIndex = 6;
			this.tbbRight.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbRight.ToolTipText = "По правому краю";
			// 
			// iml
			// 
			this.iml.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.iml.ImageSize = new System.Drawing.Size(16, 16);
			this.iml.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iml.ImageStream")));
			this.iml.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// cboFont
			// 
			this.cboFont.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cboFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboFont.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
			this.cboFont.IntegralHeight = false;
			this.cboFont.Items.AddRange(new object[] {
																								 "Arial",
																								 "Arial Black",
																								 "Arial Narrow",
																								 "Arial Narrow Special G1",
																								 "Arial Narrow Special G2",
																								 "Arial Special G1",
																								 "Arial Special G2",
																								 "Arial Unicode MS",
																								 "Batang",
																								 "Book Antiqua",
																								 "Bookman Old Style",
																								 "Century",
																								 "Century Gothic",
																								 "Comic Sans MS",
																								 "Courier New",
																								 "CyrillicChancellor",
																								 "CyrillicCooper",
																								 "CyrillicGaramond",
																								 "CyrillicGoth",
																								 "CyrillicHeavy",
																								 "CyrillicHelv",
																								 "CyrillicHover",
																								 "CyrillicRevue",
																								 "CyrillicRibbon",
																								 "Garamond",
																								 "Georgia",
																								 "Haettenschweiler",
																								 "Impact",
																								 "Lucida Console",
																								 "Lucida Sans Unicode",
																								 "Map Symbols",
																								 "Marlett",
																								 "Microsoft Sans Serif",
																								 "Monotype Sorts",
																								 "MS Mincho",
																								 "MS Outlook",
																								 "MT Extra",
																								 "Palatino Linotype",
																								 "PMingLiU",
																								 "SimSun",
																								 "StylusImperialCyr",
																								 "Symbol",
																								 "Tahoma",
																								 "Times New Roman",
																								 "Times New Roman Special G1",
																								 "Times New Roman Special G2",
																								 "Trebuchet MS",
																								 "Verdana",
																								 "Webdings",
																								 "Wingdings",
																								 "Wingdings 2",
																								 "Wingdings 3",
																								 "Arial",
																								 "Arial Black",
																								 "Arial Narrow",
																								 "Arial Narrow Special G1",
																								 "Arial Narrow Special G2",
																								 "Arial Special G1",
																								 "Arial Special G2",
																								 "Arial Unicode MS",
																								 "Batang",
																								 "Book Antiqua",
																								 "Bookman Old Style",
																								 "Century",
																								 "Century Gothic",
																								 "Comic Sans MS",
																								 "Courier New",
																								 "CyrillicChancellor",
																								 "CyrillicCooper",
																								 "CyrillicGaramond",
																								 "CyrillicGoth",
																								 "CyrillicHeavy",
																								 "CyrillicHelv",
																								 "CyrillicHover",
																								 "CyrillicRevue",
																								 "CyrillicRibbon",
																								 "Garamond",
																								 "Georgia",
																								 "Haettenschweiler",
																								 "Impact",
																								 "Lucida Console",
																								 "Lucida Sans Unicode",
																								 "Map Symbols",
																								 "Marlett",
																								 "Microsoft Sans Serif",
																								 "Monotype Sorts",
																								 "MS Mincho",
																								 "MS Outlook",
																								 "MT Extra",
																								 "Palatino Linotype",
																								 "PMingLiU",
																								 "SimSun",
																								 "StylusImperialCyr",
																								 "Symbol",
																								 "Tahoma",
																								 "Times New Roman",
																								 "Times New Roman Special G1",
																								 "Times New Roman Special G2",
																								 "Trebuchet MS",
																								 "Verdana",
																								 "Webdings",
																								 "Wingdings",
																								 "Wingdings 2",
																								 "Wingdings 3",
																								 "Arial",
																								 "Arial Black",
																								 "Arial Narrow",
																								 "Arial Narrow Special G1",
																								 "Arial Narrow Special G2",
																								 "Arial Special G1",
																								 "Arial Special G2",
																								 "Arial Unicode MS",
																								 "Batang",
																								 "Book Antiqua",
																								 "Bookman Old Style",
																								 "Century",
																								 "Century Gothic",
																								 "Comic Sans MS",
																								 "Courier New",
																								 "CyrillicChancellor",
																								 "CyrillicCooper",
																								 "CyrillicGaramond",
																								 "CyrillicGoth",
																								 "CyrillicHeavy",
																								 "CyrillicHelv",
																								 "CyrillicHover",
																								 "CyrillicRevue",
																								 "CyrillicRibbon",
																								 "Garamond",
																								 "Georgia",
																								 "Haettenschweiler",
																								 "Impact",
																								 "Lucida Console",
																								 "Lucida Sans Unicode",
																								 "Map Symbols",
																								 "Marlett",
																								 "Microsoft Sans Serif",
																								 "Monotype Sorts",
																								 "MS Mincho",
																								 "MS Outlook",
																								 "MT Extra",
																								 "Palatino Linotype",
																								 "PMingLiU",
																								 "SimSun",
																								 "StylusImperialCyr",
																								 "Symbol",
																								 "Tahoma",
																								 "Times New Roman",
																								 "Times New Roman Special G1",
																								 "Times New Roman Special G2",
																								 "Trebuchet MS",
																								 "Verdana",
																								 "Webdings",
																								 "Wingdings",
																								 "Wingdings 2",
																								 "Wingdings 3"});
			this.cboFont.MaxDropDownItems = 20;
			this.cboFont.Name = "cboFont";
			this.cboFont.Size = new System.Drawing.Size(192, 23);
			this.cboFont.TabIndex = 1;
			this.cboFont.SelectedIndexChanged += new System.EventHandler(this.OnFontChanged);
			// 
			// cboFontSize
			// 
			this.cboFontSize.Items.AddRange(new object[] {
																										 "8",
																										 "9",
																										 "10",
																										 "11",
																										 "12",
																										 "14",
																										 "16",
																										 "18",
																										 "20",
																										 "22",
																										 "24",
																										 "26",
																										 "28",
																										 "36",
																										 "48",
																										 "72"});
			this.cboFontSize.Location = new System.Drawing.Point(200, 0);
			this.cboFontSize.Name = "cboFontSize";
			this.cboFontSize.Size = new System.Drawing.Size(56, 24);
			this.cboFontSize.TabIndex = 2;
			this.cboFontSize.SelectedIndexChanged += new System.EventHandler(this.OnFontSizeChanged);
			// 
			// tbbSep2
			// 
			this.tbbSep2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbCut
			// 
			this.tbbCut.ImageIndex = 7;
			// 
			// tbbCopy
			// 
			this.tbbCopy.ImageIndex = 8;
			// 
			// tbbPaste
			// 
			this.tbbPaste.ImageIndex = 9;
			// 
			// tbbSep3
			// 
			this.tbbSep3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbInsPicFile
			// 
			this.tbbInsPicFile.ImageIndex = 11;
			// 
			// tbbUndo
			// 
			this.tbbUndo.ImageIndex = 10;
			// 
			// ucRTFEditor
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.cboFontSize,
																																	this.cboFont,
																																	this.panelTB});
			this.Name = "ucRTFEditor";
			this.Size = new System.Drawing.Size(616, 248);
			this.panelTB.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

	}
}
