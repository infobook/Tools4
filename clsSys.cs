using System;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;

namespace CommandAS.Tools
{
  public enum ShellSpecialFolderConstants
  {
    ssfDESKTOP	= 0,
    ssfPROGRAMS	= 0x2,
    ssfCONTROLS	= 0x3,
    ssfPRINTERS	= 0x4,
    ssfPERSONAL	= 0x5,
    ssfFAVORITES	= 0x6,
    ssfSTARTUP	= 0x7,
    ssfRECENT	= 0x8,
    ssfSENDTO	= 0x9,
    ssfBITBUCKET	= 0xa,
    ssfSTARTMENU	= 0xb,
    ssfDESKTOPDIRECTORY	= 0x10,
    ssfDRIVES	= 0x11,
    ssfNETWORK	= 0x12,
    ssfNETHOOD	= 0x13,
    ssfFONTS	= 0x14,
    ssfTEMPLATES	= 0x15,
    ssfCOMMONSTARTMENU	= 0x16,
    ssfCOMMONPROGRAMS	= 0x17,
    ssfCOMMONSTARTUP	= 0x18,
    ssfCOMMONDESKTOPDIR	= 0x19,
    ssfAPPDATA	= 0x1a,
    ssfPRINTHOOD	= 0x1b,
    ssfLOCALAPPDATA	= 0x1c,
    ssfALTSTARTUP	= 0x1d,
    ssfCOMMONALTSTARTUP	= 0x1e,
    ssfCOMMONFAVORITES	= 0x1f,
    ssfINTERNETCACHE	= 0x20,
    ssfCOOKIES	= 0x21,
    ssfHISTORY	= 0x22,
    ssfCOMMONAPPDATA	= 0x23,
    ssfWINDOWS	= 0x24,
    ssfSYSTEM	= 0x25,
    ssfPROGRAMFILES	= 0x26,
    ssfMYPICTURES	= 0x27,
    ssfPROFILE	= 0x28,
  }

  public enum ShowExecuteWindows
  { 
    SW_HIDE =0,
    SW_NORMAL=1,
    SW_MAXIMIZE=2,
    SW_SHOWNOACTIVATE=4,
    SW_SHOW =5,
    SW_MINIMIZE=6,
    SW_SHOWMINNOACTIVE =7,
    SW_RESTORE=9,
    SW_SHOWDEFAULT =10,
  }

  /// <summary>
  /// Summary description for Sys.
  /// </summary>
  public enum ver:int
  { 
    /// <summary>
    /// "Unknown"
    /// </summary>
    VER_PLATFORM_WIN32s        = 0,
    /// <summary>
    /// "WIN9X"
    /// </summary>
    VER_PLATFORM_WIN32_WINDOWS = 1,
    /// <summary>
    /// "NT"
    /// </summary>
    VER_PLATFORM_WIN32_NT      = 2
  }
  [ StructLayout( LayoutKind.Sequential )]
  public class OSVersionInfo 
  {
    public int VersionInfoSize;
    public int BuildNumber;
    public int MinorVersion;
    public int MajorVersion;
    public ver PlatformId;
    [ MarshalAs(UnmanagedType.ByValTStr, SizeConst=128 )]    //ByValTStr
    public string versionString;
  }
  public class LibWrap
  {
    [ DllImport( "kernel32" )]
    public static extern bool GetVersionEx( [In, Out] OSVersionInfo osvi ); // [In, Out]
    //    [ DllImport( "kernel32", EntryPoint="GetVersionEx" )] 
    //      public static extern bool GetVersionEx2( ref OSVersionInfo osvi );  
    [DllImport ("shell32.dll") ]
    public static extern int ShellExecute
      (int hwnd,              // Handle to a parent window.
      string strVerb,       // Action to be performed.
      string strFileName,   // File or object on which to execute the specified verb.
      string strParameters, // Parameters to be passed to the application.
      string strDirectory,  // Default directory.
      int nShowCmd);         // Flags.
    // Managed class demonstrates Runtime's Platform Invocation Service
    // (P/Invoke) to call unmanaged code from managed code.
    //_cdecl 

    /* от Microsoft */
    [DllImport("version.dll")]
    public static extern bool GetFileVersionInfo (string sFileName,
      int handle, int size, byte[] infoBuffer);
    [DllImport("version.dll")]
    public static extern int GetFileVersionInfoSize (string sFileName,
      out int handle);
   
