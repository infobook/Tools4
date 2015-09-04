using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CommandAS.Tools;
using CommandAS.Tools.Forms;

namespace CommandAS.Tools.Controls
{

	/// <summary>
	/// Базовый класс для отображения иерархической структуры.
	/// Основная функциональность:
	/// - набор базовых виртуальных функций (Load, SearchXXX);
	/// - поиск (рекурсивный обход дерева вперед/назад со сравнением по коду/тексту);
	/// - реализация интерфейса "Навигация по истории состояний";
	/// - поддержка Drag&Drop (по умолчанию выключина - this.AllowDrop=false).
	/// Вторая версия CASTreeViewCommon. Убрана работа с локальным меню 
	/// (она явна грамоздка и лишняя в базовом классе!!!)
	/// </summary>
	public class CASTreeViewBase : System.Windows.Forms.TreeView, INavClient
	{
		#region PROPERTY
		/// <summary>
		/// предыдущий выбранный узел
		/// </summary>
		private TreeNode                  _prevTreeNodeSelected;

		/// <summary>
		/// Текст для пустого элемента
		/// </summary>
		protected const string EMPTY_ITEM_TEXT  = "<пусто>";
		protected bool										mIsLocking;
		//protected CommonIconCollection		mCIC=null;
		protected IconCollection					mICol=null;
		/// <summary>
		/// Искомый текст.
		/// </summary>
		protected string									mFindText;
		/// <summary>
		/// с учетом регистра букв
		/// </summary>
		protected	bool										mIsMatchCase;
		protected	bool										mIsWordWhole;
		/// <summary>
		/// Номер искомого элемента, если их много (>1). 
		/// Используется для продолжения поиска вперед или назад.
		/// </summary>
		protected int											mFindQnt;
		protected Font										mFoundItemFont
		{
			//а может лучше (Font)Font.Clone() ??
			get { return new Font(this.Font.FontFamily.Name, this.Font.Size, FontStyle.Bold); }
		}

		protected MenuItem								mMIDel;
		protected MenuItem								mMIIns;
		protected MenuItem								mMIChoice;
		protected MenuItem								mMIRename;
		protected MenuItem								mMIProp;
		protected MenuItem								mMIFind;

		public Error											pErr; 
		public string											pEmptyItemText = EMPTY_ITEM_TEXT;
		public string											pPathDelim = "~";
		/// <summary>
		/// Возможность выбирать узлы
		/// </summary>
		public bool												pIsMaySelectedNode;
		/// <value>
		/// Возвращает уровень текущего <see cref="TreeView.SelectedNode"/> элемента иерархии.
		/// </value>
		public int												pLevelSelectedNode
		{
			get 
			{
				int level = -1; 
				try{ level = Level(SelectedNode); }
				catch{}
				return level; 
			}
		}
		
		public IconCollection							IconCollection
		{
			set 
			{ 
				mICol = value;
				if (value != null)
					ImageList = value.pImageList;
			}
			get { return mICol; }
		}

		/// <summary>
		/// Текст поиска
		/// </summary>
		public string											pFindTxt
		{
			set{	mFindText = value;	}
		}

		//в некоторых случаях блокировать вызов поиска снаружи
		public bool												pUseLockFind=false;

		public MenuItem										pMenuItem_Del
		{
			get { return mMIDel;}
		}
		public MenuItem										pMenuItem_Ins
		{
			get { return mMIIns;}
		}
		public MenuItem										pMenuItem_Choice
		{
			get { return mMIChoice;}
		}
		public MenuItem										pMenuItem_Rename
		{
			get { return mMIRename;}
		}
		public MenuItem										pMenuItem_Property
		{
			get { return mMIProp;}
		}
		public MenuItem										pMenuItem_Find
		{
			get { return mMIFind;}
		}


		#endregion property.

		#region EVENTS
		//событие при нахождении над узлом
		public event IBNodeViewDragDropHandler	onMyDragOver;
		//событие при нахождении после отпускания узла
		public event IBNodeViewDragDropHandler	onMyDragRun;
		public event EvH_CasTVCommand						onDoCommand;
		public event EvH_CasTV									onAfterSelectRight;
		public delegate void IBNodeViewDragDropHandler(TreeNode Source, TreeNode Destination,DragEventArgs e);
		
		#endregion

		public CASTreeViewBase()
		{
			pErr = new Error();

			mICol = null;

			pIsMaySelectedNode = false;

			mFindText			=	string.Empty;
			mIsMatchCase	= false;
			mIsWordWhole	= false;
			mIsLocking		= false;

			_prevTreeNodeSelected = null;

			DoubleClick			+= new System.EventHandler(OnDoubleClickTreeItem);
			MouseDown				+= new System.Windows.Forms.MouseEventHandler(OnMouseDown);
			BeforeLabelEdit += new NodeLabelEditEventHandler (OnBeforeLabelEdit);
			AfterLabelEdit	+= new NodeLabelEditEventHandler (OnAfterLabelEdit);

			mMIDel		= null;
			mMIIns		= null;
			mMIChoice	= null;
			mMIRename	= null;
			mMIProp		= null;
			mMIFind		= null;

			#region DragAndDrop:
			AllowDrop = false; // по умолчанию - выключено !!!
			DragEnter+=new DragEventHandler(this_DragEnter);
			DragOver+=new DragEventHandler(this_DragOver);
			ItemDrag+=new ItemDragEventHandler(this_ItemDrag);
			QueryContinueDrag+=new QueryContinueDragEventHandler(this_QueryContinueDrag);
			DragDrop+=new DragEventHandler(this_DragDrop);
			#endregion
		}

