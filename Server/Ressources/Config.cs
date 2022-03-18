using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Ressources
{
    public class Config
    {
        private string name;
        private List<Server> configurations;
        private List<DiamonDMain.Partie> games;

        public string getnameconf() { return this.name; }
        public List<Server> getconfgurationconf() { return this.configurations; }
        public List<DiamonDMain.Partie> getgamesconf() { return this.games; }

    }
}
