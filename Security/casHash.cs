using System;
using System.Security.Cryptography;

namespace CommandAS.Tools.Security
{

  /// <summary>
  /// Класс casHash предназначен для синтезирования Hash для .
  /// - Персоны (алгоритм 1)
  /// - Авто (пока не опреден)
  /// - возможно для чего то еще (пароль ??)
  /// </summary>
  public class CasHash
  {
    private string mHash = string.Empty;

    public CasHash()
    {
    }
    /// <summary>
    /// Поддержка старых ф-ций (при генерации и записи Hash)
    /// </summary>
    /// <param name="aBuf"></param>
    /// <returns></returns>

    static public string HashBuf(string aBuf)
    {
      return aBuf.GetHashCode().ToString();
    }

    //static public int HashBuf(byte[] aBuf)
    //{
    //  string str=string.Empty;
    //  for(int i=0;i<aBuf.Length;i++)
    //    str+=aBuf[i].ToString()+"\n";
    //  return str.GetHashCode();
    //}

    /// <summary>
    /// Функция для отработки Hash по 4 параметрам:
    /// - для Персоны (Ф,И,О,дата рождения в формате dd.mm.yyyy)
    /// </summary>
    /// <param name="Param1"></param>
    /// <param name="Param2"></param>
    /// <param name="Param3"></param>
    /// <param name="Param4"></param>
    /// <returns></returns>
    static public byte[] Hash1(string Param1, string Param2, string Param3, string Param4)
    {
      //      HashAlgorithm ha=new HMACSHA1();
      //************************* 1 
      /*
            byte[] key = new byte[KEY_SIZE];
            byte[] data = new byte[DATA_SIZE];

            HMACSHA1 hmac = new HMACSHA1(key);
            CryptoStream cs = new CryptoStream(Stream.Null, shaM, CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.Close();

            byte[] result = shaM.Hash;
      */
      //************************* 2
      string input = string.Empty;
      if (Param1 != null && Param1.Length > 0)
        input += Param1.ToLower() + "\n";
      else
        input += "null\n";

      if (Param2 != null && Param2.Length > 0)
        input += Param2.ToLower() + "\n";
      else
        input += "null\n";

      if (Param3 != null && Param3.Length > 0)
        input += Param3.ToLower() + "\n";
      else
        input += "null\n";

      if (Param4 != null && Param4.Length > 0)
        input += Param4.ToLower() + "\n";
      else
        input += "null\n";
      //берем байты !!!
      byte[] data = System.Text.UnicodeEncoding.Default.GetBytes(input);

      //SHA1 sha = new SHA1CryptoServiceProvider(); 
      MD5 md5 = new MD5CryptoServiceProvider();

      byte[] result = md5.ComputeHash(data);
      //      string ret=System.Text.Encoding.Default.GetString (result);

      return result;
    }
    /// <summary>
    /// По массиву строк возвращаем Hash MD5
    /// </summary>
    /// <param name="Params"></param>
    /// <returns></returns>
    static public byte[] HashMD5byte(string[] Params)
    {
      byte[] ret = new byte[] { };
      if (Params == null && Params.Length == 0)
        return ret;

      string input = string.Empty;
      foreach (string param in Params)
        if (param != null && param.Length > 0)
          input += param.ToLower() + "\n";

      MD5 md5 = new MD5CryptoServiceProvider();
      byte[] data = System.Text.UnicodeEncoding.Default.GetBytes(input);
      byte[] result = md5.ComputeHash(data);
      return result;
    }
    static public string HashMD5(string[] Params)
    {
      return System.Text.Encoding.Default.GetString(HashMD5byte(Params));
    }
    static public string HashMD5hex(string[] Params)
    {
      return Converters.toHex(HashMD5(Params));
    }
    static public string HashMD5hex(string Param)
    {
      return Converters.toHex(HashMD5(new string[] { Param }));
    }

    static public string GetPasswordHash(string aPwd)
    {
      return GetHashSHA1(aPwd);
    }
    static public string GetPasswordHashHex(string aPwd)
    {
      return Converters.toHex(GetPasswordHash(aPwd));
    }

    static public string GetHashMD5(string aParam)
    {
      MD5 md = new MD5CryptoServiceProvider();
      byte[] data = System.Text.UnicodeEncoding.Default.GetBytes(aParam);
      byte[] res = md.ComputeHash(data);
      return System.Text.Encoding.Default.GetString(res);
    }
    static public string GetHashMD5Hex(string aParam)
    {
      MD5 md = new MD5CryptoServiceProvider();
      byte[] data = System.Text.UnicodeEncoding.Default.GetBytes(aParam);
      return Converters.toHex(md.ComputeHash(data));
    }

    static public string GetHashSHA1(string aParam)
    {
      SHA1 sh = new SHA1CryptoServiceProvider();
      byte[] data = System.Text.UnicodeEncoding.Default.GetBytes(aParam);
      byte[] res = sh.ComputeHash(data);
      return System.Text.Encoding.Default.GetString(res);
    }
    static public string GetHashSHA1Hex(string aParam)
    {
      SHA1 sh = new SHA1CryptoServiceProvider();
      byte[] data = System.Text.UnicodeEncoding.Default.GetBytes(aParam);
      return Converters.toHex(sh.ComputeHash(data));
    }
  }
}
