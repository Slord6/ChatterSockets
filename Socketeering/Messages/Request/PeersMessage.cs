using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class PeersMessage : Message
    {

        public PeersMessage(string rawMessage) : base(rawMessage)
        {
        }

        public PeersMessage(string source, string destination) : base(source, destination, NodeControl.PEERS, null)
        {
        }
    }
}
