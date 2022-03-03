using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Serveur
    {
        public static void Start() { 

             byte[] buffer = new byte[1024];
             string data = null;
             IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 0 });
             IPEndPoint endPoint = new IPEndPoint(ip, 1234);
             Socket listener = new Socket(ip.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
        
    try
    {
        listener.Bind(endPoint);
        listener.Listen(6);
        Socket client = listener.Accept();
        while (true)
        {
        int length = client.Receive(buffer);
                data += Encoding.ASCII.GetString(buffer, 0, length);
        if (data.IndexOf("<EOF>") > -1)
        break;
        }
            Console.WriteLine("Received: " + data);
        client.Shutdown(SocketShutdown.Both);
        client.Close();
        }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
    }
    }
}
