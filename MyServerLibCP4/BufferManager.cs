using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace MyServerLibCP4
{
    /// <summary>
    /// This class creates a single large buffer which can be divided up and assigned to SocketAsyncEventArgs objects for use
    /// with each socket I/O operation.  This enables bufffers to be easily reused and gaurds against fragmenting heap memory.
    /// 스레드 세이프 하지 않다
    /// The operations exposed on the BufferManager class are not thread safe.
    /// </summary>
    class BufferManager
    {
        int NumBytes;                 // the total number of bytes controlled by the buffer pool
        byte[] Buffer;                // the underlying byte array maintained by the Buffer Manager
        Stack<int> FreeIndexPool;     // 
        int CurrentIndex;
        int BufferSize;

        public BufferManager(int totalBytes, int bufferSize)
        {
            NumBytes = totalBytes;
            CurrentIndex = 0;
            BufferSize = bufferSize;
            FreeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// Allocates buffer space used by the buffer pool
        /// </summary>
        public void InitBuffer()
        {
            // create one big large buffer and divide that out to each SocketAsyncEventArg object
            Buffer = new byte[NumBytes];
        }

        /// <summary>
        /// Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object
        /// </summary>
        /// <returns>true if the buffer was successfully set, else false</returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {

            if (FreeIndexPool.Count > 0)
            {
                args.SetBuffer(Buffer, FreeIndexPool.Pop(), BufferSize);
            }
            else
            {
                if ((NumBytes - BufferSize) < CurrentIndex)
                {
                    return false;
                }
                args.SetBuffer(Buffer, CurrentIndex, BufferSize);
                CurrentIndex += BufferSize;
            }
            return true;
        }

        /// <summary>
        /// Removes the buffer from a SocketAsyncEventArg object.  This frees the buffer back to the 
        /// buffer pool
        /// </summary>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            FreeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
            args.Dispose();
        }


    }
}
