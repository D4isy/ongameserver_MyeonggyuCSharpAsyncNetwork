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

        public string IPAddress { get; set; }

        public int Port { get; set; }

        public eState CurrentState { get; set; }


        TcpListener Listener;
        Thread ListenerThread;
        
        AsyncIOManager ASIOManager;
        
        AsyncSocket Prototype;
        

        public Acceptor(AsyncIOManager asio, AsyncSocket ap, string aipaddress, int aport)
        {
            ASIOManager = asio;
            Prototype = ap;
            IPAddress = aipaddress;
            Port = aport;
            CurrentState = eState.eState_None;
        }

        public void Start()
        {
            if (eState.eState_None != CurrentState)
            {
                return;
            }

            ListenerThread = new Thread(new ThreadStart(DoListen));
            ListenerThread.Start();

            CurrentState = eState.eState_Run;
        }

        void DoListen()
        { 
            try
            {
                if ("0.0.0.0" == IPAddress)
                {
                    Listener = new TcpListener(System.Net.IPAddress.Any, Port);
                }
                else
                {
                    Listener = new TcpListener(System.Net.IPAddress.Parse(IPAddress), Port);
                }

                Listener.Start();

                do
                {                    
                    ASIOManager.registerSocket(Listener.AcceptTcpClient(), Prototype);

                } while (true);
            }
            catch (Exception /*ex*/)
            {
            }
        }
    }
}