		/// <summary>
		/// Определяет уровень в иерархии (начиная с 1).
		/// </summary>
		/// <param name="aNd">элемент иерархии (<see cref="System.Windows.Forms.TreeNode"/>) для которого определяется уровень</param>
		/// <returns>уровень начиная с 1 (для корня)</returns>
		public static int Level(TreeNode aNd)
		{
			int level = 1; 
			if (aNd != null)
				while ((aNd = aNd.Parent) != null) 
					level++; 
			 
			return level; 
		}

		/// <summary>
		/// Возвращает TreeItemData (доп. данный) для корневого элемента.
		/// </summary>
		/// <param name="aNd">элемент иерархии</param>
		/// <returns>TreeItemData (доп. данный)</returns>
//		public TreeItemData RootData  (TreeNode aNd)
//		{
//			TreeItemData ret = null; 
//			if (aNd != null)
//			{
//				while (aNd.Parent != null)
//					aNd = aNd.Parent;
//				ret = (TreeItemData)aNd.Tag;
//			}
//			 
//			return ret; 
//		}

		public TreeNode	Root(TreeNode aTN)
		{
			TreeNode ret = aTN; 
			if (ret != null)
				while (ret.Parent != null)
					ret = ret.Parent;
			return ret; 
		}
		
		#region GetRelativePath ...
		public string GetRelativePath ()
		{
			return GetRelativePath (SelectedNode, pPathDelim);
		}
		public string GetRelativePath (TreeNode aTn)
		{
			return GetRelativePath (aTn, pPathDelim);
		}
		/// <summary>
		/// Возвращает относительный путь (индекс в коллекции Nodes) для элемента иерархии.
		/// </summary>
		/// <param name="aTn"></param>
		/// <returns></returns>
		public static string GetRelativePath (TreeNode aTn, string aPathDelim)
		{
			string ret = string.Empty;
			while (aTn != null)
			{
				ret += aTn.Index+aPathDelim;
				aTn = aTn.Parent;
			}
			string[] ss = ret.Split(aPathDelim.ToCharArray());
			ret = string.Empty;
			for (int ii = ss.Length-2; ii >= 0; ii--)
				ret += ss[ii]+aPathDelim;

			if (ret.Length > 0)
				ret = ret.Substring(0, ret.Length-1);

			return ret;
		}
		#endregion

		#region Load ...
		/// <summary>
		/// Загрузка дерева с корневого элемента без раскрытия следующего уровня.
		/// </summary>
		public void Load()
		{
			Load(false);
		}

		public virtual void Load(bool aExpandLevel)
		{
		}
		public virtual void Load(PlaceCode aPC, bool aExpandLevel)
		{
			SelectedNode = SearchByCode(aPC);
			if (SelectedNode != null && aExpandLevel)
				SelectedNode.Expand();
			//TreeNode retNd = SearchByCode(aPlace, aCode);
			//if (retNd != null)
			//	SelectedNode = retNd;
		}

