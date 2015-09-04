using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using CommandAS.Tools; 
using CommandAS.Tools.Controls;

namespace CommandAS.Tools.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class dlgCasTreeViewAll : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button cmdAdd;
		private System.Windows.Forms.Button cmdEdit;
		private System.Windows.Forms.Button cmdDelete;
		private System.Windows.Forms.Button cmdSelect;
		private System.Windows.Forms.Button cmdProperty;
		private System.Windows.Forms.Button cmdCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		protected CASTreeViewAll		mTv;
		protected bool							mIsMayEdit;

		public int									pSelectedItemPlace;
		public int									pSelectedItemCode;
		public string								pSelectedItemText;

		public bool									pIsMayEdit
		{
			get { return mIsMayEdit; }
			set 
			{
				mIsMayEdit = value;
				cmdAdd.Visible = value;
				cmdEdit.Visible = value;
				cmdDelete.Visible = value;
				cmdSelect.Visible = value;
				cmdProperty.Visible = value;
				mTv.pIsMayEdit = value;
			}
		}

		public	CASTreeViewAll			pTreeView
		{
			//set { mTv = value;}
			get { return mTv; }
		}

		public	object							pRefBook;

		public dlgCasTreeViewAll(CASTreeViewAll	aTreeView):	this(aTreeView, false){}
		public dlgCasTreeViewAll(CASTreeViewAll	aTreeView, bool aIsMayEdit)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		
			mTv = aTreeView;
			mTv.Visible = true;
			mTv.Parent = this;
			mTv.OnDoCommand += new EvH_CasTVCommand(OnTreeItemDoCommand);
			mTv.OnBeforeViewItemProperty += new CasObjectEventHandler(OnBeforeViewProperty);

			CASTools.SetCommandButton(cmdAdd, eCommand.Add);
			CASTools.SetCommandButton(cmdEdit, eCommand.Edit);
			CASTools.SetCommandButton(cmdDelete, eCommand.Delete);
			CASTools.SetCommandButton(cmdSelect, eCommand.Choice);
			CASTools.SetCommandButton(cmdProperty, eCommand.Property);

			//cmdSelect.DialogResult = DialogResult.OK;

			mTv.BringToFront();
			mTv.TabIndex = 0;

			pIsMayEdit	= aIsMayEdit;
			pRefBook		= null;
		}

		private void OnLoad(object sender, System.EventArgs e)
		{
			OnResize(new EventArgs());
			if (pSelectedItemCode > 0) 
				mTv.Load(pSelectedItemPlace, pSelectedItemCode, false);
			//mTv.Focus(); 
		}

		private void OnResize(object sender, System.EventArgs e)
		{
			if (mIsMayEdit)
			{
				mTv.Top = 0;
				mTv.Left = 0;
				mTv.Width = ClientSize.Width;
				mTv.Height = ClientSize.Height -cmdAdd.Height-3;
			}
			else
			{
				mTv.Top = 0;
				mTv.Left = 0;
				mTv.Width = ClientSize.Width;
				mTv.Height = ClientSize.Height;
			}
			mTv.Invalidate(); 
		}
		/*protected override void OnResize(System.EventArgs ea)
		{
			tv.Invalidate(); 
			base.OnResize(ea);

		}*/
		private void OnBeforeViewProperty(object sender, CasObjectEventArgs e)
		{
			e.pObject = pRefBook;
			e.pInt = ((CASTreeItemData)mTv.SelectedNode.Tag).pPlace; 
		}

		private void OnTreeItemDoCommand(object sender, EvA_CasTVCommand e)
		{
			if (e.pCommand == eCommand.Choice)
				SelectedItem();
		}

		private void DoCommandAdd(object sender, System.EventArgs e)
		{
			mTv.DoCommand(eCommand.Add|eCommand.Edit);
		}

		private void DoCommandEdit(object sender, System.EventArgs e)
		{
			mTv.DoCommand(eCommand.Edit);
		}

		private void DoCommandDelete(object sender, System.EventArgs e)
		{
			mTv.DoCommand(eCommand.Delete);
		}

		private void DoCommandSelect(object sender, System.EventArgs e)
		{
			SelectedItem();
		}

		private void DoCommandProperty(object sender, System.EventArgs e)
		{
			mTv.DoCommand(eCommand.Property);
		}

		private void SelectedItem()
		{
			if (mTv.SelectedNode == null)
				return;

			CASTreeItemData tid = (CASTreeItemData)mTv.SelectedNode.Tag;
			pSelectedItemPlace	= tid.pPlace;
			pSelectedItemCode		= tid.pCode;
			pSelectedItemText		= mTv.SelectedNode.Text;
			
			DialogResult = DialogResult.OK; 
		}

		private void DoCommandCancel(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel; 
		}


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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cmdAdd = new System.Windows.Forms.Button();
			this.cmdEdit = new System.Windows.Forms.Button();
			this.cmdDelete = new System.Windows.Forms.Button();
			this.cmdSelect = new System.Windows.Forms.Button();
			this.cmdProperty = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// cmdAdd
			// 
			this.cmdAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.cmdAdd.Location = new System.Drawing.Point(0, 248);
			this.cmdAdd.Name = "cmdAdd";
			this.cmdAdd.Size = new System.Drawing.Size(23, 23);
			this.cmdAdd.TabIndex = 1;
			this.cmdAdd.Click += new System.EventHandler(this.DoCommandAdd);
			// 
			// cmdEdit
			// 
			this.cmdEdit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.cmdEdit.Location = new System.Drawing.Point(24, 248);
			this.cmdEdit.Name = "cmdEdit";
			this.cmdEdit.Size = new System.Drawing.Size(23, 23);
			this.cmdEdit.TabIndex = 2;
			this.cmdEdit.Click += new System.EventHandler(this.DoCommandEdit);
			// 
			// cmdDelete
			// 
			this.cmdDelete.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.cmdDelete.Location = new System.Drawing.Point(48, 248);
			this.cmdDelete.Name = "cmdDelete";
			this.cmdDelete.Size = new System.Drawing.Size(23, 23);
			this.cmdDelete.TabIndex = 3;
			this.cmdDelete.Click += new System.EventHandler(this.DoCommandDelete);
			// 
			// cmdSelect
			// 
			this.cmdSelect.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.cmdSelect.Location = new System.Drawing.Point(80, 248);
			this.cmdSelect.Name = "cmdSelect";
			this.cmdSelect.Size = new System.Drawing.Size(23, 23);
			this.cmdSelect.TabIndex = 4;
			this.cmdSelect.Click += new System.EventHandler(this.DoCommandSelect);
			// 
			// cmdProperty
			// 
			this.cmdProperty.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.cmdProperty.Location = new System.Drawing.Point(264, 248);
			this.cmdProperty.Name = "cmdProperty";
			this.cmdProperty.Size = new System.Drawing.Size(23, 23);
			this.cmdProperty.TabIndex = 5;
			this.cmdProperty.Click += new System.EventHandler(this.DoCommandProperty);
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(8, 8);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.TabIndex = 6;
			this.cmdCancel.TabStop = false;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.Click += new System.EventHandler(this.DoCommandCancel);
			// 
			// dlgCasTreeViewAll
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(288, 272);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.cmdCancel,
																																	this.cmdProperty,
																																	this.cmdSelect,
																																	this.cmdDelete,
																																	this.cmdEdit,
																																	this.cmdAdd});
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "dlgCasTreeViewAll";
			this.Resize += new System.EventHandler(this.OnResize);
			this.Load += new System.EventHandler(this.OnLoad);
			this.ResumeLayout(false);

		}
		#endregion

	}
}
