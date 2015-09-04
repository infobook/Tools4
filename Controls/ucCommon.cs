using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using Microsoft.Win32;
using CommandAS.Tools;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class ucCommon : System.Windows.Forms.UserControl
	{
		#region PROPERTY:
		private StatusBarPanel							_sbp;
		private bool												_isModified; 
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		protected string										mTitle;
		protected bool											mIsModifiedFlagLock;
		protected string										mRegPath;
		/// <summary>
		/// Установка текста в панеле строки состояния, для производных классов,
		/// если указатель на нее установлен.
		/// </summary>
		protected string										mSBText
		{
			set 
			{
				if (_sbp != null)
					_sbp.Text = value;
			}
			get
			{
				return _sbp.Text;
			}
		}


		public IconCollection								pIconCollection;
		public string												pRegPath
		{
			set
			{
				mRegPath = value;
			}
		}
		public StatusBarPanel								pStatusBarPanel
		{
			set 
			{ 
				_sbp = value; 
			}
		}

		public bool													pIsModified
		{
			get {return _isModified;}
			set
			{
				if (mIsModifiedFlagLock)
					return;

				_isModified = value;
				if (OnModifiedFlagChanged != null)
					OnModifiedFlagChanged (this, new EventArgs());
			}
		}

		public virtual string								pControlTitle
		{
			get {	return mTitle; }
			set { mTitle = value;}
		}

		#endregion property.

		///  EVENTS:
		public event EventHandler 					OnModifiedFlagChanged;

		public ucCommon()
		{
			_isModified					= false;
			mRegPath						= string.Empty;
			pIconCollection			= null;
			mIsModifiedFlagLock	= false;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.Load += new EventHandler (DoLoad);
		}

		private void DoLoad(object sender, System.EventArgs e)
		{
			LoadParameterFromRegister();
		}

		protected void Modified()
		{
			pIsModified = true;
		}

    #region Блок сохранения и восстановления параметров в реестре

		protected virtual void  LoadParameter(RegistryKey aRegkey)
		{
		}

		protected virtual void SaveParameter(RegistryKey aRegkey)
		{
		}

		public void LoadParameterFromRegister()
		{
			if (mRegPath.Length == 0)
				return;

			try 
			{
				RegistryKey regkey = Registry.CurrentUser.OpenSubKey(mRegPath);
				if (regkey != null)
					LoadParameter(regkey);
			}
			catch{}
		}

		public void SaveParameterToRegister()
		{
			if (mRegPath.Length == 0)
				return;

			RegistryKey regkey = Registry.CurrentUser.OpenSubKey(mRegPath, true);
			if (regkey == null) 
				regkey = Registry.CurrentUser.CreateSubKey(mRegPath);

			SaveParameter(regkey);

			regkey.Close();
		}

    #endregion

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
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
