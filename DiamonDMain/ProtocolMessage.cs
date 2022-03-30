using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class ProtocolMessage
    {
        public int version { get; set; }   
        
        public ProtocolMessage() { }
        public ProtocolMessage(int laVersion) { this.version = laVersion; }
    }
    
    public class ProtocolMessageClient : ProtocolMessage
    {
        public string encryption { get; set; }
        public string compression { get; set; }

        public ProtocolMessageClient() : base() { }
        public ProtocolMessageClient(int LaVersion) : base(LaVersion) { }
        public ProtocolMessageClient(int laVersion, string leEncryption, string laCompression) : base(laVersion)
        {
            this.encryption = leEncryption;
            this.compression = laCompression;
        }
    }

    public class ProtocolMessageServer : ProtocolMessage
    {
        public List<string> encryption { get; set; }
        public List<string> compression { get; set; }

        public ProtocolMessageServer() : base() { }
        public ProtocolMessageServer(int LaVersion) : base(LaVersion) {  }

        //public ProtocolMessageServer(int laVersion, List<string> LeEncryption, List<string> LaCompression) : base(laVersion)
        //{
        //    this.encryption = LeEncryption;
        //    this.compression = LaCompression;
        //    //Pour version suivante
        //    //this.encryption = new List<string> { "aes-128", "aes-256", "rsa-1024", "rsa-2048"};
        //    //this.compression = new List<string> { "gzip", "deflate", "br" };
        //}
    }
}
