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
        private bool exit;
        private Coffre_fort chest;

        private int diamands { set; get; }

        Joueur(String nom)
        {
            this.nom = nom;
            diamands = 0;
            this.chest = new Coffre_fort();
        }
        void AddDiamond(int val)
        {
            diamands += val;
        }

        public bool Sortir()
        {
            //Quand le joueur sort il garde ses diamands dans son coffre
            if (this.exit)
            {
                chest.SetDiamonds(this.diamands);
                return true;
            }
                

            return false;
        }
    }
}
