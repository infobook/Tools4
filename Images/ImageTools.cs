using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CommandAS.Tools
{
	public enum iconAlign
	{
		Default=0,
		leftTop,
		leftButtom,
		RightTop,
		RightButtom,
		Center=Default
	}
	/// <summary>
	/// ImageTools - основная библиотека преобразований картинок.
	/// </summary>
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

		//static public Image Summary2Image(Image big, Image small, iconAlign ImageAligment)
		//{
		//	return Summary2Image(big, small, ImageAligment, 2/3, 2/3);
		//}

		//static public Image Summary2Image(Image big, Image small, iconAlign ImageAligment, float aW, float aH)
		static public Image Summary2Image(Image big, Image small, iconAlign ImageAligment)
		{
			Image ret=big.Clone() as Image;
			Graphics g=Graphics.FromImage(ret);
			/* делаем абалденное качество ! */

			g.CompositingQuality=CompositingQuality.HighQuality;
			g.InterpolationMode=InterpolationMode.HighQualityBilinear;
			g.SmoothingMode=SmoothingMode.AntiAlias;

			//делаем копию в четверть !!
			int srcX=0,srcY=0;
			Image small2=null;
			//если они равны - создаем X/2 и Y/2
			if (big.Size==small.Size)
				//small2=CreateThumbnail((int)(big.Width*aW),(int)(big.Height*aH),small);
				small2=CreateThumbnail(big.Width*3/4,big.Height*3/4,small);
			else //иначе - эта картинка уже маленькая !! (например ShortCut)
				small2=small;

			Size sz=small2.Size;
			Rectangle rect=new Rectangle(new Point(0,0),sz);
			switch(ImageAligment)
			{
				case iconAlign.Center:	
					srcX=ret.Width/2 - small2.Width/2;
					srcY=srcX;
					break;
				case iconAlign.leftTop:	
					srcX=0; 
					srcY=0;
					break;
				case iconAlign.leftButtom:	
					srcX=0; 
					srcY=ret.Height - small2.Height;
					break;
				case iconAlign.RightTop:	
					srcX=ret.Width- small2.Width;
					srcY=0;
					break;
				case iconAlign.RightButtom:
					srcX=ret.Width - small2.Width; 
					srcY=srcX;
					break;
			}
			rect.Location=new Point(srcX,srcY);
			g.DrawImage(small2,rect);
			g.Dispose();
			return ret;
		}

		#endregion --Рисуем картинку одну поверх другой !
		
		#region Рисуем "недоступную" картинку
		
		/// <summary>
		/// Возвращаем "недоступное" изображение
		/// </summary>
		/// <param name="img"></param>
		/// <returns></returns>
		static public Image DisableImage(Image img)
		{
			//double Y = Red * 0.299 + Green * 0.587 + Blue * 0.114
			Image ret=img.Clone() as Image;
			//Image ret=new Bitmap(img.Width,img.Height);
			Graphics g = Graphics.FromImage(ret);
			/* делаем абалденное качество ! */
			g.CompositingQuality=CompositingQuality.HighQuality;
			g.InterpolationMode=InterpolationMode.HighQualityBilinear;
			g.SmoothingMode=SmoothingMode.AntiAlias;
        
			ColorMatrix cm = new ColorMatrix(new float[][]{
																											new float[]{0.5f,0.5f,0.5f,0,0},
																											new float[]{0.5f,0.5f,0.5f,0,0},
																											new float[]{0.5f,0.5f,0.5f,0,0},
																											new float[]{0,0,0,1,0,0},
																											new float[]{0,0,0,0,1,0},
																											new float[]{0,0,0,0,0,1}});
        
			ImageAttributes ia = new ImageAttributes();
			ia.SetColorMatrix(cm);
			g.DrawImage(img,new Rectangle(0,0,img.Width,img.Height),0,0,img.Width,img.Height,GraphicsUnit.Pixel,ia);
			g.Dispose();

			return ret;
		}

		#endregion --Рисуем "недоступную" картинку

		#region Создаем картинку для кнопки

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

		#endregion --Создаем картинку для кнопки

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

    /// <summary>
    /// Ресайз(уменьшение) загружаемой картинки по требованию
    /// http://www.developeru.info/%D0%A0%D0%B5%D1%81%D0%B0%D0%B9%D0%B7%D1%83%D0%BC%D0%B5%D0%BD%D1%8C%D1%88%D0%B5%D0%BD%D0%B8%D0%B5%D0%97%D0%B0%D0%B3%D1%80%D1%83%D0%B6%D0%B0%D0%B5%D0%BC%D0%BE%D0%B9%D0%9A%D0%B0%D1%80%D1%82%D0%B8%D0%BD%D0%BA%D0%B8%D0%9F%D0%BE%D0%A2%D1%80%D0%B5%D0%B1%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D1%8E.aspx
    /// </summary>
    /// <param name="aImage"></param>
    /// <param name="aWidth"></param>
    /// <param name="aHeight"></param>
    /// <returns></returns>
    public static Image DecreaseImage(Image aImage, int aWidth, int aHeight)
    {
      Bitmap imgOutput = new Bitmap(aWidth, aHeight);
      imgOutput.MakeTransparent(Color.Black);
      Graphics newGraphics = Graphics.FromImage(imgOutput);
      newGraphics.Clear(Color.FromArgb(0, 255, 255, 255));
      newGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
      newGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
      newGraphics.DrawImage(aImage, 0, 0, aWidth, aHeight);
      newGraphics.Flush();

      return imgOutput;
    }

    public static string ImageInformation(Image aImage)
    {
      if (aImage == null)
        return "<нет изображения>";

      long sz1 = aImage.Size.Width * aImage.Size.Height * Image.GetPixelFormatSize(aImage.PixelFormat) / 8;
      long sz = sz1 / 1024;
      string zs = " Кбайт";
      if (sz > 1024)
      {
        sz /= 1024;
        zs = " Mбайт";
      }
      return "размер (ШхВ): " + aImage.Size.Width + "х" + aImage.Size.Height + " [" + sz + zs + " или " + sz1.ToString("N") + " байт]";
    }

    public static Image DecreaseImageSize(Image aImage, int aMaxSize, out string aSS)
    {
      Image imgRet = aImage;

      aSS = ImageInformation(aImage);

      if (aMaxSize == 0)
        return imgRet;

      int cx, cy;
      if (aImage.Width > aImage.Height)
      {
        if (aMaxSize > aImage.Width)
          return imgRet;

        cx = aMaxSize;
        cy = cx * aImage.Height / aImage.Width;
      }
      else
      {
        if (aMaxSize > aImage.Height)
          return imgRet;

        cy = aMaxSize;
        cx = cy * aImage.Width / aImage.Height;
      }

      imgRet = DecreaseImage(aImage, cx, cy);

      aSS = "Коррекция изображения: прежний " + aSS + "; новый " + ImageInformation(imgRet);

      return imgRet;

    }

  }
}
