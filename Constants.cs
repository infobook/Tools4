using System;

namespace CommandAS.Tools
{
	/// <summary>
	/// Набор общих констант:
	/// </summary>
	public class CommonConst
	{
		private CommonConst()	{}

		public const string PleaseWait = "Подождите пожалуйста!";

		/// <summary>
		/// Минимальная дата для DateTimePicker
		/// </summary>
		public static DateTime DATE_MINIMUM
		{
			get 
			{	
				return(new DateTime(1900,1,1)); 
			}
		}

		public const string OPENFILEDIALOG_FILTER_IMAGE = 
			"Все файлы с изображением|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.jfif;*.png;*.tif;*.tiff|" +
			"Windows Bitmap (*.bmp)|*.bmp|" +
			"Windows Icon (*.ico)|*.ico|" +
			"Graphics Interchange Format (*.gif)|*.gif|" +
			"JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg;*.jfif|" +
			"Portable Network Graphics (*.png)|*.png|" +
			"Tag Image File Format (*.tif)|*.tif;*.tiff|" +
			"All Files (*.*)|*.*";
	}
}
