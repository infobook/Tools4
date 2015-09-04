using System;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using CommandAS.Tools;

namespace CommandAS.Tools.Controls
{

	public class SelectedTreeItemEventArgs: EventArgs
	{
		private int mPlace;
		private int mCode;
		private string mText;

		public int pPlace 
		{ 
			get { return mPlace;}
		}

		public int pCode 
		{ 
			get { return mCode;}
		}
		public string pText
		{
			get { return mText; }
		}

		public SelectedTreeItemEventArgs(int aPlace, int aCode, string aText)
		{
			mPlace = aPlace;
			mCode = aCode;
			mText = aText;
		}
	}

	/// <summary>
	/// Элемент управления, позволяет осуществить выбор из иерархической структуры (дерева).
	/// Должено быть определено свойство tv, классом производным от CasTreeView, т.к.
	/// сам класс CasTreeView не имеет содержания и для явного использования не подходит !!!
	/// </summary>
	public class SelectFromTree : System.Windows.Forms.UserControl
	{
		#region PROPERTY:
		private System.Windows.Forms.TextBox	txt;
		private System.Windows.Forms.Button		cmd;

		protected	CasTreeView		tv;
		protected int						mTreeItemPlace;
		protected int						mTreeItemCode;
		protected int						mTreeViewHeight;
		//protected bool			mTwinWidth;

		public CasTreeView			pTreeView
		{
			set
			{ 
				tv = value; 
				tv.Visible = false;
			}
		}
    /// <summary>
    /// Транслируем максимальный размер символов для текстового поля
    /// </summary>
    public int MaxLength
    {
      get
      {
        return txt.MaxLength;
      }
      set
      {
        txt.MaxLength=value;
      }

    }
		public int							pRootCode
		{
			set { tv.pRootItemCode = value; }
		}
		
		public BorderStyle			pBorderStyle
		{
			set {
				txt.BorderStyle = value;
				tv.BorderStyle = value;
			}
		}
		
		public int							pItemPlace
		{
			get { return mTreeItemPlace;}
		}
		public int							pItemCode
		{
			get { return mTreeItemCode;}
		}
		public string						pItemText
		{
			get { return txt.Text; }
			set { txt.Text = value; }
		}
		public int							pTextHeight
		{
			get { return txt.Height; }
		}

		public bool								pDisplayContextMenu
		{
			//get { return tv.pDisplayContextMenu; }
			set { tv.pIsMayEdit = value; }
		}

		#endregion

		public delegate void SelectedTreeItemEventHandler(object sender, SelectedTreeItemEventArgs e);
		public event SelectedTreeItemEventHandler OnSelectedTreeItem;
		public delegate void StartSelectTreeItemEventHandler(object sender, EventArgs e);
		public event StartSelectTreeItemEventHandler OnStartSelecteTreeItem;
		public event EventHandler OnLeaveSelecteFromTree;
		public event EventHandler OnValueChanged;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SelectFromTree() : this(null){}
		public SelectFromTree(OleDbConnection aCn)
		{
			mTreeItemPlace = 0;
			mTreeItemCode = 0;
			mTreeViewHeight = 100;
			//mTwinWidth = false;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			tv = new CasTreeView();
			tv.pDBConnection = aCn;
			tv.Visible = false;
			this.Leave  += new EventHandler (OnTxtLeave);
		}
		
		public void SetItem(int aCode)
		{
			SetItem(0, aCode);
		}
		public void SetItem(int aPlace, int aCode)
		{
			mTreeItemPlace = aPlace;
			mTreeItemCode = aCode;
			if(mTreeItemCode > 0 )
			{
				tv.Load (mTreeItemPlace, mTreeItemCode, false);
				if (tv.SelectedNode != null)
					txt.Text = tv.SelectedNode.Text; 
			}
		}
		public void SetItem(int aPlace, int aCode, string aText)
		{
			mTreeItemPlace = aPlace;
			mTreeItemCode = aCode;
			txt.Text = aText;
		}

		#region EVENTS:
		protected override void OnPaint (PaintEventArgs e)
		{
			if (tv.Visible) 
			{
				if (this.Parent != null && this.Parent.Height > 100 )
					mTreeViewHeight = this.Parent.Height*2/3; 
				this.Height = txt.Height+mTreeViewHeight;
				tv.Left = 0;
				tv.Top = txt.Height;
				//tv.Width = mTwinWidth ? this.Width*2 : this.Width;
				tv.Width = this.Width;
				tv.Height = mTreeViewHeight;
			}
			else
			{
				this.Height = txt.Height;
			}
		}
		private void OnLostFocus (object sender, System.EventArgs e)
		{
			if (cmd.Focused) return;
			TvHide();
		}

