using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    internal class PeerSyncMessage : RefableMessage
    {
        public List<string> Peers
        {
            get => GetControlArgSafe("PEERS").Split(" ").ToList();
            set => SetControlArg("REMOTE", string.Join(" ", value));
        }

        public PeerSyncMessage(string rawMessage) : base(rawMessage)
        {
        }

        public PeerSyncMessage(string source, string destination, List<string> peers, string? @ref)
            : base(source, destination, NodeControl.PEER_SYNC, @ref,
            new Dictionary<string, string>() { { "PEERS", string.Join(" ", peers)} })
        {
        }
    }
}
