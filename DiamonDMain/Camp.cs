using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Camp
    {
        public Dictionary<int, Joueur> JoueurCamp;
        public Grotte laGrotte;

        public Dictionary<int, Joueur> getjoueurCamp()
        {
            return JoueurCamp;
        }
       
        public int addCampUtilisateur(int joueur)
        {
            if (laGrotte.Joueurgrotte.ContainsKey(joueur))
            {
                JoueurCamp.Add(joueur, laGrotte.Joueurgrotte[joueur]);
            }
            laGrotte.Joueurgrotte.Remove(joueur);
            return joueur;
        }
        public Grotte GetGrotte() { return laGrotte; }
    }
}
