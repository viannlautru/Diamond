using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Joueur
    {
        public string name { get; set; }
        public string id { get; set; }
        public bool exit { set; get; }
        public Coffre_fort chest { set; get; }
        public Carte deck { set; get; }
        public int main { set; get; }
        public int diamands { set; get; }
        public Joueur() { }
        public Joueur(string nom, string id)
        {
            this.name = nom;
            this.id = id;
            diamands = 0;
            this.chest = new Coffre_fort();
        }

        void AddDiamond(int val)
        {
            diamands += val;
        }
        public void Sortir(String rep)    
        {
            //Quand le joueur sort il garde ses diamands dans son coffre
            if (rep == "Oui")
            {
                exit = true;
                chest.AddDiamonds(this.diamands);
                diamands = 0;
            }
        }
    }
}