		/// <summary>
		/// Загрузка элементов иерархии по относительному пути
		/// </summary>
		/// <param name="aItemPath"></param>
		/// <param name="aExpandLevel"></param>
		public void LoadByRelativePath(string aItemPath, bool aExpandLevel)
		{
			if (aItemPath == null || aItemPath.Length == 0) 
			{
				Load(true);
				pErr.text = "path empty";
				return;
			}
			BeginUpdate();
			Nodes.Clear();
			Load(aExpandLevel); 
			string[] ss = aItemPath.Split(pPathDelim.ToCharArray());
			if (ss.Length>0)
			{
				try
				{
					TreeNode tn = Nodes[Convert.ToInt32(ss[0])];
					for (int ii=1; ii < ss.Length && tn != null ; ii++)
					{
						tn.Expand();
						tn = tn.Nodes[Convert.ToInt32(ss[ii])];
					}
					if (tn != null)
					{
						SelectedNode = tn;
						if (aExpandLevel)
							SelectedNode.Expand(); 
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			EndUpdate();
		}
		#endregion
		
		#region Search:
		#region Search by text
		public TreeNode SearchByText(string aText)
		{
			return SearchByText(aText, false, false, true);
		}
		public TreeNode SearchByText(string aText, bool isMatchCase)
		{
			return SearchByText(aText, isMatchCase, false, true);
		}
		public TreeNode SearchByText(string aText, bool isMatchCase, bool isWordWhole)
		{
			return SearchByText(aText, isMatchCase, isWordWhole, true);
		}
		public virtual TreeNode SearchByText(string aText, bool isMatchCase, bool isWordWhole, bool isFindForward, TreeNode aBeginFind)
		{
			return this.SearchByText(aText, isMatchCase, isWordWhole, isFindForward);
		}
		public virtual TreeNode SearchByText(string aText, bool isMatchCase, bool isWordWhole, bool isFindForward)
		{
			mFindQnt = 0;
			mFindText = aText;
			mIsMatchCase = isMatchCase;
			mIsWordWhole =isWordWhole;

			int fndQnt = 0;

			if (mFindText.Length>0)
			{
				SelectedNode = RecSearchByText(Nodes, ref fndQnt);
				return SelectedNode;
			}
			else
				return null;
		}

		protected virtual TreeNode SearchByTextStartBack(TreeNode aTN)
		{
			++mFindQnt;
			int fndQnt = mFindQnt;
			TreeNode retNd = RecSearchByText(Nodes, ref fndQnt);
			if (retNd == null)
				mFindQnt--;

			return retNd;
		}

		protected virtual TreeNode SearchByTextStartForward(TreeNode aTN)
		{
			mFindQnt--;
			if (mFindQnt < 0) 
				mFindQnt = 0;
			int fndQnt = mFindQnt;
			return RecSearchByText(Nodes, ref fndQnt);
		}
		
		protected TreeNode RecSearchByText(TreeNodeCollection aTnc, ref int aFindCount)
		{
			TreeNode retNd = null;
			foreach (TreeNode tn in aTnc)
			{
				if (tn.Nodes.Count > 0)
					retNd = RecSearchByText(tn.Nodes, ref aFindCount);
				if (retNd != null)
				{
					if (aFindCount>0)
					{
						aFindCount--;
						retNd = null;
					}
					else
						break;
				}

				if (CompareTextForFind(tn.Text))
				{
					if (aFindCount>0)
						aFindCount--;
					else
					{
						retNd = tn;
						break;
					}
				}
			}
			return retNd;
		}

		protected bool CompareTextForFind (string aText)
		{
			bool ret = false;

			if (mIsMatchCase)
			{ // с учетом регистра букв
				if (mIsWordWhole)
				{ // слово цельком
					ret = aText.Equals(mFindText);
				}
				else
				{ // вхождение слова
					//ret = aText.StartsWith(mFindText);
					ret = (aText.IndexOf(mFindText)>= 0);
				}
			}
			else
			{ // без учета регистра букв
				if (mIsWordWhole)
				{ // слово цельком
					ret = aText.ToUpper().Equals(mFindText.ToUpper());
				}
				else
				{ // вхождение слова
					//ret = aText.ToUpper().StartsWith(mFindText.ToUpper());
					ret = (aText.ToUpper().IndexOf(mFindText.ToUpper())>= 0);
				}
			}

			return ret;
		}
		#endregion
		#region Search by code searching ...
		public TreeNode SearchByCode(int aCode)
		{
			return SearchByCode(Nodes, 0, aCode);
		}
		public TreeNode SearchByCode(int aPlace, int aCode)
		{
			return SearchByCode(Nodes, aPlace, aCode);
		}
		public virtual TreeNode SearchByCode(PlaceCode aPC)
		{
			return SearchByCode(Nodes, aPC.place, aPC.code);
		}
		public TreeNode SearchByCode(TreeNodeCollection aTnc, int aPlace, int aCode)
		{
			TreeNode retNd = null;
			foreach (TreeNode tn in aTnc)
			{
				if (tn.Nodes.Count > 0)
					retNd = SearchByCode(tn.Nodes, aPlace, aCode);
				if (retNd != null)
					break;
				if (CompareByCode(tn, new PlaceCode(aPlace, aCode)))
				{
					retNd = tn;
					break;
				}
			}
			return retNd;
		}

		protected virtual bool CompareByCode(TreeNode aTN, PlaceCode aPC)
		{
			bool ret = false;
			CASTreeItemData tid = aTN.Tag as CASTreeItemData;
			if (tid != null)
				ret = tid.pPC.Equals(aPC);

			return ret;
		}
		#endregion
		#region Реализация рекурсивного вызова по всему загруженному дереву
		/// <summary>
		/// Возвращает набор Nodes для реализации фильтра
		/// </summary>
		/// <param name="aTnc"></param>
		/// <param name="aPlace"></param>
		/// <param name="aCode"></param>
		/// <returns></returns>
		public TreeNodeCollection SearchByNodes(TreeNodeCollection tNodes, string text)
		{
			mIsLocking=true;
			TreeNodeCollection ret=_SearchByNodes(tNodes,text);
			mIsLocking=false;
			return ret;
		}
		/// <summary>
		/// Реализация
		/// </summary>
		/// <param name="tNodes"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		protected TreeNodeCollection _SearchByNodes(TreeNodeCollection tNodes, string text)
		{
			TreeNodeCollection retNd = (new TreeNode()).Nodes;
			foreach (TreeNode tn in tNodes)
			{
				if (tn.Nodes.Count>0)
				{
					//если это текущая коллекция - добавляем
					TreeNodeCollection Nd=_SearchByNodes(tn.Nodes,text);
					if (Nd!=null)
					{
						foreach (TreeNode tnAdd in Nd)
							retNd.Add((TreeNode)tnAdd.Clone());
					}
				}
				else
				{
					//если это текущий узел - добавляем
					if (tn.Text.ToLower().IndexOf(text.ToLower())==0)
						retNd.Add((TreeNode)tn.Clone());
				}      
			}

			if (retNd.Count==0)
				retNd=null;
			return retNd;
		}

		/// <summary>
		/// Возвращает набор Nodes для реализации фильтра
		/// </summary>
		/// <param name="aTnc"></param>
		/// <param name="aPlace"></param>
		/// <param name="aCode"></param>
		/// <returns></returns>
		public TreeNodeCollection SearchByNodes(TreeNodeCollection tNodes, int aPlace, int aCode)
		{
			mIsLocking=true;
			TreeNodeCollection ret=_SearchByNodes(tNodes,aPlace,aCode);
			mIsLocking=false;
			return ret;
		}
		/// <summary>
		/// Реализация
		/// </summary>
		/// <param name="tNodes"></param>
		/// <param name="aPlace"></param>
		/// <param name="aCode"></param>
		/// <returns></returns>
		protected TreeNodeCollection _SearchByNodes(TreeNodeCollection tNodes, int aPlace, int aCode)
		{
			TreeNodeCollection retNd = (new TreeNode()).Nodes;
			foreach (TreeNode tn in tNodes)
			{      
				if (tn.Nodes.Count>0)
				{
					//если это текущая коллекция - добавляем
					TreeNodeCollection Nd=_SearchByNodes(tn.Nodes,aPlace,aCode);
					if (Nd!=null)
					{
						foreach (TreeNode tnAdd in Nd)
							retNd.Add((TreeNode)tnAdd.Clone());
					}
				}
				else
				{
					TreeItemData td=tn.Tag as TreeItemData;
					//если это текущий узел - добавляем
					if (td!=null && td.pPlace==aPlace && td.pCode==aCode)
						retNd.Add((TreeNode)tn.Clone());
				}
			}
			if (retNd.Count==0)
				retNd=null;
			return retNd;

		}

		#endregion
		#endregion Search ...

		protected override void OnPaint(PaintEventArgs e)
		{
			if (!mIsLocking)
				base.OnPaint(e);
		}

		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			if (!mIsLocking && this.SelectedNode != null)
			{	/// запоминаем предыдущий выбранный узел
				_prevTreeNodeSelected=this.SelectedNode;
			}
			base.OnBeforeSelect(e);
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			if (!mIsLocking)
			{
				base.OnAfterSelect(e);
			}
		}

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
			{
				OnFind(this, new EventArgs());
			}
			else if (e.KeyCode == Keys.F3)
			{
				TreeNode tn = null;
				if (e.Shift)
					tn = SearchByTextStartBack(SelectedNode);
				else
					tn = SearchByTextStartForward(SelectedNode);

				if (tn == null)
				{
					MessageBox.Show ("Искомый текст ["+mFindText+"] больше не найден!");
					RestorePrevSelectNode();
				}
				else
				{
					SelectedNode = tn;
					SelectedNode.NodeFont = mFoundItemFont;
					_prevTreeNodeSelected = tn;
				}
				SelectedNode.EnsureVisible();	
			}
			else if (e.KeyCode == Keys.Space)
			{
				OnChoice(this, new EventArgs());
			}
      else if (e.KeyCode == Keys.F2)
      {
        if (onDoCommand != null)
          onDoCommand(this, new EvA_CasTVCommand(SelectedNode, eCommand.Edit)); 
      }
      else if (e.KeyCode == Keys.Insert)
      {
        if (onDoCommand != null)
          onDoCommand(this, new EvA_CasTVCommand(SelectedNode, eCommand.Add));
      }
      else if (e.KeyCode == Keys.Delete)
      {
        if (onDoCommand != null)
          onDoCommand(this, new EvA_CasTVCommand(SelectedNode, eCommand.Delete));
      }
    }

		private void OnBeforeLabelEdit (object sender, NodeLabelEditEventArgs e)
		{
			if (mMIDel != null)
				mMIDel.Shortcut = Shortcut.None; 
			if (mMIIns != null)
				mMIIns.Shortcut = Shortcut.None; 
		}

		private void OnAfterLabelEdit (object sender, NodeLabelEditEventArgs e)
		{
			if (mMIDel != null)
				mMIDel.Shortcut = Shortcut.Del; 
			if (mMIIns != null)
				mMIIns.Shortcut = Shortcut.Ins; 
		}

		private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			TreeNode tn = GetNodeAt(e.X, e.Y);
			if (tn != null)
			{
				if (e.Button == MouseButtons.Right)
				{
					if (onAfterSelectRight != null)
						onAfterSelectRight(this, new EvA_CasTV(tn));
				}
        SelectedNode = tn;
			}
		}

