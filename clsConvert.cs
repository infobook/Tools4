using System;
using System.Text;
using System.Collections;

namespace CommandAS.Tools
{
	/// <summary>
	/// Класс clsConvert для перевода форматов данных (rtf <-> HTML <-> TXT).
	/// </summary>
	public class Converters
	{
		/*struct CodeList
		{
			public string Code;
			public string Status; //P=Pending;A=Active;G=Paragraph;D=Dead;K=Killed
			//"Dead" means the code is active but will be killed at next text
			//"Pending" means it//s waiting for text - if the code is canceled before text appears it will be killed
			//"Active" means there is text using the code at this moment
			//"Paragraph" means that the code stays active until the next paragraph: "/pard" or "/pntext"
		}*/
		#region Отображение и конвертирование строк и байтов в Hex и наоборот
		#region конвертируем в Hex

		/// <summary>
		/// Кодируем в 16-е представление (каждый символ - 2 байта)
		/// </summary>
		/// <param name="source">Массив байт</param>
		/// <returns></returns>
		static public string toHex(byte[] src)
		{
			return toHex(src, string.Empty);
		}
		static public string toHex(int src)
		{
			return toHex(new byte[]{Convert.ToByte(src)}, string.Empty);
		}

		static public string toHex(byte[] src, string aDelim)
		{
			string ret=string.Empty;
			if (src!=null && src.Length>0)
			{
				for (int i=0;i<src.Length;i++)
				{
					string s=System.Uri.HexEscape((char)src[i]);
					ret+=s.Substring(1,2)+aDelim; //отрезаем % в начале
				}
				ret=ret.ToLower();
			}
			if (aDelim.Length > 0 && ret.Length > aDelim.Length)
				ret = ret.Substring(0, ret.Length - aDelim.Length);

			return ret;
		}

		static public ulong toLong(byte[] src)
		{
			ulong ret=0;
			ulong[] b=new ulong[src.Length];
			if (src!=null && src.Length>0)
			{
				for (int i=0;i<src.Length;i++)
				{
					ulong bt=(ulong)src[i];
					bt <<= i*4;
					b[i]=bt;
					ret +=bt; ;
				}
			}
			return ret;
		}

		static public byte[] toByte(ulong src,int sizeArray)
		{
			byte[] ret=new byte[sizeArray];
			for (int i=0;i<sizeArray;i++)
			{
				ulong btn=src >> (sizeArray-i-1)*4;
				ret[i]=System.Convert.ToByte(btn & 0xff);
				src-=btn;
			}
			return ret;
		}

		static public string toChars(byte[] src)
		{
			string ret=string.Empty;
			if (src!=null && src.Length>0)
				ret = System.Text.Encoding.ASCII.GetString(src);
			return ret;
		}

		/// <summary>
		/// Кодируем в 16-е представление (каждый символ - 2 байта)
		/// </summary>
		/// <param name="source">Строка</param>
		/// <returns></returns>
		static public string toHex(string source)
		{
			byte[] src=System.Text.Encoding.Default.GetBytes(source);
			return toHex(src);
		}
		static public string toHex(string source, string aDelim)
		{
			byte[] src=System.Text.Encoding.Default.GetBytes(source);
			return toHex(src, aDelim);
		}
		#endregion
		#region Конвертируем из Hex
		/// <summary>
		/// Возращаем строку из 16-ого представления (каждый символ - 2 байта)
		/// </summary>
		/// <param name="source">Строка</param>
		/// <returns></returns>
		static public byte[] fromHexA(string src)
		{
			if (src!=null && src.Length>0)
			{
				byte[] ret=new byte[src.Length/2];
				int i=0;
				for (int j=0;j<src.Length;)
				{
					char c=System.Uri.HexUnescape("%"+src.Substring(j,2),ref j);
					ret[i]=(byte)c; //отрезаем % в начале
					i++;
				}
				return ret;
			}
			else
				return null;
		}

		static public string fromHex(string source)
		{
			byte[] ret= fromHexA(source);
			return System.Text.Encoding.Default.GetString(ret);
		}
		static public int fromHex2(string source)
		{
			int ret=0;
			source=source.ToLower();
			if(source.Length/2!=(int)(source.Length/2))
				source="0"+source;

			for(int i=0;i<source.Length;i=i+2)
			{
				int currL=intFromChar(source[i]);
				ret+=currL>>8;
				int currH=intFromChar(source[i+1]);
				ret+=currH>>16;
			}
			return ret;
		}
		static private int intFromChar(char curr)
		{
			int ret=0;
			switch(curr)
			{
				case '1': ret=1;	break;
				case '2': ret=2;	break;
				case '3': ret=3;	break;
				case '4': ret=4;	break;
				case '5': ret=5;	break;
				case '6': ret=6;	break;
				case '7': ret=7;	break;
				case '8': ret=8;	break;
				case '9': ret=9;	break;
				case 'a': ret=10;	break;
				case 'b': ret=11;	break;
				case 'c': ret=12;	break;
				case 'd': ret=13;	break;
				case 'e': ret=14;	break;
				case 'f': ret=15;	break;
			}
			return ret;
		}
		#endregion

