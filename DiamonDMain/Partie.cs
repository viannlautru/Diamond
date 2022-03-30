using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Partie
    {
        private string id;
        private int max_players;
        private Dictionary<int, Grotte> caves;
        private int cards_quantity;
        private List<DiamonDMain.Danger> traps;
        private int type;   //Standard pour l'instant type = 0 partie rapide : type = 1
        private Dictionary<string, Joueur> Joueurgrotte; 
        public Grotte laGrotte;
        public Camp leCamp;
        public Joueur joueur;
        public Dictionary<int, Carte> ToutelesCarte;

        //private Dictionary<int, Grotte> grottes;

        public void StartGame(Dictionary<string, Joueur> joueurs)
        {
            Partie partie = new Partie(joueurs);
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
            if (type == 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    caves.Add(i, new Grotte());
                }
            }
        }

        public void CreaCarte()
        {
            //Creation du packet de carte on divise part 4 pour les 4 type de cartes on avoir un bon nombre égale de carte
            //n est la récupération de la quantité de carte dans le dossier config de la partie
            var i = 0;
            var n = 30;
            while (i < n)
            {
                if(i<=n/2)
                    ToutelesCarte.Add(i,new Trophee());
                else if (i <= n / 2 * 2)
                    ToutelesCarte.Add(i, new Tresor());
                i++;
            }

        }
        public void Fingrotte(){
            for (var i =0; i >= laGrotte.getjoueurGrotte().Count; i++) {
                //enleve les joueurs de la grotte et les mets dans le camp
                leCamp.addCampUtilisateur(laGrotte.Joueurgrotte.Keys.ElementAt(i)) ;
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
        public bool tirageCarte()
        {
            //tirage aléatoire sur la liste toueCarte qui est comme la pioche 
            var rand = new Random();
            for (var i = 0; i >= leCamp.getjoueurCamp().Count; i++)
            {
                int key = ToutelesCarte.Keys.ElementAt(rand.Next(ToutelesCarte.Count));
                Carte c = ToutelesCarte[key];
                if (c.GetType().Name == "Tresor")
                    Partager((Tresor)c);
                ToutelesCarte.Remove(key);
            }
                return true;
        }
        public void Partager(Tresor tresor)
        {
            int montant = tresor.GetMontant() / laGrotte.getjoueurGrotte().Count;
            laGrotte.getjoueurGrotte().All(c => { c.Value.diamands += montant; return true; });
        }
    }
}
