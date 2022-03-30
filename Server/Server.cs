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
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Server
{
    public class Server
    {

        //Object Server
        public string name1 { get; set; }
        public int port { get; set; }
        public string password { get; set; }
        public int maxconnexions { get; set; }
        public int timeout1 { get; set; }
        public string game1 { get; set; }

        public Server() { }
        public Server(string name, int port, string pwd, int max, int timeout, string game)
        {
            this.name1 = name;
            this.port = port;
            this.password = pwd;
            this.maxconnexions = max;
            this.timeout1 = timeout;
            this.game1 = game;
        }
        //

        private static string templateYaml = @"address: string         port: int       password: string        name: string";
        private static IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private static IPEndPoint endPoint;
        private static Socket listener;
        private static int version = 1;
        private static Ressources.Config configChoose;
        private static DiamonDMain.ProtocolMessageServer protocolChoose;
        private static Server serverChoose;

        private static DiamonDMain.Partie game;

        //Variables globales
        private static byte[] msgOK = Encoding.ASCII.GetBytes("OK");
        private static byte[] msgKO = Encoding.ASCII.GetBytes("KO");

        private static int connexions = 0;
        private static Dictionary<string, DiamonDMain.Joueur> joueurs = new Dictionary<string, DiamonDMain.Joueur>();

        private static DiamonDMain.Joueur player1;
        private static DiamonDMain.Joueur player2;

        private static int roomOpen = -1;


        public static void Start()
        {
            GetConfig();
            GetProtocol();            
            
            endPoint = new IPEndPoint(ip, serverChoose.port);
            listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
        
        public static bool SendProtocol(Socket client)
        {
            string yaml = DiamonDMain.Yaml.Serialize(protocolChoose);
            byte[] msg = Encoding.ASCII.GetBytes(yaml);

            bool send = SendWork(msg, client);
            return send;
        }

        public static bool SendID(Socket client)
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
            return send;
        }

        public static int SendPort(Socket client)
        {
            //On ouvre un socket pour trouver un port disponible            
            Socket portSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint end = new IPEndPoint(ip, 0);
            portSocket.Bind(end);

            byte[] msg;
            int newPort = 0;

            if (roomOpen != -1)
            {
                portSocket.Dispose();
                msg = Encoding.ASCII.GetBytes(roomOpen.ToString());
            }
            else
            {
                newPort = ((IPEndPoint)portSocket.LocalEndPoint).Port;
                portSocket.Dispose();
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

        public static Socket CreateRoom(int port)
        {
            //on créer un serveur (une salle) qui récupère les joueurs
            IPEndPoint newEndPoint = new IPEndPoint(ip, port);
            Socket room = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            room.Bind(newEndPoint);
            room.Listen(serverChoose.maxconnexions);
            return room;
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
                client.Close();
            }
        }        

        public static void ConnectClient(Socket client)
        {
            if (connexions == serverChoose.maxconnexions)
            {
                connexions = 0;
                StartGame();
            }

            byte[] buffer = new byte[1024];
            string data = null;           

            byte[] msg = null;
            int bytesSent = 0;
            string notOK = "KO";

            //Envoi du protocol à sérialiser (1)             
            bool envoiProtocol = SendProtocol(client);
            if (!envoiProtocol)
                client.Close();

            //Reçoit réponse protocol + name + password à désérialiser (4)
            //Reçoit protocol
            int length = client.Receive(buffer);
            string yaml = Encoding.ASCII.GetString(buffer, 0, length);
            DiamonDMain.ProtocolMessageClient protocolClient = new();
            protocolClient = DeserializeProtocolClient(yaml);

            //Reçoit name
            length = client.Receive(buffer);
            string nameClient = Encoding.ASCII.GetString(buffer, 0, length);

            //Reçoit password
            length = client.Receive(buffer);
            string passwordClient = Encoding.ASCII.GetString(buffer, 0, length);
            

            //Vérifier name et password
            if (serverChoose.password == passwordClient)
            {
                //Envoi ID (5)                
                bool envoiID = SendID(client);
                if (!envoiID)
                    client.Close();

                //Envoi port (5)
                int roomPort = SendPort(client);
                if (roomPort == -1)
                    client.Close();

                //Envoi OK ou KO si connexion accepté(5) et créer la room
                if (envoiProtocol && envoiID && roomPort != -1)
                {
                    SendOKorKO(1, client);
                    client.Close();

                    //Créer salle et attend tous les joueurs
                    if (connexions == 0 || connexions == serverChoose.maxconnexions)
                    {                        
                        var thread = new Thread(() =>
                        {
                            Socket roomCreate = CreateRoom(roomPort);
                            while (serverChoose.maxconnexions != connexions)
                            {
                                client = roomCreate.Accept();
                                connexions++;
                            }
                        });
                        thread.Start();

                    }

                    ReceiveIdPassword(client);
                }
                else
                {
                    SendOKorKO(0, client);
                }
            }
            else
            {
                SendOKorKO(0, client);
            }

            //listener.Dispose();
        }

        public static void StartGame()
        {

        }

        public static void CreatePlayer(string passwordClient, string playerID)
        {
            if (connexions == 1)
            {
                player1 = new DiamonDMain.Joueur(passwordClient, ClientTest.Ressources.Player1.GetName(),
                    playerID, serverChoose.port);
                joueurs.Add(playerID, player1);
            }
            if (connexions == 2)
            {
                player2 = new DiamonDMain.Joueur(passwordClient, ClientTest.Ressources.Player2.GetName(),
                    playerID, serverChoose.port);
                joueurs.Add(playerID, player2);
            }
        }
    

        public static void ListenNewClient()
        {            
            Socket client = listener.Accept();
            ConnectClient(client);
        }

        public static void ReceiveIdPassword(Socket clientConnect)
        {

            byte[] buffer = new byte[1024];

            //Reçoit ID + password (8)
            int length = clientConnect.Receive(buffer);
            string data = Encoding.ASCII.GetString(buffer, 0, length);
            string playerID = data;

            length = clientConnect.Receive(buffer);
            data = Encoding.ASCII.GetString(buffer, 0, length);
            string passwordClient = data;

            //Envoi OK (9)
            if (serverChoose.password == passwordClient)
            {
                SendOKorKO(1, clientConnect);
                CreatePlayer(passwordClient, playerID);
            }
            else
            {
                SendOKorKO(0, clientConnect);
            }
        }

        public static void InsertGamer(String tempName, String tempPWD)
        {
            tempPWD = "0044";

            String path = @"\Diamond\Ressources\client.yaml";
            using (FileStream fs = File.Create(path))
            {
                string str = templateYaml.Replace("<PORT>", endPoint.Port.ToString()).Replace("<PWD>", tempPWD).Replace("<NAME>", tempName).Replace("<ADRESS>", endPoint.Address.ToString());
                UTF8Encoding encoding = new UTF8Encoding(true);
                byte[] text = encoding.GetBytes(str);
                fs.Write(text, 0, text.Length);
            }

        }
        public static string Openleficher(String tempName, String tempPWD)
        {
            String path = @"Ressources\Server.yaml";
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] buffer = new byte[fs.Length];
                if (fs.CanRead)
                    fs.Read(buffer, 0, (int)fs.Length);
                return buffer.ToString();
            }
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
                serverNames += serv.name1 + "   ";
            }

            Console.WriteLine("Choisir un server :");
            Console.WriteLine(serverNames);
            string configName = Console.ReadLine();

            foreach (Server serv in configChoose.configurations)
            {
                if (configName == serv.name1)
                    serverChoose = serv;
            }
        }

        public static void GetProtocol()
        {
            //Récupère protocol
            Console.WriteLine("Choisir le protocol :");
            Console.WriteLine("1");
            string protocolName = Console.ReadLine();
            string longPath = "";

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
            Server s = new Server("Diamond", 1234, "123", 2, 3000, "jeu");
            List<Server> servers = new List<Server>();
            servers.Add(s);
            Ressources.Config c = new("conf", servers);
            string serv = DiamonDMain.Yaml.Serialize(c);
        }
    }
}
