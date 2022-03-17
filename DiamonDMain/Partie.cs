using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class Partie
    {
        private string id;
        private int max_players;
        private int caves;
        private int cards_quantity;
        private List<DiamonDMain.Danger> traps;
        private int type;   //Standard pour l'instant type = 0 partie rapide : type = 1
        //private Dictionary<int, Grotte> grottes;

        //Partie rapide => 2 grottes
        public Partie(int type = 0)
        {
            this.type = type;
            if (type == 0)
            {
                for (int i = 1; i <= 5; i++)
                {                    
                    //grottes.Add(i, new Grotte());
                }
                    
            }


        }
    }
}
