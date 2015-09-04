using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CommandAS.Tools.Barcode
{
  public class BarCode39 : BarCodeBase
  {
    public enum AlignType
    {
      Left, Center, Right
    }

    public enum BarCodeWeight
    {
      Small = 1, Medium, Large
    }

    private String alphabet39 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";

    private String[] coded39Char = 
		{
			/* 0 */ "000110100", 
			/* 1 */ "100100001", 
			/* 2 */ "001100001", 
			/* 3 */ "101100000",
			/* 4 */ "000110001", 
			/* 5 */ "100110000", 
			/* 6 */ "001110000", 
			/* 7 */ "000100101",
			/* 8 */ "100100100", 
			/* 9 */ "001100100", 
			/* A */ "100001001", 
			/* B */ "001001001",
			/* C */ "101001000", 
			/* D */ "000011001", 
			/* E */ "100011000", 
			/* F */ "001011000",
			/* G */ "000001101", 
			/* H */ "100001100", 
			/* I */ "001001100", 
			/* J */ "000011100",
			/* K */ "100000011", 
			/* L */ "001000011", 
			/* M */ "101000010", 
			/* N */ "000010011",
			/* O */ "100010010", 
			/* P */ "001010010", 
			/* Q */ "000000111", 
			/* R */ "100000110",
			/* S */ "001000110", 
			/* T */ "000010110", 
			/* U */ "110000001", 
			/* V */ "011000001",
			/* W */ "111000000", 
			/* X */ "010010001", 
			/* Y */ "110010000", 
			/* Z */ "011010000",
			/* - */ "010000101", 
			/* . */ "110000100", 
			/*' '*/ "011000100",
			/* $ */ "010101000",
			/* / */ "010100010", 
			/* + */ "010001010", 
			/* % */ "000101010", 
			/* * */ "010010100" 
		};


    //private String code = string.Empty;
    private AlignType align = AlignType.Left;
    private int leftMargin = 0;
    private int topMargin = 0;
    private int height = 20;
    private BarCodeWeight weight = BarCodeWeight.Small;

    private bool showHeader = false;
    private bool showFooter = false;
    private String headerText = string.Empty;
    private Font headerFont = new Font("Courier", 18);
    private Font footerFont = new Font("Courier", 8);

    //public String pBarCode
    //{
    //  get { return code; }
    //  set { code = value.ToUpper(); }
    //}

    public AlignType pVertAlign
    {
      get { return align; }
      set { align = value; }
    }

    public int pBarCodeHeight
    {
      get { return height; }
      set { height = value; }
    }

    public int pLeftMargin
    {
      get { return leftMargin; }
      set { leftMargin = value; }
    }

    public int pTopMargin
    {
      get { return topMargin; }
      set { topMargin = value; }
    }

    public bool pShowHeader
    {
      get { return showHeader; }
      set { showHeader = value; }
    }

    public bool pShowFooter
    {
      get { return showFooter; }
      set { showFooter = value; }
    }

    public String pHeaderText
    {
      get { return headerText; }
      set { headerText = value; }
    }

    public BarCodeWeight pWeight
    {
      get { return weight; }
      set { weight = value; }
    }

    public Font pHeaderFont
    {
      get { return headerFont; }
      set { headerFont = value; }
    }

    public Font pFooterFont
    {
      get { return footerFont; }
      set { footerFont = value; }
    }

    public BarCode39()
      : base()
    {
    }

    public override Image DrawBitmap(string csMessage, int widthImage)
    {
      Image img = new Bitmap(widthImage, height);
      Graphics g = Graphics.FromImage(img);
      g.CompositingQuality = CompositingQuality.HighQuality;
      //g.InterpolationMode = InterpolationMode.HighQualityBilinear;
      //g.TextContrast = 10;
      //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      //рисуем фон
      g.Clear(Color.Transparent);

      String intercharacterGap = "0";
      String str = '*' + csMessage.ToUpper() + '*';
      int strLength = str.Length;

      for (int i = 0; i < csMessage.Length; i++)
      {
        if (alphabet39.IndexOf(csMessage[i]) == -1 || csMessage[i] == '*')
        {
          g.DrawString("INVALID BAR CODE TEXT", new Font(FontFamily.GenericSerif, 8L), Brushes.Red, 10, 10);
          return img;
        }
      }

      String encodedString = "";

      for (int i = 0; i < strLength; i++)
      {
        if (i > 0)
          encodedString += intercharacterGap;

        encodedString += coded39Char[alphabet39.IndexOf(str[i])];
      }

      int encodedStringLength = encodedString.Length;
      int widthOfBarCodeString = 0;
      double wideToNarrowRatio = 3;


      if (align != AlignType.Left)
      {
        for (int i = 0; i < encodedStringLength; i++)
        {
          if (encodedString[i] == '1')
            widthOfBarCodeString += (int)(wideToNarrowRatio * (int)weight);
          else
            widthOfBarCodeString += (int)weight;
        }
      }

      int x = 0;
      int wid = 0;
      int yTop = 0;
      SizeF hSize = g.MeasureString(headerText, headerFont);
      SizeF fSize = g.MeasureString(csMessage, footerFont);

      int headerX = 0;
      int footerX = 0;

      if (align == AlignType.Left)
      {
        x = leftMargin;
        headerX = leftMargin;
        footerX = leftMargin;
      }
      else if (align == AlignType.Center)
      {
        x = (widthImage - widthOfBarCodeString) / 2;
        headerX = (widthImage - (int)hSize.Width) / 2;
        footerX = (widthImage - (int)fSize.Width) / 2;
      }
      else
      {
        x = widthImage - widthOfBarCodeString - leftMargin;
        headerX = widthImage - (int)hSize.Width - leftMargin;
        footerX = widthImage - (int)fSize.Width - leftMargin;
      }

      if (showHeader)
      {
        yTop = (int)hSize.Height + topMargin;
        g.DrawString(headerText, headerFont, Brushes.Black, headerX, topMargin);
      }
      else
      {
        yTop = topMargin;
      }

      for (int i = 0; i < encodedStringLength; i++)
      {
        if (encodedString[i] == '1')
          wid = (int)(wideToNarrowRatio * (int)weight);
        else
          wid = (int)weight;

        g.FillRectangle(i % 2 == 0 ? Brushes.Black : Brushes.White, x, yTop, wid, height);

        x += wid;
      }

      yTop += height;

      if (showFooter)
        g.DrawString(csMessage, footerFont, Brushes.Black, footerX, yTop);

      g.Flush(); //сбрасываем все измениния
      g.Dispose();

      return img;
    }

  }
}
