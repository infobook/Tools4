using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CommandAS.Tools.Barcode
{
  public class BarCodeBase
  {

    public BarCodeBase()
    {
    }

    public virtual Image DrawBitmap(string csMessage)
    {
      return new Bitmap(20, 20);
    }

    public virtual Image DrawBitmap(string csMessage, int widthImage)
    {
      return new Bitmap(20, widthImage);
    }
  }
}
