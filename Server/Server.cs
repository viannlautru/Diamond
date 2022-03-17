using System;
using System.IO;
using System.Net;
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
        //private static string name;
        //private static int port;
        //private static string password;
        //private static int max_connextions;
        //private static int timeout;
        //private static DiamonDMain.Partie game;        

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
                while (true)
                {
                    int length = client.Receive(buffer);
                    data += Encoding.ASCII.GetString(buffer, 0, length);
                    if (data.IndexOf("<EOF>") > -1)
                        break;
                }
                Console.WriteLine("Received: " + data);

                DiamonDMain.ProtocolMessageServer protocol = new DiamonDMain.ProtocolMessageServer(1);
                byte[] msg = Encoding.ASCII.GetBytes(protocol.ToString());
                client.Send(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
        public static Boolean Exists(String tempName, String tempPWD)
        {
            String path = @"\Client\Ressources\client.yaml";
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] buffer = new byte[fs.Length];
                if (fs.CanRead)
                    fs.Read(buffer, 0, (int)fs.Length);
                return buffer[5].ToString() == tempName && buffer[7].ToString() == tempPWD;
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
