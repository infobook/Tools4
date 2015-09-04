using System;
using System.Drawing;
using System.Windows.Forms;

namespace CommandAS.Tools
{
	/// <summary>
	/// Пересчисления имен иконок.
	/// !!! ВАЖНО !!! НЕ ЗАБУДЬ ПОПРАВИТЬ ПЕРЕЧИСЛЕНИЕ eIBCIconName !!! ВАЖНО !!! 
	/// </summary>
	public enum eCCIconName
	{
		Blank											= 0,
		Exit_Power48							= 1,
		Save_Diskette48						= 2,
		About32										= 3,
    ExportImport32						= 4,
		Exit32										= 5,
		Delete_TrashEmpty32				= 6,
		Delete_TrashEmpty48				= 7,
		New_Text48								= 8,
		Edit_Text48								= 9,
		Param48										= 10,
    About48										= 11,
		Filter32									= 12,
		MMDoc											= 13,
		Folder										= 14,
		FolderOpen								= 15,
		Foto											= 16,
    Doc     									= 17,
    DocNew  									= 18,
    DocOpen 									= 19,
    FolderNew									= 20,
    Refresh 									= 21,
    FolderDisable							= 22,
    FolderDisable_Open				= 23,
    Doc_Disable								= 24,
    Doc_Disable_Open					= 25,
    Anketa  									= 26,
		FilterOn									= 27,
		FilterOff									= 28,
		ArrowUp										= 29,
		ArrowDown									= 30,
    Print									    = 31,
    Preview									  = 32,
		Run												=	33,
		Shell32HDLocal						=	34,
		Shell32FolderClose				=	35,
		Shell32FolderOpen					=	36,
		Find											=	37,
		Renumering								=	38,
		NavBackward								=	39,
		NavForward								=	40,
		ViewsPoins								=	41,
		// !!! ДАЛЕЕ ВСТАВЛЕНА ЗАРЕЗЕРВИРОВАННЫЕ ИМЕНА ИКОНОК
		// ПРИ ДОБАВЛЕНИИ ЗАМЕНЯТЬ !!!
		ZRESERVE_42								=	42,
		ZRESERVE_43								=	43,
		ZRESERVE_44								=	44,
		ZRESERVE_45								=	45,
		ZRESERVE_46								=	46,
		ZRESERVE_47								=	47,
		ZRESERVE_48								=	48,
		ZRESERVE_49								=	49,
		ZRESERVE_50								=	50,
		ZRESERVE_51								=	51,
		ZRESERVE_52								=	52,
		ZRESERVE_53								=	53,
		ZRESERVE_54								=	54,
		ZRESERVE_55								=	55,
		ZRESERVE_56								=	56,
		ZRESERVE_57								=	57,
		ZRESERVE_58								=	58,
		ZRESERVE_59								=	59,
		ZRESERVE_60								=	60,
		ZRESERVE_61								=	61,
		ZRESERVE_62								=	62,
		ZRESERVE_63								=	63,
		ZRESERVE_64								=	64
	}

	/// <summary>
	/// 
	/// </summary>
	public class CommonIconCollection
	{
		//protected string[]			mIconName;
		protected ImageList			mImL;

		public ImageList				pImageList
		{
			get { return mImL; }
		}

		public int							pIconSize
		{
			get { return mImL.ImageSize.Height; }
			set { mImL.ImageSize = new Size(value,value); }
		}

		public CommonIconCollection():this(48)
		{
		}
		public CommonIconCollection(int aIconSize)
		{
			//mIconName = new String[32];
			mImL = new ImageList();
			
			pIconSize = aIconSize < 16 ? 32 : aIconSize;
			//System.ty
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Exit_Power48.ico"));
			//mIconName[(int)eCCIconName.Exit_Power48] = "Exit_Power48";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Save_Diskette48.ico"));
			//mIconName[(int)eCCIconName.Save_Diskette48] = "Save_Diskette48";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.About32.ico"));
			//mIconName[(int)eCCIconName.About32] = "About32";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.ExpImp32.ico"));
			//mIconName[(int)eCCIconName.ExportImport32] = "ExpImp32";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Exit32.ico"));
			//mIconName[(int)eCCIconName.Exit32] = "Exit32";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Delete_TrashEmpty32.ico"));
			//mIconName[(int)eCCIconName.Delete_TrashEmpty32] = "Delete_TrashEmpty32";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Delete_TrashEmpty48.ico"));
			//mIconName[(int)eCCIconName.Delete_TrashEmpty48] = "Delete_TrashEmpty48";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.New_Text48.ico"));
			//mIconName[(int)eCCIconName.New_Text48] = "New_Text48";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Edit_Text48.ico"));
			//mIconName[(int)eCCIconName.Edit_Text48] = "Edit_Text48";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Param48.ico"));
			//mIconName[(int)eCCIconName.Param48] = "Param48";
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.About48.ico"));
      //mIconName[(int)eCCIconName.About48] = "About48";
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Filter32.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.MMDoc.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Folder48.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.FolderOpen48.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Foto.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Doc.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.DocNew.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.DocOpen.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.FolderNew.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Refresh.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.FolderDisable.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.FolderDisable_Open.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Doc_Disable.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Doc_Disable_Open.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Anketa48.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.FilterOn32.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.FilterOff32.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.ArrowUp.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.ArrowDown.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Print.ico"));
      mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Preview.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Run.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Shell32HDLocal.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Shell32FolderClose.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Shell32FolderOpen.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Find32.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Renumering.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.NavBackward.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.NavForward.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Views.ico"));
			// !!! ДАЛЕЕ ВСТАВЛЕНА ПУСТАЯ ИКОНА
			// ЗАРЕЗЕРВИРОВАННОЕ МЕСТНО В КОЛЛЕКЦИИ
			// ПРИ ДОБАВЛЕНИИ ЗАМЕНЯТЬ !!!
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
			mImL.Images.Add(new Icon(typeof(CommonIconCollection),"Images.Icons.Blank.ico"));
		}

