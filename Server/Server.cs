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
        private static string templateYaml = @"address: string         port: int       password: string        name: string";
        private static IPAddress ip;
        private static IPEndPoint endPoint;
        private static Socket listener;
        private static int version = 1;
        private static Ressources.Config configChoose;
        private static DiamonDMain.ProtocolMessageServer protocolChoose;

        private static string name = "Diamond";
        private static int port = 1234;
        private static string password = "123";
        private static int max_connexions = 2; //5 à mettre pour V1
        private static int timeout = 5000;
        private static DiamonDMain.Partie game;

        //Variables globales
        private static byte[] msgOK = Encoding.ASCII.GetBytes("OK");
        private static byte[] msgKO = Encoding.ASCII.GetBytes("KO");

        private static int connexions = 0;
        private static Dictionary<string, DiamonDMain.Joueur> joueurs = new Dictionary<string, DiamonDMain.Joueur>();

        private static DiamonDMain.Joueur player1;
        private static DiamonDMain.Joueur player2;

        private static int roomOpen = -1;
        private static Socket roomCreate;

        public static void Start()
        {
            //Récupère configuration du server
            Console.WriteLine("Choisir une configuration :");
            Console.WriteLine("Server, ...");
            string configName = Console.ReadLine();

            string longPath = GetPath(configName);
            configChoose = DeserializeConfig(longPath);

            //Récupère protocol
            Console.WriteLine("Choisir le protocol :");
            Console.WriteLine("1");
            string protocolName = Console.ReadLine();
            if (protocolName == "1")
                longPath = GetPath("Protocol1");
            else
                longPath = GetPath(protocolName);
            protocolChoose = DeserializeProtocol(longPath);

            string test = Serialize(protocolChoose);


            ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
            endPoint = new IPEndPoint(ip, port);
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



            DiamonDMain.ProtocolMessageServer protocol = new(version);
            //Convertir le protocol
            string json = JsonConvert.SerializeObject(protocol, Formatting.Indented);
            //
            byte[] msg = Encoding.ASCII.GetBytes(json);

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
            IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
            //on créer un serveur (une salle) qui récupère les joueurs
            IPEndPoint newEndPoint = new IPEndPoint(ip, port);
            Socket room = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            room.Bind(newEndPoint);
            room.Listen(max_connexions);
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

        public static void SenOKorKO(int i, Socket client)
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
            if (connexions == max_connexions)
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
            string json = Encoding.ASCII.GetString(buffer, 0, length);
            DiamonDMain.ProtocolMessageServer protocol = null;
            protocol = JsonConvert.DeserializeObject<DiamonDMain.ProtocolMessageServer>(json);

            //Reçoit name
            length = client.Receive(buffer);
            data = Encoding.ASCII.GetString(buffer, 0, length);
            string nameClient = data;

            //Reçoit password
            length = client.Receive(buffer);
            data = Encoding.ASCII.GetString(buffer, 0, length);
            string passwordClient = data;

            //Vérifier name et password
            if (name == nameClient && password == passwordClient)
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
                    SenOKorKO(1, client);
                    //Créer salle et attend tous les joueurs
                    if (connexions == 0 || connexions == max_connexions)
                    {
                        client.Close();
                        roomCreate = CreateRoom(roomPort);

                    }
                    client = roomCreate.Accept();
                    connexions++;
                    ReceiveIdPassword(client);
                }
                else
                {
                    SenOKorKO(0, client);
                }
            }
            else
            {
                SenOKorKO(0, client);
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
                    playerID, port);
                joueurs.Add(playerID, player1);
            }
            if (connexions == 2)
            {
                player2 = new DiamonDMain.Joueur(passwordClient, ClientTest.Ressources.Player2.GetName(),
                    playerID, port);
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
            if (password == passwordClient)
            {
                SenOKorKO(1, clientConnect);
                CreatePlayer(passwordClient, playerID);
            }
            else
            {
                SenOKorKO(0, clientConnect);
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

        public static string Serialize(object o)
        {
            var stringBuilder = new StringBuilder();
            var serializer = new Serializer();
            serializer.Serialize(new IndentedTextWriter(new StringWriter(stringBuilder)), o);
            return stringBuilder.ToString();
        }

        public static string GetPath(string name)
        {
            string fileName = @"Ressources\" + name + ".yaml";
            string thePath = Path.GetFullPath(fileName);
            thePath = thePath.Replace(@"\bin\Debug\net6.0", "");
            return thePath;
        }

        static void Main(string[] args)
        {            
            Start();
            Console.ReadLine();
        }
    }
}
