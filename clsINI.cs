using System;
using System.Text;
using System.Runtime.InteropServices;

namespace CommandAS.Tools
{
  /// <summary>
  /// ����� ��� ������ � ini �������.
  /// ��������� API ������� :
  /// 
  /// GetPrivateProfileString       � retrieve an individual value associated with a named section and key.
  /// WritePrivateProfileString     � set an individual value associated with a named section and key. 
  /// GetPrivateProfileInt          � retrieve an integer value associated with a named section and key. 
  /// WritePrivateProfileInt        � set an integer value associated with a named section and key. 
  /// GetPrivateProfileSection      � retrieve all the keys and values associated with a named section. 
  /// WritePrivateProfileSection    � set all the keys and values associated with a named section. 
  /// GetPrivateProfileSectionNames � retrieve all the section names in an INI file. 
  /// </summary>
  public class Ini
  {
    private string m_IniFile="";
    const string CONST_DLL  ="kernel32";
    const string DefaultValue=""; //<-DefaultValueKeyIfKeyNotFoundOrEmptyOrSectionNotFound->";
    const int MAX_LEN_STRING=1024;
    #region ��������� �-��� ������ � ��������
   [DllImport (CONST_DLL,CharSet=CharSet.Ansi, EntryPoint="GetPrivateProfileStringA")]
    public static extern 
      int GetString
      (
        string              SectionName, 
        string              KeyName, 
        string              Default,
        StringBuilder       ReturnBuffer, 
        int                 SizeReturnBuffer,
        string              FileName  
      );

    [ DllImport(CONST_DLL,CharSet=CharSet.Ansi, EntryPoint="GetPrivateProfileString")]
    public static extern 
      int GetBytes
      (
        string lpAppName, 
        string lpKeyName, 
        string lpDefault, 
        byte[] lpReturnedString, 
        int nSize, 
        string lpFileName
      );

    [DllImport (CONST_DLL, EntryPoint="WritePrivateProfileStringA")]
    private static extern 
      bool WriteIniKey(
      string              SectionName, 
      string              KeyName, 
      string              KeyValue,
      string              FileName  );

