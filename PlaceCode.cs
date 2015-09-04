using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace CommandAS.Tools
{
  /// <summary>
  /// Структура для хранения составного идентификатора.
  /// Состоит из двух целых чисел (Int32 - 4 байта) - place и code.
  /// </summary>
  public struct PlaceCode
  {
    /// <summary>
    /// Константа разделителя для строкового представления идентификатора
    /// в виде "place[DELIM]code"
    /// </summary>
    public const char DELIM = ':';

    /// <summary>
    /// Первая составляющая идентификатора.
    /// </summary>
    public int place;
    /// <summary>
    /// Вторая составляющая идентификатора.
    /// </summary>
    public int code;
    /// <summary>
    /// Признак корректных кодов Place и Code
    /// </summary>
    public bool IsDefined
    {
      [DebuggerStepThrough]
      get { return place > 0 && code > 0; }
    }
    /// <summary>
    /// Признак новой записи!
    /// </summary>
    public bool IsNew
    {
      [DebuggerStepThrough]
      get { return place >= 0 && code == 0; }
    }

    public bool IsPlaceOrCodeZero
    {
      [DebuggerStepThrough]
      get { return (place == 0 || code == 0); }
    }

    /// <summary>
    /// Строковое представление place code через разделитель DELIM.
    /// Строка вида "place[DELIM]code". 
    /// Например для DELIM = ':' - "99:7" или "2:375"
    /// </summary>
    [XmlIgnoreAttribute]  // <- add M.Tor 28.08.2014
    public string PlaceDelimCode
    {
      [DebuggerStepThrough]
      get
      {
        return PlaceCode2PDC(this);
      }
      [DebuggerStepThrough]
      set
      {
        this = PDC2PlaceCode(value);
      }
    }

    //public PlaceCode (): this (0, 0) {} - для структур НЕЛЬЗЯ !!!
    [DebuggerStepThrough]
    public PlaceCode(int aPlace, int aCode)
    {
      place = aPlace;
      code = aCode;
    }
    [DebuggerStepThrough]
    public PlaceCode(object aPlace, object aCode)
    {
      place = CASTools.ConvertToInt32Or0(aPlace);
      code = CASTools.ConvertToInt32Or0(aCode);
    }

    /// <summary>
    /// Метод перегружен для "умения" сравнивать объекты типа PlaceCode
    /// </summary>
    /// <param name="pc">PlaceCode</param>
    /// <returns>Если одинаковы - истина</returns>
    public override bool Equals(object pc)
    {
      if (pc.GetType() == typeof(PlaceCode))
      {
        PlaceCode PC = (PlaceCode)pc;
        return this.code == PC.code && this.place == PC.place;
      }
      else
        return false;
    }

    public static bool operator ==(PlaceCode aPC1, PlaceCode aPC2)
    {
      return aPC1.Equals(aPC2);
    }

    public static bool operator !=(PlaceCode aPC1, PlaceCode aPC2)
    {
      return !aPC1.Equals(aPC2);
    }
    /// <summary>
    /// Перегружаем для правильной работы
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "place=" + place + ", code=" + code;
    }
    /// <summary>
    /// Перегруженный метод
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return this.ToString().GetHashCode();
      //return 0;
    }


    [DebuggerStepThrough]
    public static string PlaceCode2PDC(PlaceCode aPC)
    {
      return aPC.place.ToString() + PlaceCode.DELIM + aPC.code.ToString();
    }
    [DebuggerStepThrough]
    public static PlaceCode PDC2PlaceCode(string aPDC)
    {
      PlaceCode pc = new PlaceCode(0, 0);
      if (aPDC != null && aPDC.Length > 0)
      {
        string[] s = aPDC.Split(DELIM);
        if (s.Length == 2)
        {
          pc.place = Convert.ToInt32(s[0]);
          pc.code = Convert.ToInt32(s[1]);
        }
      }
      return pc;
    }
    /// <summary>
    /// Возвращаем пустую структуру
    /// </summary>
    public static readonly PlaceCode Empty = new PlaceCode(0, 0);
  }
}
