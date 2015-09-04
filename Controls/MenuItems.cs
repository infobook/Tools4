using System;
using System.Drawing;
using System.Windows.Forms;

namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class MenuItemColor : MenuItem
	{
		private Color				mColor;
		private string			mColorName;

		public Color				pColor
		{
			get { return mColor;  }
			set 
			{ 
				mColor = value; 
				if (value == Color.Black)
					mColorName = "������";
				else if (value == Color.Blue)
					mColorName = "�����";
				else if (value == Color.Green)
					mColorName = "�������";
				else if (value == Color.Red)
					mColorName = "�������";
				else if (value == Color.Yellow)
					mColorName = "������";
				else
					mColorName = value.Name;
			}
		}

		public MenuItemColor() : this (Color.Empty) {}
		public MenuItemColor(Color aColor) : base()
		{
			pColor = aColor;
			Text = aColor.Name;
			OwnerDraw = true;
		}

		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			e.ItemHeight = 18;
			e.ItemWidth = 100;  // ����� ���������� !!!
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			base.OnDrawItem(e);
			Rectangle rect = e.Bounds;
			rect.X += 1;
			rect.Y += 1;
			rect.Height -= 2;
			rect.Width = rect.Height;
			e.Graphics.FillRectangle(new SolidBrush(mColor == Color.Empty ? e.BackColor : mColor), rect);
			rect = e.Bounds;
			rect.X += (rect.Height+1);
			rect.Y += 1;
			rect.Height -= 2;
			rect.Width -= (rect.Height+1);
			e.Graphics.DrawString(mColorName, new Font(FontFamily.GenericSansSerif, 8),new SolidBrush(Color.Black),rect);
		}
	}
}
