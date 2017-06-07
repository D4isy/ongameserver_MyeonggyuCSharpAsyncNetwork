using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace MyServerLibCP4
{    
    public class Acceptor
    {
        public enum eState
        {
            eState_None,
            eState_Run,
        };

        private TcpListener listener;
        private Thread listenerThread;
        //private ASNetService theASIO;
        private ASIOManager theASIO;
        public string ipaddress
        {
            get;
            set;
        }

        public int port
        {
            get;
            set;
        }

        public eState state
        {
            get;
            set;
        }
        private ASSocket prototype;



        public Acceptor(ASIOManager asio, ASSocket ap, string aipaddress, int aport)
        {
            theASIO = asio;
            prototype = ap;
            ipaddress = aipaddress;
            port = aport;
            state = eState.eState_None;
        }

        public void Start()
        {
            if (eState.eState_None != state)
                return;

            listenerThread = new Thread(new ThreadStart(DoListen));
            listenerThread.Start();

            state = eState.eState_Run;
        }

        void DoListen()
        { 
            try
            {
                if ("0.0.0.0" == ipaddress)
                    listener = new TcpListener(System.Net.IPAddress.Any, port);
                else
                    listener = new TcpListener(System.Net.IPAddress.Parse(ipaddress), port);
                listener.Start();

                do
                {                    
                    theASIO.registerSocket(listener.AcceptTcpClient(), prototype);
                } while (true);
            }
            catch (Exception /*ex*/)
            {
            }
        }
    }
}
