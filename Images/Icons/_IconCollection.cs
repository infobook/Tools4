using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace CommandAS.Tools
{
	/// <summary>
	/// Класс, назначение которого - поддержка перечислений
	/// </summary>
	public class IconCollectionKeys
	{
		#region Перечисления ключй для иконок (Аналог Enum)

		public const string Blank							= "Blank";
		public const string Exit							= "Exit";
		public const string Save							= "Save";
		public const string SaveHistory				= "SaveHistory";

		public const string SaveHistory_Yes		= "SaveHistory_Yes";
		public const string SaveHistory_No		= "SaveHistory_No";

		public const string History						= "History";

		public const string About							= "About";
		public const string New								= "New";
		public const string Delete						= "Delete";
		public const string Edit							= "Edit";
		public const string Filter						= "Filter";
		public const string Folder						= "Folder";
		public const string FolderOpen				= "FolderOpen";
		public const string Foto							= "Foto";
		public const string Refresh 					= "Refresh";
		public const string Notes							= "Notes";

		public const string Find							=	"Find";
		public const string Print							= "Print";
		public const string PrintPreview			= "PrintPreview";
		public const string Preview						= "Preview";
		public const string Scanner						= "Scanner";

		public const string Clock							= "Clock";

		public const string ArrowUp						= "ArrowUp";
		public const string ArrowDown					= "ArrowDown";
		public const string ArrowBackward			=	"ArrowBackward";
		public const string ArrowForward			=	"ArrowForward";

		public const string ViewsPoins				=	"ViewsPoins";
		public const string ViewsLine					=	"ViewsLine";
		public const string Views2						=	"Views2";

		public const string Lock							=	"Lock";
		public const string Unlock						=	"Unlock";

		public const string ExportImport			= "ExportImport";
		public const string Renumering				=	"Renumering";

		public const string ShellHDLocal			=	"ShellHDLocal";
		//как в магнитофоне
		public const string First							=	"First";
		public const string Prev							=	"Prev";
		public const string Next							=	"Next";
		public const string Last							=	"Last";

		public const string People						=	"People";
		public const string Execute						=	"Execute";
		public const string FilterOn					= "FilterOn";
		public const string FilterOff					= "FilterOff";

		public const string Additive					= "Additive";
		public const string Minus							= "Minus";
		public const string Exchange					= "Exchange";
		public const string FromFile					= "FromFile";
		public const string Copy							= "Copy";
		public const string Paste							= "Paste";

		public const string Property					= "Property";
		public const string Setting						= "Setting";

		public const string Zoom_Plus					=	"Zoom_Plus";
		public const string Zoom_Minus				=	"Zoom_Minus";

		public const string ImageRotate_Left	=	"ImageRotate_Left";
		public const string ImageRotate_Right	=	"ImageRotate_Right";
		public const string ImageFillReal			=	"ImageFillReal";

    public const string ExcelFiles = "ExcelFiles";
    public const string Reserve2 = "Reserve2";
    public const string Reserve3 = "Reserve3";
    public const string Reserve4 = "Reserve4";
    public const string Reserve5 = "Reserve5";
    public const string Reserve6 = "Reserve6";
    public const string Reserve7 = "Reserve7";
    public const string Reserve8 = "Reserve8";
    public const string Reserve9 = "Reserve9";

		#endregion --Аналог Enum

		#region резерв ставки

//		public const string MMDoc						= "MMDoc"; //???
//		public const string Anketa  				= "Anketa";

		#endregion --резерв ставки

		protected IconCollectionKeys(){}
	}
	
	/// <summary>
	/// Класс для работы с иконками приложений
	/// </summary>
	public class IconCollection : IconCollectionKeys, ICloneable
	{
		#region local vars

		protected Hashtable mColl=new Hashtable();
		protected ImageList	mIList;

		#endregion --local vars

		#region constructors

		public IconCollection()	:	this (0){}
		public IconCollection(int IconSize)
		{
      IconSize = IconSize != 0 ? IconSize : 32;
			mIList=new ImageList();
			mIList.ImageSize=new Size(IconSize,IconSize);
			loadIconBase();
		}

		#endregion --constructors

		#region protected function
		
		/// <summary>
		/// Эту функцию в наследуемых классах следует переписать
		/// или использовать полную версию loadImage
		/// </summary>
		/// <param name="resource"></param>
		/// <returns>Image</returns>
		protected virtual Image loadImage(string resource)
		{
			return loadImage(typeof(IconCollection),resource);
		}
		/// <summary>
		/// Вспомогательная ф-я для загрузки иконок из ресурсов приложений
		/// </summary>
		/// <param name="type"></param>
		/// <param name="resource"></param>
		/// <returns>Image</returns>
		protected Image loadImage(Type type, string resource)
		{
			Image ret=new Bitmap(type,resource);
			return ret;
		}


		#endregion --local function

		#region loadIconBase
		/// <summary>
		/// для загрузки
		/// </summary>
		private void loadIconBase()
		{
			#region olds
			/*
				"Images.Icons.Exit_Power48.ico";
				"Images.Icons.Save_Diskette48.ico";
				"Images.Icons.About32.ico";
				"Images.Icons.ExpImp32.ico";
				"Images.Icons.Exit.ico";
				"Images.Icons.Delete_TrashEmpty32.ico";
				"Images.Icons.Delete_TrashEmpty48.ico";
				"Images.Icons.New_Text48.ico";
				"Images.Icons.Edit_Text48.ico";
				"Images.Icons.Param48.ico";
				"Images.Icons.About48.ico'";
				"Images.Icons.Filter32.ico";
				"Images.Icons.MMDoc.ico";
				"Images.Icons.Folder48.ico";
				"Images.Icons.FolderOpen48.ico";
				"Images.Icons.Foto.ico";
				"Images.Icons.Doc.ico";
				"Images.Icons.DocNew.ico";
				"Images.Icons.DocOpen.ico";
				"Images.Icons.FolderNew.ico";
				"Images.Icons.Refresh.ico";
				"Images.Icons.FolderDisable.ico";
				"Images.Icons.FolderDisable_Open.ico";
				"Images.Icons.Doc_Disable.ico";
				"Images.Icons.Doc_Disable_Open.ico";
				"Images.Icons.Anketa48.ico";
				"Images.Icons.FilterOn32.ico";
				"Images.Icons.FilterOff32.ico";
				"Images.Icons.ArrowUp.ico";
				"Images.Icons.ArrowDown.ico";
				"Images.Icons.Print.ico";
				"Images.Icons.Preview.ico";
				"Images.Icons.Run.ico";
				"Images.Icons.Shell32HDLocal.ico";
				"Images.Icons.Shell32FolderClose.ico";
				"Images.Icons.Shell32FolderOpen.ico";
				"Images.Icons.Find32.ico";
				"Images.Icons.Renumering.ico";
				"Images.Icons.NavBackward.ico";
				"Images.Icons.NavForward.ico";
				"Images.Icons.Views.ico";
				*/
			#endregion --olds
			const string pref="Images.Icons.";

			Add(loadImage(pref+"Blank.ico"),				Blank);
			Add(loadImage(pref+"About_i.ico"),			About);
			Add(loadImage(pref+"Exit_Power48.ico"),	Exit);

			Add(loadImage(pref+"Left.ico"),					ArrowBackward);
			Add(loadImage(pref+"Right.ico"),				ArrowForward);
			Add(loadImage(pref+"Up.ico"),						ArrowUp);
			Add(loadImage(pref+"Down.ico"),					ArrowDown);

			Add(loadImage(pref+"Folder.ico"),				Folder);
			Add(loadImage(pref+"FolderOpen.ico"),		FolderOpen);

			Add(loadImage(pref+"Save.ico"),					Save);
			Add(loadImage(pref+"Save_History.ico"),	SaveHistory);
			Add(loadImage(pref+"Save_History_Yes.ico"),	SaveHistory_Yes);
			Add(loadImage(pref+"Save_History_No.ico"),	SaveHistory_No);

			Add(loadImage(pref+"History2.ico"),			History);

			Add(loadImage(pref+"Trash.ico"),				Delete);
			Add(loadImage(pref+"New.ico"),					New);
			Add(loadImage(pref+"Edit.ico"),					Edit);
				
			Add(loadImage(pref+"Find.ico"),					Find);
			Add(loadImage(pref+"Printer.ico"),			Print);
			Add(loadImage(pref+"Preview.ico"),			Preview);
			Add(loadImage(pref+"PrintPreview.ico"),	PrintPreview);
      Add(loadImage(pref+"Scanner.ico"),      Scanner);

			Add(loadImage(pref+"Clock.ico"),				Clock);

			Add(loadImage(pref+"Photo.ico"),				Foto);
			Add(loadImage(pref+"Refresh.ico"),			Refresh);
			Add(loadImage(pref+"Notes.ico"),				Notes);

			Add(loadImage(pref+"Views.ico"),				ViewsPoins);
			Add(loadImage(pref+"Views1.ico"),				ViewsLine);
			Add(loadImage(pref+"Views2.ico"),				Views2);

			Add(loadImage(pref+"Lock.ico"),					Lock);
			Add(loadImage(pref+"Unlock.ico"),				Unlock);

			Add(loadImage(pref+"ExportImport.ico"), ExportImport);

			Add(loadImage(pref+"Forward.ico"),			First);
			Add(loadImage(pref+"Revers.ico"),				Prev);
			Add(loadImage(pref+"Play.ico"),					Next);
			Add(loadImage(pref+"Backward.ico"),			Last);
			Add(loadImage(pref+"People2.ico"),			People);
			Add(loadImage(pref+"Execute3.ico"),			Execute);

			Add(loadImage(pref+"Filter.ico"),				Filter);
			Add(loadImage(pref+"FilterOn.ico"),			FilterOn);
			Add(loadImage(pref+"FilterOff.ico"),		FilterOff);

			Add(loadImage(pref+"Add.ico"),					Additive);
			Add(loadImage(pref+"Minus.ico"),				Minus);
			Add(loadImage(pref+"Exchange.ico"),			Exchange);
			Add(loadImage(pref+"FromFile.ico"),			FromFile);

			Add(loadImage(pref+"Copy.ico"),					Copy);
			Add(loadImage(pref+"Paste.ico"),				Paste);

			Add(loadImage(pref+"tools.ico"),				Property);
			Add(loadImage(pref+"Setting.ico"),			Setting);

			Add(loadImage(pref+"View_minus.ico"),		Zoom_Minus);
			Add(loadImage(pref+"View_plus.ico"),		Zoom_Plus);

			Add(loadImage(pref+"RotateLeft2.ico"),	ImageRotate_Left);
			Add(loadImage(pref+"RotateRight2.ico"),	ImageRotate_Right);
			Add(loadImage(pref+"FillReal.ico"),			ImageFillReal);

      /// РЕЗЕРВ
      /// чтобы не сбивались номера далее (Kernel), при последующем добавлении
      Add(loadImage(pref + "ExcelFiles.ico"), ExcelFiles);
      Add(loadImage(pref + "Blank.ico"), Reserve2);
      Add(loadImage(pref + "Blank.ico"), Reserve3);
      Add(loadImage(pref + "Blank.ico"), Reserve4);
      Add(loadImage(pref + "Blank.ico"), Reserve5);
      Add(loadImage(pref + "Blank.ico"), Reserve6);
      Add(loadImage(pref + "Blank.ico"), Reserve7);
      Add(loadImage(pref + "Blank.ico"), Reserve8);
      Add(loadImage(pref + "Blank.ico"), Reserve9);
			
		}

		#endregion --loadIconBase

		#region Операции с коллекцией иконок

		//Добавляем в коллекцию
		public bool Add(Image img, string key)
		{
			if (mColl.ContainsKey(key) && img!=null)
				return false;
			//добавляем в коллекцию иконок и узнаем индекс иконки
			int index=-1;
			if (img.Width==mIList.ImageSize.Width)
			{

				index=mIList.Images.AddStrip(img);
				mColl.Add(key, index);
			}
			else
			{
				Image img2=img.GetThumbnailImage(mIList.ImageSize.Height,mIList.ImageSize.Width,null,IntPtr.Zero);
				index=mIList.Images.Count;
				mIList.Images.Add(img2);
				mColl.Add(key, index);
			}
			return true;
		}

		/// <summary>
		/// Заменяем изображение по ключу
		/// </summary>
		/// <param name="img">Изображение</param>
		/// <param name="key">Ключ изображения</param>
		public bool Replace(Image img,string key)
		{
			bool ret=false;
			int index=Index(key);
			if (index!=-1)
			{
				mIList.Images[index]=img;
				ret=true;
			}
			return ret;
		}

		/// <summary>
		/// Функция для перезагрузки иконок с новым размером !!!
		/// Надо подумать о реализации ??!!??
		/// </summary>
		public void Reload(int newSizeIcons)
		{
			if (newSizeIcons==mIList.ImageSize.Width)
				return ;
			ImageList newIL=new ImageList();
			newIL.ImageSize=new Size(newSizeIcons,newSizeIcons);
			foreach(Image img in mIList.Images)
			{
				Image nImg=img.GetThumbnailImage(newSizeIcons,newSizeIcons,null,IntPtr.Zero);
				newIL.Images.Add(nImg);
			}

			mIList=newIL;
		}


		/// <summary>
		/// Генерация иконки "на лету" и добавление в коллекцию
		/// если уже есть в коллекции - просто возвращаем индекс Image
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int FolderIndex(string key)
		{
			int ret=Index(Folder);
			//нет картинки для рисования изображения
			if (!mColl.ContainsKey(key) || ret==-1)
			{
				//возвращаем пустую иконку Folder
				return ret;
			}
			Image imgFolder=mIList.Images[ret];
			string newKey=key+"_Folder";

			if (!mColl.ContainsKey(newKey))
			{
				Image imgKey=Image(key);
				Image img=ImageTools.Summary2Image(imgFolder,imgKey,iconAlign.RightTop);
				Add(img,newKey);
			}

			ret=Index(newKey);
			return ret;
		}
 
		/// <summary>
		/// Генерация иконки "на лету" и добавление в коллекцию
		/// если уже есть в коллекции - просто возвращаем индекс Image
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int DisableIndex(string key)
		{
			int ret=Index(key);
			//нет картинки для рисования изображения
			if (!mColl.ContainsKey(key) || ret==-1)
			{
				//возвращаем пустую иконку Folder
				return ret;
			}
			string newKey=key+"_Disable";

			if (!mColl.ContainsKey(newKey))
			{
				Image imgDraw=mIList.Images[ret];
				Image img=ImageTools.DisableImage(imgDraw);
				Add(img,newKey);
			}
			ret=Index(newKey);

			return ret;
		}

		#endregion --Операции с коллекцией иконок

		#region Получение изображения или индекса иконки

		/// <summary>
		/// Возвращаем изображение по ключу
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Image Image(string key)
		{
			Image ret=null;
			if (!mColl.ContainsKey(key))
				return ret;
			int index=(int)mColl[key];
			if (index!=-1)
				ret=mIList.Images[index];
			return ret;
		}

		/// <summary>
		/// Возвращаем иконку по ключу (не реализовано !!!)
		/// 28.10.2004 AndryC
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Icon Icon(string key)
		{
			Icon ret=null;
			if (!mColl.ContainsKey(key))
				return ret;
			int index=(int)mColl[key];
			if (index!=-1)
			{
				Bitmap img = mIList.Images[index] as Bitmap;
				ret = System.Drawing.Icon.FromHandle(img.GetHicon()); 
			}
			return ret;
		}
		/// <summary>
		/// По ключу возвращаем индекс в ImageList
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int Index(string key)
		{
			int ret=-1;
			if (!mColl.ContainsKey(key))
				return ret;
			ret=(int)mColl[key];
			return ret;
		}


		#endregion --Получение изображения или индекса иконки

		#region Стандартные функции

		/// <summary>
		/// Возвращаем размер текущей коллекции
		/// </summary>
		public int	pIconSize
		{
			get 
			{ 
				return mIList.ImageSize.Height; 
			}
		}

		/// <summary>
		/// Возвращаем всю коллекцию иконок
		/// </summary>
		public			ImageList pImageList
		{
			get
			{
				return mIList;
			}
		}

		#endregion --Стандартные функции

		#region ICloneable Members

		//пока не доделано (а надо ли ?)
		//28.10.2004 AndryC
		public object Clone()
		{
			IconCollection clone=this.MemberwiseClone() as IconCollection;
			clone.mColl=mColl.Clone() as Hashtable;
			clone.mIList=mIList; //loadIconBase()
			return clone;
		}

		#endregion
	}

	#region CommonIconCollection - пережитки прошлого
	/*
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


    public virtual int GetIndexfromType(int TypeR)
    {
      return 0;
    }
    public virtual int GetIndexfromTypeDisable(int TypeR)
    {
      return 0;
    }
	}
*/
	#endregion --CommonIconCollection - пережитки прошлого
}
