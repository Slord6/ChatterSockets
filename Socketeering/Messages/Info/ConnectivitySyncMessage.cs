using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    internal class ConnectivitySyncMessage : RefableMessage
    {
        public string Remote
        {
            get => GetControlArgSafe("REMOTE");
            set => SetControlArg("REMOTE", value);
        }

        public string Method
        {
            get => GetControlArgSafe("METHOD");
            set => SetControlArg("METHOD", value);
        }

        public bool Reachable
        {
            get => bool.Parse(GetControlArgSafe("REACHABLE"));
            set => SetControlArg("REACHABLE", value.ToString());
        }

        public ConnectivitySyncMessage(string rawMessage) : base(rawMessage)
        {
        }

        public ConnectivitySyncMessage(string source, string destination, string remote, string method, bool reachable, string? @ref)
            : base(source, destination, NodeControl.CONNECTIVITY_SYNC, @ref,
            new Dictionary<string, string>() { { "REMOTE", remote }, { "METHOD", method }, { "REACHABLE", reachable.ToString() } })
        {
        }
    }
}
