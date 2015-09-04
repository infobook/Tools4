using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Globalization;
using CommandAS.Tools.WinAPI;


namespace CommandAS.Tools.Controls
{
	/// <summary>
	/// Summary description for UserControl1.
	///	These are the special characters used in the first field of the mask:

	///	Character	Meaning in mask

	///	!	If a ! character appears in the mask, optional characters are represented in the 
	///		text as leading blanks. If a ! character is not present, optional characters are 
	///		represented in the text as trailing blanks.
	///	>	If a > character appears in the mask, all characters that follow are in uppercase 
	///		until the end of the mask or until a < character is encountered.
	///	<	If a < character appears in the mask, all characters that follow are in lowercase 
	///		until the end of the mask or until a > character is encountered.

	///	<>	If these two characters appear together in a mask, no case checking is done and 
	///		the data is formatted with the case the user uses to enter the data.
	///	\	The character that follows a \ character is a literal character. Use this character 
	///		to use any of the mask special characters as a literal in the data.
	///	L	The L character requires an alphabetic character only in this position. 
	///		For the US, this is A-Z, a-z.
	///	l	The l character permits only an alphabetic character in this position, 
	///		but doesn't require it.

	///	A	The A character requires an alphanumeric character only in this position. 
	///		For the US, this is A-Z, a-z, 0-9.
	///	a	The a character permits an alphanumeric character in this position, 
	///		but doesn't require it.
	///	C	The C character requires an arbitrary character in this position.
	///	c	The c character permits an arbitrary character in this position, 
	///		but doesn't require it.
	///	0	The 0 character requires a numeric character only in this position.
	///	9	The 9 character permits a numeric character in this position, 
	///		but doesn't require it.

	///	#	The # character permits a numeric character or a plus or minus sign in this position, 
	///		but doesn't require it.
	///	:	The : character is used to separate hours, minutes, and seconds in times. 
	///		If the character that separates hours, minutes, and seconds is different in the 
	///		regional settings of the Control Panel utility on your computer system, 
	///		that character is used instead.
	///	/	The / character is used to separate months, days, and years in dates. 
	///		If the character that separates months, days, and years is different in the regional 
	///		settings of the Control Panel utility on your computer system, 
	///		that character is used instead.
	///	_	The _ character automatically inserts spaces into the text. When the user enters 
	///		characters in the field, the cursor skips the _ character.
	/// </summary>

	//[ControlStylesAttribute(ControlStyles.Selectable)]
	public class MaskEdit : System.Windows.Forms.TextBox
	{
		private const int MC_LITERAL				=  1;
		private const int MC_REQUERED_ALPHABETIC	=  2;
		private const int MC_ALPHABETIC				=  3;
		private const int MC_REQUERED_ALPHANUMERIC	=  4;
		private const int MC_ALPHANUMERIC			=  5;
		private const int MC_REQUERED_ARBITRARY		=  6;
		private const int MC_ARBITRARY				=  7;
		private const int MC_REQUERED_NUMERIC		=  8;
		private const int MC_NUMERIC				=  9;
		private const int MC_SIGNED_NUMERIC			= 10;
		private const int MC_DECIMAL_SEPARATOR		= 11;
		private const int MC_TIME_SEPARATOR			= 12;
		private const int MC_DATE_SEPARATOR			= 13;

		private class TextNotValidException: System.Exception
		{
			public TextNotValidException()
			{
				//Exception("The value entered into MaskEdit control does not correspond the mask");
			}
		}


