using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace MyServerLibCP4
{   
    
    public class ASSocket
    {
        // 아래 static멤버들은 static함수가 호출되면 순서되로 new 된다.
        private static UniqueLongGenerator uidGen = new UniqueLongGenerator(0, 1);
        private static ASSocket asPrototype = new ASSocket();
        public string remoteAddress = "";
        public TcpClient tcpclient = null;
        private int iocount = 0;
        public bool isSending = false;

        public static ASSocket GetPrototype()
        {
            return asPrototype;            
        }

        public ASSOCKDESC theDESC
        {
            get;
            set;
        }

        public static long retrieveUID()
        {
            // .net 4.0에선 enter를 try안에서 하고, 성공여부를 체크해서 finally에서 exit하라고 하는데....
            long id = 0;
            bool acc = false;
            try
            {
                System.Threading.Monitor.Enter(uidGen, ref acc);
                id = uidGen.retrieve();
            }
            finally
            {
                // 2012-9-28 finally 확인, try안에서 예외가 발생하면 이 문맥에선 catch가 없기때문에, 이 finally가 실행되고 난뒤 예외가 상위호출측으로 재전파된다.
                // 예외가 발생하면 이 블럭이 실행되고, 다시 전파되나? 그렇지 않으면 0이 반환되어 심각한 오류가 발생
                // 일반적으로 이 함수에선 예외가 발생하지 않는다.
                if (acc)
                    System.Threading.Monitor.Exit(uidGen);
            }
            return id;
        }

        public static void releaseUID(long id)
        {
            bool acc = false;
            try
            {
                System.Threading.Monitor.Enter(uidGen, ref acc);
                uidGen.release(id);
            }
            finally
            {                
                // 일반적으로 이 함수에선 예외가 발생하지 않는다.
                if (acc)
                    System.Threading.Monitor.Exit(uidGen);
            }
        } 

        // 이 함수는 객체풀링등을 할때 상속받은 계층에서 사용하시길~
        protected void Finit()
        {
            remoteAddress = "";
            tcpclient = null;
            theDESC.theSender = null;
        }

        public ASSocket()
        {
            iocount = 0;
            theDESC = new ASSOCKDESC();
            theDESC.theSender = null;
            theDESC.managedID = retrieveUID();
            theDESC.createTick = System.Environment.TickCount;            
        }

        public virtual ASSocket Clone()
        {
            return new ASSocket();
        }

        public void SetTcpClient(TcpClient client)
        {
            if (null != tcpclient)
            {
                // temp code 예외를 던져야한다!?
                return;
            }

            tcpclient = client;
        }

        public int exitIO()
        {
            return System.Threading.Interlocked.Decrement(ref iocount);
        }

        public int enterIO()
        {
            return System.Threading.Interlocked.Increment(ref iocount);
        }

        // 받은 내용을 어떻게 처리할지는 이 함수를 재정의해서 처리하시오!
        public virtual void handleReceived(int length, byte[] data, int offset, INetworkReceiver receiver)
        {
            // todo 보통 아래 receiver.notifyMessage 함수에서 프로토콜에 맞는 객체를 생성하면 될것이다.
            //  생성은 쓰레드제어없이 가능하게하고, enqueuing을 쓰레드제어하면 된다.
            // 객체파괴가 어떻게 일어날지 모르니, desc는 값을 복사해서 사용하자.                
            ASSOCKDESC desc = new ASSOCKDESC();
            desc.createTick = theDESC.createTick;
            desc.managedID = theDESC.managedID;
            desc.theSender = theDESC.theSender;
            receiver.notifyMessage(desc, length, data, offset);
        }
    }
}
