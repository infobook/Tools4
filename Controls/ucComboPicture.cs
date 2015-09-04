using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CommandAS.Tools;

namespace CommandAS.Tools.Controls
{
  /// <summary>
  /// Summary description for ucComboPicture.
  /// </summary>
  public class ucComboPicture : UserControl
  {
    //��������� ���� ���� ...
    private class cComboimage : EvA_CodeText
    {
      public int image=0;
      public cComboimage(int aCode, string aText):base(aCode,aText){}
    }
    private System.Windows.Forms.ComboBox combo;

    private System.ComponentModel.Container components = null;
    private ImageList ic;

    public ucComboPicture()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitForm call
      this.combo.DrawMode= DrawMode.OwnerDrawFixed;
      this.combo.DrawItem+=new DrawItemEventHandler(onDrawItem);;
      this.combo.DropDownStyle=ComboBoxStyle.DropDownList;
      this.combo.Dock=DockStyle.Fill;
      Init();
			this.TabStop = false;
    }
    public ImageList ImageList
    {
      get
      {
        return ic;
      }
      set
      {
        ic=value;
      }
    }

		public int IndexImageFromType(int index)
		{
			int i=0;
			foreach(EvA_CodeText ec in combo.Items)
			{
				if (ec!=null && ec.pCode==index)
				{
					cComboimage cl=combo.Items[i] as cComboimage;
					if (cl!=null)
						return cl.image;
				}
				i++;
			}
			return -1;
		}

    public ComboBox ComboBox
    {
      get
      {
        return combo;
      }
      set
      {
        combo=value;
      }
    }
    private void onResize(object sender,EventArgs e)
    {
      this.Height=combo.Height;
    }   
    protected void Init()
    {
      ic=null;
    }

    /// <summary>
    /// ��������� � ���������
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int AddItem(EvA_CodeText obj,int indexImage)
    {
      int ret=-1;
      if (obj!=null)
      {
        cComboimage cl=new cComboimage(obj.pCode,obj.pText);
        cl.image=indexImage;
        ret=combo.Items.Add(cl);
      }
      return ret;
    }

    /// <summary>
    /// ������ ������ image
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private int getIndex(int index)
    {
      //������ � ��������� combo
      int image=0;
      if (ic!=null && index!=-1 && combo.Items.Count>index)
      {
        cComboimage s=(cComboimage)combo.Items[index];
        if(s!=null && s.image==index)
          image=s.image;
      }
      return image;
    }
    /// <summary>
    /// ������ image �� ����
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Image getImage(int index)
    {
      //���� ��� �� ��� 0 ���������� ��������
      Bitmap image=new Bitmap(this.Font.Height,this.Font.Height);
      if(combo!=null && index!=-1 && combo.Items.Count>index)
      {
        cComboimage s=(cComboimage)combo.Items[index];
        if(s!=null && ic.Images.Count>s.image)
          image=new Bitmap(ic.Images[s.image],this.Font.Height,this.Font.Height);
      }
      return image;
    }
    /// <summary>
    /// ������ text �� index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private string getText(int index)
    {
      //���� ��� �� ��� 0 ���������� ��������
      string ret=string.Empty;
      if(combo!=null && index!=-1 && combo.Items.Count>0 && combo.Items.Count>index)
      {
        cComboimage s=(cComboimage)combo.Items[index];
        if(s!=null)
          ret=s.pText;
      }
      return ret;
    }

    private void onDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
    {	
      Color cBack=Color.Empty;
      Color cFore=Color.Empty;
      int delta=this.Font.Height/2; //������ ������ �� ������
      //���� ����� �� ��������� ��� �������
      if((e.State & DrawItemState.Focus)==0)
      {													
        cFore=SystemColors.WindowText;
        if(this.Enabled)
          cBack=SystemColors.Window;
        else
          cBack=SystemColors.Control;
        SolidBrush solBack=new SolidBrush(cBack);
        SolidBrush solFore=new SolidBrush(cFore);
        //this code keeps the last item drawn from having a Bisque background. 
        e.Graphics.FillRectangle(solBack, e.Bounds);					
        e.Graphics.DrawString(getText(e.Index),this.Font, solFore, new Point(this.Font.Height+delta,e.Bounds.Y));
        e.Graphics.DrawImage(getImage(e.Index), new Point(e.Bounds.X, e.Bounds.Y));					
      }					
      else
      {
        //���� ����� ��������� ��� ������� - ���������
        cFore=SystemColors.ActiveCaptionText;
        cBack=SystemColors.ActiveCaption;

        SolidBrush solFore=new SolidBrush(cFore);
        SolidBrush solBack=new SolidBrush(cBack);
        e.Graphics.FillRectangle(solBack, e.Bounds); //����������� �������
        //�����������
        e.Graphics.DrawString(getText(e.Index),this.Font, solFore, new Point(this.Font.Height+delta,e.Bounds.Y));
        e.Graphics.DrawImage(getImage(e.Index), new Point(e.Bounds.X, e.Bounds.Y));			
      }
    }	

		#region staff .Net

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if(components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

		#region Component Designer generated code
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.combo = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // combo
      // 
      this.combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.combo.Name = "combo";
      this.combo.Size = new System.Drawing.Size(200, 21);
      this.combo.TabIndex = 0;
      // 
      // ucComboPicture
      // 
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.combo});
      this.Name = "ucComboPicture";
      this.Size = new System.Drawing.Size(200, 24);
      this.ResumeLayout(false);

    }
		#endregion

		#endregion --staff .Net
	}
}
