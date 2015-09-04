#define USE_INTERNAL_SHDOCVW
// # define USE_AURIGMA_SHDOCVW

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.Diagnostics;
using System.Reflection;
using System.Drawing;

#if USE_INTERNAL_SHDOCVW

using Microsoft.InternetExplorer.Interop;

#else
#if USE_AURIGMA_SHDOCVW

using Microsoft.InternetExplorer.Interop;

#else

using SHDocVw;

#endif
#endif

// О.Михайлик, декабрь 2003
// копирайт-не копирайт, но уважение имейте.

namespace Microsoft.InternetExplorer.ActiveX
{
    [AxHost.ClsidAttribute(WebBrowserControl.ClsidString)]
    [DefaultEvent("NavigateComplete")]
    [ToolboxItem(true)]
    //[ToolboxBitmap(typeof(WebBrowserControl))]
    public class WebBrowserControl : AxHost 
    {
        internal const string ClsidString="{8856f961-340a-11d0-a96b-00c04fd705a2}";
        const int WM_SETFOCUS = 7;
        const int WM_KILLFOCUS = 8;

        #region Private AxHost fields
        private IWebBrowser2 ocx;        
        private WebBrowserEventMulticaster eventMulticaster;        
        private AxHost.ConnectionPointCookie cookie;
        #endregion
        
        public WebBrowserControl() : base(ClsidString) 
        {
        }

        #region Title property
        // original control omit this property
        string m_Title;

        [Browsable(false)]
        public string Title
        {
            get { return m_Title; }
        }
        #endregion

        #region mshtml properties
        [Browsable(false)]
        public mshtml.IHTMLDocument2 HtmlDocument
        {
            get
            {
                if( this.ocx == null )
                    return null;
                else
                    return this.Document as mshtml.IHTMLDocument2;
            }
        }

        [Browsable(false)]
        public mshtml.HTMLBodyClass HtmlBody
        {
            get
            {
                if( this.HtmlDocument==null )
                    return null;
                else
                    return this.HtmlDocument.body as mshtml.HTMLBodyClass;
            }
        }

        [Browsable(false)]
        public mshtml.HTMLWindow2Class HtmlWindow
        {
            get
            {
                if( this.HtmlDocument==null )
                    return null;
                else
                    return this.HtmlDocument.parentWindow as mshtml.HTMLWindow2Class;
            }
        }
        #endregion

				#region New Property Control
				public bool ReadOnly
				{
					get
					{
						bool ret=false;
						IHTMLDocument2 doc = (IHTMLDocument2) this.HtmlDocument; 
						if (doc != null)  
						{
							mshtml.IHTMLElement3 el = (mshtml.IHTMLElement3) doc.GetBody(); 
							ret = bool.Parse(el.contentEditable);  
						}
						return ret;
					}
					set
					{
						IHTMLDocument2 doc = (IHTMLDocument2) this.HtmlDocument; 
						if (doc != null)  
						{
							mshtml.IHTMLElement3 el = (mshtml.IHTMLElement3) doc.GetBody(); 
							el.contentEditable=value.ToString();  
						}
					}
				}
				#endregion

