using System;
using System.IO;
//using ICSharpCode.SharpZipLib.BZip2;

namespace CommandAS.Tools
{
	/// <summary>
	///  ласс работы с массивом бинарных данных.
	/// ѕервые три байта массива определ€ют формат:
	/// RTF - Rich Text Format;
	/// HTM	- HTML формат;
	/// BZ2 - bzip2 основан на Burrows Wheeler Transform - это обратимый алгоритм 
	///   перестановки символов во входном потоке, 
	///		позвол€ющий эффективно сжать полученный в результате преобразовани€ блок данных.
	///		ƒомашн€€ страница: http://www.bzip2.org/ јвтор: Julian Seward
	///	B64	- Base64
	///	
	///	FUTURE make crypto:
	///	GST - GOST
	///	DES
	///	RC2
	///	TDE - TripleDES
	///	RDL - Rijndael
	/// </summary>
	
	public enum eBinaryFormat
	{
		Undefined		= 0,
		Text				= 1,
		Rtf					= 2,
		Html				= 3,

		BZip2				= 10,
    GZip        = 12,

		Base64			= 20
	}

	public class CasBinary
	{
		protected const	string _TXT			="TXT";
		protected const	string _RTF			="RTF";
		protected const	string _HTM			="HTM";
		protected const	string _BZ2			="BZ2";
		protected const	string _B64			="B64";

		//protected byte[]				mBA;
		//public byte[]						pByteArr
		//{
		//	set {mBA = value;}
		//	get {return mBA; }
		//}

		public eBinaryFormat						pFormat;

		public CasBinary()
		{
			pFormat = eBinaryFormat.Undefined;
		}

