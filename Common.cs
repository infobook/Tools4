using System;
using System.Reflection;  
using System.Runtime.InteropServices;  
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace CommandAS.Tools
{

	/// <summary>
	/// Перечиление стандартных команд.
	/// Битовые, чтоб была возможность,где это нужно, делать совмещенные,
	/// например Add|Rename.
	/// </summary>
	public enum eCommand 
	{
		None			= 0,

		Show			= 0x00000001, // показать
		Choice		= 0x00000002, // выбрать

		Add				= 0x00000004, 
		Edit			= 0x00000008, // переименовать(Rename)/редактировать(Edit)
		Property	= 0x00000010,
		Delete		= 0x00000020,

		Print			= 0x00000040,

		Save			=	0x00000080,
		AddFolder = 0x00000100,
		Find			= 0x00000200,

		MoveUp		= 0x00000400,
		MoveDown	= 0x00000800,

		Import		= 0x00001000,
		Export		= 0x00002000,

		AddLink		= 0x00004000	//добавить ссылку

		// Удалить ссылку на объект(но не физически объект)
		//DeleteLink = 1024,

};

	public class CasObjectEventArgs: EventArgs
	{
		public object	pObject;
		public int		pInt;

		public CasObjectEventArgs(): this(null, 0){}
		public CasObjectEventArgs(object aObject, int aInt)
		{
			pObject = aObject;
			pInt		= aInt;
		}
	}

	public delegate void CasObjectEventHandler(object sender, CasObjectEventArgs e);

	public class CASTools
	{
		//нельзя создать экземпляр класса !!!
		private CASTools()
		{
		}
    static public string Date2StringSQL(DateTime aDate)
    {
      //return "'"+aDate.Month.ToString()+"/"+aDate.Day.ToString()+"/"+aDate.Year.ToString()+"'";   
      return "'"+aDate.ToShortDateString()+"'";  
    }

    /// <summary>
    /// Получить DateTime с нулевым (00:00.000) временем
    /// </summary>
    /// <param name="aDate">исходная дата с временем</param>
    /// <returns>дату с нулевым временем</returns>
    static public DateTime DateWithoutTime (DateTime aDate)
    {
      return new DateTime(aDate.Year, aDate.Month, aDate.Day);
			//return aDate.Date;
    }

    /// <summary>
    /// Получить строку в формате dd.MM.yyyy
    /// </summary>
    /// <param name="aDate">исходная дата с временем</param>
    /// <returns>дату с нулевым временем</returns>
    static public string DateWithFullYear (DateTime value)
    {
      string aSep=System.Globalization.DateTimeFormatInfo.CurrentInfo.DateSeparator;
      return value.Day.ToString("00")+aSep+value.Month.ToString("00")+aSep+value.Year.ToString("0000");   
    }
    /// <summary>
		/// Получить DateTime с заданной датой и временем
		/// </summary>
		/// <param name="aDate">исходная дата (время отбрасывается)</param>
		/// <param name="aTime">исходная время</param>
		/// <returns>дата</returns>
		static public DateTime DatePlusTime (DateTime aDate, TimeSpan aTime)
		{
			return new DateTime(aDate.Year, aDate.Month, aDate.Day, aTime.Hours, aTime.Minutes, aTime.Seconds, aTime.Milliseconds);   
		}
    /// <summary>
    /// Получить DateTime с заданной датой и временем (часы:минуты), отбросив секунды и миллисекунды
    /// </summary>
    /// <param name="aDate">исходная дата (без времени)</param>
    /// <param name="aTime">исходное время</param>
    /// <returns></returns>
    static public DateTime DatePlusTimeSmall(DateTime aDate, TimeSpan aTime)
    {
      return new DateTime(aDate.Year, aDate.Month, aDate.Day, aTime.Hours, aTime.Minutes, 0, 0);
    }
    /// <summary>
    /// Получить DateTime с самым позднем (в секундах) временем.
    /// </summary>
    /// <param name="aDate">исходная дата</param>
    /// <returns>дату с максимальным (в секундах) временем</returns>
    static public DateTime DateLatestTime (DateTime aDate)
    {
      return (DateWithoutTime(aDate).AddDays(1)).AddSeconds(-1); 
    }

    //  конвертирование месяца в родительный падеж по номеру
    [DebuggerStepThrough]
    static public string ConvertMonthToGenitiveCase(int aMonthID)
    {
        switch (aMonthID)
        {
            case 1: return "января";
            case 2: return "февраля";
            case 3: return "марта";
            case 4: return "апреля";
            case 5: return "мая";
            case 6: return "июня";
            case 7: return "июля";
            case 8: return "августа";
            case 9: return "сентября";
            case 10: return "октября";
            case 11: return "ноября";
            case 12: return "декабря";
        }
        return string.Empty;
    }
    static public string ConvertMonthToNominativeCase(int aMonthID)
    {
      switch (aMonthID)
      {
        case 1: return "январь";
        case 2: return "февраль";
        case 3: return "март";
        case 4: return "апрель";
        case 5: return "май";
        case 6: return "июнь";
        case 7: return "июль";
        case 8: return "август";
        case 9: return "сентябрь";
        case 10: return "октябрь";
        case 11: return "ноябрь";
        case 12: return "декабрь";
      }
      return string.Empty;
    }  

		static public DateTime DateLastDayOfMonth (int aYear, int aMonth)
		{
      if (aMonth < 12)
        return (new DateTime(aYear, aMonth, 1)).AddMonths(1).AddDays(-1);
      else
			  return new DateTime(aYear, aMonth, 31);
		}

		[DebuggerStepThrough]
    static public int ConvertToInt32Or0(object aObj)
    {
      int ret = 0;
      try
      {
        ret = Convert.ToInt32(aObj);
      }
      catch {}
      return ret;
    }
		[DebuggerStepThrough]
		static public int ConvertToInt32Or0(string aString)
    {
      int ret = 0;
      try
      {
        ret = (int)ConvertToUInt32Or0(aString);
      }
      catch {}
      return ret;
    }
    static public uint ConvertToUInt32Or0(string aString)
    {
      uint ret = 0;
      try
      {
        if (aString.IndexOf("-")!=-1)
          ret = (uint)Convert.ToInt32(aString);
        else
          ret = Convert.ToUInt32(aString);
      }
      catch {}
      return ret;
    }
		[DebuggerStepThrough]
		static public long ConvertToInt64Or0(string aString)
    {
      long ret = 0;
      try
      {
        ret = Convert.ToInt64(aString);
      }
      catch {}
      return ret;
    }
    static public ulong ConvertToUInt64Or0(string aString)
    {
      ulong ret = 0;
      try
      {
        ret = (ulong)Convert.ToInt64(aString);
      }
      catch {}
      return ret;
    }

		
    public static string GetButtonIcon(eCommand aCmd)
    {
      string ret = "9";
      switch (aCmd)
      {
        case eCommand.Add:
          ret = ((char)110).ToString();
          break;
        case eCommand.Edit:
          ret = ((char)0x61).ToString();
          break;
        case eCommand.Delete:
          ret = ((char)0x72).ToString();
          break;
        case eCommand.Choice:
          ret = ((char)0x76).ToString();
          break;

				case eCommand.Save:
					ret = ((char)0x32).ToString();
					break;

				case eCommand.AddLink:
					ret = ((char)0x37).ToString();
					break;

				case eCommand.Property:
          ret = "2";
          break;
      }
      return ret;
    }

    public static void SetCommandButton(Button aButton, eCommand aCmd)
    {
      aButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      aButton.Font = new Font("Marlett",8,FontStyle.Bold);
      switch (aCmd)
      {
        case eCommand.Add:
          aButton.Text = ((char)110).ToString();
          aButton.ForeColor = Color.White;  
          break;
        case eCommand.Edit:
          aButton.Text = ((char)0x61).ToString();
          aButton.ForeColor = Color.Blue;  
          break;
        case eCommand.Delete:
          aButton.Text = ((char)0x72).ToString();
          aButton.ForeColor = Color.Red;
          break;
        case eCommand.Choice:
          aButton.Text = ((char)0x76).ToString();
          aButton.ForeColor = Color.Green;
          break;
        case eCommand.Property:
          aButton.Text = "2";
          aButton.ForeColor = Color.Yellow;
          break;
      }
    }

		public static void DataTableDeleteAllEmptyRow(DataView aDView, DataTable aDTable)
		{
			for(int iRow = 0; iRow < aDView.Count-1; iRow++) 
			{
				string rowVal = string.Empty; 
				foreach (DataColumn dCol in aDTable.Columns)
				{
					object val=aDView[iRow][dCol.Ordinal];
					if (!val.Equals(dCol.DefaultValue))
						rowVal+= val.ToString();
				}
				if (rowVal.Length == 0)
					aDView.Delete(iRow); 
				else
					break;
			}
		}
    public static void DataTableDeleteLastEmptyRow(DataView aDView, int aCurrentRowIndex, DataTable aDTable)
    {
      for(int iRow = aDView.Count-1; iRow > aCurrentRowIndex; iRow--) 
      {
        string rowVal = string.Empty; 
        foreach (DataColumn dCol in aDTable.Columns)
          rowVal += aDView[iRow][dCol.Ordinal].ToString().Equals("0")?string.Empty:aDView[iRow][dCol.Ordinal].ToString();
        if (rowVal.Length == 0)
          aDView.Delete(iRow); 
        else
          break;
      }
    }

    public static double DistanceBetweenPoint (Point aP1, Point aP2)
    {
      int a = aP1.X-aP2.X;
      int b = aP1.Y-aP2.Y;
      return Math.Sqrt(a*a+b*b);
    }
	
    public static bool IsEqualByByte(string aStr1, string aStr2)
    {
      //byte[] a = aStr1;
      //byte[] b = aStr2;
      //return a.Equals(b);
      return aStr1.GetHashCode() == aStr2.GetHashCode();
    }

    /// <summary>
    /// Функция нормализует Rectangle Control (form)
    /// </summary>
    /// <param name="owner">Элемент под которым размещаемся!</param>
    /// <param name="slave">Элемент, который размещаем!</param>
    /// <returns></returns>
    public static Rectangle GetBoundsControl(Control Owner,Control slave)
    {
      return GetBoundsControl(Owner.RectangleToScreen(Owner.Bounds),slave);
    }
    /// <summary>
    /// Функция нормализует Rectangle Control (form)
    /// </summary>
    /// <param name="ownerSceen">Rectanle эелемента под которым размещаемся в координатах экрана!</param>
    /// <param name="slave">Элемент, который размещаем!</param>
    /// <returns></returns>
    public static Rectangle GetBoundsControl(Rectangle ownerScreen,Control slave)
    {
      Rectangle ret=slave.Bounds; //slave.RectangleToScreen(slave.Bounds);
      int delta=4;
      //узнаем координаты в экране
      Point p=ownerScreen.Location;
      //устанавливаем под или над контролом - владельцем?
      if (p.Y+ownerScreen.Height+slave.Height+delta>Screen.PrimaryScreen.WorkingArea.Height) //Screen.GetBounds(null).Height
        //смещаем наверх
        p.Offset(0,-(ownerScreen.Height+slave.Height));
      //устанавливаем координаты (если изменилось положение на экране)
      if (ret.Y!=p.Y+ownerScreen.Height)
        ret.Y=p.Y+ownerScreen.Height;
      if(ret.X!=p.X)
        ret.X=p.X;
      //если ушли за пределы экрана справа
      if(ret.Left+slave.Width+delta>Screen.PrimaryScreen.WorkingArea.Width) //Screen.GetBounds(owner).Width
        ret.X=Screen.PrimaryScreen.WorkingArea.Width-ret.Width-delta;
      //нормализуем !
      if (ret.X<0)
        ret.X=0;
      if (ret.Y<0)
        ret.Y=0;
      if (ret.Width<20)
        ret.Width=20;
      if (ret.Height<20)
        ret.Height=20;
      return ret;
    }

		public static TreeNode SearchByTextInTreeNodeCollection(TreeNodeCollection aTnc, string aFindText, ref int aFindCount, bool isMatchCase)
		{
			TreeNode retNd = null;
			foreach (TreeNode tn in aTnc)
			{
				if (tn.Nodes.Count > 0)
					retNd = SearchByTextInTreeNodeCollection(tn.Nodes, aFindText, ref aFindCount, isMatchCase);
				if (retNd != null)
				{
					if (aFindCount>0)
					{
						aFindCount--;
						retNd = null;
					}
					else
						break;
				}
				if (isMatchCase)
				{
					if (tn.Text.StartsWith(aFindText))
					{
						if (aFindCount>0)
							aFindCount--;
						else
						{
							retNd = tn;
							break;
						}
					}
				}
				else
				{
					if (tn.Text.ToUpper().StartsWith(aFindText.ToUpper()))
					{
						if (aFindCount>0)
							aFindCount--;
						else
						{
							retNd = tn;
							break;
						}
					}
				}
			}
			return retNd;
		}

		public static void MessageSorryDoItLater()
		{
			MessageBox.Show("Извините!!! Данная операция будет реализована позже.","",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

		public static string GetCorrectFolder(string aText)
		{
			if (aText.Length == 0)
				return @"\";

			if (aText.ToCharArray()[aText.Length-1] == '\\')
				return aText;
			else
				return aText+@"\";
		}


		public static string AddWordToEndIfNotPresent(string aTxt, string aWord)
		{
			if (aTxt.Length == 0)
				return aWord;

			if (!aTxt.Substring(aTxt.Length-aWord.Length).Equals(aWord))
				aTxt += aWord;

			return aTxt;
		}

		public static string RemoveWordFromEndIfNotPresent(string aTxt, string aWord)
		{
			if (aTxt.Length > 0)
			{
				int ii = aTxt.LastIndexOf(aWord);
				if (ii >= 0 && aTxt.Length-ii == aWord.Length)
					aTxt = aTxt.Substring(0,ii);
			}
			return aTxt;
		}

		public static string StringsDelimSymbol(params string[] list)
		{
			if (list.Length < 2)
				return string.Empty;

			string ret = list[1];
			
			for ( int ii = 2; ii < list.Length; ii++ )
			{
				if (list[ii].Length > 0)
					ret += list[0]+list[ii];
			}

			return ret;
		}

		public static string GetModifiedTitle(string aText, bool aModified)
		{
			string ret = aText;
			string lastSymb = string.Empty;

			if (aText.Length > 0) 
				lastSymb = aText.Substring(aText.Length-1,1);

			if (aModified)
			{
				if (!lastSymb.Equals("*"))
					ret = aText+"*";
			}
			else
			{
				if (lastSymb.Equals("*"))
					ret = aText.Substring(0,aText.Length-1);
			}
			
			return ret;
		}

		public static string PathWithEndSeparator(string aPath)
		{
			return
				aPath.EndsWith(Path.DirectorySeparatorChar.ToString()) ?
			aPath : aPath + Path.DirectorySeparatorChar;
		}
	}

	
	/// <summary>
	/// Набор вспомогательных функций работы с реестром.
	/// M.Tor
	/// 15.09.2005
	/// </summary>
	public class CASToolsReg
	{

		protected CASToolsReg(){}

		public static void LoadDataGridParameter(RegistryKey aRegkey, DataGridTableStyle aDgrStyle, string aTabName)
		{
			try
			{
				string tmpStr = aRegkey.GetValue(aTabName+"ColWidth").ToString();
				if (tmpStr.Length > 0 && aDgrStyle != null)
				{
					string[] aTT = tmpStr.Split(new char[] {';'});
					for (int ii=0; ii < aTT.Length; ii++)
						aDgrStyle.GridColumnStyles[ii].Width = Convert.ToInt32(aTT[ii]);
				}
			}
			catch{}
		}
    public static void LoadDataGridParameter(RegistryKey aRegkey, DataGridView aDGV, string aTabName)
    {
      try
      {
        string tmpStr = aRegkey.GetValue(aTabName + "ColWidth").ToString();
        if (tmpStr.Length > 0 && aDGV != null)
        {
          string[] aTT = tmpStr.Split(new char[] { ';' });
          for (int ii = 0; ii < aTT.Length; ii++)
            aDGV.Columns[ii].Width = Convert.ToInt32(aTT[ii]);
        }
      }
      catch { }
    }

    public static void SaveDataGridParameter(RegistryKey aRegkey, DataGridTableStyle aDgrStyle, string aTabName)
		{
			if (aRegkey == null) 
				return;

			string tabColWidth = string.Empty;
			if (aDgrStyle != null)
			{
				for (int ii = 0; ii < aDgrStyle.GridColumnStyles.Count-1; ii++)
					tabColWidth += aDgrStyle.GridColumnStyles[ii].Width.ToString() + ";";
				tabColWidth += aDgrStyle.GridColumnStyles[aDgrStyle.GridColumnStyles.Count-1].Width.ToString();
				aRegkey.SetValue(aTabName+"ColWidth", tabColWidth);
			}
		}

    public static void SaveDataGridParameter(RegistryKey aRegkey, DataGridView aDGV, string aTabName)
    {
      if (aRegkey == null)
        return;

      string tabColWidth = string.Empty;
      if (aDGV != null && aDGV.Columns.Count > 0)
      {
        for (int ii = 0; ii < aDGV.Columns.Count - 1; ii++)
          tabColWidth += aDGV.Columns[ii].Width.ToString() + ";";
        tabColWidth += aDGV.Columns[aDGV.Columns.Count - 1].Width.ToString();
        aRegkey.SetValue(aTabName + "ColWidth", tabColWidth);
      }
    }

    public static void LoadSizeLocationMaximizeForm(RegistryKey aRegkey, Form aForm)
    {
      if ((int)aRegkey.GetValue("FormWindowStateMaximized") == 1)
        aForm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      else
        LoadSizeLocationForm(aRegkey, aForm);
    }
    public static void LoadSizeLocationForm(RegistryKey aRegkey, Form aForm)
    {
      string[] aStr = aRegkey.GetValue("FormSize").ToString().Split('|');
      if (aStr.Length == 2)
        aForm.Size = new Size(Convert.ToInt32(aStr[0]), Convert.ToInt32(aStr[1]));
      aStr = aRegkey.GetValue("FormLocation").ToString().Split('|');
      if (aStr.Length == 2)
        aForm.Location = new Point(Convert.ToInt32(aStr[0]), Convert.ToInt32(aStr[1]));
    }

    public static void SaveSizeLocationMaximizeForm(RegistryKey aRegkey, Form aForm)
    {

      if (aForm.WindowState == System.Windows.Forms.FormWindowState.Normal)
      {
        SaveSizeLocationForm(aRegkey, aForm);
        aRegkey.SetValue("FormWindowStateMaximized", 0);
      }
      else if (aForm.WindowState == System.Windows.Forms.FormWindowState.Maximized)
      {
        aRegkey.SetValue("FormWindowStateMaximized", 1);
      }
    }
    public static void SaveSizeLocationForm(RegistryKey aRegkey, Form aForm)
    {
      aRegkey.SetValue("FormSize", aForm.Size.Width + "|" + aForm.Size.Height);
      aRegkey.SetValue("FormLocation", aForm.Location.X + "|" + aForm.Location.Y);
    }

  }
	
  /// <summary>
	/// Из статьи "Автоматическое управление памятью в .NET"
	/// Автор: Игорь Ткачев	
	/// The RSDN Group
	/// Источник: RSDN Magazine #1
	/// </summary>

	[AttributeUsage(AttributeTargets.Field)]
	public class UsingAttribute : Attribute 
	{
	}

	public abstract class DisposableType: IDisposable
	{
		bool disposed = false;

		~DisposableType()
		{
			if (!disposed) 
			{
				disposed = true;
				Dispose(false);
			}
		}

		public void Dispose()
		{
			if (!disposed) 
			{
				disposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			DisposeFields(this);
		}

		static public void DisposeFields(object obj)
		{
			foreach (
				FieldInfo field in 
				obj.GetType().GetFields(BindingFlags.Public | 
				BindingFlags.NonPublic |
				BindingFlags.Instance))
			{
				if (Attribute.IsDefined(field,typeof(UsingAttribute))) 
				{
					object val = field.GetValue(obj);
					if (val != null)
					{
						if (val is IDisposable)
						{
							((IDisposable)val).Dispose();
						}
						else if (val.GetType().IsCOMObject)
						{
							// на всякий случай добавим сюда 
							// и освобождение COM-объектов
							Marshal.ReleaseComObject(val);
						}
					}
				}
			}
		}
	}
}