    // The 3rd parameter - "out string pValue" - is automatically
    // marshaled from Ansi to Unicode:
    [DllImport("version.dll")]
    unsafe public static extern int VerQueryValue (byte[] pBlock,
      string pSubBlock, out string pValue, out uint len);
    // This VerQueryValue overload is marked with 'unsafe' because 
    // it uses a short*:
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!111
    /// Correct by M.Tor 04.04.2013
    //[DllImport("version.dll")]
    //unsafe public static extern int VerQueryValue(byte[] pBlock,
    //  string pSubBlock, out short * pValue, out uint len);
    [DllImport("version.dll")]
    unsafe public static extern int VerQueryValue (byte[] pBlock,
      string pSubBlock, out IntPtr pValue, out uint len);
    /**************************************/
  }
  //get the path to the System32 or System folder

  //  public static void SetCursorWait():this.SetCursor(System.Windows.Forms);
  public class MouseCursor
  {
    /// <summary>
    /// Устанавливаем любой курсор у мыши
    /// </summary>
    public static void SetCursor(Cursor curs)
    {
      Cursor.Current=curs;
      Cursor.Show();
    }
    /// <summary>
    /// Устанавливаем "занятый курсор" у мыши
    /// </summary>
    public static void SetCursorWait()
    {
      SetCursor(Cursors.WaitCursor);
    }
    public static void SetCursorHand()
    {
      SetCursor(Cursors.Hand);
    }    /// <summary>
    /// Восстанавливаем курсор у мыши
    /// </summary>
    public static void SetCursorDefault()
    {
      Cursor.Current=Cursors.Default;
      Cursor.Show();
    }

  }
  public class Sys
  {
    /// <summary>
    /// возвращает свободное кол-во байт на указанном диске
    /// </summary>
    /// <param name="cDrive"><c>название диска c, c:, c:\aaa.tre </c></param>
    /// <returns><c>Возвращаем количество байт (double) свободного пространства, если ошибка - 0</c></returns>
    public static double FreeSpaceDrive(string cDrive)
    {
      if (cDrive.Length>2)
      {
        //лишнее отрезаем
        cDrive=cDrive.Substring(0,2);
        if (cDrive.Substring(1,1)!=":")
          cDrive=cDrive.Substring(0,1)+":";
      }
      else if (cDrive.Length==2 && cDrive.Substring(1,1)==":")
      {
        //все верно!
      }
      else if (cDrive.Length==1)
        cDrive=cDrive+":";
      else
        return 0;
      try
      {
        cDrive="win32_logicaldisk.deviceid='"+cDrive+"'";
        ManagementObject disk = new ManagementObject(cDrive);
        disk.Get();
        return Convert.ToDouble(disk.Properties["FreeSpace"].Value);
      }
      catch
      {
        return 0;
      }
    }
    #region Version OS
    /// <summary>
    /// Вспомогательная служебная функция
    /// </summary>
    /// <param name="osvi"></param>
    private void Ver(out OSVersionInfo osvi)
    {
      osvi = new OSVersionInfo();
      osvi.VersionInfoSize = Marshal.SizeOf( osvi );
      LibWrap.GetVersionEx( osvi );
    }
    /// <value>
    /// Возвращаем платформу OS
    /// </value>
    public ver OS
    {
      get
      {
        OSVersionInfo osvi=null;
        try
        {
          Ver (out osvi);
        }
        catch{}
        return (ver)(osvi.PlatformId);
      }
    }
    /// <value>
    /// Возвращаем строку с полной версией об OS
    /// </value>
    public string OSFull
    {
      get
      {
        string str="";
        try
        {
          OSVersionInfo osvi;
          Ver (out osvi);
          switch(osvi.PlatformId)
          {
            case ver.VER_PLATFORM_WIN32_NT:      { str+="WINNT "  ;break;}
            case ver.VER_PLATFORM_WIN32_WINDOWS: { str+="WIN9X "  ;break;}
            case ver.VER_PLATFORM_WIN32s:        { str+="Unknown ";break;}
          }
          str+=osvi.BuildNumber.ToString();
          str+="."+osvi.MajorVersion.ToString()+"."+osvi.MinorVersion.ToString()+" ";
          str+=osvi.versionString;
        }
        catch{}
        return str;
      }
    }