		private void OnDoubleClickTreeItem(object sender, System.EventArgs e)
		{
			if ((SelectedNode != null && SelectedNode.Nodes.Count == 0) || pIsMaySelectedNode)
				DoCommand(SelectedNode, eCommand.Choice);
		}

		#region DoCommandXXX ...
		public virtual void DoCommand(TreeNode aTn, eCommand aCmd)
		{
			if (aCmd == eCommand.Find && !pUseLockFind) //если можно искать
			{
				dlgFind dlg = new dlgFind();
				dlg.StartPosition = FormStartPosition.CenterScreen;
				dlg.pFindText = mFindText;
				dlg.pIsMatchCase = mIsMatchCase;
				dlg.pIsWordWhole = mIsWordWhole;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					this.Parent.Refresh();
					TreeNode tn = null;
					if (dlg.pIsDirectionForward)
						tn = SearchByText(dlg.pFindText, dlg.pIsMatchCase, dlg.pIsWordWhole, true);
					else
						tn = SearchByText(dlg.pFindText, dlg.pIsMatchCase, dlg.pIsWordWhole, false);
					if (tn == null)
					{
						MessageBox.Show ("Искомый текст ["+mFindText+"] не найден!");
						RestorePrevSelectNode();
					}
					else
					{
						SelectedNode = tn;
						SelectedNode.NodeFont = mFoundItemFont;
						_prevTreeNodeSelected = tn;
					}
					SelectedNode.EnsureVisible();	
				}
			}
			else if (onDoCommand != null)
				onDoCommand(this, new EvA_CasTVCommand(aTn, aCmd)); 
		}

