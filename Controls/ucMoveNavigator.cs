using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CommandAS.Tools;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class ucMoveNavigator : System.Windows.Forms.UserControl
	{
		private int															_pos;
		public int															_minVal;
		public int															_maxVal;
		protected System.Windows.Forms.ToolTip	_tt;

		private System.Windows.Forms.Button _cmdFirst;
		private System.Windows.Forms.Button _cmdPrev;
		private System.Windows.Forms.Button _cmdLast;
		private System.Windows.Forms.Button _cmdNext;
		private System.Windows.Forms.TextBox _txt;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public int										pMinValue
		{
			get { return _minVal; }
			set
			{
				_minVal = value;
				//_CommandView();
        pPosition = _pos;
      }
		}
		public int										pMaxValue
		{
			get { return _maxVal; }
			set
			{
				_maxVal = value;
				//_CommandView();
        pPosition = _pos;
			}
		}
		public int										pPosition
		{
			get {return _pos;}
			set
			{
        if (value >= _minVal && value <= _maxVal)
          _pos = value;
        else if (value > _maxVal)
          _pos = _maxVal;
        else
          _pos = _minVal;

        _CommandView();
        _txt.Text = _pos.ToString() + " из " + (_maxVal > 0 ? _maxVal - _minVal + 1 : 0).ToString();
        //_txt.Text = value.ToString() + " из " + (_maxVal - _minVal + 1).ToString();
			}
		}

		public event EvH_MoveNavigator					MoveNavigator;

		public ucMoveNavigator()
		{
			_minVal = 1;
			_maxVal = 1;
			_pos = 1;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			_tt=new ToolTip();
			_tt.SetToolTip(_cmdFirst,"К началу");
			_tt.SetToolTip(_cmdPrev,"К предыдущему");
			_tt.SetToolTip(_cmdNext,"К следующему");
			_tt.SetToolTip(_cmdLast,"В конец");


			_cmdFirst.Tag = eDirection.First;
			_cmdPrev.Tag = eDirection.Prev;
			_cmdNext.Tag = eDirection.Next;
			_cmdLast.Tag = eDirection.Last;
			_txt.Tag = eDirection.Position;
			//_txt.Focus();

			//this.Load += new EventHandler(ucMoveNavigator_Load);
			this.Enter += new EventHandler(ucMoveNavigator_Enter);
		}

		private void DoCommand(object sender, System.EventArgs e)
		{
			eDirection dir = (eDirection)(((Control)sender).Tag);

			switch (dir)
			{
				case eDirection.First:
					pPosition = _minVal;
					break;
				case eDirection.Prev:
					pPosition --;
					break;
				case eDirection.Next:
					pPosition ++;
					break;
				case eDirection.Last:
					pPosition = _maxVal;
					break;
				case eDirection.Position:
					try {	pPosition = Convert.ToInt32(_txt.Text);	}
					catch{}
					break;
			}

			if (MoveNavigator != null)
				MoveNavigator(this, new EvA_MoveNavigator(dir, _pos));

      this.Refresh();
		}

		//private void ucMoveNavigator_Load(object sender, EventArgs e)
		//{
		//	_CommandView();
		//}
		private void ucMoveNavigator_Enter(object sender, EventArgs e)
		{
			_CommandView();
		}

		private void _CommandView()
		{
			_cmdFirst.Enabled = (_pos > _minVal);
			_cmdPrev.Enabled = _cmdFirst.Enabled;
			_cmdLast.Enabled = (_pos < _maxVal);
			_cmdNext.Enabled = _cmdLast.Enabled;
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
			this._cmdFirst = new System.Windows.Forms.Button();
			this._cmdPrev = new System.Windows.Forms.Button();
			this._cmdLast = new System.Windows.Forms.Button();
			this._cmdNext = new System.Windows.Forms.Button();
			this._txt = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _cmdFirst
			// 
			this._cmdFirst.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this._cmdFirst.Location = new System.Drawing.Point(1, 1);
			this._cmdFirst.Name = "_cmdFirst";
			this._cmdFirst.Size = new System.Drawing.Size(24, 26);
			this._cmdFirst.TabIndex = 0;
			this._cmdFirst.Text = "9";
			this._cmdFirst.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this._cmdFirst.Click += new System.EventHandler(this.DoCommand);
			// 
			// _cmdPrev
			// 
			this._cmdPrev.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this._cmdPrev.Location = new System.Drawing.Point(26, 1);
			this._cmdPrev.Name = "_cmdPrev";
			this._cmdPrev.Size = new System.Drawing.Size(24, 26);
			this._cmdPrev.TabIndex = 1;
			this._cmdPrev.Text = "3";
			this._cmdPrev.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this._cmdPrev.Click += new System.EventHandler(this.DoCommand);
			// 
			// _cmdLast
			// 
			this._cmdLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cmdLast.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this._cmdLast.Location = new System.Drawing.Point(144, 1);
			this._cmdLast.Name = "_cmdLast";
			this._cmdLast.Size = new System.Drawing.Size(24, 26);
			this._cmdLast.TabIndex = 4;
			this._cmdLast.Text = ":";
			this._cmdLast.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this._cmdLast.Click += new System.EventHandler(this.DoCommand);
			// 
			// _cmdNext
			// 
			this._cmdNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cmdNext.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this._cmdNext.Location = new System.Drawing.Point(119, 1);
			this._cmdNext.Name = "_cmdNext";
			this._cmdNext.Size = new System.Drawing.Size(24, 26);
			this._cmdNext.TabIndex = 3;
			this._cmdNext.Text = "4";
			this._cmdNext.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this._cmdNext.Click += new System.EventHandler(this.DoCommand);
			// 
			// _txt
			// 
			this._txt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._txt.Location = new System.Drawing.Point(56, 4);
			this._txt.Name = "_txt";
			this._txt.Size = new System.Drawing.Size(56, 22);
			this._txt.TabIndex = 2;
			this._txt.Text = "";
			this._txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this._txt.Validated += new System.EventHandler(this.DoCommand);
			// 
			// ucMoveNavigator
			// 
			this.Controls.Add(this._txt);
			this.Controls.Add(this._cmdNext);
			this.Controls.Add(this._cmdLast);
			this.Controls.Add(this._cmdPrev);
			this.Controls.Add(this._cmdFirst);
			this.Name = "ucMoveNavigator";
			this.Size = new System.Drawing.Size(168, 28);
			this.ResumeLayout(false);

		}
		#endregion
	}

	public enum eDirection
	{
		Position,
		First,
		Prev,
		Next,
		Last
	}

	public delegate void EvH_MoveNavigator(object sender, EvA_MoveNavigator e);

	public class EvA_MoveNavigator: EventArgs 
	{
		private eDirection			_dir;
		private int							_pos;

		public eDirection				pDirection
		{
			get { return _dir; }
		}

		public int							pPosition
		{
			get { return _pos; }
		}

		public EvA_MoveNavigator (eDirection aDirection):this (aDirection, 0) 
		{}
		public EvA_MoveNavigator (eDirection aDirection, int aPos)
		{
			_dir = aDirection;
			_pos = aPos;
		}
	}

}
