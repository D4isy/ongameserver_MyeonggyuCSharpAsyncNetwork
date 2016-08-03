using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

// MyServerLib C# version
// 2012-6-24 1차 버전 완성
//  2012-6-25 성능향상을 위해 매니저락걸리는 소켓그룹을 분산시켰다. ASIOManager가 ASNetService를 여러개 생성
// veruna2k@nate.com (이메일 및 네이트온 메신져)
//  궁금하시거나 버그리포팅?(^^;;)은 위 주소로 부탁드려요~
// 2012-9-11
//  - receiver에거 ASSOCKDESC 전달시 ASSocket의 멤버 레퍼런스가 아닌, 값이 복사된 새로운 ASSOCKDESC 객체로 넘어감 (넷계층에서 원본객체가 파괴 안될수도 있기 때문에~)
// 2012-9-19
//  - bugfix : ResultConnect에서 연결성공후 첫번째 리시빙 요청안하는 버그 수정
//  - bugfix : []연산자로 dictionary 접근시 키에 해당하는 value가 없으면 널반환이 아니라 예외를던진다.

// 1.1 .net 3.5
//  .net 3.5 에 맞게 업그레이드~
//  ASSocket.handleReceived 변경
//  INetworkReceiver.notifyMessage 변경

namespace MyServerLibCP4
{
    public enum MyErrorCode
    {
        None,
        NoSuchSocket,
        OutOfMemory,
        SizeError,
        AlreadyPostConnect,
    }
    // facade격에 해당하는 클래스
    //  응용측에서 인스턴스를 하나 생성해서 관리하세요.
    //  acceptor와 짝을 이루고 싶고, 여러 acceptor를 만들고 싶은 경우가 있기 때문에, 싱글톤으로 안했습니다.
    public class ASIOManager
    {
        public static int Min_NetContainer = 1;
        public static int Max_NetContainer = 10;
        private INetworkReceiver theReceiver;
        private Dictionary<long, ASNetService> theServices = new Dictionary<long, ASNetService>();
        private int maxcnt;
        private int currentid;
        private int iosize; // 한번에 읽기/쓰기 최대 크기
        private int ioframemax; // 풀링할 saea의 최대 수 maxconnect와 multiple을 곱해서
        private BufferManager buffermanager;
        private SocketAsyncEventArgsPool saeaPool;

        public int IOMAXSIZE
        {
            get { return iosize; }
        }

        public SocketAsyncEventArgs RetreiveSAEA()
        {
            lock (this)
            {
                SocketAsyncEventArgs saea = new SocketAsyncEventArgs();
                if (false == buffermanager.SetBuffer(saea))
                    throw new OutOfMemoryException("RetreiveSAEA");
                return saea;

            }
            //return saeaPool.Pop();
        }

        public void ReleaseSAEA(SocketAsyncEventArgs e)
        {
            lock (this)
            {
                buffermanager.FreeBuffer(e);
            }
            //saeaPool.Push(e);
        }

        // io * maxconnect * multiple 만큼 버퍼가 할당됩니다.
        public ASIOManager(int cnt, INetworkReceiver receiver, int io, int maxconnect, int multiple)
        {
            if (Min_NetContainer > cnt) cnt = Min_NetContainer;
            if (Max_NetContainer < cnt) cnt = Max_NetContainer;
            
            maxcnt = cnt;
            theReceiver = receiver;

            for (long index = 1; index <= maxcnt; index++)
            {
                ASNetService theone = new ASNetService(theReceiver, this);
                theone.idx = index;
                theServices.Add(index, theone);
            }

            currentid = 1;

            iosize = io;
            ioframemax = maxconnect * multiple;
            buffermanager = new BufferManager(iosize * ioframemax, iosize);
            buffermanager.InitBuffer();
            saeaPool = new SocketAsyncEventArgsPool(ioframemax);

            //SocketAsyncEventArgs saea;
            //for (int i = 0; i < ioframemax; i++)
            //{
            //    // completed와 usertoken은 실제 사용하는 문맥에서 결정한다.
            //    saea = new SocketAsyncEventArgs();
            //    buffermanager.SetBuffer(saea);
            //    saeaPool.Push(saea);
            //}
        }

        public int connectSocket(int reqID, ASSocket socket, string ipaddress, int port)
        {
            long sel = 1;
            ASNetService selected = theServices[sel];
            return selected.connectSocket(reqID, socket, ipaddress, port);
        }

        internal int registerSocket(TcpClient acceptedclient, ASSocket prototype)
        {
            ASNetService selected = theServices[currentid];
            int ret = selected.registerSocket(acceptedclient, prototype);
            if (maxcnt < ++currentid)
                currentid = 1;
            return ret;
        }
    }
}
