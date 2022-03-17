using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Client
    {
        public void Connect()
        {
            IPAddress ip = new IPAddress(new Byte[] { 127, 0, 0, 1 });
            IPEndPoint endPoint = new IPEndPoint(ip, 1234);
            try
            {
                Socket socket = new Socket(ip.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                int bytesSent = socket.Send(msg);
                Console.WriteLine("Sent: " + bytesSent + " bytes.");
                //socket.Shutdown(SocketShutdown.Both);
                //socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }



        
    }
}
