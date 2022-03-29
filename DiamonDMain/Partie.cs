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


    }
}