		private enum CharCase { ccDefault, ccToUpper, ccToLower }
		private struct MaskItem
		{
			public int Kind;
			public char Simbol;
			public char OldSimbol;
			public CharCase CharCase;
			public MaskItem(int kind, CharCase charCase) 
			{ 
				Kind = kind; 
				Simbol = '\0'; 
				OldSimbol = '\0'; 
				CharCase = charCase;
			}
			public MaskItem(int kind, CharCase charCase, char simbol) 
			{ 
				Kind = kind; 
				Simbol = simbol; 
				OldSimbol = simbol; 
				CharCase = charCase;
			}
			public bool AcceptChar(char charCode)
			{   
				switch (Kind)
				{
					case MC_LITERAL:
					case MC_DECIMAL_SEPARATOR:		
					case MC_TIME_SEPARATOR:		
					case MC_DATE_SEPARATOR:		
						return false;
					case MC_REQUERED_ALPHABETIC:	
					case MC_ALPHABETIC: 			
						return char.IsLetter(charCode);
					case MC_REQUERED_ALPHANUMERIC:	
					case MC_ALPHANUMERIC:		    
						return char.IsLetterOrDigit(charCode);
					case MC_REQUERED_ARBITRARY:		
					case MC_ARBITRARY:				
						return char.IsLetterOrDigit(charCode) || char.IsPunctuation(charCode) 
							|| char.IsSymbol(charCode) 	|| char.IsSeparator(charCode);
					case MC_REQUERED_NUMERIC:		
					case MC_NUMERIC:				
						return char.IsDigit(charCode);
					case MC_SIGNED_NUMERIC:			
						return char.IsDigit(charCode) || (charCode == '-') || (charCode == '+');
					default: 
						return false;
				}
			}
			public bool IsStaticSimbol()
			{
				return(Kind == MC_LITERAL 
					|| Kind == MC_DECIMAL_SEPARATOR
					|| Kind == MC_TIME_SEPARATOR
					|| Kind == MC_DATE_SEPARATOR);
			}
			public bool IsRequired()
			{
				return(Kind == MC_REQUERED_ALPHABETIC 
					|| Kind == MC_REQUERED_ALPHANUMERIC
					|| Kind == MC_REQUERED_ARBITRARY
					|| Kind == MC_REQUERED_NUMERIC);
			}
			public bool IsValid()
			{
				if (IsStaticSimbol()) 
					return true; 
				else if (Simbol == '\0') 
					return !IsRequired(); else 
					return AcceptChar(Simbol);
			}
		}
		private ArrayList m_Mask = new ArrayList();
		private int m_first		=  0; // first nonliteral mask element
		private int m_last		= -1; // last  nonliteral mask element
		private int m_CaretPos	=  0;
		private bool m_InShiftState = false;

		private Char m_EmptyChar = '_';
		private Char m_BlankChar = ' ';
		private	string m_EditMask = "";
		private	string m_DisplayFormat = "";
		private bool m_IncludeLiterals = true;

