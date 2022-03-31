using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization.NamingConventions;

namespace DiamonDMain
{
    class Program
    {
        private static ArrayList a = new ArrayList { new Danger(), new Tresor(), new Trophee() };

        static void Main(string[] args)
        {
            Carte c = Tirercarte();
            Tresor t = new Tresor();
            Joueur j = new Joueur("abc", "1");
            Dictionary<int, Joueur> liste = new Dictionary<int, Joueur>();
            liste.Add(1, j);
            liste.Add(2, new Joueur("bn,;l", "bhnj"));
            liste.Add(3, new Joueur(",;l", "bnj"));
            liste.Add(4, new Joueur("bn;l", "bj"));
            Partager(t, liste);
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
        public static void Partager(Carte c, Dictionary<int, Joueur> joueurs)
        {
            int montant = ((Tresor)c).Partager(joueurs.Count);
            joueurs.All(c => { c.Value.diamands += montant; return true; });
        }
    }
}
