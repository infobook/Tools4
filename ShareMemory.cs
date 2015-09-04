using System;
using System.Security.Principal;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CommandAS.Tools
{
	unsafe public class ShareMemory
	{		
		protected const string	GUIDForSharedMem = "162988D5-CB46-4f70-AB8C-4534767DDCA8";
		protected const string	GUIDForSharedMutex = "8FE2FFD8-DA5E-4c31-8DD2-11E6BD94B458";

		protected int						m_nMapping				=	0;				
		unsafe protected char*	mcharPointer		=	null;	
		unsafe protected void*	mVoidPointer		=	null;
		protected IntPtr				mIntPtrForFileView	=	IntPtr.Zero;		
		protected Mutex					mMutexForSharedMem	=	null;

		public string pLogin
		{
			get
			{
				char[] LogCharArray;	
		
				mMutexForSharedMem.WaitOne();			

				int nIndex = 0;
				LogCharArray = new char[255]; 

				while( mcharPointer[nIndex] != '\0' )
				{
					LogCharArray[nIndex] = mcharPointer[nIndex];
					++nIndex;
				}			

				string LogInString = new string(LogCharArray,0, nIndex); 
			
				mMutexForSharedMem.ReleaseMutex(); 

				return LogInString;
			}
			set
			{
				int nLength	=	value.Length;

				if(nLength > 255)
					return;
				else
				{		
					mMutexForSharedMem.WaitOne();

					char[] LogCharArray = value.ToCharArray(); 

					int nIndex ;

					for(nIndex = 0; nIndex < nLength; ++nIndex)
					{
						mcharPointer[nIndex] = LogCharArray[nIndex];
					}

					mcharPointer[nIndex] = '\0';

					mMutexForSharedMem.ReleaseMutex();
				}
			}
		}

		public string pPassWord
		{
			get
			{
				char[] LogCharArray;

				mMutexForSharedMem.WaitOne();	
			
				int nIndex = 0;
				LogCharArray = new char[255]; 

				while( mcharPointer[nIndex + 256] != '\0' )
				{
					LogCharArray[nIndex] = mcharPointer[nIndex + 256];
					++nIndex;
				}			
			

				string LogInString = new string(LogCharArray); 		
	
				mMutexForSharedMem.ReleaseMutex(); 

				return LogInString;
			}
			set
			{
				int nLength	=	value.Length;			

				if(nLength > 255)
					return;
				else
				{	
					mMutexForSharedMem.WaitOne();

					char[] PassWordCharArray = value.ToCharArray(); 

					int nIndex ;

					for(nIndex = 0; nIndex < nLength; ++nIndex)
					{
						mcharPointer[nIndex + 256] = PassWordCharArray[nIndex];
					}

					mcharPointer[nIndex + 256] = '\0';

					mMutexForSharedMem.ReleaseMutex();
				}			
			}
		}

		public ShareMemory()
		{			
			bool bFirstCeated			=	false;
			
			mMutexForSharedMem			=	new Mutex(true, GUIDForSharedMutex, out bFirstCeated);

			Trace.Assert(mMutexForSharedMem != null);		
		
			if(bFirstCeated)
				mMutexForSharedMem.ReleaseMutex();
		}

		public bool Initialize()
		{
			Trace.Assert(m_nMapping == 0);
			Trace.Assert(mIntPtrForFileView  == IntPtr.Zero);		
			Trace.Assert(mcharPointer == null);	
			Trace.Assert(mVoidPointer == null);	

			if(mIntPtrForFileView != IntPtr.Zero)
				return false;

			if(m_nMapping != 0)
				return false;

			if(mcharPointer != null)
				return false;

			if(mVoidPointer != null)
				return false;		

			if(mMutexForSharedMem == null)
				return false;
			
 
			int nMapping =  CreateFileMapping(
				(int)FileHandleValues.INVALID_HANDLE_VALUE,
				null,
				(int)SharedFileProtection.PAGE_READWRITE,
				0,
				512,
				GUIDForSharedMem
				);	
			

			Trace.Assert(nMapping != 0);
			Trace.Assert(nMapping != (int)FileHandleValues.INVALID_HANDLE_VALUE);

			if (nMapping == 0 || nMapping == (int)(FileHandleValues.INVALID_HANDLE_VALUE))
				return false;

			m_nMapping = nMapping;

			int dwResult = GetLastError();

			if((dwResult != (int)SystemErrorCodes.ERROR_ALREADY_EXISTS) &&  (dwResult != (int)SystemErrorCodes.ERROR_SUCCESS))
				return false;		

			mIntPtrForFileView  =	MapViewOfFile (
				m_nMapping,
				(int)(FileAttributes.STANDARD_RIGHTS_REQUIRED|FileAttributes.SECTION_QUERY|FileAttributes.SECTION_MAP_WRITE |FileAttributes.SECTION_MAP_READ |FileAttributes.SECTION_MAP_EXECUTE |FileAttributes.SECTION_EXTEND_SIZE),
				0,
				0,
				0
				);

			mVoidPointer		=	(void*)mIntPtrForFileView;

			Trace.Assert(mVoidPointer != null); 

			if(mVoidPointer == null)
			{
				CloseHandle(m_nMapping);
				return false;
			}

			mcharPointer	=	(char*)(mVoidPointer);		
	
			return true;
		}

		public bool UnInitialize()
		{
			Trace.Assert(mIntPtrForFileView != IntPtr.Zero);	

			if(mIntPtrForFileView == IntPtr.Zero)
				return false;

			UnmapViewOfFile(mIntPtrForFileView);

			mIntPtrForFileView	=	IntPtr.Zero;

			Trace.Assert(m_nMapping != 0);	

			if(m_nMapping == 0)
				return false;

			CloseHandle(m_nMapping);

			m_nMapping			=   0;

			mcharPointer		=	null;
			mVoidPointer		=   null;

			return true;
		}


		#region WinAPI ...
		[FlagsAttribute]
		internal enum FileAttributes
		{			
			SECTION_QUERY       = 0x0001,
			SECTION_MAP_WRITE   = 0x0002,
			SECTION_MAP_READ    = 0x0004,
			SECTION_MAP_EXECUTE = 0x0008,
			SECTION_EXTEND_SIZE = 0x0010,			
			STANDARD_RIGHTS_REQUIRED    = (0x000F0000),
			FILE_MAP_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED|SECTION_QUERY|
				SECTION_MAP_WRITE |
				SECTION_MAP_READ |
				SECTION_MAP_EXECUTE |
				SECTION_EXTEND_SIZE
		}

		internal enum SystemErrorCodes
		{
			ERROR_SUCCESS		 = 0,
			ERROR_ALREADY_EXISTS = 183
		}

		internal enum FileHandleValues
		{
			INVALID_HANDLE_VALUE = -1
		}

		internal enum SharedFileProtection : byte
		{
			PAGE_READONLY = 0x02,
			PAGE_READWRITE = 0x04
		}

		[DllImport( "kernel32.dll", SetLastError=true )]
		private static extern int CreateFileMapping( 
			int						hFile, 
			SECURITY_ATTRIBUTES		lpAttributes,
			int						flProtect,
			int						dwMaximumSizeHigh,
			int						dwMaximumSizeLow,
			string					lpName 
			);
		[DllImport( "kernel32.dll", SetLastError=true )]
		private static extern IntPtr MapViewOfFile(
			int hFileMappingObject,
			int dwDesiredAccess,
			int dwFileOffsetHigh,
			int dwFileOffsetLow,
			int dwNumberOfBytesToMap
			);

		[DllImport( "kernel32.dll", SetLastError=true )]
		private static extern IntPtr UnmapViewOfFile(IntPtr FileHandle);

		[DllImport( "kernel32.dll" )]
		private static extern bool CloseHandle(int hObject);

		[DllImport( "kernel32.dll" )]
		private static extern int GetLastError();		

		[StructLayout( LayoutKind.Sequential)]
		internal class SECURITY_ATTRIBUTES 
		{ 
			public int		nLength; 
			public object	lpSecurityDescriptor; 
			public bool		bInheritHandle; 

			public SECURITY_ATTRIBUTES()
			{
				nLength = Marshal.SizeOf( typeof( SECURITY_ATTRIBUTES ) );

				lpSecurityDescriptor = null;
				bInheritHandle		 = false;
			}
		} 

		[DllImport( "Ole32.dll" )] 
		private static extern uint CoCreateGuid(
			[Out ] out Guid clsid			
			);
		#endregion
	}
}
