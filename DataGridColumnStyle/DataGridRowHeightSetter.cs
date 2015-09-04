using System;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;


namespace CommandAS.Tools.DataGridColumnStyle
{
	/// <summary>
	/// Класс работает с высотой строк в DataGrid
	/// </summary>
	public class DataGridRowHeightSetter
	{
		private DataGrid dg;
		private ArrayList rowObjects;	

		public DataGridRowHeightSetter(DataGrid dg)
		{
			this.dg = dg;
			InitHeights();
		}

		private void InitHeights()
		{
			MethodInfo mi = dg.GetType().GetMethod("get_DataGridRows",BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

			System.Array dgra = (System.Array)mi.Invoke(this.dg,null); 

			rowObjects = new ArrayList(); 
			foreach (object dgrr in dgra) 
			{ 
				if (dgrr.ToString().EndsWith("DataGridRelationshipRow")==true) 
					rowObjects.Add(dgrr); 
			} 
		}

		public int this[int row]
		{
			get
			{
				try
				{
					PropertyInfo pi = rowObjects[row].GetType().GetProperty("Height"); 
					return (int) pi.GetValue(rowObjects[row], null);
				}
				catch
				{
					throw new ArgumentException("invalid row index");
				}
			}
			set
			{
				try
				{
					PropertyInfo pi = rowObjects[row].GetType().GetProperty("Height"); 
					pi.SetValue(rowObjects[row], value, null); 
				}
				catch
				{
					throw new ArgumentException("invalid row index");
				}
			}
		}
	}
}
