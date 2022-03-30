using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;

namespace ClientTest
{
    public class ClientParser
    {
        public byte[] address { get; set; }
        public int port { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        
    }
}
