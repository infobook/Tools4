#define USE_INTERNAL_SHDOCVW
// # define USE_AURIGMA_SHDOCVW

#if USE_INTERNAL_SHDOCVW

using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.InternetExplorer.Interop
{
  [Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E")]
    [TypeLibType(TypeLibTypeFlags.FHidden | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
    [DefaultMember("Name")]
    [ComImport]
  interface IWebBrowser2
    {
        [DispId(100)]
        void GoBack();
    
        [DispId(101)]
        void GoForward();
    
        [DispId(102)]
        void GoHome();
    
        [DispId(103)]
        void GoSearch();
    
        [DispId(104)]
        void Navigate(
            [In] string URL,
            [In] ref object Flags,
            [In] ref object TargetFrameName,
            [In] ref object PostData,
            [In] ref object Headers );
    
        [DispId(-550)]
        void Refresh();
    
        [DispId(105)]
        void Refresh2(
            [In] ref object Level );
    
        [DispId(106)]
        void Stop();
    
        [DispId(200)]
        object Application { [return:MarshalAs(UnmanagedType.IDispatch)] get; }
    
        [DispId(201)]
        object Parent { [return:MarshalAs(UnmanagedType.IDispatch)] get; }
    
        [DispId(202)]
        object Container { [return:MarshalAs(UnmanagedType.IDispatch)] get; }
    
        [DispId(203)]
        object Document { [return:MarshalAs(UnmanagedType.IDispatch)] get; }
    
        [DispId(204)]
        bool TopLevelContainer { get; }
    
        [DispId(205)]
        string Type { get; }
    
        [DispId(206)]
        int Left { get; set; }
    
        [DispId(207)]
        int Top { get; set; }
    
        [DispId(208)]
        int Width { get; set; }
    
        [DispId(209)]
        int Height { get; set; }
    
        [DispId(210)]
        string LocationName { get; }
    
        [DispId(211)]
        string LocationURL { get; }
    
        [DispId(212)]
        bool Busy { get; }
    
        [DispId(300)]
        void Quit();
    
        [DispId(301)]
        void ClientToWindow(
            [In,Out] ref int pcx,
            [In,Out] ref int pcy );
    
        [DispId(302)]
        void PutProperty(
            [In] string Property,
            [In] object vtValue );
    
        [DispId(303)]
        object GetProperty(
            [In] string Property );
    
        [DispId(0)]
        string Name { get; }
    
        [DispId(-515)]
        int HWND { get; }
    
        [DispId(400)]
        string FullName { get; }
    
        [DispId(401)]
        string Path { get; }
    
        [DispId(402)]
        bool Visible { get; set; }
    
        [DispId(403)]
        bool StatusBar { get; set; }
    
        [DispId(404)]
        string StatusText { [DispId(404)] get; [DispId(404)] set; }
    
        [DispId(405)]
        int ToolBar { get; set; }
    
        [DispId(406)]
        bool MenuBar { get; set; }
    
        [DispId(407)]
        bool FullScreen { get; set; }
    
        [DispId(500)]
        void Navigate2(
            [In] ref object URL,
            [In] ref object Flags,
            [In] ref object TargetFrameName,
            [In] ref object PostData,
            [In] ref object Headers );
    
        [DispId(501)]
        Microsoft.InternetExplorer.Interop.OLECMDF QueryStatusWB(
            [In] Microsoft.InternetExplorer.Interop.OLECMDID cmdID );
    
        [DispId(502)]
        void ExecWB(
            [In] Microsoft.InternetExplorer.Interop.OLECMDID cmdID,
            [In] Microsoft.InternetExplorer.Interop.OLECMDEXECOPT cmdexecopt,
            [In] ref object pvaIn,
            [In,Out] ref object pvaOut );
    
        [DispId(503)]
        void ShowBrowserBar(
            [In] ref object pvaClsid,
            [In] ref object pvarShow,
            [In] ref object pvarSize );
    
        [DispId(-525)]
        ReadyState ReadyState { get; }
    
        [DispId(550)]
        bool Offline { get; set; }
    
        [DispId(551)]
        bool Silent { get; set; }
    
        [DispId(552)]
        bool RegisterAsBrowser { get; set; }
    
        [DispId(553)]
        bool RegisterAsDropTarget { get; set; }
    
        [DispId(554)]
        bool TheaterMode { get; set; }
    
        [DispId(555)]
        bool AddressBar { get; set; }
    
        [DispId(556)]
        bool Resizable { get; set; }
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
		object CreateStyleSheet(string bstrHref, int lIndex); 
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
	#region Interface  IHTMLBody
	/*
	interface IHTMLBody
	{
		int addBehavior (string bstrUrl,ref object pvarFactory);
		void addFilter (ref object pUnk);
		mshtml.IHTMLDOMNode appendChild (newChild);
		IHTMLElement applyElement (apply,where);
		bool attachEvent (event ev,pdisp);
		void blur();
		void clearAttributes();
		Void click
		mshtml.IHTMLDOMNode cloneNode (fDeep)
		String componentFromPoint (x,y)
		Boolean contains (pChild)
		Object createControlRange
		mshtml.IHTMLTxtRange createTextRange
		Void detachEvent (event,pdisp)
		Void doScroll (component)
		Boolean dragDrop
		Boolean FireEvent (bstrEventName,pvarEventObject)
		Void focus
		String accessKey *
		Object aLink *
		Object all *
		Object attributes *
		String background *
		Object behaviorUrns *
		Object bgColor *
		String bgProperties *
		Object bottomMargin *
		Boolean canHaveChildren *
		Boolean canHaveHTML *
		Object childNodes *
		Object children *
		String className *
		Int32 clientHeight *
		Int32 clientLeft *
		Int32 clientTop *
		Int32 clientWidth *
		String contentEditable *
		mshtml.IHTMLCurrentStyle currentStyle *
		String dir *
		Boolean disabled *
		Object document *
		mshtml.IHTMLFiltersCollection filters *
		mshtml.IHTMLDOMNode firstChild *
		Int32 glyphMode *
		Boolean hideFocus *
		String id *
		Boolean inflateBlock *
		String innerHTML *
		String innerText *
		Boolean isContentEditable *
		Boolean isDisabled *
		Boolean isMultiLine *
		Boolean isTextEdit *
		String lang *
		String language *
		mshtml.IHTMLDOMNode lastChild *
		Object leftMargin *
		Object link *
		mshtml.IHTMLDOMNode nextSibling *
		String nodeName *
		Int32 nodeType *
		Object nodeValue *
		Boolean noWrap *
		Int32 offsetHeight *
		Int32 offsetLeft *
		mshtml.IHTMLElement offsetParent *
		Int32 offsetTop *
		Int32 offsetWidth *
		Object onactivate *
		Object onafterprint *
		Object onafterupdate *
		Object onbeforeactivate *
		Object onbeforecopy *
		Object onbeforecut *
		Object onbeforedeactivate *
		Object onbeforeeditfocus *
		Object onbeforepaste *
		Object onbeforeprint *
		Object onbeforeunload *
		Object onbeforeupdate *
		Object onblur *
		Object oncellchange *
		Object onclick *
		Object oncontextmenu *
		Object oncontrolselect *
		Object oncopy *
		Object oncut *
		Object ondataavailable *
		Object ondatasetchanged *
		Object ondatasetcomplete *
		Object ondblclick *
		Object ondeactivate *
		Object ondrag *
		Object ondragend *
		Object ondragenter *
		Object ondragleave *
		Object ondragover *
		Object ondragstart *
		Object ondrop *
		Object onerrorupdate *
		Object onfilterchange *
		Object onfocus *
		Object onfocusin *
		Object onfocusout *
		Object onhelp *
		Object onkeydown *
		Object onkeypress *
		Object onkeyup *
		Object onlayoutcomplete *
		Object onload *
		Object onlosecapture *
		Object onmousedown *
		Object onmouseenter *
		Object onmouseleave *
		Object onmousemove *
		Object onmouseout *
		Object onmouseover *
		Object onmouseup *
		Object onmousewheel *
		Object onmove *
		Object onmoveend *
		Object onmovestart *
		Object onpage *
		Object onpaste *
		Object onpropertychange *
		Object onreadystatechange *
		Object onresize *
		Object onresizeend *
		Object onresizestart *
		Object onrowenter *
		Object onrowexit *
		Object onrowsdelete *
		Object onrowsinserted *
		Object onscroll *
		Object onselect *
		Object onselectstart *
		Object onunload *
		String outerHTML *
		String outerText *
		Object ownerDocument *
		mshtml.IHTMLElement parentElement *
		mshtml.IHTMLDOMNode parentNode *
		mshtml.IHTMLElement parentTextEdit *
		mshtml.IHTMLDOMNode previousSibling *
		Object readyState *
		int readyStateValue *
		Object recordNumber *
		Object rightMargin *
		mshtml.IHTMLStyle runtimeStyle *
		String scopeName *
		String scroll *
		Int32 scrollHeight *
		Int32 scrollLeft *
		Int32 scrollTop *
		Int32 scrollWidth *
		Int32 sourceIndex *
		mshtml.IHTMLStyle style *
		Int16 tabIndex *
		String tagName *
		String tagUrn *
		Object text *
		String title *
		Object topMargin *
		String uniqueID *
		Int32 uniqueNumber *
		Object vLink *
		String getAdjacentText (where)
		Object getAttribute (strAttributeName,lFlags)
		mshtml.IHTMLDOMAttribute getAttributeNode (bstrName)
		mshtml.IHTMLRect getBoundingClientRect
		mshtml.IHTMLRectCollection getClientRects
		mshtml.IHTMLElementCollection getElementsByTagName (v)
		Object getExpression (propname)
		Boolean hasChildNodes
		mshtml.IHTMLElement insertAdjacentElement (where,insertedElement)
		void insertAdjacentHTML (where,html);
		void insertAdjacentText (where,text);
		mshtml.IHTMLDOMNode insertBefore (newChild,refChild)
		void mergeAttributes (mergeThis,pvarFlags)
		void normalize();
		void releaseCapture();
		Boolean removeAttribute (strAttributeName,lFlags)
		mshtml.IHTMLDOMAttribute removeAttributeNode (pattr)
		Boolean removeBehavior (cookie)
		mshtml.IHTMLDOMNode removeChild (oldChild)
		Boolean removeExpression (propname)
		Void removeFilter (pUnk)
		mshtml.IHTMLDOMNode removeNode (fDeep)
		String replaceAdjacentText (where,newText)
		mshtml.IHTMLDOMNode replaceChild (newChild,oldChild)
		mshtml.IHTMLDOMNode replaceNode (replacement)
		void scrollIntoView (varargStart);
		void setActive();
		void setAttribute (strAttributeName,AttributeValue,lFlags)
		mshtml.IHTMLDOMAttribute setAttributeNode (pattr)
		void setCapture (containerCapture)
		void setExpression (propname,expression,language)
		mshtml.IHTMLDOMNode swapNode (otherNode)
		string toString();
	}
	*/
	#endregion
}
#endif