using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Info
{
    internal class DisconnectingMessage : RefableMessage
    {

        public string? WillWait
        {
            get => GetControlArg("WILLWAIT");
            set => SetControlArg("WILLWAIT", value);
        }

        public DisconnectingMessage(string rawMessage) : base(rawMessage)
        {
        }

        public DisconnectingMessage(string source, string destination, string? @ref, TimeSpan? willWait) : base(source, destination, NodeControl.DISCONNECTING, @ref,
            willWait != null ? new Dictionary<string, string> { { "WILLWAIT", willWait.ToString() } } : new Dictionary<string, string>())
        {
        }
    }
}
