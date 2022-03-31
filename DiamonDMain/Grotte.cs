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
        public Dictionary<string, Joueur> Joueurgrotte;
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
        public Dictionary<string, Joueur> getjoueurGrotte()
        {
            return Joueurgrotte;
        }
        public void Initjoueurs(Dictionary<string, Joueur> Joueur)
        {
            this.Joueurgrotte = Joueur;
        }
        public string addGrotteUtilisateur(string joueur)
        {
            if (leCamp.JoueurCamp.ContainsKey(joueur))
            {
                Joueurgrotte.Add(joueur, leCamp.JoueurCamp[joueur]);
            }
            leCamp.JoueurCamp.Remove(joueur);
            return joueur;
        }

    }
}
