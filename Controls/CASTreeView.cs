using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb;
using CommandAS.Tools;
using CommandAS.Tools.Forms;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// Тип элемента иерархии.
	/// </summary>
	public enum eTreeItemType 
	{
		/// <summary>
		/// не пределен
		/// </summary>
		Undefined,
		/// <summary>
		/// узел, может содержать подчиненые элементы ("детей")
		/// </summary>
		Node,
		/// <summary>
		/// конечный элемент - лист
		/// </summary>
		Item,
		/// <summary>
		/// пустой элемент
		/// </summary>
		Empty
	};

	/// <summary>
	/// Данные хранящиеся в таге (TreeNode.Tag)
	/// каждого элемента дерева (TreeView).
	/// </summary>
	public class CASTreeItemData
	{
		public PlaceCode									pPC;

		public int												pPlace
		{
			get { return pPC.place;  }
			set	{ pPC.place = value; }
		}
		public int												pCode
		{
			get { return pPC.code;  }
			set	{ pPC.code = value; }
		}
		public int												pTemp;

		public CASTreeItemData() : this (PlaceCode.Empty){}
		public CASTreeItemData(int aPlace, int aCode) : this (new PlaceCode(aPlace, aCode)) {}
		public CASTreeItemData(PlaceCode aPC)
		{
			pPC			= aPC;
			pTemp		= 0;
		}
	}
	/// <summary>
	/// Данные хранящиеся в таге (TreeNode.Tag)
	/// каждого элемента дерева (TreeView).
	/// </summary>
	public class TreeItemData : CASTreeItemData
	{
		public string						pPath;
		public eTreeItemType		pItemType; //eTreeItemType

		public TreeItemData(): this(0, 0)
		{
		}
		public TreeItemData(int aPlace, int aCode): base(aPlace, aCode)
		{
			pPath		= string.Empty;
			pItemType	= eTreeItemType.Empty;
		}
	}

	#region Event delegate:
	public delegate void EvH_CasTV(object sender, EvA_CasTV e);
	public delegate void EvH_CasTVCommand(object sender, EvA_CasTVCommand e);
	public delegate void CasTVLoadByCodeEHandler(object sender, CasTVLoadByCodeArgs e);
	public delegate void CasTVReadEventHandler(object sender, CasTVReadEArgs e);

	public class EvA_CasTV: TreeViewEventArgs
	{
		public EvA_CasTV(TreeNode aTn): this(aTn, TreeViewAction.Unknown)
		{
		}
		public EvA_CasTV(TreeNode aTn, TreeViewAction aAction): base(aTn, aAction)
		{
		}
	}
	public class EvA_CasTVCommand: TreeViewEventArgs
	{
		private eCommand	mCommand;
		public	eCommand	pCommand
		{
			get { return mCommand; }
		}

		public EvA_CasTVCommand(TreeNode aTn, eCommand aCmd): base(aTn)
		{
			mCommand = aCmd;
		}
	}
	public class CasTVLoadByCodeArgs: EventArgs
	{	
		private int			mPlace;
		private int			mCode;

		public string		pPath;
		public int			pPlace
		{ get { return mPlace; } }
		public int			pCode
		{ get { return mCode; } }

		public CasTVLoadByCodeArgs(int aPlace, int aCode)
		{
			mPlace = aPlace;
			mCode  = aCode;
		}
	}
	public class CasTVReadEArgs: TreeViewEventArgs
	{
		private OleDbDataReader mDr;
		public OleDbDataReader pDataReader
		{
			get {return mDr;}
		}
		public CasTVReadEArgs(TreeNode aTn, OleDbDataReader aDr): base(aTn)
		{
			mDr=aDr;
		}
	}
	#endregion

	/// <summary>
	/// 
	/// </summary>
	public class CASTreeViewCommon : System.Windows.Forms.TreeView, INavClient
	{
		#region PROPERTY
		private MenuItem									mMIDel;
		private MenuItem									mMIIns;
		private TreeNode                  mTreeViewNode; //предыдущий выбранный узел

		/// <summary>
		/// Текст для пустого элемента
		/// </summary>
		protected const string EMPTY_ITEM_TEXT  = "<пусто>";

		protected bool										mIsLocking;
		protected eCommand								mMenuCommandSet;
		/*
				protected eCommand                mMenuCommandSet
				{
					get
					{
						return _MenuCommandSet;
					}
					set
					{
						if (mMenuCommandSet!=value)
						{
							mMenuCommandSet=value;
							//вызываем событие изменения контекстного меню
							if (!mIsLocking)
								OnDisplayContextMenu(this,EventArgs.Empty);
						}
					}
				}
		*/
		protected bool										mIsMayEdit;
		protected IconCollection					mCIC;
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
			get { return new Font(this.Font.FontFamily.Name, this.Font.Size, FontStyle.Bold); }
		}

		public Error											pErr; 
		public string											pEmptyItemText = EMPTY_ITEM_TEXT;
		public string											pPathDelim = "~";
		public bool												pIsMayEdit
		{
			get { return mIsMayEdit;}
			set 
			{
				mIsMayEdit = value;
				if (value)
				{
					pEmptyItemText = EMPTY_ITEM_TEXT;
					// установим соответствующие биты
					mMenuCommandSet |= eCommand.Add|eCommand.Edit|eCommand.Delete|eCommand.Property;
				}
				else
				{
					pEmptyItemText = string.Empty;
					// сбросим соответствующие биты
					mMenuCommandSet &= ~(eCommand.Add|eCommand.Edit|eCommand.Delete|eCommand.Property);
				}
			}
		}
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
		
		public IconCollection				pIconCollection
		{
			set 
			{ 
				mCIC = value;
				if (value != null)
					ImageList = value.pImageList;
			}
			get { return mCIC; }
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

		//protected StateHistoryNavigateManager	mSHNM;
		//public bool												pIsSaveStateHistory
		//{
		//	get { return (mSHNM != null);}
		//	set
		//	{
		//		if (value)
		//		{
		//			mSHNM = new StateHistoryNavigateManager();
		//		}
		//		else
		//			mSHNM = null;
		//	}
		//}
		//protected bool										mIsMayStorySH;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion property.

		public event EvH_CasTV								OnNewTreeItemForNode;
		public event EvH_CasTVCommand					OnDoCommand;
		public event EvH_CasTV								AfterSelectRight;
		
		public CASTreeViewCommon(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();
		}

		public CASTreeViewCommon()
		{
			mCIC = null;
			//mSHNM = null;
			//mIsMayStorySH = true;

			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
		}

		/// <summary>
		/// Определяет уровень в иерархии (начиная с 1).
		/// </summary>
		/// <param name="aNd">элемент иерархии (<see cref="System.Windows.Forms.TreeNode"/>) для которого определяется уровень</param>
		/// <returns>уровень начиная с 1 (для корня)</returns>
		public int Level(TreeNode aNd)
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
		public TreeItemData RootData  (TreeNode aNd)
		{
			TreeItemData ret = null; 
			if (aNd != null)
			{
				while (aNd.Parent != null)
					aNd = aNd.Parent;
				ret = (TreeItemData)aNd.Tag;
			}
			 
			return ret; 
		}

		
		#region манипуляции с меню

		public void AddMenuCommand (eCommand aCmd)
		{
			mMenuCommandSet |= aCmd;
		}

		public void RemoveMenuCommand (eCommand aCmd)
		{
			mMenuCommandSet &= ~aCmd;
		}

		public void ClearMenuCommand()
		{
			mMenuCommandSet = eCommand.None;
		}

		#endregion

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
		public virtual void Load(int aPlace, int aCode, bool aExpandLevel)
		{
			SelectedNode = SearchByCode(aPlace, aCode);
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
					for (int ii=1; ii < ss.Length; ii++)
					{
						tn = tn.Nodes[Convert.ToInt32(ss[ii])];
						SelectedNode = tn;
					}
					if (aExpandLevel && SelectedNode != null)
						SelectedNode.Expand(); 
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			EndUpdate();
		}
		#endregion

		/// <summary>
		/// Находит в коллекции Node-ов элемент по паре Place:Code в таге.
		/// </summary>
		/// <param name="aNodeColl">коллекция Node-ов в которой осуществляется поиск</param>
		/// <param name="aPlace">place</param>
		/// <param name="aCode">код</param>
		/// <returns>найденный элемент коллекции, если найден, иначе null</returns>
		public TreeNode FindNodeByCode (TreeNodeCollection aNodeColl, int aPlace, int aCode) 
		{
			CASTreeItemData tid = null;
			foreach(TreeNode tn in aNodeColl)
			{
				tid = tn.Tag as CASTreeItemData;
				if (tid != null && tid.pPlace == aPlace && tid.pCode == aCode)
					return tn;
			}

			return null;
		}
		
		protected virtual void AddNextLevel(TreeNode tn)
		{
		}

		/// <summary>
		/// Виртуальный метод добавляет новую структуру CASTreeItemData в
		/// таг TreeNode. Может быть переопределен в производном классе,
		/// если нужно добавить другую структуру, обязательно производную
		/// от CASTreeItemData.
		/// </summary>
		/// <param name="aNode">элемент дерева TreeNode</param>
		protected virtual void NewTreeItemForNode (TreeNode aTn) 
		{
			if (OnNewTreeItemForNode != null)
				OnNewTreeItemForNode(this, new EvA_CasTV(aTn));
		}


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
			return ((CASTreeItemData)aTN.Tag).pPC.Equals(aPC);
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
				/// запоминаем предыдущий выбранный узел
				mTreeViewNode=this.SelectedNode;
			base.OnBeforeSelect(e);
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			if (!mIsLocking)
			{
				base.OnAfterSelect(e);
				//if (mSHNM != null && mIsMayStorySH)
				//	mSHNM.pCurrentState = e.Node;
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
					mTreeViewNode = tn;
				}
				SelectedNode.EnsureVisible();	
			}
			else if (e.KeyCode == Keys.Space)
			{
				OnChoice(this, new EventArgs());
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

		private void OnDisplayContextMenu (object sender, System.EventArgs e)
		{
			if (this.ContextMenu == null)
				return;
			if (sender==null)
				return;

			this.ContextMenu.MenuItems.Clear();
			mMIDel = null;
			mMIIns = null;

			if (SelectedNode != null)
			{
				CASTreeItemData tid = (CASTreeItemData)SelectedNode.Tag;
				if ((int)(mMenuCommandSet&eCommand.Choice)>0 && tid.pCode>0)
					this.ContextMenu.MenuItems.Add(new MenuItem("Выбрать", new EventHandler(OnChoice),Shortcut.CtrlIns));
			}

			string addText = "Добавить";

			if ((int)(mMenuCommandSet&eCommand.AddFolder)>0)
			{
				if (ContextMenu.MenuItems.Count > 0)
					this.ContextMenu.MenuItems.Add(new MenuItem("-"));
				this.ContextMenu.MenuItems.Add(new MenuItem("Добавить раздел", new EventHandler(OnNewFolder),Shortcut.None));
				addText = "Добавить элемент";
			}
			if ((int)(mMenuCommandSet&eCommand.Add)>0)
			{
				if (ContextMenu.MenuItems.Count > 0 && addText.Equals("Добавить"))
					this.ContextMenu.MenuItems.Add(new MenuItem("-"));
				mMIIns = new MenuItem(addText, new EventHandler(OnNew),Shortcut.Ins); 
				this.ContextMenu.MenuItems.Add(mMIIns);
			}

			if (SelectedNode != null)
			{
				CASTreeItemData tid = (CASTreeItemData)SelectedNode.Tag;
				if (tid.pCode>0)
				{
					if ((int)(mMenuCommandSet&eCommand.Delete)>0)
					{
						mMIDel = new MenuItem("Удалить", new EventHandler(OnDelete),Shortcut.Del);
						this.ContextMenu.MenuItems.Add(mMIDel);
					}

					if ((int)(mMenuCommandSet&eCommand.Edit)>0)
						this.ContextMenu.MenuItems.Add(new MenuItem("Переименовать", new EventHandler(OnRename),Shortcut.F2));
					if ((int)(mMenuCommandSet&eCommand.Property)>0)
					{
						this.ContextMenu.MenuItems.Add(new MenuItem("-"));
						this.ContextMenu.MenuItems.Add(new MenuItem("Свойства", new EventHandler(OnProperty),Shortcut.ShiftF2));
					}
				}
			}

			if ((int)(mMenuCommandSet&eCommand.Find)>0)
			{
				if (this.ContextMenu.MenuItems.Count>0 ) //если надо - добавляем разделитель
					this.ContextMenu.MenuItems.Add(new MenuItem("-"));
				this.ContextMenu.MenuItems.Add(new MenuItem("Найти", new EventHandler(OnFind),Shortcut.CtrlF));
			}
		}
		private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			TreeNode tn = GetNodeAt(e.X, e.Y);
			if (tn != null)
			{
				if (e.Button == MouseButtons.Right)
				{
					if (AfterSelectRight != null)
						AfterSelectRight(this, new EvA_CasTV(tn));
				}
			}
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
						mTreeViewNode = tn;
					}
					SelectedNode.EnsureVisible();	
				}
			}
			else if (OnDoCommand != null)
				OnDoCommand(this, new EvA_CasTVCommand(aTn, aCmd)); 
		}

		private void OnChoice (object sender, System.EventArgs e)
		{
			if (SelectedNode != null && (SelectedNode.Nodes.Count == 0 || pIsMaySelectedNode))
				DoCommand(SelectedNode, eCommand.Choice);
		}
		private void OnNew (object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Add);
		}
		private void OnNewFolder (object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.AddFolder);
		}
		private void OnDelete (object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Delete);
		}
		private void OnRename (object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Edit);
		}
		private void OnProperty (object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Property);
		}
		private void OnFind(object sender, System.EventArgs e)
		{
			DoCommand(SelectedNode, eCommand.Find);
		}

		private void OnDoubleClickTreeItem(object sender, System.EventArgs e)
		{
			if ((int)(mMenuCommandSet&eCommand.Choice)>0 && ((SelectedNode != null && SelectedNode.Nodes.Count == 0) || pIsMaySelectedNode))
				DoCommand(SelectedNode, eCommand.Choice);
		}

		#endregion

		public void RestorePrevSelectNode()
		{
			if (mTreeViewNode!=null) // && mTreeViewNode.TreeView!=null) //т.е. узел есть в дереве !!!
				this.SelectedNode=mTreeViewNode;
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
								Load(tid.pPlace, tid.pCode, false);
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
		//		public void GoToBackward()
		//		{
		//			if (mSHNM != null)
		//			{
		//				mSHNM.GoToBackward();
		//				if (mSHNM.pCurrentState != null)
		//				{
		//					CASTreeItemData tid = (mSHNM.pCurrentState as TreeNode).Tag as CASTreeItemData;
		//					if (tid != null && tid.pPC.IsDefined)
		//					{
		//						mIsMayStorySH = false;
		//						try	{ Load(tid.pPlace, tid.pCode, false);	}
		//						finally	{	mIsMayStorySH = true;	}
		//					}
		//				}
		//			}
		//		}
		//
		//		public void GoToForward()
		//		{
		//			if (mSHNM != null)
		//			{
		//				mSHNM.GoToForward();
		//				if (mSHNM.pCurrentState != null)
		//				{
		//					CASTreeItemData tid = (mSHNM.pCurrentState as TreeNode).Tag as CASTreeItemData;
		//					if (tid != null && tid.pPC.IsDefined)
		//					{
		//						mIsMayStorySH = false;
		//						try	{ Load(tid.pPlace, tid.pCode, false);	}
		//						finally	{	mIsMayStorySH = true;	}
		//					}
		//				}
		//			}
		//		}
		#endregion
		
		public virtual object Clone()
		{
			CASTreeViewCommon ct=(CASTreeViewCommon)base.MemberwiseClone();
			if (mMIDel != null)
				ct.mMIDel=mMIDel.CloneMenu();
			if (mMIIns != null)
				ct.mMIIns=mMIIns.CloneMenu();
			ct.mTreeViewNode=mTreeViewNode;
			ct.mIsLocking=mIsLocking;
			ct.mMenuCommandSet=mMenuCommandSet;
			ct.mIsMayEdit=mIsMayEdit;
			ct.mCIC=mCIC;
			if (mFindText != null)
				ct.mFindText=(string)mFindText.Clone();
			ct.mIsMatchCase=mIsMatchCase;
			ct.mIsWordWhole=mIsWordWhole;
			ct.mFindQnt=mFindQnt;
			if (pErr != null)
				ct.pErr=(Error)pErr.Clone();

			ct.components=components;
			if (pEmptyItemText != null)
				ct.pEmptyItemText=(string)pEmptyItemText.Clone();
			if (pPathDelim != null)
				ct.pPathDelim =(string)pPathDelim.Clone();
			ct.mIsMayEdit = mIsMayEdit;
			return ct;
		}
    
		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			pErr = new Error();

			pIsMayEdit = true;
			pIsMaySelectedNode = false;

			mFindText = string.Empty;
			mIsMatchCase = false;
			mIsWordWhole = false;
			mIsLocking = false;

			mTreeViewNode=null;

			DoubleClick		+= new System.EventHandler(OnDoubleClickTreeItem);
			MouseDown	+= new System.Windows.Forms.MouseEventHandler(OnMouseDown);
			BeforeLabelEdit += new NodeLabelEditEventHandler (OnBeforeLabelEdit);
			AfterLabelEdit += new NodeLabelEditEventHandler (OnAfterLabelEdit);

			ContextMenu = new ContextMenu();
			ContextMenu.Popup += new EventHandler (OnDisplayContextMenu);
			mMIDel = null;
    
			//this.AfterSelect+=new TreeViewEventHandler(onAfterSelectNode);
			//this.BeforeSelect+=new TreeViewCancelEventHandler(onBeforeSelectNode);
		}
		#endregion
	}


	// // // // // // // // // // // // // CasTreeView // // // // // // // // // // // // // // // // // // //

	/// <summary author="M.Tor" created="30.04.2003">
	/// Класс для отображения иерархической структуры ("дерево").
	/// Расчитан на поуровневую загрузку из БД.
	/// </summary>
	/// <remarks>
	/// Сам по себе данный класс нечего путного отобразить не может. 
	/// Для реального отображения необходимо либо перехватить события OnLoadByCode,
	/// OnWhatCommandBeforeLoadNextLevel и OnDefineDataForNewNode.
	/// Либо написать производный класс, где переопределить виртуальные функции:
	/// virtual void Load(int aPlace, int aCode, bool aExpandLevel),
	/// virtual void WhatCommandBeforeLoadNextLevel (TreeNode aTn),
	/// virtual void DefineDataForNewNode(TreeNode aTn, OleDbDataReader aReader). 
	/// </remarks> 
	public class CasTreeView : CASTreeViewCommon
	{
		#region CONSTANTS:
		/// <summary>
		/// Текст для корневого элемента
		/// </summary>
		protected const string ROOT_NAME				= "***";
		#endregion

		#region PROPERTY:
		private TreeNode					_tnBeginFindParent;

		protected OleDbCommand		mCommand = null;

		public int								pRootItemCode = 0;
		//  добавлено dsy 03.08.2005
		/// <summary>
		/// Видимость корня некоторой ветки дерева, с которого дерево должно грузиться.
		/// </summary>
		public bool								pIsRootVisible;
	 
		/// <value>
		/// Текущее соединение с БД.
		/// </value>
		public OleDbConnection		pDBConnection
		{
			set { mCommand.Connection = value; }
			get { return mCommand.Connection;  }
		}

		public OleDbCommand				pCommand
		{
			get { return mCommand; }
		}

		/// <summary>
		/// Возвращает дополнительные данные (TreeItemData) корневого элемента для текущего.
		/// </summary>
		public TreeItemData				pCurrentRootData
		{
			get { return RootData(SelectedNode); }
		}

		#endregion
		//использование события After или Before для загрузки уровня
		public bool								pUseEventAfterExpand;
		/// <summary>
		/// Если да - используем BeginUpdate и EndUpdate() на дереве !!!
		/// </summary>
		public bool								pUseLockUpdate;
		#region Event declaration:
		public event EvH_CasTV								OnWhatCommandBeforeLoadNextLevel;
		public event CasTVLoadByCodeEHandler  OnLoadByCode;
		public event CasTVReadEventHandler		OnDefineDataForNewNode;
		#endregion

		private System.ComponentModel.Container components = null;
		
		#region constructor

		public CasTreeView(): this (null){}
		public CasTreeView(OleDbConnection aCn)
		{
			mCommand = new OleDbCommand();
			mCommand.Connection = aCn;
			_tnBeginFindParent = null;

			InitializeComponent();
			//как и было раньше !!!
			pUseEventAfterExpand=false;
			pUseLockUpdate=true;
			pIsRootVisible = false;	//  по умолчанию корень не будет виден
		}

		#endregion --constructor

		#region PUBLIC METHOD

		/// <summary>
		/// Загрузка дерева с корневого элемента.
		/// </summary>
		/// <param name="aExpandLevel">true - раскрывать следующий уровень корневого элемента;
		/// false - нет</param>
		public override void Load(bool aExpandLevel)
		{
			Nodes.Clear();
			if (SelectedNode == null)
			{
				//BeginUpdate();
				AddNextLevel(null);
				foreach(TreeNode tn in Nodes)
					AddNextLevel(tn); 
				if (aExpandLevel && Nodes.Count > 0)
					Nodes[0].Expand(); 
				//EndUpdate();
			}
			else
			{
				TreeItemData tid = (TreeItemData)(SelectedNode.Tag);
				Load(tid.pPath, tid.pPlace, aExpandLevel);
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
		public virtual void Load(string aItemPath, int aItemPlace, bool aExpandLevel)
		{
			if (aItemPath == null || aItemPath.Length == 0) 
			{
				Load(true);
				pErr.text = "path empty";
				return;
			}

			int ii = 0;
			//BeginUpdate();
			UpdateLock(true);
			Nodes.Clear();
			//AddNextLevel(null);
			Load(); 
			string[] ss = aItemPath.Split(pPathDelim.ToCharArray());
			// сначало пропустим все до root-a
			if (pRootItemCode > 0)
				for (; ii < ss.Length; ii++)
					if (pRootItemCode == Convert.ToInt32(ss[ii]))
					{
						if(!pIsRootVisible)
							ii++;
						break;
					}

			TreeNode tn = null;
			if (ii < ss.Length)
				tn = FindNodeByCode(Nodes, aItemPlace, Convert.ToInt32(ss[ii]));
			try
			{
				AddNextLevel(tn);
				if (tn != null)
					SelectedNode = tn;
				for (ii++; ii < ss.Length && tn != null; ii++)
				{
					tn = FindNodeByCode(tn.Nodes, aItemPlace, Convert.ToInt32(ss[ii]));
					if (tn != null)
						SelectedNode = tn;
				}
				if (aExpandLevel && tn!= null)
					SelectedNode.Expand(); 

				if (tn == null)
					Load(false);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			//EndUpdate();
			UpdateLock(false);
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

			int pl = ((TreeItemData)aCurrentTN.Tag).pPlace;
			int cd = ((TreeItemData)aCurrentTN.Tag).pCode;
			mIsLocking = true;
			UpdateLock(true);	//BeginUpdate();

			AddNextLevel(ptn);
			foreach (TreeNode itn in ptn.Nodes)
				AddNextLevel(itn);
			TreeNode tn = FindNodeByCode(ptn.Nodes, pl, cd);
			if (tn != null)
				SelectedNode = tn;
			else
				SelectedNode = ptn;

			UpdateLock(false);//EndUpdate();
			//восстанавливаем !
			if (expandThisNode)
				SelectedNode.Expand();
			mIsLocking = false;
		}
		//реулируем работу с отображением на дереве
		protected void UpdateLock(bool begin)
		{
			if (!pUseLockUpdate) //если не заморожено !!!
			{
				if (begin)
					BeginUpdate();
				else
					EndUpdate();
			}
		}
		#endregion PUBLIC METHOD ...

		#region VIRTUAL METHOD:

		/// <summary>
		/// Загрузка дерева и установка на текущем элементе по паре Place:Code.
		/// </summary>
		/// <remarks>
		/// Есть две возможности. Либо перехватить событие OnLoadByCode и там определить
		/// путь (e.pPath) для элемента Place:Code. Либо в производном классе переопределить 
		/// данный метов где найти path элемента и вызвать Load <see cref="CasTreeView.Load(string, int, bool)"/> по пути.
		/// </remarks> 
		/// <param name="aPlace">place</param>
		/// <param name="aCode">код</param>
		/// <param name="aExpandLevel">true - раскрывать следующий уровень от текущего элемента;
		/// false - нет</param>
		public override void Load(int aPlace, int aCode, bool aExpandLevel)
		{
			CasTVLoadByCodeArgs lbca = new CasTVLoadByCodeArgs(aPlace, aCode);
			if (OnLoadByCode != null)
				OnLoadByCode(this, lbca);

			if (lbca.pPath != null && lbca.pPath.Length > 0)
				Load(lbca.pPath, aPlace, aExpandLevel);  
			else
				Load(aExpandLevel);
		}
		/// <summary>
		/// Mетод вызывается перед загрузкой новой ветви дерева для элемента (TreeNode aTn),
		/// с целью получения команды (pCommand), с помощью которой, будет выполнена
		/// эта загрузка. 
		/// </summary>
		/// <remarks>
		/// Есть две возможности определения этой команды. Первая, перехватить
		/// событие OnWhatCommandBeforeLoadNextLevel и определить команду. Вторая, сделать
		/// класс производный от данного и переопределить данный вируальный метод, в котором
		/// определить команду.
		/// </remarks> 
		/// <param name="aTn">элемент дерева TreeNode для которого выполняется загузка ветви</param>
		protected virtual void WhatCommandBeforeLoadNextLevel (TreeNode aTn)
		{
			if (OnWhatCommandBeforeLoadNextLevel != null)
				OnWhatCommandBeforeLoadNextLevel(this, new EvA_CasTV(aTn)); 
		}

		/// <summary>
		/// Метод вызывается после добавления нового элемента дерева TreeNode, с целью определения
		/// дополнительных данных (TreeItemData или производных) в теге элемента.
		/// </summary>
		/// <remarks>
		/// Есть две возможности определения данных. Первая, перехватить
		/// событие OnDefineDataForNewNode и определить данные используя OleDbDataReader aReader.
		/// Вторая, сделать класс производный от данного и переопределить данный вируальный метод,
		/// в котором определить данные используя OleDbDataReader aReader.
		/// </remarks> 
		/// <param name="aTn">новый элемент дерева TreeNode, в тег которого помещаются данные</param>
		/// <param name="aReader">источник данных</param>
		protected virtual void DefineDataForNewNode(TreeNode aTn, OleDbDataReader aReader)
		{
			if (OnDefineDataForNewNode != null)
				OnDefineDataForNewNode(this, new CasTVReadEArgs(aTn, aReader)); 
		}

		/// <summary>
		/// Виртуальный метод добавляет новую структуру CASTreeItemData в
		/// таг TreeNode. Может быть переопределен в производном классе,
		/// если нужно добавить другую структуру, обязательно производную
		/// от CASTreeItemData.
		/// </summary>
		/// <param name="aNode">элемент дерева TreeNode</param>
		protected override void NewTreeItemForNode (TreeNode aTn) 
		{
			aTn.Tag = null;

			base.NewTreeItemForNode(aTn);

			if (aTn.Tag == null)
				aTn.Tag = new TreeItemData ();
		}

		/// <summary>
		/// Добавляет новый пустой элемент на уровень ниже.
		/// </summary>
		/// <remarks>
		/// Т.к. метод виртуальный, то может быть переопределен в производном классе, 
		/// хотя совсем не обязательно.
		/// </remarks> 
		/// <param name="tn">элемент иерархии для которого загружается новый пустой элемент
		/// на уровень ниже ("новорожденный")</param>
		/// <param name="aText">название (отображение=text) "новорожденного"</param>
		protected virtual void AddEmptyItem(TreeNode tn, string aText)
		{
			if (aText.Length == 0)
				return;

			TreeNode ntn = new TreeNode(aText);
			NewTreeItemForNode(ntn);
			TreeItemData tid = (TreeItemData) ntn.Tag; 
			tid.pItemType = eTreeItemType.Empty;
			if (tn == null)
				Nodes.Add(ntn);
			else
				tn.Nodes.Add(ntn);
		}

		#endregion

		#region PRIVATE METHODS:
		//отрабатываем действия при наполнении узлов
		//пока не работает, возможно в будущем ...
		protected virtual void doEvents()
		{
			//Application.DoEvents();
			//this.Refresh();
		}
		protected override void AddNextLevel(TreeNode tn)
		{
			this.AddNextLevel(tn,true);
		}
		/// <summary>
		/// Добавляет следующий уровень в иерархию (дерево).
		/// </summary>
		/// <param name="tn">элемент иерархии для которого загружается следующий уровень ("дети")</param>
		protected void AddNextLevel(TreeNode tn,bool open)
		{
			if (tn != null)
			{
				TreeItemData tnd = (TreeItemData)(tn.Tag);
				if (tnd.pItemType == eTreeItemType.Empty || tnd.pItemType == eTreeItemType.Item)
					return;
				tn.Nodes.Clear();
			}

			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			UpdateLock(true); //BeginUpdate();
			WhatCommandBeforeLoadNextLevel(tn);

			if (mCommand.CommandText == string.Empty)
			{
				TreeNode ntn = new TreeNode(ROOT_NAME);
				NewTreeItemForNode(ntn);
				Nodes.Clear(); 
				Nodes.Add(ntn);
				DefineDataForNewNode(ntn, null);
				doEvents();
			}
			else if (mCommand.CommandText.Equals("empty"))
			{
			}
			else
			{
				OleDbDataReader dtReader = null;
				try
				{
					dtReader = mCommand.ExecuteReader();
					while (dtReader.Read())
					{
						//----------------------------------------------------
						// 		fields(0)[varchar]	- item name               //|
						TreeNode ntn = new TreeNode(dtReader.GetString(0)); //|
						//---------------------------------------------------- 
						NewTreeItemForNode(ntn);
						if (tn == null)
							Nodes.Add(ntn);
						else
							tn.Nodes.Add(ntn);
						DefineDataForNewNode(ntn, dtReader);

						doEvents();
						//RefreshNode(ntn);
					}
					if (tn != null && tn.Nodes.Count == 0 && ((TreeItemData)(tn.Tag)).pItemType == eTreeItemType.Node)
						AddEmptyItem (tn, pEmptyItemText);
					else if (tn == null && Nodes.Count == 0)
						AddEmptyItem (null, pEmptyItemText);
				}
				catch(Exception e)
				{
					pErr.ex = e;
					pErr.Show(); 
				}
				finally
				{
					// Always call Close when done reading.
					if (dtReader != null)
						dtReader.Close();
				}
			}
			UpdateLock(false); //EndUpdate();
			this.Cursor = System.Windows.Forms.Cursors.Default;   
		}
		private void RefreshNode(TreeNode tn)
		{
			if (!pUseLockUpdate) //если не заморожено !!!
				tn.EnsureVisible();
		}
		#endregion

		#region EVENTS Handler:

		protected override void OnBeforeExpand(TreeViewCancelEventArgs tvcea)
		{
			base.OnBeforeExpand(tvcea);
		
			if (!pUseEventAfterExpand)
			{
				//BeginUpdate();
				foreach (TreeNode tn in tvcea.Node.Nodes)
					AddNextLevel(tn);
				//EndUpdate();
			}
		}	

		protected override void OnAfterExpand(TreeViewEventArgs e)
		{
			base.OnAfterExpand(e);
			if (pUseEventAfterExpand)
			{
				//BeginUpdate();

				foreach (TreeNode tn in e.Node.Nodes)
					AddNextLevel(tn);

				//EndUpdate();
			}
		}

		#endregion

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
			if (mFindText.Length>0)
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
		}
		#endregion

		#endregion Staff.Net
	}


	// // // // // // // // // // // // CASTreeViewAll // // // // // // // // // // // // // // // // // // // //

	/// <summary author="M.Tor" created="29.05.2003">
	/// Класс для отображения иерархической структуры ("дерево").
	/// Расчитан на ПОЛНУЮ (отсюда All в конце) загрузку дерева.
	/// </summary>
	/// <remarks>
	/// Сам по себе данный класс нечего путного отобразить не может. 
	/// Для реального отображения необходимо написать производный класс,
	/// где определить функцию:
	/// public void LoadAll(int aRootCode), которая осуществляет
	/// полную загрузка дерева (иерархию коллекций Nodes)
	/// </remarks> 
	public class CASTreeViewAll: CASTreeViewCommon
	{
		private eCommand	mLastCommand;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string			pTextForNewItem;

		#region Event declaration:
		public event CasObjectEventHandler 	OnBeforeViewItemProperty;
		public event CasObjectEventHandler 	OnBeforeDeleteItem;
		#endregion

		public CASTreeViewAll()
		{
			LabelEdit = false; 
			mFindQnt = 0;
			InitTextForNewItem();

			this.KeyDown += new KeyEventHandler(OnKeyDown);
			//this.AfterLabelEdit  += new NodeLabelEditEventHandler(OnTreeAfterLabelEdit);
			//this.BeforeLabelEdit += new NodeLabelEditEventHandler(OnTreeBeforeLabelEdit); 
		}

		public void ClearAllNodes()
		{
			Nodes.Clear();
		}

		public void AddNodesFromExistCollection(TreeNode aNd)
		{
			if (aNd != null)
				foreach (TreeNode tn in aNd.Nodes)
					Nodes.Add((tn.TreeView != null) ? (TreeNode)tn.Clone() : tn); 
		}

		public int CountTotal(TreeNodeCollection aTnc)
		{
			int ret = 0;
			foreach (TreeNode tn in aTnc)
			{
				if (tn.Nodes.Count > 0)
					ret += CountTotal(tn.Nodes);
				ret++;
			}
			return ret;
		}

		public void DoCommand(eCommand aCmd)
		{
			this.DoCommand(SelectedNode, aCmd);
		}

		public override void DoCommand(TreeNode aTn, eCommand aCmd)
		{
			mLastCommand = aCmd;

			if (SelectedNode == null && (aCmd&eCommand.Add) != eCommand.Add)
				return;

			if (!pIsMayEdit && (aCmd&(eCommand.Add|eCommand.Edit|eCommand.Delete))>0)
				return;

			//BeginUpdate(); 
			switch (aCmd)
			{
				case eCommand.Add:
					ItemAdd_TV();
					break;
				case eCommand.Add|eCommand.Edit:
					ItemAddRemane_TV();
					break;
				case eCommand.Delete:
					ItemDelete();
					break;
				case eCommand.Edit:
					ItemRename_TV();
					break;
				case eCommand.Property :
					ItemProperty();
					break;
			}

			base.DoCommand (aTn, aCmd);
			//EndUpdate();
		}

		protected void InitTextForNewItem()
		{
			pTextForNewItem = "[Новый элемент]";
		}

		protected virtual bool ItemAdd()
		{
			return false;
		}
		protected virtual bool ItemRename()
		{
			return false;
		}
		protected virtual bool ItemDelete()
		{
			Nodes.Remove(SelectedNode);  
			return true;
		}
		protected virtual void ItemProperty()
		{
		}

		protected void ItemAddNewNode()
		{
			TreeNode tn = new TreeNode(pTextForNewItem);
			NewTreeItemForNode (tn);
			if (SelectedNode != null && SelectedNode.Parent != null)
				SelectedNode.Parent.Nodes.Add(tn);
			else
				Nodes.Add(tn);
			SelectedNode = tn;
		}

		protected void ItemAdd_TV()
		{
			ItemAddNewNode();
			if (ItemAdd())
			{
				CASTreeItemData tid;
				if (SelectedNode.Parent == null)
				{
					tid = (CASTreeItemData)SelectedNode.Tag;
					ReloadLevel(tid.pPlace, 0);
				}
				else
				{
					tid = (CASTreeItemData)SelectedNode.Parent.Tag;
					ReloadLevel(tid.pPlace, tid.pCode);
				}
				SelectedNode = SearchByText(pTextForNewItem);
			}
		}

		protected void ItemAddRemane_TV()
		{
			ItemAddNewNode();
			ItemRename_TV();
		}

		protected void ItemRename_TV()
		{
			//this.SelectedNode.EndEdit(false); 
			//OnTreeBeforeLabelEdit(this, new NodeLabelEditEventArgs(SelectedNode, SelectedNode.Text));
			//this.Refresh(); 
			//AfterLabelEdit  += new NodeLabelEditEventHandler(OnTreeAfterLabelEdit);
			this.LabelEdit = true;
			this.SelectedNode.BeginEdit();  
			//MessageBox.Show(this.GetHashCode().ToString("Hash code = {0}"));    
		}

		//private void OnTreeBeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		//{
		//	e.Node.Text = e.Node.Text;   
		//}

		//private void OnTreeAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		//{
		//	this.LabelEdit = false;
		//}

		//		private void OnTreeAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		//		{
		//			if (e.Label == null && mLastCommand != (eCommand.Add|eCommand.Edit)) // press ESC - canceled
		//				e.CancelEdit = true;
		//			else
		//			{
		//				bool ret = false;
		//				string prevText = e.Node.Text;
		//				if (e.Label != null)
		//					e.Node.Text = e.Label;  
		//				if (mLastCommand == (eCommand.Add|eCommand.Edit))
		//				{
		//					ret = ItemAdd();
		//					if(!ret)
		//						Nodes.Remove(e.Node);  
		//				}
		//				else if (mLastCommand == eCommand.Edit)
		//				{
		//					ret = ItemRename(); 
		//				}
		//				if (!ret)
		//					e.Node.Text = prevText;
		//
		//				//e.Node.EndEdit(ret); 
		//				//e.CancelEdit = !ret;
		//				if (ret)
		//				{
		//					e.CancelEdit = true;
		//					CASTreeItemData tid;
		//					if (e.Node.Parent == null)
		//					{
		//						tid = (CASTreeItemData)e.Node.Tag;
		//						ReloadLevel(tid.pPlace, 0);
		//					}
		//					else
		//					{
		//						tid = (CASTreeItemData)e.Node.Parent.Tag;
		//						ReloadLevel(tid.pPlace, tid.pCode);
		//					}
		//					SelectedNode = SearchByText(e.Node.Text);
		//				}
		//			}
		//			InitTextForNewItem();
		//			this.LabelEdit = false;
		//			//this.AfterLabelEdit  -= new NodeLabelEditEventHandler(OnTreeAfterLabelEdit);
		//		}

		/*protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);
 
			if (e.Label == null && mLastCommand != (eCommand.Add|eCommand.Edit)) // press ESC - canceled
				e.CancelEdit = true;
			else
			{
				bool ret = false;
				string prevText = e.Node.Text;
				if (e.Label != null)
					e.Node.Text = e.Label;  
				if (mLastCommand == (eCommand.Add|eCommand.Edit))
				{
					ret = ItemAdd();
					if(!ret)
						Nodes.Remove(e.Node);  
				}
				else if (mLastCommand == eCommand.Edit)
				{
					ret = ItemRename(); 
				}
				if (!ret)
					e.Node.Text = prevText;

				e.Node.EndEdit(ret); 
				//e.CancelEdit = !ret;
				if (ret)
				{
					//e.CancelEdit = true;
					CASTreeItemData tid;
					if (e.Node.Parent == null)
					{
						tid = (CASTreeItemData)e.Node.Tag;
						ReloadLevel(tid.pPlace, 0);
					}
					else
					{
						tid = (CASTreeItemData)e.Node.Parent.Tag;
						ReloadLevel(tid.pPlace, tid.pCode);
					}
					SelectedNode = SearchByText(e.Node.Text);
				}
			}
			InitTextForNewItem();
			this.LabelEdit = false;
		}*/

		protected void EventBeforeViewItemProperty(ref object aRefBook, ref int aPlace)
		{
			CasObjectEventArgs ae = new CasObjectEventArgs(aRefBook, aPlace);
			if (OnBeforeViewItemProperty != null)
				OnBeforeViewItemProperty(this, ae);
			aRefBook	= ae.pObject;
			aPlace		= ae.pInt;
		}

		protected void EventBeforeDeleteItem(ref object aObj, ref int aPlace)
		{
			CasObjectEventArgs ae = new CasObjectEventArgs(aObj, aPlace);
			if (OnBeforeDeleteItem != null)
				OnBeforeDeleteItem(this, ae);
			aObj		= ae.pObject;
			aPlace	= ae.pInt;
		}

		private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//if (this.LabelEdit)
			//	return;

			if (e.KeyCode == Keys.Insert)
				DoCommand(eCommand.Add|eCommand.Edit);
			else if (e.KeyCode == Keys.Delete)
				DoCommand(eCommand.Delete);
			else if (e.KeyCode == Keys.F2)
				DoCommand(eCommand.Edit);
				//			else if (e.KeyCode == Keys.F3 || (e.Modifiers == Keys.Control && e.KeyCode == Keys.F))
				//			{
				//				int fndQnt;
				//				if (e.Shift)
				//				{
				//					mFindQnt--;
				//					if (mFindQnt < 0) 
				//						mFindQnt = 0;
				//					fndQnt = mFindQnt;
				//					TreeNode tn = SearchByTextPart(Nodes, ref fndQnt, false);
				//					if (tn != null)
				//						SelectedNode = tn;
				//				}
				//				else if (e.KeyCode == Keys.Escape)
				//				{
				//					if (this.SelectedNode.IsEditing)
				//						MessageBox.Show ("CancelEdit");
				//					else
				//						MessageBox.Show ("Keys.Escape");
				//				}
				//				else
				//				{
				//					++mFindQnt;
				//					fndQnt = mFindQnt;
				//					TreeNode tn = SearchByTextPart(Nodes, ref fndQnt, false);
				//					if (tn != null)
				//						SelectedNode = tn;
				//					else
				//						mFindQnt--;
				//				}
				//			}
			else if (e.KeyCode == Keys.Space)
				DoCommand(SelectedNode, eCommand.Choice);
		}


		/// <summary>
		/// </summary>
		/// <param name="aNode">элемент дерева TreeNode</param>
		protected override void NewTreeItemForNode (TreeNode aTn) 
		{
			aTn.Tag = null;

			base.NewTreeItemForNode(aTn);

			if (aTn.Tag == null)
				aTn.Tag = new CASTreeItemData ();
		}

		#region Load ...
		/// <summary>
		/// Виртуальная функция загрузки ВСЕЙ иерархии в память (TreeNode)
		/// начиная с указаного узла. 
		/// ОБЯЗАТЕЛЬНО должна быть определена в производнос классе.
		/// </summary>
		/// <param name="aRootCode">узел с которого выполняется загрузка иерархии</param>
		//public virtual void LoadAll(int aRootCode)
		//{
		//}
		public void ReloadLevel(PlaceCode pc)
		{
			ReloadLevel(pc.place,pc.code);
		}

		public virtual void ReloadLevel(int aPlaceOwner, int aCodeOwner)
		{
		}

		public override void Load(int aPlace, int aCode, bool isExpand)
		{
			//bool ret = false;
			TreeNode tn = SearchByCode(aPlace, aCode);
			if (tn != null)
			{
				SelectedNode = tn;
				if (isExpand)
					SelectedNode.Expand();
				//ret = true;
			}
			//return ret;
		}

		public virtual bool Load(string aText, bool isExpand)
		{
			bool ret = false;
			TreeNode tn = SearchByText(aText, true);
			if (tn != null)
			{
				SelectedNode = tn;
				if (isExpand)
					SelectedNode.Expand();  
				ret = true;
			}
			return ret;
		}
		#endregion Load ...

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
				pTextForNewItem						= null;
				OnBeforeViewItemProperty	= null;
				//CASTools.DisposeFields(this); 
			}
			base.Dispose( disposing );
		}
	}
}