    #endregion
		
		#region Version .Net FrameWork
		/// <summary>
		/// Возвращаем номер текущей версии .NetFrameWork
		/// </summary>
		public static Version CurrentNetFrameWork
		{
			get
			{
				Control ctrl=new Control();
				Version ver=new Version(ctrl.ProductVersion);
				ctrl.Dispose();
				return ver;
			}
		}

		/// <summary>
		/// Возвращаем, если текущяя версия .NetFrameWork >= сверенной
		/// </summary>
		/// <param name="verion"></param>
		/// <returns></returns>
		public static bool LargeVersionNetFrameWork(Version ver)
		{
			return LargeVersionNetFrameWork(ver,CurrentNetFrameWork);
		}	

		/// <summary>
		/// Возвращаем, если одна версия .NetFrameWork >= сверенной
		/// </summary>
		/// <param name="verion"></param>
		/// <returns></returns>
		public static bool LargeVersionNetFrameWork(Version ver,Version currVerr)
		{
			bool ret=(currVerr.Major>=ver.Major && 
								currVerr.Minor>=ver.Minor && 
								currVerr.Build>=ver.Build && 
								currVerr.Revision>=ver.Revision);
			return ret;
		}			
		#endregion --Version .Net FrameWork

    public static ArrayList ManagermentFromParam(string Query)
    {
      #region Win32_OperatingSystem
      /*class Win32_OperatingSystem : CIM_OperatingSystem
      {
        string BootDevice  ;
        string BuildNumber  ;
        string BuildType  ;
        string Caption  ;
        string CodeSet  ;
        string CountryCode  ;
        string CreationClassName  ;
        string CSCreationClassName  ;
        string CSDVersion  ;
        string CSName  ;
        sint16 CurrentTimeZone  ;
        boolean Debug  ;
        string Description  ;
        boolean Distributed  ;
        string EncryptionLevel  ;
        uint8 ForegroundApplicationBoost  ;
        uint64 FreePhysicalMemory  ;
        uint64 FreeSpaceInPagingFiles  ;
        uint64 FreeVirtualMemory  ;
        datetime InstallDate  ;
        datetime LastBootUpTime  ;
        datetime LocalDateTime  ;
        string Locale  ;
        string Manufacturer  ;
        uint32 MaxNumberOfProcesses  ;
        uint64 MaxProcessMemorySize  ;
        string Name  ;
        uint32 NumberOfLicensedUsers  ;
        uint32 NumberOfProcesses  ;
        uint32 NumberOfUsers  ;
        string Organization  ;
        uint32 OSLanguage  ;
        uint32 OSProductSuite  ;
        uint16 OSType  ;
        string OtherTypeDescription  ;
        string PlusProductID  ;
        string PlusVersionNumber  ;
        boolean Primary  ;
        uint8 QuantumLength  ;
        uint8 QuantumType  ;
        string RegisteredUser  ;
        string SerialNumber  ;
        uint16 ServicePackMajorVersion  ;
        uint16 ServicePackMinorVersion  ;
        uint64 SizeStoredInPagingFiles  ;
        string Status  ;
        string SystemDevice  ;
        string SystemDirectory  ;
        string SystemDrive  ;
        uint64 TotalSwapSpaceSize  ;
        uint64 TotalVirtualMemorySize  ;
        uint64 TotalVisibleMemorySize  ;
        string Version  ;
        string WindowsDirectory  ;
      };*/
      #endregion
      //Win32_OperatingSystem;
			//Win32_Service
			ArrayList ret=new ArrayList();
      try
      {
        SelectQuery query = new SelectQuery(Query); //cDrive
        // Instantiate an object searcher with this query
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(query); 
        foreach (ManagementBaseObject Var in searcher.Get())
        {
					string str=string.Empty;
					Hashtable tb=new Hashtable();
					foreach(PropertyData pVal in Var.Properties)
					{
						string val =pVal.Value as string;
						if (val!=null && val.Length>0)
							tb.Add(pVal.Name,val);
					}
					ret.Add(tb);
        }
      }
      catch{}
      return ret;
    }
    /// <value>
    /// Возвращаем путь к директории Windows, если ошибка возвращаем ""
    /// </value>
    public static string DirectoryWindows
    {
      get
      {
        //        return Normal(Environment. GetFolderPath(Environment.SpecialFolder.WinSystem);
        return (NormalDir(Environment.SystemDirectory));
      }
    }
    /// <value>
    /// Возвращаем путь к директории System, если ошибка возвращаем ""
    /// </value>
    public static string DirectorySystem
    {
      get
      {
        return NormalDir(Environment.SystemDirectory); //GetFolderPath(Environment.SpecialFolder.WinSystem);
        //        return (NormalDir(ManagermentFromParam("SystemDirectory")));
      }
    }

