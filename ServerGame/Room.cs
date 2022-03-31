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
        private static string password;
        

        private static Task createRoom;
        public static Dictionary<string, Socket> sockets = new();
        public static Dictionary<string, DiamonDMain.Joueur> joueurs = new Dictionary<string, DiamonDMain.Joueur>();

        public static async Task<Socket> StartServer(IPEndPoint endPoint, int max, string pwd, int timeout)
        {
            newEndPoint = endPoint;
            password = pwd;

            if (connexions == max || connexions == 0)
            {
                createRoom = Task.Run(() =>
                {
                    maxconnexions = max;

                    //on créer un serveur (une salle) qui récupère les joueurs
                    room  = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    room.ExclusiveAddressUse = true;
                    room.ReceiveTimeout = timeout;
                    room.SendTimeout = timeout;
                    room.Bind(newEndPoint);
                    room.Listen(maxconnexions + 1);
                });
                await createRoom;
            }
            connexions++;
            return room;
        }

        public static async Task StartGame(Socket room, int port, string id, string name)
        {
            Task receive = Task.Run(() =>
            {
                Socket client = room.Accept();
                sockets.Add(port.ToString() + id, client);

                //Reçoit id de session (8)
                string idSession = Get(client);

                SendOKorKO(1, client);

                //Reçoit password (8)
                string pwdClient = Get(client);

                //Envoi OK ou KO (9)
                if (pwdClient == password)
                    SendOKorKO(1, client);
                else
                    SendOKorKO(0, client);

                string OK = Get(client);

                //Créer joueur et ajoute dans la liste
                DiamonDMain.Joueur player = new DiamonDMain.Joueur(name, id);
                joueurs.Add(port.ToString() + id, player);

                
                if (maxconnexions == connexions)
                {
                    //Envoi PLAY
                    foreach (var key in sockets.Keys)
                    {
                        Socket model = sockets[key];
                        int lePort = int.Parse(key.Substring(0, 5));
                        if (port == lePort)
                            SendPlay(model);

                        //Envoi instructions
                        SendInstructions(model);

                        //Envoi OK
                        SendOKorKO(1, model);
                    }
                    connexions = 0;
                }
            });
            await receive;
        }

        public static async Task Game()
        {
            Task start = Task.Run(() =>
            {
                DiamonDMain.Partie.StartGame(joueurs);
            });
            await start;
        }

        public static string Get(Socket client)
        {
            byte[] buffer = new byte[1024];
            int length = client.Receive(buffer);
            string data = Encoding.ASCII.GetString(buffer, 0, length);
            return data;
        }

        public static void SendOKorKO(int i, Socket socket)
        {
            try
            {
                if (i == 1)
                    socket.Send(Encoding.ASCII.GetBytes("OK"));
                else
                {
                    socket.Send(Encoding.ASCII.GetBytes("KO"));
                    socket.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendWork(byte[] msg, Socket client)
        {
            if (client.Connected)
            {
                try
                {
                    int bytesSent;
                    if (msg == null)
                        bytesSent = client.Send(Encoding.ASCII.GetBytes("KO"));
                    else
                        bytesSent = client.Send(msg);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void SendPlay(Socket client)
        {
            byte[] msg = Encoding.ASCII.GetBytes("La partie va commencer");
            SendWork(msg, client);
        }

        public static void SendInstructions(Socket client)
        {
            string instructions = @" => Règles du jeu : 4 cartes 
                    - Cartes Trésor : un nombre de diamants aléatoire est à partager équitablement entre les joueurs 
                    encore présents dans la grotte.
                    - Cartes Trophée: la carte Trophée reste sur le chemin de la grotte et rien ne se passe. La carte 
                    ne prendra sa valeur qu’au moment où un joueur quittera la grotte en sa possession.
                    - Cartes Danger : si un danger apparait pour la première fois depuis votre entrée dans la grotte, 
                    il ne se passe rien et l’expédition continue. Par contre, si le même danger est révélé une seconde
                    fois, tous les joueurs encore présents dans la grotte rentrent immédiatement au campement les 
                    mains vides. L’une des deux cartes Danger identiques est retirée du jeu et seule l’autre est 
                    remise dans la pioche de cartes Expédition.
                    - Carte Sortir : vous rentrez au campement. Sur le chemin du retour, ramassez tous les Diamants 
                    restants sur les cartes Trésor. Si plusieurs joueurs sortent en même temps, ils se partagent en 
                    parts égales l’ensemble des Diamants restants. Si plusieurs joueurs sortent en même temps, personne
                    ne prend les cartes Trophées. Par contre, si un joueur est seul à sortir, il prend toutes les 
                    cartes Trophées présentes dans la grotte, qui lui rapportent des Diamants. 
                    Tous les Diamonts récoltées serons ensuite mis dans votre coffre.
                    => Fin de la manche
                    Une expédition prend fin lorsque tous les joueurs sont rentrés au campement ou lorsqu’un 
                    même Danger apparaît pour la deuxième fois dans la grotte.";

            byte[] msg = Encoding.UTF8.GetBytes(instructions);

            SendWork(msg, client);
        }
    }
}
