using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Ressources
{
    class Config
    {
        private string address;
        private int port;
        private string password;
        private string name;

        //Constructeur
        public Config(string lAddress, int lePort, string lePassword, string leName)
        {
            this.address = lAddress;
            this.port = lePort;
            this.password = lePassword;
            this.name = leName;
        }

        //Getters
        public string GetAddress() { return this.address; }
        public int GetPort() { return this.port; }
        public string GetPassword() { return this.password; }
        public string GetName() { return this.name; }
    }
}
