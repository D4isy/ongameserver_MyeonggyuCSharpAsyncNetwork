using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace MyServerLibCP4
{
    // 응용계층에서 ASNetService를 생성하긴 해야하지만, 그렇다고 ASNetService를 직접 가져다 쓰면 안된다.    
    public interface INetworkSender
    {
        int postingSend(ASSOCKDESC sockdesc, int length, byte[] data);

        int disconnectSocket(ASSOCKDESC sockdesc);

        //int connectSocket(int reqID, ASSocket socket, string ipaddress, int port);

        int releaseASSOCKDESC(ASSOCKDESC sockdesc);

        // c#에선 TcpListener.AcceptTcpClient가 반환하는 TcpClient객체레퍼런스를 넘겨줘야한다.
        //  즉 c++에선 int 값을 넘겨줬지만, c#에선 TcpClient 이 객체를 기본 베이스로 사용해야 한다.
        //int registerSocket(TcpClient acceptedclient, ASSocket prototype);
    }
}
