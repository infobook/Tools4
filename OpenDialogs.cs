using System;
using System.Windows.Forms;
using System.Runtime.InteropServices; 
using System.Collections;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.Remoting;

namespace CommandAS.Tools.Dialogs
{
	/// <summary>
	/// Класс для показывания слева в диалоге открытия нужных "наполнителей"
	/// Пример использования :
	/// OpenDialogPlaces o = new OpenDialogPlaces();
	/// o.Places.Add((int)OpenDialogPlaces.EnumPlaces.LastDocuments);
	/// o.Places.Add((int)OpenDialogPlaces.EnumPlaces.MyDocuments);
	/// o.Places.Add((int)OpenDialogPlaces.EnumPlaces.Favorites);
	/// o.Places.Add((int)OpenDialogPlaces.EnumPlaces.SendTo);
	/// o.Places.Add(@"c:\My Articles");
	/// o.Init();
	/// o.OpenDialog.ShowDialog();
	/// o.Reset();
	/// </summary>
	public class OpenDialogPlaces
	{
		#region Local API function

		[DllImport("advapi32.dll")]
		private static extern int RegOverridePredefKey(int hkey, int hnewKey);

		[DllImport("advapi32.dll")]
		private static extern int RegCloseKey(int hKey);

		[DllImport("advapi32.dll",CharSet=CharSet.Ansi,EntryPoint="RegCreateKeyA")] //RegCreateKeyA
		private static extern int RegCreateKey(int hKey,string lpSubKey,int phkResult);

		const int HKEY_CURRENT_USER= -1; //0x80000001;

		#endregion --Local API function

		#region sources
		/*
		 * RegCloseKey Lib "advapi32.dll" (ByVal hKey As Long) As Long
		 * Private Declare Function RegOpenKeyEx Lib "advapi32" Alias "RegOpenKeyExA" _
		(ByVal hKey As Long, ByVal lpSubKey As String, ByVal ulOptions As Long, _
		ByVal samDesired As Long, phkResult As Long) As Long

			Private Declare Function RegCreateKey Lib "advapi32.dll" Alias "RegCreateKeyA" (ByVal hKey As Long, ByVal lpSubKey As String, phkResult As Long) As Long
		
			[DllImport("myregutil.dll")]
			private static extern IntPtr InitializeRegistry();
			[DllImport("myregutil.dll")]
			private static extern int ResetRegistry(IntPtr hKey);

		public class Opp
		{
			HKEY InitializeRegistry() //APIENTRY 
			{
				HKEY hkMyCU;
				RegCreateKey(HKEY_CURRENT_USER, "Dino", &hkMyCU);
				RegOverridePredefKey(HKEY_CURRENT_USER, hkMyCU);
				return hkMyCU;
			}

			void ResetRegistry(HKEY hkMyCU)
			{
				RegOverridePredefKey(HKEY_CURRENT_USER, NULL);
				RegCloseKey(hkMyCU);
				return;
			}
		}
		*/
		// cannot inherit from OpenFileDialog (sealed)

		#endregion --sources

		#region Local vars

		private const string Key_PlacesBar = @"Software\Microsoft\Windows\CurrentVersion\Policies\ComDlg32\PlacesBar";
		private const string DINAMO_KEY="Dino";

		private RegistryKey m_fakeKey;
		private int m_overriddenKey;
		private OpenFileDialog m_openFileDialog;
		private ArrayList m_places;

		#endregion --Local vars

		public enum EnumPlaces
		{
			Desctop=0,
			IE=1,
			Programms=2,
			ControlPanel=3,
			PrintersFaxes=4,
			MyDocuments=5,
			Favorites=6,
			AutoLoading=7,
			LastDocuments,
			SendTo=9,
			RecycleBin=10,
			MainMenu=11,
			MyMusic=12
		}

		
		#region constructor-destructor

		public OpenDialogPlaces()
		{
			m_places = new ArrayList();
			m_openFileDialog = new OpenFileDialog();				
		}

		~OpenDialogPlaces()
		{
			Reset();
		}

		#endregion --constructor-destructor

		public OpenFileDialog OpenDialog
		{
			get {return m_openFileDialog;}
		}
		
		public ArrayList Places
		{
			get {return m_places;}
		}

		public void Init()
		{
			SetupFakeRegistryTree();
		}

		#region работа с реестром через API

		private int InitializeRegistry()
		{
			/*
			HKEY hkMyCU;
			RegCreateKey(HKEY_CURRENT_USER, "Dino", &hkMyCU);
			RegOverridePredefKey(HKEY_CURRENT_USER, hkMyCU);
			return hkMyCU;
			*/
			int ret=0;
			ret=RegCreateKey(HKEY_CURRENT_USER,DINAMO_KEY,ret);
			RegOverridePredefKey(HKEY_CURRENT_USER, ret);
			return ret;
		}

		private void ResetRegistry(int hkMyCU)
		{
			RegOverridePredefKey(HKEY_CURRENT_USER, 0);
			RegCloseKey(hkMyCU);
		}

		public void Reset()
		{
			ResetRegistry(m_overriddenKey);
		}

		#endregion --работа с реестром через API

		private void SetupFakeRegistryTree()
		{
			m_fakeKey = Registry.CurrentUser.CreateSubKey(DINAMO_KEY);
			m_overriddenKey = InitializeRegistry();

			// at this point, "dino" equals places key
			// write dynamic places here reading from Places
			for(int i=0; i<Places.Count; i++)
			{
				if(Places[i] != null)
				{
					RegistryKey reg = Registry.CurrentUser.CreateSubKey(Key_PlacesBar);
					reg.SetValue("Place" + i.ToString(), Places[i]);  
				}
			}
		}
	}
}
