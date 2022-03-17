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
        public Danger() : base("Danger"){

        }
        public String Piege()
        {
            return "Boum";
        }
    }
    public class Trophee : Carte
    {
        public Trophee() : base("Trophee")
        {

        }

    }
    public class Tresor : Carte
    {
        private int montant = 0;
        public Tresor() : base("Tresor")
        {
            var rand = new Random();
            montant = rand.Next(1000);
        }
        public int Partager()
        {
            return (montant - montant%GetInstances())/GetInstances();
        }
    }
}
