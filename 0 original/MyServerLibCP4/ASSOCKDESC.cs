using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyServerLibCP4
{
    // 이 클래스는 멤버만 존재하는데...
    public class ASSOCKDESC
    {
        public INetworkSender theSender
        {
            get;
            set;
        }

        public long managedID
        {
            get;
            set;
        }

        // 48일 순환되어 manaagedID와 tick이 같은 소켓이 과연 존재할까?
        public int createTick
        {
            get;
            set;
        }

        public ASSOCKDESC()
        {
        }

        public void Serialize(Packet packet)
        {
            packet.WriteLong(managedID);
            packet.WriteInt(createTick);
        }            
    }
}
