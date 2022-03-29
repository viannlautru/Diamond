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
    }
}