		//public byte[] TranslateTo(byte[] aSrc, eFormat aFormat)
		//{
		//	pFormat = aFormat;
		//	return TranslateTo(aSrc);
		//}
		public byte[] TranslateTo(byte[] aSrc, eBinaryFormat aFormat)
		{
			if (aSrc == null || aSrc.Length == 0)
				return new Byte[0];

			MemoryStream retStream = TranslateTo(new MemoryStream(aSrc), aFormat);
			if (retStream != null)
				return retStream.ToArray();
			else
				return new Byte[0];
		}
		public MemoryStream TranslateTo(MemoryStream aSrc, eBinaryFormat aFormat)
		{
			MemoryStream retStream = aSrc;
			pFormat = aFormat;

			switch (aFormat)
			{
				case eBinaryFormat.Text:
					retStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(_TXT));
					aSrc.WriteTo(retStream);
					break;
				case eBinaryFormat.Rtf:
					retStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(_RTF));
					aSrc.WriteTo(retStream);
					break;
				case eBinaryFormat.Html:
					retStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(_HTM));
					aSrc.WriteTo(retStream);
					break;
				case eBinaryFormat.BZip2:
          retStream = new MemoryStream(casArchivator.Compress(aSrc));
					//retStream = new MemoryStream(4096);
					//retStream.Write(System.Text.Encoding.Default.GetBytes(_BZ2), 0, 3);
					//MemoryStream destStream = new MemoryStream(4096);
					//BZip2.Compress(aSrc, destStream, 4096);
					//byte[] bt = destStream.ToArray();
					//retStream.Write(bt, 0, bt.Length);
					break;
				case eBinaryFormat.Base64:
					retStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(
						_B64 + Convert.ToBase64String(aSrc.ToArray())));
					break;
			}
			return retStream;
		}

		public string TranslateToBase64(byte[] aSrc)
		{
			return _B64 + Convert.ToBase64String(aSrc);
		}
		public string TranslateToRTF(string aSrc)
		{
			return _RTF + aSrc;
		}
		public string TranslateToHTML(string aSrc)
		{
			return _HTM + aSrc;
		}
		//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//--//

		public MemoryStream TranslateFrom(MemoryStream aSrc)
		{
			pFormat = eBinaryFormat.Undefined;
			MemoryStream retStream = TranslateFromOneStep(aSrc);
			eBinaryFormat lastFormat = pFormat;
			while (pFormat != eBinaryFormat.Undefined)
			{
				lastFormat = pFormat;
				retStream = TranslateFromOneStep(retStream);
			}
			pFormat = lastFormat;

			return retStream;
		}

		public MemoryStream TranslateFromOneStep(MemoryStream aSrc)
		{
			MemoryStream retStream = aSrc;

			byte[] bt = new Byte[3];
			aSrc.Position = 0;
			aSrc.Read(bt,0,3);
      string fmt = System.Text.Encoding.Default.GetString(bt);
			switch (fmt)
			{
				case _TXT:
					pFormat = eBinaryFormat.Text;
					retStream = new MemoryStream(aSrc.ToArray(), 3, (int)aSrc.Length-3, false);
					break;
				case _RTF:
					pFormat = eBinaryFormat.Rtf;
					retStream = new MemoryStream(aSrc.ToArray(), 3, (int)aSrc.Length-3, false);
					break;
				case _HTM:
					pFormat = eBinaryFormat.Html;
					//retStream = new MemoryStream(aSrc.ToArray(), 3, (int)aSrc.Length-3, false);
					aSrc.WriteTo(retStream);
					break;
				case _BZ2:
					pFormat = eBinaryFormat.BZip2;
          retStream = casArchivatorZip084.Decompress(aSrc);
          //aSrc = new MemoryStream(aSrc.ToArray(), 3, (int)aSrc.Length-3, false);
					//retStream = new MemoryStream(4096);
					//BZip2.Decompress(aSrc, retStream);
					break;
				case "BZh":
					pFormat = eBinaryFormat.BZip2;
          aSrc.Position = 0;
          retStream = casArchivatorZip084.Decompress(aSrc);
          //retStream = new MemoryStream(4096);
					//aSrc.Position = 0;
					//BZip2.Decompress(aSrc, retStream);
					break;
				case _B64:
					pFormat = eBinaryFormat.Base64;
					retStream = new MemoryStream(Convert.FromBase64String(aSrc.ToString().Substring(3)));
					break;
				default:
					pFormat = eBinaryFormat.Undefined;
					break;
			}

      if (pFormat == eBinaryFormat.Undefined)
      {
        if (bt[0] == 31 && bt[1] == 139)
        {
          pFormat = eBinaryFormat.GZip;
          retStream = casArchivator.Decompress(aSrc);
        }
      }
			return retStream;

		}

		public byte[] TranslateFromBase64(string aSrc)
		{
			string fmt = aSrc.Substring(0,3);
			if (fmt.Equals(_B64))
			{
				pFormat = eBinaryFormat.Base64;
				return Convert.FromBase64String(aSrc.Substring(3));
			}
			else
			{
				pFormat = eBinaryFormat.Undefined;
				return System.Text.Encoding.Default.GetBytes(aSrc);
			}
		}
		public string TranslateFromText(string aSrc)
		{
			string fmt = aSrc.Substring(0,3);
			if (fmt.Equals(_TXT))
			{
				pFormat = eBinaryFormat.Text;
				return aSrc.Substring(3);
			}
			else
			{
				pFormat = eBinaryFormat.Undefined;
				return aSrc;
			}
		}
		public string TranslateFromRTF(string aSrc)
		{
			string fmt = aSrc.Substring(0,3);
			if (fmt.Equals(_RTF))
			{
				pFormat = eBinaryFormat.Rtf;
				return aSrc.Substring(3);
			}
			else
			{
				pFormat = eBinaryFormat.Undefined;
				return aSrc;
			}
		}
		public string TranslateFromHTML(string aSrc)
		{
			string fmt = aSrc.Substring(0,3);
			if (fmt.Equals(_HTM))
			{
				pFormat = eBinaryFormat.Html;
				return aSrc.Substring(3);
			}
			else
			{
				pFormat = eBinaryFormat.Undefined;
				return aSrc;
			}
		}
	}
}
