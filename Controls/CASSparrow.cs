using System;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{

	/// <summary>
	/// 
	/// </summary>
	public class CASSparrow : System.Windows.Forms.Control //System.Windows.Forms.UserControl
	{
		#region PROPERTY:

		protected System.Windows.Forms.Button		cmd;

		protected TextBox											txt;
		protected	CASTreeViewCommon						tv;
		protected PlaceCode										mPC;
		protected int													mTreeViewHeight;
		protected bool												mIsTVVisable;
		protected Label												mLblParents;
		protected int													mQntParentsView;

		//всплывающее окно!
		private Form													frm;
		private Rectangle											sfrm; //запоминаем размеры!
		protected System.Windows.Forms.Label		lblSize;
		private System.Windows.Forms.ToolTip  tTip;
		private System.ComponentModel.IContainer components;
		/// <summary>
		/// ¬ TextBox можно добавл€ть элемент, которого нет в справочнике
		/// и при этом он не добавл€етс€ туда.
		/// </summary>
		public bool														pIsMayBeWithoutRefbook;
		public bool														pDownSelectIfNotFound;
		/// <summary>
		/// –аскрывать узел при открытии иерархии.
		/// </summary>
		public bool														pIsExpandLevelWhenLoad;

		#region Public Property

		/// <summary>
		/// ѕри наведении фокуса на форму - всегда вставать на текстовое поле
		/// </summary>
		/// <returns></returns>
		public new bool                       Focus()
		{
			return FocusTvTxt();
		}

		public bool FocusTvTxt()
		{
			if (mIsTVVisable)
				return tv.Focus(); 
			else
				return txt.Focus(); 
		}

    public new bool                       Focused
    {
      get
      {
        if (mIsTVVisable)
          return tv.Focused; 
        else
          return txt.Focused; 
      }
    }
		public TextBox												pTextBox
		{
			get {return txt;}
		}

		public CASTreeViewCommon							pTreeView
		{
			set
			{ 
				if (tv!=null)
					tv.Dispose();

				tv = value; 
				mIsTVVisable = false;
				if (tv != null)
				{
					tv.Font=this.Font;
					tv.AddMenuCommand(eCommand.Choice);
					tv.Visible = mIsTVVisable;
					tv.pIsMaySelectedNode = true;
				}
			}
			get { return tv;}
		}
		
		public BorderStyle										pBorderStyle
		{
			set 
			{
				txt.BorderStyle = value;
				tv.BorderStyle = value;
			}
		}
		
		public PlaceCode											pItemPC
		{
			get { return mPC;  }
			set { mPC = value; }
		}
		public int														pItemPlace
		{
			get { return mPC.place; }
			set { mPC.place = value;}
		}
		public int														pItemCode
		{
			get { return mPC.code;}
		}
		public string													pItemText
		{
			get { return txt.Text; }
			set { txt.Text = value; }
		}

		public int														pItemBinding
		{
			set
			{
				mPC.code = value;
				if (mPC.code >0)
				{
					tv.Load (mPC.place, mPC.code, false);
					if (tv.SelectedNode !=null)
					{
						txt.Text = tv.SelectedNode.Text;
						if (mLblParents != null)
							mLblParents.Text = GetParentsFrom (tv.SelectedNode);
					}
				}
				else
					txt.Text=string.Empty;
			}
			get
			{
				return mPC.code;
			}
		}

		#endregion --Public Property
  
		#endregion

		#region Public events

		public delegate void SelectedTreeItemEventHandler(object sender, EvA_SelectedTreeItem e);
		public event SelectedTreeItemEventHandler OnSelectedTreeItem;
		//public event EventHandler OnLeaveSelecteFromTree;
		public event EventHandler OnValueChanged;

		#endregion Public events

		#region constructor

		public CASSparrow() : this (false) {}
		public CASSparrow(bool aIsDataGridTextBox) : this (aIsDataGridTextBox, 0) {}
		public CASSparrow(bool aIsDataGridTextBox, int aQntParentsView)
		{
			mPC = PlaceCode.Empty;
			mTreeViewHeight = 100;
			pIsMayBeWithoutRefbook = false;
			pDownSelectIfNotFound = true;
			pIsExpandLevelWhenLoad = false;
			mIsTVVisable = false;

			SuspendLayout();

			mQntParentsView = aQntParentsView;
			if (mQntParentsView == 0)
			{
				mLblParents = null;
			}
			else
			{
				mLblParents = new Label();
				mLblParents.Size = new System.Drawing.Size(60, 22);
				mLblParents.TextAlign = ContentAlignment.MiddleRight;
				this.Controls.Add(mLblParents);
			}

			if (aIsDataGridTextBox)
				txt = new DataGridTextBox();
			else
				txt = new TextBox();
			txt.Name = "txt";
			txt.Size = new System.Drawing.Size(160, 22);
			txt.TabIndex = 0;
			txt.Text = "";
			//txt.AutoSize=true;
			txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			txt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
			txt.TextChanged += new EventHandler(OnTextChanged);
			this.Controls.Add(txt);

			tv = new CASTreeViewCommon();
			tv.Font=this.Font;
			tv.ClearMenuCommand();              //удал€ем всЄ меню !
			tv.AddMenuCommand(eCommand.Choice); //разрешаем только выбирать !
			tv.Visible = mIsTVVisable;
			
			this.ResumeLayout(false);
			this.Layout +=new LayoutEventHandler(OnControlLayout);

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			OnResize(EventArgs.Empty);
		}

		#endregion --constructor

		#region SetItem
		public void SetItem(int aCode)
		{
			SetItem(new PlaceCode(0, aCode));
		}
		public void SetItem(int aPlace, int aCode)
		{
			SetItem(new PlaceCode(aPlace, aCode));
		}
		public void SetItem(PlaceCode aPC)
		{
			mPC = aPC;
			if(mPC.code > 0 )
			{
				tv.Load (mPC.place, mPC.code, false);
				if (tv.SelectedNode != null)
				{
					txt.Text = tv.SelectedNode.Text; 
					if (mLblParents != null)
						mLblParents.Text = GetParentsFrom (tv.SelectedNode);
				}
			}
		}

		private string GetParentsFrom (TreeNode aTN)
		{
			string ret = string.Empty;

			string [] ss = aTN.FullPath.Split(tv.PathSeparator.ToCharArray());
			int ii = ss.Length - mQntParentsView - 1;
			if (ii < 0)
				ii = 0;
			for (; ii < ss.Length - 1; ii++)
				ret += ss[ii] + tv.PathSeparator;

			return ret;
		}

		public void SetItem(PlaceCode aPC, string aText)
		{
			mPC = aPC;
			txt.Text = aText;
		}
		public void SetItem(int aPlace, int aCode, string aText)
		{
			mPC.place = aPlace;
			mPC.code = aCode;
			txt.Text = aText;
		}
		#endregion

		#region block AddNodes-AddTreeView-RemoveAllNodes

		/// <summary>
		/// ƒобавл€ем в ноды с верхним уровнем
		/// </summary>
		/// <param name="aNode"></param>
		public void AddNodes(TreeNode aNode)
		{
			AddNodes(aNode,false);
		}
		/// <summary>
		/// ƒобавл€ем в ноды как хотим
		/// </summary>
		/// <param name="aNode"></param>
		/// <param name="viewRoot">ѕоказывать верхний уровень ?</param>
		public void AddNodes(TreeNode aNode,bool viewRoot)
		{
			if (aNode != null)
			{
				if (viewRoot)
					tv.Nodes.Add((TreeNode)aNode.Clone());
				else
				{
					foreach(TreeNode tn in aNode.Nodes)
						tv.Nodes.Add((TreeNode)tn.Clone());
				}
			}
		}
		/// <summary>
		/// ƒобавл€ем в ноды как хотим
		/// </summary>
		/// <param name="aNode"></param>
		public void AddTreeView(CASTreeViewCommon aView)
		{
			if (aView != null)
			{
				tv=(CASTreeViewCommon)aView.Clone();
				tv.Refresh();
			}
		}

		public void RemoveAllNodes()
		{
			tv.Nodes.Clear();
			//while(tv.Nodes.Count>0)
			//	tv.Nodes.RemoveAt(0); 
		}
		#endregion --block AddNodes-AddTreeView-RemoveAllNodes

		#region EVENTS:
			
		private void cmd_Click(object sender, System.EventArgs e)
		{
			TV(!mIsTVVisable);
		}

		private void OnTreeItemDoCommand(object sender, EvA_CasTVCommand e)
		{
			if (e.pCommand == eCommand.Choice)
				SelectedItem();
		}

		private void OnTextChanged(object sender, System.EventArgs e)
		{
			if (txt.Text.Length == 0)
			{
				mPC.code = 0;
				//удал€ем все подсказки
				tTip.RemoveAll();
			}
			else if(tv.SelectedNode!=null)
				//показываем полный путь
				tTip.SetToolTip(txt,tv.SelectedNode.FullPath);
			if (OnValueChanged != null)		
				OnValueChanged(this, new EventArgs());  
		}

    protected override bool ProcessKeyMessage(ref Message m)
    {
      switch ((Keys)((int) m.WParam))
      {
        case Keys.Escape:
          this.Parent.Focus();
          break;
          //return true;
        case Keys.Tab:
          // neutralize Tab key up message
          return true;
        default:
          break;
      }
      return base.ProcessKeyMessage( ref m );
    }

		private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (txt.ReadOnly)
				return ;
      
			if (e.KeyCode == Keys.F4)
			{ //вызываем "дерев€нный" комбик
				//просто показываем - оно само найдетс€ !
				TV(true);
				e.Handled = true;
			}
		}

		private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (txt.ReadOnly)
				return;

			//e.Handled = false;
			int selectionStart;
			int selectionLength;
			if (txt.Text.Length > 0 && ((int)e.KeyChar) > 0x1F )
			{
				string newTxt;
				if (txt.SelectionStart > 0 && txt.SelectionStart < txt.Text.Length)
					newTxt = txt.Text.Substring(0,txt.SelectionStart) + e.KeyChar.ToString(); 
				else
					newTxt = txt.Text + e.KeyChar.ToString(); 
				int fq = 0;
				TreeNode tn = CASTools.SearchByTextInTreeNodeCollection(tv.Nodes, newTxt, ref fq, false);
				if (tn != null)
				{
					mPC = ((CASTreeItemData)tn.Tag).pPC; 
					selectionStart = newTxt.Length;
					selectionLength = tn.Text.Length - newTxt.Length;
					txt.Text = tn.Text;
					txt.SelectionStart = selectionStart;
					txt.SelectionLength = selectionLength;
					e.Handled = true;
					tTip.SetToolTip(txt,tn.FullPath);
				}
				else
				{
					mPC = PlaceCode.Empty;
					tTip.RemoveAll();
					if (!mIsTVVisable && pDownSelectIfNotFound)
					{
						txt.Focus(); 
					}
				}
			}
		}
		
		#endregion

		#region local functions
		/// <summary>
		/// ¬ыбираем из дерева
		/// </summary>
		private void SelectedItem()
		{
			try
			{
				//if (tv.SelectedNode != null)
				//{
				mPC = ((CASTreeItemData)tv.SelectedNode.Tag).pPC; 
				txt.Text = tv.SelectedNode.Text;
				tTip.SetToolTip(txt,tv.SelectedNode.FullPath);
				if (mLblParents != null)
					mLblParents.Text = GetParentsFrom (tv.SelectedNode);
				if (OnSelectedTreeItem != null)
					OnSelectedTreeItem (this, new EvA_SelectedTreeItem(mPC,txt.Text)); 
				//}
			}
#if DEBUG
			catch (Exception ex) 
			{
				MessageBox.Show("CasSparrow : "+ex.Message); 
#else
      catch
      {
#endif
				tTip.RemoveAll();
			}
			finally
			{
				TV(false);
			}
		}


		private void TV(bool isShow)
		{
			if (mIsTVVisable == isShow)
				return;

			mIsTVVisable = isShow;
			if (isShow)
			{
				cmd.Text = "5";
				InitFrmTV();
				ShowFrmTV();        

				tv.BeginUpdate();
				if (txt.Text.Length>0)
				{
					if (tv.SelectedNode != null && tv.SelectedNode.Text.Equals(txt.Text))
					{ // ничего не делаем ...
					}
					else
					{ // ищем
						TreeNode tn = tv.SearchByText(txt.Text, true, true, true, null);
						if (tn !=null)
							tv.SelectedNode = tn;
						else if (mPC.code > 0)
							tv.Load (mPC.place, mPC.code, pIsExpandLevelWhenLoad); 
					}
				}
				else if (mPC.code > 0)
					tv.Load (mPC.place, mPC.code, pIsExpandLevelWhenLoad); 
				else
					tv.Load();
				tv.EndUpdate();

				//показываем выделенный узел
				if (tv.SelectedNode!=null)
					tv.SelectedNode.EnsureVisible();
				if (tv.CanFocus)
					tv.Focus();
			}
			else
			{
				cmd.Text = "6";
				ToolHide();

				if (mPC.code == 0 && pIsMayBeWithoutRefbook==false)
					txt.Text = string.Empty; 
			}
			this.Refresh();
		}
		
		#endregion --local functions

    #region –аботаем с формой - выводом TV

		/// <summary>
		/// —оздаем новое окно, если оно еще не создано!
		/// </summary>
		private void InitFrmTV()
		{
			if (frm==null)
			{
				frm=new System.Windows.Forms.Form();
				frm.StartPosition=FormStartPosition.Manual;
				frm.FormBorderStyle= FormBorderStyle.SizableToolWindow; //FormBorderStyle.Sizable
				frm.ControlBox=false;
				if (sfrm.IsEmpty)
					frm.Size=new Size(this.Width,150);
				else
					frm.Bounds=sfrm;
				frm.Font=this.Font;
				frm.ShowInTaskbar=false;
				frm.KeyPreview=true;

				//tv.LostFocus				+= new EventHandler (OnLostFocus);
				//tv.AfterSelect			+= new TreeViewEventHandler(OnTreeItemSelected);
				tv.OnDoCommand			+= new EvH_CasTVCommand(OnTreeItemDoCommand);

				frm.KeyDown   +=new KeyEventHandler(OnFrmKeyDown);
				frm.Deactivate+=new EventHandler(onToolHide);
				//оптимизируем и не закрываем окно !!!
				frm.Closing+=new CancelEventHandler(onToolClosing);
			}
		}
		/// <summary>
		/// ќбрабатываем спец. клавиши
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFrmKeyDown(object sender,KeyEventArgs e)
		{
			//если esc - выходим!
			if (e.KeyCode == Keys.Escape)
				ToolHide();
			else if(e.KeyCode == Keys.Enter && tv.SelectedNode!=null)
			{
				SelectedItem();
				ToolHide();
			}
		}
		/// <summary>
		/// ѕозиционируем окно в координатах
		/// </summary>
		private void ShowFrmTV()
		{
			if (frm!=null)
			{
				//узнаем координаты в экране txt
				if (sfrm.IsEmpty)
				{
					frm.Width=this.Width;
				}
				else
				{
					frm.Bounds=sfrm;
				}
				if (mLblParents == null)
					frm.Bounds= CASTools.GetBoundsControl(txt,frm);
				else
					frm.Bounds= CASTools.GetBoundsControl(mLblParents,frm);

				//if (tv.Parent==null)
				{
					tv.Parent=frm;
					tv.Dock=DockStyle.Fill;
				}
				tv.Visible = true;
				tv.Focus();

				lblSize.Visible=true;
				lblSize.Top=frm.Height-lblSize.Height-2;
				lblSize.Left=frm.Width-lblSize.Width-2;
				//lblSize.BringToFront();
        
				frm.BringToFront();
				frm.Visible=true;
			}
		}
		//оптимизируем и не закрываем окно !!!
		private void onToolClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ToolHide();
			e.Cancel=true;
		}
		/// <summary>
		/// ѕрограммно убираем окно и выполн€ем отмену
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void onToolHide(object sender,EventArgs e)
		{
			ToolHide();
		}
		private void ToolHide()
		{
			if (frm!=null)
			{
				//lblSize.SendToBack(); //пр€чем уголок
				sfrm = frm.Bounds;
				frm.Visible=false;
			}
			cmd.Text = "6"; //чтобы кнопка отработала
			mIsTVVisable=false;
			this.Focus();
		}
    #endregion

    #region ƒелаем контрол красивым ...

		private void onResize()
		{

			txt.Top=0;
			txt.Height=this.Font.Height+4; //на рамку 

			cmd.Top=0;
			cmd.Height=txt.Height-2; //на рамку 
			cmd.Width=SystemInformation.VerticalScrollBarWidth+2; //на рамку
			cmd.Left=Width-cmd.Width;

			if (mLblParents != null)
			{
				mLblParents.Top = 0;
				mLblParents.Height = txt.Height;
				mLblParents.Left=0;
				int ww = this.Width-cmd.Width;
				mLblParents.Width = mQntParentsView > 1 ? ww*2/3 : ww/2;

				txt.Left = mLblParents.Width+2;
				txt.Width= ww - txt.Left;
			}
			else
			{
				txt.Left=0;
				txt.Width= this.Width-cmd.Width;
			}

			this.Height=txt.Height;

			if (tv!=null)
				tv.Font=this.Font;
		}

		protected override void OnResize(System.EventArgs e)
		{
			onResize();
		}

		private void onFontChange(object sender, System.EventArgs e)
		{
			onResize();
		}
		//  перерисовка дл€ нормального отображени€ стрелки развертывани€ дерева дл€ любых разрешений экрана
		private void OnControlLayout(object sender, LayoutEventArgs e)
		{
			onResize();
		}
    #endregion
		
		#region Staff .Net

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
				Controls.Clear();
				//if (tv != null)
				//{
				//	tv.Parent								= null;
				//	tv.SelectedNode					= null;
				//	tv.LabelEdit						= false;
				//	tv.Visible							= false;	
				//	RemoveAllNodes();
				//	tv											= null;
				//}

				if (frm!=null)
					frm.Dispose();
				frm=null;

				txt.Dispose();
				txt = null;
				cmd.Dispose();
				cmd = null;

				OnSelectedTreeItem			= null;
				OnValueChanged					= null;

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
			this.components = new System.ComponentModel.Container();
			this.cmd = new System.Windows.Forms.Button();
			this.lblSize = new System.Windows.Forms.Label();
			this.tTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// cmd
			// 
			this.cmd.BackColor = System.Drawing.SystemColors.Control;
			this.cmd.Dock = System.Windows.Forms.DockStyle.Right;
			this.cmd.Font = new System.Drawing.Font("Marlett", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.cmd.Location = new System.Drawing.Point(140, 0);
			this.cmd.Name = "cmd";
			this.cmd.Size = new System.Drawing.Size(20, 24);
			this.cmd.TabIndex = 1;
			this.cmd.Text = "6";
			this.cmd.Click += new System.EventHandler(this.cmd_Click);
			// 
			// lblSize
			// 
			this.lblSize.AutoSize = true;
			this.lblSize.BackColor = System.Drawing.Color.Transparent;
			this.lblSize.Font = new System.Drawing.Font("Marlett", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.lblSize.Location = new System.Drawing.Point(223, 0);
			this.lblSize.Name = "lblSize";
			this.lblSize.Size = new System.Drawing.Size(17, 15);
			this.lblSize.TabIndex = 2;
			this.lblSize.Text = "o";
			this.lblSize.Visible = false;
			// 
			// CASSparrow
			// 
			this.Controls.Add(this.lblSize);
			this.Controls.Add(this.cmd);
			this.Size = new System.Drawing.Size(160, 24);
			this.TabStop = false;
			this.FontChanged += new System.EventHandler(this.onFontChange);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion --Staff .Net
	}
}

