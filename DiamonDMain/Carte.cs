using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Carte
    {
        public string type { get; set; }
        private static int instances = 0;
        public Dictionary<int, Carte> CarteJouer;
        public Carte(string type)
        {
            this.type = type;
            SetActiveInstances(this.GetType());
        }

        protected void SetActiveInstances(Type t)
        {
            instances++;
        }
        public int GetInstances()
        {
            return instances;
        }

        public Dictionary<int, Carte> getCartejouer()
        {
            return CarteJouer;
        }

    }
    public class Danger : Carte
    {
        public string name { get; set; }
        public int quantity { get; set; }

        public Danger(String name = "") : base("Danger")
        {
            this.name = name;
        }
        public String GetPiege()
        {
            return this.name;
        }
    }
    public class Trophee : Carte
    {
        public Trophee() : base("Trophee")
        {

        }
        public Trophee GetTrophee()
        {
            return this;
        }
    }
    public class Tresor : Carte
    {
        public int montantTresor { get; set; }
        public Tresor() : base("Tresor")
        {
            var rand = new Random();
            montantTresor = rand.Next(10);
        }
        public int GetMontant() { return montantTresor; }
        public int Partager(int nbJoueurs)
        {
            int mont = (montantTresor - montantTresor % nbJoueurs) / nbJoueurs;
            montantTresor -= mont;
            return mont;
        }
    }
}
