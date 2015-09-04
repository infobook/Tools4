using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CommandAS.Tools.Controls
{
  #region Public Enum
    public enum OLECMDID
    {
      OLECMDID_OPEN = 1, 
      OLECMDID_NEW = 2, 
      OLECMDID_SAVE = 3, 
      OLECMDID_SAVEAS = 4, 
      OLECMDID_SAVECOPYAS = 5, 
      OLECMDID_PRINT = 6, 
      OLECMDID_PRINTPREVIEW = 7, 
      OLECMDID_PAGESETUP = 8, 
      OLECMDID_SPELL = 9, 
      OLECMDID_PROPERTIES = 10, 
      OLECMDID_CUT = 11, 
      OLECMDID_COPY = 12, 
      OLECMDID_PASTE = 13, 
      OLECMDID_PASTESPECIAL = 14, 
      OLECMDID_UNDO = 15, 
      OLECMDID_REDO = 16, 
      OLECMDID_SELECTALL = 17, 
      OLECMDID_CLEARSELECTION = 18, 
      OLECMDID_ZOOM = 19, 
      OLECMDID_GETZOOMRANGE = 20,
      OLECMDID_UPDATECOMMANDS = 21,
      OLECMDID_REFRESH = 22,
      OLECMDID_STOP = 23,
      OLECMDID_HIDETOOLBARS = 24,
      OLECMDID_SETPROGRESSMAX = 25,
      OLECMDID_SETPROGRESSPOS = 26,
      OLECMDID_SETPROGRESSTEXT = 27,
      OLECMDID_SETTITLE = 28,
      OLECMDID_SETDOWNLOADSTATE = 29,
      OLECMDID_STOPDOWNLOAD = 30 
  }

    public enum OLECMDEXECOPT
    {
      OLECMDEXECOPT_DODEFAULT = 0,
      OLECMDEXECOPT_PROMPTUSER = 1,
      OLECMDEXECOPT_DONTPROMPTUSER = 2,
      OLECMDEXECOPT_SHOWHELP = 3
    }

  #endregion
	public class HtmlControl : AxHost, IWebBrowserEvents
	{
		public const int OLEIVERB_UIACTIVATE = -4; 
		public const string ABOUT_BLANK = "about:blank"; 

		private IWebBrowser										control;
		private AxHost.ConnectionPointCookie	cookie;
		private string												url;
		private string												html;
		private string												cssStyleSheet;
		private bool													initialized;

		public event BrowserNavigateEventHandler BeforeNavigate;
		public event BrowserNavigateEventHandler NavigateComplete;

		public HtmlControl() : base("8856f961-340a-11d0-a96b-00c04fd705a2")
		{
			control				= null;
			url						= string.Empty;
			html					= string.Empty;
			cssStyleSheet = string.Empty;
			initialized		= false;
		} 

		public virtual void RaiseNavigateBlank()
		{
			RaiseNavigateComplete(ABOUT_BLANK);
		}
        
		public virtual void RaiseNavigateComplete(string url)
		{
			if (initialized)
			{
				BrowserNavigateEventArgs e = new BrowserNavigateEventArgs(url, false);
				if (NavigateComplete != null)
					NavigateComplete(this, e);
			}
		} 
        
		public virtual void RaiseBeforeNavigate(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool cancel)
		{
			if (initialized)
			{
				BrowserNavigateEventArgs e = new BrowserNavigateEventArgs(url, false);
				if (BeforeNavigate != null)
					BeforeNavigate(this, e);
				cancel = e.Cancel;
			}
		} 
        
		public string CascadingStyleSheet
		{
			get
			{
				return cssStyleSheet;
			}
			set
			{
				cssStyleSheet = value;
				ApplyCascadingStyleSheet();
			}
		}

		public string Url 
		{ 
			set
			{
				this.url = value;
				object flags = 0;
				object targetFrame = string.Empty;
				object postData = string.Empty;
				object headers = string.Empty;
				this.control.Navigate(this.url,ref flags, ref targetFrame, ref postData, ref headers);
			}
			get { return url; }
		}
        
		public string Html 
		{ 
			set
			{
				this.html = value;
				ApplyBody(html);
				this.Refresh();
			}
			get
			{
				return this.html;
			}
		}
        
		protected override void DetachSink()
		{
			try { this.cookie.Disconnect(); }
			catch  {}
		} 
        
		protected override void CreateSink()
		{
			try { this.cookie = new ConnectionPointCookie(this.GetOcx(), this, typeof(IWebBrowserEvents)); }
			catch { }
		} 
        
		protected override void AttachInterfaces()
		{
			try 
			{
				this.control = (IWebBrowser) this.GetOcx(); 
			}
			catch {}
		} 
        
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			object flags = 0;
			object targetFrame = string.Empty;
			object postData = string.Empty;
			object headers = string.Empty;
			this.control.Navigate(ABOUT_BLANK, ref flags, ref targetFrame, ref postData, ref headers);
			
			MethodInvoker mi = new MethodInvoker(this.DelayedInitialize);
			this.BeginInvoke(mi);
		} 

		public void DelayedInitialize()  
		{
			initialized = true;
			if (html == "" || html==null)
				html=ABOUT_BLANK;
			ApplyBody(html);
			UIActivate();
			ApplyCascadingStyleSheet();
		}

		//public void Print()  
		//{
		//	if (control != null)
		//	{
		//		this.control.ExecWB( 
		//	}
		//}

		public void PrintPreview()  
		{
			if (control != null)
			{
				//try
				//{
				object pvaIn = string.Empty;
				object pvaOut =string.Empty;
				//        this.control.ExecWB(OLECMDID.OLECMDID_PRINTPREVIEW, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref pvaIn, ref pvaOut);
				//this.control.ExecWB(OLECMDID.OLECMDID_PRINTPREVIEW, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER);
				//this.control.ExecWB((int) OLECMDID.OLECMDID_PRINTPREVIEW, (int) OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref pvaIn, ref pvaOut);
				//}
				//catch {}
			}
		}
		/*
		public void Navigate(string url)  
		{
			if (control != null)
			{
				//try
				//{
				object oFlags=0;
				object oTarget,oPost,oHead;
				oTarget=oPost=oHead=string.Empty;
				this.control.Navigate(url,ref oFlags,ref oTarget,ref oPost,ref oHead);
				//        this.control.ExecWB(OLECMDID.OLECMDID_PRINTPREVIEW, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER);
				//this.control.ExecWB((int) OLECMDID.OLECMDID_PRINTPREVIEW, (int) OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref pvaIn, ref pvaOut);
				//}
				//catch {}
			}
		}
*/
		void UIActivate()
		{
			/*if (this.control==null)
			{
				MessageBox.Show("Не удалось создать Browser!");
			}
			else
				this.DoVerb(OLEIVERB_UIACTIVATE);
			*/
			if (this.control!=null)
				this.DoVerb(OLEIVERB_UIACTIVATE);
		}

		void ApplyBody(string value)
		{ 
			if (control != null)
			{
				IHTMLElement el = null;
				IHTMLDocument2 doc = this.control.GetDocument();
				/*
				if (doc != null) 
				{
					UIActivate();
					object str = (object)value;
					//System.Text.StringBuilder str=new System.Text.StringBuilder(value);
					//doc.write(ref value);
					doc.write(str);
				}
				*/
				
				if (doc != null) 
				{
					UIActivate();
					try
					{
						el = doc.GetBody();
					}
#if DEBUG
					catch //(Exception ex)
					{
						el=null;
						//MessageBox.Show(ex.Message);
					}
#else
					catch
					{
						el=null;
					}
#endif
					if (el != null) 
					{
						el.SetInnerHTML(value);
						return;
					}
				}
			}
		}
    void ApplyCascadingStyleSheet()
    {
      if (control != null)
      {
        IHTMLDocument2 htmlDoc = control.GetDocument();
        if (htmlDoc != null)
        {
//          if (cssStyleSheet!=null && cssStyleSheet!="")
					try
					{
						htmlDoc.createStyleSheet(cssStyleSheet, 0);
					}
					catch {} /*(Exception ex)
					{
						MessageBox.Show(ex.Message);
					}*/
        }
      }
    }
  } 
  #region Interface IHTMLDocument2 
  [
    InterfaceType(ComInterfaceType.InterfaceIsDual), 
    ComVisible(true), 
    Guid(@"332C4425-26CB-11D0-B483-00C04FD90119")
    ]
    interface IHTMLDocument2 
    {
			//void write  ([MarshalAs(UnmanagedType.SafeArray)] params object[] psarray); //Member of mshtml.IHTMLDocument2
			//void write  (ref string psarray); //Member of mshtml.IHTMLDocument2
		//, SafeArraySubType=System.Runtime.InteropServices.VarEnum.VT_BSTR
			//void write  ([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=System.Runtime.InteropServices.VarEnum.VT_BSTR)] ref object psarray); //Member of mshtml.IHTMLDocument2
			void write(params object[] psarray); //Member of mshtml.IHTMLDocument2
			/* Что это ???*/
			void DummyWrite(int psarray); 
			void DummyWriteln(int psarray); 		
		
			[return: MarshalAs(UnmanagedType.Interface)] 
      object GetScript(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetAll(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      IHTMLElement GetBody(); 
        
			[return: MarshalAs(UnmanagedType.Interface)] 
      object GetActiveElement(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetImages(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetApplets(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetLinks(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetForms(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetAnchors(); 
        
      void SetTitle(string p); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetTitle(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetScripts(); 
        
      void SetDesignMode(string p); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetDesignMode(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetSelection(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetReadyState(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetFrames(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetEmbeds(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetPlugins(); 
        
      void SetAlinkColor(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetAlinkColor(); 
        
      void SetBgColor(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetBgColor(); 
        
      void SetFgColor(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetFgColor(); 
        
      void SetLinkColor(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetLinkColor(); 
        
      void SetVlinkColor(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetVlinkColor(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetReferrer(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetLocation(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetLastModified(); 
        
      void SetURL(string p); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetURL(); 
        
      void SetDomain(string p); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetDomain(); 
        
      void SetCookie(string p); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetCookie(); 
        
      void SetExpando(bool p); 
        
      [return: MarshalAs(UnmanagedType.Bool)] 
      bool GetExpando(); 
        
      void SetCharset(string p); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetCharset(); 
        
      void SetDefaultCharset(string p); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetDefaultCharset(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetMimeType(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetFileSize(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetFileCreatedDate(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetFileModifiedDate(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetFileUpdatedDate(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetSecurity(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetProtocol(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string GetNameProp(); 
              
      [return: MarshalAs(UnmanagedType.Interface)] 
      object Open(string URL, object name, object features, object replace); 
        
      void Close(); 
        
      void Clear(); 

			[return: MarshalAs(UnmanagedType.Bool)] 
      bool QueryCommandSupported(string cmdID); 
        
      [return: MarshalAs(UnmanagedType.Bool)] 
      bool QueryCommandEnabled(string cmdID); 
        
      [return: MarshalAs(UnmanagedType.Bool)] 
      bool QueryCommandState(string cmdID); 
        
      [return: MarshalAs(UnmanagedType.Bool)] 
      bool QueryCommandIndeterm(string cmdID); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string QueryCommandText(string cmdID); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object QueryCommandValue(string cmdID); 
        
      [return: MarshalAs(UnmanagedType.Bool)] 
      bool ExecCommand(string cmdID, bool showUI, object value); 
        
      [return: MarshalAs(UnmanagedType.Bool)] 
      bool ExecCommandShowHelp(string cmdID); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object CreateElement(string eTag); 
        
      void SetOnhelp(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnhelp(); 
        
      void SetOnclick(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnclick(); 
        
      void SetOndblclick(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOndblclick(); 
        
      void SetOnkeyup(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnkeyup(); 
        
      void SetOnkeydown(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnkeydown(); 
        
      void SetOnkeypress(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnkeypress(); 
        
      void SetOnmouseup(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnmouseup(); 
        
      void SetOnmousedown(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnmousedown(); 
        
      void SetOnmousemove(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnmousemove(); 
        
      void SetOnmouseout(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnmouseout(); 
        
      void SetOnmouseover(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnmouseover(); 
        
      void SetOnreadystatechange(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnreadystatechange(); 
        
      void SetOnafterupdate(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnafterupdate(); 
        
      void SetOnrowexit(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnrowexit(); 
        
      void SetOnrowenter(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnrowenter(); 
        
      void SetOndragstart(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOndragstart(); 
        
      void SetOnselectstart(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnselectstart(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object ElementFromPoint(int x, int y); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetParentWindow(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object GetStyleSheets(); 
        
      void SetOnbeforeupdate(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnbeforeupdate(); 
        
      void SetOnerrorupdate(object p); 
        
      [return: MarshalAs(UnmanagedType.Struct)] 
      object GetOnerrorupdate(); 
        
      [return: MarshalAs(UnmanagedType.BStr)] 
      string toString(); 
        
      [return: MarshalAs(UnmanagedType.Interface)] 
      object createStyleSheet(string bstrHref, int lIndex); 
    } 
  #endregion
	
  #region Interface IHTMLElement
  [
  ComVisible(true), 
  Guid(@"3050F1FF-98B5-11CF-BB82-00AA00BDCE0B"), 
  InterfaceType(ComInterfaceType.InterfaceIsDual)
  ]
  interface IHTMLElement 
  {
    void SetAttribute(string strAttributeName, object AttributeValue, int lFlags); 
        
    void GetAttribute(string strAttributeName, int lFlags, object[] pvars); 
        
    [return: MarshalAs(UnmanagedType.Bool)] 
    bool RemoveAttribute(string strAttributeName, int lFlags); 
        
    void SetClassName(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetClassName(); 
        
    void SetId(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetId(); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetTagName(); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    IHTMLElement GetParentElement(); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    object GetStyle(); 
        
    void SetOnhelp(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnhelp(); 
        
    void SetOnclick(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnclick(); 
        
    void SetOndblclick(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOndblclick(); 
        
    void SetOnkeydown(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnkeydown(); 
        
    void SetOnkeyup(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnkeyup(); 
        
    void SetOnkeypress(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnkeypress(); 
        
    void SetOnmouseout(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnmouseout(); 
        
    void SetOnmouseover(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnmouseover(); 
        
    void SetOnmousemove(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnmousemove(); 
        
    void SetOnmousedown(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnmousedown(); 
        
    void SetOnmouseup(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnmouseup(); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    object GetDocument(); 
        
    void SetTitle(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetTitle(); 
        
    void SetLanguage(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetLanguage(); 
        
    void SetOnselectstart(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnselectstart(); 
        
    void ScrollIntoView(object varargStart); 
        
    [return: MarshalAs(UnmanagedType.Bool)] 
    bool Contains(IHTMLElement pChild); 
        
    [return: MarshalAs(UnmanagedType.I4)] 
    int GetSourceIndex(); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetRecordNumber(); 
        
    void SetLang(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetLang(); 
        
    [return: MarshalAs(UnmanagedType.I4)] 
    int GetOffsetLeft(); 
        
    [return: MarshalAs(UnmanagedType.I4)] 
    int GetOffsetTop(); 
        
    [return: MarshalAs(UnmanagedType.I4)] 
    int GetOffsetWidth(); 
        
    [return: MarshalAs(UnmanagedType.I4)] 
    int GetOffsetHeight(); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    IHTMLElement GetOffsetParent(); 
        
    void SetInnerHTML(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetInnerHTML(); 
        
    void SetInnerText(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetInnerText(); 
        
    void SetOuterHTML(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetOuterHTML(); 
        
    void SetOuterText(string p); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string GetOuterText(); 
        
    void InsertAdjacentHTML(string where, string html); 
        
    void InsertAdjacentText(string where, string text); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    IHTMLElement GetParentTextEdit(); 
        
    [return: MarshalAs(UnmanagedType.Bool)] 
    bool GetIsTextEdit(); 
        
    void Click(); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    object GetFilters(); 
        
    void SetOndragstart(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOndragstart(); 
        
    [return: MarshalAs(UnmanagedType.BStr)] 
    string toString(); 
        
    void SetOnbeforeupdate(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnbeforeupdate(); 
        
    void SetOnafterupdate(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnafterupdate(); 
        
    void SetOnerrorupdate(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnerrorupdate(); 
        
    void SetOnrowexit(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnrowexit(); 
        
    void SetOnrowenter(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnrowenter(); 
        
    void SetOndatasetchanged(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOndatasetchanged(); 
        
    void SetOndataavailable(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOndataavailable(); 
        
    void SetOndatasetcomplete(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOndatasetcomplete(); 
        
    void SetOnfilterchange(object p); 
        
    [return: MarshalAs(UnmanagedType.Struct)] 
    object GetOnfilterchange(); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    object GetChildren(); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    object GetAll(); 
  } 
  #endregion
  #region Interface IWebBrowser
/*
  [Guid(@"B722BCCB-4E68-101B-A2BC-00AA00404770")]
  interface IOleCommandTarget 
  {
/*
  Methods in VTable Order 
  IUnknown Methods Description 
  QueryInterface Returns pointers to supported interfaces. 
  AddRef Increments reference count. 
  Release Decrements reference count. 

  IOleCommandTarget Methods Description 
  QueryStatus Queries object for status of commands 
  Exec Execute a command 
*/
/*
//    void IOleCommandTarget();

    [return: MarshalAs(UnmanagedType.Interface)] 
    object QueryStatus(OLECMDID olecmd, OLECMDEXECOPT olecmdtext);

  }
*/
/*
  interface IUnknown 
  {
    [return: MarshalAs(UnmanagedType.Interface)] 
    object QueryInterface();

    [return: MarshalAs(UnmanagedType.U4)] 
    uint AddRef();

    [return: MarshalAs(UnmanagedType.U4)] 
    uint Release();
  }
*/
  [Guid(@"eab22ac1-30c1-11cf-a7eb-0000c05bae0b")] 
  interface IWebBrowser 
  {
/*
//    [return: MarshalAs(UnmanagedType.Interface)] 
    HRESULT ExecWB(          OLECMDID cmdID,
      OLECMDEXECOPT cmdexecopt,
      VARIANT* pvaIn,
      VARIANT* pvaOut
      );
*/
//    void ExecWB(OLECMDID olecmd,OLECMDEXECOPT oleexe,ref object pIin, ref object oOut);
    void GoBack(); 
    void GoForward(); 
    void GoHome(); 
    void GoSearch(); 
    void Navigate(string Url, ref object Flags, ref object targetFrame, ref object postData, ref object headers); 
    void Refresh(); 
    void Refresh2(); 
    void Stop(); 
    void GetApplication(); 
    void GetParent(); 
    void GetContainer(); 
        
    [return: MarshalAs(UnmanagedType.Interface)] 
    IHTMLDocument2 GetDocument(); 
  } 

  #endregion
  #region Interface IWebBrowserEvents
  [
  Guid(@"eab22ac2-30c1-11cf-a7eb-0000c05bae0b"), 
  InterfaceType(ComInterfaceType.InterfaceIsIDispatch)
  ]
  public interface IWebBrowserEvents 
  {
    [DispId(100)] 
    void RaiseBeforeNavigate(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool cancel); 
        
    [DispId(101)] 
    void RaiseNavigateComplete(string url); 
  } 

  #endregion
  #region Delegates
  public delegate void BrowserNavigateEventHandler(object s, BrowserNavigateEventArgs e);

  public class BrowserNavigateEventArgs : CancelEventArgs
  {
    private string url;
        
    public BrowserNavigateEventArgs(string url, bool cancel)
      : base(cancel)
    {
      this.url = url;
    }
        
    public string Url 
    { 
      get
      {
        return this.url;
      } 
    }
  } 
  #endregion
} 

