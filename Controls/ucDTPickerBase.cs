using System;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// Summary description for ucDTPickerBase.
	/// </summary>
	public class ucDTPickerBase : System.Windows.Forms.DateTimePicker
	{

		public ucDTPickerBase():base(){}
		public new object Value
		{
			get
			{
				return base.Value as object;
			}
			set
			{
				if (value==null)
					base.Value=CommonConst.DATE_MINIMUM;
				else if (value is DateTime)
					base.Value=(DateTime)value;
			}
		}
	}
}