		//public int	GetIconIndex(string aIconName)
		//{
		//	for (int ii=0; ii < mIconName.Length; ii++)
		//		if (mIconName[ii].Equals(aIconName))
		//			return ii;
		//
		//	return -1;
		//}

		//public int	GetIconIndexFromTBBTagMenu(string aMenuName)
		//{
		//	return GetIconIndex(aMenuName);
		//}
    public virtual int GetIndexfromType(int TypeR)
    {
      return 0;
    }
    public virtual int GetIndexfromTypeDisable(int TypeR)
    {
      return 0;
    }
	}

	public enum iconAlign
	{
		Default=0,
		leftTop,
		leftButtom,
		RightTop,
		RightButtom,
		Center=Default
	}
	
	public class ImageTools
	{
		private ImageTools(){} //нельзя создавать !
		
		#region Создаем маленькую картинку указанных размеров
		static public Image CreateThumbnail(int width,int height,Image img)
		{
			return CreateThumbnail(new Size(width,height),img);
		}
		static public Image CreateThumbnail(Size size,Image img)
		{
			Image.GetThumbnailImageAbort myImageCallback =new Image.GetThumbnailImageAbort(imageCallback);
			Image mPhoto=img.GetThumbnailImage(size.Width,size.Height,myImageCallback ,IntPtr.Zero);
			return mPhoto;
		}
		private static bool imageCallback()
		{
			return false;
		}

		#endregion --Создаем маленькую картинку указанных размеров

		#region Рисуем картинку одну поверх другой !

		static public Image Summary2Image(Image big, Image small)
		{
			return Summary2Image(big,small,iconAlign.Default);
		}

		static public Image Summary2Image(Image big, Image small, iconAlign ImageAligment)
		{
			Image ret=(Image)big.Clone();
			Graphics g=Graphics.FromImage(ret);
			//делаем копию в четверть !!
			int srcX=0,srcY=0;
			Image small2=null;
			//если они равны - создаем X/2 и Y/2
			if (big.Size==small.Size)
				small2=CreateThumbnail(big.Width/2,big.Height/2,small);
			else //иначе - эта картинка уже маленькая !! (например ShortCut)
				small2=small;

			Size sz=small2.Size;
			Rectangle rect=new Rectangle(new Point(0,0),sz);
			switch(ImageAligment)
			{
				case iconAlign.Center:	
					srcX=small2.Width/2; 
					srcY=srcX;
					break;
				case iconAlign.leftTop:	
					srcX=0; 
					srcY=0;
					break;
				case iconAlign.leftButtom:	
					srcX=0; 
					srcY=small2.Width/2;
					break;
				case iconAlign.RightTop:	
					srcX=0; 
					srcY=small2.Width;
					break;
				case iconAlign.RightButtom:
					srcX=small2.Width; 
					srcY=small2.Width;
					break;
			}
			rect.Location=new Point(srcX,srcX);
			g.DrawImage(small2,rect);
			g.Dispose();
			return ret;
		}

		#endregion --Рисуем картинку одну поверх другой !

		static public Bitmap CreateBitmapButton(int aWH, Color clr,Font fnt,string txt,Color fone)
		{
			Bitmap   bm = new Bitmap(aWH, aWH);
			Graphics grfx   = Graphics.FromImage(bm);
			SizeF    sizef  = grfx.MeasureString(txt.Substring(0,1), fnt);
			float    fScale = Math.Min(bm.Width / sizef.Width, bm.Height / sizef.Height);
			//Font font = new Font(fnt.Name, fScale * fnt.SizeInPoints+3, fnt.Style);
			StringFormat strfmt = new StringFormat();
			strfmt.Alignment = strfmt.LineAlignment = StringAlignment.Center;

			grfx.Clear(fone);
			//grfx.DrawString(txt, font, new SolidBrush(clr), bm.Width / 2, bm.Height / 2, strfmt);
			grfx.DrawString(txt, fnt, new SolidBrush(clr), bm.Width / 2, bm.Height / 2, strfmt);
			grfx.Dispose();

			return bm;
		}
		#region ScaleImageIsotropically...

