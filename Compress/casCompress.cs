using System;
using System.IO;
using ICSharpCode.SharpZipLib.BZip2;
using System.IO.Compression;


namespace CommandAS.Tools
{
  /// <summary>
	/// 
	/// </summary>
	public class casArchivator
	{

		public casArchivator()
		{
		}

    public static byte[] Compress(MemoryStream aSrcStream)
		{
			MemoryStream destStream = new MemoryStream(4096);
      GZipStream compressedzipStream = new GZipStream(destStream, CompressionMode.Compress, true);
      byte[] buffer = aSrcStream.ToArray();
      compressedzipStream.Write(buffer, 0, buffer.Length);
      compressedzipStream.Close();
      return destStream.ToArray();
		}
		public static byte[] Compress(byte[] aBt)
		{
		  MemoryStream destStream = new MemoryStream(4096);
      GZipStream compressedzipStream = new GZipStream(destStream, CompressionMode.Compress, true);
      compressedzipStream.Write(aBt, 0, aBt.Length);
      compressedzipStream.Close();
      return destStream.ToArray();
		}
		public static byte[] Compress(string aSrc)
		{
			byte[] bt = System.Text.Encoding.Default.GetBytes(aSrc);
			MemoryStream destStream = new MemoryStream(4096);
      GZipStream compressedzipStream = new GZipStream(destStream, CompressionMode.Compress, true);
      compressedzipStream.Write(bt, 0, bt.Length);
      compressedzipStream.Close();
      return destStream.ToArray();
		}

		public static string Decompress(byte[] aSrc)
		{
      return System.Text.Encoding.Default.GetString(DecompressBase(new MemoryStream(aSrc)));
		}

    public static MemoryStream Decompress(Stream aSrcStream)
    {
      return new MemoryStream(DecompressBase(aSrcStream));
    }

		public static byte [] DecompressBase(Stream aSrcStream)
		{
      // first determine the uncompressed size
      aSrcStream.Position = 0;
      GZipStream zipStream = new GZipStream(aSrcStream, CompressionMode.Decompress);
      //int offset = 0;
      int totalBytes = 0;
      byte[] smallBuffer = new byte[100];
      while (true)
      {
        int bytesRead = zipStream.Read(smallBuffer, 0, 100);
        if (bytesRead == 0)
        {
          break;
        }
        //offset += bytesRead;
        totalBytes += bytesRead;
      }
      //zipStream.Close();

      // second decompress data
      aSrcStream.Position = 0;
      zipStream = new GZipStream(aSrcStream, CompressionMode.Decompress);
      byte[] buffer = new byte[totalBytes];
      zipStream.Read(buffer, 0, totalBytes);
      zipStream.Close();

      return buffer;

      //aSrcStream.Position = 0;
      //string strResult = "";
      //int totalLength = 0;
      //byte[] writeData = new byte[4096];
      //Stream s2 = new GZipStream(aSrcStream, CompressionMode.Decompress);
      //while (true)
      //{
      //  int size = s2.Read(writeData, 0, writeData.Length);
      //  if (size > 0)
      //  {
      //    totalLength += size;
      //    strResult += System.Text.Encoding.Unicode.GetString(writeData, 0, size);
      //  }
      //  else
      //  {
      //    break;
      //  }
      //}
      //s2.Close();
      //return System.Text.Encoding.Unicode.GetBytes(strResult);
		}
	}

  public class casArchivatorZip084
  {

    public casArchivatorZip084()
    {
    }


    public static byte[] Compress(Stream aSrcStream)
    {
      MemoryStream destStream = new MemoryStream(4096);
      BZip2.Compress(aSrcStream, destStream, 4096);
      return destStream.ToArray();
    }

    /*public static byte[] Compress(MemoryStream aSrcStream)
    {
      MemoryStream destStream = new MemoryStream(4096);
      BZip2.Compress(aSrcStream, destStream, 4096);
      return destStream.ToArray();
      //return aSrcStream.ToArray();
    }*/
    public static byte[] Compress(byte[] aBt)
    {
      MemoryStream srcStream = new MemoryStream(aBt);
      MemoryStream destStream = new MemoryStream(4096);
      BZip2.Compress(srcStream, destStream, 4096);
      return destStream.ToArray();
    }
    public static byte[] Compress(string aSrc)
    {
      byte[] bt = System.Text.Encoding.Default.GetBytes(aSrc);
      MemoryStream srcStream = new MemoryStream(bt);
      MemoryStream destStream = new MemoryStream(4096);
      BZip2.Compress(srcStream, destStream, 4096);
      return destStream.ToArray();
    }



    public static string Decompress(byte[] aSrc) //unclench
    {
      MemoryStream srcStream = new MemoryStream(aSrc);
      MemoryStream destStream = new MemoryStream(4096);
      BZip2.Decompress(srcStream, destStream);
      return System.Text.Encoding.Default.GetString(destStream.ToArray());
    }

    public static MemoryStream Decompress(Stream aSrcStream) //unclench
    {
      //MemoryStream srcStream = new MemoryStream(aSrc);
      //MemoryStream destStream = null;
      //if (aSrcStream.Length < 100000)
      //{
      MemoryStream destStream = new MemoryStream(4096);
      BZip2.Decompress(aSrcStream, destStream);
      //}
      //else
      //{
      //	destStream = new MemoryStream((int)aSrcStream.Length * 50);
      //	BZip2.Decompress(aSrcStream, destStream);
      //}
      return destStream;
    }

  }
}