		public MaskEdit()
		{
			this.Name = "MaskEdit";
			this.Size = new System.Drawing.Size(100, 20);
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ContextMenu = new ContextMenu();
		}
		protected override void Dispose(bool disposing)
		{
			m_Mask.Clear();
			m_Mask = null;
			base.Dispose( disposing );
		}
		private void InitializeComponent()
		{
		}
		public void Validate()
		{
			for (int i=0; i<m_Mask.Count; i++) 
				if (!((MaskItem)m_Mask[i]).IsValid())	
					throw new TextNotValidException();
		}
		private void TranslateMask(String mask)
		{
			string s;
			char c;
			CharCase charCase = CharCase.ccDefault;
			m_Mask.Clear();
			for(int i=0; i<mask.Length; i++)
			{
  				c = mask[i];
				switch (c)
				{
					case '>': charCase = CharCase.ccToUpper; break;
					case '<': 
						if ((i<mask.Length-1) && (mask[i+1] == '>')) 
							{ charCase = CharCase.ccDefault; i++; } else 
							{ charCase = CharCase.ccToLower; } 
						break;
					case '\\':
						if (i<mask.Length-1) { c = mask[i+1]; i++; }
						m_Mask.Add(new MaskItem(MC_LITERAL, CharCase.ccDefault, c)); 
						break;
					case 'L': m_Mask.Add(new MaskItem(MC_REQUERED_ALPHABETIC, charCase)); break;
					case 'l': m_Mask.Add(new MaskItem(MC_ALPHABETIC, charCase)); break;
					case 'A': m_Mask.Add(new MaskItem(MC_REQUERED_ALPHANUMERIC, charCase)); break;
					case 'a': m_Mask.Add(new MaskItem(MC_ALPHANUMERIC, charCase)); break;
					case 'C': m_Mask.Add(new MaskItem(MC_REQUERED_ARBITRARY, charCase)); break;
					case 'c': m_Mask.Add(new MaskItem(MC_ARBITRARY, charCase)); break;
					case '0': m_Mask.Add(new MaskItem(MC_REQUERED_NUMERIC, charCase)); break;
					case '9': m_Mask.Add(new MaskItem(MC_NUMERIC, charCase)); break;
					case '#': m_Mask.Add(new MaskItem(MC_SIGNED_NUMERIC, charCase)); break;
					case '.': 
						s = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
						for (int j=0; j<s.Length; j++)
							m_Mask.Add(new MaskItem(MC_DECIMAL_SEPARATOR, CharCase.ccDefault, s[j])); 
						break;
					case ':': 
						s = DateTimeFormatInfo.CurrentInfo.TimeSeparator;
						for (int j=0; j<s.Length; j++)
							m_Mask.Add(new MaskItem(MC_TIME_SEPARATOR, CharCase.ccDefault, s[j])); 
						break;
					case '/': 
						s = DateTimeFormatInfo.CurrentInfo.DateSeparator;
						for (int j=0; j<s.Length; j++)
							m_Mask.Add(new MaskItem(MC_DATE_SEPARATOR, CharCase.ccDefault, s[j])); 
						break;
					default:  m_Mask.Add(new MaskItem(MC_LITERAL, CharCase.ccDefault, c)); break;
				}
			}
			bool FirstFound = false;
			m_first = 0;
			m_last  = 0;
			for(int i=0;i<m_Mask.Count;i++)
			{
				MaskItem m = (MaskItem)m_Mask[i];
				if (!m.IsStaticSimbol())
				{
					if (!FirstFound) { m_first = i; FirstFound = true; }
					m_last = i;
				}
			}
		}
		private int First(int pos) // go to the first nonliteral
		{
			return m_first - pos;
		}
		private int Last(int pos) // go to the first nonliteral
		{
			return m_last - pos;
		}
		private int Next(int pos) // go to the next nonliteral
		{
			int nextpos = pos;
			if (nextpos <= m_last)
			{
				nextpos += 1;
				while (nextpos <= m_last 
					&& ((MaskItem)m_Mask[nextpos]).IsStaticSimbol() 
					&& nextpos < m_last) nextpos += 1;
			}
			return nextpos - pos;
		}
		private int Prev(int pos)	// go to the prev nonliteral
		{
			int prevpos = pos;
			if (prevpos > m_first)
			{
				prevpos -= 1;
				while (((MaskItem)m_Mask[prevpos]).IsStaticSimbol()
					&& prevpos > m_first) prevpos -= 1;
			}
			return prevpos - pos;
		}
		private void Cancel()
		{
			for (int i=0; i<m_Mask.Count; i++) 
			{
				MaskItem m = (MaskItem)m_Mask[i];
				m.Simbol = m.OldSimbol;
				m_Mask[i] = m;
			}
			m_CaretPos = 0;
		}
		private void SetCursor(int pos, int length)
		{
			if (pos < 0) pos = 0;
			if (pos > m_Mask.Count) pos = m_Mask.Count;
			if (m_Mask.Count-pos < length) length = m_Mask.Count-pos;
			if ((length == 0 || length == 1) && pos > m_last) 
			{
				Select(pos, 0);
				m_CaretPos = pos;
				return;
			}
			if (length < 0)
				Select(pos+length, -length);
			else if (length == 0)
			  Select(pos, 1); else
			  Select(pos, length);
			m_CaretPos = pos;
		}
		private void ClearSelected()
		{
			if (SelectionStart <= m_last)
			for (int i=SelectionStart; i<SelectionStart+SelectionLength; i++)
			{
				MaskItem m = (MaskItem)m_Mask[i];
				if (!m.IsStaticSimbol())
				{
					m.Simbol = '\0';
					m_Mask[i] = m;
				}
			}
		}
		private string GetDisplayText()
		{ 
			string s = "";
			for (int i=0; i<m_Mask.Count; i++) 
			{
				MaskItem m = (MaskItem)m_Mask[i];
				if (m.Simbol == '\0') 
					s += EmptyChar; else 
					s += m.Simbol; 
			}
			return s;
		}
		private void InternalSetText()
		{
			if (!IsHandleCreated) CreateHandle();
			int ss = SelectionStart;
			int sl = SelectionLength;
			IntPtr p = Marshal.StringToHGlobalAnsi(GetDisplayText());
			try
			{
				SendMessage(Handle, WM.SETTEXT, 0, p.ToInt32());
				SelectionStart = ss;
				SelectionLength = sl;
			}
			finally
			{
				Marshal.FreeHGlobal(p);
			}
			//base.Text = GetDisplayText();
		}
		private void OnWM_LBUTTONUP(ref Message m)
		{
			m_CaretPos = SelectionStart;
			if (SelectionLength == 0)
			{
				SetCursor(SelectionStart, 1);
				if (m_CaretPos < m_Mask.Count)
				{
					MaskItem mi = (MaskItem)m_Mask[m_CaretPos];
					if (mi.Kind == MC_LITERAL)
					{
						int delta = Next(m_CaretPos);
						delta += Prev(m_CaretPos+delta);
						SetCursor(m_CaretPos+delta, 1);
					}
				}
			}
		}
		private bool OnWM_KEYDOWN(ref Message m)
		{
			Keys key = (Keys)m.WParam.ToInt32();
			int delta = 0;
			switch (key)
			{
				case Keys.Up:
				case Keys.Left:  delta = Prev(m_CaretPos);  break;
				case Keys.Down:
				case Keys.Right: delta = Next(m_CaretPos);  break;
				case Keys.Home:  delta = First(m_CaretPos); break;
				case Keys.End:   delta = Last(m_CaretPos);  break;
				case Keys.Escape: 
				{ 
					Cancel();
					InternalSetText();
					Select(0, m_Mask.Count); 
				} return true;
				case Keys.Delete: ClearSelected(); break;
				case Keys.Back:
				{
					if (SelectionLength > 1) 
						ClearSelected();
					else
					{
						delta = Prev(m_CaretPos);
						MaskItem mi = (MaskItem)m_Mask[m_CaretPos+delta];
						mi.Simbol = '\0'; 
						m_Mask[m_CaretPos+delta] = mi;
					}
				} break;
				case Keys.ShiftKey: m_InShiftState = true; return true;
				default: return false;
			}
			base.Text = GetDisplayText();
			if (!m_InShiftState) 
			{
				SetCursor(m_CaretPos+delta, 1);
			}
			else
			{
				int length = SelectionLength;
				if (delta < 0) 
					SetCursor(m_CaretPos+delta, length - delta); else
					SetCursor(m_CaretPos, length + delta);
			}
			return true;
		}
		private void OnWM_KEYCHAR(ref Message m)
		{
			char c = (char)m.WParam.ToInt32();
			if (m_CaretPos > m_last) return;
			MaskItem mi = (MaskItem)m_Mask[m_CaretPos]; 
			if (mi.AcceptChar(c)) 
			{
				ClearSelected();
				switch (mi.CharCase)
				{
					case CharCase.ccDefault: c = c; break;
					case CharCase.ccToLower: c = char.ToLower(c); break;
					case CharCase.ccToUpper: c = char.ToUpper(c); break;
				}
				mi.Simbol = c;
				m_Mask[m_CaretPos] = mi;
				int delta = Next(m_CaretPos);
				InternalSetText();
				SetCursor(m_CaretPos+delta, 1);
				Invalidate();
			}
			else if (c=='.' || c==',')
			{
				for (int i=m_CaretPos; i<m_last; i++)
				{
					mi = (MaskItem)m_Mask[i]; 
					if (mi.Kind == MC_DECIMAL_SEPARATOR)
					{
						m_CaretPos = i-1;
						int delta = Next(m_CaretPos);
						SetCursor(m_CaretPos+delta, 1);
						Invalidate();
					}
				}
			}
		}
		private void OnWM_PAINT(ref Message m)
		{
			StringFormat f = new StringFormat(StringFormatFlags.NoWrap);
			Graphics g = Graphics.FromHwnd(Handle);
			Brush brush = new SolidBrush(BackColor);
			g.FillRectangle(brush, ClientRectangle);
			brush = new SolidBrush(ForeColor);
			if (IsNumber)
			{
				double d = double.Parse(Text);
				f.Alignment = StringAlignment.Far;
				g.DrawString(d.ToString(m_DisplayFormat), Font, brush, ClientRectangle, f);
			}
			else if (IsDateTime)
			{
				DateTime d = DateTime.Parse(Text);
				f.Alignment = StringAlignment.Near;
				g.DrawString(d.ToString(m_DisplayFormat), Font, brush, ClientRectangle, f);
			}
			g.Dispose();
		}
		private void OnWM_SETFOCUS(ref Message m)
		{		
			for (int i=0; i<m_Mask.Count; i++) 
			{
				MaskItem mi = (MaskItem)m_Mask[i];
				mi.OldSimbol = mi.Simbol;
				m_Mask[i] = mi;
			}
			InternalSetText();
			m_CaretPos = m_first;
			Select(0, m_Mask.Count);
		}
		private void OnWM_KILLFOCUS(ref Message m)
		{	
			try
			{
				Validate();
			}
			catch (TextNotValidException)
			{
				MessageBeep(0);
				//SendMessage(Handle, WM_SETFOCUS, 0, 0);
				SetFocus(Handle);
			}
			finally
			{
				Invalidate();
			}
		}
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case WM.LBUTTONUP:
				{
					base.WndProc(ref m);
					OnWM_LBUTTONUP(ref m);
				} 
				break;
				case WM.KEYDOWN: 
				{
					if (!OnWM_KEYDOWN(ref m))
						base.WndProc(ref m); 
				}
				break;
				case WM.KEYUP:
				{
					switch ((Keys)m.WParam.ToInt32())
					{
						case Keys.ShiftKey: m_InShiftState = false; 
						return;
					}
				}
				break;
				case WM.KEYCHAR:
				{
					OnWM_KEYCHAR(ref m);
				}
				break;
				case WM.PAINT:
				{	
					InternalSetText();
					base.WndProc(ref m);
					if (!Focused) OnWM_PAINT(ref m);
				} 
				break;
				case WM.SETFOCUS:
					base.WndProc(ref m);
					OnWM_SETFOCUS(ref m);
				break;
				case WM.KILLFOCUS:
					base.WndProc(ref m);
					OnWM_KILLFOCUS(ref m);
				break;
				default:
				{
					base.WndProc(ref m);
				}
				break;
			}
		}
		[RefreshPropertiesAttribute(RefreshProperties.Repaint)]
		[Description("The edit mask associated with the control")]
		public string EditMask 
		{
			get 
			{ 
				return m_EditMask; 
			} 
			set 
			{ 
				try
				{
					TranslateMask(value); 
					m_EditMask = value; 
					Text = Text; 
				}
				catch 
				{ 
					Text = ""; 
				}
				finally	
				{ 
					Invalidate(); 
				}
			} 
		}
		internal bool ShouldSerializeEditMask()
		{
			return m_EditMask != "";
		}
		internal void ResetEditMask()
		{
			m_EditMask = "";
			m_Mask.Clear();
		}
		[Description("The display mask associated with the control")]
		public String DisplayFormat 
		{ 
			get { return m_DisplayFormat; } 
			set { m_DisplayFormat = value; Invalidate(); } 
		}
		internal bool ShouldSerializeDisplayMask()
		{
			return m_DisplayFormat != "";
		}
		internal void ResetDisplayFormat()
		{
			m_DisplayFormat = "";
		}
		[RefreshPropertiesAttribute(RefreshProperties.Repaint)]
		[Description("Indicates whether literal characters from the mask should be included as part of the text for the edit control.")]
		[DefaultValue(true)]
		public Boolean IncludeLiterals
		{
			get { return m_IncludeLiterals; }
			set { m_IncludeLiterals = value; Invalidate(); }
		}
		[RefreshPropertiesAttribute(RefreshProperties.Repaint)]
		[Description("The character that appears in the edit control for empty then focused (characters that have not been entered)")]
		[DefaultValue('_')]
		public Char EmptyChar
		{
			get { return m_EmptyChar; }
			set { if (value == '\0') m_EmptyChar = '_'; else m_EmptyChar = value; Invalidate(); }
		}
		[RefreshPropertiesAttribute(RefreshProperties.Repaint)]
		[Description("The character that appears in the edit control for blanks (characters that have not been entered)")]
		[DefaultValue(' ')]
		public Char BlankChar
		{
			get { return m_BlankChar; }
			set { if (value == '\0') m_BlankChar = ' '; else m_BlankChar = value; Invalidate(); }
		}
		public bool IsNumber
		{
			get 
			{ 
				try
				{ 
					string s = Text.Trim(); 
					double.Parse(s); 
					return true;
				}
				catch (FormatException)
				{
					return false;
				}
			}
		}
		public bool IsDateTime
		{
			get 
			{ 
				try
				{ 
					string s = Text.Trim(); 
					DateTime.Parse(s); 
					return true;
				}
				catch (FormatException)
				{
					return false;
				}
			}
		}
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value 
		{ 
			get 
			{    
				string s = Text;
				if (IsNumber)   return double.Parse(s.Trim());
				if (IsDateTime) return DateTime.Parse(s.Trim());
				return s;
			} 
			set 
			{ 
				Text = value.ToString(); 
			}
		}
		[Category("Misc")]
		[RefreshPropertiesAttribute(RefreshProperties.Repaint)]
		public override string Text 
		{ 
			get 
			{ 
				string s = "";
				for (int i=0; i<m_Mask.Count; i++) 
				{
					MaskItem m = (MaskItem)m_Mask[i];
					if (m.Kind == MC_LITERAL)
					{
						if (IncludeLiterals) s += m.Simbol; 
					}
					else if (m.Simbol == '\0') 
						s += BlankChar; else 
						s += m.Simbol; 
				}
				return s;
			}
			set 
			{	
				for (int i=0; i<m_Mask.Count; i++)
				{
					MaskItem m = (MaskItem)m_Mask[i];
					if (!m.IsStaticSimbol())
					{
						m.OldSimbol = m.Simbol;
						m.Simbol = '\0';
						m_Mask[i] = m;
					}
				}
				try
				{
					char c;
					string s;
					int i = 0;
					int pos = 0;
					while (i < m_Mask.Count)
					{
						MaskItem m = (MaskItem)m_Mask[i];
						c = value[pos];
						switch (m.Kind)
						{
							case MC_LITERAL: 
								if (!IncludeLiterals)
									if (c != m.Simbol) 
										throw new TextNotValidException();
									else
										pos++;
								i++;
							break;
							case MC_DECIMAL_SEPARATOR: 
							{
								s = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
								for (int j=0; j<s.Length; j++)
								{
									c = value[pos];
									m = (MaskItem)m_Mask[i];
									if (c != m.Simbol) 
										throw new TextNotValidException();
									i++;
									pos++;
								}
							}
							break;
							case MC_TIME_SEPARATOR: 
							{
								s = DateTimeFormatInfo.CurrentInfo.TimeSeparator;
								for (int j=0; j<s.Length; j++)
								{
									c = value[pos];
									m = (MaskItem)m_Mask[i];
									if (c != m.Simbol) 
										throw new TextNotValidException();
									i++;
									pos++;
								}
							}
							break;
							case MC_DATE_SEPARATOR: 
							{
								s = DateTimeFormatInfo.CurrentInfo.DateSeparator;
								for (int j=0; j<s.Length; j++)
								{
									c = value[pos];
									m = (MaskItem)m_Mask[i];
									if (c != m.Simbol) 
										throw new TextNotValidException();
									i++;
									pos++;
								}
							}
							break;
							default: 
							{
								if (c == BlankChar)
									c = '\0'; 
								else
								{
									if (!m.AcceptChar(c))
										throw new TextNotValidException();
									switch (m.CharCase)
									{
										case CharCase.ccDefault: c = c; break;
										case CharCase.ccToLower: c = char.ToLower(c); break;
										case CharCase.ccToUpper: c = char.ToUpper(c); break;
									}
								}
								m.Simbol = c;
								m_Mask[i] = m;
								i++;
								pos++;
							} 
							break;
						}
					}
					InternalSetText();
					Invalidate();
				}
				catch
				{
					Cancel();
					InternalSetText();
					Invalidate();
					//MessageBox.Show("The value = \""+value+"\" is not valid");
				}
			}
		}

		
		[DllImport("User32.dll")]
		public static extern int SendMessage(
			IntPtr hWnd,    // handle to destination window
			uint Msg,    // message
			int wParam,  // first message parameter
			int lParam   // second message parameter
			);
		[DllImport("User32.dll")]
		public static extern bool MessageBeep(
			uint uType   // sound type
			);
		[DllImport("User32.dll")]
		public static extern int SetFocus(
			IntPtr hWnd   // handle to window
			);	
	}
}
