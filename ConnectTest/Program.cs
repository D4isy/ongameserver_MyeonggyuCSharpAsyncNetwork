using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyServerLibCP4;

namespace ConnectTest
{
    class MyReceiver : INetworkReceiver
    {
        public int cnt;
        public ASSOCKDESC sinfo;
        public Dictionary<long, ASSOCKDESC> theSessions = new Dictionary<long, ASSOCKDESC>();

        // addressinfo => TcpClient.Client.RemoteEndPoint
        // 새로운 연결이 들어왔습니다.
        public void notifyRegisterSocket(ASSOCKDESC sockdesc, string addressinfo)
        {
            Console.WriteLine(sockdesc.managedID + " Connected");
            lock (this)
            {
                theSessions.Add(sockdesc.managedID, sockdesc);
            }
        }

        // 소켓연결이 해제되었습니다.
        public void notifyReleaseSocket(ASSOCKDESC sockdesc, ASSocket socket)
        {
            Console.WriteLine(sockdesc.managedID + " Disconnected");
            lock (this)
            {
                theSessions.Remove(sockdesc.managedID);
            }
            sockdesc.theSender.releaseASSOCKDESC(sockdesc);
        }

        // connectSocket에 대한 결과
        //  bSuccess가 false이면 ex가 null이 아닌 개체로 전송됩니다~
        public void notifyConnectingResult(int requestID, ASSOCKDESC sockdesc, bool bSuccess, Exception ex)
        {
            if (true == bSuccess)
            {
                Console.WriteLine("Connected");
                sinfo = sockdesc;
                System.Threading.Interlocked.Increment(ref cnt);
            }
            else
                Console.WriteLine(ex.Message);                  
        }

        public void notifyMessage(ASSOCKDESC sockdesc, int length, byte[] data, int offset)
        {
            //lock (this)
            //{
            // 라인피드가 붙어있는 문자열을 WriteLine하면 출력이 제대로 안된다.
            // 바이트스트림으로 온 유니코드를 유니코드로 다시 맵핑하기 (단지 바이트로 오는것뿐, 코드체계가 바뀐건 아님)
            string s = System.Text.Encoding.Unicode.GetString(data, offset, length);
            Console.Write(s);
            //}
        }        
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1 Gb array
               // byte[] arr = new byte[1024 * 1024 * 1024];
                MyReceiver receiver = new MyReceiver();
                ASIOManager netserver = new ASIOManager(1,receiver, 1024, 2000, 512);
                ASSocket prototype = ASSocket.GetPrototype();

                for (int i = 0; i < 10000; i++)
                {
                    netserver.connectSocket(i + 1, prototype.Clone(), "192.168.0.79", 3210);
                    //netserver.connectSocket(i + 1, prototype.Clone(), "192.168.0.90", 9295);
                }

                Console.ReadLine();
                Console.WriteLine(receiver.cnt);
                Console.ReadLine();

                System.Threading.Thread.Sleep(5000);

                int cnt = 1;
                while (true)
                {
                    Packet p = new Packet();

                    int size = 4 + (8 * cnt);

                    p.WriteInt(size);

                    for (int i = 0; i < cnt; ++i)
                    {
                        long d = 1;
                        p.WriteLong(d);
                    }
                    receiver.sinfo.theSender.postingSend(receiver.sinfo, p.Position, p.Buffer);
                    cnt++;
                    if (cnt > 11)
                        cnt = 1;
                    System.Threading.Thread.Sleep(10);
                }

                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
