using System;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CommandAS.Tools
{
	[Guid("68FC47AF-C9C5-4fd4-A5E7-DE4341B5E567")]
	public interface INavClient
	{
		object State { get; set; }
		bool IsEqual (object aState1, object aState2);
	}

	/// <summary>
	/// 
	/// </summary>
	public class StateHistoryNavigateManager
	{
		protected Stack				mStackB;
		protected Stack				mStackF;
		protected object			mCurState;


		public bool						pCanBackward
		{
			get { return mStackB.Count > 0; }
		}

		public bool						pCanForward
		{
			get { return mStackF.Count > 0; }
		}

		public virtual object	pCurrentState
		{
			get { return mCurState; }
			set
			{
				if (mCurState!= null)
				{
					if (mCurState.Equals(value))
						return;
					mStackB.Push(mCurState);
					mStackF.Clear();
				}

				mCurState = value;
			}
		}

		public StateHistoryNavigateManager() : this (32) {}
		public StateHistoryNavigateManager(int aStackCapacity)
		{
			mStackB = new Stack(aStackCapacity);
			mStackF = new Stack(aStackCapacity);
			mCurState = null;
		}

		public virtual void GoToBackward()
		{
			if (mStackB.Count > 0)
			{
				mStackF.Push(mCurState);
				mCurState = mStackB.Pop();
			}
			else
				mCurState = null;
		}

		public virtual void GoToForward()
		{
			if (mStackF.Count > 0)
			{
				mStackB.Push(mCurState);
				mCurState = mStackF.Pop();
			}
			else
				mCurState = null;
		}
	}

	public class StateHistoryNMAppIdle :  StateHistoryNavigateManager
	{
		protected INavClient	mClient;
		protected bool				mWhileNavigate;

		public override object pCurrentState
		{
			get { return mCurState; }
		}

		public StateHistoryNMAppIdle(INavClient aClient) : this (aClient, 32) {}
		public StateHistoryNMAppIdle(INavClient aClient, int aStackCapacity)
			: base (aStackCapacity)
		{
			mClient = aClient;
			mWhileNavigate = false;
			Application.Idle +=new EventHandler(Application_Idle);
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			if (mWhileNavigate)
			{
				mWhileNavigate = false;
				return;
			}

			if (mCurState == null)
				mCurState = mClient.State;
			else
			{
				object stateNow = mClient.State;
				if (!mClient.IsEqual(stateNow, mCurState))
				{
					mStackB.Push(mCurState);
					mCurState = stateNow;
					mStackF.Clear();
				}
			}
		}

		public override void GoToBackward()
		{
			base.GoToBackward();
			mWhileNavigate = true;
			mClient.State = mCurState;
		}

		public override void GoToForward()
		{
			base.GoToForward();
			mWhileNavigate = true;
			mClient.State = mCurState;
		}	
	}

}
