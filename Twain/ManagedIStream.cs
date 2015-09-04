// To compile:
// csc /unsafe /t:library /out:StreamMarshaler.dll StreamMarshaler.cs ComStream.cs ManagedIStream.cs

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

[ClassInterface(ClassInterfaceType.None)]
public class ManagedIStream : IStream, IDisposable
{
  // The managed stream being wrapped
  Stream originalStream;

  // Constructor
  public ManagedIStream(Stream stream)
  {
    if (stream != null)
    {
      originalStream = stream;
    }
    else
    {
      throw new ArgumentNullException("stream");
    }
  }

  // Finalizer
  ~ManagedIStream()
  {
    Close();
  }

  // Property to get original stream object
  public Stream UnderlyingStream
  {
    get 
    { 
      if (originalStream == null)
        throw new ObjectDisposedException("originalStream");

      return originalStream; 
    }
  }

  // Reads a specified number of bytes from the stream object 
  // into memory starting at the current seek pointer
  public void Read(byte [] pv, int cb, IntPtr pcbRead)
  {
    if (originalStream == null)
      throw new ObjectDisposedException("originalStream");

    if (pcbRead == IntPtr.Zero)
    {
      // User isn't interested in how many bytes were read
      originalStream.Read(pv, 0, cb);
    }
    else
    {
      Marshal.WriteInt32(pcbRead, originalStream.Read(pv, 0, cb));
    }
  }

  // Writes a specified number of bytes into the stream object 
  // starting at the current seek pointer
  public void Write(byte [] pv, int cb, IntPtr pcbWritten)
  {
    if (originalStream == null)
      throw new ObjectDisposedException("originalStream");

    if (pcbWritten == IntPtr.Zero)
    {
      // User isn't interested in how many bytes were written
      originalStream.Write(pv, 0, cb);
    }
    else
    {
      long originalPosition = originalStream.Position;
      originalStream.Write(pv, 0, cb);
      Marshal.WriteInt32(pcbWritten, 
        (int)(originalStream.Position - originalPosition));
    }
  }
        
  // Changes the seek pointer to a new location relative to the beginning
  // of the stream, the end of the stream, or the current seek pointer
  public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
  {
    if (originalStream == null)
      throw new ObjectDisposedException("originalStream");

    // The enum values of SeekOrigin match the enum values of
    // STREAM_SEEK, so we can just cast the dwOrigin to a SeekOrigin

    if (plibNewPosition == IntPtr.Zero)
    {
      // User isn't interested in new position
      originalStream.Seek(dlibMove, (SeekOrigin)dwOrigin);
    }
    else
    {
      Marshal.WriteInt64(plibNewPosition, 
        originalStream.Seek(dlibMove, (SeekOrigin)dwOrigin));
    }
  }

  // Changes the size of the stream object
  public void SetSize(long libNewSize)
  {
    if (originalStream == null)
      throw new ObjectDisposedException("originalStream");

    originalStream.SetLength(libNewSize);
  }

  // Copies a specified number of bytes from the current seek pointer
  // in the stream to the current seek pointer in another stream
  public void CopyTo(IStream pstm, long cb, 
    IntPtr pcbRead, IntPtr pcbWritten)
  {
    if (originalStream == null)
      throw new ObjectDisposedException("originalStream");

    byte [] sourceBytes = new byte[cb];
    int currentBytesRead = 0;
    int bytesWritten = 0;

    while (bytesWritten < cb)
    {
      currentBytesRead = 
        originalStream.Read(sourceBytes, 0, (int)(cb - bytesWritten));

      // Has the end of the stream been reached?
      if (currentBytesRead == 0) break;

      // Stream.Write throws an exception if all bytes can't be written
      originalStream.Write(sourceBytes, 0, currentBytesRead);
      bytesWritten += currentBytesRead;
    }

    if (pcbRead != IntPtr.Zero) Marshal.WriteInt64(pcbRead, bytesWritten);
    if (pcbWritten != IntPtr.Zero)
      Marshal.WriteInt64(pcbWritten, bytesWritten);
  }

  // Ensures that any changes made to a stream object open in
  // transacted mode are reflected in the parent storage object
  public void Commit(int grfCommitFlags)
  {
    if (originalStream == null)
      throw new ObjectDisposedException("originalStream");

    originalStream.Flush();
  }

  // Discards all changes that have been made to a transacted 
  // stream since the last IStream.Commit call
  public void Revert()
  {
    throw new NotImplementedException(
      "This stream does not support reverting.");
  }

  // Restricts access to a specified range of bytes in the stream
  public void LockRegion(long libOffset, long cb, int dwLockType)
  {
    throw new COMException("This stream does not support locking.",
      unchecked((int)0x80030001));
  }

  // Removes the access restriction on a range of bytes
  public void UnlockRegion(long libOffset, long cb, int dwLockType)
  {
    throw new COMException("This stream does not support unlocking.",
      unchecked((int)0x80030001));
  }

  // Retrieves the STATSTG structure for this stream
  public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
  {
    if (originalStream == null)
      throw new ObjectDisposedException("originalStream");

    pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG();
    pstatstg.type = 2; // STGTY_STREAM
    pstatstg.cbSize = originalStream.Length;
    pstatstg.grfMode = 2; // STGM_READWRITE;
    pstatstg.grfLocksSupported = 2; // LOCK_EXCLUSIVE
  }

  // Creates a new stream object that references the same bytes as the 
  // original stream but provides a separate seek pointer to those bytes
  public void Clone(out IStream ppstm)
  {
    throw new NotImplementedException("This stream cannot be cloned.");
  }

  // Closes (disposes) the stream
  public void Close()
  {
    if (originalStream != null)
    {
      originalStream.Close();
      originalStream = null;
      GC.SuppressFinalize(this);
    }
  }

  // IDisposable.Dispose
  void IDisposable.Dispose()
  {
    Close();
  }
}