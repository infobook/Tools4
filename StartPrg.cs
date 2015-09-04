using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CommandAS.Tools.WinAPI;

namespace CommandAS.Tools
{

  sealed public class CheckOneInstance
  {
    static Mutex mutex;           //��������� ���������� ����� ������ ��� ������  
    CheckOneInstance() { }

    public static bool IsInstanceExist(bool activateFirstInstance, string instanceName)
    {
      Process currentProcess = Process.GetCurrentProcess();
      bool createdNew;
      //��� Mutex'a ������� �� ����� ������������ ����� + ��������� ���������  
      string mutexName = currentProcess.MainModule.ModuleName + " " + instanceName;
      mutex = new Mutex(false, mutexName, out createdNew);
      if (!createdNew)
      {
        if (!activateFirstInstance)
          return true;
        //�������� ��� ������ �������� (�������� ����� ��� ���������� '.exe')    
        string processName = currentProcess.MainModule.ModuleName;
        processName = processName.Substring(0, processName.IndexOf(".exe"));
        //���������� ��� �������� � ������� ������    
        foreach (Process process in Process.GetProcessesByName(processName))
        {
          //������� ��������� ��� �� ����������     
          if (process.Id == currentProcess.Id)
            continue;
          //����� ���� ������ ���������� � ���������� ������
          //������������ �����. ��������� ���-�� ��� ��� '���' ����
          if (process.MainModule.FileName != currentProcess.MainModule.FileName)
            continue;
          IntPtr hWnd;
          if (process.MainWindowHandle == IntPtr.Zero)
          {
            //�� �����-�� �������� (��������, � ���� ShowInTaskBar = false)     
            //MainWindowHandle ����� 0. ���������� ����� ���� ��� ������     
            //Win32 API �������     
            hWnd = FindWindowEngine.Find(process.Id);
            if (hWnd == IntPtr.Zero)
              break;  //���� ��� � ��������� �����, �������.    
          }
          else
            hWnd = process.MainWindowHandle;
          //������������ ���� ����������
          if (!Win32.IsWindowVisible(hWnd))
          {
            IntPtr hSemaphore = Win32.OpenSemaphore(Win32.SEMAPHORE_ALL_ACCESS, false, instanceName);
            int previouseCount;
            Win32.ReleaseSemaphore(hSemaphore, 1, out previouseCount);
            break;
          }
          if (Win32.IsIconic(hWnd))
            Win32.ShowWindow(hWnd, Win32.SW_RESTORE);
          else
            Win32.SetForegroundWindow(hWnd);
          break;
        }
        return true;
      }
      return false;
    }
  }

  //���� ����� ������������� ����� ��� ������ ����� ���� '�������� ������'
  //���� ���������� ��������� ���������
  sealed public class FindWindowEngine
  {
    //��������� ���������� ����� ������ ��� ������
    FindWindowEngine() { }

    static IntPtr hWndFirstInstance;
    public static IntPtr Find(int processID)
    {
      Win32.EnumWindowsProcDelegate enumProc = new Win32.EnumWindowsProcDelegate(EnumWindowsProc);
      //���������� ��� ���� '��������' ������
      Win32.EnumWindows(enumProc, (IntPtr)processID);
      return hWndFirstInstance;
    }
    static bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam)
    {
      int processIDFinded;
      //�������� ID ��������, ���������� ������ hWnd 
      Win32.GetWindowThreadProcessId(hWnd, out processIDFinded);
      if (processIDFinded == lParam.ToInt32())
      {//����� ������� �������
        //��������� hWnd � ���������� ������� ����
        hWndFirstInstance = hWnd;
        return false;
      }
      //���������� ������� ����
      return true;
    }
  }


  /*  DotGotnet.ru (�����)
    ������ ����� ������ ��� ������ ���������, ���� �����: ��� "������������" ���� � tray, 
    ������� ����������� �������, ������� ���������� ����� � ��������� Non signaled, ����� ������� �����, 
    � ������� �������� ������ ����� ����� ��������� ������ ������������ ������ ��� ���������� ��������, 
    �����, ��� ��� ������������, ���������� ����. �������������, ��� ����, ����� �������� ���� ��� ����� 
    (�� ����� �� ������ ������ ������ ��������) ���������� �������. ������, ����� ����� ���������� ��������, 
    ���������� (����� ������� SetForegroundWindow/ShowWindow) ��������� ���� �� ���������, 
    ��� ������ IsWindowVisible, � ���� ��� ��������, ������� semaphore � ���������� ���.
  */
  public class Semaphore : WaitHandle
  {
    public Semaphore(int initialCount, int maximumCount, string name)
    {
      Handle = Win32.CreateSemaphore(IntPtr.Zero, initialCount, maximumCount, name);
    }
    public static IntPtr Open(string name)
    {
      return Win32.OpenSemaphore(Win32.SEMAPHORE_ALL_ACCESS, false, name);
    }
    public int Release(int countRelease)
    {
      int previouseCount;
      Win32.ReleaseSemaphore(Handle, countRelease, out previouseCount);
      return previouseCount;
    }
  }
}
