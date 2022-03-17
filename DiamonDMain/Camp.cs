using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    class Camp
    {
        private Dictionary<int, Joueur> JoueurCamp;

        public int addCampUtilisateur(int joueur)
        {
            JoueurCamp.Remove(joueur);
            return joueur;
        }
    }
}
