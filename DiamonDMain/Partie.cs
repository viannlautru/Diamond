﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Partie
    {
        public string id { get; set; }
        public int maxplayers { get; set; }
        public int caves { get; set; }
        public int allCardsquantity { get; set; }
        public List<Danger> traps { get; set; }


        private int type { get; set; }  //Standard pour l'instant type = 0 partie rapide : type = 1
        private Dictionary<string, Joueur> Joueurgrotte { get; set; }
        public Grotte laGrotte { get; set; }
        public Camp leCamp { get; set; }
        public Joueur joueur { get; set; }

        public Partie()
        {
            this.type = 0;
            CreateCaves();
        }

        public static List<Carte> allCards = new List<Carte>();
        public static List<Carte> PlayedCards = new List<Carte>();
        public Dictionary<int, Joueur> JoueurIntermediaire;
  


        public static void StartGame(Dictionary<string, Joueur> joueurs)
        {
            Partie partie = new Partie(joueurs);
            Console.ReadLine();
        }

        //Partie rapide => 2 grottes
        public Partie(Dictionary<string, Joueur> Joueurgrotte, int type = 0)
        {
            this.type = type;
            this.Joueurgrotte = Joueurgrotte;
            CreateCaves();
        }
        public void CreateCaves()
        {
            if (caves <= 0)
                EndGame();
            else
                caves--;
        }
        public void EndGame()
        {
            Console.WriteLine("Finit");
        }
        public static void CreaCarte()
        {
            List<Partie> games = Yaml.DeserializeGame("Server");
            Partie gameChoose = games.First();
            foreach (var trap in gameChoose.traps)
            {
                var tq = 0;
                int q = trap.quantity;
                string t = trap.name;
                while (tq < q)
                {
                    //Ajouter les traps dans la pioche
                    allCards.Add(trap);
                    tq++;
                }
            }
            //Creation du packet de carte on divise part 4 pour les 4 type de cartes on avoir un bon nombre égale de carte
            //n est la récupération de la quantité de carte dans le dossier config de la partie
            var i = 0;
            var n = gameChoose.allCardsquantity;
            while (i < n)
            {
                if (i <= n / 2)
                    allCards.Add(new Trophee());
                else
                    allCards.Add(new Tresor());
                i++;
            }
        }

        public void ExitCave(Carte trap)
        {
            foreach (var card in PlayedCards)
            {
                if (card == trap)
                    Fingrotte();
            }
        }
        public void getCartUtilise(Carte card)
        {
            switch (card.GetType().Name)
            {
                case "Danger":
                    ExitCave(card);
                    break;
                case "Trophee":
                    break;
                case "Tresor":
                    Partager(card);
                    break;
            }
            PlayedCards.Add(card);
        }
        public string forechcardsTrophee()
        {
            var nombreDiamsTrophee = 0;
            foreach (var o in PlayedCards)
            {
                if("Trophee" == o.GetType().Name)
                {
                    nombreDiamsTrophee += ((Trophee)o).montantTrophee;
                    PlayedCards.Remove(o);
                }
                
            }
            return nombreDiamsTrophee + ",";
        } 
        public List<Carte> RecupCardsDiams()
        {
            List<Carte> CardsTresorrest = new List<Carte>();
            foreach (var o in PlayedCards)
            {
                if ("Tresor" == o.GetType().Name)
                {
                    CardsTresorrest.Add(o);
                    PlayedCards.Remove(o);
                }
            }
            return CardsTresorrest;
        }
        public void Fingrotte()
        {
            for (var i = 0; i >= laGrotte.getjoueurGrotte().Count; i++)
            {
                //enleve les joueurs de la grotte et les mets dans le camp
                leCamp.addCampUtilisateur(laGrotte.Joueurgrotte.Keys.ElementAt(i));
            }
        }
        public void debutgrotte()
        {
            for (var i = 0; i >= leCamp.getjoueurCamp().Count; i++)
            {
                //ajout des joueurs dans la grotte
                laGrotte.addGrotteUtilisateur(leCamp.JoueurCamp.Keys.ElementAt(i));
            }
        }
        public bool Continuer()
        {
            if (laGrotte.getjoueurGrotte().Count <= 0)
                return false;
            else
                return true;
        }
        public bool DistributeCards()
        {
            //tirage aléatoire sur la liste allCards qui est comme la pioche 
            int rand = new Random().Next(allCards.Count);
            Carte card = allCards[rand];
            allCards.Remove(card);

            //if (c.GetType().Name == "Tresor")
            //    Partager((Tresor)c);
            //allCards.Remove(key);
            getCartUtilise(card);
            return true;
        }
        public void pushIntermediaire(int joueur)
        {
            if (laGrotte.Joueurgrotte.ContainsKey(joueur))
            {
                JoueurIntermediaire.Add(joueur, laGrotte.Joueurgrotte[joueur]);
            }
            laGrotte.Joueurgrotte.Remove(joueur);
        }
        public void RecupRestDiams()
        {
            List<Carte> CardsTresorrest = RecupCardsDiams();
            foreach(var s in CardsTresorrest)
            {
                Partager(s, JoueurIntermediaire);
            }
        }
        public void Partager(Carte tresor, Dictionary<int, Joueur> joueurs = null)
        {
            int montant = 0;
            montant = ((Tresor)tresor).Partager(joueurs.Count);
            joueurs.All(c => { c.Value.diamands += montant; return true; });
        }
    }
}
