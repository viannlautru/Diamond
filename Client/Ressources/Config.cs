using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Ressources
{
    static class Config
    {
        private static string address;
        private static int port;
        private static string password = "123";
        private static string name = "client";


        //Getters
        public static string GetAddress() { return address; }
        public static int GetPort() { return port; }
        public static string GetPassword() { return password; }
        public static string GetName() { return name; }
    }
}
