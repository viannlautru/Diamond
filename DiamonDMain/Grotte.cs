using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    class Grotte
    {
        private bool Etat;
        private Dictionary<int, Joueur> Joueurgrotte;

        public Grotte (Dictionary<int, Joueur> Joueur)
        {
            this.Etat = true;
            this.Joueurgrotte = Joueur;
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
        
        public int removeUtilisateur(int joueur)
        {
            Joueurgrotte.Remove(joueur);
            return joueur;
        }
    }
}
