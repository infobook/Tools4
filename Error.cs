using System;
using System.Collections;
using System.Management;
using System.Resources;
using System.Reflection;  
using System.Windows.Forms;  

namespace CommandAS.Tools
{

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Error : DisposableType, ICloneable
	{
		public int				code;
		public string			text;
		public string			description;
    public string     header; //записываем модуль!

    protected ResourceManager mRm;
    
    public Exception	ex
		{
			set {
				if (value != null)
				{
					//mEx = value;
					code = value.GetHashCode();
					text = value.Message;
					description = value.ToString(); 
          header=value.Source;
				}
			}
			//get {return(new Exception(text));}
		}
		
		public bool				IsOk
		{
			get { return (! IsError);}
		}
		public bool				IsError
		{
			get { return (text.Length>0 || code != 0);}
		}

    public Error() : this(0, string.Empty, string.Empty, string.Empty) { }
    public Error(int aCode, string aText) : this(aCode, aText, string.Empty, string.Empty) { }
    public Error(int aCode, string aText, string aHeader) : this(aCode, aText, aHeader, string.Empty) { }
    public Error(int aCode, string aText, string aHeader, string aDescription)
		{
			mRm = new ResourceManager(
				"CommandAS.Tools.Error", 
				Assembly.GetExecutingAssembly()
			);
      code = aCode;
      text = aText;
      description = aDescription;
      header = aHeader;
    }

		public void Clear()
		{
			code = 0;
			text = string.Empty;
      header=string.Empty;
      description = string.Empty;
    }
    public void Show()
    {
       Show(MessageBoxIcon.Error,"Внимание");
    }

		public static void ShowError(string aText)
		{
			MessageBox.Show (aText,"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
    public static void ShowErrorArray(ArrayList aErrArr)
    {
      Form dlg = new Form();
      dlg.Width = 500;
      dlg.Height = 300;

      Button cmd = new Button();
      cmd.Text = "Ok";
      cmd.Left = (dlg.ClientSize.Width - cmd.Width) / 2;
      cmd.Top = dlg.ClientSize.Height - cmd.Height - 10;
      cmd.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
      cmd.DialogResult = DialogResult.OK;

      ListView lst = new ListView();
      lst.Left = 3;
      lst.Top = 3;
      lst.Width = dlg.ClientSize.Width - lst.Left * 2;
      lst.Height = cmd.Top - 10;
      lst.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

      foreach (Error err in aErrArr)
      {
        ListViewItem lvi = new ListViewItem();
        lvi.Text = err.text;
        lvi.SubItems.Add(err.description);
        lvi.SubItems.Add(err.header);
        lvi.SubItems.Add(err.code.ToString());
        lst.Items.Add(lvi);

        //lst.Items.Add("(" + err.code + ") - "+,);
      }
      lst.View = View.Details;
      lst.Columns.Add("Сообщение", -2, HorizontalAlignment.Left);
      lst.Columns.Add("Место", -2, HorizontalAlignment.Left);
      lst.Columns.Add("Пояснение", -2, HorizontalAlignment.Left);
      lst.Columns.Add("Код", -2, HorizontalAlignment.Center);

      //lst.AutoResizeColum

      dlg.Controls.Add(cmd);
      dlg.Controls.Add(lst);

      dlg.AcceptButton = cmd;
      dlg.CancelButton = cmd;

      dlg.ShowDialog();
    }

    /// <summary>
		/// Показываем сообщение с иконкой и заголовком сообщения
		/// </summary>
		/// <param name="icon"> из пространства имен MessageBoxIcon</param>
		public void Show(MessageBoxIcon icon,string nameForm)
		{
      string tCaption=string.Empty;
      if (header!=null)
        tCaption+=header+" ";
      if (icon!=MessageBoxIcon.None)
      {
        switch(icon.ToString())
        {
          case "Error"        :
          case "Hand"         : tCaption+="Ошибка!";break;
          case "Information"  : tCaption+="Информация!";break;

          case "Question"     :
          case "Asterisk"     : tCaption+="Вопрос!";break;
          case "Stop"         : tCaption+="Остановитесь!";break;
          case "Warning"      : tCaption+="Внимание!";break;
        }

      }
      if (nameForm!=null && nameForm.Length>0)
        tCaption=nameForm+" : "+tCaption;
      if (tCaption!=string.Empty && text.Length>0)
  		  MessageBox.Show (text,tCaption,MessageBoxButtons.OK,icon); 
			else if(description.Length>0)
				MessageBox.Show (description,text,MessageBoxButtons.OK,MessageBoxIcon.Error); 
      else
        MessageBox.Show (text,"",MessageBoxButtons.OK,MessageBoxIcon.Error); 
    }

		public void ShowIfIs()
		{
			if (IsError)
				Show(); 
		}

		public string GetRName(string aRName)
		{
			return mRm.GetString(aRName);
		}
		public string SetRName(string aRName)
		{
			return SetRName(aRName, string.Empty);
		}
    public string SetRName(string aRName,string aModule)
    {
      text		= mRm.GetString(aRName);
      header	=	aModule;

			return text;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				text				= null;
				description	= null;
				ex					= null;
        header      = null;
			}
			base.Dispose();
		}

    /// <summary>
    /// Клонируем себя
    /// </summary>
    /// <returns></returns>
    public virtual object Clone()
    {
      Error err = (Error)base.MemberwiseClone();
      CopyTo(err);
      return err;
    }
    public virtual void CopyTo(Error aErr)
    {
      aErr.code = code;
      aErr.text = text;
      aErr.description = description;
      aErr.mRm = mRm;
    }
	}

#if DEBUG
	/// <summary>
	/// Используется для отладки ...
	/// </summary>
	public class Helper
	{
		private Helper()
		{
		}
		
		public static void Trace ( params object[] list )
		{
			DateTime now = DateTime.Now;
			Console.Write( now.ToString( "[mm:ss.fff] " ));
			for ( int i = 0; i < list.Length; i++ )
			{
				Console.Write( "{0}; ", list[i] );
			}
			Console.WriteLine();
		}
	}	
#endif

}
