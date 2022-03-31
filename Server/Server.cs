using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Server
{
    public class Server
    {

        //Object Server
        public string name { get; set; }
        public int port { get; set; }
        public string password { get; set; }
        public int maxconnexions { get; set; }
        public int timeout { get; set; }
        public string game1 { get; set; }

        public Server() { }
        public Server(string name, int port, string pwd, int max, int timeout, string game)
        {
            this.name = name;
            this.port = port;
            this.password = pwd;
            this.maxconnexions = max;
            this.timeout = timeout;
            this.game1 = game;
        }
        //

        
        private static IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private static IPEndPoint endPoint;
        private static Socket listener;
        private static int version = 1;
        private static Ressources.Config configChoose;
        private static DiamonDMain.ProtocolMessageServer protocolChoose;
        private static Server serverChoose;
        private static Socket roomCreate;
        public static Dictionary<string, Socket> sockets = new();

        private static DiamonDMain.Partie game;

        //Variables globales
        private static byte[] msgOK = Encoding.ASCII.GetBytes("OK");
        private static byte[] msgKO = Encoding.ASCII.GetBytes("KO");

        private static int connexions;
        private static Dictionary<string, DiamonDMain.Joueur> joueurs = new Dictionary<string, DiamonDMain.Joueur>();

        private static int roomOpen;


        public static void Start()
        {
            GetConfig();
            GetProtocol();
            Console.WriteLine("Server démarré.");
            
            endPoint = new IPEndPoint(ip, serverChoose.port);
            listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.ReceiveTimeout = serverChoose.timeout;
            listener.SendTimeout = serverChoose.timeout;
            listener.Bind(endPoint);
            listener.Listen(6);
            try
            {
                while (true)
                {
                    Socket client = listener.Accept();
                    var thread = new Thread(() =>
                    {
                        ConnectClient(client);

                    });
                    thread.Start();
                }
                //listener.Dispose();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ConnectClient(Socket client)
        {
            if (connexions == serverChoose.maxconnexions)
                connexions = 0;
            if (connexions == serverChoose.maxconnexions || connexions == 0)
                roomOpen = -1;

            byte[] buffer = new byte[1024];

            //Envoi du protocol à sérialiser (1)             
            bool envoiProtocol = SendProtocol(client);
            if (!envoiProtocol)
                client.Disconnect(true);

            //Reçoit réponse protocol + name + password à désérialiser (4)
            //Reçoit protocol
            int length = client.Receive(buffer);
            string data = Encoding.ASCII.GetString(buffer, 0, length);
            DiamonDMain.ProtocolMessageClient protocolClient;
            protocolClient = DeserializeProtocolClient(data);

            //Reçoit name
            string namePlayer = Get(client);

            //Sépare deux envois du client trop rapides
            SendOKorKO(1, client);

            //Reçoit password
            string pwdClient = Get(client);

            //Vérifier password et version
            if (serverChoose.password == pwdClient)
            {
                switch (protocolClient.version)
                {
                    case 1:
                        StartV1(client, namePlayer);
                        break;
                    default:
                        StartV1(client, namePlayer);
                        break;
                }
            }
            else
                SendOKorKO(0, client);
            //listener.Dispose(); Fin du server
        }

        public static void StartV1(Socket client, string namePlayer)
        {
            if (client.Connected)
            {
                try
                {
                    //Envoi ID (5)                
                    string envoiID = SendID(client);
                    if (envoiID == "")
                        client.Disconnect(true);

                    string OK = Get(client);

                    //Envoi port (5)
                    int roomPort = SendPort(client);
                    if (roomPort == -1)
                        client.Disconnect(true);

                    OK = Get(client);

                    //Envoi OK ou KO si connexion accepté(5) et créer la room
                    if (OK == "OK" && client.Connected)
                    {
                        SendOKorKO(1, client);

                        OK = Get(client);
                        //Créer salle et attend tous les joueurs
                        IPEndPoint newEndPoint = new(ip, roomPort);
                        Task<Socket> task = ServerGame.Room.StartServer(newEndPoint, serverChoose.maxconnexions, serverChoose.password, serverChoose.timeout);


                        SendOKorKO(1, client);
                        task.Wait();
                        client.Disconnect(true);

                        Socket room = task.Result;
                        connexions++;

                        // -- Game protocole 
                        //Identification
                        Task ready = ServerGame.Room.StartGame(room, roomPort, envoiID, namePlayer);
                        ready.Wait();

                        //Game
                        Task start = ServerGame.Room.Game();
                        start.Wait();


                    }
                    else
                        SendOKorKO(0, client);
                }
                catch(Exception ex)
                {
                    client.Disconnect(true);
                    Console.WriteLine(ex);
                }
            }
            
        }

        public static bool SendProtocol(Socket client)
        {
            string yaml = DiamonDMain.Yaml.Serialize(protocolChoose);
            byte[] msg = Encoding.ASCII.GetBytes(yaml);

            bool send = SendWork(msg, client);
            return send;
        }

        public static string SendID(Socket client)
        {
            //On créer une chaine de nombre aléatoire
            Random aleatoire = new Random();
            int taille = aleatoire.Next(10, 20); //chiffre entre 10 et 20                
            string ID = "";
            for (int i = 0; i < taille; i++)
            {
                int chiffre = aleatoire.Next(10);
                ID += chiffre;
            }

            byte[] msg = Encoding.ASCII.GetBytes(ID);

            bool send = SendWork(msg, client);
            if (send) return ID;
            else return "";
        }

        public static int SendPort(Socket client)
        {
            byte[] msg;
            int newPort = 0;

            if (roomOpen != -1)
                msg = Encoding.ASCII.GetBytes(roomOpen.ToString());            
            else
            {
                newPort = CreatePort();
                msg = Encoding.ASCII.GetBytes(newPort.ToString());
            }

            bool send = SendWork(msg, client);
            if (send)
            {
                if (roomOpen != -1)
                    return roomOpen;
                else
                {
                    roomOpen = newPort;
                    return newPort;
                }
            }
            else
                return -1;
        }

        public static int CreatePort()
        {
            //On ouvre un socket pour trouver un port disponible            
            Socket portSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint end = new IPEndPoint(ip, 0);
            portSocket.Bind(end);

            int newPort = ((IPEndPoint)portSocket.LocalEndPoint).Port;
            portSocket.Dispose();
            return newPort;
        }

        public static bool SendWork(byte[] msg, Socket client)
        {
            int bytesSent;
            if (msg == null)
            {
                bytesSent = client.Send(msgKO);
                return false;
            }
            else
            {
                bytesSent = client.Send(msg);
                if (msg != null || bytesSent != 0)
                    return true;
                return false;
            }                           
        }

        public static void SendOKorKO(int i, Socket client)
        {
            if (i == 1)
                client.Send(msgOK);
            else
            {
                client.Send(msgKO);
                client.Disconnect(true);
            }
        }

        public static bool SendPlay(Socket client)
        {
            byte[] msg = Encoding.ASCII.GetBytes("La partie va commencer");

            bool send = SendWork(msg, client);
            return send;
        }

        public static bool SendInstructions(Socket client)
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

            bool send = SendWork(msg, client);
            return send;
        }

        public static string Get(Socket client)
        {
            string data = "";
            if (client.Connected && client.Available != 0)
            {
                byte[] buffer = new byte[1024];
                int length = client.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, length);
            }
            return data;
        }

        public static void CreatePlayer(string name, string id)
        {
            DiamonDMain.Joueur player = new DiamonDMain.Joueur(name, id);
            joueurs.Add(id, player);
            
        }

        public static Ressources.Config DeserializeConfig(string path)
        {
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            var config = deserializer.Deserialize<Ressources.Config>(File.ReadAllText(path));

            return config;
        }

        public static DiamonDMain.ProtocolMessageServer DeserializeProtocol(string path)
        {
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            var config = deserializer.Deserialize<DiamonDMain.ProtocolMessageServer>(File.ReadAllText(path));

            return config;
        }

        public static DiamonDMain.ProtocolMessageClient DeserializeProtocolClient(string yaml)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            var config = deserializer.Deserialize<DiamonDMain.ProtocolMessageClient>(yaml);

            return config;
        }

        public static void GetConfig()
        {
            //Récupère configuration du server
            string longPath = DiamonDMain.Yaml.GetPath("Server");
            configChoose = DeserializeConfig(longPath);

            string serverNames = "";
            foreach (Server serv in configChoose.configurations)
            {
                serverNames += serv.name + "   ";
            }

            Console.WriteLine("Choisir un server :");
            Console.WriteLine(serverNames);
            string configName = Console.ReadLine();

            while (configName == "" || !VerifConfig(configName))
            {
                Console.WriteLine("Choisir un server de la liste suivante : ");
                Console.WriteLine(serverNames);
                configName = Console.ReadLine();
            }

            //Récupère la bonne configuration
            foreach (Server serv in configChoose.configurations)
            {
                if (configName == serv.name)
                    serverChoose = serv;
            }
        }

        public static bool VerifConfig(string name)
        {
            bool trouve = false;
            foreach (Server serv in configChoose.configurations)
            {
                if (name == serv.name)
                    trouve = true;
            }
            return trouve;
        }

        public static void GetProtocol()
        {
            //Changer quand on implémente les nouvelles versions

            //Récupère protocol
            Console.WriteLine("Choisir le protocol :");
            
            Console.WriteLine("1");
            string protocolName = Console.ReadLine();

            while (protocolName == "" || protocolName != "1")
            {
                Console.WriteLine("Veuillez entrer 1");
                protocolName = Console.ReadLine();
            }

            string longPath;
            if (protocolName == "1")
                longPath = DiamonDMain.Yaml.GetPath("Protocol1");
            else
                longPath = DiamonDMain.Yaml.GetPath(protocolName);
            protocolChoose = DeserializeProtocol(longPath);
        }

        static void Main(string[] args)
        {            
            Start();
            Console.ReadLine();
        }

        public void Test()
        {
            //Server s = new Server("Diamond", 1234, "123", 2, 3000, "jeu");
            //List<Server> servers = new List<Server>();
            //servers.Add(s);
            //Ressources.Config c = new("conf", servers);
            //string serv = DiamonDMain.Yaml.Serialize(c);
        }
    }
}
