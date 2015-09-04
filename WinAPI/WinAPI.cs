using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CommandAS.Tools.WinAPI
{
	public class Kernel32_DLL
	{
		private Kernel32_DLL(){}

		[DllImport("kernel32.dll", ExactSpelling=true)]
		public static extern IntPtr GlobalLock( IntPtr handle );
		
		[DllImport("kernel32.dll", ExactSpelling=true)]
		public static extern IntPtr GlobalFree( IntPtr handle );

		[DllImport("kernel32.dll", CharSet=CharSet.Auto) ]
		public static extern void OutputDebugString( string outstr );
	
		[DllImport("kernel32.dll", ExactSpelling=true)]
		public static extern IntPtr GlobalAlloc( int flags, int size );

		[DllImport("kernel32.dll", ExactSpelling=true)]
		public static extern bool GlobalUnlock( IntPtr handle );
	}

	public class User32_DLL
	{
		private User32_DLL(){}

		[DllImport("user32.dll", ExactSpelling=true)]
		public static extern int GetMessagePos();

		[DllImport("user32.dll", ExactSpelling=true)]
		public static extern int GetMessageTime();

		[DllImport("user32.dll",EntryPoint="GetDC")]
		public static extern IntPtr GetDC(IntPtr ptr);
	}

	public class GDI32_DLL
	{
		private GDI32_DLL(){}

		[DllImport("gdi32.dll",EntryPoint="BitBlt")]
		public static extern bool BitBlt(IntPtr hdcDest,int xDest,
			int yDest,int wDest,int hDest,IntPtr hdcSource,
			int xSrc,int ySrc,int RasterOp);

		[DllImport ("gdi32.dll",EntryPoint="CreateCompatibleBitmap")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
			int nWidth, int nHeight);

		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreateDC( string szdriver, string szdevice, string szoutput, IntPtr devmode );

		[DllImport ("gdi32.dll",EntryPoint="CreateCompatibleDC")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll",EntryPoint="DeleteDC")]
		public static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll",EntryPoint="DeleteObject")]
		public static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll", ExactSpelling=true)]
		public static extern int GetDeviceCaps( IntPtr hDC, int nIndex );

		[DllImport ("gdi32.dll",EntryPoint="SelectObject")]
		public static extern IntPtr SelectObject(IntPtr hdc,IntPtr bmp);

		[DllImport("gdi32.dll", ExactSpelling=true)]
		public static extern int SetDIBitsToDevice( IntPtr hdc, int xdst, int ydst,
			int width, int height, int xsrc, int ysrc, int start, int lines,
			IntPtr bitsptr, IntPtr bmiptr, int color );
	}

	public class GDIPlus_DLL
	{
		private GDIPlus_DLL(){}

		[DllImport("gdiplus.dll", ExactSpelling=true)]
		internal static extern int GdipCreateBitmapFromGdiDib( IntPtr bminfo, IntPtr pixdat, ref IntPtr image );

		[DllImport("gdiplus.dll", ExactSpelling=true, CharSet=CharSet.Unicode)]
		internal static extern int GdipSaveImageToFile( IntPtr image, string filename, [In] ref Guid clsid, IntPtr encparams );

		[DllImport("gdiplus.dll", ExactSpelling=true, CharSet=CharSet.Unicode)]
    internal static extern int GdipSaveImageToStream(IntPtr image, System.Runtime.InteropServices.ComTypes.IStream sm, [In] ref Guid clsid, IntPtr encparams);

		[DllImport("gdiplus.dll", ExactSpelling=true)]
		internal static extern int GdipDisposeImage( IntPtr image );

	}

  public class Win32
  {
    //Делегат для функции EnumWindows
    public delegate bool EnumWindowsProcDelegate(IntPtr hWnd, IntPtr lParam);
    [DllImport("User32.dll")]
    public static extern bool EnumWindows(EnumWindowsProcDelegate lpEnumFunc, IntPtr lParam);
    [DllImport("User32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
    [DllImport("User32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);
    [DllImport("User32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("User32.dll")]
    public static extern bool IsIconic(IntPtr hWnd);
    [DllImport("User32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    public const int SW_SHOW = 2;		//2,6,7-good

    [DllImport("Kernel32.dll")]
    public static extern IntPtr CreateSemaphore(IntPtr lpsa, int cSemInitial, int cSemMax, string szSemName);
    [DllImport("Kernel32.dll")]
    public static extern IntPtr OpenSemaphore(int fdwAccess, bool fInherit, string lpszName);
    [DllImport("Kernel32.dll")]
    public static extern bool ReleaseSemaphore(IntPtr hSemaphore, int cRelease, out int lplPreviouse);

    public const int SW_RESTORE = 9;
    public const int SEMAPHORE_ALL_ACCESS = 0x001F0003;
    /*
        [DllImport("shell32.Dll")]
        public static extern bool Shell_NotifyIcon(
          NotifyCommand command,
          [In,Out] ref NOTIFYICONDATA data
          );
    */
    [DllImport("Ctfutb.h", SetLastError = true)]
    public static extern bool SetIcon
      (IntPtr hIcon);

    [DllImport("shell32.Dll", SetLastError = true)]
    public static extern bool Shell_NotifyIcon
      (int dwMessage,
      ref NOTIFYICONDATA lpdata
      );
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NOTIFYICONDATA		//  изначально было с unsafe!!!
    {
      public uint cbSize;
      public IntPtr hWnd;
      public uint uID;
      public uint uFlags;
      public uint uCallbackMessage;
      public IntPtr hIcon;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]		//  или 64
      public string szTip;
      public uint dwState;
      public uint dwStateMask;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
      public string szInfo;
      public uint uTimeout;
      public uint uVersion;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string szInfoTitle;
      public uint dwInfoFlags;
      public Guid guidItem;
    }
    public enum NotifyFlags
    {
      NIF_MESSAGE = 0x00000001,
      NIF_ICON = 0x00000002,
      NIF_TIP = 0x00000004,
      NIF_STATE = 0x00000008,
      NIF_INFO = 0x00000010,
      NIF_GUID = 0x00000020,
      NIM_ADD = 0x00000000,
      NIM_MODIFY = 0x00000001,
      NIM_DELETE = 0x00000002,
      NIM_SETFOCUS = 0x00000003
    };
  }

}