    [DllImport (CONST_DLL, EntryPoint="WritePrivateProfileSectionA")]
    private static extern 
      bool WriteIniSection(
      string              SectionName, 
      string              KeyName, 
      string              KeyValue,
      string              FileName  );
    #endregion
    #region �������������� ���������� � ��-�� (Property) ������
    /// <summary>
    /// ���������� ��� INI �����
    /// ��� ��������� ��������� � ���������
    /// </summary>
    public string IniFile
    {
      set
      {
        m_IniFile = value;
      }
      get
      {
        return m_IniFile;
      }
    }
    #endregion
    #region ������ �� �������� (string)
    /// <summary>
    /// ���������� �������� ��������� �����
    /// </summary>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <param name="KeyName"><c>�������� �����</c></param>
    /// <returns><c>���������� �������� ��������� �����</c></returns>
    public string GetKeyString(string SectionName, string KeyName)
    {
      return this.GetKeyString(this.IniFile, SectionName, KeyName);
    }
    /// <summary>
    /// ���������� �������� ��������� �����
    /// </summary>
    /// <param name="FileName"><c>������ ���� � ini �����</c></param>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <param name="KeyName"><c>�������� �����</c></param>
    /// <returns><c>���������� �������� ��������� �����</c></returns>
    public string GetKeyString(string FileName, string SectionName, string KeyName)
    {
      int CountCharacters =0;
      string str="";
      if (this.FileExist(FileName))
      {
        try
        {
          StringBuilder KeyValue =new StringBuilder(MAX_LEN_STRING);
          CountCharacters = GetString(SectionName, KeyName, DefaultValue, KeyValue, KeyValue.MaxCapacity, FileName);
          str=KeyValue.ToString().Trim();
          if (CountCharacters > 0 && DefaultValue!=str.Substring(0, CountCharacters))
            str = str.Substring(0, CountCharacters);
          else
            //�-� ������� �������� �� ���������
            str =DefaultValue;
        }
        catch{ str =DefaultValue; }
      }
      return str;
    }
    #endregion
    #region ������ � ������� (int � long)
    /// <summary>
    /// ���������� �������� �������� ����� (long)
    /// </summary>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <param name="KeyName"><c>�������� �����</c></param>
    /// <returns><c>���������� �������� �������� �����</c></returns>
    public long GetKeyLong(string SectionName, string KeyName)
    {
      return this.GetKeyLong(this.IniFile, SectionName, KeyName);
    }
    /// <summary>
    /// ���������� �������� �������� ����� (long)
    /// </summary>
    /// <param name="FileName"><c>������ ���� � ini �����</c></param>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <param name="KeyName"><c>�������� �����</c></param>
    /// <returns><c>���������� �������� �������� �����</c></returns>
    public long GetKeyLong(string Filename,string SectionName, string KeyName)
    {
      try
      {
        return Convert.ToInt64(this.GetKeyString(Filename, SectionName, KeyName));
      }
      catch
      {
        return 0;
      }
    }    /// <summary>
    /// ���������� �������� �������� ����� (int)
    /// </summary>
    /// <param name="FileName"><c>������ ���� � ini �����</c></param>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <param name="KeyName"><c>�������� �����</c></param>
    /// <returns><c>���������� �������� �������� �����</c></returns>
    public int GetKeyInt(string SectionName, string KeyName)
    {
      return this.GetKeyInt(this.IniFile, SectionName, KeyName);
    }
    /// <summary>
    /// ���������� �������� �������� ����� (int)
    /// </summary>
    /// <param name="FileName"><c>������ ���� � ini �����</c></param>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <param name="KeyName"><c>�������� �����</c></param>
    /// <returns><c>���������� �������� �������� �����</c></returns>
    public int GetKeyInt(string FileName, string SectionName, string KeyName)
    {
      //Optional defNum As Long = 0
      try
      {
        return Convert.ToInt32(this.GetKeyString(FileName, SectionName, KeyName));
      }
      catch
      {
        return 0;
      }
    }
    #endregion
    #region ������ � ������� (Bool)
    /// <summary>
    /// ���������� ���������� �������� �����
    /// </summary>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <param name="KeyName"><c>�������� �����</c></param>
    /// <returns><c>���������� ���������� �������� �����</c></returns>
    public bool GetKeyBoolean(string SectionName, string KeyName)
    {
      return this.GetKeyBoolean(this.IniFile,SectionName,KeyName);
    }
    /// <summary>
    /// ���������� ���������� �������� �����
    /// </summary>
    /// <param name="FileName"><c>������ ���� � ini �����</c></param>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <param name="KeyName"><c>�������� �����</c></param>
    /// <returns><c>���������� ���������� �������� �����</c></returns>
    public bool GetKeyBoolean(string FileName, string SectionName, string KeyName)
    {
      try
      {
        string characters = GetKeyString(FileName, SectionName, KeyName);
        if (characters == "" || 
          characters != "1" || 
          characters.ToLower() == "false" || 
          Convert.ToInt32(characters) == 0 )
          return false;
        else
          return true;
      }
      catch
      {
        return false;
      }
    }
    #endregion
    #region ������ � ������� ������ (StringCollection)
    /// <summary>
    /// ���������� ��������� �����
    /// � ������� ������ � ini �����
    /// </summary>
    /// <returns><c>��������� ����� �� ������ 
    /// System.Collections.Specialized.StringCollection</c></returns>
    public System.Collections.Specialized.StringCollection GetSections() 
    {
      return GetSections(this.IniFile);
    }
    /// <summary>
    /// ���������� ��������� �����
    /// � ������� ������ � ini �����
    /// </summary>
    /// <param name="filename"><c>���� � ini �����</c></param>
    /// <returns><c>��������� ����� �� ������ System.Collections.Specialized.StringCollection</c></returns>
    public System.Collections.Specialized.StringCollection GetSections(string filename) 
    {
      System.Collections.Specialized.StringCollection items = new System.Collections.Specialized.StringCollection();
      try
      {
        byte[] buffer = new byte[32768];
        int beg=0;
        string ss="";
        int bufLen = GetBytes(null,null,DefaultValue, buffer, buffer.GetUpperBound(0), filename);
        if (bufLen > 0) 
        {
          for (int i=0; i < bufLen; i++) 
          {               
            if (buffer[i] != 0) 
            {
              ss=System.Text.UnicodeEncoding.Default.GetString (buffer, beg, i-beg+1);
            }
            else 
            {
              if (ss.Length > 0) 
              {
                items.Add(ss);
                ss="";
                beg=i+1;
              }
            }
          }
        }
      }
      catch
      {
        //��������� �����-�� ������: 
        items.Clear();
      }
      return items;
    } 

