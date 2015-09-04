using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CommandAS.Tools
{
	[Guid("621A3C48-34D8-47af-817C-1819A225BB78")]
	public interface IStateItem
	{
		DataRowState	pState					{set; get;}
	}

	[Guid("64E15718-B4F2-4091-B37F-693E1FCE2C05")]
	public interface IImageItem : IStateItem
	{
		int						pPlace					{set; get;}
		int						pCode						{set; get;}
		Image					pImage					{set; get;}
		string				pText						{set; get;}
	}

	public class PicItem : IImageItem
	{
		//protected int						mPlace;
		//protected int						mCode;
		protected PlaceCode			mPC;
		protected Image					mImage;
		protected string				mText;
		protected DataRowState	mState;

		public PlaceCode				pPC
		{
			get { return mPC;  }
			set { mPC = value; }
		}
		
		public int							pPlace
		{
			get { return mPC.place; }
			set { mPC.place = value;}
		}
		public int							pCode
		{
			get { return mPC.code; }
			set { mPC.code = value;}
		}

		/// <summary>
		/// образ
		/// </summary>
		public  Image						pImage
		{
			get { return mImage; }
			set { mImage = value;}
		}
		/// <summary>
		/// текстовый комментарий к образу
		/// </summary>
		public string						pText
		{
			get { return mText; }
			set { mText = value;}
		}
		/// <summary>
		/// состояние записи см. DataRowState
		/// </summary>
		public DataRowState			pState
		{
			get { return mState; }
			set { mState = value;}
		}


		public PicItem()
		{
			mPC			= PlaceCode.Empty;
			mImage	= null;
			mText		= string.Empty;
			mState	= DataRowState.Unchanged;
		}
	}

	/// <summary>
	/// Коллекция любых объектов на основе ArrayList 
	/// с отслеживанием состояния (DataRowState) объекта.
	/// </summary>
	public class StateCollection : ArrayList
	{
		#region PROPERTY:

		private int							mCount;

		protected int						mCurrentIndex;
		protected Error					mErr;

		/// <summary>
		/// Устанавливает/возвращает текущий индекс с учетом удаленных элементов.
		/// </summary>
		public int														pCurrentIndex
		{
			set
			{
				//if (value >= Count)
				//	value = Count-1;

				int nci = -1;
				for (mCurrentIndex = 0; mCurrentIndex < Count && value >=0 ; mCurrentIndex++)
					if (((IStateItem)this[mCurrentIndex]).pState != DataRowState.Deleted)
					{
						value --;
						nci = mCurrentIndex;
					}
				mCurrentIndex = nci;
			}
			get
			{
				int ret = -1;
				for (int ii = 0; ii <= mCurrentIndex; ii++)
				{
					if (((IStateItem)this[ii]).pState != DataRowState.Deleted)
						ret++;
				}
				return ret;
			}
		}

		/// <summary>
		/// Получить текущий элемент [интерфейс IStateItem] коллекции.
		/// </summary>
		public virtual IStateItem							pCurrentItem
		{
			get 
			{ 
				object ret = null;
				if (mCurrentIndex != -1)
					ret = this[mCurrentIndex]; 
				return (ret as IStateItem);
			}
		}

		/// <summary>
		/// Возвращает кол-во элементов коллекции с учетом удаленных.
		/// </summary>
		public int														pCount
		{
			get { return mCount; }
		}

		/// <summary>
    /// Возвращаем массив актуальных объектов
		/// </summary>
		public virtual object[] objects
		{
			get
			{
				object[] ret=new object[pCount];
				int j=0;
				for (int i=0;i<pCount;i++)
				{
					if (((IStateItem)this[i]).pState!=DataRowState.Deleted)
					{
						ret[j++]=this[i];
					}
				}
				if (j>0)
				{
//					ret.CopyTo(ret,j);
				}
				return ret;
			}
		}

		#endregion PROPERTY.

		public StateCollection() : base(){ Init(); }
		public StateCollection(int capacity) : base(capacity){ Init(); }
		public StateCollection(System.Collections.ICollection c):base(c){ Init(); }

		private void Init()
		{
			mErr					= new Error();
			mCount				= 0;
			mCurrentIndex = -1;
		}

		public void GoFirst()
		{
			for (mCurrentIndex = 0; 
				mCurrentIndex<Count-1 && ((IStateItem)this[mCurrentIndex]).pState == DataRowState.Deleted;
				mCurrentIndex++);
		}
		//это начальная позиция ?
		public bool IsFirst
		{
			get
			{
				bool ret=(mCurrentIndex<1);
				return ret;
			}
		}
		//это последняя позиция ?
		public bool IsLast
		{
			get
			{
				bool ret=(mCurrentIndex==(mCount-1));
				if (mCount==0)
					ret=true;
				return ret;
			}
		}
		public void GoPrev()
		{
			int ii = mCurrentIndex-1;
			for (; ii>0 && ((IStateItem)this[ii]).pState == DataRowState.Deleted; ii--);

			if (ii>=0 && ((IStateItem)this[ii]).pState != DataRowState.Deleted)
				mCurrentIndex = ii;
		}
		public void GoNext()
		{
			int ii = mCurrentIndex+1;
			for (;ii<Count-1 && ((IStateItem)this[ii]).pState == DataRowState.Deleted; ii++);

			if (ii<Count && ((IStateItem)this[ii]).pState != DataRowState.Deleted && mCurrentIndex != ii)
				mCurrentIndex = ii;
		}
		public void GoLast()
		{
			for (mCurrentIndex = Count-1; 
				mCurrentIndex>0 && ((IStateItem)this[mCurrentIndex]).pState == DataRowState.Deleted; 
				mCurrentIndex--);
		}
		public void RotateForward()
		{
			int iPrev = mCurrentIndex;
			GoNext();
			if (iPrev == mCurrentIndex)
				GoFirst();
		}
		public void RotateBackward()
		{
			int iPrev = mCurrentIndex;
			GoPrev();
			if (iPrev == mCurrentIndex)
				GoLast();
		}
		public void AcceptChanges()
		{
			/*foreach (IStateItem isi in this)
			{
				if (isi.pState == DataRowState.Deleted)
          base.RemoveAt(IndexOf(isi));
				else
					isi.pState = DataRowState.Unchanged;
			}*/
      // MaryM 07.10.2009
      for (int ii = this.Count-1; ii >= 0; ii--)
      {
        IStateItem isi = (IStateItem)this[ii];
        if (isi.pState == DataRowState.Deleted)
          base.RemoveAt(ii);
        else
          isi.pState = DataRowState.Unchanged;
      }
		}

    public void RemoveCurrent()
		{
			if (mCurrentIndex != -1)
				RemoveAt(mCurrentIndex);
		}

		public void MoveForward()
		{
			if (mCurrentIndex < (mCount-1) && mCurrentIndex != -1)
			{
				object co = this[mCurrentIndex];
				//base.Remove(co);
				base.RemoveAt(IndexOf(co));
				base.Insert(++mCurrentIndex,co);
				IStateItem iii = (IStateItem)co;
				if (iii != null)
					iii.pState = DataRowState.Modified;
				iii = (IStateItem)this[mCurrentIndex-1];
				if (iii != null)
					iii.pState = DataRowState.Modified;
			}
		}

		public void MoveBackward()
		{
			if (mCurrentIndex > 0)
			{
				object co = this[mCurrentIndex];
				//base.Remove(co);
				base.RemoveAt(IndexOf(co));
				base.Insert(--mCurrentIndex,co);
				IStateItem iii = (IStateItem)co;
				if (iii != null)
					iii.pState = DataRowState.Modified;
				iii = (IStateItem)this[mCurrentIndex+1];
				if (iii != null)
					iii.pState = DataRowState.Modified;
			}
		}

		public override int Add(object obj)
		{
			try
			{
				IStateItem iii = (IStateItem)obj;
				if (iii != null)
				{
					base.Add(obj);
					iii.pState = DataRowState.Added;
					GoLast();
					mCount++;
				}
			}
			catch {};
			return mCount-1;
		}

		public override void Remove(object obj)
		{
			try
			{
				IStateItem iii = (IStateItem)obj;
				this.RemoveAt(IndexOf(obj));
			}
			catch {}
		}

		public override void RemoveAt(int ind)
		{
			int prevIndex  = pCurrentIndex; 

			IStateItem iii = ((IStateItem)this[ind]);
			if (iii.pState == DataRowState.Added)
				base.RemoveAt(ind);
			else
				iii.pState = DataRowState.Deleted;
			mCount--;
			pCurrentIndex = prevIndex;
		}

		public override void Clear()
		{
			base.Clear();
			Init();
		}


		//public virtual IStateItem NewImageItem ()
		//{
		//	return null;
		//}

		/// <summary>
		/// Мы поддерживаем клонирование !!!
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			StateCollection oc=(StateCollection) base.MemberwiseClone();

			oc.mCurrentIndex=mCurrentIndex;
			oc.mCount				=mCount;

			return oc;
		}
	}

	public class ImageCollection : StateCollection
	{
		/// <summary>
		/// Получить текущий элемент [интерфейс IImageItem] коллекции.
		/// </summary>
		public new IImageItem							pCurrentItem
		{
			get 
			{ 
				object ret = null;
				if (mCurrentIndex != -1 && mCurrentIndex < Count)
					ret = this[mCurrentIndex]; 
				return (ret as IImageItem);
			}
		}

		/*
		public override void Remove(object obj)
		{
			try
			{
				IImageItem iii = (IImageItem)obj;
				base.Remove(obj);
			}
			catch {}
		}

		public override int Add(object obj)
		{
			try
			{
				IImageItem iii = (IImageItem)obj;
				base.Add(obj);
			}
			catch {};
			return pCount-1;
		}
		*/

		/// <summary>
		/// Возвращаем все картинки из коллекции
		/// </summary>
		public Image[] Images
		{
			get
			{
				int i=0;
				Image[] ret=new Image[this.Count];
				foreach(object obj in this)
				{
					IImageItem ii=obj as IImageItem;
					if (ii!=null)
						ret[i++]=ii.pImage;
				}
				return ret;
			}
		}
		public virtual IImageItem NewImageItem ()
		{
			return null;
		}
	}
}
