using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ClientTest
{
    class Client
    {
        public void Connect()
        {
            IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
            IPEndPoint endPoint = new IPEndPoint(ip, 1234);            
            try
            {
                Socket socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                int bytesSent = 0;

                //Reçoit le protocole et le désérialise (2)
                byte[] buffer = new byte[1024 * 4];
                int length = socket.Receive(buffer);
                string json = Encoding.ASCII.GetString(buffer, 0, length);
                CheckKO(json, socket);

                DiamonDMain.ProtocolMessageClient? protocol = null;
                if (json != null)
                     protocol = JsonConvert.DeserializeObject<DiamonDMain.ProtocolMessageClient>(json);

                Console.WriteLine("Version ?");
                int? version;
                version = Int32.TryParse(Console.ReadLine(), out var res) ? res : (int?)null;
                //Renvoi la réponse du protocole (ProtocolMessageClient) sérialisé (3)
                if (version != null)
                {
                    int v = (int)version;
                    DiamonDMain.ProtocolMessageClient protocolClient = new(v);
                    json = JsonConvert.SerializeObject(protocolClient);
                    byte[] theMsg = Encoding.ASCII.GetBytes(json);
                    bytesSent = socket.Send(theMsg);
                }
                

                //Demande infos du serveur au client
                Console.WriteLine("Nom serveur :");
                string? name = Console.ReadLine();
                Console.WriteLine("Mdp serveur :");
                string? password = Console.ReadLine();

                if (name != null && password != null)
                {
                    //Envoi name (3)
                    byte[] msg = Encoding.ASCII.GetBytes(name);
                    bytesSent = socket.Send(msg);

                    //Envoi password (3)
                    msg = Encoding.ASCII.GetBytes(password);
                    bytesSent = socket.Send(msg);
                }

                //Reçoit ID + Port + OK ou KO (6)
                length = socket.Receive(buffer);
                string data = Encoding.ASCII.GetString(buffer, 0, length);
                CheckKO(data, socket);
                string ID = data;

                length = socket.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, length);
                CheckKO(data, socket);
                int port = int.Parse(data);

                length = socket.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, length);
                CheckKO(data, socket);
                string OK = data;

                if (OK == "OK")
                {
                    //Client se connecte à la salle (7)
                    socket.Close();
                    RoomConnect(port);

                    //Envoi ID (7)
                    byte[] msg = Encoding.ASCII.GetBytes(ID);
                    bytesSent = socket.Send(msg);

                    //Envoi password (7)
                    if (password == null)
                        Stop(socket);
                    else
                    {
                        msg = Encoding.ASCII.GetBytes(password);
                        bytesSent = socket.Send(msg);
                    }

                    //Reçoit confirmation (10)
                    length = socket.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, length);
                    CheckKO(data, socket);
                    OK = data;

                    if (OK == "KO")
                        Stop(socket);
                }
                //Déconnexion
                else
                    Stop(socket);


            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }

        public void CheckKO(string msg, Socket socket)
        {
            if (msg == "KO")
                Stop(socket);            
        }

        public static void Stop(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            Console.WriteLine("Déconnexion du serveur");
            Console.ReadLine();
            Environment.Exit(1);
        }

        public void RoomConnect(int port)
        {
            //on se connecte à la salle
            IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });            
            IPEndPoint newEndPoint = new IPEndPoint(ip, port);
            Socket room = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            room.Connect(newEndPoint);
            Console.WriteLine("Vous êtes dans une salle.");
        }

        //public void StartRoom()
        //{
        //    IPEndPoint newEndPoint = new IPEndPoint(ip, port);
        //    newSocket.Connect(newEndPoint);
        //    Console.WriteLine("Vous êtes dans une salle.");
        //}




    }
}