		private void OnTxtLeave (object sender, System.EventArgs e)
		{
			if (OnLeaveSelecteFromTree != null)
				OnLeaveSelecteFromTree (this, new EventArgs());
		}
			
		private void OnDoubleClickTreeItem(object sender, System.EventArgs e)
		{
			try
			{
				mTreeItemPlace = ((TreeItemData)tv.SelectedNode.Tag).pPlace; 
				mTreeItemCode = ((TreeItemData)tv.SelectedNode.Tag).pCode; 
				txt.Text = tv.SelectedNode.Text;
				SelectedTreeItemEventArgs ee = new SelectedTreeItemEventArgs(mTreeItemPlace, mTreeItemCode, txt.Text);
				if (OnSelectedTreeItem != null)
					OnSelectedTreeItem (this, ee); 
			}
			catch(Exception ex) 
			{
				MessageBox.Show(ex.Message);  
			}
			finally
			{
				TvHide();
			}
		}

		private void cmd_Click(object sender, System.EventArgs e)
		{
			tv.Visible = ! tv.Visible;
			this.Invalidate ();
			if (tv.Visible)
			{
        tv.Parent = this;
        tv.Load (mTreeItemPlace, mTreeItemCode, false); 
        this.BringToFront();
        tv.Focus();
        if (OnStartSelecteTreeItem != null)
          OnStartSelecteTreeItem (this, new EventArgs()); 
        tv.DoubleClick += new System.EventHandler(OnDoubleClickTreeItem);
        tv.LostFocus += new EventHandler (OnLostFocus);
			}
			else
				TvHide();

		}

		private void OnTextChanged(object sender, System.EventArgs e)
		{
			if (txt.Text.Length == 0)
				mTreeItemCode = 0;
			if (OnValueChanged != null)		
				OnValueChanged(this, new EventArgs());  
		}
		
		#endregion

		public void FocusTvTxt()
		{
			if (tv.Visible)
				tv.Focus(); 
			else
				txt.Focus(); 
		}

		private void TvHide()
		{
			tv.Hide(); 
			tv.DoubleClick -= new System.EventHandler(OnDoubleClickTreeItem);
			tv.LostFocus -= new EventHandler (OnLostFocus);
			tv.Parent = null;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      this.txt = new System.Windows.Forms.TextBox();
      this.cmd = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txt
      // 
      this.txt.AutoSize = false;
      this.txt.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txt.Name = "txt";
      this.txt.Size = new System.Drawing.Size(160, 24);
      this.txt.TabIndex = 0;
      this.txt.Text = "";
      this.txt.TextChanged += new System.EventHandler(this.OnTextChanged);
      // 
      // cmd
      // 
      this.cmd.BackColor = System.Drawing.SystemColors.Menu;
      this.cmd.Dock = System.Windows.Forms.DockStyle.Right;
      this.cmd.Font = new System.Drawing.Font("Marlett", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
      this.cmd.Location = new System.Drawing.Point(140, 0);
      this.cmd.Name = "cmd";
      this.cmd.Size = new System.Drawing.Size(20, 24);
      this.cmd.TabIndex = 1;
      this.cmd.TabStop = false;
      this.cmd.Text = "6";
      this.cmd.Click += new System.EventHandler(this.cmd_Click);
      // 
      // SelectFromTree
      // 
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.cmd,
                                                                  this.txt});
      this.Name = "SelectFromTree";
      this.Size = new System.Drawing.Size(160, 24);
      this.FontChanged += new System.EventHandler(this.onFontChange);
      this.ResumeLayout(false);

    }
		#endregion

    private void onFontChange(object sender, System.EventArgs e)
    {
      Font newFont=new Font("Marlett",this.Font.Size-1, FontStyle.Bold);
      Graphics g =this.CreateGraphics();
      float wi = g.MeasureString("6",newFont).Width;
      cmd.Font=newFont;
      cmd.Width=(int)wi;
      cmd.Left=Width-cmd.Width;
      g=null;
    }

	}

}
