using System;
using System.Collections;

namespace DiamonDMain
{
    class Program
    {
        private static ArrayList a = new ArrayList { new Danger(), new Tresor(), new Trophee() };

        static void Main(string[] args)
        {
            Carte c = Tirercarte();
        }
        public static Carte Tirercarte()
        {
            var rand = new Random();
            return (Carte)a[rand.Next(a.Count)];
        }
    }
}
