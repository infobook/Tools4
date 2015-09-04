using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace CommandAS.Tools.Barcode
{
  public class BarCodeScaner
  {
    private bool _continue;
    private SerialPort _serialPort;
    private Thread _thread;
    private Error _err;

    public ThreadState pThreadState
    {
      get { return _thread.ThreadState; }
    }

    public Error pError
    {
      get { return _err; }
    }

    public event EvH_Text BarCodeRead;

    public BarCodeScaner()
    {
      ThreadStart ts = new ThreadStart(ReadPort);
      _thread = new Thread(ts);
      _serialPort = new SerialPort();
      _err = new Error();
      _continue = false;
    }

    ~BarCodeScaner()
    {
      Finish();
    }

    public void Finish()
    {
      _continue = false;

      if (_serialPort.IsOpen)
        _serialPort.Close();

      if (_thread.ThreadState != ThreadState.Unstarted)
      {
        _thread.Join(300);
        //_thread.Abort();
      }

    }

    public void Start(string aPortName)
    {
      _continue = true;
      _err.Clear();

      if (!_serialPort.IsOpen)
      {
        _serialPort.PortName = aPortName;
        _serialPort.Open();
      }
      if (!_serialPort.PortName.Equals(aPortName))
      {
        _serialPort.Close();
        _serialPort.PortName = aPortName;
        _serialPort.Open();
      }

      if (_thread.ThreadState == ThreadState.Unstarted)
      {
        _thread.Start();
      }
      else if (_thread.ThreadState == ThreadState.Suspended)
      {
        _thread.Resume();
      }
    }

    public void Stop()
    {
      _continue = false;

      if (_serialPort.IsOpen)
        _serialPort.Close();

      if (_thread.ThreadState == ThreadState.Running)
      {
        _thread.Suspend();
      }
    }

    private void ReadPort()
    {
      while (_continue)
      {
        try
        {
          string txt = _serialPort.ReadLine();
          if (txt.Length > 0)
          {
            txt = txt.Replace("\r", string.Empty);
            //SetText(txt); // + Environment.NewLine);
            if (BarCodeRead != null)
              BarCodeRead(this, new EvA_Text(txt));
          }
        }
        catch (Exception ex)
        {
          _err.ex = ex;
        }
      }
    }

    /// “ак в форме TextBox-у передовать считанный текст, 
    /// иначе ошибка передачи данных между потоками!!!

    //void _bcs_BarCodeRead(object sender, EvA_Text e)
    //{
    //  SetText(e.pText);
    //}

    //// This delegate enables asynchronous calls for setting
    //// the text property on a TextBox control.
    //delegate void SetTextCallback(string text);

    //private void SetText(string text)
    //{
    //  // InvokeRequired required compares the thread ID of the
    //  // calling thread to the thread ID of the creating thread.
    //  // If these threads are different, it returns true.
    //  if (this._txtRead.InvokeRequired)
    //  {
    //    SetTextCallback d = new SetTextCallback(SetText);
    //    this.Invoke(d, new object[] { text });
    //  }
    //  else
    //  {
    //    this._txtRead.Text = text;
    //  }
    //}

  }
}
