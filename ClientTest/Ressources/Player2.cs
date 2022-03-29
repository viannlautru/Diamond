using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest.Ressources
{
    public static class Player2
    {
        private static string address { get; set; }
        private static int port { get; set; }
        private static string password = "123";
        private static string name = "Joueur2";


        //Getters
        public static string GetPassword() { return password; }
        public static string GetName() { return name; }
    }
}
