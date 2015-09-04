using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using NetSpell.SpellChecker;

namespace CommandAS.Tools.Controls
{
  public partial class ucHTML : UserControl
  {
    private string _srcHTMLText;

    public WebBrowser pWB
    {
      get { return _wb; }
    }

    public bool pDesignMode
    {
      get
      {
        bool ret = false;
        if (_wb.Document != null)
        {
          mshtml.HTMLDocumentClass doc = _wb.Document.DomDocument as mshtml.HTMLDocumentClass;
          if (doc != null)
            ret = doc.designMode.ToUpper().Equals("ON");
        }
        return ret;
      }
      set
      {
        if (_wb.Document != null)
        {
          mshtml.HTMLDocumentClass doc = _wb.Document.DomDocument as mshtml.HTMLDocumentClass;
          if (doc != null)
            doc.designMode = value ? "On" : "Off";
        }
        if (value)
          Controls.Add(pTS);
        else
          Controls.Remove(pTS);
      }
    }

    public string Text
    {
      get { return DocHtml; }
      set { DocHtml = value; }
    }

    public string DocHtml
    {
      get 
	    {
		  string ret = null;
		  if (_wb.Document != null && _wb.Document.Body != null)
			ret = _wb.Document.Body.InnerHtml;
		  return ret;
	    }

      set 
      {
        if (_wb.Document != null && value != null) // && value.Length > 0)
        {
          if (_wb.Document.Body != null)
          {
            //_wb.Document.Body.KeyPress -= new HtmlElementEventHandler(Body_KeyPress);
            _wb.Document.Body.KeyDown -= new HtmlElementEventHandler(Body_KeyPress);
          }

          HtmlDocument doc = _wb.Document.OpenNew(true);
          if (value.Length == 0)
          {
            value = "<html><body></body></html>";
          }
          doc.Write(value);

          _srcHTMLText = value;
          if (_wb.Document.Body != null)
          {
            //doc.Body.KeyPress += new HtmlElementEventHandler(Body_KeyPress);
            doc.Body.KeyDown += new HtmlElementEventHandler(Body_KeyPress);
          }

          //_wb.Refresh(WebBrowserRefreshOption.Normal);
          this.Refresh();
        }
      }
    }

    public bool pIsModified
    {
      get
      {
        bool ret = false;
        if (pDesignMode && _wb.Document.Body != null && _wb.Document.Body.InnerHtml != null)
          ret = !_wb.Document.Body.InnerHtml.Equals(_srcHTMLText);
        return ret;
      }
    }

    public bool pIsEmpty
    {
      get
      {
        Boolean ret = true;
        if (_wb.Document != null && _wb.Document.Body != null && _wb.Document.Body.InnerText != null)
          ret = (_wb.Document.Body.InnerText.Trim().Length == 0);
        return ret;
      }
    }

    public bool pIsPrint
    {
      get { return _tsbPrint.Visible; }
      set
      {
        _tsbPrint.Visible = value;
        _tsbPreview.Visible = value;
        _tsbSepPrint.Visible = value;
      }
    }

    public event EventHandler OnControlChanged;

    private Spelling _spell;
    public static NetSpell.SpellChecker.Dictionary.WordDictionary pWordDic = new NetSpell.SpellChecker.Dictionary.WordDictionary();

