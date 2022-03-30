using DiamonDMain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerGame
{
    public class Start
    {
        private static IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 2 });
        private static IPEndPoint newEndPoint;
        public static Socket room = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        private static int port;
        private static int max_connexions = 2;


        static void StartServer(int thePort, Socket client)
        {
            //on créer un serveur (une salle) qui récupère les joueurs
            newEndPoint = new IPEndPoint(ip, thePort);
            room.Bind(newEndPoint);
            room.Listen(max_connexions);

            room = client.Accept();

            var thread = new Thread(() =>
            {

            });
            thread.Start();

            Console.ReadLine();
        }

        //public static void ReceiveIdPassword(Socket clientConnect)
        //{
            
        //    byte[] buffer = new byte[1024];

        //    //Reçoit ID + password (8)
        //    int length = clientConnect.Receive(buffer);
        //    string data = Encoding.ASCII.GetString(buffer, 0, length);
        //    string playerID = data;

        //    length = clientConnect.Receive(buffer);
        //    data = Encoding.ASCII.GetString(buffer, 0, length);
        //    string passwordClient = data;

        //    //Envoi OK (9)
        //    if (password == passwordClient)
        //    {
        //        SenOKorKO(1, clientConnect);
        //        CreatePlayer(passwordClient, playerID);
        //    }
        //    else
        //    {
        //        SenOKorKO(0, clientConnect);
        //    }
        //}


    }
}
