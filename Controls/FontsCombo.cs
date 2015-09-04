using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{	
  /// <summary>
  /// Класс для заполнения существующими шрифтами системы
  /// Комбобокса с написанием названия шрифтов
  /// </summary>
  public class ucComboFonts : ComboBox
  {
    private float fontsize=0;
    private const bool extShow=true;
    string samplestr = " - Привет, Hello";
    public ucComboFonts():this(0,20,extShow){}
    public ucComboFonts(int Height, int MaxDropDownItems):this(float.Parse(Height.ToString()),MaxDropDownItems,extShow){}
    public ucComboFonts(float Height, int MaxDropDownItems):this(Height,MaxDropDownItems,extShow){}
    /// <summary>
    /// Создаем комбик со шрифтами
    /// </summary>
    /// <param name="Height"></param>
    /// <param name="MaxDropDownItems"></param>
    /// <param name="extensFont"></param>
    public ucComboFonts(float Height, int MaxDropDownItems,bool extensFont)
    {				
      this.MaxDropDownItems = MaxDropDownItems;
      this.IntegralHeight = false;
      if (Height>0)
      {
        this.Height=(int)Height;
        fontsize=Height;
      }
      else
        fontsize=this.Font.Size;
      try
      {
        this.Font=new Font(this.Font.FontFamily,fontsize);
      }
      catch{}
      this.Sorted = false;
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawVariable;
      StoreFonts(extensFont);
      this.EnabledChanged+=new EventHandler(onEnabled);
    }
    private void onEnabled(object sender,EventArgs e)
    {
      if (Enabled)
        this.BackColor=SystemColors.Window;
      else
        this.BackColor=SystemColors.Control;
    }
    public void ChangeFontSize(float fs)
    {
      if (fs>6)
      {
        fontsize=fs;
      }
    }
    public void StoreFonts(bool fullDescriptionsFonts)
    {
      both = fullDescriptionsFonts;
      foreach (FontFamily ff in FontFamily.Families)
      {
        if(ff.IsStyleAvailable(FontStyle.Regular))
          Items.Add(ff.Name);												
      }			
			
      if(Items.Count > 0)
        SelectedIndex=0;
      //			ttimg = new Bitmap(GetType(),"ttfbmp.bmp");
    }

    protected override void OnMeasureItem(System.Windows.Forms.MeasureItemEventArgs e)
    {	
      if(e.Index > -1)
      {
        int w = 0;
        string fontstring = Items[e.Index].ToString();						
        Graphics g = CreateGraphics();
        e.ItemHeight = (int)g.MeasureString(fontstring, new Font(fontstring,fontsize)).Height;
        w = (int)g.MeasureString(fontstring, new Font(fontstring,fontsize)).Width;
        if(both)
        {
          float fs=fontsize;
          int h1 = (int)g.MeasureString(samplestr, new Font(fontstring,fs)).Height;
          int h2 = (int)g.MeasureString(Items[e.Index].ToString(), new Font("Arial",fs)).Height;
          int w1 = (int)g.MeasureString(samplestr, new Font(fontstring,fs)).Width;
          int w2 = (int)g.MeasureString(Items[e.Index].ToString(), new Font("Arial",fs)).Width;
          if(h1 > h2 )
            h2 = h1;
          e.ItemHeight = h2;
          w = w1 + w2;
        }
        //				w += ttimg.Width*2;
        if(w > maxwid)
          maxwid=w;
//        if(e.ItemHeight > 20)
//          e.ItemHeight = 20;
//        e.ItemHeight = (int)fontsize;
      }
      base.OnMeasureItem(e);
    }

    protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
    {	
      if(e.Index > -1)
      {
        string fontstring = Items[e.Index].ToString();
        nfont = new Font(fontstring,fontsize);
        Font afont = new Font("Arial",fontsize);

        if(both) //полное описание фонтов (с названием и написанием)
        {
          int w = (int)e.Graphics.MeasureString(fontstring, afont).Width;
          if((e.State & DrawItemState.Focus)==0)
          {
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), //SystemColors.Window
              //							e.Bounds.X+ttimg.Width,e.Bounds.Y,e.Bounds.Width,e.Bounds.Height);
              e.Bounds.X,e.Bounds.Y,e.Bounds.Width,e.Bounds.Height);

            e.Graphics.DrawString(fontstring,afont,new SolidBrush(SystemColors.WindowText),
              //							e.Bounds.X+ttimg.Width*2,e.Bounds.Y);	
              e.Bounds.X,e.Bounds.Y);	

            e.Graphics.DrawString(samplestr,nfont,new SolidBrush(SystemColors.WindowText),
              //							e.Bounds.X+w+ttimg.Width*2,e.Bounds.Y);	
              e.Bounds.X+w,e.Bounds.Y);	
          }
          else
          {
            e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), //this.BackColor
              e.Bounds.X,e.Bounds.Y,e.Bounds.Width,e.Bounds.Height);
            e.Graphics.DrawString(fontstring,afont,new SolidBrush(SystemColors.HighlightText),
              //              e.Bounds.X+ttimg.Width*2,e.Bounds.Y);
              e.Bounds.X,e.Bounds.Y);
            e.Graphics.DrawString(samplestr,nfont,new SolidBrush(SystemColors.HighlightText),
//              e.Bounds.X+w+ttimg.Width*2,e.Bounds.Y);
              e.Bounds.X+w,e.Bounds.Y);
          }	
        }
        else
        {

          if((e.State & DrawItemState.Focus)==0)
          {
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), //SystemColors.Window
              //							e.Bounds.X+ttimg.Width,e.Bounds.Y,e.Bounds.Width,e.Bounds.Height);
              e.Bounds.X,e.Bounds.Y,e.Bounds.Width,e.Bounds.Height);
            e.Graphics.DrawString(fontstring,nfont,new SolidBrush(SystemColors.WindowText),
              //							e.Bounds.X+ttimg.Width*2,e.Bounds.Y);
              e.Bounds.X,e.Bounds.Y);
				
          }
          else
          {
            e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight),
              //							e.Bounds.X+ttimg.Width,e.Bounds.Y,e.Bounds.Width,e.Bounds.Height);
              e.Bounds.X,e.Bounds.Y,e.Bounds.Width,e.Bounds.Height);
            e.Graphics.DrawString(fontstring,nfont,new SolidBrush(SystemColors.HighlightText),
              //							e.Bounds.X+ttimg.Width*2,e.Bounds.Y);
              e.Bounds.X,e.Bounds.Y);
          }			
        }

        //				e.Graphics.DrawImage(ttimg, new Point(e.Bounds.X, e.Bounds.Y)); 
      }
      base.OnDrawItem(e);
    }

    private void InitializeComponent()
    {
      this.panel1 = new System.Windows.Forms.Panel();
      // 
      // panel1
      // 
      this.panel1.Location = new System.Drawing.Point(17, 17);
      this.panel1.Name = "panel1";
      this.panel1.TabIndex = 0;

    }

    Font nfont;
    bool both = false;
    int maxwid = 0;
    private System.Windows.Forms.Panel panel1;
//    Image ttimg;

    protected override void OnDropDown(System.EventArgs e)
    {
      this.DropDownWidth = maxwid+30;
    }		
		
  }

}
