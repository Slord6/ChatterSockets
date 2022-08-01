using Socketeering.Messages.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Request
{
    internal class ConnectivityMessage : Message
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

        public ConnectivityMessage(string rawMessage) : base(rawMessage)
        {
        }

        public ConnectivityMessage(string source, string destination, string remote, string method) : base(source, destination, NodeControl.CONNECTIVITY,
            new Dictionary<string, string>() { { "REMOTE", remote }, { "METHOD", method } })
        {
        }
    }
}
