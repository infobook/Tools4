using System;

namespace CommandAS.Tools.WinAPI
{
	public class WM
	{
		public const int SETFOCUS			= 0x0007;
		public const int KILLFOCUS		= 0x0008;
		public const int SETTEXT			= 0x000C;
		public const int PAINT				= 0x000F;

		public const int KEYDOWN			= 0x0100;
		public const int KEYUP				= 0x0101;
		public const int KEYCHAR			= 0x0102;

		public const int LBUTTONUP		= 0x0202;

		//const int WM_CUT = 0x300;
		//const int WM_COPY = 0x301;
		//const int WM_PASTE = 0x302;
		//const int WM_CLEAR = 0x303;

		private WM(){}
	}
}
