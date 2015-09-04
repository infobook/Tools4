using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms; 
using CommandAS.Tools.WinAPI;
using System.Runtime.InteropServices;
using System.IO;

namespace CommandAS.Tools.TwainLib
{

	[StructLayout(LayoutKind.Sequential, Pack=2)]
	internal class BITMAPINFOHEADER
	{
		public int      biSize;
		public int      biWidth;
		public int      biHeight;
		public short    biPlanes;
		public short    biBitCount;
		public int      biCompression;
		public int      biSizeImage;
		public int      biXPelsPerMeter;
		public int      biYPelsPerMeter;
		public int      biClrUsed;
		public int      biClrImportant;
	}

	public class TwainImage
	{
		#region Property

		BITMAPINFOHEADER	bmi;
		Rectangle			bmprect;
		IntPtr	dibhand;
		IntPtr	bmpptr;
		IntPtr	pixptr;

		#endregion

		public TwainImage( IntPtr dibhandp )
		{
			bmprect = new Rectangle( 0, 0, 0, 0 );
			dibhand = dibhandp;
			bmpptr = Kernel32_DLL.GlobalLock( dibhand );
			pixptr = GetPixelInfo( bmpptr );
		}

		~TwainImage()
		{
			if( dibhand != IntPtr.Zero )
			{
				Kernel32_DLL.GlobalFree( dibhand );
				dibhand = IntPtr.Zero;
			}

		}

//		protected override void OnPaint( System.Windows.Forms.PaintEventArgs e )
//		{
//			Rectangle	cltrect = ClientRectangle;
//			Rectangle	clprect = e.ClipRectangle;
//			Point		scrol = AutoScrollPosition;
//
//			Rectangle	realrect = clprect;
//			realrect.X -= scrol.X;
//			realrect.Y -= scrol.Y;
//
//			SolidBrush brbg = new SolidBrush( Color.Black );
//			if( realrect.Right > bmprect.Width )
//			{
//				Rectangle	bgri = clprect;
//				int ovri = bmprect.Width - realrect.X;
//				if( ovri > 0 )
//				{
//					bgri.X += ovri;
//					bgri.Width -= ovri;
//				}
//				e.Graphics.FillRectangle( brbg, bgri );
//			}
//
//			if( realrect.Bottom > bmprect.Height )
//			{
//				Rectangle	bgbo = clprect;
//				int ovbo = bmprect.Height - realrect.Y;
//				if( ovbo > 0 )
//				{
//					bgbo.Y += ovbo;
//					bgbo.Height -= ovbo;
//				}
//				e.Graphics.FillRectangle( brbg, bgbo );
//			}
//
//			realrect.Intersect( bmprect );
//			if( ! realrect.IsEmpty )
//			{
//				int bot = bmprect.Height - realrect.Bottom;
//				IntPtr hdc = e.Graphics.GetHdc();
//				SetDIBitsToDevice( hdc, clprect.X, clprect.Y, realrect.Width, realrect.Height,
//					realrect.X, bot, 0, bmprect.Height, pixptr, bmpptr, 0 );
//				e.Graphics.ReleaseHdc(hdc);
//			}
//		}


    public Image GetImage2()
    {
      Bitmap me = new Bitmap(bmprect.Width, bmprect.Height);
      Graphics g = Graphics.FromImage(me);

      IntPtr hdc = g.GetHdc();
      GDI32_DLL.SetDIBitsToDevice(hdc, 0, 0, bmprect.Width, bmprect.Height, 0, 0, 0, bmprect.Height, pixptr, bmpptr, 0);
      g.ReleaseHdc(hdc);

      return me;
    }

		public Image GetImage()
		{
			Image ret = null;

      try
      {
        Guid clsid = Guid.Empty;
        foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
        {
          if (codec.FilenameExtension.IndexOf("*.JPG") >= 0)
          {
            clsid = codec.Clsid;
            break;
          }
        }

        IntPtr img = IntPtr.Zero;
        int st = GDIPlus_DLL.GdipCreateBitmapFromGdiDib(bmpptr, pixptr, ref img);
        if ((st != 0) || (img == IntPtr.Zero))
          return ret;

        /*
        string tempFile = Path.GetTempFileName(); // "TEMPSCANIMAGE.JPG";
        st = GDIPlus_DLL.GdipSaveImageToFile (img, tempFile, ref clsid, IntPtr.Zero);
        if (st == 0)
        {
          //Image iff = Image.FromFile(tempFile);
          //ret = iff.Clone() as Image;
          ret = (Image)Image.FromFile(tempFile);
          GDIPlus_DLL.GdipDisposeImage(img);
          File.Delete(tempFile);
        }
        */

        MemoryStream ms = new MemoryStream(); 
        ManagedIStream mis = new ManagedIStream(ms);
        st = GDIPlus_DLL.GdipSaveImageToStream(img, mis, ref clsid, IntPtr.Zero);
        if (st == 0)
        {
          ret = new Bitmap(ms);
          GDIPlus_DLL.GdipDisposeImage(img);
        }
        GC.SuppressFinalize(mis);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Source + " : " + ex.Message + Environment.NewLine + " [" + ex.StackTrace + "]", "Error !!!");
      }
			return ret;
		}

		protected IntPtr GetPixelInfo( IntPtr bmpptr )
		{
			bmi = new BITMAPINFOHEADER();
			Marshal.PtrToStructure( bmpptr, bmi );

			bmprect.X = bmprect.Y = 0;
			bmprect.Width = bmi.biWidth;
			bmprect.Height = bmi.biHeight;

			if( bmi.biSizeImage == 0 )
				bmi.biSizeImage = ((((bmi.biWidth * bmi.biBitCount) + 31) & ~31) >> 3) * bmi.biHeight;

			int p = bmi.biClrUsed;
			if( (p == 0) && (bmi.biBitCount <= 8) )
				p = 1 << bmi.biBitCount;
			p = (p * 4) + bmi.biSize + (int) bmpptr;
			return (IntPtr) p;
		}
	} // class PicForm
}
