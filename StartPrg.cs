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
    static Mutex mutex;           //Создавать экземпляры этого класса нет смысла  
    CheckOneInstance() { }

    public static bool IsInstanceExist(bool activateFirstInstance, string instanceName)
    {
      Process currentProcess = Process.GetCurrentProcess();
      bool createdNew;
      //Имя Mutex'a состоит из имени исполняемого файла + строковая константа  
      string mutexName = currentProcess.MainModule.ModuleName + " " + instanceName;
      mutex = new Mutex(false, mutexName, out createdNew);
      if (!createdNew)
      {
        if (!activateFirstInstance)
          return true;
        //получаем имя нашего процесса (название файла без расширения '.exe')    
        string processName = currentProcess.MainModule.ModuleName;
        processName = processName.Substring(0, processName.IndexOf(".exe"));
        //перебираем все процессы с искомым именем    
        foreach (Process process in Process.GetProcessesByName(processName))
        {
          //текущий экземпляр нас не интересует     
          if (process.Id == currentProcess.Id)
            continue;
          //могут быть разные приложения с одинаковым именем
          //исполняемого файла. Проверяем что-бы это был 'наш' файл
          if (process.MainModule.FileName != currentProcess.MainModule.FileName)
            continue;
          IntPtr hWnd;
          if (process.MainWindowHandle == IntPtr.Zero)
          {
            //По каким-то причинам (например, у окна ShowInTaskBar = false)     
            //MainWindowHandle равен 0. Попытаемся найти окно при помощи     
            //Win32 API функций     
            hWnd = FindWindowEngine.Find(process.Id);
            if (hWnd == IntPtr.Zero)
              break;  //Окно так и неудалось найти, выходим.    
          }
          else
            hWnd = process.MainWindowHandle;
          //Активизируем окно приложения
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

  //Этот класс предоставляет метод для поиска среди окон 'верхнего уровня'
  //окна созданного указанным процессом
  sealed public class FindWindowEngine
  {
    //Создавать экземпляры этого класса нет смысла
    FindWindowEngine() { }

    static IntPtr hWndFirstInstance;
    public static IntPtr Find(int processID)
    {
      Win32.EnumWindowsProcDelegate enumProc = new Win32.EnumWindowsProcDelegate(EnumWindowsProc);
      //Перебираем все окна 'верхнего' уровня
      Win32.EnumWindows(enumProc, (IntPtr)processID);
      return hWndFirstInstance;
    }
    static bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam)
    {
      int processIDFinded;
      //Получаем ID процесса, создавшего данный hWnd 
      Win32.GetWindowThreadProcessId(hWnd, out processIDFinded);
      if (processIDFinded == lParam.ToInt32())
      {//Нашли искомый процесс
        //сохраняем hWnd и прекращаем перебор окон
        hWndFirstInstance = hWnd;
        return false;
      }
      //продолжаем перебор окон
      return true;
    }
  }


  /*  DotGotnet.ru (форум)
    Задачу можно решить при помощи семафоров, идея такая: при "сворачивании" окна в tray, 
    создаем именованный семафор, который изначально будет в состоянии Non signaled, затем создаем поток, 
    в функции которого первым делом будет ожидаться момент освобождения только что созданного семафора, 
    далее, при его освобождении, показываем окно. Следовательно, для того, чтобы показать окно нам нужно 
    (не важно из какого потока какого процесса) освободить семафор. Теперь, когда новое приложение стартует, 
    необходимо (перед вызовом SetForegroundWindow/ShowWindow) проверить окно на видимость, 
    при помощи IsWindowVisible, и если оно невидимо, открыть semaphore и освободить его.
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
