using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace MyServerLibCP4
{
    /// <summary>
    /// TODO: 현재 사용하고 있지 않음. 사용하도록 수정하기. 참고 http://lab.gamecodi.com/board/zboard.php?id=GAMECODILAB_Lecture_series&no=61
    /// </summary>
    class SocketAsyncEventArgsPool
    {
        //Stack<SocketAsyncEventArgs> m_Pool;
        ConcurrentBag<SocketAsyncEventArgs> m_Pool = new ConcurrentBag<SocketAsyncEventArgs>();
        
        /// <summary>
        /// Add a SocketAsyncEventArg instance to the pool
        /// </summary>
        /// <param name="item">The SocketAsyncEventArgs instance to add to the pool</param>
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }

            m_Pool.Add(item);
        }

        /// <summary>
        /// Removes a SocketAsyncEventArgs instance from the pool
        /// </summary>
        /// <returns>The object removed from the pool</returns>
        public SocketAsyncEventArgs Pop()
        {
            if(m_Pool.TryTake(out var item) == false)
            {
                return null;
            }

            return item;
        }

        /// <summary>
        /// The number of SocketAsyncEventArgs instances in the pool
        /// </summary>
        public int Count { get { return m_Pool.Count; }
        }

    }
}
