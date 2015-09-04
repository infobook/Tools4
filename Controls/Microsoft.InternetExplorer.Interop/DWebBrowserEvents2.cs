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
    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
    [TypeLibType(TypeLibTypeFlags.FHidden | TypeLibTypeFlags.FDispatchable)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface DWebBrowserEvents2
    {        
        [DispId(102)]
        void StatusTextChange(
            string Text );
    
        [DispId(108)]
        void ProgressChange(
            int Progress,
            int ProgressMax );
    
        [DispId(105)]
        void CommandStateChange(
            int Command,
            bool Enable );
    
        [DispId(106)]
        void DownloadBegin();
    
        [DispId(104)]
        void DownloadComplete();
    
        [DispId(113)]
        void TitleChange(
            string Text );
    
        [DispId(112)]
        void PropertyChange(
            string szProperty );
    
        [DispId(250)]
        void BeforeNavigate2(
            [MarshalAs(UnmanagedType.IDispatch)]
            object pDisp,
            ref object URL,
            ref object Flags,
            ref object TargetFrameName,
            ref object PostData,
            ref object Headers,
            ref bool Cancel );
    
        [DispId(251)]
        void NewWindow2(
            [MarshalAs(UnmanagedType.IDispatch)]
            ref object ppDisp,
            ref bool Cancel );
    
        [DispId(252)]
        void NavigateComplete2(
            [MarshalAs(UnmanagedType.IDispatch)]
            object pDisp,
            ref object URL );
    
        [DispId(259)]
        void DocumentComplete(
            [MarshalAs(UnmanagedType.IDispatch)]
            object pDisp,
            ref object URL );
    
        [DispId(253)]
        void OnQuit();
    
        [DispId(254)]
        void OnVisible(
            bool Visible );
    
        [DispId(255)]
        void OnToolBar(
            bool ToolBar );
    
        [DispId(256)]
        void OnMenuBar(
            bool MenuBar );
    
        [DispId(257)]
        void OnStatusBar(
            bool StatusBar );
    
        [DispId(258)]
        void OnFullScreen(
            bool FullScreen );
    
        [DispId(260)]
        void OnTheaterMode(
            bool TheaterMode );
    
        [DispId(262)]
        void WindowSetResizable(
            bool Resizable );
    
        [DispId(264)]
        void WindowSetLeft(
            int Left );
    
        [DispId(265)]
        void WindowSetTop(
            int Top );
    
        [DispId(266)]
        void WindowSetWidth(
            int Width );
    
        [DispId(267)]
        void WindowSetHeight(
            int Height );
    
        [DispId(263)]
        void WindowClosing(
            bool IsChildWindow,
            ref bool Cancel );
    
        [DispId(268)]
        void ClientToHostWindow(
            ref int CX,
            ref int CY );
    
        [DispId(269)]
        void SetSecureLockIcon(
            int SecureLockIcon );
    
        [DispId(270)]
        void FileDownload(
            ref bool Cancel );
    
        [DispId(271)]
        void NavigateError(
            [MarshalAs(UnmanagedType.IDispatch)]
            object pDisp,
            ref object URL,
            ref object Frame,
            ref object StatusCode,
            ref bool Cancel );
    
        [DispId(225)]
        void PrintTemplateInstantiation(
            [MarshalAs(UnmanagedType.IDispatch)]
            object pDisp );
    
        [DispId(226)]
        void PrintTemplateTeardown(
            [MarshalAs(UnmanagedType.IDispatch)]
            object pDisp );
    
        [DispId(227)]
        void UpdatePageStatus(
            [MarshalAs(UnmanagedType.IDispatch)]
            object pDisp,
            ref object nPage,
            ref object fDone );
    
        [DispId(272)]
        void PrivacyImpactedStateChange(
            bool bImpacted );
    
    }
}

#endif