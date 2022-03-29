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
            foreach (var p in laGrotte.Joueurgrotte)
            {
                if (p.Key == joueur)
                    JoueurCamp.Add(joueur, p.Value);
            }
            laGrotte.Joueurgrotte.Remove(joueur);
            return joueur;
        }
    }
}
