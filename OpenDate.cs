using System;

namespace CommandAS.Tools
{
	/// <summary>
	/// Класс "Открытая Дата" - дата в которой допустимо определение части информации.
  /// Например известен только год а день и месяц нет или наоборот известен день и месяц, 
  /// а год нет.
	/// </summary>
	public class OpenDate : ICloneable
	{
		protected int mDay;
		protected int mMonth;
		protected int mYear;

		/// <summary>
		/// дата
		/// если не определена, то 0
		/// </summary>
		public int pDay
		{
			get { return mDay; }
			set
			{
				if (value > 0 && value < 32)
					mDay = value;
				else
					mDay = 0;
			}
		}
		/// <summary>
		/// месяц
		/// если не определен, то 0
		/// </summary>
		public int pMonth
		{
			get { return mMonth; }
			set
			{
				if (value > 0 && value < 13)
					mMonth = value;
				else
					mMonth = 0;
			}
		}
		/// <summary>
		/// год
		/// если не определен, то 0
		/// </summary>
		public int pYear
		{
			get { return mYear; }
			set	{	mYear=value;  }
		}

    public object	pDateTime
    {
      get
      {
        object ret=null;
        if (mYear>0 && mMonth>0 && mDay>0)
          ret= new DateTime(mYear,mMonth,mDay);
        return ret;
      }      
      set
      {
        bool error=false;
        try
        {
          /*
          string[] expectedFormats = {"D", "d","G", "g", "f", "F"}; //смотрим краткую или полную дату!
          IFormatProvider format = new System.Globalization.CultureInfo("ru-RU");
          string[] str=val.Split(System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator.ToCharArray());
          */
          //DateTime dt=new DateTime(int.Parse(str[2]),int.Parse(str[1]),int.Parse(str[0])); //DateTime.ParseExact(value.ToString(),"d.mm.yyyy",format);
          DateTime dt = (DateTime) value;//DateTime.ParseExact(val,"D",format);
          mYear		= dt.Year;
          mMonth	= dt.Month; 
          mDay		= dt.Day;
        }
        catch //(Exception ex)
        {
//          System.Windows.Forms.MessageBox.Show(ex.Message,"Ошибка !");
          error=true;
          Init();
        }
        if (error) //если предыдущий разбор неверен !!!
        {
          try
          {
            pString=value.ToString();
          }
          catch
          {
            Init();
          }
        }
      }
    }
		/// <summary>
		/// Дата в формате DateTime
		/// Если какая та часть даты не указана (0), возращает DateTime.MinValue.
		/// </summary>
		public System.DateTime	pDate
		{
			get 
			{
				DateTime dt = DateTime.MinValue;
				try
				{
					dt = new DateTime(mYear, mMonth, mDay);
				}
				catch{}
				return dt; 
			}
			set 
			{ 
				mYear		= value.Year;
				mMonth	= value.Month; 
				mDay		= value.Day;
			}
		}

		/// <summary>
		/// Дата в строке формата YYYYMMDD
		/// </summary>
		public string	pString
		{
			get 
			{
				return GetString(mYear, mMonth, mDay);
			}
			set
			{
        Init();
				if(value==null)
					value=string.Empty;

				if (value.Length > 7)
				{
					mYear = CASTools.ConvertToInt32Or0(value.Substring(0,4));
					if (mYear == 0)
						mYear = CASTools.ConvertToInt32Or0(value.Substring(2,2));
					mMonth = CASTools.ConvertToInt32Or0(value.Substring(4,2));
					mDay = CASTools.ConvertToInt32Or0(value.Substring(6,2));
				}
			}
		}


		public string pFormatString
		{
			get 
			{
				return GetFormatString(System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator);
			}
      set
      {
        SetOpenDate(value, System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator);
      }
		}

		public int					pLength
		{
			get { return 8; }
		}

		public bool					pIsValue
		{
			get 
			{
				return mDay>0 || mMonth>0 || mYear> 0;
			}
		}

		public OpenDate()
		{
			Init();
		}

		public OpenDate(int aYear, int aMonth, int aDay)
		{
			pYear		= aYear;
			pMonth	= aMonth;
			pDay		= aDay;
		}
    public OpenDate(string pOpenDate)
    {
      pString=pOpenDate;
    }
		public void Init()
		{
			mYear		= 0;
			mMonth	= 0;
			mDay		= 0;
		}

