using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    class TCPListener
    {
        private static TcpListener tcpListener;
        public TCPListener(IPAddress ip, int port){
            try{
                tcpListener = new TcpListener(ip,port);
            }
            catch (Exception e){
                Console.WriteLine(e.ToString());
            }
        }
    }

}