    public ucHTML()
    {
      _srcHTMLText = string.Empty;

      InitializeComponent();

      _wb.Navigate("about:blank");

      //pImageCollection = null;

      #region ToolsStrip Image
      _tsbCut.Image = Bitmap.FromHicon(ToolsRes.iiCut.Handle);
      _tsbCopy.Image = Bitmap.FromHicon(ToolsRes.iiCopy.Handle);
      _tsbPaste.Image = Bitmap.FromHicon(ToolsRes.iiPaste.Handle);
      _tsbBold.Image = Bitmap.FromHicon(ToolsRes.iiBold.Handle);
      _tsbItalic.Image = Bitmap.FromHicon(ToolsRes.iiItalic.Handle);
      _tsbUnderline.Image = Bitmap.FromHicon(ToolsRes.iiUnderline.Handle);
      _tsbLeft.Image = Bitmap.FromHicon(ToolsRes.iiJustifyLeft.Handle);
      _tsbCenter.Image = Bitmap.FromHicon(ToolsRes.iiJustifyCenter.Handle);
      _tsbRight.Image = Bitmap.FromHicon(ToolsRes.iiJustifyRight.Handle);
      _tsbUndo.Image = Bitmap.FromHicon(ToolsRes.iiUndo.Handle);
      _tsbRedo.Image = Bitmap.FromHicon(ToolsRes.iiRedo.Handle);
      //_tscFont.Image = Bitmap.FromHicon(ToolsRes.iifo.Handle);
      _tsbPrint.Image = Bitmap.FromHicon(ToolsRes.iiPrint.Handle);
      _tsbPreview.Image = Bitmap.FromHicon(ToolsRes.iiPreview.Handle);
      #endregion

      #region ToolsStrip ToolTipText
      _tsbCut.ToolTipText = ToolsRes.fCut;
      _tsbCopy.ToolTipText = ToolsRes.fCopy;
      _tsbPaste.ToolTipText = ToolsRes.fPaste;
      _tsbBold.ToolTipText = ToolsRes.fBold;
      _tsbItalic.ToolTipText = ToolsRes.fItalic;
      _tsbUnderline.ToolTipText = ToolsRes.fUnderline;
      _tsbLeft.ToolTipText = ToolsRes.fJustifyLeft;
      _tsbCenter.ToolTipText = ToolsRes.fJustifyCenter;
      _tsbRight.ToolTipText = ToolsRes.fJustifyRight;
      _tsbUndo.ToolTipText = ToolsRes.fUndo;
      _tsbRedo.ToolTipText = ToolsRes.fRedo;
      _tscFont.ToolTipText = ToolsRes.fFontChange;
      _tsbPrint.ToolTipText = ToolsRes.fPrintText;
      _tsbPreview.ToolTipText = ToolsRes.fPrintPreviewText;
      _tsbSpellChecker.ToolTipText = ToolsRes.fSpellChecker;
      #endregion

      int ind = 0;
      foreach (FontFamily ff in FontFamily.Families)
      {
        if (ff.IsStyleAvailable(FontStyle.Regular))
          ind = _tscFont.Items.Add(ff.Name);
	    }
      _tscFont.ComboBox.DropDownWidth = _tscFont.ComboBox.Width * 2;

	    for (ind = 1; ind < 8; ind++)
	      _tscSize.Items.Add(ind);
	
      Controls.Remove(pTS);

      //_wb.
      //_wb.GotFocus += new EventHandler(_wb_GotFocus);

      _tsbSpellChecker.Image =  ToolsRes.Spelling;
      this.components = new System.ComponentModel.Container();
      _spell = new Spelling(this.components);
      _spell.Dictionary = pWordDic;
      //_spell.ReplacedWord += new Spelling.ReplacedWordEventHandler(_spell_ReplacedWord);
      //_spell.DeletedWord += new Spelling.DeletedWordEventHandler(_spell_DeletedWord);
      _spell.EndOfText += new Spelling.EndOfTextEventHandler(_spell_EndOfText);
    }

    //void _wb_GotFocus(object sender, EventArgs e)
    //{
    //  //if (!pDesignMode && _wb.Document != null && _wb.Document.Body != null)
    //  //  _wb.Document.Body.RemoveFocus();
    //}

	  /// <summary>
	  /// «адать FontFamily.Name и Font.Size. Ќе хочет работать: у ComboBox'ов
	  /// значени€ выбраны, но начинаешь печатать - ничего хорошего. Ќадо доработать.
	  /// </summary>
	  /// <param name="aFFamily">FontFamily.Name</param>
	  /// <param name="aFSize">Size [1..7]</param>
	  public void SetFont(string aFFamily, int aFSize)
	  {
		  _tscFont.SelectedIndex = _tscFont.FindString(aFFamily);
		  _wb.Document.ExecCommand("FontName", false, aFFamily);
		  aFSize = (aFSize > 7) ? 7 : (aFSize < 1) ? 1 : aFSize;
		  _tscSize.SelectedIndex = _tscSize.FindString(aFSize.ToString());
		  _wb.Document.ExecCommand("FontSize", false, aFSize.ToString());
	  }

    /// <summary>
      /// ƒобавление текста в DocHTML после текста и перед закрывающими тегами
      /// </summary>
      /// <param name="aText">ƒобавл€емый текст</param>
    public void AddText(string aText)
      {
          string EndTags = string.Empty;
          string DHText = (DocHtml == null) ? "" : DocHtml;
          int LastS = 0;
          bool IsLast = false;

          while (!IsLast)
          {
              // ≈сли в конце текста (DHText) не тег, т.е. последний символ не '>',
              // то добавл€ем aText и закрываем конечными тегами
              // »наче, убираем последний тег, который приписывем EndTags
              if (DHText.Length == 0 || DHText.LastIndexOf('>') != DHText.Length - 1)
              {
                  DocHtml = DHText + " " + aText + EndTags;
                  IsLast = true;
              }
              else
              {
                  LastS = DHText.LastIndexOf("</");
                  EndTags = DHText.Substring(LastS) + EndTags;
                  DHText = DHText.Remove(LastS);
              }
          }
      }