    public static string GetString(string aDT)
    {
      return GetString(aDT, System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator);
    }
    public static string GetString(string aDT, string aSep)
    {
      string ret = string.Empty;
      DateTime dt = DateTime.MinValue;
      try
      {
        dt = Convert.ToDateTime(aDT);
        ret = GetString(dt);
      }
      catch { }

      if (ret.Length == 0)
      {
        string[] ss = aDT.Split(aSep.ToCharArray());
        if (ss.Length == 3)
        {
          ret = GetString(CASTools.ConvertToInt32Or0(ss[2]), CASTools.ConvertToInt32Or0(ss[1]), CASTools.ConvertToInt32Or0(ss[0]));
        }
      }

      return ret;
    }

    public static string GetString(DateTime aDT)
    {
      return GetString(aDT.Year, aDT.Month, aDT.Day);
    }

		public static string GetString(int aYear, int aMonth, int aDay)
		{
			string ret =
				"YYYY"+(aYear==0?string.Empty:aYear.ToString("00"))+
				(aMonth==0?"MM":aMonth.ToString("00"))+
				(aDay==0?"DD":aDay.ToString("00"));
			return ret.Substring(ret.Length-8); 
		}
    /// <summary>
    /// Разбираем строку для просмотра по нашему !!!
    /// </summary>
    /// <param name="aString"></param>
    /// <returns></returns>
    public static string GetRusFromOpenDate(string aString)
    {
      string ret=string.Empty;
      if (aString!=null && aString.Length > 7)
      {
        int mYear = CASTools.ConvertToInt32Or0(aString.Substring(0,4));
        if (mYear == 0)
          mYear = CASTools.ConvertToInt32Or0(aString.Substring(2,2));
        int mMonth = CASTools.ConvertToInt32Or0(aString.Substring(4,2));
        int mDay = CASTools.ConvertToInt32Or0(aString.Substring(6,2));
        string aSep= System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator;
        string sp = new String('_',2);
        ret=(mDay>0 ? mDay.ToString("00") : sp)+aSep+
          (mMonth>0 ? mMonth.ToString("00") : sp)+aSep+
          (mYear>0 ? mYear.ToString() : sp);

      }
      return ret;
    }

		public string GetFormatString(string aSep)
		{
			string sp = new String('_',2);
			return 
					(mDay>0 ? mDay.ToString("00") : sp)+aSep+
					(mMonth>0 ? mMonth.ToString("00") : sp)+aSep+
					(mYear>0 ? mYear.ToString() : sp);
		}

    public void SetOpenDate(string aDate, string aSep)
    {
      string[] ss = aDate.Split(aSep.ToCharArray());
      if (ss.Length == 3)
      {
        mDay = CASTools.ConvertToInt32Or0(ss[0]);
        mMonth = CASTools.ConvertToInt32Or0(ss[1]);
        mYear = CASTools.ConvertToInt32Or0(ss[2]);
      }
    }

    /// <summary>
    /// Проверяет дату на корректность и исправляет если возможно.
    /// </summary>
    /// <returns>
    /// true - дата была корректна и не исправлялась
    /// false - дата была некорректна и исправлялась
    /// </returns>
    public bool CheckAndCorrect()
    {
      bool ret = mDay < 32;


      if (!ret)
        mDay = 31;

      if (mMonth > 12)
      {
        mMonth = 12;
        ret = false;
      }

      switch (mMonth)
      {
        case 0:
          break;
        case 2:
          if (mDay > 28 && mYear > 0)
          {
            int d = new DateTime(mYear, 3, 1).AddDays(-1).Day;
            if (ret)
              ret = (mDay == d);
            mDay = d;
          }
          break;
        case 4:
        case 6:
        case 9:
        case 11:
          if (mDay == 31)
          {
            ret = false;
            mDay = 30;
          }
          break;
      }

      return ret;
    }

    public virtual object Clone()
    {
      OpenDate od=(OpenDate) base.MemberwiseClone();
      od.mDay=mDay;
      od.mMonth=mMonth;
      od.mYear=mYear;

      return od;
    }
	}
}