    /// <value>
    /// Возвращаем путь ко временной директории системы
    /// </value>
    public static string DirectoryTemp
    {
      get
      { 
        string dirName = System.IO.Path.GetTempPath();
        return (NormalDir(dirName));
      }
    }
    /// <value>
    /// Возвращаем путь ко временному файлу во временной директории системы
    /// </value>
    public static string FileTemp
    {
      get
      { 
        string fileName = System.IO.Path.GetTempFileName();
        return fileName;
      }
    }

    /// <summary>
    /// нормализуем имя директорию, вставляя в конце слэш, если его нет.
    /// </summary>
    /// <param name="dir">Входное название директории</param>
    /// <returns>Выходное название директории</returns>
    private static string NormalDir(string dir)
    {
      if (dir.Length > 1 && dir.Substring(dir.Length-1,1)!="\\")
        dir+="\\";
      return dir;
    }
    /// <summary>
    /// Запускаем программы по ассоциации !!!
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns></returns>
    static public ProcessStartInfo Run(string FileName)
    {
      try
      {
        ProcessStartInfo si=new ProcessStartInfo(FileName);

        si.UseShellExecute=true;
        si.ErrorDialog=false;
        si.CreateNoWindow=false;
        Process.Start(si);
        return si;
      }
      catch
      {
        return null;
      }
    }
    /// <summary>
    /// Запускаем программы по ассоциации !!!
    /// </summary>
    /// <param name="Programm"></param>
    /// <param name="Parameters"></param>
    /// <returns></returns>
    static public ProcessStartInfo Start(string Programm, string Parameters)
    {
      try
      {
        ProcessStartInfo si=new ProcessStartInfo(Programm,Parameters);
        si.UseShellExecute=true;
        Process.Start(si);
        return si;
      }
      catch
      {
        return null;
      }
    }
    // Main is marked with 'unsafe' because it uses pointers:
    unsafe public static string FileVersionInfo (string filename) 
    {
      string ret=string.Empty;
      try 
      {
      /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!111
      /// Correct by M.Tor 04.04.2013
      /// Для 64 разрядной версии Win это (то, что под комментариями!) не работает!!!
      /// 
        int handle = 0;
        // Figure out how much version info there is:
        int size =LibWrap.GetFileVersionInfoSize(filename,out handle);
        // Be sure to allocate a little extra space for safety:
        byte[] buffer = new byte[size+2];
        LibWrap.GetFileVersionInfo (filename, handle, size, buffer);
        //short* subBlock = null;
        IntPtr subBlock = IntPtr.Zero;
        uint len = 0;
        // Get the locale info from the version info:
        LibWrap.VerQueryValue (buffer,"\\VarFileInfo\\Translation", out subBlock, out len);
        //string spv ="\\StringFileInfo\\" + subBlock[0].ToString("X4")+ subBlock[1].ToString("X4") + "\\ProductVersion";
        int block1 = Marshal.ReadInt16(subBlock);
        int block2 = Marshal.ReadInt16(subBlock,2);
        string spv =string.Format(@"\StringFileInfo\{0:X4}{1:X4}\ProductVersion", block1, block2);
        /// 
        /// Get the ProductVersion value for this program:
        //string versionInfo;
        //LibWrap.VerQueryValue (buffer, spv,out versionInfo, out len);
        //ret = versionInfo;
        IntPtr pVersion = IntPtr.Zero;
        LibWrap.VerQueryValue(buffer, spv, out pVersion, out len);
        ret = Marshal.PtrToStringAnsi(pVersion);
      /// 
    }
#if DEBUG
      catch (Exception e) 
      {
        Console.WriteLine ("Caught unexpected exception " + e.ToString() + "\n\n" + e.Message);
      }
#else
      catch{}
#endif
          
      return ret;
    }

  }
}
