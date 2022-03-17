using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    public class ProtocolMessage
    {
        private int version;

        public ProtocolMessage(int laVersion) { this.version = laVersion; }
        public int GetVersion() { return this.version; }
        public void SetVersion(int laVersion) { this.version = laVersion; }
    }

    public class ProtocolMessageClient : ProtocolMessage
    {
        private string encryption;
        private string compression;

        public ProtocolMessageClient(int laVersion, string leEncryption, string laCompression) : base(laVersion)
        {
            this.encryption = leEncryption;
            this.compression = laCompression;
        }

        public string GetEncryption() { return this.encryption; }
        public string GetCompression() { return this.compression; }
    }

    public class ProtocolMessageServer : ProtocolMessage
    {
        private List<string> encryption;
        private List<string> compression;

        public ProtocolMessageServer(int laVersion) : base(laVersion)
        {
            this.encryption = new List<string>();
            this.compression = new List<string>();
            //Pour version suivante
            //this.encryption = new List<string> { "aes-128", "aes-256", "rsa-1024", "rsa-2048"};
            //this.compression = new List<string> { "gzip", "deflate", "br" };
        }

        public List<string> GetEncryption() { return this.encryption; }
        public List<string> GetCompression() { return this.compression; }
    }
}
