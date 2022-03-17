using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    class Coffre_fort
    {
        private string name;
        private int diamonds;
        private Dictionary<string, Carte> decouvertes;

        //Contructeur
        public Coffre_fort(string leName)
        {
            this.name = leName;
            this.diamonds = 0;
            this.decouvertes = new Dictionary<string, Carte>();
        }

        //Getters
        public string GetName() { return this.name; }
        public int GetDiamonds() { return this.diamonds; }
        public Dictionary<string, Carte> GetDecouvertes() { return this.decouvertes; }

        //Setters
        public void SetDiamonds(int theDiamonds) { this.diamonds = theDiamonds; }
    }
}