		static public string fromUTF8_Win1251(string source)
		{
			string ret=string.Empty;
			if (source!=null && source.Length>0)
			{
				byte[] bb = System.Text.Encoding.Default.GetBytes(source);
				ret = System.Text.Encoding.UTF8.GetString(bb);
			}
			return ret;
		}
		/// <summary>
		/// Кодируем в Base64
		/// </summary>
		/// <param name="source">Входная строка</param>
		/// <returns>Выходная строка</returns>
		static public string toBase64(string source)
		{
			if (source.Length==0)
				return source;

			System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
			byte[] theBytes = enc.GetBytes( source );
			string buf= Convert.ToBase64String( theBytes );

			//      string buf = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(source));
			//      if (buf.Length>0 && buf.Substring(buf.Length-1,1)=="=")
			//убираем в конце признак кодировки =
			//        buf=buf.Substring(0,buf.Length-1);
			return buf;
		}
		/// <summary>
		/// Раскодируем из Base64
		/// </summary>
		/// <param name="source">Входная строка</param>
		/// <returns>Выходная строка</returns>
		static public string fromBase64(string source)
		{
			if (source.Length<2)
				return source;

			//      if (source.Length>0 && source.Substring(source.Length-1,1)!="=")
			//вставляем в конце признак кодировки =
			//        source+="=";
			byte[] theBytes=null;
			System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
			try
			{
				theBytes = Convert.FromBase64String(source);
			}
			catch{}
			if (theBytes==null) //если ошибка
			{
				try
				{
					theBytes = Convert.FromBase64String(source+"=");
				}
				catch{}
			}
			string buf = enc.GetString( theBytes);
			return buf;
		}
		/// <summary>
		/// Формируем строку для последующей работы кликов
		/// </summary>
		/// <param name="place"></param>
		/// <param name="code"></param>
		/// <param name="type"></param>
		/// <param name="path"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string madeURL(int place,int code,int type,string path,string name)
		{
			return "&place=" + place
				+"&code=" + code 
				+"&type=" + type
				+"&path=" + path
				+"&name=" + Converters.toBase64(name)
				+"&=";
		}
		/// <summary>
		/// Сокращеннный вариант для поиска подстроки например
		/// </summary>
		/// <param name="place"></param>
		/// <param name="code"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string madeURL(int place,int code,int type)
		{
			return "&place=" + place
				+"&code=" + code 
				+"&type=" + type;
		}
		public Converters()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public string Rtf2Html(string txt)
		{
			#region Объявление переменных
			/*
		string strCurPhrase;
		string strHTML;
		//CodeList[] Codes;
		//CodeList[] CodesBeg;      //beginning codes
		string[] NextCodes;
		string[] NextCodesBeg;    //beginning codes for next text
		string[] CodesTmp;        //temp stack for copying
		string[] CodesTmpBeg;     //temp stack for copying beg

		string strCR;             //string to use for CRs - blank if +CR not chosen in options
		string strBeforeText;
		string strBeforeText2;
		string strBeforeText3;
		bool gPlain;              //true if all codes shouls be popped before next text
		bool gWBPlain;            //plain will be true after next text
		string[] strColorTable;   //table of colors
		int lColors;              //# of colors
		string strEOL;            //string to include before <br>
		string strBOL;            //string to include after <br>
		int lSkipWords;           //number od words to skip from current
		bool gBOL;                //a <br> was inserted but no non-whitespace text has been inserted
		bool gPar ;               //true if paragraph was reached since last text
		int lBrLev;               //bracket level when finding matching brackets
		string strSecTmp;         //temporary section buffer
		bool gIgnorePard;         //should pard end list items or not?
		string[] strFontTable;    //table of fonts
		int lFonts;               //# of fonts
		string strFont;
		string strTable;
		string strFontColor;      //current font color for setting up fontstring
		string strFontSize;       //current font size for setting up fontstring
		int lFontSize;
		short iDefFontSize;       //default font size
		bool gDebug;              //for debugging
		bool gStep;               //for debugging
		*/
			#endregion
			return "";
		}
	}
	/// <summary>
	/// Вспомогальный класс для разбора строки ссылки на объект
	/// </summary>
	public class GetURL
	{
		private int place,code,type;
		private string path,name,text,url;
		public GetURL():this(null){}
		/// <summary>
		/// Записываем сразу строку для последующего разбора
		/// </summary>
		/// <param name="url"></param>
		public GetURL(string url)
		{
			Init();
			if (url!=null)
				Parse(url);
		}
		/// <summary>
		/// Очищаем переменные
		/// </summary>
		public void Init()
		{
			place=code=type=0; 
			path=name=text=string.Empty;
		}
		/// <summary>
		/// Если важное есть - возвращем true
		/// </summary>
		public bool isInfo
		{
			get
			{
				return place!=0 && code!=0 && type!=0 && text!=string.Empty;
			}
		}
		public bool isEmpty
		{
			get
			{
				return place==0 && code==0 && type==0;
			}
		}
		public void Parse()
		{
			Parse(null);
		}
		public void Parse(string url)
		{
			if (url==this.url)
				return;     //уже разбирали!    
        
			Init();       //очищаемся
			this.url=url; //запоминаем
			//разбираем 
			//http:&DateTimeLabelUniqueValue=290803_105153&place=12&code=123&type=1415&path=12~98~992&name=TQRCBD4EIAA/BEAEPgRBBEIEPgQgAEIENQQ6BEEEQgQgADQEOwRPBCAAPgRCBDsEMAQ0BDoEOAQ&=исторический
			string[] st=url.Split('&');
			if (st.Length==1) //что это ?
				return ;
			try
			{
				for (int i=0;i<st.Length;i++)
				{
					string s=st[i];
					if (s.Length>6 && s.Substring(0,6)=="place=")
						place=Convert.ToInt32(s.Substring(6,s.Length-6));
					else if (s.Length>5 && s.Substring(0,5)=="code=")
						code=Convert.ToInt32(s.Substring(5,s.Length-5));
					else if (s.Length>5 && s.Substring(0,5)=="type=")
						type=Convert.ToInt32(s.Substring(5,s.Length-5));
					else if (s.Length>5 && s.Substring(0,5)=="path=")
						path=s.Substring(5,s.Length-5);
					else if (s.Length>5 && s.Substring(0,5)=="name=")
						name=Converters.fromBase64(s.Substring(5,s.Length-5));
					else if (s.Length>1 && s.Substring(0,1)=="=")
					{
						text=s.Substring(1,s.Length-1);
						if (text.IndexOf("\n")!=-1)
							text=text.Substring(0,text.IndexOf("\n"));
					}
				}
			}
			catch{}
		}
		/// <summary>
		/// Если 2 объекта одинаковы - возвращает истину!
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			GetURL p=(GetURL)obj;
			if (p!=null && p.isInfo && this.isInfo && p.Place==this.Place && p.Code==this.Code)
				return true;
			else
				return false;
		}
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}
		/*
				public static bool operator == (GetURL aURL1, GetURL aURL2)
				{
					if (aURL1==null || aURL2==null)
						return true;
					else
						return aURL1.Equals(aURL2);
				}

				public static bool operator != (GetURL aURL1, GetURL aURL2)
				{
					if (aURL1==null || aURL2==null)
						return false;
					else
						return !aURL1.Equals(aURL2);
				}
		*/    
		/// <summary>
		/// Возвращаем коллекцию только уникальных компонентов
		/// </summary>
		/// <param name="source">Источник массива объектов GetURL</param>
		/// <param name="dest">Приемник массива объектов GetURL</param>
		/// <returns>Массив уникальных объектов GetURL</returns>
		static public ArrayList AddLinks(ArrayList source,ArrayList dest)
		{
			if (source==null || source.Count==0)
			{
				if (dest!=null && dest.Count>0)
					return dest;
				else
					return null;
			}
			else if (dest==null || dest.Count==0)
			{
				if (source!=null && source.Count>0)
					return source;
				else
					return null;
			}
			for (int i=0;i<source.Count;i++)
			{
				GetURL p=source[i] as GetURL;
				if (p!=null && p.isInfo)
				{
					bool canAdd=true;
					for (int j=0;j<dest.Count;j++)
					{
						GetURL s=dest[j] as GetURL;
						if (s!=null && s.Equals(p))
						{
							canAdd=false;  //повтор
							break;
						}
					}
					if (canAdd)
						dest.Add(p);
				}
			}
			return dest;
		}
		#region Возвращаемые (разобранные) параметры
		/// <summary>
		/// Возвращаем разобранный Place
		/// </summary>
		public int Place
		{
			get{return place;}
		}
		/// <summary>
		/// Возвращаем разобранный Code
		/// </summary>
		public int Code
		{
			get{return code;}
		}
		/// <summary>
		/// Возвращаем разобранный Type
		/// </summary>
		public int Type
		{
			get{return type;}
		}
		/// <summary>
		/// Возвращаем разобранный Path
		/// </summary>
		public string Path
		{
			get{return path;}
		}
		/// <summary>
		/// Возвращаем разобранный Name
		/// </summary>
		public string Name
		{
			get{return name;}
		}
		/// <summary>
		/// Возвращаем текст под ссылкой
		/// </summary>
		public string Text
		{
			get{return text;}
		}
		/// <summary>
		/// Возвращаем путь (url)
		/// </summary>
		public string Url
		{
			get
			{
				return this.url;
			}
		}
		#endregion
	}

	#region Test
	/*
	void ClearNext(Optional strExcept As String)
	{
		int l;
		if (strExcept.Length > 0)
		if InNext(strExcept)
			while (NextCodes(1) != strExcept)
			{
				ShiftNext();
				ShiftNextBeg();
			}
		//ReDim NextCodes[0];
		//ReDim NextCodesBeg[0];
        
	}

	void ClearFont()
	{
		strFont = string.Empty;
		strTable = string.Empty;
		strFontColor = string.Empty;
		strFontSize = string.Empty;
		lFontSize = 0;
	}
Function Codes2NextTill(strCode As String)
Dim strTmp As String
Dim strTmpbeg As String
Dim l As Long

For l = 1 To UBound(Codes)
If Codes(l).Code = strCode Then Exit For
If Codes(l).Status <> "K" And Codes(l).Status <> "D" Then
If Not InNext(strCode) Then
UnShiftNext (Codes(l).Code)
UnShiftNextBeg (CodesBeg(l).Code)
End If
End If
Next l
End Function

Function GetColorTable(strSecTmp As String, strColorTable() As String)
		//get color table data and fill in strColorTable array
Dim lColors As Long
Dim lBOS As Long
Dim lEOS As Long
Dim strTmp As String
    
lBOS = InStr(strSecTmp, "\colortbl")
ReDim strColorTable(0)
lColors = 1
If lBOS <> 0 Then
lBOS = InStr(lBOS, strSecTmp, ";")
lEOS = InStr(lBOS, strSecTmp, ";}")
If lEOS <> 0 Then
lBOS = InStr(lBOS, strSecTmp, "\red")
While ((lBOS <= lEOS) And (lBOS <> 0))
ReDim Preserve strColorTable(lColors)
strTmp = Trim(Hex(Mid(strSecTmp, lBOS + 4, 1) & IIf(IsNumeric(Mid(strSecTmp, lBOS + 5, 1)), Mid(strSecTmp, lBOS + 5, 1), string.Empty;) & IIf(IsNumeric(Mid(strSecTmp, lBOS + 6, 1)), Mid(strSecTmp, lBOS + 6, 1), string.Empty;)))
If Len(strTmp) = 1 Then strTmp = "0" & strTmp
strColorTable(lColors) = strColorTable(lColors) & strTmp
lBOS = InStr(lBOS, strSecTmp, "\green")
strTmp = Trim(Hex(Mid(strSecTmp, lBOS + 6, 1) & IIf(IsNumeric(Mid(strSecTmp, lBOS + 7, 1)), Mid(strSecTmp, lBOS + 7, 1), string.Empty;) & IIf(IsNumeric(Mid(strSecTmp, lBOS + 8, 1)), Mid(strSecTmp, lBOS + 8, 1), string.Empty;)))
If Len(strTmp) = 1 Then strTmp = "0" & strTmp
strColorTable(lColors) = strColorTable(lColors) & strTmp
lBOS = InStr(lBOS, strSecTmp, "\blue")
strTmp = Trim(Hex(Mid(strSecTmp, lBOS + 5, 1) & IIf(IsNumeric(Mid(strSecTmp, lBOS + 6, 1)), Mid(strSecTmp, lBOS + 6, 1), string.Empty;) & IIf(IsNumeric(Mid(strSecTmp, lBOS + 7, 1)), Mid(strSecTmp, lBOS + 7, 1), string.Empty;)))
If Len(strTmp) = 1 Then strTmp = "0" & strTmp
strColorTable(lColors) = strColorTable(lColors) & strTmp
lBOS = InStr(lBOS, strSecTmp, "\red")
lColors = lColors + 1
Wend
End If
End If
End Function

Function GetFontTable(strSecTmp As String, strFontTable() As String)
		//get font table data and fill in strFontTable array
Dim lFonts As Long
Dim lBOS As Long
Dim lEOS As Long
Dim strTmp As String
    
lBOS = InStr(strSecTmp, "\fonttbl")
ReDim strFontTable(0)
lFonts = 0
If lBOS <> 0 Then
lEOS = InStr(lBOS, strSecTmp, ";}}")
If lEOS <> 0 Then
lBOS = InStr(lBOS, strSecTmp, "\f0")
While ((lBOS <= lEOS) And (lBOS <> 0))
ReDim Preserve strFontTable(lFonts)
While ((Mid(strSecTmp, lBOS, 1) <> " ") And (lBOS <= lEOS))
lBOS = lBOS + 1
Wend
lBOS = lBOS + 1
strTmp = Mid(strSecTmp, lBOS, InStr(lBOS, strSecTmp, ";") - lBOS)
strFontTable(lFonts) = strFontTable(lFonts) & strTmp
lBOS = InStr(lBOS, strSecTmp, "\f" & (lFonts + 1))
lFonts = lFonts + 1
Wend
End If
End If
End Function


Function InNext(strTmp) As Boolean
Dim gTmp As Boolean
Dim l As Long
    
l = 1
gTmp = False
While l <= UBound(NextCodes) And Not gTmp
If NextCodes(l) = strTmp Then gTmp = True
l = l + 1
Wend
InNext = gTmp
End Function

Function InNextBeg(strTmp) As Boolean
Dim gTmp As Boolean
Dim l As Long
    
l = 1
gTmp = False
While l <= UBound(NextCodesBeg) And Not gTmp
If NextCodesBeg(l) = strTmp Then gTmp = True
l = l + 1
Wend
InNextBeg = gTmp
End Function

Function InCodes(strTmp, Optional gActiveOnly As Boolean = False) As Boolean
Dim gTmp As Boolean
Dim l As Long
    
l = 1
gTmp = False
While l <= UBound(Codes) And Not gTmp
If gActiveOnly Then
If Codes(l).Code = strTmp And (Codes(l).Status = "A" Or Codes(l).Status = "G") Then gTmp = True
Else
If Codes(l).Code = strTmp Then gTmp = True
End If
l = l + 1
Wend
InCodes = gTmp
End Function

Function InCodesBeg(strTmp) As Boolean
Dim gTmp As Boolean
Dim l As Long
    
l = 1
gTmp = False
While l <= UBound(CodesBeg) And Not gTmp
If CodesBeg(l).Code = strTmp Then gTmp = True
l = l + 1
Wend
InCodesBeg = gTmp
End Function

Function NabNextLine(strRTF As String) As String
{
	int l=0;
    
l = InStr(strRTF, vbCrLf)
If l = 0 Then l = Len(strRTF)
NabNextLine = TrimAll(strRTF.substring(0,l));
if (l==strRTF.Length)
	strRTF = string.Empty;
else
	strRTF = TrimAll(Mid(strRTF, l));
}


Function NabNextWord(strLine As String) As String
Dim l As Long
Dim lvl As Integer
Dim gEndofWord As Boolean
Dim gInCommand As Boolean    //current word is command instead of plain word
    
gInCommand = False
l = 0
lvl = 0
		//strLine = TrimifCmd(strLine)
If Left(strLine, 1) = "}" Then
strLine = Mid(strLine, 2)
NabNextWord = "}"
GoTo 
finally
End If
If Left(strLine, 1) = "{" Then
strLine = Mid(strLine, 2)
NabNextWord = "{"
GoTo 
finally
End If
If Left(strLine, 2) = "\//" Then
NabNextWord = Left(strLine, 4)
strLine = Mid(strLine, 5)
GoTo 
finally
End If
While Not gEndofWord
l = l + 1
If l >= Len(strLine) Then
If l = Len(strLine) Then l = l + 1
gEndofWord = True
ElseIf InStr("\{}", Mid(strLine, l, 1)) Then
If l = 1 And Mid(strLine, l, 1) = "\" Then gInCommand = True
If Mid(strLine, l + 1, 1) <> "\" And l > 1 And lvl = 0 Then
gEndofWord = True
End If
ElseIf Mid(strLine, l, 1) = " " And lvl = 0 And gInCommand Then
gEndofWord = True
End If
Wend
    
If l = 0 Then l = Len(strLine)
NabNextWord = Left(strLine, l - 1)
While Len(NabNextWord) > 0 And InStr("{}", Right(NabNextWord, 1)) And l > 0
NabNextWord = Left(NabNextWord, Len(NabNextWord) - 1)
l = l - 1
Wend
strLine = Mid(strLine, l)
If Left(strLine, 1) = " " Then strLine = Mid(strLine, 2)
finally:
End Function

Function NabSection(strRTF As String, lPos As Long) As String
																														//grab section surrounding lPos, strip section out of strRTF and return it
Dim lBOS As Long         //beginning of section
Dim lEOS As Long         //ending of section
Dim strChar As String
Dim lLev As Long         //level of brackets/parens
Dim lRTFLen As Long
    
lRTFLen = Len(strRTF)
    
lBOS = lPos
strChar = Mid(strRTF, lBOS, 1)
lLev = 1
While lLev > 0
lBOS = lBOS - 1
If lBOS <= 0 Then
lLev = lLev - 1
Else
strChar = Mid(strRTF, lBOS, 1)
If strChar = "}" Then
lLev = lLev + 1
ElseIf strChar = "{" Then
lLev = lLev - 1
End If
End If
Wend
lBOS = lBOS - 1
If lBOS < 1 Then lBOS = 1
    
lEOS = lPos
strChar = Mid(strRTF, lEOS, 1)
lLev = 1
While lLev > 0
lEOS = lEOS + 1
If lEOS >= lRTFLen Then
lLev = lLev - 1
Else
strChar = Mid(strRTF, lEOS, 1)
If strChar = "{" Then
lLev = lLev + 1
ElseIf strChar = "}" Then
lLev = lLev - 1
End If
End If
Wend
lEOS = lEOS + 1
If lEOS > lRTFLen Then lEOS = lRTFLen
NabSection = Mid(strRTF, lBOS + 1, lEOS - lBOS - 1)
strRTF = Mid(strRTF, 1, lBOS) & Mid(strRTF, lEOS)
strRTF = rtf2html_replace(strRTF, vbCrLf & vbCrLf, vbCrLf)
End Function

Function Next2Codes()
				//move codes from pending ("next") stack to front of current stack
Dim lNumCodes As Long
Dim lNumNext As Long
Dim l As Long
    
If UBound(NextCodes) > 0 Then
If InNext("</li>") Then
For l = UBound(NextCodes) To 1 Step -1
If NextCodes(l) = "</li>" And l > 1 Then
NextCodes(l) = NextCodes(l - 1)
NextCodesBeg(l) = NextCodesBeg(l - 1)
NextCodes(l - 1) = "</li>"
NextCodesBeg(l - 1) = "<li>"
End If
Next l
End If
        
lNumCodes = UBound(Codes)
lNumNext = UBound(NextCodes)
ReDim Preserve Codes(lNumCodes + lNumNext)
ReDim Preserve CodesBeg(lNumCodes + lNumNext)
For l = UBound(Codes) To 1 Step -1
If l > lNumNext Then
Codes(l) = Codes(l - lNumNext)
CodesBeg(l) = CodesBeg(l - lNumNext)
Else
Codes(l).Code = NextCodes(lNumNext - l + 1)
CodesBeg(l).Code = NextCodesBeg(lNumNext - l + 1)
Select Case Codes(l).Code
Case "</td></tr></table>", "</li>"
Codes(l).Status = "PG"
CodesBeg(l).Status = "PG"
Case Else
Codes(l).Status = "P"
CodesBeg(l).Status = "P"
End Select
End If
Next l
ReDim NextCodes(0)
ReDim NextCodesBeg(0)
End If
End Function

Function Codes2Next()
					//move codes from "current" stack to pending ("next") stack
Dim lNumCodes As Long
Dim l As Long
    
If UBound(Codes) > 0 Then
lNumCodes = UBound(NextCodes)
ReDim Preserve NextCodes(lNumCodes + UBound(Codes))
ReDim Preserve NextCodesBeg(lNumCodes + UBound(Codes))
For l = 1 To UBound(Codes)
NextCodes(lNumCodes + l) = Codes(l).Code
NextCodesBeg(lNumCodes + l) = CodesBeg(l).Code
Next l
ReDim Codes(0)
ReDim CodesBeg(0)
End If
End Function

Function ParseFont(strColor As String, strSize As String) As String
Dim strTmpFont As String
    
If strColor & strSize = string.Empty; Then
strTmpFont = string.Empty;
Else
strTmpFont = "<font"
If strColor <> string.Empty; Then
strTmpFont = strTmpFont & " color=string.Empty;" & strColor & string.Empty;string.Empty;
End If
If strSize <> string.Empty; And Val(strSize) <> iDefFontSize Then
strTmpFont = strTmpFont & " size=" & strSize
End If
strTmpFont = strTmpFont & ">"
End If
ParseFont = strTmpFont
End Function

Function PopCode() As String
If UBound(Codes) > 0 Then
PopCode = Codes(UBound(Codes)).Code
ReDim Preserve Codes(UBound(Codes) - 1)
End If
End Function

Function ProcessAfterTextCodes() As String
Dim strTmp As String
Dim l As Long
Dim lLastKilled As Long
Dim lRetVal As Long
    
																																																//check for/handle font change
If strFont <> GetLastFont Then
KillCode ("</font>")
If Len(strFont) > 0 Then
lRetVal = ReplaceInNextBeg("</font>", strFont)
If lRetVal = 0 Then
PushNext ("</font>")
PushNextBeg (strFont)
End If
End If
Else
If Not InNext("</li>") Then ReviveCode ("</font>")
End If
        
			//now handle everything killed and move codes farther in to next
		//    ie: \b B\i B \u B\i0 B \u0\b0 => <b>B<i>B<u>B</u>B</i><u>B</u></b>
strTmp = string.Empty;
If UBound(Codes) > 0 Then
lLastKilled = 0
For l = UBound(Codes) To 1 Step -1
If Codes(l).Status = "K" Then
lLastKilled = l
Exit For
End If
Next l
If lLastKilled > 0 Then
For l = 1 To lLastKilled
strTmp = strTmp & Codes(l).Code
If Codes(l).Code = "</li>" Then strTmp = strTmp & strCR
Next l
For l = lLastKilled To 1 Step -1
If Codes(l).Status <> "D" And Codes(l).Status <> "K" Then
If Not InNext(Codes(l).Code) Then
PushNext (Codes(l).Code)
PushNextBeg (CodesBeg(l).Code)
End If
Codes(l).Status = "K"
CodesBeg(l).Status = "K"
End If
Next l
End If
End If
ProcessAfterTextCodes = strTmp
End Function
Function GetActiveCodes() As String
Dim strTmp As String
Dim l As Long
    
strTmp = string.Empty;
If UBound(Codes) > 0 Then
For l = 1 To UBound(Codes)
strTmp = strTmp & Codes(l).Code
Next l
End If
GetActiveCodes = strTmp
End Function

Function GetLastFont() As String
Dim strTmp As String
Dim l As Long
    
strTmp = string.Empty;
If UBound(Codes) > 0 Then
For l = UBound(Codes) To 1 Step -1
If Codes(l).Code = "</font>" Then
strTmp = CodesBeg(l).Code
Exit For
End If
Next l
End If
GetLastFont = strTmp
End Function

Function SetPendingCodesActive()
Dim strTmp As String
Dim l As Long
    
strTmp = string.Empty;
If UBound(Codes) > 0 Then
For l = 1 To UBound(Codes)
If Codes(l).Status = "P" Then
Codes(l).Status = "A"
CodesBeg(l).Status = "A"
ElseIf Codes(l).Status = "PG" Then
Codes(l).Status = "G"
CodesBeg(l).Status = "G"
End If
Next l
End If
End Function

Function KillCode(strCode As String, Optional strExcept As String = string.Empty;) As Long
																																											//mark all codes of type strCode as killed
		//    except where status = strExcept
		//    if strCode = "*" then mark all killed
Dim strTmp As String
Dim l As Long
        
strTmp = string.Empty;
If UBound(Codes) > 0 Then
If Left(strExcept, 1) = "<" Then    //strExcept is either a code or a status
For l = 1 To UBound(Codes)
If (Codes(l).Code = strCode Or strCode = "*") And Codes(l).Code <> strExcept Then
Codes(l).Status = "K"
CodesBeg(l).Status = "K"
End If
If strCode = "*" And Codes(l).Code = strExcept Then Exit For
Next l
Else
For l = 1 To UBound(Codes)
If (Codes(l).Code = strCode Or strCode = "*") And Codes(l).Status <> strExcept Then
Codes(l).Status = "K"
CodesBeg(l).Status = "K"
End If
Next l
End If
End If
End Function

Function GetAllCodesTill(strTill As String) As String
																														//get all codes except strTill
Dim strTmp As String
Dim l As Long
    
strTmp = string.Empty;
If UBound(Codes) > 0 Then
For l = UBound(Codes) To 1 Step -1
If Codes(l).Code = strTill Then
Exit For
Else
If Not InNextBeg(CodesBeg(l).Code) And Codes(l).Status <> "D" Then
strTmp = strTmp & Codes(l).Code
Codes(l).Status = "K"
CodesBeg(l).Status = "K"
End If
End If
Next l
End If
GetAllCodesTill = strTmp
End Function


Function GetAllCodesBeg() As String
Dim strTmp As String
Dim l As Long
    
strTmp = string.Empty;
If UBound(CodesBeg) > 0 Then
For l = UBound(CodesBeg) To 1 Step -1
If CodesBeg(l).Status = "P" Then
strTmp = strTmp & CodesBeg(l).Code
CodesBeg(l).Status = "A"
Codes(l).Status = "A"
ElseIf CodesBeg(l).Status = "PG" Then
strTmp = strTmp & CodesBeg(l).Code
CodesBeg(l).Status = "G"
Codes(l).Status = "G"
End If
Next l
End If
GetAllCodesBeg = strTmp
End Function

Function GetAllCodesBegTill(strTill As String) As String
																																										//get all codes except strTill - stop if strTill reached
		//"<table"
Dim strTmp As String
Dim l As Long
    
strTmp = string.Empty;
If UBound(CodesBeg) > 0 Then
For l = 1 To UBound(CodesBeg)
If Codes(l).Code = strTill Then
Exit For
Else
If CodesBeg(l).Status = "P" Then
strTmp = strTmp & CodesBeg(l).Code
Codes(l).Status = "A"
CodesBeg(l).Status = "A"
ElseIf CodesBeg(l).Status = "PG" Then
strTmp = strTmp & CodesBeg(l).Code
Codes(l).Status = "G"
CodesBeg(l).Status = "G"
End If
End If
Next l
End If
GetAllCodesBegTill = strTmp
End Function






Function ShiftNext() As String
																																//get 1st code off list and shorten list
Dim l As Long
    
If UBound(NextCodes) > 0 Then
ShiftNext = NextCodes(1)
For l = 1 To UBound(NextCodes) - 1
NextCodes(l) = NextCodes(l + 1)
Next l
ReDim Preserve NextCodes(UBound(NextCodes) - 1)
End If
End Function
Function ShiftNextBeg() As String
																	//get 1st code off list and shorten list
Dim l As Long
    
If UBound(NextCodesBeg) > 0 Then
ShiftNextBeg = NextCodesBeg(1)
For l = 1 To UBound(NextCodesBeg) - 1
NextCodesBeg(l) = NextCodesBeg(l + 1)
Next l
ReDim Preserve NextCodesBeg(UBound(NextCodesBeg) - 1)
End If
End Function


Function ProcessWord(strWord As String)
Dim strTmp As String
Dim strTmpbeg As String
Dim l As Long
Dim gPopAll As Boolean
Dim lRetVal As Long
    
Dim strTableAlign As String    //current table alignment for setting up tablestring
Dim strTableWidth As String    //current table width for setting up tablestring
    
If lSkipWords > 0 Then
lSkipWords = lSkipWords - 1
Exit Function
End If
If Left(strWord, 1) = "\" Or Left(strWord, 1) = "{" Or Left(strWord, 1) = "}" Then
strWord = Trim(strWord)
Select Case Left(strWord, 2)
Case "}"
If lBrLev = 0 Then
lRetVal = KillCode("*", "G")
ClearNext ("</li>")
ClearFont
End If
Case "\//"    //special characters
strTmp = HTMLCode(Mid(strWord, 3))
If Left(strTmp, 6) = "<rtf>:" Then
strSecTmp = Mid(strTmp, 7) & " " & strSecTmp
Else
strSecTmp = strTmp & " " & strSecTmp
End If
Case "\b"    //bold
If strWord = "\b" Then
If InCodes("</b>", True) Then
																									 //                    Codes2NextTill ("</b>")
Else
PushNext ("</b>")
PushNextBeg ("<b>")
End If
ElseIf strWord = "\bullet" Then
																	 //If Not (Codes(UBound(Codes)).Code = "</li>" And Codes(UBound(Codes)).Status = "A") Then
PushNext ("</li>")
PushNextBeg ("<li>")
		//End If
ElseIf strWord = "\b0" Then    //bold off
If InCodes("</b>") Then
Codes2NextTill ("</b>")
KillCode ("</b>")
End If
If InNext("</b>") Then
RemoveFromNext ("</b>")
End If
End If
Case "\c"
If strWord = "\cf0" Then    //color font off
strFontColor = string.Empty;
strFont = ParseFont(strFontColor, strFontSize)
ElseIf Left(strWord, 3) = "\cf" And IsNumeric(Mid(strWord, 4)) Then  //color font
		//get color code
l = Val(Mid(strWord, 4))
If l <= UBound(strColorTable) And l > 0 Then
strFontColor = "#" & strColorTable(l)
End If
                
																								//insert color
If strFontColor <> "#" Then
strFont = ParseFont(strFontColor, strFontSize)
If InNext("</font>") Then
ReplaceInNextBeg "</font>", strFont
ElseIf InCodes("</font>") Then
PushNext ("</font>")
PushNextBeg (strFont)
Codes2NextTill "</font>"
KillCode ("</font>")
Else
PushNext ("</font>")
PushNextBeg (strFont)
End If
End If
End If
Case "\f"
If Left(strWord, 3) = "\fs" And IsNumeric(Mid(strWord, 4)) Then  //font size
l = Val(Mid(strWord, 4))
lFontSize = Int((l / 7) - 0)    //calc to convert RTF to HTML sizes
If lFontSize > 8 Then lFontSize = 8
If lFontSize < 1 Then lFontSize = 1
strFontSize = Trim(lFontSize)
If Val(strFontSize) = iDefFontSize Then strFontSize = string.Empty;
		//insert size
strFont = ParseFont(strFontColor, strFontSize)
End If
Case "\i"
If strWord = "\i" Then //italics
If InCodes("</i>", True) Then
															 //                    Codes2NextTill ("</i>")
Else
PushNext ("</i>")
PushNextBeg ("<i>")
End If
ElseIf strWord = "\i0" Then //italics off
If InCodes("</i>") Then
Codes2NextTill ("</i>")
KillCode ("</i>")
End If
If InNext("</i>") Then
RemoveFromNext ("</i>")
End If
End If
Case "\l"
		//If strWord = "\listname" Then
		//    lSkipWords = 1
		//End If
Case "\n"
If strWord = "\nosupersub" Then    //superscript/subscript off
If InCodes("</sub>", True) Then
Codes2NextTill ("</sub>")
KillCode ("</sub>")
End If
If InNext("</sub>") Then
RemoveFromNext ("</sub>")
End If
If InCodes("</sup>", True) Then
Codes2NextTill ("</sup>")
KillCode ("</sup>")
End If
If InNext("</sup>") Then
RemoveFromNext ("</sup>")
End If
End If
Case "\p"
If strWord = "\par" Then
If Not (InCodes("</ul>") Or InCodes("</li>")) Then
strBeforeText2 = strBeforeText2 & strEOL & "<br>" & strCR
Else
lRetVal = KillCode("</li>")
RemoveFromNext ("</li>")
End If
gBOL = True
gPar = True
							 //If InCodes("</ul>") Then
		//    PushNext ("</li>")
		//    PushNextBeg ("<li>")
		//End If
ElseIf strWord = "\pard" Then
For l = 1 To UBound(CodesBeg)
If Codes(l).Status = "G" Or Codes(l).Status = "PG" Then
Codes(l).Status = "K"
CodesBeg(l).Status = "K"
End If
Next l
If Not gIgnorePard Then
If InCodes("</li>") Then
lRetVal = KillCode("</li>")
RemoveFromNext ("</li>")
End If
End If
gPar = True
ElseIf strWord = "\plain" Then
lRetVal = KillCode("*", "G")
ClearFont
ElseIf strWord = "\pnlvlblt" Then //bulleted list
If Not InNext("</li>") Then
PushNext ("</li>")
PushNextBeg ("<li>")
End If
			//PushNext ("</ul>")
		//PushNextBeg ("<ul>")
ElseIf strWord = "\pntxta" Then //numbered list?
lSkipWords = 1
ElseIf strWord = "\pntxtb" Then //numbered list?
lSkipWords = 1
ElseIf strWord = "\pntext" Then //bullet
If Not InNext("</li>") Then
PushNext ("</li>")
PushNextBeg ("<li>")
Codes2NextTill ("</table>")
KillCode ("*")
End If
End If
Case "\q"
If strWord = "\qc" Then    //centered
strTableAlign = "center"
strTableWidth = "100%"
If InNext("</td></tr></table>") Then
																			//?
Else
strTable = "<table width=" & strTableWidth & "><tr><td align=string.Empty;" & strTableAlign & string.Empty;">"
End If
If InNext("</td></tr></table>") Then
ReplaceInNextBeg "</td></tr></table>", strTable
ElseIf InCodes("</td></tr></table>") Then
PushNext ("</td></tr></table>")
PushNextBeg (strTable)
Codes2NextTill "</td></tr></table>"
Else
PushNext ("</td></tr></table>")
PushNextBeg (strTable)
End If
ElseIf strWord = "\qr" Then    //right justified
strTableAlign = "right"
strTableWidth = "100%"
If InNext("</td></tr></table>") Then
																			//?
Else
strTable = "<table width=" & strTableWidth & "><tr><td align=string.Empty;" & strTableAlign & string.Empty;">"
End If
If InNext("</td></tr></table>") Then
ReplaceInNextBeg "</td></tr></table>", strTable
ElseIf InCodes("</td></tr></table>") Then
PushNext ("</td></tr></table>")
PushNextBeg (strTable)
Codes2NextTill "</td></tr></table>"
Else
PushNext ("</td></tr></table>")
PushNextBeg (strTable)
End If
End If
Case "\s"
If strWord = "\strike" Then    //strike text
If Codes(UBound(Codes)).Code <> "</s>" Or (Codes(UBound(Codes)).Code = "</s>" And CodesBeg(UBound(Codes)).Code = string.Empty;) Then
PushNext ("</s>")
PushNextBeg ("<s>")
End If
ElseIf strWord = "\strike0" Then    //strike off
If InCodes("</s>") Then
Codes2NextTill ("</s>")
KillCode ("</s>")
End If
If InNext("</s>") Then
RemoveFromNext ("</s>")
End If
ElseIf strWord = "\super" Then    //superscript
If Codes(UBound(Codes)).Code <> "</sup>" Or (Codes(UBound(Codes)).Code = "</sup>" And CodesBeg(UBound(Codes)).Code = string.Empty;) Then
PushNext ("</sup>")
PushNextBeg ("<sup>")
End If
ElseIf strWord = "\sub" Then    //subscript
If Codes(UBound(Codes)).Code <> "</sub>" Or (Codes(UBound(Codes)).Code = "</sub>" And CodesBeg(UBound(Codes)).Code = string.Empty;) Then
PushNext ("</sub>")
PushNextBeg ("<sub>")
End If
End If

				//If strWord = "\snext0" Then    //style
																					 //    lSkipWords = 1
		//End If
Case "\u"
If strWord = "\ul" Then    //underline
If InCodes("</u>", True) Then
															 //                    Codes2NextTill ("</u>")
Else
PushNext ("</u>")
PushNextBeg ("<u>")
End If
ElseIf strWord = "\ulnone" Then    //stop underline
If InCodes("</u>") Then
Codes2NextTill ("</u>")
KillCode ("</u>")
End If
If InNext("</u>") Then
RemoveFromNext ("</u>")
End If
End If
End Select
Else
If Len(strWord) > 0 Then
If Trim(strWord) = string.Empty; Then
If gBOL Then strWord = rtf2html_replace(strWord, " ", "&nbsp;")
strCurPhrase = strCurPhrase & strBeforeText3 & strWord
Else
																										 //regular text
If gPar Then
strBeforeText = strBeforeText & ProcessAfterTextCodes
Next2Codes
strBeforeText3 = GetAllCodesBeg
gPar = False
Else
strBeforeText = strBeforeText & ProcessAfterTextCodes
Next2Codes
strBeforeText3 = GetAllCodesBegTill("</td></tr></table>")
End If
RemoveBlanks
                
strCurPhrase = strCurPhrase & strBeforeText
strBeforeText = string.Empty;
strCurPhrase = strCurPhrase & strBeforeText2
strBeforeText2 = string.Empty;
strCurPhrase = strCurPhrase & strBeforeText3 & strWord
strBeforeText3 = string.Empty;
gBOL = False
End If
End If
End If
End Function

Function PushNext(strCode As String)
If Len(strCode) > 0 Then
ReDim Preserve NextCodes(UBound(NextCodes) + 1)
NextCodes(UBound(NextCodes)) = strCode
End If
End Function

Function UnShiftNext(strCode As String)
		//stick strCode on front of list and move everything over to make room
Dim l As Long
    
If Len(strCode) > 0 Then
ReDim Preserve NextCodes(UBound(NextCodes) + 1)
If UBound(NextCodes) > 1 Then
For l = UBound(NextCodes) To 1 Step -1
NextCodes(l) = NextCodes(l - 1)
Next l
End If
NextCodes(1) = strCode
End If
End Function

Function UnShiftNextBeg(strCode As String)
Dim l As Long
    
If Len(strCode) > 0 Then
ReDim Preserve NextCodesBeg(UBound(NextCodesBeg) + 1)
If UBound(NextCodesBeg) > 1 Then
For l = UBound(NextCodesBeg) To 1 Step -1
NextCodesBeg(l) = NextCodesBeg(l - 1)
Next l
End If
NextCodesBeg(1) = strCode
End If
End Function

Function PushNextBeg(strCode As String)
ReDim Preserve NextCodesBeg(UBound(NextCodesBeg) + 1)
NextCodesBeg(UBound(NextCodesBeg)) = strCode
End Function


Function RemoveBlanks()
Dim l As Long
Dim lOffSet As Long
    
l = 1
lOffSet = 0
While l <= UBound(CodesBeg) And l + lOffSet <= UBound(CodesBeg)
If CodesBeg(l).Status = "K" Or CodesBeg(l).Status = string.Empty; Then     //And Not (Codes(l) = "</font>" And Len(strFont) > 0) Then
lOffSet = lOffSet + 1
Else
l = l + 1
End If
If l + lOffSet <= UBound(CodesBeg) Then
Codes(l) = Codes(l + lOffSet)
CodesBeg(l) = CodesBeg(l + lOffSet)
End If
Wend
If lOffSet > 0 Then
ReDim Preserve Codes(UBound(Codes) - lOffSet)
ReDim Preserve CodesBeg(UBound(CodesBeg) - lOffSet)
End If
End Function

Function RemoveFromNext(strRem As String)
Dim l As Long
Dim m As Long
    
If UBound(NextCodes) < 1 Then GoTo 
finally
l = 1
While l < UBound(NextCodes)
If NextCodes(l) = strRem Then
For m = l To UBound(NextCodes) - 1
NextCodes(m) = NextCodes(m + 1)
NextCodesBeg(m) = NextCodesBeg(m + 1)
Next m
l = m
Else
l = l + 1
End If
Wend
ReDim Preserve NextCodes(UBound(NextCodes) - 1)
ReDim Preserve NextCodesBeg(UBound(NextCodesBeg) - 1)
finally:
End Function

Function rtf2html_replace(ByVal strIn As String, ByVal strRepl As String, ByVal strWith As String) As String
																																																					 //replace all instances of strRepl in strIn with strWith
Dim i As Integer
    
If ((Len(strRepl) = 0) Or (Len(strIn) = 0)) Then
rtf2html_replace = strIn
Exit Function
End If
i = InStr(strIn, strRepl)
While i <> 0
strIn = Left(strIn, i - 1) & strWith & Mid(strIn, i + Len(strRepl))
i = InStr(i + Len(strWith), strIn, strRepl)
Wend
rtf2html_replace = strIn
End Function

Function ReviveCode(strCode As String)
Dim l As Long
    
For l = 1 To UBound(Codes)
If Codes(l).Code = strCode Then
Codes(l).Status = "A"
CodesBeg(l).Status = "A"
End If
Next l
End Function

Function ReplaceInNextBeg(strCode As String, strWith As String) As Long
Dim l As Long
Dim lCount As Long    //number of codes replaced
    
lCount = 0
For l = 1 To UBound(NextCodes)
If NextCodes(l) = strCode Then
NextCodesBeg(l) = strWith
lCount = lCount + 1
End If
Next l
ReplaceInNextBeg = lCount
End Function

Function ReplaceInCodesBeg(strCode As String, strWith As String)
Dim l As Long
    
l = 1
While l <= UBound(Codes) And Codes(l).Code <> strCode
l = l + 1
Wend
If Codes(l).Code = strCode Then
If CodesBeg(l).Code <> strWith Then
CodesBeg(l).Code = strWith
Codes(l).Status = "P"
CodesBeg(l).Status = "P"
Else
Codes(l).Status = "P"
CodesBeg(l).Status = "P"
End If
End If
End Function

Function rtf2html3(strRTF As String, Optional strOptions As String) As String
																																								//Version 3.01
		//Copyright Brady Hegberg 2000
		//  I//m not licensing this software but I//d appreciate it if
		//  you//d to consider it to be under an lgpl sort of license
    
																											 //More information can be found at
		//http://www2.bitstream.net/~bradyh/downloads/rtf2htmlrm.html
    
		//Converts Rich Text encoded text to HTML format
		//if you find some text that this function doesn//t
																											//convert properly please email the text to
		//bradyh@bitstream.net
    
		//Options:
		//+H              add an HTML header and footer
		//+G              add a generator Metatag
		//+T="MyTitle"    add a title (only works if +H is used)
		//+CR             add a carraige return after all <br>s
		//+I              keep html codes intact
		//+F=X            default font size (blanks out any changes to this size - saves on space)
    
string strHTML;
string strRTFTmp;
int l;
int lTmp;
int lTmp2;
int lTmp3;
int lRTFLen;
int lBOS;                 //beginning of section
int lEOS;                 //end of section
string strTmp;
string strTmp2;
string strEOS;                 //string to be added to end of section
Dim strBOS As String             //string to be added to beginning of section
Dim strEOP As String             //string to be added to end of paragraph
Dim strBOL As String             //string to be added to the begining of each new line
Dim strEOL As String             //string to be added to the end of each new line
Dim strEOLL As String            //string to be added to the end of previous line
Dim strCurFont As String         //current font code eg: "f3"
Dim strCurFontSize As String     //current font size eg: "fs20"
Dim strCurColor As String        //current font color eg: "cf2"
Dim strFontFace As String        //Font face for current font
Dim strFontColor As String       //Font color for current font
Dim lFontSize As Integer         //Font size for current font
Const gHellFrozenOver = False    //always false
Dim gSkip As Boolean             //skip to next word/command
Dim strCodes As String           //codes for ascii to HTML char conversion
Dim strCurLine As String         //temp storage for text for current line before being added to strHTML
Dim strFontCodes As String       //list of font code modifiers
Dim gSeekingText As Boolean      //True if we have to hit text before inserting a </FONT>
Dim gText As Boolean             //true if there is text (as opposed to a control code) in strTmp
Dim strAlign As String           //"center" or "right"
Dim gAlign As Boolean            //if current text is aligned
Dim strGen As String             //Temp store for Generator Meta Tag if requested
Dim strTitle As String           //Temp store for Title if requested
Dim gHTML As Boolean             //true if html codes should be left intact
Dim strWordTmp As String         //temporary word buffer
Dim strEndText As String         //ending text


		// очищаем 
ReDim Codes(0);
ReDim CodesBeg(0);
ClearNext


strHTML = string.Empty;
gPlain = False
gBOL = True
gPar = False
strCurPhrase = string.Empty;
    
													//setup +CR option
If InStr(strOptions, "+CR") <> 0 Then strCR = vbCrLf Else strCR = string.Empty;
																													 //setup +HTML option
If InStr(strOptions, "+I") <> 0 Then gHTML = True Else gHTML = False
																												//setup default font size option
If InStr(strOptions, "+F=") <> 0 Then
l = InStr(strOptions, "+F=") + 3
strTmp = Mid(strOptions, l, 1)
iDefFontSize = 0
While IsDig(strTmp)
iDefFontSize = iDefFontSize * 10 + Val(strTmp)
l = l + 1
strTmp = Mid(strOptions, l, 1)
Wend
End If

strRTFTmp = TrimAll(strRTF)

If Left(strRTFTmp, 1) = "{" And Right(strRTFTmp, 1) = "}" Then strRTFTmp = Mid(strRTFTmp, 2, Len(strRTFTmp) - 2)
    
																																				//setup list (bullets) status
If InStr(strRTFTmp, "\list\") <> 0 Then
		//I//m not sure if this is in any way correct but it seems to work for me
																																					 //sometimes \pard ends a list item sometimes it doesn//t
gIgnorePard = True
Else
gIgnorePard = False
End If
    
																																																																			//setup color table
lBOS = InStr(strRTFTmp, "\colortbl")
If lBOS > 0 Then
strSecTmp = NabSection(strRTFTmp, lBOS)
GetColorTable strSecTmp, strColorTable()
End If
    
											//setup font table
lBOS = InStr(strRTFTmp, "\fonttbl")
If lBOS > 0 Then
strSecTmp = NabSection(strRTFTmp, lBOS)
GetFontTable strSecTmp, strFontTable()
End If
    
											//setup stylesheets
lBOS = InStr(strRTFTmp, "\stylesheet")
If lBOS > 0 Then
strSecTmp = NabSection(strRTFTmp, lBOS)
									//ignore stylesheets for now
End If
    
			//setup info
lBOS = InStr(strRTFTmp, "\info")
If lBOS > 0 Then
strSecTmp = NabSection(strRTFTmp, lBOS)
									//ignore info for now
End If
    
			//list table
lBOS = InStr(strRTFTmp, "\listtable")
If lBOS > 0 Then
strSecTmp = NabSection(strRTFTmp, lBOS)
									//ignore info for now
End If
    
			//list override table
lBOS = InStr(strRTFTmp, "\listoverridetable")
If lBOS > 0 Then
strSecTmp = NabSection(strRTFTmp, lBOS)
									//ignore info for now
End If

lBrLev = 0
While Len(strRTFTmp) > 0
strSecTmp = NabNextLine(strRTFTmp)
While Len(strSecTmp) > 0
strWordTmp = NabNextWord(strSecTmp)
If lBrLev > 0 Then
If strWordTmp = "{" Then
lBrLev = lBrLev + 1
ElseIf strWordTmp = "}" Then
lBrLev = lBrLev - 1
End If
strWordTmp = string.Empty;
ElseIf strWordTmp = "\*" Or strWordTmp = "\pict" Then
																											 //skip \pnlvlbt stuff
lBrLev = 1
strWordTmp = string.Empty;
ElseIf strWordTmp = "\pntext" Then
																		//get bullet codes but skip rest for now
lBrLev = 1
End If
If Len(strWordTmp) > 0 Then
If gDebug Then ShowCodes (strWordTmp)  //for debugging only
If Len(strWordTmp) > 0 Then ProcessWord strWordTmp
End If
Wend
Wend
    
																											//get any remaining codes in stack
strEndText = strEndText & GetActiveCodes
strBeforeText2 = rtf2html_replace(strBeforeText2, "<br>", string.Empty;)
strBeforeText2 = rtf2html_replace(strBeforeText2, vbCrLf, string.Empty;)
strCurPhrase = strCurPhrase & strBeforeText & strBeforeText2 & strEndText
strBeforeText = string.Empty;
strBeforeText2 = string.Empty;
strBeforeText3 = string.Empty;
strHTML = strHTML & strCurPhrase
strCurPhrase = string.Empty;
rtf2html3 = strHTML
End Function
Function IsDig(strChar As String) As Boolean
If Len(strChar) = 0 Then
IsDig = False
Else
IsDig = InStr("1234567890", strChar)
End If
End Function


Function GetCodes(strWordTmp As String) As String
Dim strTmp As String
Dim l As Long
    
strTmp = "CurWord: "
If Len(strWordTmp) > 20 Then
strTmp = strTmp & Left(strWordTmp, 20) & "..."
Else
strTmp = strTmp & strWordTmp
End If
strTmp = strTmp & vbCrLf & vbCrLf & "BegCodes: "
For l = 1 To UBound(CodesBeg)
strTmp = strTmp & CodesBeg(l).Code & " (" & CodesBeg(l).Status & "), "
Next l
strTmp = strTmp & vbCrLf & "Codes: "
For l = 1 To UBound(Codes)
strTmp = strTmp & Codes(l).Code & " (" & Codes(l).Status & "), "
Next l
strTmp = strTmp & vbCrLf & vbCrLf & "NextBegCodes: "
For l = 1 To UBound(NextCodesBeg)
strTmp = strTmp & NextCodesBeg(l) & ", "
Next l
strTmp = strTmp & vbCrLf & "NextCodes: "
For l = 1 To UBound(NextCodes)
strTmp = strTmp & NextCodes(l) & ", "
Next l
strTmp = strTmp & vbCrLf & vbCrLf & "Font String: " & strFont
strTmp = strTmp & vbCrLf & vbCrLf & "Before Text: " & strBeforeText2
GetCodes = strTmp
End Function

Function TrimAll(ByVal strTmp As String) As String
Dim l As Long
    
strTmp = Trim(strTmp)
l = Len(strTmp) + 1
While l <> Len(strTmp)
l = Len(strTmp)
If Right(strTmp, 1) = vbCrLf Then strTmp = Left(strTmp, Len(strTmp) - 1)
If Left(strTmp, 1) = vbCrLf Then strTmp = Right(strTmp, Len(strTmp) - 1)
If Right(strTmp, 1) = vbCr Then strTmp = Left(strTmp, Len(strTmp) - 1)
If Left(strTmp, 1) = vbCr Then strTmp = Right(strTmp, Len(strTmp) - 1)
If Right(strTmp, 1) = vbLf Then strTmp = Left(strTmp, Len(strTmp) - 1)
If Left(strTmp, 1) = vbLf Then strTmp = Right(strTmp, Len(strTmp) - 1)
Wend
TrimAll = strTmp
End Function

Function HTMLCode(strRTFCode As String) As String
																									//given rtf code return html code
Select Case strRTFCode
Case "00"
HTMLCode = "&nbsp;"
Case "a9"
HTMLCode = "&copy;"
Case "b4"
HTMLCode = "&acute;"
Case "ab"
HTMLCode = "&laquo;"
Case "bb"
HTMLCode = "&raquo;"
Case "a1"
HTMLCode = "&iexcl;"
Case "bf"
HTMLCode = "&iquest;"
Case "c0"
HTMLCode = "&Agrave;"
Case "e0"
HTMLCode = "&agrave;"
Case "c1"
HTMLCode = "&Aacute;"
Case "e1"
HTMLCode = "&aacute;"
Case "c2"
HTMLCode = "&Acirc;"
Case "e2"
HTMLCode = "&acirc;"
Case "c3"
HTMLCode = "&Atilde;"
Case "e3"
HTMLCode = "&atilde;"
Case "c4"
HTMLCode = "&Auml;"
Case "e4", "99"
HTMLCode = "<rtf>:\super TM\nosupersub"
Case "c5"
HTMLCode = "&Aring;"
Case "e5"
HTMLCode = "&aring;"
Case "c6"
HTMLCode = "&AElig;"
Case "e6"
HTMLCode = "&aelig;"
Case "c7"
HTMLCode = "&Ccedil;"
Case "e7"
HTMLCode = "&ccedil;"
Case "d0"
HTMLCode = "&ETH;"
Case "f0"
HTMLCode = "&eth;"
Case "c8"
HTMLCode = "&Egrave;"
Case "e8"
HTMLCode = "&egrave;"
Case "c9"
HTMLCode = "&Eacute;"
Case "e9"
HTMLCode = "&eacute;"
Case "ca"
HTMLCode = "&Ecirc;"
Case "ea"
HTMLCode = "&ecirc;"
Case "cb"
HTMLCode = "&Euml;"
Case "eb"
HTMLCode = "&euml;"
Case "cc"
HTMLCode = "&Igrave;"
Case "ec"
HTMLCode = "&igrave;"
Case "cd"
HTMLCode = "&Iacute;"
Case "ed"
HTMLCode = "&iacute;"
Case "ce"
HTMLCode = "&Icirc;"
Case "ee"
HTMLCode = "&icirc;"
Case "cf"
HTMLCode = "&Iuml;"
Case "ef"
HTMLCode = "&iuml;"
Case "d1"
HTMLCode = "&Ntilde;"
Case "f1"
HTMLCode = "&ntilde;"
Case "d2"
HTMLCode = "&Ograve;"
Case "f2"
HTMLCode = "&ograve;"
Case "d3"
HTMLCode = "&Oacute;"
Case "f3"
HTMLCode = "&oacute;"
Case "d4"
HTMLCode = "&Ocirc;"
Case "f4"
HTMLCode = "&ocirc;"
Case "d5"
HTMLCode = "&Otilde;"
Case "f5"
HTMLCode = "&otilde;"
Case "d6"
HTMLCode = "&Ouml;"
Case "f6"
HTMLCode = "&ouml;"
Case "d8"
HTMLCode = "&Oslash;"
Case "f8"
HTMLCode = "&oslash;"
Case "d9"
HTMLCode = "&Ugrave;"
Case "f9"
HTMLCode = "&ugrave;"
Case "da"
HTMLCode = "&Uacute;"
Case "fa"
HTMLCode = "&uacute;"
Case "db"
HTMLCode = "&Ucirc;"
Case "fb"
HTMLCode = "&ucirc;"
Case "dc"
HTMLCode = "&Uuml;"
Case "fc"
HTMLCode = "&uuml;"
Case "dd"
HTMLCode = "&Yacute;"
Case "fd"
HTMLCode = "&yacute;"
Case "ff"
HTMLCode = "&yuml;"
Case "de"
HTMLCode = "&THORN;"
Case "fe"
HTMLCode = "&thorn;"
Case "df"
HTMLCode = "&szlig;"
Case "a7"
HTMLCode = "&sect;"
Case "b6"
HTMLCode = "&para;"
Case "b5"
HTMLCode = "&micro;"
Case "a6"
HTMLCode = "&brvbar;"
Case "b1"
HTMLCode = "&plusmn;"
Case "b7"
HTMLCode = "&middot;"
Case "a8"
HTMLCode = "&uml;"
Case "b8"
HTMLCode = "&cedil;"
Case "aa"
HTMLCode = "&ordf;"
Case "ba"
HTMLCode = "&ordm;"
Case "ac"
HTMLCode = "&not;"
Case "ad"
HTMLCode = "&shy;"
Case "af"
HTMLCode = "&macr;"
Case "b0"
HTMLCode = "&deg;"
Case "b9"
HTMLCode = "&sup1;"
Case "b2"
HTMLCode = "&sup2;"
Case "b3"
HTMLCode = "&sup3;"
Case "bc"
HTMLCode = "&frac14;"
Case "bd"
HTMLCode = "&frac12;"
Case "be"
HTMLCode = "&frac34;"
Case "d7"
HTMLCode = "&times;"
Case "f7"
HTMLCode = "&divide;"
Case "a2"
HTMLCode = "&cent;"
Case "a3"
HTMLCode = "&pound;"
Case "a4"
HTMLCode = "&curren;"
Case "a5"
HTMLCode = "&yen;"
Case "85"
HTMLCode = "..."
End Select
End Function

Function TrimifCmd(ByVal strTmp As String) As String
Dim l As Long
    
l = 1
While Mid(strTmp, l, 1) = " "
l = l + 1
Wend
If Mid(strTmp, l, 1) = "\" Or Mid(strTmp, l, 1) = "
{
" Then
strTmp = Trim(strTmp)
Else
If Left(strTmp, 1) = " " Then strTmp = Mid(strTmp, 2)
strTmp = RTrim(strTmp)
End If
TrimifCmd = strTmp
End Function
*/
	#endregion
}                                                          
#endregion