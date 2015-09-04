using System;
using System.Collections;
using System.IO;

namespace CommandAS.Tools
{
  /// <summary>
  /// Summary description for TemporaryFiles.
  /// </summary>
  public class TemporaryFiles : DisposableType
  {

    private ArrayList _fa;

    public string pGetTempFileName
    {
      get
      {
        string fn = Path.GetTempFileName();
        _fa.Add(fn);
        return fn;
      }
    }

    public string GetTempFileName(String aFileName)
    {
      string fn = Path.GetTempPath() + aFileName;
      _fa.Add(fn);
      return fn;
    }
    //string fn = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
    //fn += Path.DirectorySeparatorChar+"f"+DateTime.Now.ToString("MMddhhmmss")+".cif";

    public TemporaryFiles()
    {
      _fa = new ArrayList(8);
    }


    public void DeleteAllFiles()
    {
      foreach (string fn in _fa)
      {
        try { File.Delete(fn); }
        catch { }
      }
      _fa.Clear();
    }

    protected override void Dispose(bool disposing)
    {
      //if( disposing )
      //{
      DeleteAllFiles();
      //}
      base.Dispose();
    }

  }
}
