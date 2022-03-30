using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerGame
{
    public class Room
    {
        private static IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private static IPEndPoint newEndPoint;
        public static Socket room = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        private static int port;
        private static int maxconnexions;
        private static int connexions;

        public static async void StartServer(int lePort, int max)
        {
            if (connexions == max || connexions == 0)
            {
                await Task.Run(() =>
                {
                    port = lePort;
                    maxconnexions = max;

                    //on créer un serveur (une salle) qui récupère les joueurs
                    newEndPoint = new IPEndPoint(ip, port);
                    room.Bind(newEndPoint);
                    room.Listen(maxconnexions);

                    Socket server = room.Accept();
                    connexions++;
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    Socket server = room.Accept();
                    connexions++;
                });
            }
            
            

            //var thread = new Thread(() =>
            //{

            //});
            //thread.Start();
        }
    }
}
