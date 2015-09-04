using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CommandAS.Tools.Barcode
{
  public class BarCodeI2of5 : BarCodeBase
  {
    int m_nStartingXPixel;

    const string m_BarStart = "nnnn";
    const string m_BarEnd = "wnn";
    const int m_Delta = 0; //отступы со всех сторон

    const int m_nStartingYPixel = 320;

    const float m_dHeight = 0.5F;
    const float m_dNarrowBar = 0.012F;
    const float m_dRatio = 3.0F;
    const float m_dFinalHeight = 1.0F;

    public BarCodeI2of5()
      : base()
    {
    }

    /// <summary>
    /// Строим BarCode 2 from 5
    /// </summary>
    /// <param name="csMessage"></param>
    /// <returns></returns>
    public override Image DrawBitmap(string csMessage)
    {
      return DrawBitmap(csMessage, 0);
    }

    public override Image DrawBitmap(string csMessage, int widthImage)
    {
      int m_nNarrowBarPixelWidth, m_nWideBarPixelWidth;

      Image img = new Bitmap(10, 10);
      Graphics g = Graphics.FromImage(img);

      m_nStartingXPixel = 2;

      float nXAxisDpi = g.DpiX;
      float nYAxisDpi = g.DpiY;
      // load the final attributes that depend on the device context
      int m_nPixelHeight = (int)(nYAxisDpi * m_dFinalHeight + 0.5);
      //ширина тонкой полоски ~ 1
      m_nNarrowBarPixelWidth = (int)(nXAxisDpi * m_dNarrowBar + 0.5);
      //шитрина толстого символа ~ 3
      m_nWideBarPixelWidth = (int)(m_dRatio * m_nNarrowBarPixelWidth);
      g.Dispose();
      img.Dispose();
      return DrawBitmap(csMessage, widthImage, m_nPixelHeight, m_nNarrowBarPixelWidth, m_nWideBarPixelWidth);
    }

    public Image DrawBitmap(string csMessage, int widthImage, int heightImage, int NarrowBarWidth, int WideBarWidth)
    {
      if (csMessage == null || csMessage.Trim().Length == 0)
        return null;
      //делаем четным число цифр! А ЗАЧЕМ !!!
      if ((csMessage.Length % 2) != 0)
        csMessage = "0" + csMessage;

      m_nStartingXPixel = 2;

      int width = 0; //ширина будущей картинки
      int height = 0;

      StringBuilder fullBarCode = new StringBuilder(m_BarStart);
      for (int i = 0; i < csMessage.Length; i += 2)
      {
        int nNumber = csMessage[i] - '0';
        nNumber = nNumber * 10;
        nNumber += csMessage[i + 1] - '0';
        fullBarCode.Append(RetrievePattern(nNumber)); //0-99
      }
      fullBarCode.Append(m_BarEnd);     //полный BarCode
      int m_nFinalBarcodePixelWidth = GetWidth(fullBarCode.ToString(), NarrowBarWidth, WideBarWidth);
      width = m_nFinalBarcodePixelWidth;  //истинная ширина BarCode
      if (widthImage > width)
        width = widthImage;
      height = heightImage;

      //создаем заново с нужными размерами
      Image img = new Bitmap(width, height);
      Graphics g = Graphics.FromImage(img);
      g.CompositingQuality = CompositingQuality.HighQuality;
      //g.InterpolationMode = InterpolationMode.HighQualityBilinear;
      g.TextContrast = 10;
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
      //рисуем фон
      g.Clear(Color.Transparent);
      DrawPattern(fullBarCode.ToString(), ref g, img.Size, NarrowBarWidth, WideBarWidth); // draw the all characters
      //img.Save("c:/barcode.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
      g.Flush(); //сбрасываем все измениния
      g.Dispose();
      return img;
    }
    private int GetWidth(string barSymbol, int NarrowBarWidth, int WideBarWidth)
    {
      int ret = 0;
      for (int i = 0; i < barSymbol.Length; i++)
      {
        if (barSymbol[i] == 'n')
          ret += NarrowBarWidth;
        else
          ret += WideBarWidth;
      }
      return ret;
    }
    /// <summary>
    /// draws the passed character pattern at the end of the barcode
    /// </summary>
    /// <param name="csCharPattern">the bar pattern to draw</param>
    /// <param name="img"></param>
    private int DrawPattern(string csCharPattern, ref Graphics img, Size s, int NarrowBarWidth, int WideBarWidth)
    {
      int nTempWidth;
      for (int i = 0; i < csCharPattern.Length; i++)
      {
        //int nTempWidth =GetWidth(c.ToString());
        // decide if narrow or wide bar
        if (csCharPattern[i] == 'n')
          nTempWidth = NarrowBarWidth;
        else
          nTempWidth = WideBarWidth;
        // X value for loop
        for (int nXPixel = m_nStartingXPixel; nXPixel < m_nStartingXPixel + nTempWidth; nXPixel++)
        {
          // if this is a bar
          Color bar = Color.Empty;
          if (i % 2 == 0)
            bar = Color.Black;
          else
            bar = Color.White;
          //nXPixel,nYPixel
          img.DrawLine(new Pen(new SolidBrush(bar)), nXPixel, m_Delta, nXPixel, s.Height - m_Delta * 2);
        }
        // advance the starting position
        m_nStartingXPixel += nTempWidth;
      }
      return m_nStartingXPixel;
    }

    /// <summary>
    /// retrieves the bar pattern for a given character
    /// </summary>
    /// <param name="nTwoDigitNumber">the input character to get the bar pattern for</param>
    /// <returns>the bar pattern for the input character</returns>
    private string RetrievePattern(int nTwoDigitNumber)
    {
      string ret = string.Empty;
      switch (nTwoDigitNumber)
      {
        case 0: ret = "nnnnwwwwnn"; break;
        case 1: ret = "nwnnwnwnnw"; break;
        case 2: ret = "nnnwwnwnnw"; break;
        case 3: ret = "nwnwwnwnnn"; break;
        case 4: ret = "nnnnwwwnnw"; break;
        case 5: ret = "nwnnwwwnnn"; break;
        case 6: ret = "nnnwwwwnnn"; break;
        case 7: ret = "nnnnwnwwnw"; break;
        case 8: ret = "nwnnwnwwnn"; break;
        case 9: ret = "nnnwwnwwnn"; break;
        case 10: ret = "wnnnnwnwwn"; break;
        case 11: ret = "wwnnnnnnww"; break;
        case 12: ret = "wnnwnnnnww"; break;
        case 13: ret = "wwnwnnnnwn"; break;
        case 14: ret = "wnnnnwnnww"; break;
        case 15: ret = "wwnnnwnnwn"; break;
        case 16: ret = "wnnwnwnnwn"; break;
        case 17: ret = "wnnnnnnwww"; break;
        case 18: ret = "wwnnnnnwwn"; break;
        case 19: ret = "wnnwnnnwwn"; break;
        case 20: ret = "nnwnnwnwwn"; break;
        case 21: ret = "nwwnnnnnww"; break;
        case 22: ret = "nnwwnnnnww"; break;
        case 23: ret = "nwwwnnnnwn"; break;
        case 24: ret = "nnwnnwnnww"; break;
        case 25: ret = "nwwnnwnnwn"; break;
        case 26: ret = "nnwwnwnnwn"; break;
        case 27: ret = "nnwnnnnwww"; break;
        case 28: ret = "nwwnnnnwwn"; break;
        case 29: ret = "nnwwnnnwwn"; break;
        case 30: ret = "wnwnnwnwnn"; break;
        case 31: ret = "wwwnnnnnnw"; break;
        case 32: ret = "wnwwnnnnnw"; break;
        case 33: ret = "wwwwnnnnnn"; break;
        case 34: ret = "wnwnnwnnnw"; break;
        case 35: ret = "wwwnnwnnnn"; break;
        case 36: ret = "wnwwnwnnnn"; break;
        case 37: ret = "wnwnnnnwnw"; break;
        case 38: ret = "wwwnnnnwnn"; break;
        case 39: ret = "wnwwnnnwnn"; break;
        case 40: ret = "nnnnwwnwwn"; break;
        case 41: ret = "nwnnwnnnww"; break;
        case 42: ret = "nnnwwnnnww"; break;
        case 43: ret = "nwnwwnnnwn"; break;
        case 44: ret = "nnnnwwnnww"; break;
        case 45: ret = "nwnnwwnnwn"; break;
        case 46: ret = "nnnwwwnnwn"; break;
        case 47: ret = "nnnnwnnwww"; break;
        case 48: ret = "nwnnwnnwwn"; break;
        case 49: ret = "nnnwwnnwwn"; break;
        case 50: ret = "wnnnwwnwnn"; break;
        case 51: ret = "wwnnwnnnnw"; break;
        case 52: ret = "wnnwwnnnnw"; break;
        case 53: ret = "wwnwwnnnnn"; break;
        case 54: ret = "wnnnwwnnnw"; break;
        case 55: ret = "wwnnwwnnnn"; break;
        case 56: ret = "wnnwwwnnnn"; break;
        case 57: ret = "wnnnwnnwnw"; break;
        case 58: ret = "wwnnwnnwnn"; break;
        case 59: ret = "wnnwwnnwnn"; break;
        case 60: ret = "nnwnwwnwnn"; break;
        case 61: ret = "nwwnwnnnnw"; break;
        case 62: ret = "nnwwwnnnnw"; break;
        case 63: ret = "nwwwwnnnnn"; break;
        case 64: ret = "nnwnwwnnnw"; break;
        case 65: ret = "nwwnwwnnnn"; break;
        case 66: ret = "nnwwwwnnnn"; break;
        case 67: ret = "nnwnwnnwnw"; break;
        case 68: ret = "nwwnwnnwnn"; break;
        case 69: ret = "nnwwwnnwnn"; break;
        case 70: ret = "nnnnnwwwwn"; break;
        case 71: ret = "nwnnnnwnww"; break;
        case 72: ret = "nnnwnnwnww"; break;
        case 73: ret = "nwnwnnwnwn"; break;
        case 74: ret = "nnnnnwwnww"; break;
        case 75: ret = "nwnnnwwnwn"; break;
        case 76: ret = "nnnwnwwnwn"; break;
        case 77: ret = "nnnnnnwwww"; break;
        case 78: ret = "nwnnnnwwwn"; break;
        case 79: ret = "nnnwnnwwwn"; break;
        case 80: ret = "wnnnnwwwnn"; break;
        case 81: ret = "wwnnnnwnnw"; break;
        case 82: ret = "wnnwnnwnnw"; break;
        case 83: ret = "wwnwnnwnnn"; break;
        case 84: ret = "wnnnnwwnnw"; break;
        case 85: ret = "wwnnnwwnnn"; break;
        case 86: ret = "wnnwnwwnnn"; break;
        case 87: ret = "wnnnnnwwnw"; break;
        case 88: ret = "wwnnnnwwnn"; break;
        case 89: ret = "wnnwnnwwnn"; break;
        case 90: ret = "nnwnnwwwnn"; break;
        case 91: ret = "nwwnnnwnnw"; break;
        case 92: ret = "nnwwnnwnnw"; break;
        case 93: ret = "nwwwnnwnnn"; break;
        case 94: ret = "nnwnnwwnnw"; break;
        case 95: ret = "nwwnnwwnnn"; break;
        case 96: ret = "nnwwnwwnnn"; break;
        case 97: ret = "nnwnnnwwnw"; break;
        case 98: ret = "nwwnnnwwnn"; break;
        case 99: ret = "nnwwnnwwnn"; break;
      }
      return ret;
    }
  }
}
