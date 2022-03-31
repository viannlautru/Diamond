using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ClientTest
{
    class Client
    {
        public static ClientParser parser = new();
        public static IPAddress ip;
        public static IPEndPoint endPoint;
        public static DiamonDMain.ProtocolMessageServer protocol;

        public int bytesSent = 0;
        public void Connect()
        {
            //On obtient config du client
            GetConfig();
         
            try
            {                
                Socket socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                

                //Reçoit le protocole et le désérialise (2)
                GetProtocol(socket);

                //Renvoi la réponse du protocole (3)
                SendProtocol(socket);

                //Envoi name (3)
                SendName(socket);

                //Reçoit OK du server
                byte[] buffer = new byte[1024];
                int length = socket.Receive(buffer);
                string data = Encoding.ASCII.GetString(buffer, 0, length);
                CheckKO(data, socket);

                //Envoi password (3)
                SendPwd(socket);

                
                //Reçoit ID + Port + OK ou KO (6)
                length = socket.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, length);
                CheckKO(data, socket);
                string ID = data;

                SendOKorKO(1, socket);

                length = socket.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, length);
                CheckKO(data, socket);
                int port = int.Parse(data);

                SendOKorKO(1, socket);

                length = socket.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, length);
                CheckKO(data, socket);
                string OK = data;

                SendOKorKO(1, socket);

                if (OK == "OK")
                {
                    length = socket.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, length);
                    CheckKO(data, socket);
                    OK = data;

                    //Client se déconnecte du serveur et se connecte à la salle (7)
                    Socket room = RoomConnect(port);
                    socket.Close();

                    Console.WriteLine("En attente d'autres joueurs...");

                    //Envoi ID de session (7)
                    SendSession(room, ID);

                    length = room.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, length);
                    CheckKO(data, room);
                    OK = data;

                    //Envoi password (7)
                    SendPwd(room);

                    //Reçoit OK ou KO (10)
                    length = room.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, length);
                    CheckKO(data, room);
                    OK = data;

                    SendOKorKO(1, room);

                    //Reçoit PLAY
                    length = room.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, length);
                    CheckKO(data, room);
                    string play = data;

                    Console.WriteLine(play);

                    SendOKorKO(1, room);

                    //Reçoit instructions
                    buffer = new byte[2048];
                    length = room.Receive(buffer);
                    data = Encoding.UTF8.GetString(buffer, 0, length);
                    CheckKO(data, room);
                    string instructions = data;

                    Console.WriteLine(instructions);

                    SendOKorKO(1, room);

                    //Reçoit OK ou KO
                    length = room.Receive(buffer);
                    data = Encoding.ASCII.GetString(buffer, 0, length);
                    CheckKO(data, room);
                    OK = data;



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

        public void GetConfig()
        {
            Console.WriteLine("Choisir une config :");
            string pathDirectory = Path.GetFullPath("Ressources");
            pathDirectory = pathDirectory.Replace(@"\bin\Debug\net6.0", "");
            string[] files = Directory.GetFileSystemEntries(pathDirectory);
            string resp = "";
            foreach (string file in files)
            {
                if (file.IndexOf(".yaml") != -1)
                {
                    string newFile = file.Replace(".yaml", "");
                    resp += newFile.Substring(file.IndexOf(@"\Ressources\") + 12) + "   ";
                }

            }
            Console.WriteLine(resp);
            string? path = null;
            while (path == null) { path = Console.ReadLine(); }
            path = DiamonDMain.Yaml.GetPath(path);

            parser = DeserializePathParser(path);
            ip = new IPAddress(parser.address);
            endPoint = new IPEndPoint(ip, parser.port);
        }

        public void GetProtocol(Socket socket)
        {
            byte[] buffer = new byte[1024 * 4];
            int length = socket.Receive(buffer);
            string yaml = Encoding.ASCII.GetString(buffer, 0, length);
            CheckKO(yaml, socket);

            if (yaml != null)
                protocol = DeserializeProtocol(yaml);
        }

        public void SendProtocol(Socket socket)
        {
            int version = protocol.version;
            DiamonDMain.ProtocolMessageClient protocolClient = new();
            if (version == 1)
                protocolClient = new(protocol.version, "", "");
            //A refaire plus tard lors d'une évolution avec le protocole
            else
            {
                Console.WriteLine("Version ?");
                int? vers;
                vers = Int32.TryParse(Console.ReadLine(), out var res) ? res : (int?)null;

                if (vers != null)
                {
                    int v = (int)version;
                    protocolClient = new(v);
                }
            }

            string yaml = DiamonDMain.Yaml.Serialize(protocolClient);
            byte[] theMsg = Encoding.ASCII.GetBytes(yaml);
            bytesSent = socket.Send(theMsg);
        }

        public void SendName(Socket socket)
        {
            byte[] name = Encoding.ASCII.GetBytes(parser.name);
            bytesSent = socket.Send(name);
        }

        public void SendPwd(Socket socket)
        {
            byte[] pwd = Encoding.ASCII.GetBytes(parser.password);
            bytesSent = socket.Send(pwd);
        }

        public void SendSession(Socket room, string id)
        {
            byte[] session = Encoding.ASCII.GetBytes(id);
            bytesSent = room.Send(session);
        }

        public void CheckKO(string msg, Socket socket)
        {
            if (msg == "KO")
                Stop(socket);            
        }

        public void SendOKorKO(int i, Socket socket)
        {
            if (i == 1)
                socket.Send(Encoding.ASCII.GetBytes("OK"));
            else
            {
                socket.Send(Encoding.ASCII.GetBytes("KO"));
                socket.Close();
            }
        }

        public static void Stop(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            Console.WriteLine("Déconnexion du serveur");
            Console.ReadLine();
            Environment.Exit(1);
        }

        public Socket RoomConnect(int port)
        {
            //on se connecte à la salle
            IPAddress ip = new IPAddress(new byte[] { 127, 0, 0, 1 });            
            IPEndPoint newEndPoint = new IPEndPoint(ip, port);
            Socket room = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            room.Connect(newEndPoint);
            Console.WriteLine("Vous êtes dans la salle : " + port);
            return room;
        }        

        public ClientParser DeserializePathParser(string path)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            var config = deserializer.Deserialize<ClientParser>(File.ReadAllText(path));

            return config;
        }

        public DiamonDMain.ProtocolMessageServer DeserializeProtocol(string yaml)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            var config = deserializer.Deserialize<DiamonDMain.ProtocolMessageServer>(yaml);

            return config;
        }
    }
}
