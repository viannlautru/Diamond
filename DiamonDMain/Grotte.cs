using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Grotte
    {
        public bool Etat;
        public Dictionary<int, Joueur> Joueurgrotte;
        public Camp leCamp;

        public Grotte ()
        {
            this.Etat = true;
        }
        public bool getEtatGrotte()
        {
            return Etat;
        }
        public void setEtatGrotte(bool Etatgrotte)
        {
            this.Etat = Etatgrotte;
        }
        public Dictionary<int, Joueur> getjoueurGrotte()
        {
            return Joueurgrotte;
        }
        public void Initjoueurs(Dictionary<int, Joueur> Joueur)
        {
            this.Joueurgrotte = Joueur;
        }
        public int addGrotteUtilisateur(int joueur)
        {
            foreach (var p in leCamp.JoueurCamp) {
                if(p.Key == joueur)
                    Joueurgrotte.Add(joueur,p.Value);
            }
            leCamp.JoueurCamp.Remove(joueur);
            return joueur;
        }
    }
}