    public void Navigate(string aURL)
    {
      _wb.Navigate(aURL);
    }

    public void Print(bool bPreview)
    {
      if (bPreview)
        _wb.ShowPrintPreviewDialog();
      else
        //_wb.Document.ExecCommand("Print", false, null);
        _wb.Print();
    }

    public void DocRefresh()
    {
      _wb.Document.ExecCommand("Refresh", false, null);
    }

    void Body_KeyPress(object sender, HtmlElementEventArgs e)
    {
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbCut_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("Cut", false, null);
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbCopy_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("Copy", false, null);
    }

    private void _tsbPaste_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("Paste", false, null);
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbBold_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("Bold", false, null);
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbItalic_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("Italic", false, null);
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbUnderline_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("Underline", false, null);
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbLeft_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("JustifyLeft", false, null);
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbCenter_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("JustifyCenter", false, null);
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbRight_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("JustifyRight", false, null);
      if (pIsModified)
      {
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    private void _tsbUndo_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("Undo", false, null);
    }

    private void _tsbRedo_Click(object sender, EventArgs e)
    {
      _wb.Document.ExecCommand("Redo", false, null);
    }

    private void _tscFont_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (_tscFont.Text.Length > 0)
      {
        _wb.Document.ExecCommand("FontName", false, _tscFont.Text);
        if (pIsModified)
        {
          if (this.OnControlChanged != null)
            this.OnControlChanged(this, EventArgs.Empty);

          this.OnTextChanged(new EventArgs());
        }
      }
    }

    private void _tscSize_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (_tscSize.Text.Length > 0)
      {
        _wb.Document.ExecCommand("FontSize", false, _tscSize.Text);
        if (pIsModified)
        {
          if (this.OnControlChanged != null)
            this.OnControlChanged(this, EventArgs.Empty);

          this.OnTextChanged(new EventArgs());
        }
    }
    }

    private void _tsbPrint_Click(object sender, EventArgs e)
    {
      _wb.Print();
      //_wb.Document.ExecCommand("Print", false, null);
    }

    private void _tsbPreview_Click(object sender, EventArgs e)
    {
      _wb.ShowPrintPreviewDialog();
    }

    private void _tsbSpellChecker_Click(object sender, EventArgs e)
    {

      try
      {
        _spell.Text = DocHtml;
        _spell.SpellCheck();
        //_spell.SuggestionForm.FormClosing += new FormClosingEventHandler(SuggestionForm_FormClosing);
        //_spell.SuggestionForm.VisibleChanged += new EventHandler(SuggestionForm_VisibleChanged);
        _spell.SuggestionForm.OnOk += new EventHandler(SuggestionForm_OnOk);
      }
      catch (Exception ex)
      {
        Error.ShowError(ex.Message);
      }
    }

    void SuggestionForm_OnOk(object sender, EventArgs e)
    {
      _spell.SuggestionForm.OnOk -= new EventHandler(SuggestionForm_OnOk);

      if (!DocHtml.Equals(_spell.Text))
      {
        DocHtml = _spell.Text;
        if (this.OnControlChanged != null)
          this.OnControlChanged(this, EventArgs.Empty);

        this.OnTextChanged(new EventArgs());
      }
    }

    //void SuggestionForm_FormClosing(object sender, FormClosingEventArgs e)
    //{
    //  _spell.SuggestionForm.VisibleChanged -= new EventHandler(SuggestionForm_VisibleChanged);
    //}

    //void SuggestionForm_VisibleChanged(object sender, EventArgs e)
    //{
    //  _spell.SuggestionForm.VisibleChanged -= new EventHandler(SuggestionForm_VisibleChanged);
    //}

    //void _spell_ReplacedWord(object sender, ReplaceWordEventArgs e)
    //{
    //  DocHtml =
    //    DocHtml.Substring(0, e.TextIndex)
    //    + e.ReplacementWord
    //    + DocHtml.Substring(e.TextIndex + e.Word.Length);
    //}

    //void _spell_DeletedWord(object sender, SpellingEventArgs e)
    //{
    //  DocHtml =
    //    DocHtml.Substring(0, e.TextIndex)
    //    + DocHtml.Substring(e.TextIndex + e.Word.Length);
    //}

    void _spell_EndOfText(object sender, EventArgs e)
    {
      SuggestionForm_OnOk(sender, e);
    }

  }
}