    /// <summary>
		/// Из книги "Программирование для Microsoft Windows на C#" Ч.Петцольд
		/// Глава 11. Изображения и битовые карты. стр.444
		/// </summary>
		/// <param name="grfx"></param>
		/// <param name="image"></param>
		/// <param name="rect"></param>
		static public void ScaleImageIsotropically(Graphics grfx, Image image, Rectangle rect,bool clear)
		{
			if (image == null)
				return;

			SizeF sizef = new SizeF(image.Width / image.HorizontalResolution,
				image.Height / image.VerticalResolution);

			float fScale = Math.Min(rect.Width  / sizef.Width,
				rect.Height / sizef.Height);

			sizef.Width  *= fScale;
			sizef.Height *= fScale;
      //сначала все чистим!
      if (clear)
        grfx.FillRectangle(new SolidBrush(SystemColors.Control),rect);
      //затем выводим
			grfx.DrawImage(image, rect.X + (rect.Width  - sizef.Width ) / 2,
				rect.Y + (rect.Height - sizef.Height) / 2,
				sizef.Width, sizef.Height);
		}
		/// <summary>
		/// Перегрузка вывода картинки с предварительной очисткой территорией
		/// </summary>
		/// <param name="grfx"></param>
		/// <param name="image"></param>
		/// <param name="rect"></param>
		static public void ScaleImageIsotropically(Graphics grfx, Image image, Rectangle rect)
		{
			ScaleImageIsotropically(grfx,image,rect,false);
		}

		#endregion --ScaleImageIsotropically

		#region Выводим сразу все картинки в Rectangle и возвращаем области рисования !!!

		static public Rectangle[] ScaleImageIsotropicallyAll(Graphics g, Image[] images, Rectangle rect)
		{
			return ScaleImageIsotropicallyAll(g,images,rect,0,Point.Empty,true);
		}

		static public Rectangle[] ScaleImageIsotropicallyAll(Graphics g, Image[] images, Rectangle rect, bool clear)
		{
			return ScaleImageIsotropicallyAll(g,images,rect,0,Point.Empty,clear);
		}

		static public Rectangle[] ScaleImageIsotropicallyAll(Graphics g, Image[] images, Rectangle rect, int XYSize, Point AutoScrollPosition, bool clear)
		{
			if (images == null || images.Length==0)
				return new Rectangle[]{};

			if (XYSize==0)
				XYSize=100; //размер по умолчанию
			Rectangle[] ret=new Rectangle[images.Length];

			int CountInLine=0; //временно !!!
			if (CountInLine==0)
				CountInLine=8; //сколько в линию рисуем картинок
			int i=0, delta=4;
			int lines=(images.Length / CountInLine)+1;
			Rectangle maxR=new Rectangle(0,0,CountInLine*(XYSize+delta),lines*(XYSize+delta));

			if (AutoScrollPosition.IsEmpty)
				AutoScrollPosition=new Point(maxR.Size);

			//rect=maxR;
			//сначала все чистим!
			if (clear)
				g.FillRectangle(new SolidBrush(SystemColors.Control),rect);

			g.TextContrast=10; //=new Region(maxR);

			StringFormat frm=new StringFormat();
			frm.LineAlignment=StringAlignment.Far;
			frm.Alignment=StringAlignment.Center;
			Font fnt=new Font("MS Sans Serif",8);

			int limit=fnt.Height; //узнаем высоту
			Size sizeImage=new Size(XYSize,XYSize-limit); //размеры картинки

			Brush brush=Brushes.Blue;
			Brush brushLight=Brushes.Red;
			Brush brushLight2=Brushes.Yellow;
			Point point=rect.Location;
			foreach(Image image in images)
			{
				int line =(i/CountInLine); //текущая линия
				int numer =(i % CountInLine) ;//номер в строке текущего изображения
				string text=(i+1)+".";
				//Point beginPoint=point;
				//beginPoint.Offset(numer*(XYSize+delta),line*(XYSize+delta));
				Point beginPoint=new Point(numer*(XYSize+delta), line*(XYSize+delta)); //+ (Size) AutoScrollPosition;

				Rectangle R= new Rectangle(beginPoint,sizeImage);
				Image small=CreateThumbnail(XYSize,XYSize,image);
				ScaleImageIsotropically(g,small,R,false);
				R.Inflate(0,limit/2);
				Rectangle Rt=R;
				Rt.Inflate(1,1);
				g.DrawString(text,fnt,brushLight2,Rt,frm);
				Rt.Inflate(-1,-1);
				g.DrawString(text,fnt,brushLight,Rt,frm);
				g.DrawString(text,fnt,brush,R,frm);
				ret[i]=R; //запоминаем и возвращаем !!!

				i++;
			}
			return ret;
		}

		#endregion --Выводим сразу все картинки в Rectangle
  }
}
