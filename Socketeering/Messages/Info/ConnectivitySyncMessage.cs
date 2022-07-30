using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    internal class ConnectivitySyncMessage : Message
    {
        public ConnectivitySyncMessage(string rawMessage) : base(rawMessage)
        {
        }

        public ConnectivitySyncMessage(string source, string destination, string remote, string method, bool reachable) : base(source, destination, NodeControl.CONNECTIVITY_SYNC,
            new Dictionary<string, string>() { { "REMOTE", remote }, { "METHOD", method }, { "REACHABLE", reachable.ToString() } })
        {
        }
    }
}
