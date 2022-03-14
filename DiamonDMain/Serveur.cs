﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Serveur
    {
        private static string templateYaml = @"address: string         port: int       password: string        name: string";
        private static IPAddress ip;
        private static IPEndPoint endPoint;
        private static Socket listener ;
        public static void Start() { 

             byte[] buffer = new byte[1024];
             string data = null;
             ip = new IPAddress(new byte[] { 127, 0, 0, 0 });
             endPoint = new IPEndPoint(ip, 1234);
             listener = new Socket(ip.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
        
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
        client.Shutdown(SocketShutdown.Both);
        client.Close();
        }
    catch (Exception e)
    {
        Console.WriteLine(e.ToString());
    }
    }
        public static void CreateGamer(String tempName, String tempPWD)
        {
            tempPWD = "0044";

            String path = @"C:\Users\madyd\source\repos\Diamond\DiamonDMain\Ressources\" + tempName + tempPWD + ".yaml";
            using (FileStream fs = File.Create(path))
            {
                string str = templateYaml.Replace("<PORT>", endPoint.Port.ToString()).Replace("<PWD>", tempPWD).Replace("<NAME>", tempName).Replace("<ADRESS>", endPoint.Address.ToString());
                UTF8Encoding encoding = new UTF8Encoding(true);
                byte[] text = encoding.GetBytes(str);
                fs.Write(text, 0, text.Length);
            }

        }
        public static Boolean Connected(String tempName, String tempPWD)
        {
            String path = @"C:\Users\madyd\source\repos\Diamond\DiamonDMain\Ressources\" + tempName + tempPWD + ".yaml";
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] buffer = new byte[fs.Length];
                if (fs.CanRead)
                    fs.Read(buffer, 0, (int)fs.Length);
                return buffer[5].ToString() == tempName && buffer[7].ToString() == tempPWD;
            }
        }
    }
}
