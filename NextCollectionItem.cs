using System;
using System.Collections.Generic;
using System.Text;

namespace CommandAS.Tools
{
  /// <summary>
  /// Class for buildinng various integer (or none) sequences (collection)
  /// according this previous items.
  /// M.Tor
  /// 15.01.2013
  /// </summary>
  public class NextCollectionItem
  {
    private int[] _ss;
    private int _ssLength;
    private int[] _ts;
    private int _tsLength;
    private int _cycleLength;

    /// <summary>
    /// Считать циклом если есть одно или более повторений, в противном случае
    /// считать циклом если нет нарушений
    /// </summary>
    public bool pIsOnlyMoreOneRepeat;

    public int[] pSourceSequenceWOL
    {
      set
      {
        _ss = value;
        //_ssLength = -1;
      }
      get
      {
        return _ss;
      }
    }
    public int pSourceSequenceLength
    {
      set
      {
        //if (value)
        _ssLength = value;
      }
      get { return _ssLength; }
    }
    public int[] pSourceSequence
    {
      set
      {
        _ss = value;
        if (_ss != null)
          _ssLength = _ss.Length;
        else
          _ssLength = 0;
      }
      get
      {
        return _ss;
      }
    }
    public int[] pTargetSequenceWOL
    {
      set
      {
        _ts = value;
      }
    }
    public int pTargetSequenceLength
    {
      set
      {
        _tsLength = value;
      }
      get { return _ssLength; }
    }
    public int[] pTargetSequence
    {
      set
      {
        _ts = value;
        if (_ts != null)
          _tsLength = _ts.Length;
        else
          _tsLength = 0;
      }
    }

    public int pCycleLength
    {
      get { return _cycleLength; }
    }

    public NextCollectionItem()
    {
      pIsOnlyMoreOneRepeat = true;
      Initialize();
    }

    public void Initialize()
    {
      _ss = null;
      _ssLength = 0;
      _ts = null;
      _tsLength = 0;
      _cycleLength = 0;
    }

    /// <summary>
    /// Define cycle (repeated sequences).
    /// Example sequences:
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-
    /// |a|a|a|b|b|a|bIa|a|a|b|b|a|bIa|a|a|b|b
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-
    /// |0|0|0|0|0|0|0|0|0|0|1|1|1|1|1|1|1|1|1
    /// |0|1|2|3|4|5|6|7|8|9|0|1|2|3|4|5|6|7|8
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-
    /// </summary>
    /// <returns>index next item in cycle</returns>
    public int DefineCycle()
    {
      int ii = 0;
      int jj = 0;

      for (int di = 2; di < _ssLength; )
      {

        for (jj = 0; jj < di; jj++)
        {

          if ((ii + di + jj) == _ssLength)
          {
            if (!pIsOnlyMoreOneRepeat)
              _cycleLength = di;
            return jj;
          }

          if (_ss[jj] != _ss[ii + di + jj])
          {
            break;
          }
        }

        if (jj == di)
        {
          _cycleLength = di;
          ii += di;
        }
        else
        {
          di++;
          ii = 0;
          _cycleLength = 0;
        }
      }

      return jj;
    }

    public void ContinueSequences(int[] aTS, int aTSLength)
    {
      pTargetSequenceWOL = aTS;
      pTargetSequenceLength = aTSLength;
      ContinueSequences();
    }

    public void ContinueSequences(int[] aTS)
    {
      pTargetSequence = aTS;
      ContinueSequences();
    }

    public void ContinueSequences()
    {
      if (_cycleLength == 0)
        return;

      int jj = DefineCycle();

      for (int kk = 0; kk < _tsLength; kk++)
      {
        if (jj == _cycleLength)
          jj = 0;

        _ts[kk] = _ss[jj++];
      }
    }

  }
}
