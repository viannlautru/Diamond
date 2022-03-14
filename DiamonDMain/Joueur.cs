using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    class Joueur
    {
        private string nom;
        private int diamands { set; get; }
        Joueur(String nom)
        {
            this.nom = nom;
            diamands = 0;
        }
    }
}
