using System;
using System.Windows.Forms;
using System.Data.OleDb;
using Microsoft.Win32;

namespace CommandAS.Tools
{
	/// <summary>
	/// Summary description for OleDBTools.
	/// </summary>
	public class OleDBTools
	{
		private OleDBTools()
		{
		}
		public static OleDbConnection OpenDBConnection(string aRegKeyStr)
		{
			if (aRegKeyStr==null || aRegKeyStr==string.Empty)
				return null;
			//Error.ShowError("Макл ! Не забудь поправить прямой вызов реестра ! Не хорошо это ...");
			return OpenDBConnection(Registry.CurrentUser.CreateSubKey(aRegKeyStr));
		}
		public static OleDbConnection OpenDBConnection(RegistryKey regkey)
		{
			if (regkey==null)
				return null;
			OleDbConnection ret = null;
			string connectionString = string.Empty;
			string tmpStr = regkey.GetValue("Server") as string;;

			if (tmpStr == null)
			{
				MessageBox.Show ("Не определена группа ключей "+regkey.ToString()+"Server");
				return ret;
			}

			try
			{
				if (tmpStr.Length>0)
				{ // указан сервер, значит БД SQL
					connectionString = 
						"Provider=SQLOLEDB; Server="+tmpStr+";"
						+ "database="+regkey.GetValue("DBName").ToString()+";"
						+ "Integrated Security=SSPI; Persist Security Info=false;";
				}
				else
				{ // не указан сервер, значит БД Access
					connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;"+
						"Data Source="+regkey.GetValue("DBName").ToString()+";";
				}
			}
			catch{}


			ret = new OleDbConnection(connectionString);
			try
			{
				ret.Open();
			}
			catch (Exception ex)
			{
				MessageBox.Show (ex.Message);
				ret = null;
			}
			return ret;
		}

	}
}
