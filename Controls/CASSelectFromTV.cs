using System;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{

  /// <summary>
  /// Тоже самое, что и  CASSparrow, но работает с CASTreeViewBase
  /// 16.09.2004
  /// </summary>
  public class CASSelectFromTV : System.Windows.Forms.UserControl
  {
    public const int DEFAULT_TV_HEIGHT = 150;

    #region PROPERTY:
    //всплывающее окно!
    private Form frm;
    private Rectangle sfrm; //запоминаем размеры!
    private System.Windows.Forms.ToolTip tTip;
    private System.ComponentModel.IContainer components;

    protected System.Windows.Forms.Button cmd;
    protected TextBox txt;
    protected CASTreeViewBase tv;
    protected PlaceCode mPC;
    protected int mTreeViewHeight;
    protected bool mIsTVVisable;
    protected System.Windows.Forms.Label lblSize;

    /// <summary>
    /// В TextBox можно добавлять элемент, которого нет в справочнике
    /// и при этом он не добавляется туда.
    /// </summary>
    public bool pIsMayBeWithoutRefbook;
    public bool pDownSelectIfNotFound;
      public ContextMenuStrip pCMenuStrip
      {
          set
          {
              txt.ContextMenuStrip = value;
              cmd.ContextMenuStrip = value;
          }
      }
    /// <summary>
    /// Раскрывать узел при открытии иерархии.
    /// </summary>
    public bool pIsExpandLevelWhenLoad;
    /// <summary>
    /// При наведении фокуса на форму - всегда вставать на текстовое поле
    /// </summary>
    /// <returns></returns>
    public new bool Focus()
    {
      return FocusTvTxt();
    }
    public new bool Focused
    {
      get
      {
        if (mIsTVVisable)
          return tv.Focused;
        else
          return txt.Focused;
      }
    }
    public TextBox pTextBox
    {
      get { return txt; }
    }

    public CASTreeViewBase pTreeView
    {
      set
      {
        if (tv == value)
          return;

        if (tv != null)
        {
          //tv.Dispose();
          tv.onDoCommand -= new EvH_CasTVCommand(OnTreeItemDoCommand);
          if (frm != null)
            frm.Controls.Remove(tv);
        }

        tv = value;
        mIsTVVisable = false;

        if (tv != null)
        {
          tv.Font = this.Font;
          //tv.AddMenuCommand(eCommand.Choice);
          tv.Visible = mIsTVVisable;
          tv.pIsMaySelectedNode = true;
          tv.onDoCommand += new EvH_CasTVCommand(OnTreeItemDoCommand);
        }
      }
      get { return tv; }
    }

    public BorderStyle pBorderStyle
    {
      set
      {
        txt.BorderStyle = value;
        tv.BorderStyle = value;
      }
    }

    public PlaceCode pItemPC
    {
      [DebuggerStepThrough]
      get { return mPC; }
      [DebuggerStepThrough]
      set { mPC = value; }
    }

    public string pItemPCString
    {
      get { return mPC.PlaceDelimCode; }
      set
      {
        PlaceCode pc = PlaceCode.PDC2PlaceCode(value);
        SetItem(pc);
      }
    }
    public int pItemPlace
    {
      get { return mPC.place; }
      set { mPC.place = value; }
    }
    public int pItemCode
    {
      [DebuggerStepThrough]
      get { return mPC.code; }
      set { SetItem(value); }
    }
    public string pItemText
    {
      get { return txt.Text; }
      set { txt.Text = value; }
    }

    [LocalizableAttribute(true)] 
    [BindableAttribute(true)]
    public int pItemBinding
    {
      set
      {
        mPC.code = value;
        if (mPC.code > 0)
        {
          tv.Load(mPC, false);
          if (tv.SelectedNode != null)
            txt.Text = tv.SelectedNode.Text;
        }
        else
          txt.Text = string.Empty;
      }
      get
      {
        return mPC.code;
      }
    }

    /// <summary>
    /// Используется только код из PlaceCode
    /// </summary>
    public bool pIsCodeOnly;

    /// <summary>
    /// НЕОБХОДИМО, чтоб корректно работало 
    ///   this.BindingContext[mPropTab].EndCurrentEdit();
    ///   (не устанавливало RowState == Modified в любом случае)
    /// </summary>
    [LocalizableAttribute(true)]
    [BindableAttribute(true)]
    public override string Text
    //public string pCodeText
    {
      get
      {
        if (pIsCodeOnly)
          return mPC.code.ToString();
        else
          return mPC.PlaceDelimCode;
      }
      set
      {
        if (pIsCodeOnly)
          mPC.code = CASTools.ConvertToInt32Or0(value);
        else
          mPC = PlaceCode.PDC2PlaceCode(value);
        if (mPC.code > 0)
        {
          tv.Load(mPC, false);
          if (tv.SelectedNode != null)
            txt.Text = tv.SelectedNode.Text;
        }
        else
          txt.Text = string.Empty;

      }
    }

    public object pItemTreeNodeTag;
    public Rectangle pSizeTV
    {
      get { return sfrm; }
      set
      {
        if ((value.Height * value.Width) > 0)
          sfrm = value;
      }
    }

    public override Color ForeColor
    {
      get
      {
        return base.ForeColor;
      }
      set
      {
        base.ForeColor = value;
        cmd.ForeColor = value;
        txt.ForeColor = value;
      }
    }

    public override Font Font
    {
      get
      {
        return base.Font;
      }
      set
      {
        base.Font = value;
        txt.Font = value;
      }
    }

    #endregion

    public delegate void SelectedTreeItemEventHandler(object sender, EvA_SelectedTreeItem e);
    public event SelectedTreeItemEventHandler OnSelectedTreeItem;
    //public event EventHandler OnLeaveSelecteFromTree;
    public event EventHandler OnValueChanged;
    public event EventHandler OnBeforeShowTV;

    public CASSelectFromTV() : this(false) { }
    public CASSelectFromTV(bool aIsDataGridTextBox)
    {
      mPC = PlaceCode.Empty;
      mTreeViewHeight = 100;
      pIsMayBeWithoutRefbook = false;
      pDownSelectIfNotFound = true;
      pIsExpandLevelWhenLoad = false;
      mIsTVVisable = false;
      pItemTreeNodeTag = null;
      pIsCodeOnly = true;

      SuspendLayout();

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
//-----------
      this.Controls.Add(txt);

      tv = new CASTreeViewBase();
      tv.Font = this.Font;
      tv.ContextMenu = new ContextMenu();
      tv.ContextMenu.MenuItems.Add(new MenuItem("Выбрать", new EventHandler(onSelectedItem), Shortcut.CtrlS));
      tv.Visible = mIsTVVisable;
      tv.onDoCommand += new EvH_CasTVCommand(OnTreeItemDoCommand);

      this.ResumeLayout(false);
      this.Layout += new LayoutEventHandler(OnControlLayout);

      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      //OnResize(EventArgs.Empty);
      //			this.ParentChanged += new EventHandler(CASSelectFromTV_ParentChanged);
    }

    public override bool Equals(object obj)
    {
      bool ret = false;
      CASSelectFromTV sft = obj as CASSelectFromTV;
      if (sft != null)
      {
        ret = mPC.Equals(sft.pItemPC);
      }
      return ret;
    }


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
      pItemTreeNodeTag = null;
      if (mPC.code > 0)
      {
        tv.Load(mPC, false);
        if (tv.SelectedNode != null)
        {
          txt.Text = tv.SelectedNode.Text;
          if (tv.SelectedNode != null)
            pItemTreeNodeTag = tv.SelectedNode.Tag;
          else
            pItemTreeNodeTag = null;
        }
      }
      else
      {
        tv.Load();
        txt.Text = string.Empty;
      }
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

    public void AddTreeNode(TreeNode aNode)
    {
      tv.Nodes.Add(aNode);
    }

    /// <summary>
    /// Добавляем в ноды с верхним уровнем
    /// </summary>
    /// <param name="aNode"></param>
    public void AddNodes(TreeNode aNode)
    {
      AddNodes(aNode, false);
    }
    /// <summary>
    /// Добавляем в ноды как хотим
    /// </summary>
    /// <param name="aNode"></param>
    /// <param name="viewRoot">Показывать верхний уровень ?</param>
    public void AddNodes(TreeNode aNode, bool viewRoot)
    {
      if (aNode != null)
      {
        if (viewRoot)
          //tv.Nodes.Add(aNode.TreeView != null ? (TreeNode)aNode.Clone() : aNode);
          tv.Nodes.Add((TreeNode)aNode.Clone());
        else
        {
          foreach (TreeNode tn in aNode.Nodes)
            tv.Nodes.Add((TreeNode)tn.Clone());
        }
      }
    }
    /// <summary>
    /// Добавляем в ноды как хотим
    /// </summary>
    /// <param name="aNode"></param>
    public void AddTreeView(CASTreeViewBase aView)
    {
      if (aView != null)
      {
        tv = (CASTreeViewBase)aView.Clone();
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

    public bool FocusTvTxt()
    {
      if (mIsTVVisable)
        return tv.Focus();
      else
        return txt.Focus();
    }

    private void cmd_Click(object sender, System.EventArgs e)
    {
      TV(!mIsTVVisable);
    }

    private void OnTreeItemDoCommand(object sender, EvA_CasTVCommand e)
    {
      if (e.pCommand == eCommand.Choice)
        onSelectedItem(null, null);
    }

    private void OnTextChanged(object sender, System.EventArgs e)
    {
      OnTextChanged(e);
    }
    protected override void OnTextChanged(EventArgs e)
    {
      if (txt.Text.Length == 0)
      {
        mPC.code = 0;
        pItemTreeNodeTag = null;
        //удаляем все подсказки
        tTip.RemoveAll();
      }
      else if (tv.SelectedNode != null)
        //показываем полный путь
        tTip.SetToolTip(txt, tv.SelectedNode.FullPath);
      if (OnValueChanged != null)
        OnValueChanged(this, new EventArgs());

      base.OnTextChanged(e);
    }

    protected override bool ProcessKeyMessage(ref Message m)
    {
      switch ((Keys)((int)m.WParam))
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
      return base.ProcessKeyMessage(ref m);
    }

    private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {
      if (txt.ReadOnly)
        return;

      if (e.KeyCode == Keys.F4)
      { //вызываем "деревянный" комбик
        //просто показываем - оно само найдется !
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
      if (txt.Text.Length > 0 && ((int)e.KeyChar) > 0x1F)
      {
        string newTxt;
        if (txt.SelectionStart > 0 && txt.SelectionStart < txt.Text.Length)
          newTxt = txt.Text.Substring(0, txt.SelectionStart) + e.KeyChar.ToString();
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
          tTip.SetToolTip(txt, tn.FullPath);
          pItemTreeNodeTag = tn.Tag;
        }
        else
        {
          mPC.code = 0;
          tTip.RemoveAll();
          if (!mIsTVVisable && pDownSelectIfNotFound)
          {
            txt.Focus();
          }
          pItemTreeNodeTag = null;
        }
      }
    }

    /// <summary>
    /// Выбираем из дерева
    /// </summary>
    protected virtual void onSelectedItem(object sender, System.EventArgs e)
    {
      try
      {
        //if (tv.SelectedNode != null)
        //{
        /*--made of dsy 30.01.2006--*/
        //  Исправлена очередность присвоений. Сначала должен переприсваиваться тэг!
        //  В начальном варианте это присвоение шло после присвоения названия txt.Text, что в свою
        //  очередь приводило к использованию старого значения тэга при получении PlaceCode текущего элемента
        pItemTreeNodeTag = tv.SelectedNode.Tag;
        mPC = ((CASTreeItemData)tv.SelectedNode.Tag).pPC;
        txt.Text = tv.SelectedNode.Text;
        txt.SelectionStart = 0;
        txt.SelectionLength = 0;
        tTip.SetToolTip(txt, tv.SelectedNode.FullPath);
        if (OnSelectedTreeItem != null)
          OnSelectedTreeItem(this, new EvA_SelectedTreeItem(mPC, txt.Text, pItemTreeNodeTag));
        //}
      }
#if DEBUG
      catch (Exception ex)
      {
        MessageBox.Show("CASSelectFromTV : " + ex.Message);
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

    /// <summary>
    /// Создаем новое окно, если оно еще не создано!
    /// </summary>
    private void InitFrmTV()
    {
      if (frm == null)
      {
        frm = new System.Windows.Forms.Form();
        frm.StartPosition = FormStartPosition.Manual;
        frm.FormBorderStyle = FormBorderStyle.SizableToolWindow; //FormBorderStyle.Sizable
        frm.ControlBox = false;
        if (sfrm.IsEmpty)
          frm.Size = new Size(this.Width, DEFAULT_TV_HEIGHT);
        else
          frm.Bounds = sfrm;
        frm.Font = this.Font;
        frm.ShowInTaskbar = false;
        frm.KeyPreview = true;

        //tv.LostFocus				+= new EventHandler (OnLostFocus);
        //tv.AfterSelect			+= new TreeViewEventHandler(OnTreeItemSelected);

        frm.KeyDown += new KeyEventHandler(OnFrmKeyDown);
        frm.Deactivate += new EventHandler(onToolHide);
        //оптимизируем и не закрываем окно !!!
        frm.Closing += new CancelEventHandler(onToolClosing);
      }
    }
    /// <summary>
    /// Обрабатываем спец. клавиши
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFrmKeyDown(object sender, KeyEventArgs e)
    {
      //если esc - выходим!
      if (e.KeyCode == Keys.Escape)
        ToolHide();
      else if (e.KeyCode == Keys.Enter && tv.SelectedNode != null)
      {
        onSelectedItem(null, null);
        ToolHide();
      }
    }
    /// <summary>
    /// Позиционируем окно в координатах
    /// </summary>
    private void ShowFrmTV()
    {
      if (frm != null)
      {
        //узнаем координаты в экране txt
        if (sfrm.IsEmpty)
        {
          frm.Width = this.Width;
        }
        else
        {
          frm.Bounds = sfrm;
        }
        frm.Bounds = CASTools.GetBoundsControl(txt, frm);

        //if (tv.Parent==null)
        {
          tv.Parent = frm;
          tv.Dock = DockStyle.Fill;
        }
        tv.Visible = true;
        tv.Focus();

        lblSize.Visible = true;
        lblSize.Top = frm.Height - lblSize.Height - 2;
        lblSize.Left = frm.Width - lblSize.Width - 2;
        //lblSize.BringToFront();

        frm.BringToFront();
        frm.Visible = true;
      }
    }

    /// <summary>
    /// оптимизируем и не закрываем окно !!!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void onToolClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      ToolHide();
      e.Cancel = true;
    }
    /// <summary>
    /// Программно убираем окно и выполняем отмену
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void onToolHide(object sender, EventArgs e)
    {
      ToolHide();
    }
    private void ToolHide()
    {
      if (frm != null)
      {
        //lblSize.SendToBack(); //прячем уголок
        sfrm = frm.Bounds;
        frm.Visible = false;
      }
      cmd.Text = "6"; //чтобы кнопка отработала
      mIsTVVisable = false;
      this.Focus();
    }

    private void TV(bool isShow)
    {
      if (mIsTVVisable == isShow)
        return;

      mIsTVVisable = isShow;
      if (isShow)
      {
        if (OnBeforeShowTV != null)
          OnBeforeShowTV(this, new EventArgs());

        cmd.Text = "5";
        InitFrmTV();
        ShowFrmTV();

        if (mPC.code > 0)
          tv.Load(mPC, pIsExpandLevelWhenLoad);
        else if (txt.Text.Length > 0)
        {
          if (tv.SelectedNode != null && tv.SelectedNode.Text.Equals(txt.Text))
          { // ничего не делаем ...
          }
          else
          { // ищем
            TreeNode tn = tv.SearchByText(txt.Text, true, true, true, null);
            if (tn != null)
              tv.SelectedNode = tn;
            else if (mPC.code > 0)
              tv.Load(mPC, pIsExpandLevelWhenLoad);
            else          /// Added by M.Tor 25.06.2008
              tv.Load();  /// Added by M.Tor 25.06.2008
          }
        }
        else
          tv.Load();

        //показываем выделенный узел
        if (tv.SelectedNode != null)
          tv.SelectedNode.EnsureVisible();
        if (tv.CanFocus)
          tv.Focus();
      }
      else
      {
        //cmd.Text = "6";
        ToolHide();

        if (mPC.code == 0 && pIsMayBeWithoutRefbook == false)
          txt.Text = string.Empty;
      }
      this.Refresh();
    }

    private void onResize()
    {
      //Graphics g=this.CreateGraphics();

      //Size s =g.MeasureString(cmd.Text,this.Font).ToSize();
      txt.Top = 0;
      txt.Left = 0;
      txt.Height = this.Font.Height + 4; //на рамку 

      cmd.Top = 0;
      //cmd.Height=txt.Height;
      cmd.Height = txt.Height - 2; //на рамку 
      cmd.Width = SystemInformation.VerticalScrollBarWidth + 2; //на рамку
      if (this.Width > cmd.Width)
      {
        cmd.Left = this.Width - cmd.Width;
        txt.Width = cmd.Left;
      }
      else
      {
        cmd.Left = 0;
        txt.Width = 0;
      }

      this.Height = txt.Height;
      //this.Width=txt.Width+cmd.Width;
      if (tv != null)
      {
        tv.Font = this.Font;
      }

      //g.Dispose();
    }

    protected override void OnResize(System.EventArgs e)
    {
      base.OnResize(e);
      onResize();
    }

    private void onFontChange(object sender, System.EventArgs e)
    {
      onResize();
    }

    //  перерисовка для нормального отображения стрелки развертывания дерева для любых разрешений экрана
    private void OnControlLayout(object sender, LayoutEventArgs e)
    {
      onResize();
    }

    #region Staff .Net

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    //protected override void Dispose(bool disposing)
    //{
    //  if (disposing)
    //  {
    //    if (components != null)
    //    {
    //      components.Dispose();
    //    }
    //    Controls.Clear();
    //    if (tv != null)
    //    {
    //      tv.Parent = null;
    //      tv.SelectedNode = null;
    //      tv.LabelEdit = false;
    //      tv.Visible = false;
    //      tv = null;
    //    }

    //    if (frm != null)
    //      frm.Dispose();
    //    frm = null;

    //    txt.Dispose();
    //    txt = null;
    //    cmd.Dispose();
    //    cmd = null;

    //    OnSelectedTreeItem = null;
    //    OnValueChanged = null;

    //  }
    //  base.Dispose(disposing);
    //}
    /// 
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
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
        this.cmd.Font = new System.Drawing.Font("Marlett", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
        this.cmd.Location = new System.Drawing.Point(140, 0);
        this.cmd.Name = "cmd";
        this.cmd.Size = new System.Drawing.Size(20, 24);
        this.cmd.TabIndex = 1;
        this.cmd.TabStop = false;
        this.cmd.Text = "6";
        this.cmd.UseVisualStyleBackColor = false;
        this.cmd.Click += new System.EventHandler(this.cmd_Click);
        // 
        // lblSize
        // 
        this.lblSize.AutoSize = true;
        this.lblSize.BackColor = System.Drawing.Color.Transparent;
        this.lblSize.Font = new System.Drawing.Font("Marlett", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
        this.lblSize.Location = new System.Drawing.Point(136, 24);
        this.lblSize.Name = "lblSize";
        this.lblSize.Size = new System.Drawing.Size(18, 12);
        this.lblSize.TabIndex = 2;
        this.lblSize.Text = "o";
        this.lblSize.Visible = false;
        // 
        // CASSelectFromTV
        // 
        this.Controls.Add(this.lblSize);
        this.Controls.Add(this.cmd);
        this.Name = "CASSelectFromTV";
        this.Size = new System.Drawing.Size(160, 24);
        this.FontChanged += new System.EventHandler(this.onFontChange);
        this.ResumeLayout(false);
        this.PerformLayout();

    }
    #endregion

    #endregion --Staff .Net
  }
}