		protected void OnChoice (object sender, System.EventArgs e)
		{
			if (SelectedNode != null && (SelectedNode.Nodes.Count == 0 || pIsMaySelectedNode))
				DoCommand(SelectedNode, eCommand.Choice);
		}
		protected void OnDelete (object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Delete);
		}
		protected void OnRename (object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Edit);
		}
		protected void OnProperty (object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Property);
		}
		protected void OnFind(object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Find);
		}

		#endregion

		public void AddMenuItem(eCommand aCmd)
		{
			if (ContextMenu == null)
				return;

			switch (aCmd)
			{
				case eCommand.Choice:
					mMIChoice = new MenuItem("Выбрать", new EventHandler(OnChoice),Shortcut.CtrlIns);
					ContextMenu.MenuItems.Add(mMIChoice);
					break;
				case eCommand.Delete:
					mMIDel = new MenuItem("Удалить", new EventHandler(OnDelete),Shortcut.Del);
					ContextMenu.MenuItems.Add(mMIDel);
					break;
				case eCommand.Edit:
					mMIRename = new MenuItem("Переименовать", new EventHandler(OnRename),Shortcut.F2);
					ContextMenu.MenuItems.Add(mMIRename);
					break;
				case eCommand.Property:
					mMIProp = new MenuItem("Свойства", new EventHandler(OnProperty),Shortcut.ShiftF2);
					ContextMenu.MenuItems.Add(mMIProp);
					break;
				case eCommand.Find:
					mMIFind = new MenuItem("Найти", new EventHandler(OnFind),Shortcut.CtrlF);
					ContextMenu.MenuItems.Add(mMIFind);
					break;
			}
		}

		public void AddMenuDelimiter()
		{
			if (ContextMenu != null)
				ContextMenu.MenuItems.Add(new MenuItem("-"));
		}

		/// <summary>
		/// Находит в коллекции Node-ов элемент по паре Place:Code в таге.
		/// </summary>
		/// <param name="aNodeColl">коллекция Node-ов в которой осуществляется поиск</param>
		/// <param name="aPlace">place</param>
		/// <param name="aCode">код</param>
		/// <returns>найденный элемент коллекции, если найден, иначе null</returns>
		public TreeNode FindNodeByCode (TreeNodeCollection aNodeColl, PlaceCode aPC) 
		{
			CASTreeItemData tid = null;
			foreach(TreeNode tn in aNodeColl)
			{
				tid = tn.Tag as CASTreeItemData;
				if (tid != null)
				{
					if (tid.pPlace == 0 && tid.pCode == aPC.code)
						return tn;
					else if (tid.pPC.Equals(aPC))
						return tn;
				}
			}

			return null;
		}
		public void RestorePrevSelectNode()
		{
			if (_prevTreeNodeSelected!=null) // && _prevTreeNodeSelected.TreeView!=null) //т.е. узел есть в дереве !!!
				this.SelectedNode=_prevTreeNodeSelected;
		}

		#region Поддержка интерфейса для навигации по истории состояний ...
		public object State
		{
			get
			{
				return this.SelectedNode;
			}
			set
			{
				if (value != null)
				{
					TreeNode tn = value as TreeNode;
					if (tn != null)
					{
						if (tn.IsVisible)
						{
							SelectedNode = tn;
							SelectedNode.EnsureVisible();
						}
						else
						{
							CASTreeItemData tid = tn.Tag as CASTreeItemData;
							if (tid != null)
								Load(tid.pPC, false);
						}
					}
				}
			}
		}

		public bool IsEqual (object aState1, object aState2)
		{
			TreeNode tn1 = aState1 as TreeNode;
			TreeNode tn2 = aState2 as TreeNode;
			bool ret = false;
			if (tn1 != null && tn2 != null)
			{
				CASTreeItemData tid1 = tn1.Tag as CASTreeItemData;
				CASTreeItemData tid2 = tn2.Tag as CASTreeItemData;
				if (tid1 != null && tid2 != null)
					ret = tid1.pPC.Equals(tid2.pPC);
			}
			else if (tn1 == null && tn2 == null)
				ret = true;
			
			return ret;
		}
		#endregion

		#region Реализация Drag & Drop

		//начало DragDrop (над каким узлом) - запоминаем
		private void this_DragEnter(object sender, DragEventArgs e)
		{
			//запоминаем ?
			e.Data.SetData(this.Name,this.Tag as TreeNode);
		}

		//делаем ЭТО - сюда ?
		private void this_DragOver(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(this.Name))
			{
				e.Effect=DragDropEffects.None;
				return;
			}

			TreeNode Source=e.Data.GetData(this.Name) as TreeNode;
			TreeItemData tvSource=Source.Tag as TreeItemData;

			TreeNode Destination= this.GetNodeAt(this.PointToClient(new Point(e.X,e.Y)));
			TreeItemData tvDest=Destination.Tag as TreeItemData;

			//что-то не так !!!
			if (Source==null || tvSource==null || Destination==null || tvDest==null)
			{
				e.Effect=DragDropEffects.None;
				return;
			}
			//чтобы не было повторов на том же уровне
			if (TreeNode.Equals(Source,Destination)
				//TreeNode.Equals(Source,Destination.Parent) || 
				|| TreeNode.Equals(Source.Parent,Destination)
				//||TreeItemData.Equals(Source,Destination)
				)
			{
				e.Effect=DragDropEffects.None;
				return;
			}
			TreeItemData root= this.Root(Destination).Tag as TreeItemData;
			//все возможности только на своей иерархии объектов
			if (root==null)
			{
				e.Effect=DragDropEffects.None;
				return;
			}
			if (onMyDragOver!=null)
				onMyDragOver(Source,Destination,e);
			myDragOver(Source,Destination,e);
		}
		//запускаем процесс Drag & Drop
		private void this_ItemDrag(object sender, ItemDragEventArgs e)
		{
			TreeNode nodeDrag=e.Item as TreeNode;
			if (nodeDrag==null)
				return;
			this.Tag =nodeDrag; //запоминаем
			/*
			if (e.Button==MouseButtons.Left)
			{
				IBNodeItemData td=nodeDrag.Tag as IBNodeItemData;
				//группы копируются всегда
				if (td.pObjTypeR==RBC.OBJECT_TYPE_GROUP)
					this.DoDragDrop(nodeDrag, DragDropEffects.Copy);
				else
					this.DoDragDrop(nodeDrag, DragDropEffects.Link);
			}
			else if(e.Button==MouseButtons.Right)
				this.DoDragDrop(nodeDrag, DragDropEffects.Move);
				*/
			this.DoDragDrop(nodeDrag, DragDropEffects.Move);
		}

		//запрашиваем продолжение
		private void this_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			/*
			else if(td.pObjTypeR!=RBC.OBJECT_TYPE_GROUP)
			{
				e.Action=DragAction.Cancel;
				return;
			}
			*/
		}

		private void this_DragDrop(object sender, DragEventArgs e)
		{
			TreeNode Source=e.Data.GetData(this.Name) as TreeNode;
			TreeNode Destination= this.GetNodeAt(this.PointToClient(new Point(e.X,e.Y)));
			//имеет смысл только если реализуется клиентом MyDragRun !!!
			if (onMyDragRun!=null)
			{
				onMyDragRun(Source,Destination,e);
				myDragRun(Source,Destination,e);
			}
		}
		
		#region Virtual functons
		//можно что-то проверить !!!
		public virtual void myDragOver(TreeNode Source, TreeNode Destination,DragEventArgs e)
		{
			//ничего не делаем !!!
			if (e.Effect==DragDropEffects.None)
				return ;
			e.Effect=e.AllowedEffect;
		}
		//реализация самого действия
		public virtual void myDragRun(TreeNode Source, TreeNode Destination,DragEventArgs e)
		{
			//ничего не делаем - ошибка!!!
			if (e.Effect==DragDropEffects.None)
				return ;
			//выполняем только видимые изменения !!
			if(e.AllowedEffect==DragDropEffects.Move)
			{
				Source.Remove();
			}
			int indNode=Destination.Nodes.Add(Source.Clone() as TreeNode);
			Destination.Nodes[indNode].EnsureVisible();
			this.Tag=null;
		}

		#endregion --Virtual functons

		#endregion --Реализация Drag & Drop
		
		public virtual object Clone()
		{
			CASTreeViewBase ct=(CASTreeViewBase)base.MemberwiseClone();
      if (mMIDel != null)
			  ct.mMIDel=mMIDel.CloneMenu();
      if (mMIIns != null)
			  ct.mMIIns=mMIIns.CloneMenu();
			ct._prevTreeNodeSelected=_prevTreeNodeSelected;
			ct.mIsLocking=mIsLocking;
			ct.mICol=mICol;
			ct.pIsMaySelectedNode = pIsMaySelectedNode;
      if (mFindText != null)
			  ct.mFindText=(string)mFindText.Clone();
			ct.mIsMatchCase=mIsMatchCase;
			ct.mIsWordWhole=mIsWordWhole;
			ct.mFindQnt=mFindQnt;
			ct.pErr=(Error)pErr.Clone();

			ct.pEmptyItemText=(string)pEmptyItemText.Clone();
			ct.pPathDelim =(string)pPathDelim.Clone();
			return ct;
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public abstract class CASTreeViewStep : CASTreeViewBase
	{
		private TreeNode					_tnBeginFindParent;
		protected bool						mIsLockExpand;


    /// <summary>
    /// Для поддержания множества "корней" в дереве
    /// 04.12.2006 by M.Tor
    /// </summary>
    private int _currRootInd;
    protected ArrayList mArrRoots;

    /// <summary>
    ///  для совместимости с пред. версиями
    /// </summary>
    public int pRootItemPlace
    {
      get { return ((PlaceCode)mArrRoots[_currRootInd]).place; }
      set 
      {
        PlaceCode pc = (PlaceCode)mArrRoots[_currRootInd];
        if (pc != null ) pc.place = value; 
      }
    }
    public int pRootItemCode
    {
      get { return ((PlaceCode)mArrRoots[_currRootInd]).code;  }
      set
      {
        PlaceCode pc = (PlaceCode)mArrRoots[_currRootInd];
        if (pc != null) pc.code = value;
      }

    }
		public PlaceCode					pPCRoot
		{
			get { return (PlaceCode)mArrRoots[_currRootInd]; }
			set { mArrRoots[_currRootInd] = value; }
		}

		//  добавлено dsy 03.08.2005
		/// <summary>
		/// Видимость корня некоторой ветки дерева, с которого дерево должно грузиться.
		/// </summary>
		public bool								pIsRootVisible;

    public CASTreeViewStep()
		{
			_tnBeginFindParent	= null;
			//pRootItemCode				= 0;
			mIsLockExpand				= false;
			pIsRootVisible			= true;

      mArrRoots = new ArrayList(4);
      _currRootInd = 0;
      mInitArrRoots();
		}

    protected virtual void mInitArrRoots()
    {
      mArrRoots.Add(PlaceCode.Empty);
    }

    public int AddRoot(PlaceCode aPC)
    {
      return mArrRoots.Add(aPC);
      /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
      /// продумать навигацию по _currRootInd, если будет нужно в дальнейшем
      /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    public void ClearRootsCollection()
    {
      mArrRoots.Clear();
    }

		/// <summary>
		/// Абстрактная функция которую нужно ОБЯЗАТЕЛЬНО! определить в производном классе.
		/// Загружает потомков узла [aTn].
		/// </summary>
		protected abstract void BeforeExpandNode(TreeNode aTn);

		/// <summary>
		/// Загрузка дерева с корневого элемента.
		/// </summary>
		/// <param name="aExpandLevel">true - раскрывать следующий уровень корневого элемента;
		/// false - нет</param>
		public override void Load(bool aExpandLevel)
		{
			Nodes.Clear();
			SelectedNode = null;
			BeforeExpandNode (null);
			//AddNextLevel(null);
			//foreach(TreeNode tn in Nodes)
			//	AddNextLevel(tn); 
			if (Nodes.Count>0)
			{
				SelectedNode = Nodes[0];
				if (aExpandLevel)
					Nodes[0].Expand();
			}
		}

		/// <summary>
		/// Загрузка дерева и установка на текущем элементе по пути.
		/// </summary>
		/// <param name="aItemPath">путь</param>
		/// <param name="aItemPlace">place</param>
		public void Load(string aItemPath, int aItemPlace)
		{
			Load(aItemPath, aItemPlace, false);
		}

		/// <summary>
		/// Загрузка дерева и установка на текущем элементе по пути.
		/// </summary>
		/// <param name="aItemPath">путь</param>
		/// <param name="aItemPlace">place</param>
		/// <param name="aExpandLevel">true - раскрывать следующий уровень от текущего элемента;
		/// false - нет</param>
		/// <summary>
		public virtual void Load(string aItemPath, int aItemPlace, bool aExpandLevel)
		{
			if (aItemPath == null || aItemPath.Length == 0) 
			{
				Load(true);
				pErr.text = "path empty";
				return;
			}

			int ii = 0;
			BeginUpdate();
      Nodes.Clear();
			Load(); 
			string[] ss = aItemPath.Split(pPathDelim.ToCharArray());

      #region added by DSY 27.08.2008
      //  если корневых узлов больше 1
      bool isFound = false;
      for (int i = 0; i < mArrRoots.Count; i++)
      {
        ii = 0;
        //_currRootInd += i; - by M.Tor (21.09.2009) - почему-то было так !?!?!?
        _currRootInd = i;
        // сначало пропустим все до root-a
        if (pRootItemCode > 0)
          for (; ii < ss.Length; ii++)
            if (pRootItemCode == Convert.ToInt32(ss[ii]))
            {
              if (!pIsRootVisible && mArrRoots.Count < 2)
                ii++;
              isFound = true;
              break;
            }
        if (isFound)
          break;
      }
      #endregion

      TreeNode tn = null;
      if (ii < ss.Length)
        tn = FindNodeByCode(Nodes, new PlaceCode(aItemPlace, CASTools.ConvertToInt32Or0(ss[ii])));
      try
      {
        if (tn != null)
        {
          //tn.Expand();
          OnBeforeExpand(new TreeViewCancelEventArgs(tn, false, TreeViewAction.Expand));
          for (ii++; ii < ss.Length && tn != null; ii++)
          {
            tn = FindNodeByCode(tn.Nodes, new PlaceCode(aItemPlace, CASTools.ConvertToInt32Or0(ss[ii])));
            if (tn != null && tn.Nodes.Count > 0) // - можно и без этого - нет не получается
            {
              //tn.Expand();
              OnBeforeExpand(new TreeViewCancelEventArgs(tn, false, TreeViewAction.Expand));
            }
          }
        }

        if (tn == null)
          Load(false);
        else
        {
          mIsLockExpand = true;
          SelectedNode = tn;
          if (aExpandLevel)
            SelectedNode.Expand();
          mIsLockExpand = false;
        }
      }
#if DEBUG
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
#else
      catch {}
#endif
			EndUpdate();
		}

		public void LoadLevel(TreeNode aTN)
		{
			mIsLocking = true;
			BeginUpdate();

			BeforeExpandNode(aTN);

			EndUpdate();
			mIsLocking = false;
		}

		public void ReloadLevel ()
		{
			ReloadLevel (SelectedNode);
		}

		public void ReloadLevel (TreeNode aCurrentTN)
		{
			if (aCurrentTN==null)
			{
				Load(true);
				return ;
			}
			TreeNode ptn = aCurrentTN.Parent;
			if (ptn==null)
			{
				//самый верхний уровень
				Load(aCurrentTN.IsExpanded);
				return ;
			}
			//узнаём состояние
			bool expandThisNode=aCurrentTN.IsExpanded;

			//Изменил Димон 11.09.06 [PlaceCode pc = ((TreeItemData)aCurrentTN.Tag).pPC;]
			//для совместимости с PCTVItemData
			PlaceCode pc = ((CASTreeItemData)aCurrentTN.Tag).pPC;
			mIsLocking = true;
			BeginUpdate();

			BeforeExpandNode(ptn);
			//AddNextLevel(ptn);
			//foreach (TreeNode itn in ptn.Nodes)
			//	AddNextLevel(itn);
			TreeNode tn = FindNodeByCode(ptn.Nodes, pc);
			if (tn != null)
				SelectedNode = tn;
			else
				SelectedNode = ptn;

			EndUpdate();
			//восстанавливаем !
			if (expandThisNode)
				SelectedNode.Expand();
			mIsLocking = false;
		}


		/*protected virtual void AddForNode(TreeNode aTn)
		{
			foreach (TreeNode tn in aTn.Nodes)
				AddNextLevel(tn);
		}*/

		protected override void OnBeforeExpand(TreeViewCancelEventArgs tvcea)
		{
			base.OnBeforeExpand(tvcea);
			//AddForNode(tvcea.Node);
			if (!mIsLockExpand)
			{
				mIsLockExpand = true;
				try{BeforeExpandNode(tvcea.Node);}
				catch{}
				finally{mIsLockExpand = false;}
			}
		}	

		#region Search ...
		public override TreeNode SearchByText(string aText, bool isMatchCase, bool isWordWhole, bool isFindForward)
		{
			if (SelectedNode == null)
				return null;

			return SearchByText(aText, isMatchCase, isWordWhole, isFindForward, SelectedNode);
		}
		public override TreeNode SearchByText(
			string aText, 
			bool isMatchCase, 
			bool isWordWhole, 
			bool isFindForward,
			TreeNode aBeginFind)
		{
			mFindText = aText;  
			mIsMatchCase = isMatchCase;
			mIsWordWhole = isWordWhole;

			if (aBeginFind == null)
			{ // то, ищем с начала
				if(this.Nodes.Count > 0)
					aBeginFind = this.Nodes[0];
				_tnBeginFindParent = null;
			}
			else
			{
				_tnBeginFindParent = aBeginFind.Parent;
			}

			if (isFindForward)
				return SearchByTextStartForward(aBeginFind);
			else
				return SearchByTextStartBack(aBeginFind);
		}

		protected override TreeNode SearchByTextStartForward(TreeNode aTN)
		{
			TreeNode retTN = null;
			if (mFindText.Length>0 && aTN != null)
			{
				mIsLocking = true;

				BeginUpdate();
				//UpdateLock(true);
				//this
				try
				{
					if (CompareTextForFind(aTN.Text))
					{
						retTN = aTN;
					}
					else
					{
						do
						{
							retTN = SearchByTextForward(aTN);
							if (retTN != null)
							{
								// метим всех предков чтоб не закрыть после
								for (TreeNode tt = retTN.Parent; tt != null;)
								{
									((CASTreeItemData)tt.Tag).pTemp = 1;
									tt = tt.Parent;
								}
								break;
							}
							else
								aTN.Collapse(); // закрываем узлы где нет искомого элемента

							if (aTN.NextNode != null)
							{
								aTN = aTN.NextNode;
							}
							else
							{
								do
								{
									aTN = aTN.Parent;
									if (aTN != null)
									{
										if (((CASTreeItemData)aTN.Tag).pTemp == 0)
											aTN.Collapse(); // закрываем узлы где нет искомого элемента

										if (aTN == _tnBeginFindParent)
											break;
										if (aTN.NextNode != null)
										{
											aTN = aTN.NextNode;
											break;
										}
									}
								} 
								while (aTN != null); //_tnBeginFindParent);
							}
							if (aTN != null && CompareTextForFind(aTN.Text))
							{
								retTN = aTN;
								break;
							}
						}
						while (aTN != _tnBeginFindParent && aTN != null);
					}
				}
				finally
				{
					//UpdateLock(false);
					EndUpdate();
					//ResumeLayout(false);
					mIsLocking = false;
				}
			}
			return retTN;
		}

		protected override TreeNode SearchByTextStartBack(TreeNode aTN)
		{
			TreeNode retTN = null;
			if (mFindText.Length>0)
			{
				mIsLocking = true;
				//SuspendLayout();
				//UpdateLock(true);
				BeginUpdate();
				//this
				try
				{
					//TreeNode currParent = null;
					do
					{
						/// .
						if (aTN.PrevNode != null)
						{
							aTN = aTN.PrevNode;
							// ищем ниже, только если перешли к старшему "брату"
							retTN = SearchByTextBack(aTN);
							if (retTN != null)
							{
								// метим всех предков чтоб не закрыть после
								for (TreeNode tt = retTN.Parent; tt != null;)
								{
									((CASTreeItemData)tt.Tag).pTemp = 1;
									tt = tt.Parent;
								}
								break;
							}
							else
								aTN.Collapse(); // закрываем узлы где нет искомого элемента
						}
						else
						{
							do
							{
								aTN = aTN.Parent;
								if (aTN != null)
								{
									if (((CASTreeItemData)aTN.Tag).pTemp == 0)
										aTN.Collapse(); // закрываем узлы где нет искомого элемента

									if (aTN == _tnBeginFindParent)
										break;
									if (aTN.PrevNode != null)
										break;
								}
							} 
							while (aTN != null); //_tnBeginFindParent);
						}


						/// .
						if (aTN != null && CompareTextForFind(aTN.Text))
						{
							retTN = aTN;
							break;
						}

					}
					while (aTN != _tnBeginFindParent && aTN != null);
				}
				finally
				{
					EndUpdate();
					//UpdateLock(false);
					//ResumeLayout(false);
					mIsLocking = false;
				}
			}
			return retTN;
		}

		private TreeNode SearchByTextForward(TreeNode aTN)
		{
			TreeNode retNd = null;
			foreach (TreeNode tn in aTN.Nodes)
			{
				if (CompareTextForFind(tn.Text))
				{
					retNd = tn;
					break;
				}
				SelectedNode = tn;
				if (tn.Nodes.Count > 0) 
				{
					retNd = SearchByTextForward(tn);
					if (retNd != null)
						break;
					else
						tn.Collapse();
				}
			}

			return retNd;
		}

		private TreeNode SearchByTextBack(TreeNode aTN)
		{
			TreeNode retNd = null;
			for (TreeNode tn = aTN.LastNode; tn != null; )
			{
				SelectedNode = tn;
				if (tn.Nodes.Count > 0) 
				{
					retNd = SearchByTextBack(tn);
					if (retNd != null)
						break;
					else
						tn.Collapse();
				}

				if (CompareTextForFind(tn.Text))
				{
					retNd = tn;
					break;
				}
				tn = tn.PrevNode;
			}

			return retNd;
		}
		#endregion search.
	}


}
