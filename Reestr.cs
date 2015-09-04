using System;
using Microsoft.Win32;

namespace CommandAS.Tools
{
	/// <summary>
	/// 
	/// </summary>
	public class Reestr
	{
		public Reestr()
		{
		}

		public static string GetString(RegistryKey aRegkey, string aKeyName)
		{
			object gv = aRegkey.GetValue(aKeyName);
			if (gv != null)
				return gv.ToString();
			else
				return string.Empty;
		}

		public static bool GetBoolean(RegistryKey aRegkey, string aKeyName)
		{
			object gv = aRegkey.GetValue(aKeyName);
			if (gv != null)
				return Convert.ToBoolean(gv);
			else
				return false;
		}
	}
}
