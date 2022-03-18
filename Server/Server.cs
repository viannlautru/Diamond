using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace Server
{
    public class Server
    {
        private static string templateYaml = @"address: string         port: int       password: string        name: string";
        private static IPAddress ip;
        private static IPEndPoint endPoint;
        private static Socket listener;
        private static string name;
        private static int port;
        private static string password;
        private static int max_connextions;
        private static int timeout;
        private static DiamonDMain.Partie game;

        public static void Start()
        {

            byte[] buffer = new byte[1024];
            string data = null;
            ip = new IPAddress(new byte[] { 127, 0, 0, 1 });
            endPoint = new IPEndPoint(ip, 1234);
            listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(endPoint);
                listener.Listen(6);
                Socket client = listener.Accept();

                int length = client.Receive(buffer);
                byte[] msg = null;
                int bytesSent = 0;
                string notOK = "KO";

                //Envoi du protocol à sérialiser (1)             
                string envoiProtocol = SendProtocol(client);
                if (envoiProtocol == notOK)
                {
                    msg = Encoding.ASCII.GetBytes(notOK);
                    bytesSent = client.Send(msg);
                }                

                //Reçoit réponse protocol + name + password à désérialiser (4)
                //Reçoit name
                data += Encoding.ASCII.GetString(buffer, 0, length);
                string name = data;

                //Reçoit password
                data += Encoding.ASCII.GetString(buffer, 0, length);
                string password = data;

                //Envoi ID (5)                
                string envoiID = SendID(client);
                if (envoiID == notOK)
                {
                    msg = Encoding.ASCII.GetBytes(notOK);
                    bytesSent = client.Send(msg);
                }

                //Envoi port (5)
                string envoiPort = SendPort(client);
                if (envoiPort == notOK)
                {
                    msg = Encoding.ASCII.GetBytes(notOK);
                    bytesSent = client.Send(msg);
                }

                //Envoi OK ou KO si connexion accepté (5)
                if (envoiProtocol != notOK && envoiID != notOK && envoiPort != notOK)
                {
                    msg = Encoding.ASCII.GetBytes("OK");
                    bytesSent = client.Send(msg);
                }

                listener.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                
            }
        }

        public static string SendProtocol(Socket client)
        {
            string response = "OK";

            DiamonDMain.ProtocolMessageServer protocol = new DiamonDMain.ProtocolMessageServer(1);
            //Convertir le protocol
            //

            byte[] msg = Encoding.ASCII.GetBytes(protocol.ToString());
            int bytesSent = client.Send(msg);
            if (msg == null || bytesSent == 0)
                response = "KO";

            return response;
        }

        public static string SendID(Socket client)
        {            
            string response = "OK";

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
            int bytesSent = client.Send(msg);
            if (msg == null || bytesSent == 0)
                response = "KO";

            return response;
        }

        public static string SendPort(Socket client)
        {
            string response = "OK";

            //On ouvre un socket pour trouver un port disponible
            Socket portSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint end = new IPEndPoint(ip, 0);
            portSocket.Bind(end);

            int port = ((IPEndPoint)portSocket.LocalEndPoint).Port;
            byte[] msg = Encoding.ASCII.GetBytes(port.ToString());
            int bytesSent = client.Send(msg);
            portSocket.Dispose();

            if (msg == null || bytesSent == 0)
                response = "KO";

            return response;
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
        public static Ressources.Config DeserializeUser(String path)
        {
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            Ressources.Config user = (Ressources.Config)formatter.Deserialize(stream);
            stream.Close();
            return user;
        }

        

        static void Main(string[] args)
        {
            Start();
            Console.ReadLine();
        }
    }
}
