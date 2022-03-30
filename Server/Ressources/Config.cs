using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Ressources
{
    public class Config
    {
        public string name { get; set; }
        public List<Server> configurations { get; set; }
        public List<DiamonDMain.Partie> games { get; set; }

        public Config() { }
        public Config(string name, List<Server> conf)
        {
            this.name = name;
            configurations = conf;
        }
    }
}
