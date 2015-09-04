using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using CommandAS.Tools;


namespace CommandAS.Tools.Forms
{
	/// <summary>
	/// 
	/// </summary>
	public class dlgSelectFolder : System.Windows.Forms.Form
	{
	
		class DirectoryTreeView: TreeView
		{
			public int IndexHDLocal=-1;
			public DirectoryTreeView()
			{
				// Make a little more room for long directory names.
				//Width *= 2;

				PathSeparator = @"\";

				Sorted = true;

				// Get images for tree.
				//ImageList = aIconColl.pImageList;

				// Construct tree.
				RefreshTree();
			}
			public void RefreshTree()
			{
				// Turn off visual updating and clear tree.

				BeginUpdate();
				Nodes.Clear();

				// Make disk drives the root nodes. 

				string[] astrDrives = Directory.GetLogicalDrives();

				foreach (string str in astrDrives)
				{
					TreeNode tnDrive = new TreeNode(str, IndexHDLocal, IndexHDLocal);
					Nodes.Add(tnDrive);
					AddDirectories(tnDrive);

					if (str == "C:\\")
						SelectedNode = tnDrive;
				}
				EndUpdate();
			}
			void AddDirectories(TreeNode tn)
			{
				tn.Nodes.Clear();

				string          strPath = tn.FullPath;
				DirectoryInfo   dirinfo = new DirectoryInfo(strPath);
				DirectoryInfo[] adirinfo;

				try
				{
					adirinfo = dirinfo.GetDirectories();
				}
				catch
				{
					return;
				}

				foreach (DirectoryInfo di in adirinfo)
				{
					TreeNode tnDir = new TreeNode(di.Name, IndexHDLocal,IndexHDLocal);
					tn.Nodes.Add(tnDir);

					// We could now fill up the whole tree with this statement:
					//        AddDirectories(tnDir);
					// But it would be too slow. Try it!
				}
			}
			protected override void OnBeforeExpand(TreeViewCancelEventArgs tvcea)
			{
				base.OnBeforeExpand(tvcea);

				BeginUpdate();

				foreach (TreeNode tn in tvcea.Node.Nodes)
					AddDirectories(tn);

				EndUpdate();
			}
		}

		private string						_selFolder;
		private DirectoryTreeView _tvd;
		//private TreeView _tvd;
		private System.Windows.Forms.Button _cmdSelect;
		private System.Windows.Forms.Button _cmdCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private static Size								_ps;
		private static Point							_pl;

		public string							pSelectedFolder
		{
			get
			{
				if (_tvd.SelectedNode != null)
					return _tvd.SelectedNode.FullPath;
				else
					return string.Empty;
			}
			set
			{
				_selFolder = value;
			}

		}

		public dlgSelectFolder(IconCollection aIconColl)
		{
			_selFolder = string.Empty;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_tvd.ImageList = aIconColl.pImageList;
			_tvd.IndexHDLocal=aIconColl.Index(IconCollection.ShellHDLocal);

			Load += new EventHandler(DoLoad);
			Closed += new EventHandler(DoClosed);

			StartPosition = FormStartPosition.Manual;
		}

		private void DoLoad(object sender, EventArgs e)
		{
			try
			{
				string [] ss = _selFolder.Split(_tvd.PathSeparator.ToCharArray()[0]);
				TreeNode tn = null;
				foreach(TreeNode tt in _tvd.Nodes)
				{
					if (tt.Text.Equals(ss[0]+_tvd.PathSeparator))
					{
						_tvd.SelectedNode = tt;
						break;
					}
				}
				for (int ii=2; ii<ss.Length; ii++)
				{
					tn = _tvd.SelectedNode;
					foreach(TreeNode tt in tn.Nodes)
					{
						if (tt.Text.Equals(ss[ii]))
						{
							_tvd.SelectedNode = tt;
							break;
						}
					}
				}
				_tvd.SelectedNode.EnsureVisible();
			}
			catch {}

			if (!_ps.IsEmpty)
				Size = _ps;
			if (!_pl.IsEmpty)
				Location = _pl;

		}

		private void DoClosed(object sender, EventArgs e)
		{
			_ps = Size;
			_pl = Location;
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
			this._tvd = new DirectoryTreeView();
			this._cmdSelect = new System.Windows.Forms.Button();
			this._cmdCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _tvd
			// 
			this._tvd.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this._tvd.ImageIndex = -1;
			this._tvd.Name = "_tvd";
			this._tvd.SelectedImageIndex = -1;
			this._tvd.Size = new System.Drawing.Size(296, 276);
			this._tvd.TabIndex = 0;
			// 
			// _cmdSelect
			// 
			this._cmdSelect.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this._cmdSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._cmdSelect.Location = new System.Drawing.Point(64, 284);
			this._cmdSelect.Name = "_cmdSelect";
			this._cmdSelect.Size = new System.Drawing.Size(104, 25);
			this._cmdSelect.TabIndex = 1;
			this._cmdSelect.Text = "Выбрать";
			// 
			// _cmdCancel
			// 
			this._cmdCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this._cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cmdCancel.Location = new System.Drawing.Point(184, 284);
			this._cmdCancel.Name = "_cmdCancel";
			this._cmdCancel.Size = new System.Drawing.Size(104, 25);
			this._cmdCancel.TabIndex = 2;
			this._cmdCancel.Text = "Отменить";
			// 
			// dlgSelectFolder
			// 
			this.AcceptButton = this._cmdSelect;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.CancelButton = this._cmdCancel;
			this.ClientSize = new System.Drawing.Size(296, 316);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this._cmdCancel,
																																	this._cmdSelect,
																																	this._tvd});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "dlgSelectFolder";
			this.Text = "Выбор папки";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
