using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    class Partie
    {
        private string name;
        private int type;   //Standard pour l'instant type = 0 partie rapide : type = 1
        private Dictionary<int, Grotte>;

        public Partie(int type = 0)
        {
            this.type = type;

        }
    }
}
