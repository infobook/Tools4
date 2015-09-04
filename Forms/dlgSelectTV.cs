using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Win32;
using CommandAS.Tools; 
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class dlgSelectTV : System.Windows.Forms.Form
	{
		#region Visual vars

		private System.Windows.Forms.Button _cmdCancel;
		private System.ComponentModel.IContainer components=null;

		#endregion --Visual vars
		
		#region Local vars

		protected CASTreeViewCommon						mTV;
		protected PlaceCode										mPCLoadingItem;
		protected TreeNode 										mSelectedNode;

		#endregion --Local vars

		#region Property

		public PlaceCode											pObj
		{
			set {mPCLoadingItem = value;}
		}
		public TreeNode 											pSelectedNode
		{
			get { return mSelectedNode; }
			set { mSelectedNode = value;}
		}

		#endregion --Property

		#region Public vars

		public bool														pIsLoadByObj;
		public static string									pSizeLocation = string.Empty;
		public string													pRegPath;

		#endregion --Public vars

		#region constructor

		public dlgSelectTV(CASTreeViewCommon aTV)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_cmdCancel.TabStop = false;

			mPCLoadingItem = PlaceCode.Empty;
			mSelectedNode = null;
			pIsLoadByObj = true;

			mTV = aTV;
			mTV.Dock = DockStyle.Fill;
			mTV.AddMenuCommand(eCommand.Choice);
			mTV.Parent = this;
			//this.Controls.Add(mTV);
			mTV.BringToFront();

			_cmdCancel.Click += new EventHandler (DoCommandCancel);
			this.Load += new EventHandler (OnDlgLoad);
			this.Closed += new EventHandler (OnDlgClosed);
		}

		#endregion --constructor

		#region local events

		private void OnDlgLoad(object sender, EventArgs e)
		{
			mTV.OnDoCommand += new EvH_CasTVCommand(DoCommand);
			if (pIsLoadByObj)
			{
				if (mPCLoadingItem.code > 0)
					mTV.Load(mPCLoadingItem.place, mPCLoadingItem.code, false); 
				else
					mTV.Load();
				if (mTV.SelectedNode != null)
					mTV.SelectedNode.EnsureVisible();
			}

			if (pSizeLocation.Length > 0)
			{
				string[] aStr = pSizeLocation.Split('|');
				if (aStr.Length == 4)
				{
					this.Size = new Size (Convert.ToInt32(aStr[0]), Convert.ToInt32(aStr[1]));
					this.Location = new Point(Convert.ToInt32(aStr[2]), Convert.ToInt32(aStr[3]));
				}
			}
			else
				LoadParametersFromRegister();
		}

		private void OnDlgClosed(object sender, EventArgs e)
		{
			mTV.OnDoCommand -= new EvH_CasTVCommand(DoCommand);
			mTV.Dock = DockStyle.None;
			mTV.Parent = null;
			//this.Controls.Clear();
			if (pSizeLocation.Length == 0)
				SaveParametersToRegister();
			pSizeLocation = this.Size.Width + "|" + this.Size.Height + "|" + this.Location.X + "|" + this.Location.Y;
		}

		private void DoCommandCancel(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void DoCommand(object sender, EvA_CasTVCommand e)
		{
			if (e.pCommand == eCommand.Choice)
			{
				mSelectedNode = e.Node; 
				DialogResult = DialogResult.OK;
			}
		}

		#endregion --local events

		#region registry

		//прямой вызов реестра недопустим !!!
		private void LoadParametersFromRegister()
		{
			/*
			if (pRegPath==null)
        return;
			RegistryKey regkey= Registry.LocalMachine.CreateSubKey(pRegPath);
			try 
			{
				if (regkey != null)
				{
					if((int)regkey.GetValue("StateMaximized") == 1)
						this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
					else
					{
						string[] aStr = regkey.GetValue ("FormSize").ToString().Split('|');
						this.Left = Convert.ToInt32(aStr[0]);
						this.Top = Convert.ToInt32(aStr[1]);
						this.Size = new Size (Convert.ToInt32(aStr[2]), Convert.ToInt32(aStr[3]));
					}
				}
			}
			catch
			{
			}
			*/
		}
		private void SaveParametersToRegister()
		{
	/*
			if (pRegPath==null || pRegPath.Length == 0)
				return;

			RegistryKey regkey = Registry.LocalMachine.CreateSubKey(pRegPath);
			if (regkey == null) 
				regkey = Registry.LocalMachine.CreateSubKey(pRegPath);
			if (this.WindowState == System.Windows.Forms.FormWindowState.Normal) 
			{
				regkey.SetValue("FormSize", 
					this.Left.ToString() + "|" + 
					this.Top.ToString() + "|" + 
					this.Size.Width.ToString() + "|" + 
					this.Size.Height.ToString());
				regkey.SetValue("StateMaximized", 0);
			}
			else if (this.WindowState == System.Windows.Forms.FormWindowState.Maximized)
				regkey.SetValue("StateMaximized", 1);
			
			regkey.Close();
			*/
		}

		#endregion --registry

		#region Staff .Net

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				mSelectedNode	= null;
				mTV = null;
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._cmdCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _cmdCancel
			// 
			this._cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cmdCancel.Location = new System.Drawing.Point(160, 184);
			this._cmdCancel.Name = "_cmdCancel";
			this._cmdCancel.TabIndex = 0;
			this._cmdCancel.Text = "Cancel";
			// 
			// dlgSelectTV
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this._cmdCancel;
			this.ClientSize = new System.Drawing.Size(292, 267);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this._cmdCancel});
			this.Name = "dlgSelectTV";
			this.ResumeLayout(false);

		}
		#endregion

		#endregion --Staff .Net
	}
}
