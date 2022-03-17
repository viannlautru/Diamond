using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamonDMain
{
    class ProtocolMessage
    {
        private int version;
    }

    class ProtocolMessageClient : ProtocolMessage
    {

    }

    class ProtocolMessageServer : ProtocolMessage
    {
        private List<string> encryption;
        private List<string> compression;
    }
}
