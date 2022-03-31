using System;
using System.Collections;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;

namespace DiamonDMain
{
    class Program
    {
        private static ArrayList a = new ArrayList { new Danger(), new Tresor(), new Trophee() };

        static void Main(string[] args)
        {
            Carte c = Tirercarte();
            Partie.CreaCarte();
            
        }
        public static Carte Tirercarte()
        {
            Carte c = null;
            if("" == "")
            {
            var rand = new Random();
            c = (Carte)a[rand.Next(a.Count)];
            }
            return c; 
        }
    }
}
