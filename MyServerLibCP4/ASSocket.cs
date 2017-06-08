using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace MyServerLibCP4
{   
    
    public class AsyncSocket
    {
        // 아래 static멤버들은 static함수가 호출되면 순서되로 new 된다.
        static UniqueNumberAllocator UIDAllocator = new UniqueNumberAllocator();

        //TODO: 이건 필요 없을 수 있음
        static AsyncSocket ASSocketPrototype = new AsyncSocket();

        public string RemoteAddress = "";

        public TcpClient TCPClient = null;

        int IOCount = 0;

        public bool IsSending = false;


        public static AsyncSocket GetPrototype()
        {
            return ASSocketPrototype;            
        }

        public AsyncSocket()
        {
            IOCount = 0;
            Description = new ASSOCKDESC();
            Description.NetSender = null;
            Description.ManagedID = RetrieveUID();
            Description.CreateTick = System.Environment.TickCount;
        }

        public virtual AsyncSocket Clone()
        {
            return new AsyncSocket();
        }

        public static void InitUIDAllocator(Int64 startNumber, Int64 maxCount)
        {
            UIDAllocator.Reset(startNumber, maxCount);
        }

        public ASSOCKDESC Description { get; set; }

        public static long RetrieveUID()
        {
            // .net 4.0에선 enter를 try안에서 하고, 성공여부를 체크해서 finally에서 exit하라고 하는데....
            long id = 0;
            bool acc = false;

            try
            {
                System.Threading.Monitor.Enter(UIDAllocator, ref acc);
                id = UIDAllocator.Retrieve();
            }
            finally
            {
                // 2012-9-28 finally 확인, try안에서 예외가 발생하면 이 문맥에선 catch가 없기때문에, 이 finally가 실행되고 난뒤 예외가 상위호출측으로 재전파된다.
                // 예외가 발생하면 이 블럭이 실행되고, 다시 전파되나? 그렇지 않으면 0이 반환되어 심각한 오류가 발생
                // 일반적으로 이 함수에선 예외가 발생하지 않는다.
                if (acc)
                {
                    System.Threading.Monitor.Exit(UIDAllocator);
                }
            }

            return id;
        }

        public static void ReleaseUID(long id)
        {
            bool acc = false;

            try
            {
                System.Threading.Monitor.Enter(UIDAllocator, ref acc);
                UIDAllocator.Release(id);
            }
            finally
            {
                // 일반적으로 이 함수에선 예외가 발생하지 않는다.
                if (acc)
                {
                    System.Threading.Monitor.Exit(UIDAllocator);
                }
            }
        } 

        // 이 함수는 객체풀링 등을 할때 상속받은 계층에서 사용하시길~
        protected void Finit()
        {
            RemoteAddress = "";
            TCPClient = null;
            Description.NetSender = null;
        }
        
        public void SetTcpClient(TcpClient client)
        {
            if (null != TCPClient)
            {
                // temp code 예외를 던져야한다!?
                return;
            }

            TCPClient = client;
        }

        public int ExitIO()
        {
            return System.Threading.Interlocked.Decrement(ref IOCount);
        }

        public int EnterIO()
        {
            return System.Threading.Interlocked.Increment(ref IOCount);
        }

        // 받은 내용을 어떻게 처리할지는 이 함수를 재정의해서 처리하시오!
        public virtual void HandleReceived(int length, byte[] data, int offset, INetworkReceiver receiver)
        {
            // todo 보통 아래 receiver.notifyMessage 함수에서 프로토콜에 맞는 객체를 생성하면 될것이다.
            //  생성은 쓰레드제어없이 가능하게하고, enqueuing을 쓰레드제어하면 된다.
            // 객체파괴가 어떻게 일어날지 모르니, desc는 값을 복사해서 사용하자.                
            ASSOCKDESC desc = new ASSOCKDESC();
            desc.CreateTick = Description.CreateTick;
            desc.ManagedID = Description.ManagedID;
            desc.NetSender = Description.NetSender;
            receiver.NotifyMessage(desc, length, data, offset);
        }
    }
}