        #region Ctl-prefixed properties (same names as System.Control properties)

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual object CtlParent
        {
            get 
            {
                CheckActiveXState();
                return this.ocx.Parent;
            }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual object CtlContainer 
        {
            get 
            {
                CheckActiveXState();
                return this.ocx.Container;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int CtlLeft 
        {
            get 
            {
                CheckActiveXState();
                return this.ocx.Left;
            }
            set 
            {
                CheckActiveXState();
                this.ocx.Left = value;
            }
        }
        
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int CtlTop 
        {
            get 
            {
                CheckActiveXState();return this.ocx.Top;
            }
            set 
            {
                CheckActiveXState();this.ocx.Top = value;
            }
        }
        
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int CtlWidth 
        {
            get 
            {
                CheckActiveXState();
                return this.ocx.Width;
            }
            set 
            {
                CheckActiveXState();
                this.ocx.Width = value;
            }
        }
        
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int CtlHeight 
        {
            get 
            {
                CheckActiveXState();
                return this.ocx.Height;
            }
            set 
            {
                CheckActiveXState();
                this.ocx.Height = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual bool CtlVisible 
        {
            get 
            {
                CheckActiveXState();return this.ocx.Visible;
            }
            set 
            {
                CheckActiveXState();this.ocx.Visible = value;
            }
        }
        #endregion

        #region Runtime properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Application 
        {
            get 
            {
                CheckActiveXState();
                return this.ocx.Application;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Document 
        {
            get { CheckActiveXState();return this.ocx.Document; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool TopLevelContainer 
        {
            get { CheckActiveXState();return this.ocx.TopLevelContainer; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Type 
        {
            get { CheckActiveXState();return this.ocx.Type; }
        }        

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string LocationName 
        {
            get { CheckActiveXState();return this.ocx.LocationName; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string LocationUrl 
        {
            get { CheckActiveXState();return this.ocx.LocationURL; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Busy 
        {
            get { CheckActiveXState();return this.ocx.Busy; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string BrowserName 
        {
            get { CheckActiveXState();return this.ocx.Name; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual string FullName 
        {
            get { CheckActiveXState();return this.ocx.FullName; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual IntPtr HWND 
        {
            get { CheckActiveXState();return new IntPtr(this.ocx.HWND); }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual string Path 
        {
            get { CheckActiveXState();return this.ocx.Path; }
        }
        #endregion
        
        #region Appearances
        [DispId(403)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public virtual bool StatusBar 
        {
            get { CheckActiveXState();return this.ocx.StatusBar; }
            set { CheckActiveXState();this.ocx.StatusBar = value; }
        }
        
        string m_StatusText;

        [DispId(404)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string StatusText
        {
            get
            {
                CheckActiveXState();
                if( HtmlWindow!=null )
                {
                    m_StatusText=HtmlWindow.status;
                    return m_StatusText;
                }
                else
                {
                    try { return this.ocx.StatusText; }
                    catch { return m_StatusText; }
                }
            }
            set
            {
                CheckActiveXState();
                m_StatusText=value;
                if( HtmlWindow!=null )
                    HtmlWindow.status=value;
                else
                    this.ocx.StatusText = value;
            }
        }
        
        [DispId(405)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool ToolBar 
        {
            get { CheckActiveXState(); return this.ocx.ToolBar!=0; }
            set { CheckActiveXState(); this.ocx.ToolBar = value ? -1 : 0; }
        }
        
        [DispId(406)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        [Category("Appearance")]
        public virtual bool MenuBar 
        {
            get { CheckActiveXState();return this.ocx.MenuBar; }
            set { CheckActiveXState();this.ocx.MenuBar = value; }
        }
        
        [DispId(407)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Category("Appearance")]
        public virtual bool FullScreen 
        {
            get { CheckActiveXState();return this.ocx.FullScreen; }
            set { CheckActiveXState();this.ocx.FullScreen = value; }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Bindable(BindableSupport.Yes)]
        public virtual
#if USE_INTERNAL_SHDOCVW
            ReadyState
#else
#if USE_AURIGMA_SHDOCVW
            READYSTATE
#else
            tagREADYSTATE
#endif
#endif
            ReadyState 
        {
            get { CheckActiveXState();return this.ocx.ReadyState; }
        }

        [DispId(554)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]        
        [Category("Appearance")]
        public virtual bool TheaterMode 
        {
            get { CheckActiveXState(); return this.ocx.TheaterMode; }
            set { CheckActiveXState(); this.ocx.TheaterMode = value; }
        }
        
        [DispId(555)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(true)]        
        [Category("Appearance")]
        public virtual bool AddressBar 
        {
            get { CheckActiveXState(); return this.ocx.AddressBar; }
            set { CheckActiveXState(); this.ocx.AddressBar = value; }
        }


        #endregion
        

        #region Behavior properties
        [DispId(550)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Behavior")]
        public virtual bool Offline 
        {
            get { CheckActiveXState();return this.ocx.Offline; }
            set { CheckActiveXState();this.ocx.Offline = value; }
        }

        [DispId(551)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Category("Behavior")]
        public virtual bool Silent 
        {
            get { CheckActiveXState();return this.ocx.Silent; }
            set { CheckActiveXState();this.ocx.Silent = value; }
        }
        
        [DispId(552)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]        
        [Category("Behavior")]
        public virtual bool RegisterAsBrowser 
        {
            get { CheckActiveXState();return this.ocx.RegisterAsBrowser; }
            set { CheckActiveXState();this.ocx.RegisterAsBrowser = value; }
        }
        
        [DispId(553)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(true)]        
        [Category("Behavior")]
        public virtual bool RegisterAsDropTarget 
        {
            get { CheckActiveXState();return this.ocx.RegisterAsDropTarget; }
            set { CheckActiveXState();this.ocx.RegisterAsDropTarget = value; }
        }
        
        
        [DispId(556)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(true)]        
        [Category("Behavior")]
        public virtual bool Resizable 
        {
            get { CheckActiveXState(); return this.ocx.Resizable; }
            set { CheckActiveXState(); this.ocx.Resizable = value; }
        }

        #endregion
        
        #region Other events
        public event UpdatePageStatusEventHandler UpdatePageStatus;
        
        public event ClientToHostWindowEventHandler ClientToHostWindow;
        
        public event WindowSetSizeLengthEventHandler WindowSetHeight;        
        public event WindowSetSizeLengthEventHandler WindowSetWidth;        
        public event WindowSetSizeLengthEventHandler WindowSetTop;        
        public event WindowSetSizeLengthEventHandler WindowSetLeft;
        public event EventHandler CtlVisibleChanged;
        #endregion
        
        #region Behavior events
        [Category("Behavior")]  public event WindowSetResizableEventHandler WindowSetResizable;
        [Category("Behavior")]  public event WindowClosingEventHandler WindowClosing;
        [Category("Behavior")]  public event EventHandler BeforeQuit;
        [Category("Behavior")]  public event CancelEventHandler NewWindow;
        [Category("Behavior")]  public event CommandStateChangeEventHandler CommandStateChange;
        #endregion

        #region Navigation events
        [Category("Navigation")]public event BeforeNavigateEventHandler BeforeNavigate;
        [Category("Navigation")]public event EventHandler DownloadBegin;        
        [Category("Navigation")]public event ProgressChangeEventHandler ProgressChange;
        [Category("Navigation")]public event EventHandler DownloadComplete;        
        [Category("Navigation")]public event NavigateEventHandler NavigateComplete;        
        [Category("Navigation")]public event NavigateEventHandler DocumentComplete;        
        [Category("Navigation")]public event NavigateErrorEventArgsHandler NavigateError;
        [Category("Navigation")]public event CancelEventHandler FileDownload;
        [Category("Navigation")]public event SetSecureLockIconEventHandler SetSecureLockIcon;
        [Category("Navigation")]public event PrivacyImpactedStateChangeEventHandler PrivacyImpactedStateChange;
        #endregion

        #region Print events
        [Category("Print")]public event EventHandler PrintTemplateTeardown;
        [Category("Print")]public event EventHandler PrintTemplateInstantiation;
        #endregion

        #region PropertyChanged events
        [Category("Property Changed")]
        public event EventHandler TheaterModeChanged;
        [Category("Property Changed")]
        public event EventHandler FullScreenChanged;
        [Category("Property Changed")]
        public event EventHandler StatusBarChanged;
        [Category("Property Changed")]
        public event EventHandler MenuBarChanged;
        [Category("Property Changed")]
        public event EventHandler ToolBarChanged;
        [Category("Property Changed")]
        public event PropertyChangeEventHandler PropertyChange;        
        [Category("Property Changed")]
        public event EventHandler TitleChange;        
        [Category("Property Changed")]
        public event EventHandler StatusTextChange;
        #endregion

        #region Methods
        public virtual void ShowBrowserBar(ref object pvaClsid, ref object pvarShow, ref object pvarSize) 
        {
            CheckActiveXState();
            this.ocx.ShowBrowserBar(ref pvaClsid, ref pvarShow, ref pvarSize);
        }
        
        public virtual void ExecWB(
            Microsoft.InternetExplorer.Interop.OLECMDID cmdID,
            Microsoft.InternetExplorer.Interop.OLECMDEXECOPT cmdexecopt, ref object pvaIn, ref object pvaOut) 
        {
            CheckActiveXState();
            this.ocx.ExecWB(cmdID,cmdexecopt, ref pvaIn, ref pvaOut);
        }
        
        public virtual Microsoft.InternetExplorer.Interop.OLECMDF QueryStatusWB(
            Microsoft.InternetExplorer.Interop.OLECMDID cmdID) 
        {
            CheckActiveXState();
            return this.ocx.QueryStatusWB(cmdID);
        }
        
        public virtual object GetProperty(string property) 
        {
            CheckActiveXState();
            return this.ocx.GetProperty(property);
        }
        
        public virtual void PutProperty(string property, object vtValue) 
        {
            CheckActiveXState();
            this.ocx.PutProperty(property, vtValue);
        }
        
        public virtual void ClientToWindow(ref int pcx, ref int pcy) 
        {
            CheckActiveXState();
            this.ocx.ClientToWindow(ref pcx, ref pcy);
        }
        
        public virtual void Quit() 
        {
            CheckActiveXState();
            this.ocx.Quit();
        }
        
        public virtual void Stop() 
        {
            CheckActiveXState();
            this.ocx.Stop();
        }
        
        public void RefreshBrowser(int level) 
        {
            CheckActiveXState();
            object levelObj=level;
            this.ocx.Refresh2(ref levelObj);
        }
        
        public void RefreshBrowser() 
        {
            CheckActiveXState();
            this.ocx.Refresh();
        }
        
        public void Navigate(string Url)
        {
            Navigate(
                Url,
                0,
                "",
                new byte[]{},
                "" );
        }

        public void Navigate(
            string Url,
            int flags,
            string targetFrameName,
            byte[] postData,
            string headers )
        {
            CheckActiveXState();
            object flagsObj=flags;
            object targetFrameNameObj=targetFrameName;
            object headersObj=headers;
            object postDataObj=postData;

            this.ocx.Navigate(
                Url,
                ref flagsObj,
                ref targetFrameNameObj,
                ref postDataObj,
                //postData,
                ref headersObj );
        }
        
        public virtual void GoSearch() 
        {
            CheckActiveXState();
            this.ocx.GoSearch();
        }
        
        public virtual void GoHome() 
        {
            CheckActiveXState();
            this.ocx.GoHome();
        }
        
        public virtual void GoForward() 
        {
            CheckActiveXState();
            this.ocx.GoForward();
        }
        
        public virtual void GoBack() 
        {
            CheckActiveXState();
            this.ocx.GoBack();
        }

        #endregion
        
        #region CheckActiveXState() utility

        /// <summary>
        /// If it detects error, generate exception with method/property name
        /// (inferred by Reflection).
        /// </summary>
        void CheckActiveXState()
        {
            if( this.ocx == null ) 
            {
                MethodBase wrongMethod=new System.Diagnostics.StackFrame(1,false).GetMethod();
                if( wrongMethod.Name.StartsWith("get_") )
                    throw new AxHost.InvalidActiveXStateException(
                        wrongMethod.Name.Substring("get_".Length),
                        AxHost.ActiveXInvokeKind.PropertyGet );

                else if( wrongMethod.Name.StartsWith("set_") )
                    throw new AxHost.InvalidActiveXStateException(
                        wrongMethod.Name.Substring("set_".Length),
                        AxHost.ActiveXInvokeKind.PropertySet );

                else
                    throw new AxHost.InvalidActiveXStateException(
                        wrongMethod.Name,
                        AxHost.ActiveXInvokeKind.MethodInvoke );                
            }
        }
        #endregion

        #region Sink functions

        protected override void CreateSink() 
        {
            try 
            {
                this.eventMulticaster = new WebBrowserEventMulticaster(this);
                this.cookie = new AxHost.ConnectionPointCookie(this.ocx, this.eventMulticaster, typeof(DWebBrowserEvents2));
            }
            catch (System.Exception ) 
            {
            }
        }
        
        protected override void DetachSink() 
        {
            try 
            {
                this.cookie.Disconnect();
            }
            catch (System.Exception ) 
            {
            }
        }
        
        protected override void AttachInterfaces() 
        {
            try 
            {
                this.ocx = ((IWebBrowser2)(this.GetOcx()));
            }
            catch (System.Exception ) 
            {
            }
        }

        #endregion
        
        #region Raise events

        internal void RaiseOnPrivacyImpactedStateChange(object sender, PrivacyImpactedStateChangeEventArgs e) 
        {
            if ((this.PrivacyImpactedStateChange != null)) 
            {
                this.PrivacyImpactedStateChange(sender, e);
            }
        }
        
        internal void RaiseOnUpdatePageStatus(object sender, UpdatePageStatusEventArgs e) 
        {
            if ((this.UpdatePageStatus != null)) 
            {
                this.UpdatePageStatus(sender, e);
            }
        }
        
        internal void RaiseOnPrintTemplateTeardown(object sender, EventArgs e) 
        {
            if ((this.PrintTemplateTeardown != null)) 
            {
                this.PrintTemplateTeardown(sender, e);
            }
        }
        
        internal void RaiseOnPrintTemplateInstantiation(object sender, EventArgs e) 
        {
            if ((this.PrintTemplateInstantiation != null)) 
            {
                this.PrintTemplateInstantiation(sender, e);
            }
        }
        
        internal void RaiseOnNavigateError(object sender, NavigateErrorEventArgs e) 
        {
            if ((this.NavigateError != null)) 
            {
                this.NavigateError(sender, e);
            }
        }
        
        internal void RaiseOnFileDownload(object sender, CancelEventArgs e) 
        {
            if ((this.FileDownload != null)) 
            {
                this.FileDownload(sender, e);
            }
        }
        
        internal void RaiseOnSetSecureLockIcon(object sender, SetSecureLockIconEventArgs e) 
        {
            if ((this.SetSecureLockIcon != null)) 
            {
                this.SetSecureLockIcon(sender, e);
            }
        }
        
        internal void RaiseOnClientToHostWindow(object sender, ClientToHostWindowEventArgs e) 
        {
            if ((this.ClientToHostWindow != null)) 
            {
                this.ClientToHostWindow(sender, e);
            }
        }
        
        internal void RaiseOnWindowClosing(object sender, WindowClosingEventArgs e) 
        {
            if ((this.WindowClosing != null)) 
            {
                this.WindowClosing(sender, e);
            }
        }
        
        internal void RaiseOnWindowSetHeight(object sender, WindowSetSizeLengthEventArgs e) 
        {
            if ((this.WindowSetHeight != null)) 
            {
                this.WindowSetHeight(sender, e);
            }
        }
        
        internal void RaiseOnWindowSetWidth(object sender, WindowSetSizeLengthEventArgs e) 
        {
            if ((this.WindowSetWidth != null)) 
            {
                this.WindowSetWidth(sender, e);
            }
        }
        
        internal void RaiseOnWindowSetTop(object sender, WindowSetSizeLengthEventArgs e) 
        {
            if ((this.WindowSetTop != null)) 
            {
                this.WindowSetTop(sender, e);
            }
        }
        
        internal void RaiseOnWindowSetLeft(object sender, WindowSetSizeLengthEventArgs e) 
        {
            if ((this.WindowSetLeft != null)) 
            {
                this.WindowSetLeft(sender, e);
            }
        }
        
        internal void RaiseOnWindowSetResizable(object sender, WindowSetResizableEventArgs e) 
        {
            if ((this.WindowSetResizable != null)) 
            {
                this.WindowSetResizable(sender, e);
            }
        }
        
        internal void RaiseOnOnTheaterMode(object sender, EventArgs e) 
        {
            if ((this.TheaterModeChanged != null)) 
            {
                this.TheaterModeChanged(sender, e);
            }
        }
        
        internal void RaiseOnOnFullScreen(object sender, EventArgs e) 
        {
            if ((this.FullScreenChanged != null)) 
            {
                this.FullScreenChanged(sender, e);
            }
        }
        
        internal void RaiseOnOnStatusBar(object sender, EventArgs e) 
        {
            if ((this.StatusBarChanged != null)) 
            {
                this.StatusBarChanged(sender, e);
            }
        }
        
        internal void RaiseOnOnMenuBar(object sender, EventArgs e) 
        {
            if ((this.MenuBarChanged != null)) 
            {
                this.MenuBarChanged(sender, e);
            }
        }
        
        internal void RaiseOnOnToolBar(object sender, EventArgs e) 
        {
            if ((this.ToolBarChanged != null)) 
            {
                this.ToolBarChanged(sender, e);
            }
        }
        
        internal void RaiseOnOnVisible(object sender, EventArgs e) 
        {
            if ((this.CtlVisibleChanged != null)) 
            {
                this.CtlVisibleChanged(sender, e);
            }
        }
        
        internal void RaiseOnOnQuit(object sender, System.EventArgs e) 
        {
            if ((this.BeforeQuit != null)) 
            {
                this.BeforeQuit(sender, e);
            }
        }
        
        internal void RaiseOnDocumentComplete(object sender, NavigateEventArgs e) 
        {
            if ((this.DocumentComplete != null)) 
            {
                this.DocumentComplete(sender, e);
            }
        }
        
        internal void RaiseOnNavigateComplete2(object sender, NavigateEventArgs e) 
        {
            if ((this.NavigateComplete != null)) 
            {
                this.NavigateComplete(sender, e);
            }
        }
        
        internal void RaiseOnNewWindow2(object sender, CancelEventArgs e) 
        {
            if ((this.NewWindow != null)) 
            {
                this.NewWindow(sender, e);
            }
        }
        
        internal void RaiseOnBeforeNavigate2(object sender, BeforeNavigateEventArgs e) 
        {
            if ((this.BeforeNavigate != null)) 
            {
                this.BeforeNavigate(sender, e);
            }
        }
        
        internal void RaiseOnPropertyChange(object sender, PropertyChangeEventArgs e) 
        {
            if ((this.PropertyChange != null)) 
            {
                this.PropertyChange(sender, e);
            }
        }
        
        internal void RaiseOnTitleChange(object sender, EventArgs e) 
        {
            if ((this.TitleChange != null)) 
            {
                this.TitleChange(sender, e);
            }
        }
        
        internal void RaiseOnDownloadComplete(object sender, System.EventArgs e) 
        {
            if ((this.DownloadComplete != null)) 
            {
                this.DownloadComplete(sender, e);
            }
        }
        
        internal void RaiseOnDownloadBegin(object sender, System.EventArgs e) 
        {
            if ((this.DownloadBegin != null)) 
            {
                this.DownloadBegin(sender, e);
            }
        }
        
        internal void RaiseOnCommandStateChange(object sender, CommandStateChangeEventArgs e) 
        {
            if ((this.CommandStateChange != null)) 
            {
                this.CommandStateChange(sender, e);
            }
        }
        
        internal void RaiseOnProgressChange(object sender, ProgressChangeEventArgs e) 
        {
            if ((this.ProgressChange != null)) 
            {
                this.ProgressChange(sender, e);
            }
        }
        
        internal void RaiseOnStatusTextChange(object sender, EventArgs e) 
        {
            if ((this.StatusTextChange != null)) 
            {
                this.StatusTextChange(sender, e);
            }
        }

        #endregion

        #region WebBrowserEventMulticaster
        class WebBrowserEventMulticaster :
            DWebBrowserEvents2 
        {
        
            private WebBrowserControl parent;
        
            public WebBrowserEventMulticaster(WebBrowserControl parent) 
            {
                this.parent = parent;
            }
        
            public virtual void PrivacyImpactedStateChange(bool bImpacted) 
            {
                PrivacyImpactedStateChangeEventArgs privacyimpactedstatechangeEvent = new PrivacyImpactedStateChangeEventArgs(bImpacted);
                this.parent.RaiseOnPrivacyImpactedStateChange(this.parent, privacyimpactedstatechangeEvent);
            }
        
            public virtual void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone) 
            {
                UpdatePageStatusEventArgs updatepagestatusEvent = new UpdatePageStatusEventArgs(pDisp, nPage, fDone);
                this.parent.RaiseOnUpdatePageStatus(this.parent, updatepagestatusEvent);
                nPage = updatepagestatusEvent.Page;
                fDone = updatepagestatusEvent.Done;
            }
        
            public virtual void PrintTemplateTeardown(object pDisp) 
            {
                this.parent.RaiseOnPrintTemplateTeardown(this.parent, EventArgs.Empty);
            }
        
            public virtual void PrintTemplateInstantiation(object pDisp) 
            {
                this.parent.RaiseOnPrintTemplateInstantiation(this.parent, EventArgs.Empty);
            }
        
            public virtual void NavigateError(object pDisp, ref object uRL, ref object frame, ref object statusCode, ref bool cancel) 
            {
                NavigateErrorEventArgs navigateerrorEvent = new NavigateErrorEventArgs(
                    uRL as string,
                    frame as string,
                    (int)statusCode,
                    cancel );
                this.parent.RaiseOnNavigateError(this.parent, navigateerrorEvent);
                uRL = navigateerrorEvent.Url;
                frame = navigateerrorEvent.TargetFrameName;
                statusCode = navigateerrorEvent.StatusCode;
                cancel = navigateerrorEvent.Cancel;
            }
        
            public virtual void FileDownload(ref bool cancel) 
            {
                CancelEventArgs filedownloadEvent = new CancelEventArgs(cancel);
                this.parent.RaiseOnFileDownload(this.parent, filedownloadEvent);
                cancel = filedownloadEvent.Cancel;
            }
        
            public virtual void SetSecureLockIcon(int secureLockIcon) 
            {
                SetSecureLockIconEventArgs setsecurelockiconEvent = new SetSecureLockIconEventArgs(secureLockIcon);
                this.parent.RaiseOnSetSecureLockIcon(this.parent, setsecurelockiconEvent);
            }
        
            public virtual void ClientToHostWindow(ref int cX, ref int cY) 
            {
                ClientToHostWindowEventArgs clienttohostwindowEvent = new ClientToHostWindowEventArgs(cX, cY);
                this.parent.RaiseOnClientToHostWindow(this.parent, clienttohostwindowEvent);
                cX = clienttohostwindowEvent.Point.X;
                cY = clienttohostwindowEvent.Point.Y;
            }
        
            public virtual void WindowClosing(bool isChildWindow, ref bool cancel) 
            {
                WindowClosingEventArgs windowclosingEvent = new WindowClosingEventArgs(isChildWindow, cancel);
                this.parent.RaiseOnWindowClosing(this.parent, windowclosingEvent);
                cancel = windowclosingEvent.Cancel;
            }
        
            public virtual void WindowSetHeight(int height) 
            {
                WindowSetSizeLengthEventArgs windowsetheightEvent = new WindowSetSizeLengthEventArgs(height);
                this.parent.RaiseOnWindowSetHeight(this.parent, windowsetheightEvent);
            }
        
            public virtual void WindowSetWidth(int width) 
            {
                WindowSetSizeLengthEventArgs windowsetlengthEvent = new WindowSetSizeLengthEventArgs(width);
                this.parent.RaiseOnWindowSetWidth(this.parent, windowsetlengthEvent);
            }
        
            public virtual void WindowSetTop(int top) 
            {
                WindowSetSizeLengthEventArgs windowsetlengthEvent = new WindowSetSizeLengthEventArgs(top);
                this.parent.RaiseOnWindowSetTop(this.parent, windowsetlengthEvent);
            }
        
            public virtual void WindowSetLeft(int left) 
            {
                WindowSetSizeLengthEventArgs windowsetlengthEvent = new WindowSetSizeLengthEventArgs(left);
                this.parent.RaiseOnWindowSetLeft(this.parent, windowsetlengthEvent);
            }
        
            public virtual void WindowSetResizable(bool resizable) 
            {
                WindowSetResizableEventArgs windowsetresizableEvent = new WindowSetResizableEventArgs(resizable);
                this.parent.RaiseOnWindowSetResizable(this.parent, windowsetresizableEvent);
            }
        
            public virtual void OnTheaterMode(bool theaterMode) 
            {
                this.parent.RaiseOnOnTheaterMode(this.parent, EventArgs.Empty);
            }
        
            public virtual void OnFullScreen(bool fullScreen) 
            {
                this.parent.RaiseOnOnFullScreen(this.parent, EventArgs.Empty);
            }
        
            public virtual void OnStatusBar(bool statusBar) 
            {
                this.parent.RaiseOnOnStatusBar(this.parent, EventArgs.Empty);
            }
        
            public virtual void OnMenuBar(bool menuBar) 
            {
                this.parent.RaiseOnOnMenuBar(this.parent, EventArgs.Empty);
            }
        
            public virtual void OnToolBar(bool toolBar) 
            {
                this.parent.RaiseOnOnToolBar(this.parent, EventArgs.Empty);
            }
        
            public virtual void OnVisible(bool visible) 
            {
                this.parent.RaiseOnOnVisible(this.parent, EventArgs.Empty);
            }
        
            public virtual void OnQuit() 
            {
                this.parent.RaiseOnOnQuit(this.parent, EventArgs.Empty);
            }
        
            public virtual void DocumentComplete(object pDisp, ref object uRL) 
            {
                NavigateEventArgs documentcompleteEvent = new NavigateEventArgs(pDisp, uRL);
                this.parent.RaiseOnDocumentComplete(this.parent, documentcompleteEvent);
                uRL = documentcompleteEvent.Url;
            }
        
            public virtual void NavigateComplete2(object pDisp, ref object uRL) 
            {
                NavigateEventArgs navigatecomplete2Event = new NavigateEventArgs(pDisp, uRL);
                this.parent.RaiseOnNavigateComplete2(this.parent, navigatecomplete2Event);
                uRL = navigatecomplete2Event.Url;
            }
        
            public virtual void NewWindow2(ref object ppDisp, ref bool cancel) 
            {
                CancelEventArgs newwindow2Event = new CancelEventArgs(cancel);
                this.parent.RaiseOnNewWindow2(this.parent, newwindow2Event);
                cancel = newwindow2Event.Cancel;
            }

            public virtual void BeforeNavigate2(
                object pDisp,
                ref object uRL,
                ref object flags,
                ref object targetFrameName,
                ref object postData,
                ref object headers,
                ref bool cancel )           
            {
                BeforeNavigateEventArgs beforenavigate2Event = new BeforeNavigateEventArgs(
                    pDisp,
                    uRL,
                    flags,
                    targetFrameName,
                    postData as byte[]
                    ,
                    headers,
                    cancel );
                this.parent.RaiseOnBeforeNavigate2(this.parent, beforenavigate2Event);
                cancel = beforenavigate2Event.Cancel;
            }
        
            public virtual void PropertyChange(string szProperty) 
            {
                PropertyChangeEventArgs propertychangeEvent = new PropertyChangeEventArgs(szProperty);
                this.parent.RaiseOnPropertyChange(this.parent, propertychangeEvent);
            }
        
            public virtual void TitleChange(string text) 
            {
                this.parent.m_Title=text;
                this.parent.RaiseOnTitleChange(this.parent, EventArgs.Empty);
            }
        
            public virtual void DownloadComplete() 
            {
                this.parent.RaiseOnDownloadComplete(this.parent, EventArgs.Empty);
            }
        
            public virtual void DownloadBegin() 
            {
                this.parent.RaiseOnDownloadBegin(this.parent, EventArgs.Empty);
            }
        
            public virtual void CommandStateChange(int command, bool enable) 
            {
                CommandStateChangeEventArgs commandstatechangeEvent = new CommandStateChangeEventArgs(command, enable);
                this.parent.RaiseOnCommandStateChange(this.parent, commandstatechangeEvent);
            }
        
            public virtual void ProgressChange(int progress, int progressMax) 
            {
                ProgressChangeEventArgs progresschangeEvent = new ProgressChangeEventArgs(progress, progressMax);
                this.parent.RaiseOnProgressChange(this.parent, progresschangeEvent);
            }
        
            public virtual void StatusTextChange(string text) 
            {
                this.parent.m_StatusText=text;
                this.parent.RaiseOnStatusTextChange(this.parent, EventArgs.Empty);
            }
        }
        #endregion


    }
    
    #region Event delegates
    public delegate void PrivacyImpactedStateChangeEventHandler(object sender, PrivacyImpactedStateChangeEventArgs e);
    
    public class PrivacyImpactedStateChangeEventArgs 
    {
        
        public bool Impacted;
        
        public PrivacyImpactedStateChangeEventArgs(bool impacted) 
        {
            this.Impacted = impacted;
        }
    }
    
    public delegate void UpdatePageStatusEventHandler(object sender, UpdatePageStatusEventArgs e);
    
    public class UpdatePageStatusEventArgs
    {
        public object Disp;
        public object Page;
        public object Done;
        
        public UpdatePageStatusEventArgs(object pDisp, object nPage, object fDone) 
        {
            this.Disp = pDisp;
            this.Page = nPage;
            this.Done = fDone;
        }
    }
    
    public delegate void NavigateErrorEventArgsHandler(object sender, NavigateErrorEventArgs e);
    
    public class NavigateErrorEventArgs 
    {
        public string Url;
        
        public string TargetFrameName;
        
        public int StatusCode;
        
        public bool Cancel;
        
        public NavigateErrorEventArgs(string url, string targetFrame, int statusCode, bool cancel) 
        {
            this.Url = url;
            this.TargetFrameName = targetFrame;
            this.StatusCode = statusCode;
            this.Cancel = cancel;
        }
    }
    
    public delegate void SetSecureLockIconEventHandler(object sender, SetSecureLockIconEventArgs e);
    
    public class SetSecureLockIconEventArgs
    {
        public int SecureLockIcon;
        
        public SetSecureLockIconEventArgs(int secureLockIcon) 
        {
            this.SecureLockIcon = secureLockIcon;
        }
    }
    
    public delegate void ClientToHostWindowEventHandler(object sender, ClientToHostWindowEventArgs e);
    
    public class ClientToHostWindowEventArgs
    {
        public System.Drawing.Point Point;
        
        public ClientToHostWindowEventArgs(int cX, int cY) 
        {
            this.Point=new System.Drawing.Point(cX,cY);
        }
    }
    
    public delegate void WindowClosingEventHandler(object sender, WindowClosingEventArgs e);
    
    public class WindowClosingEventArgs
    {
        
        public readonly bool IsChildWindow;        
        public bool Cancel;
        
        public WindowClosingEventArgs(bool isChildWindow, bool cancel) 
        {
            this.IsChildWindow = isChildWindow;
            this.Cancel = cancel;
        }
    }
    
    public delegate void WindowSetSizeLengthEventHandler(object sender, WindowSetSizeLengthEventArgs e);
    
    public class WindowSetSizeLengthEventArgs
    {
        public int SizeLength;
        
        public WindowSetSizeLengthEventArgs(int sizeLength) 
        {
            this.SizeLength=sizeLength;
        }
    }
    
    public delegate void WindowSetResizableEventHandler(object sender, WindowSetResizableEventArgs e);
    
    public class WindowSetResizableEventArgs
    {
        public bool Resizable;
        
        public WindowSetResizableEventArgs(bool resizable) 
        {
            this.Resizable = resizable;
        }
    }
    
    public delegate void NavigateEventHandler(object sender, NavigateEventArgs e);
    
    public class NavigateEventArgs
    {
        public readonly string Url;
        
        public NavigateEventArgs(object pDisp, object uRL) 
        {
            this.Url = uRL as string;
        }
    }
    
    public delegate void BeforeNavigateEventHandler(object sender, BeforeNavigateEventArgs e);
    
    public class BeforeNavigateEventArgs
    {
        public readonly string Url;        
        public readonly string TargetFrameName;        
        public readonly byte[] PostData;        
        public readonly string Headers;
        
        public bool Cancel;
        
        public BeforeNavigateEventArgs(object pDisp, object uRL, object flags, object targetFrameName, byte[] postData, object headers, bool cancel) 
        {
            this.Url = uRL as string;
            this.TargetFrameName = targetFrameName as string;
            this.PostData = postData;
            this.Headers = headers as string;
            this.Cancel = cancel;
        }
    }
    
    public delegate void PropertyChangeEventHandler(object sender, PropertyChangeEventArgs e);
    
    public class PropertyChangeEventArgs
    {        
        public readonly string Property;
        
        public PropertyChangeEventArgs(string szProperty) 
        {
            this.Property = szProperty;
        }
    }
    
    public delegate void CommandStateChangeEventHandler(object sender, CommandStateChangeEventArgs e);
    
    public class CommandStateChangeEventArgs
    {        
        public readonly CommandStateChangeConstants Command;        
        public readonly bool Enable;
        
        public CommandStateChangeEventArgs(int command, bool enable) 
        {
            this.Command = (CommandStateChangeConstants)command;
            this.Enable = enable;
        }
    }
    
    public delegate void ProgressChangeEventHandler(object sender, ProgressChangeEventArgs e);
    
    public class ProgressChangeEventArgs
    {        
        public readonly int Progress;        
        public readonly int ProgressMax;
        
        public ProgressChangeEventArgs(int progress, int progressMax) 
        {
            this.Progress = progress;
            this.ProgressMax = progressMax;
        }
    }
    #endregion

}
