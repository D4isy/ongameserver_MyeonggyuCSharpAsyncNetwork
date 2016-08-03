using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyServerLibCP4
{
    // 이 인터페이스는 응용계층에서 구현하고, ASNetSerivce생성시에 구현한 개체를 넘겨줘야 합니다
    public interface INetworkReceiver
    {
        // addressinfo => TcpClient.Client.RemoteEndPoint
        // 새로운 연결이 들어왔습니다.
        //  sockdesc : assock의 멤버가 복사되어 넘어져 온다.
        //  addressinfo : 받은측에서 사용할때 값을 복사해서 사용할것
        void notifyRegisterSocket(ASSOCKDESC sockdesc, string addressinfo);

        // 소켓연결이 해제되었습니다.
        //  sockdesc : assock의 멤버가 복사되어 넘어져 온다.
        //  socket : 객체풀링등을 하고자 한다면, 응용계층에서 할것~
        void notifyReleaseSocket(ASSOCKDESC sockdesc, ASSocket socket);

        // connectSocket에 대한 결과
        //  bSuccess가 false이면 ex가 null이 아닌 개체로 전송됩니다~
        void notifyConnectingResult(int requestID, ASSOCKDESC sockdesc, bool bSuccess, Exception ex);

        void notifyMessage(ASSOCKDESC sockdesc, int length, byte[] data, int offset);
    }
}
