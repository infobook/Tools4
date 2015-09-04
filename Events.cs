using System;
using System.Collections;

namespace CommandAS.Tools
{
  public delegate void EvH_Object(object sender, EvA_Object e);
  public delegate void EvH_FindByLetters(object sender, EvA_FindText e);
	public delegate void EvH_Text(object sender, EvA_Text e);
	public delegate void EvH_PlaceCode(object sender, EvA_PlaceCode e);
	//делегат для передачи сообщений при загрузке (сохранении ?)
	public delegate void EventHandlerNotify(int index,int maxIndex,object obj, ref bool cancel);

  public class EvA_Object : EventArgs
  {
    private Object _obj;

    public Object pObj
    {
      get { return _obj; }
    }

    public EvA_Object(Object aObj)
    {
      _obj = aObj;
    }

    public override string ToString()
    {
      return _obj.ToString();
    }
  }

	public class EvA_FindText: EventArgs 
	{
		private string		mSrcText;
		private string		mDestText;
		public string			pSrcTetx
		{
			get { return mSrcText; }
		}
		public string			pDestTetx
		{
			get { return mDestText; }
			set { mDestText = value; }
		}
		public EvA_FindText (string aSrcText)
		{
			mSrcText = aSrcText;
			mDestText = string.Empty;
		}
	}

	public class EvA_Text: EventArgs 
	{
		private string		_text;
		private bool			_ok;
		public string			pText
		{
			get { return _text; }
			set { _text = value;}
		}
		public bool			pOk
		{
			get { return _ok; }
			set { _ok = value; }
		}
		public EvA_Text () : this (string.Empty) {}
		public EvA_Text (string aText)
		{
			_text = aText;
		}
	}

	public class EvA_CodeText: EventArgs 
	{
		private int				mCode;
		private string		mText;

		public int				pCode
		{
			get { return mCode; }
		}
		public string			pText
		{
			get { return mText; }
		}

		public EvA_CodeText (int aCode, string aText)
		{
			mCode	= aCode;
			mText = aText;
		}

		public override string ToString()
		{
			return mText;
		}
	}

	public class EvA_PlaceCode: EventArgs 
	{
		private PlaceCode			_pc;

		public PlaceCode			pPC
		{
			get { return _pc; }
		}

		public EvA_PlaceCode (PlaceCode aPC)
		{
			_pc	= aPC;
		}

		public override string ToString()
		{
			return _pc.ToString();
		}
	}

	public class EvA_SelectedTreeItem: EventArgs
	{
		private PlaceCode			_pc;
		private string				_text;
		private object				_tag;

		public PlaceCode			pPC
		{
			get { return _pc; }
		}
		public string					pText
		{
			get { return _text;  }
			set { _text = value; }
		}
		public object					pTag
		{
			get { return _tag; }
		}

		public EvA_SelectedTreeItem(PlaceCode aPC, string aText) 
			: this (aPC, aText, null) {}
		public EvA_SelectedTreeItem(PlaceCode aPC, string aText, object aTag)
		{
			_pc		= aPC;
			_text = aText;
			_tag	= aTag;
		}
	}

	public delegate void EvH_ArrayList(object sender, EvA_ArrayList e);

	public class EvA_ArrayList: EventArgs
	{	
		private ArrayList						_al;
		public ArrayList						pAL
		{
			get { return _al; }
		}

		public EvA_ArrayList() : this(null) {}
		public EvA_ArrayList(ArrayList aAL)
		{
			_al = aAL;
		}
	}


}

