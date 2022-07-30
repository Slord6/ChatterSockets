using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class ConnectivityMessage : Message
    {
        public ConnectivityMessage(string rawMessage) : base(rawMessage)
        {
        }

        public ConnectivityMessage(string source, string destination, string remote, string method) : base(source, destination, NodeControl.CLOSE,
            new Dictionary<string, string>() { { "REMOTE", remote }, { "METHOD", method } })
        {
        }
    }
}
