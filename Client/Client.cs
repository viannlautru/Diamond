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
                Socket socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                byte[] msg = null; ;
                int bytesSent = 0;

                //Reçoit le protocole et le désérialise (2)
                byte[] buffer = new byte[1024];
                int length = socket.Receive(buffer);
                string protocol = Encoding.ASCII.GetString(buffer, 0, length);                

                //Renvoi la réponse du protocole (ProtocolMessageClient) sérialisé (3)

                //Envoi name (3)
                string name = Ressources.Config.GetName();
                msg = Encoding.ASCII.GetBytes(name);
                bytesSent = socket.Send(msg);

                //Envoi password (3)
                string password = Ressources.Config.GetPassword();
                msg = Encoding.ASCII.GetBytes(password);
                bytesSent = socket.Send(msg);

                //Reçoit ID + Port + OK ou KO (6)
                string data = Encoding.ASCII.GetString(buffer, 0, length);
                string ID = data;

                data += Encoding.ASCII.GetString(buffer, 0, length);
                string port = data;

                data += Encoding.ASCII.GetString(buffer, 0, length);
                string OK = data;

                if (OK == "ok")
                {

                }
                else
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }



        
    }
}