    #endregion
    #region ������ � ������� � ������ (StringCollection)
    /// <summary>
    /// ���������� ��������� ����� �� ������� ������ � ������
    /// </summary>
    /// <param name="FileName"><c>������ ���� � ini �����</c></param>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <returns><c>���������� ��������� ����� System.Collections.Specialized.StringCollection</c></returns>
    public System.Collections.Specialized.StringCollection GetKeys(string FileName,string SectionName)
    {
      System.Collections.Specialized.StringCollection items = new System.Collections.Specialized.StringCollection();
      try
      {
        byte[] buffer = new byte[32768];
        int beg=0;
        string ss="";
        int bufLen = GetBytes(SectionName,null,DefaultValue, buffer, buffer.GetUpperBound(0), FileName);
        if (bufLen > 0) 
        {
          for (int i=0; i < bufLen; i++) 
          {               
            if (buffer[i] != 0) 
            {
              ss=System.Text.UnicodeEncoding.Default.GetString (buffer, beg, i-beg+1);
            }
            else 
            {
              if (ss.Length > 0) 
              {
                items.Add(ss);
                ss="";
                beg=i+1;
              }
            }
          }
        }
      }
      catch
      {
        //��������� �����-�� ������: 
        items.Clear();
      }
      return items;
    }
    /// <summary>
    /// ���������� ��������� ����� �� ������� ������ � ������
    /// </summary>
    /// <param name="SectionName"><c>�������� ������</c></param>
    /// <returns><c>���������� ��������� ����� System.Collections.Specialized.StringCollection</c></returns>
    public System.Collections.Specialized.StringCollection GetKeys(string SectionName)
    {
      return this.GetKeys(this.IniFile,SectionName);
    }
    #endregion
    #region �������� � ������� (Key...)
    public bool KeyDelete(string SectionName, string KeyName)
    {
      return this.KeyDelete(this.IniFile,SectionName, KeyName);
    }
    public bool KeyDelete(string FileName, string SectionName, string KeyName)
    {
      //' Delete the selected key
      return WriteIniKey(SectionName, KeyName, null, FileName);
    }

    public bool KeyWrite(string SectionName,string KeyName,string KeyValue)
    {
      return this.KeyWrite(this.IniFile,SectionName, KeyName, KeyValue);
    }
    public bool KeyWrite(string FileName , string SectionName, string KeyName,string KeyValue)
    {
      if (KeyValue.Length == 0)
        KeyValue = " ";
      return WriteIniKey(SectionName, KeyName, KeyValue, FileName);
    }

    #endregion
    #region �������� � �������� (Section...)
    public bool SectionWrite(string SectionName)
    {
      return this.SectionWrite(SectionName,this.IniFile);
    }
    public bool SectionWrite(string FileName,string SectionName)
    {
      return WriteIniSection(SectionName,null, null, FileName);
    }

    public bool SectionDelete(string SectionName)
    {
      return this.SectionDelete(this.IniFile, SectionName);
    }
    public bool SectionDelete(string FileName,string SectionName)
    {
      return WriteIniSection(SectionName, null, null, FileName);
    }
    #endregion
    #region �������������� �-��� ������
    private bool FileExist(string FileName)
    {
      return System.IO.File.Exists(FileName);
    }
    #endregion
  }
}
