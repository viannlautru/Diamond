using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Carte
    {
        private string type { get; set; }
        private static int instances = 0;

        public Carte(string type)
        {
            this.type = type;
            SetActiveInstances(this.GetType());
        }

        protected void SetActiveInstances(Type t)
        {
            instances++;
        }
        public int GetInstances(){
            return instances;
         }

    }
    public class Danger : Carte
    {
        private string name;
        private int quantity;
        public Danger(String name = "") : base("Danger"){
            this.name = name;
        }
        public String GetPiege()
        {
            return this.name;
        }
    }
    public class Trophee : Carte
    {
        private int montantTrophe = 0;
        public Trophee() : base("Trophee")
        {
            var rand = new Random();
            montantTrophe = rand.Next(1000);
        }
        public Trophee GetTrophee()
        {
            return this;
        }
    }
    public class Tresor : Carte
    {
        private int montantTresor = 0;
        public Tresor() : base("Tresor")
        {
            var rand = new Random();
            montantTresor = rand.Next(1000);
        }
        public int Partager()
        {
            return (montantTresor - montantTresor% GetInstances())/GetInstances();
        }
    }
}
