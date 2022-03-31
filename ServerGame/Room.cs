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
        private static Socket room;
        private static int port;
        private static int maxconnexions;
        public static int connexions;
        

        private static Task createRoom;

        public static async Task<Socket> StartServer(IPEndPoint endPoint, int max)
        {
            newEndPoint = endPoint;
            if (connexions == max || connexions == 0)
            {
                createRoom = Task.Run(() =>
                {
                    maxconnexions = max;

                    //on créer un serveur (une salle) qui récupère les joueurs
                    room  = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    
                    room.Bind(newEndPoint);
                    room.Listen(maxconnexions + 1);
                });
                await createRoom;
            }
            connexions++;
            return room;
        }
    }
}
