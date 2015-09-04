using System;
using System.Collections;
using System.Data;

namespace CommandAS.Tools
{
	/// <summary>
	/// Перемешивание (тасование) таблиц, списков.
	/// </summary>
	public class Shuffle
	{
		//public Shuffle()
		//{
		//}

		//Ryan T-S helped me out with some code for this, which I wrapped up in the
		//following function:
		/// <summary>
		/// Randomizes the order of the rows in a DataTable by pulling out a single		row and moving it to the end. This
		/// process is repeated 3x per row in the table.
		/// </summary>
		/// <param name="inputTable"></param>
		/// <returns></returns>
		public static DataTable shuffleTable(DataTable inputTable)
		{
			return shuffleTable(inputTable, inputTable.Rows.Count*3);
		}
		/// <summary>
		/// Randomizes the order of the rows in a DataTable by pulling out a single	row and moving it to the end for
		/// shuffleIterations iterations.
		/// </summary>
		/// <param name="inputTable"></param>
		/// <param name="shuffleIterations"></param>
		/// <returns></returns>
		public static DataTable shuffleTable(DataTable inputTable, int	shuffleIterations)
		{
			int index;
			System.Random rnd = new Random();
			// Remove and throw to the end random rows until we have done so n*3 times (shuffles the dataset)
			for(int i = 0; i < shuffleIterations; i++)
			{
				index = rnd.Next(0,inputTable.Rows.Count-1);
				inputTable.Rows.Add(inputTable.Rows[index].ItemArray);
				inputTable.Rows.RemoveAt(index);
			}
			return inputTable;
		}

		///
		/// M.Tor modification shuffleTable
		/// 
		public static ArrayList ShuffleArrayList(ArrayList aInputArray)
		{
			return ShuffleArrayList(aInputArray, aInputArray.Count*3);
		}
		public static ArrayList ShuffleArrayList(ArrayList aInputArray, int	shuffleIterations)
		{
			int index;
			System.Random rnd = new Random();

			for(int i = 0; i < shuffleIterations; i++)
			{
				index = rnd.Next(0,aInputArray.Count);
				aInputArray.Add(aInputArray[index]);
				aInputArray.RemoveAt(index);
			}
			return aInputArray;
		}


	}
}
