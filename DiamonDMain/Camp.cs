using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Camp
    {
        public Dictionary<string, Joueur> JoueurCamp;
        public Grotte laGrotte;

        public Dictionary<string, Joueur> getjoueurCamp()
        {
            return JoueurCamp;
        }
       
        public string addCampUtilisateur(string joueur)
        {
            if (laGrotte.Joueurgrotte.ContainsKey(joueur))
            {
                if(laGrotte.Joueurgrotte[joueur].exit)
                    JoueurCamp.Add(joueur, laGrotte.Joueurgrotte[joueur]);
            }
            laGrotte.Joueurgrotte.Remove(joueur);
            return joueur;
        }
        public Grotte GetGrotte() { return laGrotte; }
    }
}
